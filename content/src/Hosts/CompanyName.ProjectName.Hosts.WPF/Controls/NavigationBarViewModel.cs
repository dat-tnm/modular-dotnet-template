using CompanyName.ProjectName.Hosts.WPF.Commands;
using CompanyName.ProjectName.Hosts.WPF.Services;
using CompanyName.ProjectName.Hosts.WPF.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace CompanyName.ProjectName.Hosts.WPF.Controls;


public partial class NavigationBarViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private string _username = string.Empty;
    private readonly ViewModels.Login.LoginView _loginView;

    public NavigationBarViewModel(INavigationService navigationService, ViewModels.Login.LoginView loginView)
    {
        _navigationService = navigationService;
        _loginView = loginView;
        LogoutCommand = new RelayCommand(Logout);
        NavigateBackCommand = new RelayCommand(NavigateBack, CanNavigateBack);
    }

    public ICommand NavigateBackCommand { get; private set; }
    public ICommand LogoutCommand { get; private set; }

    public bool NavBarVisibility => _username != string.Empty;

    public bool CanNavigateBack(object? parameter) => _navigationService.CanNavigateBack;

    public string Username
    {
        get => _username;
        set
        {
            if (_username != value)
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                OnPropertyChanged(nameof(NavBarVisibility));
            }
        }
    }

    public void SetUsername(string username)
    {
        Username = username;
    }

    private void NavigateBack(object? parameter)
    {
        _navigationService.NavigateBack();
    }

    private void Logout(object? parameter)
    {
        _navigationService.NavigateTo(_loginView);
        SetUsername(string.Empty);
    }
}
