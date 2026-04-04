using CompanyName.ProjectName.Modules.Auth.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.ProjectName.Modules.Auth.Implements
{
    public class MockUacLoginService : IUACLoginService
    {
        public string GetCurrentUsername()
        {
            return "mock_user";
        }

        public Task<IEnumerable<string>> GetRoles(string? keyword = null)
        {
            return Task.FromResult<IEnumerable<string>>(new List<string> { "Admin", "User" });
        }

        public Task<bool> IsInRole(string roleName)
        {
            return Task.FromResult(true);
        }

        public void SetCurrentUsername(string username)
        {
            return;
        }
    }
}
