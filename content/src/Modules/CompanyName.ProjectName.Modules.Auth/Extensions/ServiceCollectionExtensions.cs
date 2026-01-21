using CompanyName.ProjectName.Modules.Auth.Contracts;
using CompanyName.ProjectName.Modules.Auth.Implements;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.ProjectName.Modules.Auth.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAuthModule(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOpts = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOpts.SecretKey));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOpts.Issuer,
                        ValidAudience = jwtOpts.Audience,
                        IssuerSigningKey = key
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.TryGetValue(jwtOpts.CookieName, out var token))
                            {
                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization();
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.AddScoped<IJwtTokenService, JwtTokenService>();
        }
    }
}
