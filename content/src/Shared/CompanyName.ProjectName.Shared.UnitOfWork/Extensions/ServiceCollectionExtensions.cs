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

        public static IServiceCollection AddWPFDapperUnitOfWork(this IServiceCollection services, IConfiguration configuration)
        {
            // In WPF applications, it's common to use transient services for database connections to ensure that each operation gets a new connection instance.
            services.Configure<DbOptions>(configuration.GetSection("DbOptions"));

            services.AddTransient<IDbConnectionFactory, SqlConnectionFactory>();
            services.AddTransient<IUnitOfWork, DapperUnitOfWork>();

            return services;
        }
    }
}
