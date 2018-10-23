using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    public class PaymentOptionsViewModel : BaseViewModel
    {
        public ICommand MyAccountCommand { get { return new DelegateCommand(ExecuteMyAccountCommand, CanExecuteMyAccountCommand); } }
        public ICommand MyPayrollCommand { get { return new DelegateCommand(ExecuteMyPayrollCommand, CanExecuteMyPayrollCommand); } }
        public ICommand CreditCardCommand { get { return new DelegateCommand(ExecuteCreditCardCommand, CanExecuteCreditCardCommand); } }
        public ICommand CashCommand { get { return new DelegateCommand(ExecuteCashCommand, CanExecuteCashCommand); } }
               
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand); } }
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        private bool cashPaymentVisible = false;  

        public bool CashPaymentVisible
        {
            get { return cashPaymentVisible; }
            set { cashPaymentVisible = value; RaisePropertyChanged(() => CashPaymentVisible); }
        }

        private bool payrollPaymentVisible = false;

        public bool PayrollPaymentVisible
        {
            get { return payrollPaymentVisible; }
            set { payrollPaymentVisible = value; RaisePropertyChanged(() => PayrollPaymentVisible); }
        }

        private bool creditCardPaymentVisible = false;

        public bool CreditCardPaymentVisible
        {
            get { return creditCardPaymentVisible; }
            set { creditCardPaymentVisible = value; RaisePropertyChanged(() => CreditCardPaymentVisible); }
        }

        private string accountBalance;
        public string AccountBalance
        {
            get
            {
                if (Global.User != null)
                {
                    accountBalance = Global.User.AccountBalance.ToString("C2");
                }
                else
                {
                    accountBalance = "N/A";
                }
                return accountBalance;
            }
            set
            {
                accountBalance = value;
            }
        }

        public override void Init()
        {
            //if (Global.EnableBill || Global.EnableCoin)
            if (Global.PaymentOptions.Contains(Helpers.Enum.PaymentMethod.BillPay.ToString()) || 
                Global.PaymentOptions.Contains(Helpers.Enum.PaymentMethod.CoinPay.ToString()))
                CashPaymentVisible = true;

            if (Global.PaymentOptions.Contains(Helpers.Enum.PaymentMethod.CreditCardPay.ToString()))
                CreditCardPaymentVisible = true;

            if (Global.PaymentOptions.Contains(Helpers.Enum.PaymentMethod.MyPayrollPay.ToString()))
                PayrollPaymentVisible = true;

        }

        public PaymentOptionsViewModel()
        {
            //mdb.Init();
        }

        private void CreditCardFailedEvent(object obj)
        {
            aggregator.GetEvent<EventAggregation.PayEvent>().Publish(null);
        }

        private void ExecuteMyAccountCommand()
        {
            aggregator.GetEvent<EventAggregation.MyAccountPaymentEvent>().Publish(null);
        }

        private bool CanExecuteMyAccountCommand()
        {
            //if (Global.User == null)
            //    return false;

            //if (Convert.ToBoolean(Global.ShoppingCartItemsCount))
            //    return Global.AmountDue <= Global.User.AccountBalance ? true : false;
            //else
            //    return false;

            return Convert.ToBoolean(Global.ShoppingCartItemsCount);
        }


        private void ExecuteMyPayrollCommand()
        {
            aggregator.GetEvent<EventAggregation.MyPayrollPaymentEvent>().Publish(null);
        }

        private bool CanExecuteMyPayrollCommand()
        {
            //if (Global.User == null)
            //    return false;

            //if (Convert.ToBoolean(Global.ShoppingCartItemsCount))
            //    return Global.AmountDue <= Global.User.AccountBalance ? true : false;
            //else
            //    return false;
            return Convert.ToBoolean(Global.ShoppingCartItemsCount);
        }

        private void ExecuteCreditCardCommand()
        {
            aggregator.GetEvent<EventAggregation.CreditCardPaymentEvent>().Publish(null);
        }

        private bool CanExecuteCreditCardCommand()
        {
            return Convert.ToBoolean(Global.ShoppingCartItemsCount);
        }

        private void ExecuteCashCommand()
        {
            aggregator.GetEvent<EventAggregation.CashPaymentEvent>().Publish(null);
        }

        private bool CanExecuteCashCommand()
        {
            return Convert.ToBoolean(Global.ShoppingCartItemsCount);
        }

        private void ExecuteCancelCommand()
        {
            aggregator.GetEvent<EventAggregation.PaymentMethodCancelEvent>().Publish(null);
        }

        public override void Dispose()
        {
            //mdb.Dispose();
            //aggregator.GetEvent<EventAggregation.ShoppingCartItemsChangedEvent>().Unsubscribe((x) => { shoppingCartItemsCount = Convert.ToInt32(x); });
        }
    }
}
