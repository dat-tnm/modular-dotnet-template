using CompanyName.ProjectName.Hosts.WPF.Services;
using CompanyName.ProjectName.Hosts.WPF.ViewModels.Common;
using CompanyName.ProjectName.Modules.Auth.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace CompanyName.ProjectName.Hosts.WPF.ViewModels.MainMenu;


public partial class MainMenuViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IUACLoginService _uacLoginService;

    private bool _hasOpcodeDirectiveAdminRole;
    private bool _hasAllocateMtlDirectiveAdminRole;
    private bool _hasCmmDirectiveAdminRole;
    private bool _hasCncOpsDirectiveAdminRole;

    public MainMenuViewModel(INavigationService navigationService, IServiceProvider serviceProvider, IUACLoginService uacLoginService)
    {
        _navigationService = navigationService;
        _serviceProvider = serviceProvider;
        _uacLoginService = uacLoginService;

        // Load user roles asynchronously
        _ = LoadUserRolesAsync();
    }

    private async Task LoadUserRolesAsync()
    {
        try
        {
            //var bpmRoles = await _uacLoginService.GetRoles("BPMSetter");
            //_hasCncOpsDirectiveAdminRole = bpmRoles.Contains(UserService.CncOpsDirectiveAdmin);
            //NavigateToCncOpsCommand.NotifyCanExecuteChanged();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading user roles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    //[RelayCommand]
    //private void NavigateToCncOps()
    //{
    //    if (!_hasCncOpsDirectiveAdminRole)
    //    {
    //        MessageBox.Show($"You don't have {UserService.CncOpsDirectiveAdmin} role to open this menu!",
    //            "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
    //        return;
    //    }

    //    var view = _serviceProvider.GetRequiredService<CncOpsDirectiveListView>();
    //    _navigationService.NavigateTo(view);
    //}

}


