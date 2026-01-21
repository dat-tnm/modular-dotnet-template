using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.ProjectName.Services.Excel.Contracts.DTOs
{
    public sealed record PreviewResult(
        List<string> Headers,
        List<List<object>> Rows,
        int TotalRows,
        string SheetName);
}
