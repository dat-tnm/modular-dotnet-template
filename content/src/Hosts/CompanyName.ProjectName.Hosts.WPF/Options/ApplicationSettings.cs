using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.ProjectName.Hosts.WPF.Options
{
    public class ApplicationSettings
    {
        public const string SectionName = "ApplicationSettings";

        public string ApplicationName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
    }
}
