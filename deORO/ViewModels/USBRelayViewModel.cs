using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORO.Helpers;
using deOROSyncData;
using deORO.EventAggregation;
using deORO.Templates;
using Microsoft.Practices.Composite.Events;
using deORO.Helpers;
using deORO.Printer;
using deORO.Templates;
using deORO.ViewModels;

namespace deORO.ViewModels
{
    public class USBRelayViewModel : BaseViewModel
    {

        PrinterTemplate PrinterTemplates = new PrinterTemplate();
        PrinterRepository ReceiptsPrinter = new PrinterRepository();

        public ICommand KMtronicRelay1OnCommand { get { return new DelegateCommand(ExecuteKMtronicRelay1OnCommand, () => { return KMtronic1OnEnabled; }); } }
        public ICommand KMtronicRelay1OffCommand { get { return new DelegateCommandWithParam(ExecuteKMtronicRelay1OffCommand, () => { return KMtronic1OffEnabled; }); } }
        //public ICommand KMtronicRelay1OffCommand { get { return new DelegateCommandWithParam(ExecuteKMtronicRelay1OffCommand); } }

        public ICommand KMtronicRelay2OnCommand { get { return new DelegateCommand(ExecuteKMtronicRelay2OnCommand, () => { return KMtronic2OnEnabled; }); } }
        public ICommand KMtronicRelay2OffCommand { get { return new DelegateCommandWithParam(ExecuteKMtronicRelay2OffCommand, () => { return KMtronic2OffEnabled; }); } }
        //public ICommand KMtronicRelay2OffCommand { get { return new DelegateCommandWithParam(ExecuteKMtronicRelay2OffCommand); } }

        public ICommand CashCollectedCommand { get { return new DelegateCommand(ExecuteCashCollectedCommand); } }
        public ICommand ServiceStartCommand { get { return new DelegateCommand(ExecuteServiceStartCommand, () => { return !ServiceStarted; }); } }
        public ICommand ManageItemsCommand { get { return new DelegateCommand(ExecuteManageItemsCommand, () => { return ServiceStarted; }); } }
        public ICommand ServiceCompleteCommand { get { return new DelegateCommand(ExecuteServiceCompleteCommand, () => { return ServiceStarted; }); } }
        public ICommand ResetServiceCommand { get { return new DelegateCommand(ExecuteResetServiceCommand); } }
        public ICommand AddMoneyCommand { get { return new DelegateCommand(ExecuteAddMoneyCommand); } }


        private string userpkid;

        private SyncData sync = new SyncData();

        private USBRelay.KMTronic km = new USBRelay.KMTronic();
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        LocationServiceRepository repo = new LocationServiceRepository();

        private bool kmtronic1OffEnabled = false;
        public bool KMtronic1OffEnabled
        {
            get { return kmtronic1OffEnabled; }
            set { kmtronic1OffEnabled = value; RaisePropertyChanged(() => KMtronic1OffEnabled); }
        }

        private bool kmtronic1OnEnabled;
        public bool KMtronic1OnEnabled
        {
            get { return kmtronic1OnEnabled; }
            set { kmtronic1OnEnabled = value; RaisePropertyChanged(() => KMtronic1OnEnabled); }

        }

        private bool kmtronic2OffEnabled = false;
        public bool KMtronic2OffEnabled
        {
            get { return kmtronic2OffEnabled; }
            set { kmtronic2OffEnabled = value; RaisePropertyChanged(() => KMtronic2OffEnabled); }
        }

        private bool kmtronic2OnEnabled;
        public bool KMtronic2OnEnabled
        {
            get { return kmtronic2OnEnabled; }
            set { kmtronic2OnEnabled = value; RaisePropertyChanged(() => KMtronic2OnEnabled); }

        }

        private bool serviceStarted;

        public bool ServiceStarted
        {
            get { return serviceStarted; }
            set { serviceStarted = value; RaisePropertyChanged(() => ServiceStarted); }
        }

        public override void Init()
        {
            km.Init();
            KMtronic1OnEnabled = true;
            KMtronic1OffEnabled = false;

            KMtronic2OnEnabled = true;
            KMtronic2OffEnabled = false;

            ServiceStarted = repo.IsServicedStated();

            sync.Init();
            base.Init();
        }

