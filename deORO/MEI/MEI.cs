using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MEI.CF7000Coin;
using MEI.Coins;
using Microsoft.Practices.Composite.Events;
using deORO.EventAggregation;
using MPOST;
using deORO.Helpers;
using deORO.Communication;


namespace deORO.MEI
{
    public class MEI : deORO.Communication.ICommunicationType
    {
        private AbstractCoinChanger coinChanger;
        private Acceptor billAcceptor;
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        private static MEI mei;
        private static ICommunicationType mdb = null;


        public static ICommunicationType GetMEI()
        {
            if (mei == null)
            {
                mei = new MEI();
                mdb = deORO.MDB.E2CMDB.GetMDB();
            }

            return mei;
        }

        private MEI()
        {

        }

        public void InitCoin()
        {
            //try
            //{
            //    coinChanger.OnCoinAccepted += coinChanger_OnCoinAccepted;
            //    coinChanger.OnCoinFault += coinChanger_OnCoinFault;
            //    coinChanger.OnCommFault += coinChanger_OnCommFault;
            //    coinChanger.OnStateChanged += coinChanger_OnStateChanged;
            //    coinChanger.OnDispenseByCoinValue += coinChanger_OnDispenseByCoinValue;

            //}
            //catch (Exception ex)
            //{
            //    aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(new DeviceInitFailEventArgs() { DeviceType = Helpers.Enum.DeviceType.Coin, Message = ex.Message + " " + ex.StackTrace });
            //}
        }

        public void InitBill()
        {
            //try
            //{
            //    billAcceptor.OnConnected += billAcceptor_OnConnected;
            //    billAcceptor.OnStacked += billAcceptor_OnStacked;
            //    billAcceptor.OnJamDetected += billAcceptor_OnJamDetected;
            //}
            //catch (Exception ex)
            //{
            //    aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(new DeviceInitFailEventArgs() { DeviceType = Helpers.Enum.DeviceType.Bill, Message = ex.Message + " " + ex.StackTrace });
            //}
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
            aggregator.GetEvent<EventAggregation.CoinAcceptedEvent>().Publish(new CashEventArgs() { Amount = args.Value, Routing = args.CoinRoute.ToString() });
        }

        void billAcceptor_OnJamDetected(object sender, EventArgs e)
        {
            aggregator.GetEvent<EventAggregation.BillJamEvent>().Publish(null);
        }

        void billAcceptor_OnStacked(object sender, EventArgs e)
        {
            if (billAcceptor.DocType == DocumentType.Bill)
            {
                decimal amount = Convert.ToDecimal(billAcceptor.Bill.Value, System.Globalization.CultureInfo.InvariantCulture);
                try
                {
                    if (Global.Currency == "CAD")
                    {
                        if (billAcceptor.Bill.Value == Convert.ToDouble(1))
                        {
                            amount = 5;
                        }
                        else if (billAcceptor.Bill.Value == Convert.ToDouble(2))
                        {
                            amount = 10;
                        }
                        else if (billAcceptor.Bill.Value == Convert.ToDouble(5))
                        {
                            amount = 20;
                        }
                    }
                }
                catch { }

                aggregator.GetEvent<EventAggregation.BillAcceptedEvent>().Publish(new CashEventArgs() { Amount = amount, Routing = "Stacked" });
            }
        }

        void billAcceptor_OnConnected(object sender, EventArgs e)
        {
            if (billAcceptor.Connected)
            {
                billAcceptor.EnableAcceptance = true;
                billAcceptor.AutoStack = true;
                billAcceptor.HighSecurity = false;
                billAcceptor.DisconnectTimeout = 100;
            }
        }

        public void Dispose()
        {
            //try { coinChanger.Close(); }
            //catch { }
            //try { coinChanger = null; }
            //catch { }

            //try { billAcceptor.Close(); }
            //catch { }
            //try { billAcceptor = null; }
            //catch { }
        }


        public CoinAndBillStatusEventArgs GetCoinAndBillStatus()
        {
            return new CoinAndBillStatusEventArgs();
        }

        public void EnableBills(decimal amountDue = 0, string notesSet = "", string transactionType = "Purchase")
        {
            try
            {
                billAcceptor = new Acceptor();
                billAcceptor.OnConnected += billAcceptor_OnConnected;
                billAcceptor.OnStacked += billAcceptor_OnStacked;
                billAcceptor.OnJamDetected += billAcceptor_OnJamDetected;
                billAcceptor.Open(Global.BillAcceptorCOMPort, PowerUp.A);
            }
            catch (Exception ex)
            {
                aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(new DeviceFailEventArgs()
                {
                    DeviceType = Helpers.Enum.DeviceType.Bill,
                    Message = ex.Message + " " + ex.StackTrace
                });
            }
        }

        public void EnableCoins()
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


        public void CloseDevices()
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

            if (billAcceptor != null)
            {
                billAcceptor.OnConnected -= billAcceptor_OnConnected;
                billAcceptor.OnStacked -= billAcceptor_OnStacked;
                billAcceptor.OnJamDetected -= billAcceptor_OnJamDetected;

                try { billAcceptor.Close(); }
                catch { }
                try { billAcceptor = null; }
                catch { }
            }

        }

        public void InitCreditCard()
        {
        }

        public void EnableCreditCard()
        {
            mdb.EnableCreditCard();
        }

        public void AuthorizeAmount(decimal amount)
        {
            mdb.AuthorizeAmount(amount);
        }

        public void CompleteTransaction()
        {

        }

        public void DisableCreditCard()
        {
            mdb.DisableCreditCard();
        }

        public void Close()
        {
            //throw new NotImplementedException();
            mdb.Close();
        }


        public void ResetCoinHopper()
        {
            throw new NotImplementedException();
        }

        public void ResetBillRecycler()
        {
            throw new NotImplementedException();
        }

        public void ResetMDBCoin()
        {
            throw new NotImplementedException();
        }

        public void ResetMDBBill()
        {
            throw new NotImplementedException();
        }

    }
}
