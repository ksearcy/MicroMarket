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
using System.Windows.Media.Animation;
using System.Windows.Interactivity;

namespace deORO.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        LoginViewModel viewModel = null;

        public LoginView()
        {
            InitializeComponent();
            //WidenObject(150, TimeSpan.FromSeconds(1));
        }
        //private void WidenObject(int newWidth, TimeSpan duration)
        //{
        //    DoubleAnimation animation = new DoubleAnimation(newWidth, duration);
        //    Login.BeginAnimation(Rectangle.WidthProperty, animation);
        //}

        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = this.DataContext as LoginViewModel;

            if (viewModel != null)
                viewModel.Init();

            //Simple Fade In
            DoubleAnimation doubleanimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(250)));
            Login.BeginAnimation(OpacityProperty, doubleanimation);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
                viewModel.Dispose();
        }

    }
    

}
