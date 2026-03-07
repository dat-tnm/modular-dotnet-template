using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CompanyName.ProjectName.Hosts.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost WpfHost { get; } = CreateHostBuilder().Build();

        private static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                // Ensure base path is the folder of the executable
                var basePath = AppContext.BaseDirectory;
                config.SetBasePath(basePath);

                // appsettings.json alongside the exe
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                // Optional environment-specific file
                var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                            ?? "Production";
                config.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);

                // Environment variables / user secrets as needed
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                // Register configuration as singleton
                services.AddSingleton<IConfiguration>(configuration);

                // Register UnitOfWork (required for database access)
                services.AddWPFDapperUnitOfWork(configuration);

                // Register ViewModels

                // Register Windows
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
            });


        protected override async void OnStartup(StartupEventArgs e)
        {
            await WpfHost.StartAsync();

            // Example code: Show CustomWindow as the main window
            // var customWindow = WpfHost.Services.GetRequiredService<CustomWindow>();
            // customWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (WpfHost)
            {
                await WpfHost.StopAsync();
            }

            base.OnExit(e);
        }

        // Helper property to access configuration from anywhere in the app
        public static IConfiguration Configuration =>
            App.WpfHost.Services.GetRequiredService<IConfiguration>();
    }

}
