using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.ProjectName.Modules.Auth.Extensions
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";
        public string CookieName { get; set; } = "auth_token";
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; } = 60;

    }
}
