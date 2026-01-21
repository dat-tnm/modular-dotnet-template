using CompanyName.ProjectName.Modules.Auth.Contracts;
using CompanyName.ProjectName.Modules.Auth.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CompanyName.ProjectName.Modules.Auth.Implements
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _options;
        private readonly SigningCredentials _credentials;

        public JwtTokenService(IOptions<JwtOptions> options)
        {
            _options = options.Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            _credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        public string CreateToken(IEnumerable<System.Security.Claims.Claim> claims)
        {
            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes),
                signingCredentials: _credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
