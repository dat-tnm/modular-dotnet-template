using CompanyName.ProjectName.Services.Excel.Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.ProjectName.Services.Excel.Contracts
{
    public interface IExcelService
    {
        ImportTemplateFile GenerateImportTemplateFile(ImportDescriptor descriptor);
        List<string> GetSheetNames(string filePath);
        PreviewResult GetPreviewData(string filePath, string sheetName, int maxRows);
    }
}
