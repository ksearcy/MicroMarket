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
using System.Windows.Navigation;
using System.Windows.Shapes;
using deORO.ViewModels;

namespace deORO.Views
{
    /// <summary>
    /// Interaction logic for DiscountsMarqueeView.xaml
    /// </summary>
    public partial class DiscountsMarqueeView : UserControl
    {
        DiscountsMarqueeViewModel viewModel = null;

        public DiscountsMarqueeView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = this.DataContext as DiscountsMarqueeViewModel;

            if (viewModel != null)
                viewModel.Init();


            string Copy = " " + TextBoxMarquee.Text;
            double TextGraphicalWidth = new FormattedText(Copy, System.Globalization.CultureInfo.CurrentCulture,
                                                          System.Windows.FlowDirection.LeftToRight, new Typeface(TextBoxMarquee.FontFamily.Source),
                                                          TextBoxMarquee.FontSize, TextBoxMarquee.Foreground).WidthIncludingTrailingWhitespace;
            double TextLenghtGraphicalWidth = 0;
            //BorderTextBoxMarquee.Width = TextGraphicalWidth + 5;

            while (TextLenghtGraphicalWidth < TextBoxMarquee.ActualWidth)
            {
                TextBoxMarquee.Text += Copy;
                TextLenghtGraphicalWidth = new FormattedText(TextBoxMarquee.Text, System.Globalization.CultureInfo.CurrentCulture,
                                                            System.Windows.FlowDirection.LeftToRight, new Typeface(TextBoxMarquee.FontFamily.Source), 
                                                            TextBoxMarquee.FontSize, TextBoxMarquee.Foreground).WidthIncludingTrailingWhitespace;
            }

            TextBoxMarquee.Text += " " + TextBoxMarquee.Text;
            ThicknessAnimation ThickAnimation = new ThicknessAnimation();
            ThickAnimation.From = new Thickness(0, 0, 0, 0);
            ThickAnimation.To = new Thickness(-TextGraphicalWidth, 0, 0, 0);
            ThickAnimation.RepeatBehavior = RepeatBehavior.Forever;
            ThickAnimation.Duration = new Duration(TimeSpan.FromSeconds(20));
            TextBoxMarquee.BeginAnimation(TextBox.PaddingProperty, ThickAnimation);

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
                viewModel.Dispose();
        }


        private void Storyboard_Completed(object sender, EventArgs e)
        {
            viewModel.GetMarqueeText();
            //sb.Begin();
        }

    }
}
