using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.Helpers;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    class DiscountViewModel : BaseViewModel
    {
        public ICommand CancelCommand { get { return new DelegateCommand(ExecuteCancelCommand); } }
        public ICommand SaveCommand { get { return new DelegateCommand(ExecuteSaveCommand); } }

        private discount discount;
        private DiscountRepository repo = new DiscountRepository();
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public discount Discount
        {
            get { return discount; }
            set
            {
                discount = value;
                RaisePropertyChanged(() => Discount);
            }
        }

        public DiscountViewModel()
        {
            discount = new discount();
        }

        public DiscountViewModel(int discountId)
        {
            discount = new discount();
            discount = repo.GetDiscount(discountId);
        }

        private void ExecuteCancelCommand()
        {
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
        }

        private void ExecuteSaveCommand()
        {
            if (discount.id == 0)
            {
                if (repo.AddDiscount(discount))
                {
                    aggregator.GetEvent<EventAggregation.DiscountAddCompleteEvent>().Publish(null);
                }
                else
                {
                    aggregator.GetEvent<EventAggregation.DiscountAddFailEvent>().Publish(null);
                }
            }
            else
            {
                if (repo.UpdateDiscount(discount))
                {
                    aggregator.GetEvent<EventAggregation.DiscountUpdateCompleteEvent>().Publish(null);
                }
                else
                {
                    aggregator.GetEvent<EventAggregation.DiscountUpdateFailEvent>().Publish(null);
                }
            }

            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
        }
    }
}
