using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.CardProcessor;
using Microsoft.Practices.Composite.Events;

namespace deORO.CardReader
{
    public class CardKnox : ICardReader
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        private static CardKnox instance;
        private PaymentEngine.xTransaction.Request request = null;

        public static CardKnox Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CardKnox();
                }
                return instance;
            }
        }

        private CardKnox()
        {
            request = new PaymentEngine.xTransaction.Request();
        }

        public void Open()
        {
           
        }

        public void Close()
        {
           
        }

        public void Reset()
        {
            
        }

        public void SetParams(decimal payload)
        {
            CreditCardData data = new CreditCardData();
            aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Publish(data);
        }

        public void DataEvent(object payload)
        {
            
        }

        public void ErrorEvent(object payload)
        {
            
        }

        public void Authorize(object payload)
        {
            
        }

        public void Dispose()
        {
            
        }


    }
}
