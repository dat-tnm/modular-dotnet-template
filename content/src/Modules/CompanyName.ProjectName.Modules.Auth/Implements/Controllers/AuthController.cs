using CompanyName.ProjectName.Modules.Auth.Contracts;
using CompanyName.ProjectName.Modules.Auth.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace CompanyName.ProjectName.Modules.Auth.Implements.Controllers
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }


    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly JwtOptions _jwtOptions;

        public AuthController(IJwtTokenService jwtTokenService, IOptions<JwtOptions> jwtOptions)
        {
            _jwtTokenService = jwtTokenService;
            _jwtOptions = jwtOptions.Value;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Email != "user@example.com" || request.Password != "123456")
            {
                return Unauthorized();
            }

                var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-1"),
                new Claim(ClaimTypes.Name, "Demo user"),
                new Claim(ClaimTypes.Email, request.Email),
                new Claim(ClaimTypes.Role, "User")
            };

            var token = _jwtTokenService.CreateToken(claims);
            Response.Cookies.Append(_jwtOptions.CookieName, token, new Microsoft.AspNetCore.Http.CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes)
            });

            return Ok(new { message = "Logged in." });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(_jwtOptions.CookieName, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
            return Ok(new { message = "Logged out." });
        }
    }
}
