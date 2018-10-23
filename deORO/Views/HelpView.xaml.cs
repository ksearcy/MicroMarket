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
using WebEye;

namespace deORO.Views
{
    /// <summary>
    /// Interaction logic for HelpView.xaml
    /// </summary>
    public partial class HelpView : UserControl
    {
        HelpViewModel viewModel = null;
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public HelpView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = this.DataContext as HelpViewModel;

            if (viewModel != null)
                viewModel.Init();

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
                viewModel.Dispose();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            ButtonPlay.Visibility = System.Windows.Visibility.Collapsed;
            MedialElementHelp.Visibility = System.Windows.Visibility.Visible;
            MedialElementHelp.Play();
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            ButtonPlay.Visibility = System.Windows.Visibility.Visible;
            MedialElementHelp.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
