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

namespace CompanyName.ProjectName.Hosts.WPF.ViewModels.MainMenu;
public partial class MainMenuView : UserControl
{
    public MainMenuView(MainMenuViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }
}
