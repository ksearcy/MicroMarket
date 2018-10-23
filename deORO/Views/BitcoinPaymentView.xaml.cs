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

namespace deORO.Views
{
    /// <summary>
    /// Interaction logic for CreditCardPaymentView.xaml
    /// </summary>
    public partial class CreditCardPaymentView : UserControl
    {
        CreditCardPaymentViewModel viewModel = null;

        public CreditCardPaymentView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = this.DataContext as CreditCardPaymentViewModel;

            if (viewModel != null)
                viewModel.Init();

            //if (Helpers.Global.ZipCodeRequired)
            //{
            //    TextZip.Focus();
            //    TextZip.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
            //    {
            //        RoutedEvent = Mouse.PreviewMouseUpEvent,
            //        Source = this,
            //    });
            //}
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
                viewModel.Dispose();
        }
    }
}
