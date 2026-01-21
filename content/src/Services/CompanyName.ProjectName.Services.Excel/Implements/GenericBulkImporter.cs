using CompanyName.ProjectName.Services.Excel.Contracts;
using CompanyName.ProjectName.Services.Excel.Contracts.DTOs;
using CompanyName.ProjectName.Shared.UnitOfWork.Contracts;
using Dapper;
using ExcelDataReader;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CompanyName.ProjectName.Services.Excel.Implements
{
    internal sealed class GenericBulkImporter : IGenericBulkImporter
    {
        private readonly IUnitOfWork _uow;

        public GenericBulkImporter(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ImportResult> ImportAsync(ImportDescriptor desc, Stream fileStream, CancellationToken ct)
        {
            ValidateDescriptor(desc);

            var (data, errors, totalRows) = await ReadExcelIntoDataTableWithErrorsAsync(desc, fileStream, ct);

            if (totalRows == 0 || (errors != null && errors.Count > 0))
                return new ImportResult(totalRows, 0, 0, errors);

            var createTempSql = BuildCreateTempTableSql(desc);
            var mergeSql = BuildMergeSql(desc);

            await _uow.BeginAsync(ct);
            try
            {
                await _uow.Connection.ExecuteAsync(createTempSql, transaction: _uow.Transaction);
                await BulkCopyAsync(
                    (_uow.Connection as SqlConnection)!,
                    _uow.Transaction as SqlTransaction,
                    data,
                    "#Import",
                    ct
                );

                var (inserted, updated) = await _uow.Connection.QuerySingleAsync<(int Inserted, int Updated)>(mergeSql, transaction: _uow.Transaction);

                //var parameters = new DynamicParameters();
                //parameters.Add("@Inserted", dbType: DbType.Int32, direction: ParameterDirection.Output);
                //parameters.Add("@Updated", dbType: DbType.Int32, direction: ParameterDirection.Output);

                //await _uow.Connection.ExecuteAsync(mergeSql, parameters, transaction: _uow.Transaction);

                //var inserted = parameters.Get<int>("@Inserted");
                //var updated = parameters.Get<int>("@Updated");

                await _uow.CommitAsync();
                return new ImportResult(totalRows, inserted, updated, errors);
            }
            catch (Exception)
            {
                await _uow.RollbackAsync(ct);
                throw;
            }
        }


        private void ValidateDescriptor(ImportDescriptor d)
        {
            if (string.IsNullOrWhiteSpace(d.Schema) || string.IsNullOrWhiteSpace(d.TableName))
                throw new ArgumentException("Schema/Table must be specified.");

            if (string.IsNullOrWhiteSpace(d.SelectedSheet))
                throw new ArgumentException("SelectedSheet must be specified.");

            if (d.KeyColumns is null || d.KeyColumns.Count == 0)
                throw new ArgumentException("At least one key column is required.");

            if (d.MappingColumns is null || d.MappingColumns.Count == 0)
                throw new ArgumentException("At least one mapping column is required.");

            if (!IsSafeIdentifier(d.Schema) || !IsSafeIdentifier(d.TableName))
                throw new ArgumentException("Unsafe schema/table identifier.");

            var targetCols = d.MappingColumns.Select(m => m.TargetColumn).ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var key in d.KeyColumns)
            {
                if (!IsSafeIdentifier(key)) throw new ArgumentException($"Unsafe key identifier: {key}");

                if (!targetCols.Contains(key)) throw new ArgumentException($"Key column '{key}' must be included in MappingColumns.");
            }

            foreach (var col in targetCols)
            {
                if (!IsSafeIdentifier(col)) throw new ArgumentException($"Unsafe identifer: {col}");
            }

        }

        private bool IsSafeIdentifier(string s)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(s, @"^[A-Za-z0-9_]+$");
        }

        private async Task<(DataTable, List<ImportError>, int)> ReadExcelIntoDataTableWithErrorsAsync(
            ImportDescriptor desc,
            Stream fileStream,
            CancellationToken ct
            )
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var reader = ExcelReaderFactory.CreateReader(fileStream);

            var (dt, errors, totalRows) = await ReadAllSheets(reader, desc, ct);
            await Task.CompletedTask;

            return (dt, errors, totalRows);
        }

        private async Task<(DataTable Table, List<ImportError> Errors, int TotalRows)> ReadAllSheets(
            IExcelDataReader reader,
            ImportDescriptor desc,
            CancellationToken ct
            )
        {
            DataTable data = new DataTable();
            var errors = new List<ImportError>();
            int totalRows = 0;

            var foundSheet = false;
            var sheetName = reader.Name.ToString() ?? "(Unnamed)";
            do
            {
                if (!string.Equals(sheetName, desc.SelectedSheet, StringComparison.OrdinalIgnoreCase))
                {
                    //consume this sheet to move to next
                    while (reader.Read()) { }
                    continue;
                }

                foundSheet = true;

                // Header row
                if (!reader.Read())
                    break;

                // headerIndex: example item is <headerName,cellIndex>
                var headerIndex = BuildHeaderIndex(reader);

                if (desc.Options?.RequireHeaders ?? true)
                {
                    // check if headerIndex contains all MappingColumns 
                    foreach (var m in desc.MappingColumns)
                    {
                        if (!headerIndex.ContainsKey(m.SourceColumn))
                            errors.Add(new ImportError(desc.SelectedSheet, RowNumber: 1, $"Header '{m.SourceColumn}' not found in Excel."));
                    }

                    if (errors.Count > 0)
                        break; // header invalid, abort reading data
                }

                (data, errors, totalRows) = await ReadSheet(reader, desc, headerIndex, 1, errors, ct);

                break; // finished selected sheet
            } while (reader.NextResult()); // move to next sheet

            if (!foundSheet)
            {
                errors.Add(new ImportError(desc.SelectedSheet, 0, $"Sheet '{desc.SelectedSheet}' not found."));
            }

            await Task.CompletedTask;
            return (data, errors, totalRows);
        }

        private async Task<(DataTable Table, List<ImportError> Errors, int TotalRows)> ReadSheet(
            IExcelDataReader reader,
            ImportDescriptor desc,
            IDictionary<string, int> headerIndex,
            int startRowNum,
            List<ImportError> errors,
            CancellationToken ct
            )
        {
            // init dt
            var dt = new DataTable();
            foreach (var m in desc.MappingColumns)
                dt.Columns.Add(m.TargetColumn, ClrType(m.DbType));

            // init keySet: example items are key1value|key2value|key3value, key1value2|key2value2|key3value2. Preventing duplicated composite keys
            var keySet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            int rowNum = startRowNum;
            int totalRows = 0;

            while (reader.Read())
            {
                rowNum++;
                totalRows++;


                // keyCells: example items are key1.cell, key2.cell, key3.cell. Ensuring all key values not empty.
                var keyCells = new List<string>(desc.KeyColumns.Count);

                var row = dt.NewRow();
                bool hasError = false;

                foreach (var m in desc.MappingColumns)
                {
                    var idx = headerIndex.TryGetValue(m.SourceColumn, out var i) ? i : -1;

                    object? raw = idx >= 0 ? reader.GetValue(idx) : null;

                    var (val, err) = ConvertAndValidateCell(raw, m);
                    if (err is not null)
                    {
                        errors.Add(new ImportError(desc.SelectedSheet, rowNum, $"{m.TargetColumn}: {err}"));
                        hasError = true;
                    }

                    row[m.TargetColumn] = val ?? DBNull.Value;

                    // keyCells: add key cell value
                    if (desc.KeyColumns.Contains(m.TargetColumn))
                        keyCells.Add(val?.ToString() ?? "");
                }

                if (keyCells.Count != desc.KeyColumns.Count || keyCells.Any(key => string.IsNullOrWhiteSpace(key)))
                {
                    errors.Add(new ImportError(desc.SelectedSheet, rowNum, $"Missing required key column(s): {string.Join(", ", desc.KeyColumns)}"));
                    hasError = true;
                }

                var compositeKey = string.Join("|", keyCells);
                if (!hasError)
                {
                    if (!keySet.Add(compositeKey))
                    {
                        errors.Add(new ImportError(desc.SelectedSheet, rowNum, $"Duplicate key in file: [{compositeKey}]"));
                        hasError = true;
                    }
                }

                if (!hasError)
                    dt.Rows.Add(row);

                if (ct.IsCancellationRequested)
                    break;
            }

            return (dt, errors, totalRows);
        }

        private IDictionary<string, int> BuildHeaderIndex(IExcelDataReader reader)
        {
            var headers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var h = reader.GetValue(i)?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(h))
                    headers[h] = i;
            }

            return headers;
        }

        private (object? Value, string? Error) ConvertAndValidateCell(object? raw, ColumnMapping m)
        {
            object? val = raw switch
            {
                null => null,
                double d when m.DbType == SqlDbType.Decimal => Convert.ToDecimal(d),
                double d when m.DbType == SqlDbType.Int => Convert.ToInt32(d),
                double d when m.DbType == SqlDbType.BigInt => Convert.ToInt64(d),
                bool b when m.DbType == SqlDbType.Bit => b,
                DateTime dt when m.DbType is SqlDbType.Date or SqlDbType.DateTime2 => dt,
                string s1 when m.DbType is SqlDbType.NVarChar or SqlDbType.VarChar => s1.Trim(),
                string s1 => s1.Trim(),
                _ => raw
            };

            // Required check
            if (m.Required && (val is null || (val is string sVal && string.IsNullOrWhiteSpace(sVal))))
                return (null, "Value is required.");

            // Max length check
            if (m.Length is int max && val is string s && s.Length > max)
                return (null, $"Exceeds max length ({max}).");

            if (val is string sNum && (m.DbType == SqlDbType.Decimal || m.DbType == SqlDbType.Int || m.DbType == SqlDbType.BigInt))
            {
                if (m.DbType == SqlDbType.Decimal)
                {
                    if (!decimal.TryParse(sNum, out var d))
                        return (null, "Invalid decimal.");

                    val = d;
                }
                else if (m.DbType == SqlDbType.Int)
                {
                    if (!int.TryParse(sNum, out var i))
                        return (null, "Invalid integer.");

                    val = i;
                }
                else if (m.DbType == SqlDbType.BigInt)
                {
                    if (!long.TryParse(sNum, out var l))
                        return (null, "Invalid long integer.");

                    val = l;
                }
            }

            if (m.Transform is not null)
                val = m.Transform(val);

            if (m.IsValid is not null && !m.IsValid(val))
                return (null, "Semantic validation failed.");

            return (val, null);
        }

        private string Quote(string identifier) => $"[{identifier}]";

        private string BuildCreateTempTableSql(ImportDescriptor d)
        {
            var sb = new StringBuilder("CREATE TABLE #Import (");
            for (int i = 0; i < d.MappingColumns.Count; i++)
            {
                var m = d.MappingColumns[i];
                sb.Append(Quote(m.TargetColumn)).Append(' ').Append(ToSqlType(m));
                if (i < d.MappingColumns.Count - 1) sb.Append(", ");
            }
            sb.Append(");");

            return sb.ToString();
        }

        private string ToSqlType(ColumnMapping m)
        {
            return m.DbType switch
            {
                SqlDbType.NVarChar => m.Length is int len ? $"NVARCHAR({len})" : "NVARCHAR(MAX)",
                SqlDbType.VarChar => m.Length is int len ? $"VARCHAR({len})" : "VARCHAR(MAX)",
                SqlDbType.Decimal => (m.Precision, m.Scale) is (byte p, byte s) ? $"DECIMAL({p},{s})" : "DECIMAL(18,2)",
                SqlDbType.Int => "INT",
                SqlDbType.BigInt => "BIGINT",
                SqlDbType.Bit => "BIT",
                SqlDbType.Date => "DATE",
                SqlDbType.DateTime2 => "DATETIME2",
                SqlDbType.UniqueIdentifier => "UNIQUEIDENTIFIER",
                _ => throw new NotSupportedException($"Unsupported SqlDbType: {m.DbType}")

            };
        }

        private async Task BulkCopyAsync(SqlConnection conn, SqlTransaction? tx, DataTable table, string dest, CancellationToken ct)
        {
            using var bulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepNulls, tx);
            bulk.DestinationTableName = dest;

            foreach (DataColumn c in table.Columns)
                bulk.ColumnMappings.Add(c.ColumnName, c.ColumnName);

            await bulk.WriteToServerAsync(table, ct);
        }

        private string BuildMergeSql(ImportDescriptor d)
        {
            var target = $"{Quote(d.Schema)}.{Quote(d.TableName)}";
            var keysJoin = string.Join(" AND ", d.KeyColumns.Select(k => $"T.{Quote(k)} = S.{Quote(k)}"));

            var updateSetCols = d.MappingColumns
                .Where(m => !d.KeyColumns.Contains(m.TargetColumn)) // don't update key columns
                .Select(m => $"T.{Quote(m.TargetColumn)} = S.{Quote(m.TargetColumn)}")
                .ToList();

            // ensures keys included: union MappingColumns and KeyColumns
            var insertCols = d.MappingColumns
                .Select(m => m.TargetColumn)
                .Union(d.KeyColumns)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var insertColsSql = string.Join(", ", insertCols.Select(c => Quote(c)));
            var insertValues = string.Join(", ", insertCols.Select(c => $"S.{Quote(c)}"));

            var sb = new StringBuilder();
            sb.AppendLine("DECLARE @actions TABLE (act NVARCHAR(10));");
            sb.AppendLine($"MERGE {target} AS T");
            sb.AppendLine("USING #Import AS S");
            sb.AppendLine($"ON {keysJoin}");

            if (d.Mode is UpsertMode.UpdateOnly or UpsertMode.Upsert)
            {
                if (updateSetCols.Count > 0)
                {
                    sb.AppendLine("WHEN MATCHED THEN");
                    sb.AppendLine("  UPDATE SET " + string.Join(", ", updateSetCols));
                }
            }

            if (d.Mode is UpsertMode.InsertOnly or UpsertMode.Upsert)
            {
                sb.AppendLine("WHEN NOT MATCHED BY TARGET THEN");
                sb.AppendLine($"  INSERT ({insertColsSql})");
                sb.AppendLine($"  VALUES ({insertValues})");
            }

            sb.AppendLine("OUTPUT $action INTO @actions;");
            sb.AppendLine("DROP TABLE #Import;");
            sb.AppendLine("SELECT");
            sb.AppendLine("  (SELECT COUNT(*) FROM @actions WHERE act = 'INSERT') AS Inserted,");
            sb.AppendLine("  (SELECT COUNT(*) FROM @actions WHERE act = 'UPDATE') AS Updated;");

            return sb.ToString();
        }

        //private async Task<DataTable> ReadExcelIntoDataTableAsync(ImportDescriptor desc, Stream fileStream, CancellationToken ct)
        //{
        //    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        //    using var reader = ExcelReaderFactory.CreateReader(fileStream);
        //    var mapIndex = BuildHeaderIndex(reader, desc);

        //    var dt = new DataTable();
        //    foreach (var m in desc.MappingColumns)
        //        dt.Columns.Add(m.TargetColumn, ClrType(m.DbType));

        //    int rowNumber = 0;
        //    do
        //    {
        //        while (reader.Read())
        //        {
        //            rowNumber++;
        //            if (rowNumber == 1) continue;

        //            var row = dt.NewRow();
        //            foreach (var m in desc.MappingColumns)
        //            {
        //                var idx = mapIndex[m.SourceHeader];
        //                var raw = reader.GetValue(idx);
        //                var value = ConvertCell(raw, m);
        //                row[m.TargetColumn] = value ?? DBNull.Value;
        //            }
        //            dt.Rows.Add(row);
        //            if (ct.IsCancellationRequested) break;
        //        }
        //    } while (reader.NextResult());

        //    await Task.CompletedTask;
        //    return dt;
        //}

        //private IDictionary<string, int> BuildHeaderIndex(IExcelDataReader reader, ImportDescriptor desc)
        //{
        //    if (!reader.Read()) throw new InvalidOperationException("Empty Excel file.");
        //    var headers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        //    for (int i = 0; i < reader.FieldCount; i++)
        //    {
        //        var h = reader.GetValue(i)?.ToString()?.Trim();
        //        if (!string.IsNullOrEmpty(h))
        //            headers[h] = i;
        //    }

        //    foreach (var m in desc.MappingColumns)
        //    {
        //        if (!headers.ContainsKey(m.SourceHeader))
        //            throw new ArgumentException($"Header '{m.SourceHeader}' not found in Excel.");
        //    }

        //    return headers;
        //}

        //private object? ConvertCell(object? raw, ColumnMapping m)
        //{
        //    object? val = raw switch
        //    {
        //        null => null,
        //        double d when m.DbType == SqlDbType.Decimal => Convert.ToDecimal(d),
        //        double d when m.DbType == SqlDbType.Int => Convert.ToInt32(d),
        //        double d when m.DbType == SqlDbType.BigInt => Convert.ToInt64(d),
        //        bool b when m.DbType == SqlDbType.Bit => b,
        //        DateTime dt when m.DbType is SqlDbType.Date or SqlDbType.DateTime2 => dt,
        //        string s => s.Trim(),
        //        _ => raw
        //    };

        //    if (m.Transform is not null)
        //        val = m.Transform(val);

        //    return val;
        //}

        private Type ClrType(SqlDbType dbType) => dbType switch
        {
            SqlDbType.NVarChar or SqlDbType.VarChar => typeof(string),
            SqlDbType.Decimal => typeof(decimal),
            SqlDbType.Int => typeof(int),
            SqlDbType.BigInt => typeof(long),
            SqlDbType.Bit => typeof(bool),
            SqlDbType.Date => typeof(DateTime),
            SqlDbType.DateTime2 => typeof(DateTime),
            SqlDbType.UniqueIdentifier => typeof(Guid),
            _ => typeof(string)
        };

        // Optional Enhancements:
        // Chunking for very large files: read N rows → bulk copy → MERGE; repeat(ensure temp table persists within the transaction).
        // Audit: insert imported rows into a staging table alongside a batch ID and log counts/errors(helpful for ops).
        // Conflict policy: Instead of rejecting input duplicates, you could dedupe keeping last; change duplicate detection accordingly.
        // Multi-sheet support: loop over sheets and accumulate; keep row numbers contextual(e.g., Sheet1:Row7 in error messages).
    }
}
