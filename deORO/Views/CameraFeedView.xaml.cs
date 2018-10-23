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
using deORO.Helpers;
using deORO.ViewModels;

namespace deORO.Views
{
    /// <summary>
    /// Interaction logic for CameraFeedView.xaml
    /// </summary>
    public partial class CameraFeedView : UserControl
    {
        CameraFeedViewModel viewModel = new CameraFeedViewModel();
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        string[] streams = { Helpers.Global.Camera1, Helpers.Global.Camera2, Helpers.Global.Camera3, Helpers.Global.Camera4 };
        int indexToShow = 0;

        public CameraFeedView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
            {
                viewModel.Init();

                Task.Factory.StartNew(() => PlayStream(Border1, viewModel.camera1Uri));
                //Task.Factory.StartNew(() => PlayStream(Border2, viewModel.camera2Uri));
                //Task.Factory.StartNew(() => PlayStream(Border3, viewModel.camera3Uri));
                //Task.Factory.StartNew(() => PlayStream(Border4, viewModel.camera4Uri));
            }

            //if (Helpers.Global.EnableCameraFeed)
            //{
            //    ShowStream();

            //    timer.Interval = new TimeSpan(0, 0, Helpers.Global.CameraCycleInterval);
            //    timer.Tick += timer_Tick;
            //    timer.IsEnabled = true;
            //}
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.IsEnabled = false;
            ShowStream();
            timer.IsEnabled = true;
        }

        private void ShowStream()
        {

            for (int i = 0; i < streams.Length; i++)
            {
                if (i == indexToShow)
                {
                    Task.Factory.StartNew(() => PlayStream(Border1, streams.ElementAt(i)));
                    break;
                }
            }

            indexToShow = indexToShow == 3 ? 0 : indexToShow + 1;
            
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
                viewModel.Dispose();
        }

        public void PlayStream(Border border, string uri)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (uri != "")
                {
                    WebEye.StreamPlayerControl player = border.Child as WebEye.StreamPlayerControl;
                    if (player != null)
                    {
                        if (player.IsPlaying)
                        {
                            player.Stop();
                        }
                        try
                        {
                            player.Play(uri);
                        }
                        catch { }
                    }
                }
                else
                {
                    border.Child = null;

                    TextBlock tb = new TextBlock();
                    tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    tb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    tb.Text = LocalizationProvider.GetLocalizedValue<string>("CameraFeed.NotConfigured");
                    border.Child = tb;
                }
            });
        }

    }
}
