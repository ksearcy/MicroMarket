using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.Helpers;
using deORODataAccessApp.DataAccess;
using deORODataAccessApp.Models;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    class ReturnChangeOptionsViewModel : BaseViewModel
    {

        public ICommand DispenseChangeCommand { get { return new DelegateCommand(ExecuteDispenseChangeCommand); } }
        public ICommand CreditChangeCommand { get { return new DelegateCommandWithParam(ExecuteCreditChangeCommand); } }
        public ICommand LoginCreditChangeCommand { get { return new DelegateCommand(ExecuteLoginCreditChangeCommand); } }
        public ICommand CreateAccountCommand { get { return new DelegateCommand(ExecuteCreateAccountCommand); } }
        public ICommand NoChangeCommand { get { return new DelegateCommand(ExecuteNoChangeCommand); } }
              

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        string titleText;

        public string TitleText
        {
            get { return titleText; }
            set { titleText = value; RaisePropertyChanged(() => TitleText); }
        }

        bool dispenseChangeVisible;

        public bool DispenseChangeVisible
        {
            get { return dispenseChangeVisible; }
            set { dispenseChangeVisible = value; RaisePropertyChanged(() => DispenseChangeVisible); }
        }
        bool creditChangeVisible;

        public bool CreditChangeVisible
        {
            get { return creditChangeVisible; }
            set { creditChangeVisible = value; RaisePropertyChanged(() => CreditChangeVisible); }
        }
        bool loginCreditChangeVisible;

        public bool LoginCreditChangeVisible
        {
            get { return loginCreditChangeVisible; }
            set { loginCreditChangeVisible = value; RaisePropertyChanged(() => LoginCreditChangeVisible); }
        }
        bool createAccountVisible;

        public bool CreateAccountVisible
        {
            get { return createAccountVisible; }
            set { createAccountVisible = value; RaisePropertyChanged(() => CreateAccountVisible); }
        }

        bool noChangeVisible;

        public bool NoChangeVisible
        {
            get { return noChangeVisible; }
            set { noChangeVisible = value; RaisePropertyChanged(() => NoChangeVisible); }
        }

        public override void Init()
        {

             aggregator.GetEvent<EventAggregation.CoinJamEvent>().Subscribe(ProcessCoinJamEvent);
             aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>().Subscribe(ProcessCoinDispenseFailed);

            if (Global.User != null && Global.PaymentArgs != null)
            {
                ExecuteCreditChangeCommand(false);
            }
            else if (Global.User != null && Global.CreditToAccount != 0)
            {
                ExecuteCreditChangeCommand(true);
            }
            else
            {
                CreditChangeVisible = false;
                LoginCreditChangeVisible = true;
                CreateAccountVisible = true;
                if (Global.PaymentOptions.Contains("ChangeAbandoned"))
                    NoChangeVisible = true;
            }

            if (Global.EnableCoin == true && Global.EnableDispenseChange == true && (-Global.AmountDue) <= Global.DisableCoinDispenseWhenChangeIsGreaterThan)
            {
                DispenseChangeVisible = true;
            }
            else
            {
                DispenseChangeVisible = false;
            }

            TitleText = String.Format(LocalizationProvider.GetLocalizedValue<string>("ReturnChangeOptions.Title"), Convert.ToString(-Global.CreditToAccount));

            base.Init();
        }

        private void ExecuteDispenseChangeCommand()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Global.PaymentArgs.PaymentItems.Add(new PaymentItem
                {
                    DateTime = DateTime.Now,
                    Payment = -Global.CreditToAccount,
                    Routing = "Dispense",
                    Source = "Purchase Complete"
                });

                Global.PaymentArgs.Change = -Global.CreditToAccount;
                Global.AmountInCredit = 0;
                Global.AmountPaid = 0;
            });

            aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Publish(Global.PaymentArgs);
        }

        private void ExecuteNoChangeCommand()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Global.PaymentArgs.PaymentItems.Add(new PaymentItem
                {
                    DateTime = DateTime.Now,
                    Payment = -Global.CreditToAccount,
                    Routing = Helpers.Enum.PaymentMethod.ChangeAbandoned.ToString(),
                    Source = "Purchase Complete"
                });

                Global.PaymentArgs.Change = 0;
                Global.CreditToAccount = 0;
                Global.AmountInCredit = 0;
                Global.AmountPaid = 0;
            });

            aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Publish(Global.PaymentArgs);
        }

        private void ExecuteCreditChangeCommand(object transactionCancelled)
        {
            deOROMembershipProvider userProvider = new deOROMembershipProvider();
            userProvider.UpdateUserBalance(Global.User.UserName, -Global.CreditToAccount, "CreditToMyAccount");
            Global.User = userProvider.GetUser(Global.User.UserName) as deOROMembershipUser;

            AccountBalanceHistoryRepository accountHistoryRepo = new AccountBalanceHistoryRepository();

            if (!Convert.ToBoolean(transactionCancelled))
            {
                accountHistoryRepo.Add(Global.User.ProviderUserKey.ToString(), Global.User.AccountBalance, -Global.CreditToAccount, "CreditToMyAccount - Transaction Complete");

                App.Current.Dispatcher.Invoke(() =>
                {
                    Global.PaymentArgs.PaymentItems.Add(new PaymentItem
                    {
                        DateTime = DateTime.Now,
                        Payment = -Global.CreditToAccount,
                        Routing = Helpers.Enum.PaymentMethod.MyAccountPay.ToString(),
                        Source = "Purchase Complete"
                    });

                    Global.PaymentArgs.Change = 0;
                    Global.AmountInCredit = 0;
                    Global.AmountPaid = 0;
                });

                aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().Publish(Global.PaymentArgs);
            }
            else
            {
                accountHistoryRepo.Add(Global.User.ProviderUserKey.ToString(), Global.User.AccountBalance, -Global.CreditToAccount, "CreditToMyAccount - Transaction Cancelled");
                aggregator.GetEvent<EventAggregation.TransactionCancelEvent>().Publish(null);
            }
        }


        private void ExecuteLoginCreditChangeCommand()
        {
            aggregator.GetEvent<EventAggregation.LoginEvent>().Publish(null);
        }

        private void ExecuteCreateAccountCommand()
        {
            aggregator.GetEvent<EventAggregation.UserRegistrationEvent>().Publish(null);
        }


        private void ProcessCoinJamEvent(object obj)
        {           
            DispenseChangeVisible = false;
        }

        private void ProcessCoinDispenseFailed(object obj)
        {
            DispenseChangeVisible = false;
        }
      


        public override void Dispose()
        {
            aggregator.GetEvent<EventAggregation.CoinJamEvent>().Unsubscribe(ProcessCoinJamEvent);
            aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>().Unsubscribe(ProcessCoinDispenseFailed);
            base.Dispose();
        }
    }
}
