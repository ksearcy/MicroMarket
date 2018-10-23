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
    public class AddMoneyViewModel : BaseViewModel
    {
        public ICommand CloseCommand { get { return new DelegateCommand(ExecuteCloseCommand); } }
        public ICommand RefreshCommand { get { return new DelegateCommand(ExecuteRefreshCommand); } }
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        private deORO.CardReader.ICardReader cardReader = CardReader.CardReaderFactory.GetCreditCardReader();
        private deORO.Communication.ICommunicationType commType;

        private string billInfo;

        public string BillInfo
        {
            get { return billInfo; }
            set { billInfo = value; RaisePropertyChanged(() => BillInfo); }
        }

        public override void Init()
        {
            commType = deORO.Communication.CommunicationTypeFactory.GetCommunicationType();

            if (Global.EnableBill)
                commType.EnableBills();

            if (Global.EnableCoin)
                commType.EnableCoins();

            aggregator.GetEvent<EventAggregation.E2CGeneralEvent>().Subscribe(ProcessE2CGeneralEvent);
            base.Init();
        }

        private void ExecuteCloseCommand()
        {
            aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
        }

        private void ExecuteRefreshCommand()
        {
            BillInfo = "";

            if (Global.EnableBill)
                commType.EnableBills();

            if (Global.EnableCoin)
                commType.EnableCoins();
            
        }

        private void ProcessE2CGeneralEvent(object param)
        {
            string[] ignore = { "enabled", "disabled", "no errors", "inactive" };

            if (!ignore.Any(param.ToString().ToLower().Contains))
            {
                BillInfo += param.ToString() + "\r\n";
            }
        }
        
        public override void Dispose()
        {
            aggregator.GetEvent<EventAggregation.E2CGeneralEvent>().Unsubscribe(ProcessE2CGeneralEvent);

            if (commType != null)
            {
                commType.Close();
                commType = null;
            }
        }
    }
}
