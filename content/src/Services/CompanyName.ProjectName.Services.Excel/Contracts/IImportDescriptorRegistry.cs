using CompanyName.ProjectName.Services.Excel.Contracts.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.ProjectName.Services.Excel.Contracts
{
    public interface IImportDescriptorRegistry
    {
        ImportDescriptor? Resolve(string entityName);
    }
}
