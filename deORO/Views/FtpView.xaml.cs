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
using deORO.Communication;
using deORO.ViewModels;

namespace deORO.Views
{
    /// <summary>
    /// Interaction logic for FtpView.xaml
    /// </summary>
    public partial class FtpView : UserControl
    {

        FtpViewModel viewModel = null;

        public FtpView()
        {
            InitializeComponent();
        }
   
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = this.DataContext as FtpViewModel;

            if (viewModel != null)
                viewModel.Init();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
                viewModel.Dispose();
        }

        private void StartFtpButton_Click(object sender, RoutedEventArgs e)
        {

            FtpSyncTexboxLog.Text = "";

            Ftp FtpSync = new Ftp();
            FtpSync.SyncFTPMainMethod();

            if (FtpSync.FtpSyncLogPublic == null)
            {
                FtpSyncTexboxLog.Text = "\r\n" + @"=======================================NO FILES TO TRANSFER=======================================";
            }else{
                FtpSyncTexboxLog.Text = FtpSync.FtpSyncLogPublic;
                 
            }

        }
    }
}
