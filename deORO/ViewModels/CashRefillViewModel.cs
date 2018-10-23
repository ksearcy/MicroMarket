using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.EventAggregation;
using deORO.Helpers;
using deORODataAccessApp.Models;
using MEI.CF7000Coin;
using Microsoft.Practices.Composite.Events;
using MPOST;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using System.Globalization;
using deORO.BarcodeScanner;

namespace deORO.ViewModels
{
    class CashRefillViewModel : BaseViewModel
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand, CanExecuteCancelCommand); } }
        public ICommand OKCommand { get { return new DelegateCommand(ExecuteOKCommand, CanExecuteOKCommand); } }
        private ObservableCollection<PaymentItem> items = new ObservableCollection<PaymentItem>();
        deORO.Communication.ICommunicationType commType;
        private IBarcodeScanner barcodeScanner = BarcodeScanner.BarcodeScannerFactory.GetBarcodeScanner();

        public ObservableCollection<PaymentItem> Items
        {
            get
            {
                return items;
            }
        }

        private decimal totalRefillAmount = 0.0m;
        public decimal TotalRefillAmount
        {
            get { return totalRefillAmount; }
            set
            {
                totalRefillAmount = value;
                RaisePropertyChanged(() => TotalRefillAmount);
                Global.AmountPaid = TotalRefillAmount;
            }
        }
                

        public override void Init()
        {
            barcodeScanner.Open("Local");
            aggregator.GetEvent<EventAggregation.CoinAcceptedEvent>().Subscribe(CoinAccepted);
            aggregator.GetEvent<EventAggregation.CoinJamEvent>().Subscribe(ProcessCoinJamEvent);

            aggregator.GetEvent<EventAggregation.BillAcceptedEvent>().Subscribe(BillAccepted);
            aggregator.GetEvent<EventAggregation.BillUnknownEvent>().Subscribe(ProcessBillUnknownEvent);
            aggregator.GetEvent<EventAggregation.BillJamEvent>().Subscribe(ProcessBillJamEvent);
            aggregator.GetEvent<EventAggregation.BillCheatingEvent>().Subscribe(ProcessBillCheatingEvent);
            aggregator.GetEvent<EventAggregation.NoteReaderInhibitedEvent>().Subscribe(ProcessNoteReaderInhibitedEvent);

            commType = deORO.Communication.CommunicationTypeFactory.GetCommunicationType();
            aggregator.GetEvent<EventAggregation.BarcodeScannerDataLocalEvent>().Subscribe(ProcessBarcode);

            if (Global.PaymentOptions.Contains(Helpers.Enum.PaymentMethod.CoinRefill.ToString()))
                commType.EnableCoins();

            if (Global.PaymentOptions.Contains(Helpers.Enum.PaymentMethod.BillRefill.ToString()))
                commType.EnableBills(0, Global.NoteReaderNoteSetPayment);

            //Task.Factory.StartNew(() =>
            //    {
            //        System.Threading.Thread.Sleep(2000);
            //        aggregator.GetEvent<EventAggregation.CoinAcceptedEvent>().Publish(new CashEventArgs { Amount = 0.50m });
            //    });

            totalRefillAmount = 0.0m;
            Global.PaymentItems = items.ToList();
            items.CollectionChanged += items_CollectionChanged;

            base.Init();
        }

        void items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Global.PaymentItems = items.ToList();
        }

        private void ProcessNoteReaderInhibitedEvent(object obj)
        {
            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
            {
                Event = "NoteReaderInhibited",
                DeviceType = Helpers.Enum.DeviceType.Bill,
                Code = "ENK"
            });

            aggregator.GetEvent<EventAggregation.CashRefilCancelEvent>().Publish(null);
        }

        private void ProcessBarcode(object obj)
        {
            //Do Nothing - Ignore Barcode Scan
        }

        private void ProcessCoinJamEvent(object obj)
        {
            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(
            new TransactionErrorEventArgs
            {
                Event = "CoinJam",
                DeviceType = Helpers.Enum.DeviceType.Coin,
                Code = "EAD"
            });
        }

        private void ProcessBillCheatingEvent(object obj)
        {
            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(
            new TransactionErrorEventArgs 
            {
                Event = "BillCheating",
                DeviceType = Helpers.Enum.DeviceType.Bill,
                Code = "ENU"
            });
        }

        private void ProcessBillJamEvent(object obj)
        {
            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(
            new TransactionErrorEventArgs
            {
                Event = "BillJam",
                DeviceType = Helpers.Enum.DeviceType.Bill,
                Code = "ENA"
            });
        }

        private void ProcessBillUnknownEvent(object obj)
        {
            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(
            new TransactionErrorEventArgs
            {
                Event = "BillUnknown",
                DeviceType = Helpers.Enum.DeviceType.Bill,
                Code = "ENU"
            });
        }


        private void BillAccepted(CashEventArgs args)
        {

            PaymentItem item = new PaymentItem();
            item.Source = Helpers.Enum.PaymentMethod.BillRefill.ToString();
            item.Payment = args.Amount;
            item.DateTime = DateTime.Now;
            item.Routing = args.Routing;

            App.Current.Dispatcher.Invoke(() =>
            {
                items.Add(item);
            });

            TotalRefillAmount = items.Sum(x => x.Payment);
            RaisePropertyChanged(() => Items);
        }

        private void CoinAccepted(CashEventArgs args)
        {
            PaymentItem item = new PaymentItem();
            item.Source = Helpers.Enum.PaymentMethod.CoinRefill.ToString();
            item.Payment = args.Amount;
            item.DateTime = DateTime.Now;
            item.Routing = args.Routing;

            App.Current.Dispatcher.Invoke(() =>
            {
                items.Add(item);
            });

            TotalRefillAmount = items.Sum(x => x.Payment);
            RaisePropertyChanged(() => Items);
        }

        private void ExecuteOKCommand()
        {
            aggregator.GetEvent<EventAggregation.CashRefillCompleteEvent>().Publish(new PaymentCompleteEventArgs() { Source = "Cash Refill", PaymentItems = items.ToList() });
        }

        private bool CanExecuteCancelCommand()
        {
            if (TotalRefillAmount < Convert.ToDecimal(Global.DisableCancelTransactionAfter, CultureInfo.InvariantCulture) && Global.EnableCoin && Global.EnableDispenseChange)
                return true;
            else
                return false;
        }

        private bool CanExecuteOKCommand()
        {
            if (items.Count > 0)
            {
                aggregator.GetEvent<EventAggregation.RightToolBarEnableEvent>().Publish(false);
                return true;
            }
            else
            {
                aggregator.GetEvent<EventAggregation.RightToolBarEnableEvent>().Publish(true);
                return false;
            }
        }

        private void ExecuteCancelCommand()
        {
            items.Clear();
            if (totalRefillAmount != 0)
            {
                DispenseChange(totalRefillAmount);
            }
            else
            {
                aggregator.GetEvent<EventAggregation.CashRefilCancelEvent>().Publish(null);
            }
            //TotalRefillAmount = 0.0M;        
        }

        private void DispenseChange(decimal change)
        {
            aggregator.GetEvent<EventAggregation.CoinDispenseCompleteEvent>().Subscribe(CoinDispenseCompleted);
            aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>().Subscribe(CoinDispenseFailed);
            aggregator.GetEvent<EventAggregation.NoteDispenseFailedEvent>().Subscribe(NoteDispenseFailed);
            commType.DispenseChange(change);
        }

        private void CoinDispenseFailed(object obj)
        {
            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
            {
                Event = "CoinDispenseFailed",
                DeviceType = Helpers.Enum.DeviceType.Coin,
                Amount = Convert.ToDecimal(obj, System.Globalization.CultureInfo.InvariantCulture),
                Code = "EAN"
            });
        }

        private void NoteDispenseFailed(object obj)
        {
            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
            {
                Event = "NoteDispenseFailed",
                DeviceType = Helpers.Enum.DeviceType.Bill,
                Amount = Convert.ToDecimal(obj, System.Globalization.CultureInfo.InvariantCulture),
                Code = "ENU"
            });
        }

        private void CoinDispenseCompleted(DispenseEventArgs obj)
        {
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
                        dispense.shoppingcartpkid = null;
                        dispense.source = "Cancel Cash Refill";
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
                        dispense.shoppingcartpkid = null;
                        dispense.source = "Cancel Cash Refill";
                        dispense.created_date_time = DateTime.Now;
                        dispense.group = group;
                        dispense.type = Helpers.Enum.DeviceType.Bill.ToString();
                        dispense.pkid = Guid.NewGuid().ToString();
                        repo.AddCashDispense(dispense);
                    }
                }
            }
            catch { }

            aggregator.GetEvent<EventAggregation.CashRefilCancelEvent>().Publish(null);
        }

        public override void Dispose()
        {

            if(Global.PaymentItems != null) { Global.PaymentItems.Clear();}
            Items.Clear();
            TotalRefillAmount = 0.0M;
            if(Global.MDBVendor.Contains("E2C")) { commType.CloseDevices(); }

            barcodeScanner.Close();
            aggregator.GetEvent<EventAggregation.BarcodeScannerDataLocalEvent>().Unsubscribe(ProcessBarcode);

            aggregator.GetEvent<EventAggregation.RightToolBarEnableEvent>().Publish(true);

            aggregator.GetEvent<EventAggregation.CoinAcceptedEvent>().Unsubscribe(CoinAccepted);
            aggregator.GetEvent<EventAggregation.BillAcceptedEvent>().Unsubscribe(BillAccepted);

            aggregator.GetEvent<EventAggregation.CoinDispenseCompleteEvent>().Unsubscribe(CoinDispenseCompleted);
            aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>().Unsubscribe(CoinDispenseFailed);
            aggregator.GetEvent<EventAggregation.NoteDispenseFailedEvent>().Unsubscribe(NoteDispenseFailed);
            aggregator.GetEvent<EventAggregation.NoteReaderInhibitedEvent>().Unsubscribe(ProcessNoteReaderInhibitedEvent);

            if (commType != null)
            {
                try
                {
                     commType.CloseDevices();
                }
                catch { }

                try
                {
                    if (Global.MDBVendor.Contains("E2C")) { commType.Close(); }
                }
                catch { }

                try
                {
                    if (Global.MDBVendor.Contains("E2C")) { commType.Dispose(); }
                }
                catch { }

                commType = null;
            }


        }
    }
}
