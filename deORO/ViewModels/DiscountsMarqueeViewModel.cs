using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using deORO.Helpers;
using deORODataAccessApp.DataAccess;
using Microsoft.Practices.Composite.Events;

namespace deORO.ViewModels
{
    public class DiscountsMarqueeViewModel : BaseViewModel
    {
        private DiscountRepository repo = new DiscountRepository();
        public ICommand ShowDiscountsCommand { get { return new DelegateCommandWithParam(ExecuteShowDiscountsCommand); } }
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        private string marqueeText;

        public string MarqueeText
        {
            get { return marqueeText; }
            set { marqueeText = value; RaisePropertyChanged(() => MarqueeText); }
        }

        public override void Init()
        {
            GetMarqueeText();
            base.Init();
        }

        public void GetMarqueeText()
        {
            MarqueeText = "";
            repo.GetDiscounts().ForEach(x => MarqueeText += x.description + " | ");

            if (MarqueeText == "")
                MarqueeText = "No Discounts Available at this Time | ";
        }

        private void ExecuteShowDiscountsCommand(object obj)
        {
            aggregator.GetEvent<EventAggregation.DiscountMarqueeSelectComplete>().Publish(obj);
        }
    }
}
