using CompanyName.ProjectName.Shared.UnitOfWork.Contracts;
using CompanyName.ProjectName.Shared.UnitOfWork.Implements;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.ProjectName.Shared.UnitOfWork.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDapperUnitOfWork(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DbOptions>(configuration.GetSection("DbOptions"));

            services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
            services.AddScoped<IUnitOfWork, DapperUnitOfWork>();

            return services;
        }
    }
}
