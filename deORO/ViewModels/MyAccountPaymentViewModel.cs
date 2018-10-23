using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.EventAggregation;
using deORO.Helpers;
using deORODataAccessApp.Models;
using Microsoft.Practices.Composite.Events;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;

namespace deORO.ViewModels
{
    class MyAccountPaymentViewModel : BaseViewModel
    {
        UserRepository UserRepository = new UserRepository();

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ICommand OKCommand { get { return new DelegateCommand(ExecuteOKCommand, CanExecuteOKCommand); } }
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand); } }

        private string balanceText;
        public string BalanceText
        {
            get
            {
                if (Global.User != null)
                {
                    balanceText = string.Format("Your current account balance is {0}. Press OK to complete purchase", Helpers.Global.User.AccountBalance.ToString("C2"));
                }
                return balanceText;
            }
            set
            {
                balanceText = value;
                RaisePropertyChanged(() => BalanceText);
            }
        }

        private decimal accountBalance;

        public decimal AccountBalance
        {
            get
            {
                if (Global.User != null)
                //Sync User Addition
                {
                    try
                    {
                        decimal ActualAccountBalance = Convert.ToDecimal(UserRepository.GetUserByUsername(Global.User.UserName).account_balance, System.Globalization.CultureInfo.InvariantCulture);
                        Global.User.AccountBalance = ActualAccountBalance;
                    }
                    catch  { }
                   
                    return accountBalance = Global.User.AccountBalance;
                }
                else
                {
                    return 0;
                }
            }
            set { accountBalance = value; }
        }

        private decimal amountDue;

        public decimal AmountDue
        {
            get { return amountDue; }
            set
            {
                amountDue = value;
                RaisePropertyChanged(() => AmountDue);
            }
        }

        public override void Init()
        {
            aggregator.GetEvent<EventAggregation.AmoutDueChangeEvent>().Subscribe(x => { AmountDue = x; });
            AmountDue = Global.AmountDue;

            base.Init();
        }

        private void ExecuteCancelCommand()
        {
            aggregator.GetEvent<EventAggregation.MyAccountPaymentCancelEvent>().Publish(null);
        }

        private bool CanExecuteOKCommand()
        {
            if (AmountDue > 0 && Global.User != null && AmountDue <= Global.User.AccountBalance)
                return true;
            else
                return false;
        }

        private void ExecuteOKCommand()
        {
            deOROMembershipProvider provider = new deOROMembershipProvider();

            //if (provider.DeductUserBalance(Global.User.UserName, Global.AmountDue))
            if (provider.UpdateUserBalance(Global.User.UserName, -Global.AmountDue, "Purchase"))
            {
                aggregator.GetEvent<EventAggregation.MyAccountPaymentCompleteEvent>().Publish(provider.GetUser(Global.User.UserName)
                                                                                              as deOROMembershipUser);

                List<PaymentItem> items = new List<PaymentItem>();
                items.Add(new PaymentItem
                {
                    Source = Helpers.Enum.PaymentMethod.MyAccountPay.ToString(),
                    Payment = Global.AmountDue,
                    DateTime = DateTime.Now
                });

                aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().
                Publish(
                new PaymentCompleteEventArgs
                {
                    Source = Helpers.Enum.PaymentMethod.MyAccountPay.ToString(),
                    PaymentItems = items
                });
            }
        }

        public override void Dispose()
        {
            AmountDue = 0;
            aggregator.GetEvent<EventAggregation.AmoutDueChangeEvent>().Unsubscribe(x => { AmountDue = x; });
        }
    }
}
