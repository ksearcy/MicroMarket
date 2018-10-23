using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.EventAggregation;
using deORO.Helpers;
using deORODataAccessApp.Models;
using deORO.USBRelay;
using MEI.CF7000Coin;
using Microsoft.Practices.Composite.Events;
using MPOST;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using System.ComponentModel;

namespace deORO.ViewModels
{
    public class CashPaymentViewModel : BaseViewModel
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand, (() => { return CanExecuteCancel; })); } }


        public ICommand CreditToCommand { get { return new DelegateCommand(ExecuteCreditToCommandCommand); } }
        public ICommand LoginCommand { get { return new DelegateCommand(ExecuteLoginCommand); } }

        private ObservableCollection<PaymentItem> items = new ObservableCollection<PaymentItem>();

        deORO.Communication.ICommunicationType commType;
        //private IDialogService dialogService = new MessageBoxViewService();
        //private IDialogService popupViewService = new PopupViewService(350, 350);
        readonly static DispenseChangeViewModel dispenseChange = new DispenseChangeViewModel();

        private bool paymentComplete;
        private bool createAccountVisible;
        private bool loginVisible;


        public bool LoginVisible
        {
            get { return loginVisible; }
            set { loginVisible = value; RaisePropertyChanged(() => LoginVisible); }
        }

        private decimal amountDueBeforeRounding;

        private string titleText;

        public string TitleText
        {
            get { return titleText; }
            set { titleText = value; RaisePropertyChanged(() => TitleText); }
        }

        private string row1Height;

        public string Row1Height
        {
            get { return row1Height; }
            set { row1Height = value; RaisePropertyChanged(() => Row1Height); }
        }
        private string row2Height;

        public string Row2Height
        {
            get { return row2Height; }
            set { row2Height = value; RaisePropertyChanged(() => Row2Height); }
        }
        private string row3Height;

        public string Row3Height
        {
            get { return row3Height; }
            set { row3Height = value; RaisePropertyChanged(() => Row3Height); }
        }

        private int rowCount;

        public int RowCount
        {
            get { return rowCount; }
            set { rowCount = value; RaisePropertyChanged(() => RowCount); }
        }

        public bool CreateAccountVisible
        {
            get { return createAccountVisible; }
            set { createAccountVisible = value; RaisePropertyChanged(() => CreateAccountVisible); }
        }

        private bool textVisible;

        public bool TextVisible
        {
            get { return textVisible; }
            set { textVisible = value; RaisePropertyChanged(() => TextVisible); }
        }

        private bool cancelVisible;

        public bool CancelVisible
        {
            get { return cancelVisible; }
            set { cancelVisible = value; RaisePropertyChanged(() => CancelVisible); }
        }

        private bool canExecuteCancel;

        public bool CanExecuteCancel
        {
            get { return canExecuteCancel; }
            set { canExecuteCancel = value; RaisePropertyChanged(() => CanExecuteCancel); }
        }

        public ObservableCollection<PaymentItem> Items
        {
            get
            {
                return items;
            }
        }

        private decimal amountPaid;

        private bool CalncelButtonPressedFlag;

        public decimal AmountPaid
        {
            get { return amountPaid; }
            set
            {
                amountPaid = value;
                RaisePropertyChanged(() => AmountPaid);
                Global.AmountPaid = AmountPaid;
            }
        }

        private decimal amountInCredit;

        public decimal AmountInCredit
        {
            get { return amountInCredit; }
            set
            {
                amountInCredit = value;
                RaisePropertyChanged(() => AmountInCredit);
                Global.AmountInCredit = AmountInCredit;
            }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; RaisePropertyChanged(() => Message); }
        }

        private string buttonColor;

        public string ButtonColor
        {
            get { return buttonColor; }
            set { buttonColor = value; RaisePropertyChanged(() => ButtonColor); }
        }

        private string creditToContent;

        public string CreditToContent
        {
            get { return creditToContent; }
            set { creditToContent = value; RaisePropertyChanged(() => CreditToContent); }
        }

        private decimal amountDue;

        public decimal AmountDue
        {
            get { return amountDue; }
            set
            {
                try
                {
                    if (Global.Round != 0.0m)
                        amountDue = Math.Ceiling(value / Global.Round) * Global.Round;
                    else
                        amountDue = value;
                }
                catch
                {
                    amountDue = value;
                }

                RaisePropertyChanged(() => AmountDue);
            }
        }

        public override void Init()
        {
            aggregator.GetEvent<EventAggregation.CoinAcceptedEvent>().Subscribe(CoinAccepted);
            aggregator.GetEvent<EventAggregation.CoinJamEvent>().Subscribe(ProcessCoinJamEvent);
            aggregator.GetEvent<EventAggregation.BillUnknownEvent>().Subscribe(ProcessBillUnknownEvent);
            aggregator.GetEvent<EventAggregation.BillJamEvent>().Subscribe(ProcessBillJamEvent);
            aggregator.GetEvent<EventAggregation.BillCheatingEvent>().Subscribe(ProcessBillCheatingEvent);
            aggregator.GetEvent<EventAggregation.BillAcceptedEvent>().Subscribe(BillAccepted);
            aggregator.GetEvent<EventAggregation.AmoutDueChangeEvent>().Subscribe(ProcessAmountDueChangeEvent);
            aggregator.GetEvent<EventAggregation.NoteReaderInhibitedEvent>().Subscribe(ProcessNoteReaderInhibitedEvent);

            //items.Clear();
            amountDueBeforeRounding = Global.AmountDue;
            AmountDue = Global.AmountDue;

            //AmountPaid = 0.0m;
            AmountPaid = Items.Sum(x => x.Payment);
            AmountInCredit = 0.0m;

            //SetCalncelButtonHitsCounter
            CalncelButtonPressedFlag = false;

            commType = deORO.Communication.CommunicationTypeFactory.GetCommunicationType();

            if (Global.PaymentOptions.Contains(Helpers.Enum.PaymentMethod.CoinPay.ToString()))
                commType.EnableCoins();

            if (Global.PaymentOptions.Contains(Helpers.Enum.PaymentMethod.BillPay.ToString()))
                commType.EnableBills(amountDue, Global.NoteReaderNoteSetPayment, "Purchase");


            Row1Height = "150";
            Row2Height = "*";
            Row3Height = "150";

            RowCount = 1;
            Message = LocalizationProvider.GetLocalizedValue<string>("CashPayment.CancelTransaction");

            CreateAccountVisible = false;
            LoginVisible = false;

            paymentComplete = false;
            TextVisible = false;
            CanExecuteCancel = true;
            CancelVisible = true;
            ButtonColor = "Red";

            items.CollectionChanged += items_CollectionChanged;
            Global.PaymentItems = items.ToList();

            base.Init();

            //==============ADD THIS SIMULATE A COIN==========
            //SimulateCoin(0.05m);         

            //Task.Factory.StartNew(() =>
            //    {
            //        System.Threading.Thread.Sleep(5000);
            //        aggregator.GetEvent<EventAggregation.CoinAcceptedEvent>().Publish(new CashEventArgs { Amount = 0.05m, Routing = "CashBox" });
            //    });
        }

        void items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            if (items.Count > 0)
                CanExecuteCancel = true;
            else
                CanExecuteCancel = true;

            Global.PaymentItems = items.ToList();
        }

        private void ProcessCoinJamEvent(object obj)
        {
            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
            {
                Event = "CoinJam",
                DeviceType = Helpers.Enum.DeviceType.Coin,
                Code = "EAD"
            });
        }

        private void ProcessNoteReaderInhibitedEvent(object obj)
        {
            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
            {
                Event = "NoteReaderInhibited",
                DeviceType = Helpers.Enum.DeviceType.Bill,
                Code = "ENK"
            });

            aggregator.GetEvent<EventAggregation.PaymentMethodCancelEvent>().Publish(null);
        }

        private void ProcessAmountDueChangeEvent(decimal obj)
        {

            AmountDue = obj - AmountPaid;
            Global.AmountDue = obj - AmountPaid;

            if (Global.ShoppingCartItemsCount > 0)
            {
                if (AmountDue < 0)
                {
                    AmountInCredit = AmountDue;
                    PaymentComplete(-AmountInCredit);

                    aggregator.GetEvent<EventAggregation.AmoutDueChangeEvent>().Unsubscribe(ProcessAmountDueChangeEvent);
                }
                else if (AmountDue == 0)
                {
                    PaymentComplete();
                    aggregator.GetEvent<EventAggregation.AmoutDueChangeEvent>().Unsubscribe(ProcessAmountDueChangeEvent);
                }
            }
        }

        private void ProcessBillCheatingEvent(object obj)
        {
            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
            {
                Event = "BillCheating",
                DeviceType = Helpers.Enum.DeviceType.Bill,
                Code = "ENU"
            });
        }

        private void ProcessBillJamEvent(object obj)
        {
            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
            {
                Event = "BillJam",
                DeviceType = Helpers.Enum.DeviceType.Bill,
                Code = "ENA"
            });
        }

        private void ProcessBillUnknownEvent(object obj)
        {
            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
            {
                Event = "BillUnknown",
                DeviceType = Helpers.Enum.DeviceType.Bill,
                Code = "ENU"
            });
        }

        private void ExecuteLoginCommand()
        {
            //IDialogService popup = new PopupViewService(750, 680);
            LoginViewModel viewModel = new LoginViewModel();
            viewModel.EnableCancel = false;
            Global.CreditToAccount = -AmountInCredit;
            DialogViewService.ShowAutoCloseDialog(viewModel, 750, 680);

            Global.CreditToAccount = 0.0m;
            App.Current.Dispatcher.Invoke(() =>
            {
                items.Add(new PaymentItem
                {
                    DateTime = DateTime.Now,
                    Payment = -AmountInCredit,
                    Routing = Helpers.Enum.PaymentMethod.MyAccountPay.ToString(),
                    Source = "Purchase Complete"
                });
            });

            aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Publish(new PaymentCompleteEventArgs()
            {
                Source = "Cash / Coin",
                PaymentItems = items.ToList(),
            });

        }

        private void ExecuteCreditToCommandCommand()
        {
            if (Global.User != null) //Credit to Account
            {
                deOROMembershipProvider userProvider = new deOROMembershipProvider();
                userProvider.UpdateUserBalance(Global.User.UserName, -AmountInCredit, "CreditToMyAccount");
                Global.User = userProvider.GetUser(Global.User.UserName) as deOROMembershipUser;

                App.Current.Dispatcher.Invoke(() =>
                {
                    items.Add(new PaymentItem
                    {
                        DateTime = DateTime.Now,
                        Payment = -AmountInCredit,
                        Routing = Helpers.Enum.PaymentMethod.MyAccountPay.ToString(),
                        Source = "Purchase Complete"
                    });
                });

                aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Publish(new PaymentCompleteEventArgs()
                {
                    Source = "Cash / Coin",
                    PaymentItems = items.ToList(),
                });
            }
            else //Create New Account
            {
                //IDialogService popup = new PopupViewService(750, 680);
                UserRegistrationViewModel viewModel = new UserRegistrationViewModel();
                Global.CreditToAccount = -AmountInCredit;
                DialogViewService.ShowDialog(viewModel, 750, 680);

                //if (!popup.CancelDialog)
                //{
                //    App.Current.Dispatcher.Invoke(() =>
                //    {
                //        items.Add(new PaymentItem
                //        {
                //            DateTime = DateTime.Now,
                //            Payment = -AmountInCredit,
                //            Routing = Helpers.Enum.PaymentMethod.MyAccountPay.ToString(),
                //            Source = "Purchase Complete"
                //        });
                //    });

                //    aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Publish(new PaymentCompleteEventArgs()
                //    {
                //        Source = "Cash / Coin",
                //        PaymentItems = items.ToList(),
                //    });
                //}

                //Todo: Revist
                App.Current.Dispatcher.Invoke(() =>
                {
                    items.Add(new PaymentItem
                    {
                        DateTime = DateTime.Now,
                        Payment = -AmountInCredit,
                        Routing = Helpers.Enum.PaymentMethod.MyAccountPay.ToString(),
                        Source = "Purchase Complete"
                    });
                });

                aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Publish(new PaymentCompleteEventArgs()
                {
                    Source = "Cash / Coin",
                    PaymentItems = items.ToList(),
                });

            }

        }

        //==============ADD THIS METHOD IN INT() TO SIMULATE A COIN==========
        private void SimulateCoin(decimal SimulatedAmount = 1) {
           
            PaymentItem item = new PaymentItem();
            item.Source = Helpers.Enum.PaymentMethod.CoinPay.ToString();
            item.Payment = Convert.ToDecimal(SimulatedAmount, System.Globalization.CultureInfo.InvariantCulture);
            item.DateTime = DateTime.Now;
            item.Routing = "CoinPay";

            App.Current.Dispatcher.Invoke(() =>
            {
                items.Add(item);
            });

            AmountPaid += SimulatedAmount;
            Global.AmountDue -= SimulatedAmount;
            AmountDue -= SimulatedAmount;

            if (AmountDue < 0)
            {
                AmountInCredit = AmountDue;
                PaymentComplete(-AmountInCredit);
            }
            else if (AmountDue == 0)
            {
                PaymentComplete();
            }
        
        }

        private void ExecuteCancelCommand()
        {
            if (CalncelButtonPressedFlag == false)
            {
                CalncelButtonPressedFlag = true;
                CanExecuteCancel = false;
                Global.AmountDue += AmountPaid;
                //Global.AmountDue = amountDueBeforeRounding;      

                if (paymentComplete)
                {
                    if (Global.EnableCoin && Global.EnableDispenseChange)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            items.Add(new PaymentItem
                            {
                                DateTime = DateTime.Now,
                                Payment = AmountInCredit,
                                Routing = "Dispense",
                                Source = "Purchase Complete"
                            });
                        });
                    }

                    aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Publish(new PaymentCompleteEventArgs()
                    {
                        Source = "Cash / Coin",
                        PaymentItems = items.ToList(),
                        Change = -AmountInCredit,
                    });
                }
                else
                {
                    App.Current.Dispatcher.Invoke(() =>
                   {
                       items.Clear();
                   });

                    aggregator.GetEvent<EventAggregation.CashPaymentCancelEvent>().Publish(AmountPaid);

                }
            }       

        }

        private void CoinAccepted(CashEventArgs args)
        {
            System.Threading.Thread.Sleep(500);

            if (AmountDue < 0)
            {
                try
                {
                    commType = deORO.Communication.CommunicationTypeFactory.GetCommunicationType();
                    commType.EnableCoins();
                    commType.DispenseChange(-AmountDue);
                    commType.CloseDevices();
                    commType.Dispose();
                }
                catch { }

                return;
            }

            PaymentItem item = new PaymentItem();
            item.Source = Helpers.Enum.PaymentMethod.CoinPay.ToString();
            item.Payment = Convert.ToDecimal(args.Amount, System.Globalization.CultureInfo.InvariantCulture);
            item.DateTime = DateTime.Now;
            item.Routing = args.Routing;

            App.Current.Dispatcher.Invoke(() =>
            {
                items.Add(item);
            });

            AmountPaid += args.Amount;
            Global.AmountDue -= args.Amount;
            AmountDue -= args.Amount;

            if (AmountDue < 0)
            {
                AmountInCredit = AmountDue;
                PaymentComplete(-AmountInCredit);
            }
            else if (AmountDue == 0)
            {
                PaymentComplete();
            }
        }

        private void BillAccepted(CashEventArgs args)
        {
            PaymentItem item = new PaymentItem();
            item.Source = Helpers.Enum.PaymentMethod.BillPay.ToString();
            item.Payment = args.Amount;
            item.DateTime = DateTime.Now;
            item.Routing = args.Routing;

            App.Current.Dispatcher.Invoke(() =>
            {
                items.Add(item);
            });


            AmountPaid += args.Amount;
            Global.AmountDue -= args.Amount;
            AmountDue -= args.Amount;

            if (AmountDue < 0)
            {
                AmountInCredit = AmountDue;
                PaymentComplete(-AmountInCredit);
            }
            else if (AmountDue == 0)
            {
                PaymentComplete();
            }
        }

        private void PaymentComplete(decimal change = 0)
        {
            if (change != 0.0m)
            {
                AmountPaid = 0;
                Global.CreditToAccount = AmountInCredit;
                Global.PaymentArgs = new PaymentCompleteEventArgs()
                {
                    Source = "Cash / Coin",
                    PaymentItems = items.ToList(),
                    Change = -AmountInCredit,
                };

                aggregator.GetEvent<EventAggregation.ReturnChangeOptionsEvent>().Publish(null);

            }
            else
            {
                aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Publish(new PaymentCompleteEventArgs()
                {
                    Source = "Cash / Coin",
                    PaymentItems = items.ToList(),
                    Change = change,
                });
            }
        }

        public override void Dispose()
        {
            if (Global.PaymentItems != null) { Global.PaymentItems.Clear(); }
            aggregator.GetEvent<EventAggregation.CoinAcceptedEvent>().Unsubscribe(CoinAccepted);
            aggregator.GetEvent<EventAggregation.CoinJamEvent>().Unsubscribe(ProcessCoinJamEvent);

            aggregator.GetEvent<EventAggregation.BillAcceptedEvent>().Unsubscribe(BillAccepted);
            aggregator.GetEvent<EventAggregation.BillUnknownEvent>().Unsubscribe(ProcessBillUnknownEvent);
            aggregator.GetEvent<EventAggregation.BillJamEvent>().Unsubscribe(ProcessBillJamEvent);
            aggregator.GetEvent<EventAggregation.BillCheatingEvent>().Unsubscribe(ProcessBillCheatingEvent);

            aggregator.GetEvent<EventAggregation.AmoutDueChangeEvent>().Unsubscribe(ProcessAmountDueChangeEvent);
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
                    commType.Close();
                }
                catch { }

                commType = null;
            }

        }
    }
}