        private void ExecuteAddMoneyCommand()
        {
            AddMoneyViewModel vm = new AddMoneyViewModel();
            DialogViewService.ShowDialog(vm, 450, 450);
        }

        private void ExecuteCashCollectedCommand()
        {
            try
            {
                CashCollectionRepository repo1 = new CashCollectionRepository();
                CashCounterRepository repo2 = new CashCounterRepository();

                cash_collection collection = new cash_collection();
                collection.comments = "Cash Collected";
                collection.created_date_time = DateTime.Now;
                collection.pkid = Guid.NewGuid().ToString();
                var username = Global.User.ToString();
                collection.userpkid = Global.User.ProviderUserKey.ToString();
                if (repo1.AddCashCollection(collection))
                {
                    int records = repo2.UpdateCashCollectionPKID(collection.pkid);
                    UploadCashCollectionEvents();
                    //Add print here
                    try
                    {
                        if (Global.PrinterConnected == true)
                        {
                            ReceiptsPrinter.Print(PrinterTemplates.CustomerInfo());
                            ReceiptsPrinter.Print(PrinterTemplates.CashCollection(collection.pkid, username, repo2.GetCashCollectedList(collection.pkid)));
                            ReceiptsPrinter.Print(PrinterTemplates.Footer());
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                    DialogViewService.ShowAutoCloseDialog("Cash Collected", string.Format("{0} record(s) got updated.", records));

                }
                else
                {
                    DialogViewService.ShowAutoCloseDialog("Cash Collected", "Error occured while updating Cash Collection Details");
                }

            }
            catch
            {
                DialogViewService.ShowAutoCloseDialog("Cash Collected", "Error");
            }

        }

        private void UploadCashCollectionEvents()
        {
            Task.Factory.StartNew(() =>
                {
                    var sync = SyncDataFactory.GetSyncData();
                    sync.Init();
                    sync.UploadCashCollectionEvents(Global.User.ProviderUserKey.ToString());
                });
        }

        private void ExecuteManageItemsCommand()
        {
            ItemsViewModel items = new ItemsViewModel();
            DialogViewService.ShowDialog(items, true, 775, 750);
        }

        private void ExecuteResetServiceCommand()
        {
            try
            {
                repo.ResetService();
                ServiceStarted = false;

                DialogViewService.ShowAutoCloseDialog("Reset Service", "Reset Service");
            }
            catch (Exception ex)
            {
                DialogViewService.ShowAutoCloseDialog("Reset Service", ex.Message);
            }
        }

        private async void ExecuteServiceStartCommand()
        {
            try
            {
                Task<int> syncTask = sync.SyncScheduledItems();

                App.Current.Dispatcher.Invoke(() =>
                {
                    BusyViewModel busyView = new BusyViewModel();
                    busyView.Message = "Downloading Schedule and Updating Items Quantity. Please Wait";

                    DialogViewService.Show(busyView, 380, 100);
                });

                int result = await syncTask;
                aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);

                if (result < 0)
                {
                    DialogViewService.ShowDialog("Service Start", "No Schedule found for today");
                    ServiceStarted = false;
                }
                else
                {
                    ServiceStarted = repo.SetServiceStarted(Global.User.ProviderUserKey.ToString());
                    DialogViewService.ShowDialog("Service Start", string.Format("{0} items got updated today", result));
                }
            }
            catch (Exception ex)
            {
                DialogViewService.ShowDialog("Service Start", ex.Message);
                aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
            }

        }

        private async void ExecuteServiceCompleteCommand()
        {
            ItemRepository itemRepo = new ItemRepository();
            try
            {

                Task<int> syncTask1 = sync.UpdateScheduledStatusAndItemsQuantity();
                Task<int> syncTask2 = itemRepo.ResetOverUnderAndStale();

                App.Current.Dispatcher.Invoke(() =>
                {
                    BusyViewModel busyView = new BusyViewModel();
                    busyView.Message = "Uploading Schedule Status and Items Quantity. Please Wait";

                    DialogViewService.Show(busyView, 380, 100);
                });

                int result1 = await syncTask1;
                int result2 = await syncTask2;

                aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);

                ServiceStarted = !repo.SetServiceCompleted(Global.User.ProviderUserKey.ToString());
                DialogViewService.ShowDialog("Service Complete", "Service Complete");
            }
            catch (Exception ex)
            {
                DialogViewService.ShowDialog("Service Complete", ex.Message);
                aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);

            }

        }

