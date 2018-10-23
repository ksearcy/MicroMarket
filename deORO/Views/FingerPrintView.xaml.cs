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
using DPCtlUruNet;
using DPUruNet;
using System.Drawing;
using System.Drawing.Imaging;
using deORO.EventAggregation;
using deORO.Helpers;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORODataAccessApp.Models;
using System.IO;


namespace deORO.Views
{
    /// <summary>
    /// Interaction logic for FingerPrintView.xaml
    /// </summary>
    public partial class FingerPrintView : UserControl
    {

        private EnrollmentControl enrollmentControl = null;
        private Reader reader = null;
        private Dictionary<int, DataResult<Fmd>> fingerPrints = new Dictionary<int, DataResult<Fmd>>();
        FingerPrintViewModel viewModel = null;

        public Dictionary<int, DataResult<Fmd>> FingerPrints
        {
            get { return fingerPrints; }
            set { fingerPrints = value; }
        }

        public FingerPrintView()
        {
            InitializeComponent();
        }


        private void InitReader()
        {
            ReaderCollection readerCollection = ReaderCollection.GetReaders();
            ButtonSave.IsEnabled = false;

            if (readerCollection.Count != 0)
            {

                reader = readerCollection[0];
                reader.Reset();
                
                enrollmentControl = new EnrollmentControl(reader, Constants.CapturePriority.DP_PRIORITY_EXCLUSIVE);
                enrollmentControl.CultureInfo = LocalizationProvider.GetLocalizedValue<string>("FingerPrint.CultureInfo");
                enrollmentControl.OnStartEnroll += enrollmentControl_OnStartEnroll;
                enrollmentControl.OnEnroll += enrollmentControl_OnEnroll;
                enrollmentControl.OnDelete += enrollmentControl_OnDelete;
                enrollmentControl.OnCaptured += enrollmentControl_OnCaptured;
                enrollmentControl.OnCancel += enrollmentControl_OnCancel;

             
           
                

                enrollmentControl.MaxEnrollFingerCount = 4;
            }
            else
            {
                LabelError.Content = "Fingerprint reader not found";

            }
        }

        void enrollmentControl_OnCancel(EnrollmentControl enrollmentControl, Constants.ResultCode result, int fingerPosition)
        {

        }

        void enrollmentControl_OnCaptured(EnrollmentControl enrollmentControl, CaptureResult captureResult, int fingerPosition)
        {
            if (captureResult.Data != null)
            {
                foreach (Fid.Fiv fiv in captureResult.Data.Views)
                {
                    ImageFinger.Source = BitmapToImageSource(CreateBitmap(fiv.RawImage, fiv.Width, fiv.Height));
                }
            }

        }

        void enrollmentControl_OnDelete(EnrollmentControl enrollmentControl , Constants.ResultCode result, int fingerPosition)
        {

        }

        void enrollmentControl_OnEnroll(EnrollmentControl enrollmentControl, DataResult<Fmd> enrollmentResult, int fingerPosition)
        {
            ButtonSave.IsEnabled = true;

            if (enrollmentResult != null && enrollmentResult.Data != null)
            {
                if (fingerPrints.ContainsKey(fingerPosition))
                    fingerPrints[fingerPosition] = enrollmentResult;
                else
                    fingerPrints.Add(fingerPosition, enrollmentResult);
            }
        }

        void enrollmentControl_OnStartEnroll(EnrollmentControl enrollmentControl, Constants.ResultCode result, int fingerPosition)
        {

        }

        private void FingerPrint_Loaded(object sender, RoutedEventArgs e)
        {

            viewModel = this.DataContext as FingerPrintViewModel;

            if (viewModel != null)
                viewModel.Init();

            InitReader();


            if (reader != null)
            {
                this.winHost.Child = enrollmentControl;

            }

        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }


        public Bitmap CreateBitmap(byte[] bytes, int width, int height)
        {
            byte[] rgbBytes = new byte[bytes.Length * 3];

            for (int i = 0; i <= bytes.Length - 1; i++)
            {
                rgbBytes[(i * 3)] = bytes[i];
                rgbBytes[(i * 3) + 1] = bytes[i];
                rgbBytes[(i * 3) + 2] = bytes[i];
            }
            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            System.Drawing.Imaging.BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            for (int i = 0; i <= bmp.Height - 1; i++)
            {
                IntPtr p = new IntPtr(data.Scan0.ToInt64() + data.Stride * i);
                System.Runtime.InteropServices.Marshal.Copy(rgbBytes, i * bmp.Width * 3, p, bmp.Width * 3);
            }

            bmp.UnlockBits(data);

            return bmp;
        }
    }
}
