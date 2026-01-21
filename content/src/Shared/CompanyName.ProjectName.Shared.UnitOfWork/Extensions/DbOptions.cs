using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.ProjectName.Shared.UnitOfWork.Extensions
{
    public class DbOptions
    {
        public const string SectionName = "Database";

        public string ConnectionString { get; set; } = string.Empty;

        public int CommandTimeout { get; set; } = 30;

        public bool EnableRetry { get; set; } = true;

        public int MaxRetryAttempts { get; set; } = 3;
    }
}
