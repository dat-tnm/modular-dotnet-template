using ClosedXML.Excel;
using CompanyName.ProjectName.Services.Excel.Contracts;
using CompanyName.ProjectName.Services.Excel.Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CompanyName.ProjectName.Services.Excel.Implements
{
    internal sealed class ExcelService : IExcelService
    {
        public ImportTemplateFile GenerateImportTemplateFile(ImportDescriptor desc)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(desc.SelectedSheet ?? "Sheet1");

            for (int i = 0; i < desc.MappingColumns.Count; i++)
            {
                var mapping = desc.MappingColumns[i];
                worksheet.Cell(1, i + 1).Value = mapping.SourceColumn;
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"{desc.EntityName}_import_template.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileBytes = stream.ToArray();

            return new ImportTemplateFile(fileName, contentType, fileBytes);
        }

        public List<string> GetSheetNames(string filePath)
        {
            using var workbook = new XLWorkbook(filePath);
            return workbook.Worksheets.Select(ws => ws.Name).ToList();
        }

        public PreviewResult GetPreviewData(string filePath, string sheetName, int maxRows)
        {
            try
            {
                using var workbook = new XLWorkbook(filePath);
                var worksheet = workbook.Worksheet(sheetName);

                if (worksheet == null)
                {
                    throw new ArgumentException($"Sheet '{sheetName}' does not exist in the Excel file.");
                }

                var headers = new List<string>();
                var rows = new List<List<object>>();

                var wHeaderRow = worksheet.Row(1);
                foreach (var cell in wHeaderRow.CellsUsed())
                {
                    headers.Add(cell.GetString());
                }

                var rowCount = Math.Min(worksheet.RowsUsed().Count(), maxRows + 1);
                for (int i = 2; i <= rowCount; i++)
                {
                    var row = new List<object>();
                    var wRow = worksheet.Row(i);
                    foreach (var cell in wRow.Cells(1, headers.Count))
                    {
                        row.Add(cell.GetString() ?? string.Empty);
                    }
                    rows.Add(row);
                }

                return new PreviewResult(headers, rows, worksheet.RowsUsed().Count() - 1, sheetName);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to read the file.", e);
            }
        }
    }
}
