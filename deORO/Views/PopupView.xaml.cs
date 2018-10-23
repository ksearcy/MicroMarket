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
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Practices.Composite.Events;

namespace deORO.Views
{
    /// <summary>
    /// Interaction logic for PopupView.xaml
    /// </summary>
    public partial class PopupView : Window
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        DispatcherTimer timer = new DispatcherTimer();
        private string _timerType;

        public PopupView()
        {
            InitializeComponent();

            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Subscribe(PopClose);
            aggregator.GetEvent<EventAggregation.PopupCancelEvent>().Subscribe(PopClose);

            Row1.Height = new System.Windows.GridLength(1.0, System.Windows.GridUnitType.Star);
            Row2.Height = new System.Windows.GridLength(0.0, System.Windows.GridUnitType.Star);

            App.Current.MainWindow.Opacity = 0.3;
        }

        public PopupView(bool autoClose, string timerType = "message")
        {
            InitializeComponent();

            _timerType = timerType;

            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Subscribe(PopClose);
            aggregator.GetEvent<EventAggregation.PopupCancelEvent>().Subscribe(PopClose);

            Row1.Height = new System.Windows.GridLength(1.0, System.Windows.GridUnitType.Star);
            Row2.Height = new System.Windows.GridLength(0.0, System.Windows.GridUnitType.Star);

            App.Current.MainWindow.Opacity = 0.3;

            if (autoClose)
            {
                if (timerType == "message")
                    timer.Interval = new TimeSpan(0, 0, Helpers.Global.AutoCloseMessage);
                else
                    timer.Interval = new TimeSpan(0, 0, Helpers.Global.CardProcessorTimeout);

                timer.Tick += timer_Tick;
                timer.Start();

            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            aggregator.GetEvent<EventAggregation.PopupTimeoutEvent>().Publish(_timerType);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                this.Close();
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Current.MainWindow.Opacity = 1.0;
            timer.Stop();
        }
    }
}
