using CompanyName.ProjectName.Hosts.WPF.Commands;
using CompanyName.ProjectName.Hosts.WPF.Controls;
using CompanyName.ProjectName.Hosts.WPF.Services;
using CompanyName.ProjectName.Hosts.WPF.ViewModels.Common;
using CompanyName.ProjectName.Hosts.WPF.ViewModels.MainMenu;
using CompanyName.ProjectName.Modules.Auth.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace CompanyName.ProjectName.Hosts.WPF.ViewModels.Login;

public partial class LoginViewModel : ViewModelBase
{
    private static bool IsAutoLoginEnabled = true;

    private readonly INavigationService _navigationService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly IUACLoginService _uacLoginService;
    private Window? _parentWindow;

    public LoginViewModel(INavigationService navigationService, IServiceProvider serviceProvider, IConfiguration configuration, IUACLoginService uacLoginService)
    {
        _navigationService = navigationService;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _uacLoginService = uacLoginService;
    }

    public ICommand LoginCommand => new AsyncRelayCommand(Login);

    public void SetParentWindow(Window window)
    {
        _parentWindow = window;
    }

    private async Task Login()
    {
        if (_parentWindow == null)
        {
            MessageBox.Show("Parent window not set.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            var (exitCode, output) = await RunLoginAppAndReadOutputAsync(TimeSpan.FromMinutes(5));

            if (exitCode == 0)
            {
                // Set username in NavigationBar
                var navigationBarViewModel = _serviceProvider.GetRequiredService<NavigationBarViewModel>();
                _uacLoginService.SetCurrentUsername(output.Trim('\r', '\n'));
                navigationBarViewModel.SetUsername(output);

                // Navigate to Main Menu View after successful login
                var mainMenuView = _serviceProvider.GetRequiredService<MainMenuView>();
                _navigationService.NavigateTo(mainMenuView);

                // Re-enable and show the main window
                _parentWindow.IsEnabled = true;
                _parentWindow.Show();

                LoginViewModel.IsAutoLoginEnabled = false;
            }
            else
            {
                MessageBox.Show($"User cancelled the login.", "Login failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (OperationCanceledException)
        {
            MessageBox.Show("Login timed out. Waiting time exceeded 5 minutes.", "Login failed", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error launching login app:\n{ex}", "Login failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task<(int exitCode, string output)> RunLoginAppAndReadOutputAsync(TimeSpan timeout)
    {
        var hwnd = new WindowInteropHelper(_parentWindow!).Handle;

        var psi = new ProcessStartInfo
        {
            FileName = _configuration["UAC.Login:ExecutionPath"],
            Arguments = $"BPMSetter {LoginViewModel.IsAutoLoginEnabled} --parentHwnd {hwnd}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using var proc = new Process { StartInfo = psi, EnableRaisingEvents = false };

        try
        {
            if (!proc.Start())
                throw new InvalidOperationException("Failed to start LoginApp.exe");

            // Read STDOUT/ERR asynchronously (prevents deadlocks)
            Task<string> stdoutTask = proc.StandardOutput.ReadToEndAsync();
            Task<string> stderrTask = proc.StandardError.ReadToEndAsync();

            // Wait for exit with a timeout
            using var cts = new CancellationTokenSource(timeout);

            Task exitTask = proc.WaitForExitAsync(cts.Token);

            // Await both: exit + streams completion
            await Task.WhenAll(exitTask, stdoutTask, stderrTask);

            // Combine output if you wish, or prefer STDOUT only
            string output = stdoutTask.Result;
            string err = stderrTask.Result;

            // If you want to include stderr lines too:
            if (!string.IsNullOrWhiteSpace(err))
            {
                output = string.IsNullOrWhiteSpace(output) ? err : output + Environment.NewLine + err;
            }

            int code = proc.ExitCode;

            return (code, output ?? string.Empty);
        }
        catch
        {
            try
            {
                if (!proc.HasExited)
                {
                    proc.Kill(entireProcessTree: true);
                }
            }
            catch { /* ignore kill exceptions */ }
            throw;
        }
    }
}
