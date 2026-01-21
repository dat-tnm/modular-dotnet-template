using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.ProjectName.Services.Excel.Contracts.DTOs
{
    public sealed record UploadResult(
        string FileId,
        string FileName,
        long FileSize,
        List<string> Sheets,
        DateTime ExpiresAt);
}
