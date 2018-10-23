using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using deORO.CardProcessor;
using deORO.EventAggregation;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    public class CreditCardRefilViewModel : BaseViewModel
    {
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand, CanExecuteCancelCommand); } }
        public ICommand OKCommand { get { return new DelegateCommand(ExecuteOKCommand, CanExecuteOKCommand); } }
        public ICommand SelectOptionCommand { get { return new DelegateCommandWithParam(ExecuteSelectOptionCommand); } }
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        private decimal selectedOption = 0.0M;
        private decimal creditAddRefill = Global.CreditAddRefill;



        private deORO.CardReader.ICardReader cardReader = null;
        private deORO.CardProcessor.ICreditCardProcessor cardProcessor = null;

        //private IDialogService dialogService = new MessageBoxViewService();
        private Timer timer;

        public decimal SelectedOption
        {
            get { return selectedOption; }
            set
            {
                selectedOption = value;
                RaisePropertyChanged(() => SelectedOption);
            }
        }

        bool createAccountVisible;

        public bool CreateAccountVisible
        {
            get { return createAccountVisible; }
            set { createAccountVisible = value; RaisePropertyChanged(() => CreateAccountVisible); }
        }

        bool creditAddRefillVisible;

        public bool CreditAddRefillVisible
        {
            get { return creditAddRefillVisible; }
            set { creditAddRefillVisible = value; RaisePropertyChanged(() => CreditAddRefillVisible); }
        }

        public decimal CreditAddRefill
        {
            get { return creditAddRefill; }
            set
            {
                creditAddRefill = value;
                RaisePropertyChanged(() => CreditAddRefill);
            }
        }

        private bool canExecuteOK = false;

        public bool CanExecuteOK
        {
            get { return canExecuteOK; }
            set
            {
                canExecuteOK = value;
                RaisePropertyChanged(() => CanExecuteOK);
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

        private string message = "OK";

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                RaisePropertyChanged(() => Message);
            }
        }

        private bool fiveChecked = false;

        public bool FiveChecked
        {
            get { return fiveChecked; }
            set { fiveChecked = value; RaisePropertyChanged(() => FiveChecked); }
        }
        private bool tenChecked = false;

        public bool TenChecked
        {
            get { return tenChecked; }
            set { tenChecked = value; RaisePropertyChanged(() => TenChecked); }
        }
        private bool twentyChecked = false;

        public bool TwentyChecked
        {
            get { return twentyChecked; }
            set { twentyChecked = value; RaisePropertyChanged(() => TwentyChecked); }
        }
        private bool fiftyChecked = false;

        public bool FiftyChecked
        {
            get { return fiftyChecked; }
            set { fiftyChecked = value; RaisePropertyChanged(() => FiftyChecked); }
        }
        private bool hundredChecked = false;

        public bool HundredChecked
        {
            get { return hundredChecked; }
            set { hundredChecked = value; RaisePropertyChanged(() => HundredChecked); }
        }

        public bool IsZipRequired
        {
            get
            {
                return Global.ZipCodeRequired;
            }
        }

        public override void Init()
        {
            cardReader = CardReader.CardReaderFactory.GetCreditCardReader();
            cardProcessor = CreditCardProcessorFactory.CreditCardProcessorFactory.GetCreditCardProcessor();
            //Temp Code
            try
            {
                aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Unsubscribe(CardReaderData);
            }
            catch { }

            aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Subscribe(CardReaderData);
            aggregator.GetEvent<EventAggregation.CreditCardReaderErrorEvent>().Subscribe(CardReaderError);
            aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Subscribe(SetParamsFailError);
            aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Subscribe(ProcessCreditCardTransactionEvent);

            if (Global.CreditAddRefill > 0)
            {
                CreditAddRefillVisible = false;
            }

            FiveChecked = false;
            TenChecked = false;
            TwentyChecked = false;
            FiftyChecked = false;
            HundredChecked = false;

            CanExecuteCancel = true;
            CanExecuteOK = false;
            SelectedOption = 0.0M;
            Message = "OK";

            cardReader.Open();
            base.Init();
        }

        private void SetParamsFailError(object obj)
        {
            DialogViewService.ShowAutoCloseDialog("Credit Card Reader", obj.ToString());
        }

        private void CardReaderError(object parameter)
        {
            DialogViewService.ShowAutoCloseDialog("Credit Card Reader", "Credit Card Reader Error. Please press OK and retry");
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


            string transactionDetails = string.Format(new System.Globalization.CultureInfo("en-US"), "A0|200|{0}|1|{1}", (selectedOption * 100) + Global.CreditAddRefill, "Account Refill using Credit Card");

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

            await Task.Run(() =>
            {

                if (Helpers.Global.RunMode.ToLower() == "debug")
                {
                    cardProcessor.ProcessSale(cardData, (decimal)0.01, transactionDetails, zipCode);
                }
                else
                {
                    cardProcessor.ProcessSale(cardData, selectedOption + Global.CreditAddRefill, transactionDetails, zipCode);
                }

            });

            if (!((Global.CardReaderMake.Equals("CardKnox") || Global.CardReaderMake.Equals("NayaxMarshall"))))
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    BusyViewModel busyView = new BusyViewModel();
                    busyView.Message = LocalizationProvider.GetLocalizedValue<string>("CreditCardPayment.ProcessingTransaction");
                    //DialogViewService.ShowDialog(busyView, 250, 100);
                    aggregator.GetEvent<EventAggregation.PopupTimeoutEvent>().Subscribe(ProcessTimeout);
                    DialogViewService.ShowAutoCloseDialog(busyView, 250, 100, "credit");
                    
                });
            }
        }

        private void ProcessTimeout(object obj)
        {
            if (obj == "credit")
            {
                aggregator.GetEvent<EventAggregation.PopupTimeoutEvent>().Unsubscribe(ProcessTimeout);
                aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Unsubscribe(CardReaderData);
                //---------------If we use the following code the CreditCardTransactionFailed will be infinite in case of processing timeout------------------
                //aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Unsubscribe(ProcessCreditCardTransactionEvent);
                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                {
                    Event = "CreditCardTransactionFailed - Timeout",
                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                    Amount = selectedOption,
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
                    Amount = selectedOption,
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

                aggregator.GetEvent<EventAggregation.CreditCardTransactionCompleteEvent>().Publish(selectedOption);
            }
            else
            {
                aggregator.GetEvent<EventAggregation.CreditCardTransactionFailedEvent>().Publish(selectedOption);
            }
        }

        private void ExecuteCancelCommand()
        {

            FiveChecked = false;
            TenChecked = false;
            TwentyChecked = false;
            FiftyChecked = false;
            HundredChecked = false;

            CanExecuteCancel = true;
            CanExecuteOK = false;
            SelectedOption = 0.0M;
            Message = LocalizationProvider.GetLocalizedValue<string>("Billing.Ok");

            aggregator.GetEvent<EventAggregation.CreditCardRefilCancelEvent>().Publish(null);
        }

        private void ExecuteOKCommand()
        {
            CanExecuteOK = false;

            //Message = "Please Scan your Credit Card";
            Message = LocalizationProvider.GetLocalizedValue<string>("Billing.PleaseScan");
            cardReader.SetParams(selectedOption + Global.CreditAddRefill);

        }

        private bool CanExecuteOKCommand()
        {
            return canExecuteOK;
        }

        private bool CanExecuteCancelCommand()
        {
            return canExecuteCancel;
        }

        private void ExecuteSelectOptionCommand(object parameter)
        {

            switch (parameter.ToString())
            {
                case "5.00":
                    {
                        FiveChecked = true;
                        break;
                    }
                case "10.00":
                    {
                        TenChecked = true;
                        break;
                    }
                case "20.00":
                    {
                        TwentyChecked = true;
                        break;
                    }
                case "50.00":
                    {
                        FiftyChecked = true;
                        break;
                    }
                case "100.00":
                    {
                        HundredChecked = true;
                        break;
                    }
            }

            selectedOption = Convert.ToDecimal(parameter, System.Globalization.CultureInfo.InvariantCulture);
            canExecuteOK = true;
            Message = "OK";
            cardReader.Close();
        }

        public override void Dispose()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }

            cardReader.Close();

            aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Unsubscribe(CardReaderData);
            aggregator.GetEvent<EventAggregation.CreditCardReaderErrorEvent>().Unsubscribe(CardReaderError);
            aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Unsubscribe(ProcessCreditCardTransactionEvent);
            aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Unsubscribe(SetParamsFailError);

            aggregator.GetEvent<EventAggregation.PopupTimeoutEvent>().Unsubscribe(ProcessTimeout);
            //ExecuteCancelCommand();
        }

    }
}
