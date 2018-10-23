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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using deORO.ViewModels;

namespace deORO.Views
{
    /// <summary>
    /// Interaction logic for TransactionHistoryView.xaml
    /// </summary>
    public partial class TransactionHistoryView : UserControl
    {
        TransactionHistoryViewModel viewModel = null;
        public TransactionHistoryView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = this.DataContext as TransactionHistoryViewModel;

            if (viewModel != null)
            {
                viewModel.Init();
            }

            //fromDate.Language = XmlLanguage.GetLanguage("en-US");
            //toDate.Language = XmlLanguage.GetLanguage("en-US");

            this.TextBox_FilterText.Focus();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
                viewModel.Dispose();
        }
               

        private void toDate_CalendarOpened(object sender, RoutedEventArgs e)
        {
            //toDate.Language = XmlLanguage.GetLanguage("en-US");
            //toDate.SelectedDate = viewModel.ToDate;
            //toDate.UpdateLayout();
        }

        private void fromDate_CalendarOpened(object sender, RoutedEventArgs e)
        {
            //fromDate.Language = XmlLanguage.GetLanguage("en-US");
            //fromDate.SelectedDate = viewModel.FromDate;
            //fromDate.UpdateLayout();
        }
    }
}
