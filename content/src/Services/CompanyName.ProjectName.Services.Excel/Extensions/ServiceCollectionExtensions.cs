using CompanyName.ProjectName.Services.Excel.Contracts;
using CompanyName.ProjectName.Services.Excel.Implements;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyName.ProjectName.Services.Excel.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers Excel Import/Export services with custom registry
        /// </summary>
        public static IServiceCollection AddExcelImportExport<TRegistry>(this IServiceCollection services)
            where TRegistry : class, IImportDescriptorRegistry
        {
            services.AddScoped<IExcelService, ExcelService>();
            services.AddScoped<IGenericBulkImporter, GenericBulkImporter>();
            services.AddScoped<IImportDescriptorRegistry, TRegistry>();

            return services;
        }
    }
}
