using CompanyName.ProjectName.Shared.UnitOfWork.Contracts;
using CompanyName.ProjectName.Shared.UnitOfWork.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace CompanyName.ProjectName.Shared.UnitOfWork.Implements
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IOptions<DbOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        public string ConnectionString => _connectionString;

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
