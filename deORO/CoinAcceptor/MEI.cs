using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MEI.CF7000Coin;
using MEI.Coins;
using Microsoft.Practices.Composite.Events;
using deORO.EventAggregation;


namespace deORO.CoinAcceptor
{
    public class MEI : IDisposable
    {
        private AbstractCoinChanger coinChanger;
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public void Init()
        {
            try
            {
                coinChanger = new CF7XXX();

                coinChanger.OnCoinAccepted += coinChanger_OnCoinAccepted;
                coinChanger.OnCoinFault += coinChanger_OnCoinFault;
                coinChanger.OnCommFault += coinChanger_OnCommFault;
                coinChanger.OnStateChanged += coinChanger_OnStateChanged;
                coinChanger.OnDispenseByCoinValue += coinChanger_OnDispenseByCoinValue;

                coinChanger.Open();
            }
            catch (Exception ex)
            {
                aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(new DeviceFailEventArgs()
                {
                    DeviceType = Helpers.Enum.DeviceType.Coin,
                    Message = ex.Message + " " + ex.StackTrace
                });
            }
        }

        void coinChanger_OnDispenseByCoinValue(object sender, DispenseByCoinEventArg args)
        {
            if (args.DispenseStatus == DispenseByCoinStatus.Completed)
            {
                aggregator.GetEvent<EventAggregation.CoinDispenseCompleteEvent>().Publish(null);
            }
        }

        public void AddCredit(decimal credit)
        {
            coinChanger.AddCredit(credit);
        }

        public void CancelTransaction()
        {
            decimal credit = coinChanger.QryCurrentCredit();
            DispenseChange(credit);
        }

        public void DispenseChange(decimal change)
        {
            if (change <= 0)
                return;

            decimal shortFall = coinChanger.EstimateShortfall(change);

            if (shortFall == 0)
            {
                coinChanger.DispenseChange(change);
            }
            else
            {
                aggregator.GetEvent<EventAggregation.CoinShortfallEvent>().Publish(shortFall);
            }
        }

        void coinChanger_OnStateChanged(object sender, ChangerStateEventArg args)
        {
            if (args.GetChangerState() == ChangerStatus.Opened)
            {
                coinChanger.Enable();
            }
        }

        void coinChanger_OnCommFault(object sender, CommFaultEventArg commFaultArgs)
        {
            aggregator.GetEvent<EventAggregation.CoinErrorEvent>().Publish(commFaultArgs);
        }

        void coinChanger_OnCoinFault(object sender, Int32EventArg args)
        {
            aggregator.GetEvent<EventAggregation.CoinErrorEvent>().Publish(args);
        }

        void coinChanger_OnCoinAccepted(object sender, CurrencyEventArg args)
        {
            //aggregator.GetEvent<EventAggregation.CoinAcceptedEvent>().Publish(args);
            aggregator.GetEvent<EventAggregation.CoinAcceptedEvent>().Publish(new CashEventArgs() { Amount = args.Value, Routing = args.CoinRoute.ToString() });
        }

        public void Dispose()
        {
            if (coinChanger != null)
            {
                coinChanger.OnCoinAccepted -= coinChanger_OnCoinAccepted;
                coinChanger.OnCoinFault -= coinChanger_OnCoinFault;
                coinChanger.OnCommFault -= coinChanger_OnCommFault;
                coinChanger.OnDispenseByCoinValue -= coinChanger_OnDispenseByCoinValue;

                try { coinChanger.Disable(); }
                catch { }
                try { coinChanger.Close(); }
                catch { }
                try { coinChanger = null; }
                catch { }
            }
        }
    }
}