        //=============ARCA MX FTP DEVELOPMENT=====================
        private void FTPReporting()
        {
           


        }


        private void ExecuteKMtronicRelay1OnCommand()
        {
            EventLogRepository repo = new EventLogRepository();
            aggregator.GetEvent<EventAggregation.Relay1CloseEvent>().Subscribe(ExecuteKMtronicRelay1OffCommand);
            try
            {
                KMtronic1OnEnabled = false;
                KMtronic1OffEnabled = true;

                km.OpenRelay1();
                event_log eventlog = new event_log();
                eventlog.description = "DoorOpen";
                eventlog.source = "Relay1";
                eventlog.code = "open";
                eventlog.created_date_time = DateTime.Now;
                eventlog.pkid = Guid.NewGuid().ToString();
                eventlog.userpkid = Global.User.ProviderUserKey.ToString();
                repo.AddEventLog(eventlog);
            }
            catch { }

            CommandManager.InvalidateRequerySuggested();
        }
        private void ExecuteKMtronicRelay1OffCommand(object param = null)
        {
            EventLogRepository repo = new EventLogRepository();
            aggregator.GetEvent<EventAggregation.Relay1CloseEvent>().Unsubscribe(ExecuteKMtronicRelay1OffCommand);
            try
            {
                KMtronic1OnEnabled = true;
                KMtronic1OffEnabled = false;

                event_log eventlog = new event_log();
                eventlog.description = "DoorClosed";
                eventlog.source = "Relay1";
                eventlog.code = "closed";
                eventlog.created_date_time = DateTime.Now;
                eventlog.pkid = Guid.NewGuid().ToString();
                eventlog.userpkid = Global.User.ProviderUserKey.ToString();
                repo.AddEventLog(eventlog);
            }
            catch { }

            CommandManager.InvalidateRequerySuggested();
        }

        private void ExecuteKMtronicRelay2OnCommand()
        {
            EventLogRepository repo = new EventLogRepository();
            aggregator.GetEvent<EventAggregation.Relay2CloseEvent>().Subscribe(ExecuteKMtronicRelay2OffCommand);
            try
            {

                KMtronic2OnEnabled = false;
                KMtronic2OffEnabled = true;

                km.OpenRelay2();
                event_log eventlog = new event_log();
                eventlog.description = "TestMotorOn";
                eventlog.source = "Relay2";
                eventlog.code = "on";
                eventlog.created_date_time = DateTime.Now;
                eventlog.pkid = Guid.NewGuid().ToString();
                eventlog.userpkid = Global.User.ProviderUserKey.ToString();
                repo.AddEventLog(eventlog);
            }
            catch { }

            CommandManager.InvalidateRequerySuggested();
        }
        private void ExecuteKMtronicRelay2OffCommand(object param = null)
        {
            EventLogRepository repo = new EventLogRepository();
            aggregator.GetEvent<EventAggregation.Relay2CloseEvent>().Unsubscribe(ExecuteKMtronicRelay2OffCommand);
            try
            {

                KMtronic2OnEnabled = true;
                KMtronic2OffEnabled = false;

                event_log eventlog = new event_log();
                eventlog.description = "TestMotorOff";
                eventlog.source = "Relay2";
                eventlog.code = "off";
                eventlog.created_date_time = DateTime.Now;
                eventlog.pkid = Guid.NewGuid().ToString();
                eventlog.userpkid = Global.User.ProviderUserKey.ToString();
                repo.AddEventLog(eventlog);
            }
            catch { }

            CommandManager.InvalidateRequerySuggested();
        }

        public override void Dispose()
        {
            if (km != null)
            {
                km.CloseRelay1();
                km.CloseRelay2();
                km.Dispose();
            }
        }
    }
}
