using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Composite.Events;

namespace deORO.CardProcessor
{
    class Demo : ICreditCardProcessor
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public string userName
        {
            get
            {
                return "";
            }
            set
            {

            }
        }

        public string password
        {
            get
            {
                return "";
            }
            set
            {

            }
        }

        public string message
        {
            get
            {
                return "";
            }
            set
            {

            }
        }

        //public async Task ProcessSale(CreditCardData cardData, decimal amount, string transactionDetails = "", string zipCode = "")
        //{
        //    await Task.Run(() =>
        //        {
        //            System.Threading.Thread.Sleep(3000);
        //            aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Approved");
        //        });
        //}

        public void ChangePassword(string serialNumber, int updateStatus)
        {
            //throw new NotImplementedException();
        }

        public void ProcessSale(CreditCardData cardData, decimal amount, string transactionDetails = "", string zipCode = "")
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (e, f) =>
                {
                    System.Threading.Thread.Sleep(1000);
                    aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Approved");
                };

            bg.RunWorkerAsync();
        }
    }
}
