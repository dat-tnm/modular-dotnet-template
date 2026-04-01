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
    private string _username = "N/A";
    private readonly ViewModels.Login.LoginView _loginView;

    public NavigationBarViewModel(INavigationService navigationService, ViewModels.Login.LoginView loginView)
    {
        _navigationService = navigationService;
        _loginView = loginView;
        LogoutCommand = new RelayCommand(_ => Logout());
        NavigateBackCommand = new RelayCommand(_ => NavigateBack(), _ => CanNavigateBack);
    }

    public bool CanNavigateBack => _navigationService.CanNavigateBack;

    public string Username
    {
        get => _username;
        set
        {
            if (_username != value)
            {
                _username = value;
                OnPropertyChanged();
            }
        }
    }

    public void SetUsername(string username)
    {
        Username = string.IsNullOrWhiteSpace(username) ? "Guest" : username;
    }

    public ICommand NavigateBackCommand;
    private void NavigateBack()
    {
        _navigationService.NavigateBack();
    }

    public ICommand LogoutCommand;
    private void Logout()
    {
        _navigationService.NavigateTo(_loginView);
        SetUsername("N/A");
    }
}
