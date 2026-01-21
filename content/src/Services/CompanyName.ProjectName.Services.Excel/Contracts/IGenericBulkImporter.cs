using CompanyName.ProjectName.Services.Excel.Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.ProjectName.Services.Excel.Contracts
{
    public interface IGenericBulkImporter
    {
        Task<ImportResult> ImportAsync(ImportDescriptor descriptor, Stream fileStream, CancellationToken ct);
    }
}
