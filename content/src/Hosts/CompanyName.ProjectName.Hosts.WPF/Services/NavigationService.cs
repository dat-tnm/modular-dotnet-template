using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace CompanyName.ProjectName.Hosts.WPF.Services;

public class NavigationService : INavigationService
{
    private readonly Stack<UserControl> _navigationStack = new();
    private ContentControl? _contentControl;

    public bool CanNavigateBack => _navigationStack.Count > 1;

    public void SetContentControl(ContentControl contentControl)
    {
        _contentControl = contentControl;
    }

    public void NavigateTo(UserControl view)
    {
        if (_contentControl == null)
            throw new InvalidOperationException("ContentControl not set. Call SetContentControl first.");

        _navigationStack.Push(view);
        _contentControl.Content = view;
    }

    public void NavigateBack()
    {
        if (_contentControl == null)
            throw new InvalidOperationException("ContentControl not set.");

        if (_navigationStack.Count > 1)
        {
            _navigationStack.Pop();
            _contentControl.Content = _navigationStack.Peek();
        }
    }
}
