using CompanyName.ProjectName.Modules.Auth.Contracts;
using CompanyName.ProjectName.Modules.Auth.Extensions;
using CompanyName.ProjectName.Shared.UnitOfWork.Contracts;
using Dapper;
using Microsoft.Extensions.Options;
using System.Data;

namespace CompanyName.ProjectName.Modules.Auth.Implements
{
    public class UACLoginService : IUACLoginService
    {
        private string _currentUsername = string.Empty;
        private readonly IDbConnection _dbConnection;

        public UACLoginService(IDbConnectionFactory dbConnectionFactory, IOptions<UacLoginOptions> options)
        {
            _dbConnection = dbConnectionFactory.CreateConnection(options.Value.ConnectionString);
        }

        public string GetCurrentUsername()
        {
            return _currentUsername;
        }

        public void SetCurrentUsername(string username)
        {
            _currentUsername = username;
        }

        public async Task<IEnumerable<string>> GetRoles(string? keyword = null)
        {
            if (_currentUsername == string.Empty)
            {
                return new List<string>();
            }

            var sql = @"
SELECT r.RoleName
FROM [UAC_Login].[dbo].[UsersInRole] ur
INNER JOIN [UAC_Login].[dbo].[Roles] r ON ur.RoleId = r.Id
WHERE ur.UserId = @Username";

            if (keyword != null)
            {
                sql += " AND r.RoleName LIKE @Keyword";
            }

            var roles = _dbConnection.Query<string>(sql, new { Username = _currentUsername, Keyword = $"%{keyword}%" });
            return roles;
        }

        public async Task<bool> IsInRole(string roleName)
        {
            if (_currentUsername == string.Empty)
            {
                return false;
            }

            var sql = @"
SELECT COUNT(1)
FROM [UAC_Login].[dbo].[UsersInRole] ur
INNER JOIN [UAC_Login].[dbo].[Roles] r ON ur.RoleId = r.Id
WHERE ur.UserId = @Username AND r.RoleName = @RoleName";
            var count = _dbConnection.ExecuteScalar<int>(sql, new { Username = _currentUsername, RoleName = roleName });
            return count > 0;
        }
    }
}
