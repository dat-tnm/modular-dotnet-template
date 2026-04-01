using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.ProjectName.Modules.Auth.Extensions
{
    public class UacLoginOptions
    {
        public const string SectionName = "UAC.Login";
        public string ConnectionString { get; set; } = string.Empty;
    }
}
