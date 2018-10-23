using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Media.Imaging;
using deORO.Helpers;
using System.IO;
using System.Windows.Threading;
using System.Windows;


namespace deORO.ViewModels
{
    public class CycleThruImagesViewModel : BaseViewModel
    {

        private Timer timer;
        private string[] files;
        private int filesCount;
        private int filePos = 0;

        BitmapImage imageSource = null;

        public BitmapImage ImageSource
        {
            get
            {
                return imageSource;
            }

            set
            {
                imageSource = value;
                RaisePropertyChanged(() => ImageSource);
            }
        }

        private Uri imagePath;

        public Uri ImagePath
        {
            get
            {
                return imagePath;
            }
            set
            {
                imagePath = value;
                RaisePropertyChanged(() => ImagePath);
            }
        }

        public override void Init()
        {
            base.Init();
        }


        public CycleThruImagesViewModel()
        {
            try
            {
                InitImages();
                SetImage();

                timer = new Timer(Convert.ToDouble(Global.ImageCycleInterval));
                //timer.Enabled = true;
                //timer.Start();
                timer.Elapsed += timer_Elapsed;
            }
            catch { }

        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SetImage();
        }

        public void InitImages()
        {
            files = Directory.GetFiles(Global.SlideShowImagesPath, "*.*");
            filesCount = files.Count();
        }

        public void SetImage()
        {
            Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
             {
                 BitmapImage bitmapImage = new BitmapImage();
                 bitmapImage.BeginInit();
                 ImagePath = new Uri(files[filePos], UriKind.Relative);
                 bitmapImage.UriSource = new Uri(files[filePos], UriKind.Relative);
                 bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                 bitmapImage.EndInit();
                 bitmapImage.Freeze();

                 ImageSource = bitmapImage;
             }));

            if (++filePos == filesCount)
                filePos = 0;

        }

    }
}
