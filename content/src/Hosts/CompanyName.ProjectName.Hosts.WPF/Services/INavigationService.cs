using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace CompanyName.ProjectName.Hosts.WPF.Services;

public interface INavigationService
{
    void NavigateTo(UserControl view);
    void NavigateBack();
    bool CanNavigateBack { get; }
    void SetContentControl(ContentControl contentControl);
}