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

namespace deORO.Views
{
    /// <summary>
    /// Interaction logic for ApplicationSettingsView.xaml
    /// </summary>
    public partial class ApplicationSettingsView : UserControl
    {
        ViewModels.ApplicationSettingsViewModel vm = null;

        public ApplicationSettingsView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            vm = DataContext as ViewModels.ApplicationSettingsViewModel;

            if (vm != null)
            {
                vm.Init();
                ItemsSouceKeyValue.ItemsSource = vm.List;
            }
        }
    }
}
