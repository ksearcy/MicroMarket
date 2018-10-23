using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using deORO.CardProcessor;
using deORO.EventAggregation;
using deORO.Helpers;
using deORODataAccessApp.Models;
using Microsoft.Practices.Composite.Events;
using deORO.CardReader;

namespace deORO.ViewModels
{
    public class CreditCardPaymentViewModel : BaseViewModel
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand, CanExecuteCancelCommand); } }

        //private IDialogService dialogService = new MessageBoxViewService();
        private deORO.CardReader.ICardReader cardReader = null;
        private deORO.CardProcessor.ICreditCardProcessor cardProcessor = null;

        private Timer timer;
        private decimal amountDue;
        private decimal amountDueBeforeRounding;
        private decimal creditRoundFee;

        public decimal CreditRoundFee
        {
            get { return creditRoundFee; }
            set
            {
                creditRoundFee = Math.Round((value + (value * (Global.CreditRound / 100))), 2);
                RaisePropertyChanged(() => creditRoundFee);
            }
        }

        public decimal AmountDue
        {
            get { return amountDue; }
            set
            {
                try
                {
                    if (Global.CreditRound != 0.0m)
                    {
                        amountDue = Math.Round((value + (value * (Global.CreditRound / 100))), 2);
                        creditRoundFee = (value * (Global.CreditRound / 100));
                    }
                    else
                    {
                        amountDue = value;
                        creditRoundFee = 0;
                    }
                }
                catch
                {
                    amountDue = value;
                }

                RaisePropertyChanged(() => AmountDue);
            }
        }

        private string zipCode;

        public string ZipCode
        {
            get { return zipCode; }
            set
            {
                zipCode = value;
                RaisePropertyChanged(() => ZipCode);
            }
        }

        private bool canExecuteCancel = true;

        public bool CanExecuteCancel
        {
            get { return canExecuteCancel; }
            set
            {
                canExecuteCancel = value;
                RaisePropertyChanged(() => CanExecuteCancel);
            }
        }

        public bool IsZipRequired
        {
            get
            {
                return Global.ZipCodeRequired;
            }
        }

        private bool subscribedTimeout = false;
        //public override void Init()
        //{
        //    cardReader = CardReader.CardReaderFactory.GetCreditCardReader();
        //    cardProcessor = CreditCardProcessorFactory.CreditCardProcessorFactory.GetCreditCardProcessor();

        //    amountDueBeforeRounding = Global.AmountDue;
        //    AmountDue = Global.AmountDue;
        //    if (Helpers.Global.AmountDue != 0)
        //    {
        //        try
        //        {
        //            aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Unsubscribe(CardReaderData);
        //        }
        //        catch { }

        //        aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Subscribe(CardReaderData);
        //        aggregator.GetEvent<EventAggregation.CreditCardReaderErrorEvent>().Subscribe(CardReaderError);
        //        aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Subscribe(ProcessCreditCardTransactionEvent);
        //        aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Subscribe(SetParamsFailError);
        //        aggregator.GetEvent<EventAggregation.AmoutDueChangeEvent>().Subscribe(ProcessAmountDueChangeEvent);

        //        cardReader.Open();
        //        cardReader.SetParams(Helpers.Global.AmountDue);

        //    }

        //    base.Init();
        //}

        public override void Init()
        {
            
            cardReader = CardReader.CardReaderFactory.GetCreditCardReader();
            cardProcessor = CreditCardProcessorFactory.CreditCardProcessorFactory.GetCreditCardProcessor();

            amountDueBeforeRounding = Global.AmountDue;
            AmountDue = Global.AmountDue;
            CreditRoundFee = (AmountDue * (Global.CreditRound / 100));

            if (Helpers.Global.AmountDue != 0)
            {
                try
                {
                    aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Unsubscribe(CardReaderData);
                }
                catch { }

                aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Subscribe(CardReaderData);
                aggregator.GetEvent<EventAggregation.CreditCardReaderErrorEvent>().Subscribe(CardReaderError);
                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Subscribe(ProcessCreditCardTransactionEvent);
                aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Subscribe(SetParamsFailError);
                aggregator.GetEvent<EventAggregation.AmoutDueChangeEvent>().Subscribe(ProcessAmountDueChangeEvent);
                cardReader.SetParams(AmountDue);

            }

            base.Init();
        }

        private void ProcessAmountDueChangeEvent(decimal obj)
        {
            ExecuteCancelCommand();
        }

        private void SetParamsFailError(object obj)
        {
            DialogViewService.ShowAutoCloseDialog("Credit Card Reader", obj.ToString());
            aggregator.GetEvent<EventAggregation.CreditCardPaymentCancelEvent>().Publish(null);
        }

        private async void CardReaderData(CreditCardData cardData)
        {

            if (IsZipRequired && ZipCode == null)
            {
                DialogViewService.ShowAutoCloseDialog(
                        LocalizationProvider.GetLocalizedValue<string>("CreditCardPayment.DialogTitle"),
                        LocalizationProvider.GetLocalizedValue<string>("CreditCardPayment.ErrorZip")
                    );
                return;
            }

            //timer = new Timer();
            //try
            //{
            //    timer.Interval = Helpers.Global.CardProcessorTimeout * 1000;
            //}
            //catch
            //{
            //    timer.Interval = 60000;
            //}
            //timer.Elapsed += timer_Elapsed;
            //timer.Start();

            //CULTURE INFO SET TO English because it was causing problems with USAT transactions if the languague selected was different
            string transactionDetails = string.Format(new System.Globalization.CultureInfo("en-US"),"A0|200|{0}|1|{1}", AmountDue * 100, "Purchase using Credit Card");
            await Task.Run(() =>
            {
                if (Helpers.Global.RunMode.ToLower() == "debug")
                {
                    cardProcessor.ProcessSale(cardData, (decimal)0.01, transactionDetails, zipCode);
                }
                else
                {
                    cardProcessor.ProcessSale(cardData, AmountDue, transactionDetails, zipCode);
                }

            });

            if (!((Global.CardReaderMake.Equals("CardKnox") || Global.CardReaderMake.Equals("NayaxMarshall"))))
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (!this.subscribedTimeout)
                    {
                        BusyViewModel busyView = new BusyViewModel();
                        busyView.Message = LocalizationProvider.GetLocalizedValue<string>("CreditCardPayment.ProcessingTransaction");
                        //DialogViewService.ShowDialog(busyView, 250, 100);
                        this.subscribedTimeout = true;
                        aggregator.GetEvent<EventAggregation.PopupTimeoutEvent>().Subscribe(ProcessTimeout);
                        DialogViewService.ShowAutoCloseDialog(busyView, 250, 100, "credit");
                    }
                });
            }

        }

        private void ProcessTimeout(object obj)
        {
            if (obj == "credit")
            {
                aggregator.GetEvent<EventAggregation.PopupTimeoutEvent>().Unsubscribe(ProcessTimeout);
                this.subscribedTimeout = false;
                aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Unsubscribe(CardReaderData);
                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Unsubscribe(ProcessCreditCardTransactionEvent);
                //aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
                aggregator.GetEvent<EventAggregation.CreditCardTransactionFailedEvent>().Publish("Failed");
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                {
                    Event = "CreditCardTransactionFailed - Timeout",
                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                    Amount = Global.AmountDue,
                    Code = "EKE"
                });
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (timer != null)
            {
                aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Unsubscribe(CardReaderData);
                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                {
                    Event = "CreditCardTransactionFailed",
                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                    Amount = Global.AmountDue,
                    Code = "EKE"
                });
            }
        }

        private void ProcessCreditCardTransactionEvent(string parameter)
        {
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);

            if (parameter.Equals("Approved"))
            {
                if (Global.CardReaderMake.Equals("IDTECH"))
                {
                   Global.IDTECHReaderBusyCount = 0;
                }

                aggregator.GetEvent<EventAggregation.CreditCardTransactionCompleteEvent>().Publish(Global.AmountDue);

                List<PaymentItem> items = new List<PaymentItem>();
                items.Add(new PaymentItem
                {
                    Source = Helpers.Enum.PaymentMethod.CreditCardPay.ToString(),
                    Payment = Global.AmountDue,
                    DateTime = DateTime.Now
                });
                if (Global.CreditRound != 0)
                {
                    items.Add(new PaymentItem
                    {
                        Source = Helpers.Enum.PaymentMethod.CreditCardPay.ToString(),
                        Payment = creditRoundFee,
                        DateTime = DateTime.Now,
                        Routing = "CreditRoundFee"
                    });
                }

                aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Publish(new PaymentCompleteEventArgs()
                {
                    Source = Helpers.Enum.PaymentMethod.CreditCardPay.ToString(),
                    PaymentItems = items
                });

                //====WE MUST UNSUBSCRIBE IF WE GET AN APPROVED TRANSACTION,IF WE DON'T THE subscribedTimeout VARIABLE ALWAYS WILL BE TRUE AND THE LAST "IF" IN THE CardReaderData METHOD NEVER WILL BE EXECUTED AGAIN AFTER THE FIRST TRANSACTION 
                aggregator.GetEvent<EventAggregation.PopupTimeoutEvent>().Unsubscribe(ProcessTimeout);
                this.subscribedTimeout = false;
            }
            else
            {
                aggregator.GetEvent<EventAggregation.CreditCardTransactionFailedEvent>().Publish(parameter);

                //====WE MUST UNSUBSCRIBE IF WE GET AN ERROR, IF WE DON'T THE subscribedTimeout VARIABLE ALWAYS WILL BE TRUE AND THE LAST "IF" IN THE CardReaderData METHOD NEVER WILL BE EXECUTED AGAIN AFTER THE FIRST TRANSACTION 
                aggregator.GetEvent<EventAggregation.PopupTimeoutEvent>().Unsubscribe(ProcessTimeout);
                this.subscribedTimeout = false;
            }

        }

        private void CardReaderError(object parameter)
        {
            DialogViewService.ShowAutoCloseDialog(
                LocalizationProvider.GetLocalizedValue<string>("CreditCardPayment.DialogTitle"),
                LocalizationProvider.GetLocalizedValue<string>("CreditCardPayment.CardReadError")
                );
        }

        private void ExecuteCancelCommand()
        {
            aggregator.GetEvent<EventAggregation.CreditCardPaymentCancelEvent>().Publish(null);
        }

        private bool CanExecuteCancelCommand()
        {
            return canExecuteCancel;
        }

        public override void Dispose()
        {
            aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Unsubscribe(CardReaderData);
            aggregator.GetEvent<EventAggregation.CreditCardReaderErrorEvent>().Unsubscribe(CardReaderError);
            aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Unsubscribe(ProcessCreditCardTransactionEvent);
            aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Unsubscribe(SetParamsFailError);
            aggregator.GetEvent<EventAggregation.AmoutDueChangeEvent>().Unsubscribe(ProcessAmountDueChangeEvent);

            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            cardReader.Close();
        }

    }
}
