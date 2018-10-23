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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using deORO.ViewModels;
using deORODataAccessApp.Models;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Controls.Primitives;
using deORO.Helpers;

namespace deORO.Views
{
    /// <summary>
    /// Interaction logic for NewUserView.xaml
    /// </summary>
    /// 
    public partial class NewUserView : UserControl
    {
        NewUserViewModel viewModel = new NewUserViewModel();

        public NewUserView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = this.DataContext as NewUserViewModel;

            DatePicker DOB_Datepicker =  MyCalendar;
           
            TextBox DOB_Datepicker_TextBox = (TextBox)DOB_Datepicker.Template.FindName("PART_TextBox", DOB_Datepicker);
            DOB_Datepicker_TextBox.Visibility = System.Windows.Visibility.Hidden;
           
            if (viewModel != null)
            {
                viewModel.Init();
                viewModel.User = new User(Helpers.Global.EmailRequiredToRegister, Helpers.Global.FirstLastNameRequiredToRegister, Helpers.Global.DOBAndGenderRequired);
                if (Helpers.Global.DOBAndGenderRequired == false)
                {
                    DOB_Datepicker.Text = "01/01/1990";
                    viewModel.User.DOB = "01/01/1990";
                    RadioButtonMaleGender.IsChecked = true;
                }
                viewModel.User.Barcode = Global.InvalidUserBarcode;
                Grid.DataContext = viewModel.User;

            }
         }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
                viewModel.Dispose();
        }

        private void RadioButtonsGender_Checked(object sender, RoutedEventArgs e)
        {
            if (RadioButtonMaleGender.IsChecked == true)
            {
                viewModel.User.Gender = "Male";
            }
            else if (RadioButtonFemaleGender.IsChecked == true)
            {
                viewModel.User.Gender = "Female";
            }
            else
            {
                viewModel.User.Gender = "";
            }

        }

        private void DatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            // Finding the calendar that is child of stadart WPF DatePicker
            DatePicker datepicker = (DatePicker)sender;
            Popup popup = (Popup)datepicker.Template.FindName("PART_Popup", datepicker);
            System.Windows.Controls.Calendar cal = (System.Windows.Controls.Calendar)popup.Child;            
            cal.DisplayMode = System.Windows.Controls.CalendarMode.Decade;
           
        }

        private void DatePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (MyCalendar.Text != "" && MyCalendar.Text != null)
            {
                viewModel.User.DOB = MyCalendar.Text;
                DOB_Textblock.Text = MyCalendar.Text;
            }
            //else {
            //    viewModel.User.DOB = null;               
            //    DOB_Textblock.Text = "Date not selected!";
            
            //}

        }

 

        //private bool p_ButtonMaleIsChecked;

        /// <summary>
        /// Summary
        /// </summary>
        /// 
        //public bool ButtonMaleIsChecked
        //{
        //    get {
        //        //viewModel.UserGender = "Male";
        //        viewModel.User.Gender = "Male";

        //        return p_ButtonMaleIsChecked;
        //    }
        //    set
        //    {
        //        p_ButtonMaleIsChecked = value;
        //    }


        //}

        //private bool p_ButtonFemaleIsChecked;

        ///// <summary>
        ///// Summary
        ///// </summary>
        //public bool ButtonFemaleIsChecked
        //{
        //    get {
        //        viewModel.User.Gender = "Female";

        //        return p_ButtonFemaleIsChecked;
        //    }
        //    set
        //    {
        //        p_ButtonFemaleIsChecked = value;
        //    }
        //}



    }

    //public partial class NewUserView : UserControl
    //{
    //    NewUserViewModel viewModel = null;

    //    public NewUserView()
    //    {
    //        InitializeComponent();
    //    }

    //    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    //    {
    //        viewModel = this.DataContext as NewUserViewModel;

    //        if (viewModel != null)
    //        {
    //            viewModel.Init();
    //            viewModel.User = new User(Helpers.Global.EmailRequiredToRegister,Helpers.Global.FirstLastNameRequiredToRegister);
    //            Grid.DataContext = viewModel.User;
    //        }
    //        //Simple Fade In
    //        DoubleAnimation doubleanimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(250)));
    //        NewUser.BeginAnimation(OpacityProperty, doubleanimation);

    //    }

    //    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    //    {
    //        if (viewModel != null)
    //            viewModel.Dispose();
    //    }
    //    //public void DoTransition()
    //    //{
    //    //    double targetX = this.ActualWidth;

    //    //    this.TransitionStoryboard.Stop();
    //    //    this.TransitionStoryboard.Children.Clear();
    //    //    IEasingFunction easing = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
    //    //    DoubleAnimation translateXAnim = new DoubleAnimation()
    //    //    {
    //    //        To = targetX,
    //    //        Duration = TimeSpan.FromMilliseconds(250),
    //    //        EasingFunction = easing,
    //    //    };

    //    //    TranslateTransform t = this.Template.FindName("pageContentContainerTransform", this) as TranslateTransform;
    //    //    t.BeginAnimation(TranslateTransform.XProperty, translateXAnim);

    //    //}
    // }
}
