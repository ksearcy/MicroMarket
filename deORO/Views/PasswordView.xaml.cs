using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using deORO.ViewModels;
using deORODataAccessApp.Models;

namespace deORO.Views
{
    /// <summary>
    /// Interaction logic for PasswordView.xaml
    /// </summary>
    public partial class PasswordView : UserControl
    {
        PasswordViewModel viewModel = null;

        public PasswordView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = this.DataContext as PasswordViewModel;

            if (viewModel != null)
            {
                viewModel.User = new User();
                Grid.DataContext = viewModel.User;
            }
        }
    }
}
