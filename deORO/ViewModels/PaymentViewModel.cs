using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    public class PaymentViewModel : BaseViewModel
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        readonly static CreditCardPaymentViewModel creditCard = new CreditCardPaymentViewModel();
        readonly static CashPaymentViewModel cash = new CashPaymentViewModel();
        readonly static MyAccountPaymentViewModel myAccount = new MyAccountPaymentViewModel();
        readonly static PaymentOptionsViewModel paymentOptions = new PaymentOptionsViewModel();

        private BaseViewModel currentViewModel = paymentOptions;

        public BaseViewModel CurrentViewModel
        {
            get
            {
                return currentViewModel;
            }
            set
            {
                if (currentViewModel == value)
                    return;

                currentViewModel = value;
                RaisePropertyChanged(() => CurrentViewModel);
            }
        }

        public PaymentViewModel()
        {

        }

        public void Init()
        {

            aggregator.GetEvent<EventAggregation.CreditCardPaymentEvent>().Subscribe(ShowCreditCardPayment);
            aggregator.GetEvent<EventAggregation.CashPaymentEvent>().Subscribe(ShowCashPayment);
            aggregator.GetEvent<EventAggregation.MyAccountPaymentEvent>().Subscribe(ShowMyAccountPayment);

            aggregator.GetEvent<EventAggregation.PaymentMethodCancelEvent>().Subscribe(ShowPaymentOptions);
            //aggregator.GetEvent<EventAggregation.CreditCardReaderInitFailEvent>().Subscribe(ShowPaymentOptions);
            aggregator.GetEvent<EventAggregation.CreditCardTransactionFailedEvent>().Subscribe(ShowPaymentOptions);

            aggregator.GetEvent<EventAggregation.CashPaymentCancelEvent>().Subscribe(ShowPaymentOptions);
            aggregator.GetEvent<EventAggregation.CreditCardPaymentCancelEvent>().Subscribe(ShowPaymentOptions);
            aggregator.GetEvent<EventAggregation.MyAccountPaymentCancelEvent>().Subscribe(ShowPaymentOptions);
            
        }


        private void ShowPaymentOptions(object obj)
        {
            CurrentViewModel = PaymentViewModel.paymentOptions;
        }

        private void ShowCreditCardPayment(object obj)
        {
            CurrentViewModel = PaymentViewModel.creditCard;
        }

        private void ShowCashPayment(object obj)
        {
            CurrentViewModel = PaymentViewModel.cash;
        }

        private void ShowMyAccountPayment(object obj)
        {
            CurrentViewModel = PaymentViewModel.myAccount;
        }

        public override void Dispose()
        {
            CurrentViewModel = PaymentViewModel.paymentOptions;

            aggregator.GetEvent<EventAggregation.CreditCardPaymentEvent>().Unsubscribe(ShowCreditCardPayment);
            aggregator.GetEvent<EventAggregation.CashPaymentEvent>().Unsubscribe(ShowCashPayment);
            aggregator.GetEvent<EventAggregation.MyAccountPaymentEvent>().Unsubscribe(ShowMyAccountPayment);

            aggregator.GetEvent<EventAggregation.PaymentMethodCancelEvent>().Unsubscribe(ShowPaymentOptions);
            //aggregator.GetEvent<EventAggregation.CreditCardReaderInitFailEvent>().Unsubscribe(ShowPaymentOptions);
            aggregator.GetEvent<EventAggregation.CreditCardTransactionFailedEvent>().Unsubscribe(ShowPaymentOptions);
            
        }
    }
}
