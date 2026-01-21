using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.ProjectName.Services.Excel.Contracts.DTOs
{
    public sealed record ImportTemplateFile(
        string FileName,
        string ContentType,
        byte[] FileBytes);
}
