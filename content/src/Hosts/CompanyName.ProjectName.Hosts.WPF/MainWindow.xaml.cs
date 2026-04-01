using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompanyName.ProjectName.Hosts.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MainWindow> _logger;

        // Constructor with dependency injection
        public MainWindow(IConfiguration configuration, ILogger<MainWindow> logger)
        {
            InitializeComponent();

            _configuration = configuration;
            _logger = logger;

            LoadConfigurationExample();
        }

        private void LoadConfigurationExample()
        {
            // Example: Read configuration values
            var appName = _configuration["ApplicationSettings:ApplicationName"];
            var version = _configuration["ApplicationSettings:Version"];
            var environment = _configuration["ApplicationSettings:Environment"];
            var connectionString = _configuration["DbOptions:ConnectionString"];

            // Log the values
            _logger.LogInformation("Application: {AppName} v{Version} ({Environment})",
                appName, version, environment);
            _logger.LogInformation("Connection String: {ConnectionString}", connectionString);

            // Update window title with configuration
            this.Title = $"{appName} v{version} - {environment}";
        }
    }
}