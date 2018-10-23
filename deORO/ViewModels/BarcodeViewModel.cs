using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORO.BarcodeScanner;

namespace deORO.ViewModels
{
    public class BarcodeViewModel : BaseViewModel
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ICommand SaveCommand { get { return new DelegateCommand(ExecuteSaveCommand); } }
        private deOROMembershipProvider membership = new deOROMembershipProvider();
        //private IDialogService dialog = new MessageBoxViewService();

        private IBarcodeScanner barcodeScanner = BarcodeScanner.BarcodeScannerFactory.GetBarcodeScanner();

        private string barcode = "";
        public string Barcode
        {
            get { return barcode; }
            set
            {
                barcode = value;
                RaisePropertyChanged(() => Barcode);

                if (barcode.Length > 0)
                    EnableSaveBarcode = true;
                else
                    EnableSaveBarcode = false;
            }
        }

        private string currentBarcode;
        public string CurrentBarcode
        {
            get { return currentBarcode; }
            set
            {
                currentBarcode = value;
                RaisePropertyChanged(() => CurrentBarcode);
            }
        }

        private bool enableSaveBarcode = false;

        public bool EnableSaveBarcode
        {
            get { return enableSaveBarcode; }
            set { enableSaveBarcode = value; RaisePropertyChanged(() => EnableSaveBarcode); }
        }

        public override void Init()
        {
            barcodeScanner.Open("Local");
           
            aggregator.GetEvent<EventAggregation.BarcodeScannerDataLocalEvent>().Subscribe(ProcessBarcode);

            if (Global.User != null)
            {
                CurrentBarcode = Global.User.Barcode ?? "";
            }

            base.Init();

        }

        private void ProcessBarcode(object obj)
        {
            Barcode = obj.ToString();
            CommandManager.InvalidateRequerySuggested();
        }

        private void ExecuteSaveCommand()
        {
            string status = membership.UpdateBarcode(Global.User.UserName, Barcode);
            if (status == "Duplicate")
            {
                DialogViewService.ShowAutoCloseDialog("Change Barcode", "Barcode already exists. Please re-enter Barcode.");
            }
            else if (status == "false")
            {
                DialogViewService.ShowAutoCloseDialog("Change Barcode", "Unable to save barcode. Please retry.");
            }
            else
            {
                Global.User = membership.GetUser(Global.User.UserName) as deOROMembershipUser;
                if (CurrentBarcode.ToString().ToLower().StartsWith(Global.ConversionPrefix.ToLower()))
                {
                    AccountBalanceHistoryRepository repo1 = new AccountBalanceHistoryRepository();
                    deOROMembershipProvider userProvider = new deOROMembershipProvider();
                    repo1.Add(Global.User.ProviderUserKey.ToString(), Global.User.AccountBalance + Global.ConversionReward, Global.ConversionReward, "Conversion Redemption");
                    userProvider.UpdateUserBalance(Global.User.UserName, Global.ConversionReward, "Reward Claimed"); 
                    DialogViewService.ShowAutoCloseDialog("Conversion Reward", "Your account has been credited with " + Global.ConversionReward.ToString("c"));
                }

                CurrentBarcode = Global.User.Barcode;
                Barcode = "";

                Global.Email.SendBarcodeChanged();
                DialogViewService.ShowAutoCloseDialog("Change Barcode", "Barcode was changed successfully.");
            }
        }

        public override void Dispose()
        {
            barcodeScanner.Close();
            aggregator.GetEvent<EventAggregation.BarcodeScannerDataLocalEvent>().Unsubscribe(ProcessBarcode);
        }
    }
}

