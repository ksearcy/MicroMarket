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
    public class BitcoinViewModel : BaseViewModel
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand, CanExecuteCancelCommand); } }

        //private IDialogService dialogService = new MessageBoxViewService();
        
        private Timer timer;
        private decimal amountDue;
        private decimal amountDueBeforeRounding;
        private decimal creditRoundFee;
        


        public decimal AmountDue
        {
            get { return amountDue; }
            set
            {
                try
                {
                    if (Global.CreditRound != 0.0m)
                    {
                        amountDue = value + (value * (Global.CreditRound / 100));
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

        
        public override void Init()
        {

            amountDueBeforeRounding = Global.AmountDue;
            AmountDue = Global.AmountDue;
            if (Helpers.Global.AmountDue != 0)
            {
                

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

        private async void BitcoinData(CreditCardData cardData)
        {

            string transactionDetails = string.Format(new System.Globalization.CultureInfo("en-US"), "A0|200|{0}|1|{1}", Global.AmountDue * 100, "Purchase using Credit Card");
            await Task.Run(() =>
            {
                if (Helpers.Global.RunMode.ToLower() == "debug")
                {
                
                }
                else
                {
                    
                }

            });

            
        }

        private void ProcessTimeout(object obj)
        {
            if (obj == "credit")
            {
                aggregator.GetEvent<EventAggregation.PopupTimeoutEvent>().Unsubscribe(ProcessTimeout);
                aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Unsubscribe(BitcoinData);
                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Unsubscribe(ProcessBitcoinTransactionEvent);
                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                {
                    Event = "BitcoinFailed",
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
                aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Unsubscribe(BitcoinData);
                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                {
                    Event = "BitcoinFailed",
                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                    Amount = Global.AmountDue,
                    Code = "EKE"
                });
            }
        }

        private void ProcessBitcoinTransactionEvent(string parameter)
        {
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);

            if (parameter.Equals("Approved"))
            {
                aggregator.GetEvent<EventAggregation.CreditCardTransactionCompleteEvent>().Publish(Global.AmountDue);

                List<PaymentItem> items = new List<PaymentItem>();
                items.Add(new PaymentItem
                {
                    Source = "Bitcoin",
                    Payment = Global.AmountDue,
                    DateTime = DateTime.Now
                });

                aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Publish(new PaymentCompleteEventArgs()
                {
                    Source = "Bitcoin",
                    PaymentItems = items
                });
            }
            else
            {
                aggregator.GetEvent<EventAggregation.CreditCardTransactionFailedEvent>().Publish(parameter);
            }

        }

        private void BitcoinError(object parameter)
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
            aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Unsubscribe(BitcoinData);
            aggregator.GetEvent<EventAggregation.CreditCardReaderErrorEvent>().Unsubscribe(BitcoinError);
            aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Unsubscribe(ProcessBitcoinTransactionEvent);
            aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Unsubscribe(SetParamsFailError);
            aggregator.GetEvent<EventAggregation.AmoutDueChangeEvent>().Unsubscribe(ProcessAmountDueChangeEvent);

            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

        }

    }
}
