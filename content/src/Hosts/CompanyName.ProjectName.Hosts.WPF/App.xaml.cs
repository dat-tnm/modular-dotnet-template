using CompanyName.ProjectName.Hosts.WPF.Controls;
using CompanyName.ProjectName.Hosts.WPF.Services;
using CompanyName.ProjectName.Hosts.WPF.ViewModels.MainMenu;
using CompanyName.ProjectName.Modules.Auth.Extensions;
using CompanyName.ProjectName.Shared.UnitOfWork.Extensions;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Windows;

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

                // Register Business Directives Module
                services.AddModuleAuthForWPF(configuration);

                // Register MainWindow first
                services.AddSingleton<MainWindow>();

                // Register Navigation Service as singleton
                services.AddSingleton<INavigationService, NavigationService>();

                // Register ViewModels
                services.AddTransient<ViewModels.Login.LoginViewModel>();
                services.AddTransient<MainMenuViewModel>();
                services.AddSingleton<NavigationBarViewModel>(); // Singleton to share username across all instances

                // Register Views (V3 - new implementation)
                services.AddTransient<ViewModels.Login.LoginView>();
                services.AddTransient<MainMenuView>();

                // Register Controls
                services.AddTransient<NavigationBar>();
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

            // Get services
            var mainWindow = WpfHost.Services.GetRequiredService<MainWindow>();
            var navigationService = WpfHost.Services.GetRequiredService<CompanyName.ProjectName.Hosts.WPF.Services.INavigationService>();
            var loginView = WpfHost.Services.GetRequiredService<CompanyName.ProjectName.Hosts.WPF.ViewModels.Login.LoginView>();

            // Setup navigation service with MainWindow's ContentControl
            navigationService.SetContentControl(mainWindow.MainContentControl);

            // Navigate to Login view first
            navigationService.NavigateTo(loginView);

            // Show main window
            mainWindow.Show();

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


        // Helper property to access ServiceProvider from anywhere in the app
        public static IServiceProvider ServiceProvider => App.WpfHost.Services;
    }

}
