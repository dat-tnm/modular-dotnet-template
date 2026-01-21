using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CompanyName.ProjectName.Services.Excel.Contracts.DTOs
{
    public sealed record ImportDescriptor(
        string EntityName,
        string Schema,
        string TableName,
        IReadOnlyList<string> KeyColumns,
        IReadOnlyList<ColumnMapping> MappingColumns,
        UpsertMode Mode = UpsertMode.Upsert,
        string SelectedSheet = "Sheet1",
        ImportOptions? Options = null);

    public sealed record ColumnMapping(
        string SourceColumn,
        string TargetColumn,
        SqlDbType DbType,
        int? Length = null,
        byte? Scale = null,
        byte? Precision = null,
        bool Required = false,
        Func<object?, object?>? Transform = null,
        Func<object?, bool>? IsValid = null);

    public enum UpsertMode
    {
        InsertOnly,
        UpdateOnly,
        Upsert
    }

    public sealed record ImportOptions(
        bool RequireHeaders = true,
        int ChunkSize = 5000);
}
