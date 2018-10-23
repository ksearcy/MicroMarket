using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORO.EventAggregation;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    public class DispenseChangeViewModel : BaseViewModel
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ICommand OKCommand { get { return new DelegateCommand(ExecuteOKCommand, CanExecuteOKCommand); } }
        deORO.Communication.ICommunicationType commType;
        private string source;
        private decimal change;
        private string cartpkid;

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; RaisePropertyChanged(() => Message); }
        }

        private bool canExecuteOK;

        public bool CanExecuteOK
        {
            get { return canExecuteOK; }
            set { canExecuteOK = value; RaisePropertyChanged(() => CanExecuteOK); }
        }

        private bool isOKVisible;

        public bool IsOKVisible
        {
            get { return isOKVisible; }
            set { isOKVisible = value; RaisePropertyChanged(() => IsOKVisible); }
        }

        public override void Init()
        {
            commType = deORO.Communication.CommunicationTypeFactory.GetCommunicationType();
            aggregator.GetEvent<EventAggregation.CoinDispenseCompleteEvent>().Subscribe(CoinDispenseCompleted);
            aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>().Subscribe(CoinDispenseFailed);
            aggregator.GetEvent<EventAggregation.NoteDispenseFailedEvent>().Subscribe(CoinDispenseFailed);
            aggregator.GetEvent<EventAggregation.CoinJamEvent>().Subscribe(ProcessCoinJamEvent);

            Message = LocalizationProvider.GetLocalizedValue<string>("DispenseChange.Message");

            CanExecuteOK = true;
            IsOKVisible = false;
            base.Init();

            if (Global.EnableCoin)
                commType.EnableCoins();

            if (Global.BillDispenser)
                commType.EnableBills();
        }

        private bool CanExecuteOKCommand()
        {
            return CanExecuteOK;
        }

        private void ExecuteOKCommand()
        {
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
        }

        public void Dispense(decimal change, string source, string cartpkid)
        {
            this.change = change;
            this.source = source;
            this.cartpkid = cartpkid;

            commType.DispenseChange(change);
        }

        private void ProcessCoinJamEvent(object obj)
        {
            string jamMessage = "Coin Jam Error";

            if (obj != null)
            {

                jamMessage = obj.ToString();

            }

            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
            {
                Event = jamMessage,
                DeviceType = Helpers.Enum.DeviceType.Coin,
                ShoppingCartPkid = cartpkid,
                Code = "EAA"
            });

            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);

            aggregator.GetEvent<EventAggregation.ReturnChangeOptionsEvent>().Publish(null);

        }

        private void CoinDispenseFailed(object obj)
        {
            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
            {
                Event = "CoinDispenseFailed",
                DeviceType = Helpers.Enum.DeviceType.Coin,
                Amount = Convert.ToDecimal(obj, System.Globalization.CultureInfo.InvariantCulture),
                ShoppingCartPkid = cartpkid,
                Code = "EAN"
            });

            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);

            aggregator.GetEvent<EventAggregation.ReturnChangeOptionsEvent>().Publish(null);

           
        }

        private void NoteDispenseFailed(object obj)
        {
            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
            {
                Event = "NoteDispenseFailed",
                DeviceType = Helpers.Enum.DeviceType.Bill,
                Amount = Convert.ToDecimal(obj, System.Globalization.CultureInfo.InvariantCulture),
                ShoppingCartPkid = cartpkid,
                Code = "ENU"
            });

            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
        }

        private void CoinDispenseCompleted(EventAggregation.DispenseEventArgs obj)
        {
            Global.CreditToAccount = 0.0m;

            CashDispenseRepository repo = new CashDispenseRepository();
            string group = Guid.NewGuid().ToString();

            try
            {
                foreach (decimal d in obj.Coins)
                {
                    if (d != 0.0m)
                    {
                        cash_dispense dispense = new cash_dispense();
                        dispense.amount = d;
                        dispense.shoppingcartpkid = cartpkid;
                        dispense.source = source;
                        dispense.created_date_time = DateTime.Now;
                        dispense.group = group;
                        dispense.type = Helpers.Enum.DeviceType.Coin.ToString();
                        dispense.pkid = Guid.NewGuid().ToString();
                        repo.AddCashDispense(dispense);
                    }
                }
            }
            catch { }

            try
            {
                foreach (decimal d in obj.Notes)
                {
                    if (d != 0.0m)
                    {
                        cash_dispense dispense = new cash_dispense();
                        dispense.amount = d;
                        dispense.shoppingcartpkid = cartpkid;
                        dispense.source = source;
                        dispense.created_date_time = DateTime.Now;
                        dispense.group = group;
                        dispense.type = Helpers.Enum.DeviceType.Bill.ToString();
                        dispense.pkid = Guid.NewGuid().ToString();
                        repo.AddCashDispense(dispense);
                    }
                }
            }
            catch { }

            if (source == "Purchase Complete")
            {
                //Message = LocalizationProvider.GetLocalizedValue<string>("Message.PurchaseComplete");
                //Message = "Purchase Complete";
                //IsOKVisible = true;

                //===IN THIS CODE WE ARE SHOWING THE NEW POPUP ON THE APPLICATION

                aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
                //System.Threading.Thread.Sleep(5000);
                //aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);

                Task.Factory.StartNew(() => {
                    System.Threading.Thread.Sleep(500);
                    DialogViewService.ShowAutoCloseDialog("Change Dispensed", "Purchase Complete");
                });
                //DialogViewService.ShowAutoCloseDialog("Change Dispensed", "Purchase Complete");
                //System.Threading.Thread.Sleep(5000);

            }
            else
            {
                aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
                aggregator.GetEvent<EventAggregation.ShowPaymentOptionsEvent>().Publish(null);
            }
        }

        public override void Dispose()
        {

            if (commType != null)
            {
                try
                {
                    commType.CloseDevices();
                }
                catch { }

                try
                {
                    commType.Close();
                }
                catch { }

                commType = null;
            }

            aggregator.GetEvent<EventAggregation.CoinDispenseCompleteEvent>().Unsubscribe(CoinDispenseCompleted);
            aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>().Unsubscribe(CoinDispenseFailed);
            aggregator.GetEvent<EventAggregation.NoteDispenseFailedEvent>().Unsubscribe(NoteDispenseFailed);
            aggregator.GetEvent<EventAggregation.CoinJamEvent>().Unsubscribe(ProcessCoinJamEvent);

        }
    }
}
