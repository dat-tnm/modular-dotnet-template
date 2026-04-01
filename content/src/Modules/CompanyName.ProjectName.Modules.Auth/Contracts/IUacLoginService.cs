using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.ProjectName.Modules.Auth.Contracts
{
    public interface IUACLoginService
    {
        string GetCurrentUsername();
        void SetCurrentUsername(string username);
        Task<bool> IsInRole(string roleName);
        Task<IEnumerable<string>> GetRoles(string? keyword = null);
    }
}
