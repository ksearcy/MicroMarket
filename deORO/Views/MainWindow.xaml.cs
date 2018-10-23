using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using deORO.ViewModels;
using System.Windows.Controls.Primitives;
using Microsoft.Practices.Composite.Events;
using System.Timers;
using deORO.Helpers;
using System.Windows.Threading;
using WpfScreenHelper;





namespace deORO.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ViewModels.MainWindowViewModel viewModel = new ViewModels.MainWindowViewModel();
        ResourceDictionary skin = new ResourceDictionary();
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        System.Timers.Timer aTimer;
        DispatcherTimer timer = new DispatcherTimer();
        bool BottomPanelHidded = true;



        public MainWindow()
        {

            this.WindowStartupLocation = WindowStartupLocation.Manual;
            //this.WindowStartupLocation = System.Windows.Forms.Screen.AllScreens[2].WorkingArea;

            //System.Drawing.Rectangle workingArea = System.Windows.Forms.Screen.AllScreens[2].WorkingArea;


            InitializeComponent();
            this.DataContext = viewModel;


            //skin.Source = new Uri(@"Resources\deOROSkin.xaml", UriKind.Relative);
            //Application.Current.Resources.MergedDictionaries.Add(skin);

            if (!deORO.Helpers.Global.RunMode.ToLower().Equals("debug"))
                this.WindowStyle = System.Windows.WindowStyle.None;

            //if (!deORO.Helpers.Global.CollapseRightPane)
            //{

            //}
            aggregator.GetEvent<EventAggregation.FastTouchItemSelectedEvent>().Subscribe(HideLeftDynamicPanel);
            aggregator.GetEvent<EventAggregation.MissingBarcodeItemAddToCartCompleteEvent>().Subscribe(HideRightDynamicPanel);
            aggregator.GetEvent<EventAggregation.MissingBarcodeItemAddToCartCancelEvent>().Subscribe(HideRightDynamicPanel);
            //aggregator.GetEvent<EventAggregation.RightToolBarEnableEvent>().Subscribe(HideAllDynamicPanel);
            aggregator.GetEvent<EventAggregation.ShowDynamicPanelDialog>().Subscribe(ShowDynamicPanelDialog);
            aggregator.GetEvent<EventAggregation.DynamicPanelOKButtonClick>().Subscribe(DynamicPanelOkButton_Click);
            aggregator.GetEvent<EventAggregation.DynamicPanelPrintButtonClick>().Subscribe(DynamicPanelOkButton_Click);



        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
            {
                viewModel.Init();
            }

        }



        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel.Dispose();
        }

        private void btnLeftMenuHide_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbHideLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);
        }

        private void btnLeftMenuShow_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbShowLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);
        }


        private void btnRightMenuHide_Click(object sender, RoutedEventArgs e)
        {
            ShowHideMenu("sbHideRightMenu", btnRightMenuHide, btnRightMenuShow, pnlRightMenu);

        }

        private void btnRightMenuShow_Click(object sender, RoutedEventArgs e)
        {

            ShowHideMenu("sbShowRightMenu", btnRightMenuHide, btnRightMenuShow, pnlRightMenu);
            aTimer = new System.Timers.Timer(1250);
            aTimer.Elapsed += ReloadItems;
            aTimer.Enabled = true;
        }


        private void ReloadItems(Object source, ElapsedEventArgs e)
        {
            aTimer.Close();
            aggregator.GetEvent<EventAggregation.OpenDamagedBarcodeItemsPanel>().Publish(null);

        }


        private void ShowHideMenu(string Storyboard, Button btnHide, Button btnShow, StackPanel pnl)
        {

            if (Storyboard.Contains("sbHideBottomMenu"))
            {
                if (BottomPanelHidded == false)
                {
                  Storyboard sb = Resources[Storyboard] as Storyboard;
                  sb.Begin(pnl);
                  BottomPanelHidded = true;
                }
                
            }else{            
               Storyboard sb = Resources[Storyboard] as Storyboard;
               sb.Begin(pnl);
            }

            

            if (Storyboard.Contains("Show"))
            {
                if (btnHide != null)
                {
                    btnHide.Visibility = System.Windows.Visibility.Visible;
                }
                if (btnShow != null)
                {
                    btnShow.Visibility = System.Windows.Visibility.Hidden;
                }


            }
            else if (Storyboard.Contains("Hide"))
            {

                if (btnHide != null)
                {
                    btnHide.Visibility = System.Windows.Visibility.Hidden;
                }
                if (btnShow != null)
                {
                    btnShow.Visibility = System.Windows.Visibility.Visible;
                }
            }

        }

        private void HideLeftDynamicPanel(object parameter = null)
        {

            ShowHideMenu("sbHideLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);

        }

        private void HideRightDynamicPanel(object parameter = null)
        {

            ShowHideMenu("sbHideRightMenu", btnRightMenuHide, btnRightMenuShow, pnlRightMenu);

        }

        private void HideBottomDynamicPanel(object parameter = null)
        {
           ShowHideMenu("sbHideBottomMenu", null, null, pnlBottomMenu); 
        }

        private void HideAllDynamicPanel(object parameter = null)
        {
            ShowHideMenu("sbHideRightMenu", btnRightMenuHide, btnRightMenuShow, pnlRightMenu);
        }

        private void DynamicPanelOkButton_Click(object parameter = null)
        {
            timer.Stop();
            HideBottomDynamicPanel();
        }


        private void ShowDynamicPanelDialog(object parameter = null)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (Global.DynamicPanelDialogType == "Succsessful")
                {
                    MessageBoxMainView.successImage.Visibility = System.Windows.Visibility.Visible;
                    MessageBoxMainView.failImage.Visibility = System.Windows.Visibility.Hidden;
                    MessageBoxMainView.TextBlockMessage.Foreground = Brushes.DarkGreen;
                }
                else
                {

                    MessageBoxMainView.successImage.Visibility = System.Windows.Visibility.Hidden;
                    MessageBoxMainView.failImage.Visibility = System.Windows.Visibility.Visible;
                    MessageBoxMainView.TextBlockMessage.Foreground = Brushes.Red;

                }

                MessageBoxMainView.TextBlockMessage.Text = Global.DynamicPanelDialogMessage;

                
                ShowHideMenu("sbShowBottomMenu", null, null, pnlBottomMenu);

                BottomPanelHidded = false;

                timer.Interval = new TimeSpan(0, 0, Helpers.Global.AutoCloseMessage);
                timer.Tick += timer_Tick;
                timer.Start();
            });


        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            HideBottomDynamicPanel();

        }


    }
}
