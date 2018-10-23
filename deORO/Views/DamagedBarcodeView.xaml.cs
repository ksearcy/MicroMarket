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
using Microsoft.Practices.Composite.Events;
using System.Timers;

namespace deORO.Views
{
    /// <summary>
    /// Interaction logic for DamagedBarcodeView.xaml
    /// </summary>
    public partial class DamagedBarcodeView : UserControl
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        DamagedBarcodeViewModel viewModel = null;

        public int GridItemCount = 0;

        System.Timers.Timer aTimer;


        public DamagedBarcodeView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = this.DataContext as DamagedBarcodeViewModel;

            aggregator.GetEvent<EventAggregation.OpenDamagedBarcodeItemsPanel>().Subscribe(ReloadItems);
            aggregator.GetEvent<EventAggregation.MissingBarcodeItemAddToCartCompleteEvent>().Subscribe(CleanItems);
            aggregator.GetEvent<EventAggregation.MissingBarcodeItemAddToCartCancelEvent>().Subscribe(CleanItems);

            if (viewModel != null)
                viewModel.Init();

            //Dispatcher.BeginInvoke(new Action(() =>
            //    {
            //        System.Threading.Thread.Sleep(500);
            //        Grid.ItemsSource = viewModel.Items;
            //        Message.Visibility = System.Windows.Visibility.Collapsed;
            //    }), System.Windows.Threading.DispatcherPriority.ContextIdle, null);
        }


        private void ReloadItems(object parameter = null)
        {

            Dispatcher.BeginInvoke(new Action(() =>
            {
                System.Threading.Thread.Sleep(400);
                Grid.ItemsSource = viewModel.Items;
                GridItemCount = Grid.Items.Count;
                Message.Visibility = System.Windows.Visibility.Collapsed;
            }), System.Windows.Threading.DispatcherPriority.ContextIdle, null);


        }

        private void CleanItems(object parameter = null)
        {
            aTimer = new System.Timers.Timer(1350);
            aTimer.Elapsed += CleanItemsTimer;
            aTimer.Enabled = true;
        }

        private void CleanItemsTimer(Object source, ElapsedEventArgs e)
        {
            aTimer.Close();
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Grid.ItemsSource = null;
                Grid.Items.Refresh();
                Message.Visibility = System.Windows.Visibility.Visible;
            }), System.Windows.Threading.DispatcherPriority.ContextIdle, null);

        }


        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
                viewModel.Dispose();

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


    }
}
