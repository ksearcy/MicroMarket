//using Microsoft.Practices.Composite.Events;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;
//using System.Windows.Threading;
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
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;


namespace deORO.Views
{
    public partial class MessageBoxCompleteView : Window
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        DispatcherTimer timer = new DispatcherTimer();

        public MessageBoxCompleteView()
        {
            InitializeComponent();
            App.Current.MainWindow.Opacity = 0.3;

            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Subscribe(PopClose);
            aggregator.GetEvent<EventAggregation.PopupCancelEvent>().Subscribe(PopClose);
        }

        public MessageBoxCompleteView(string header, string message, bool autoClose = false)
            : this()
        {
            TextBlockHeaderComplete.Text = header;
            TextBlockMessageComplete.Text = message;
            App.Current.MainWindow.Opacity = 0.3;

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
        }

        private void PopClose(object obj)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                this.Close();
            });
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void PrinterButton_Click(object sender, RoutedEventArgs e)
        {
            if (Global.PrinterConnected)
            {
                this.DialogResult = true;
            }
            else
            {
                this.DialogResult = false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Current.MainWindow.Opacity = 1.0;

            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Unsubscribe(PopClose);
            aggregator.GetEvent<EventAggregation.PopupCancelEvent>().Unsubscribe(PopClose);
        }
    }
}

