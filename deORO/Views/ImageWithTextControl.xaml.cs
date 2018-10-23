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

namespace deORO.Views
{
    /// <summary>
    /// Interaction logic for ImageWithTextControl.xaml
    /// </summary>
    public partial class ImageWithTextControl : UserControl
    {
        public ImageWithTextControl()
        {
            InitializeComponent();

            this.DataContext = this;

            ImageTextColor = new SolidColorBrush(Colors.White);

            ImageTextMargin = new Thickness(0, 0, 0, 5);
        }

        public String ImageText
        {
            get { return (String)GetValue(ImageTextProperty); }
            set { SetValue(ImageTextProperty, value); }
        }

        public Thickness ImageTextMargin
        {
            get { return (Thickness)GetValue(ImageTextMarginProperty); }
            set { SetValue(ImageTextMarginProperty, value); }
        }

        public Brush ImageTextColor
        {
            get { return (Brush)GetValue(ImageTextColorProperty); }
            set { SetValue(ImageTextColorProperty, value); }
        }

        public ImageSource ImageSource
        {
            get { return base.GetValue(ImageSourceProperty) as ImageSource; }
            set { base.SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageTextProperty = DependencyProperty.Register("ImageText", typeof(string), typeof(ImageWithTextControl), new PropertyMetadata(""));
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageWithTextControl));
        public static readonly DependencyProperty ImageTextColorProperty = DependencyProperty.Register("ImageTextColor", typeof(Brush), typeof(ImageWithTextControl));
        public static readonly DependencyProperty ImageTextMarginProperty = DependencyProperty.Register("ImageTextMargin", typeof(Thickness), typeof(ImageWithTextControl));


    }
}
