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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Practices.Composite.Events;
using deORO.Helpers;
using deORO.Printer;
using deORO.Templates;
using deORO.ViewModels;

namespace deORO.Views
{
    /// <summary>
    /// Interaction logic for DialogBox.xaml
    /// </summary>
    public partial class MessageBoxMainView : UserControl
    {

        PrinterTemplate PrinterTemplates = new PrinterTemplate();
        PrinterRepository ReceiptsPrinter = new PrinterRepository();
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        DispatcherTimer timer = new DispatcherTimer();

        public MessageBoxMainView()
        {
            InitializeComponent();
            //App.Current.MainWindow.Opacity = 0.3;

            if (Global.PrinterConnected == false)
            {
               this.PrintButton.Visibility = System.Windows.Visibility.Collapsed;
            }

            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Subscribe(PopClose);
            aggregator.GetEvent<EventAggregation.PopupCancelEvent>().Subscribe(PopClose);
        }

        public MessageBoxMainView(string header, string message, bool autoClose = false)
            : this()
        {
            //TextBlockHeader.Text = header;

            TextBlockMessage.Text = message;
            
            //App.Current.MainWindow.Opacity = 0.3;

            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Subscribe(PopClose);
            aggregator.GetEvent<EventAggregation.PopupCancelEvent>().Subscribe(PopClose);


            if (autoClose)
            {
                timer.Interval = new TimeSpan(0, 0, Helpers.Global.AutoCloseMessage);
                timer.Tick += timer_Tick;
                timer.Start();
            }

        }


        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            PopClose(this);
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Unsubscribe(PopClose);
        }

        private void PopClose(object obj)
        {
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Unsubscribe(PopClose);

            //App.Current.Dispatcher.Invoke(() =>
            //{
            //    this.Close();
            //});
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            aggregator.GetEvent<EventAggregation.DynamicPanelOKButtonClick>().Publish(null);
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Unsubscribe(PopClose);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //this.DialogResult = false;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            //this.DialogResult = Global.PrinterConnected;
            //this.DialogResult = true;

            if (Global.PrinterConnected == true)
            {
                try
                {
                    ReceiptsPrinter.Print(PrinterTemplates.CustomerInfo());
                    if (Global.DialogTypeForPrint == "Purchase Complete")
                    {
                        ReceiptsPrinter.Print(PrinterTemplates.SendPaymentComplete(Global.ShoppingCartIdForPrint, Global.PaymentMethodForPrint, Global.ShoppingCartItemsForPrint));
                        if (Global.User != null) { ReceiptsPrinter.Print(PrinterTemplates.UserBalance()); }
                    }
                    else if (Global.DialogTypeForPrint == "Account Refill")
                    {
                        ReceiptsPrinter.Print(PrinterTemplates.AccountRefill(Global.PreviousAccountBalanceForPrint, Global.User.AccountBalance, Global.RefillAmountForPrint));
                    }

                    ReceiptsPrinter.Print(PrinterTemplates.Footer());
                    aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Unsubscribe(PopClose);
                }
                catch (Exception)
                {

                    throw;
                }

            }

            aggregator.GetEvent<EventAggregation.DynamicPanelPrintButtonClick>().Publish(null);


        }

        //private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    App.Current.MainWindow.Opacity = 1.0;
        //    aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Unsubscribe(PopClose);
        //    aggregator.GetEvent<EventAggregation.PopupCancelEvent>().Unsubscribe(PopClose);
        //}
    }
}
