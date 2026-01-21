using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CompanyName.ProjectName.Shared.UnitOfWork.Contracts
{
    public interface IDbConnectionFactory
    {
        public IDbConnection CreateConnection();

        string ConnectionString { get; }
    }
}
