using Microsoft.Extensions.DependencyInjection;
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

namespace CompanyName.ProjectName.Hosts.WPF.Controls
{
    /// <summary>
    /// Interaction logic for NavigationBar.xaml
    /// </summary>
    public partial class NavigationBar : UserControl
    {
        public static readonly DependencyProperty ShowBackButtonProperty =
        DependencyProperty.Register(nameof(ShowBackButton), typeof(bool), typeof(NavigationBar), new PropertyMetadata(true));

        public static readonly DependencyProperty ShowLogoutButtonProperty =
            DependencyProperty.Register(nameof(ShowLogoutButton), typeof(bool), typeof(NavigationBar), new PropertyMetadata(true));

        public bool ShowBackButton
        {
            get => (bool)GetValue(ShowBackButtonProperty);
            set => SetValue(ShowBackButtonProperty, value);
        }

        public bool ShowLogoutButton
        {
            get => (bool)GetValue(ShowLogoutButtonProperty);
            set => SetValue(ShowLogoutButtonProperty, value);
        }

        public NavigationBar()
        {
            InitializeComponent();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return;

            DataContext = App.ServiceProvider.GetRequiredService<CompanyName.ProjectName.Hosts.WPF.Controls.NavigationBarViewModel>();
        }
    }
}
