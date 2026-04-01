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

namespace CompanyName.ProjectName.Hosts.WPF.ViewModels.Common.Sample
{
    /// <summary>
    /// Interaction logic for SampleSubMenuView.xaml
    /// </summary>
    public partial class SampleSubMenuView : UserControl
    {
        public SampleSubMenuView(SampleSubMenuViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
