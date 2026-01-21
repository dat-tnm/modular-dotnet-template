using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.ProjectName.Services.Excel.Contracts.DTOs
{
    public sealed record ImportResult(
        int TotalRows,
        int Inserted, int Updated,
        IReadOnlyList<ImportError> Errors);

    public sealed record ImportError(
        string SheetName,
        int RowNumber,
        string Message);
}
