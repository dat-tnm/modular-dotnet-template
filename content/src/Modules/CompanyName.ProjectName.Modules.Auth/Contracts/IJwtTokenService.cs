using System.Security.Claims;

namespace CompanyName.ProjectName.Modules.Auth.Contracts
{
    public interface IJwtTokenService
    {
        string CreateToken(IEnumerable<Claim> claims);
    }
}
