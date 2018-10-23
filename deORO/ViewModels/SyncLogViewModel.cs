using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;
using System.Diagnostics;
using System.ComponentModel;

namespace deORO.ViewModels
{
    public class SyncLogViewModel : BaseViewModel
    {
        public ICommand SyncDataCommand { get { return new DelegateCommandWithParam(ExecuteSyncDataCommand); } }
        public ICommand SaveSyncDatesCommand { get { return new DelegateCommandWithParam(ExecuteSaveSyncDatesCommand); } }
        public ICommand UpdatePasswordCommand { get { return new DelegateCommandWithParam(ExecuteUpdatePasswordCommand); } }


        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        SynclogRepository repo1 = new SynclogRepository();
        SyncDataRepository repo2 = new SyncDataRepository();
        private BackgroundWorker workerPassword = new BackgroundWorker();

        private List<synclog> logs;
        public List<synclog> Logs
        {
            get
            {
                logs = repo1.GetLogs();
                return logs;
            }
            set
            {
                logs = value;
                RaisePropertyChanged(() => Logs);
            }
        }

        public SyncLogViewModel() { }

        DateTime? lastUploadDate;

        public DateTime? LastUploadDate
        {
            get { return lastUploadDate; }
            set { lastUploadDate = value; RaisePropertyChanged(() => LastUploadDate); }
        }

        DateTime? lastDownloadDate;

        public DateTime? LastDownloadDate
        {
            get { return lastDownloadDate; }
            set { lastDownloadDate = value; RaisePropertyChanged(() => LastDownloadDate); }
        }

        string lastDownloadStatus;

        public string LastDownloadStatus
        {
            get { return lastDownloadStatus; }
            set { lastDownloadStatus = value; RaisePropertyChanged(() => LastDownloadStatus); }
        }


        string lastUploadStatus;

        public string LastUploadStatus
        {
            get { return lastUploadStatus; }
            set { lastUploadStatus = value; RaisePropertyChanged(() => LastUploadStatus); }
        }


        public override void Init()
        {
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Subscribe((x) => { Logs = repo1.GetLogs(); });

            LastDownloadDate = repo2.GetLastDownload().date_time;
            LastUploadDate = repo2.GetLastUpload().date_time;

            LastDownloadStatus = repo2.GetLastDownload().status;
            LastUploadStatus = repo2.GetLastUpload().status;

            base.Init();
        }

        private void ExecuteSyncDataCommand(object obj)
        {
            IDialogService popupService = null;
            Process[] pname = Process.GetProcessesByName("deOROSyncData");

            if (pname.Length > 0)
            {
                //popupService = new MessageBoxViewService();
               // popupService.ShowAutoCloseDialog("Sync Data", "There is already a scheduled Sync Data in progress.");
                 DialogViewService.ShowAutoCloseDialog("Sync Data", "There is already a scheduled Sync Data in progress.");
                return;
            }

            SyncDataViewModel vm = new SyncDataViewModel();
            //popupService = new PopupViewService(250, 100);
            DialogViewService.ShowDialog(vm, 250, 100);

            LastDownloadDate = repo2.GetLastDownload().date_time;
            LastUploadDate = repo2.GetLastUpload().date_time;

            LastDownloadStatus = repo2.GetLastDownload().status;
            LastUploadStatus = repo2.GetLastUpload().status;
        }

        public override void Dispose()
        {
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Unsubscribe((x) => { Logs = repo1.GetLogs(); });
        }

        private void ExecuteUpdatePasswordCommand(object obj)
        {
            workerPassword.DoWork += worker_DoWork;
            workerPassword.RunWorkerCompleted += worker_RunWorkerCompleted;
            workerPassword.RunWorkerAsync();

            App.Current.Dispatcher.Invoke(() =>
            {
                BusyViewModel busyView = new BusyViewModel();
                busyView.Message = "Updating Password. Please Wait";

                //PopupViewService popup = new PopupViewService(250, 100);
                DialogViewService.ShowDialog(busyView, 250, 100);
            });
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Logs = repo1.GetLogs();
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);

            workerPassword.DoWork -= worker_DoWork;
            workerPassword.RunWorkerCompleted -= worker_RunWorkerCompleted;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            MainWindowViewModel.UpdatePassword();
        }

        private void ExecuteSaveSyncDatesCommand(object obj)
        {
            //IDialogService dialogService = new MessageBoxViewService();
            string message = "Sync Dates Saved Successfully";

            try
            {
                DateTime.Parse(LastUploadDate.Value.ToString());
                DateTime.Parse(LastDownloadDate.Value.ToString());
            }
            catch
            {
                message = "Invalid Date Time Format. Please correct and retry";
            }

            try
            {
                repo2.UpdateDownload(LastDownloadDate.Value, "Success");
                repo2.UpdateUpload(LastUploadDate.Value, "Success");
            }
            catch
            {
                message = "Unable to Save Sync Dates";
            }

            DialogViewService.ShowAutoCloseDialog("Sync Dates", message);
        }
    }
}
