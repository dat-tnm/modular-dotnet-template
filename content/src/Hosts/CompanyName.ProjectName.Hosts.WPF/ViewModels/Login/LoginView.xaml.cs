using System;
using System.Collections.Generic;
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

namespace CompanyName.ProjectName.Hosts.WPF.ViewModels.Login;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    public LoginView(LoginViewModel viewModel) : this()
    {
        DataContext = viewModel;

        // Set the parent window reference when loaded
        Loaded += (s, e) =>
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                viewModel.SetParentWindow(window);
            }
        };
    }
}
