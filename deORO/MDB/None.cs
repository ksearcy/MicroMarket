using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.Communication;
using deORO.EventAggregation;
using Microsoft.Practices.Composite.Events;

namespace deORO.MDB
{
    class None : ICommunicationType
    {
        private static None none;
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public static ICommunicationType GetMDB()
        {
            if (none == null)
                none = new None();

            return none;
        }

        public void InitCoin()
        {
            
        }

        public void InitBill()
        {
            
        }

        public void InitCreditCard()
        {
            
        }

        public void DispenseChange(decimal change)
        {
            aggregator.GetEvent<EventAggregation.CoinDispenseCompleteEvent>().Publish(
                           new DispenseEventArgs
                           {
                               Coins = new decimal[] {2,3}.ToList(),
                               CoinsValue = 5
                           });
        }

        public EventAggregation.CoinAndBillStatusEventArgs GetCoinAndBillStatus()
        {
            return new CoinAndBillStatusEventArgs();
        }

        public void EnableBills(decimal amountDue = 0, string notesSet = "", string transactionType = "Purchase")
        {
            
        }

        public void EnableCoins()
        {
            
        }

        public void EnableCreditCard()
        {
           
        }

        public void DisableCreditCard()
        {
           
        }

        public void AuthorizeAmount(decimal amount)
        {
           
        }

        public void CloseDevices()
        {
           
        }

        public void Dispose()
        {
            
        }


        public void Close()
        {
            //throw new NotImplementedException();
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
