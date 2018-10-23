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
    class MyPayrollPaymentViewModel : BaseViewModel
    {
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
                    balanceText = string.Format("Your current payroll balance is {0}. Press OK to complete purchase", Helpers.Global.User.PayrollBalance.ToString("C2"));
                }
                return balanceText;
            }
            set
            {
                balanceText = value;
                RaisePropertyChanged(() => BalanceText);
            }
        }

        private decimal payrollBalance;

        public decimal PayrollBalance
        {
            get
            {
                if (Global.User != null)
                    return payrollBalance = Global.User.PayrollBalance;
                   
                else
                    return 0;
            }
            set { payrollBalance = value; }
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
            aggregator.GetEvent<EventAggregation.MyPayrollPaymentCancelEvent>().Publish(null);
        }

        private bool CanExecuteOKCommand()
        {
            if (AmountDue > 0 && Global.User != null && AmountDue <= Global.User.PayrollBalance)
                return true;
            else
                return false;
        }

        private void ExecuteOKCommand()
        {
            deOROMembershipProvider provider = new deOROMembershipProvider();

            //if (provider.DeductUserBalance(Global.User.UserName, Global.AmountDue))
            if (provider.UpdateUserPayrollBalance(Global.User.UserName, -Global.AmountDue, "Payroll Purchase"))
            {
                aggregator.GetEvent<EventAggregation.MyPayrollPaymentCompleteEvent>().Publish(provider.GetUser(Global.User.UserName)
                                                                                              as deOROMembershipUser);

                List<PaymentItem> items = new List<PaymentItem>();
                items.Add(new PaymentItem
                {
                    Source = Helpers.Enum.PaymentMethod.MyPayrollPay.ToString(),
                    Payment = Global.AmountDue,
                    DateTime = DateTime.Now
                });

                aggregator.GetEvent<EventAggregation.PaymentCompleteEvent>().
                Publish(
                new PaymentCompleteEventArgs
                {
                    Source = Helpers.Enum.PaymentMethod.MyPayrollPay.ToString(),
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
