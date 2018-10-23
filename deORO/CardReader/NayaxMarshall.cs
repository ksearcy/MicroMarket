using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using deORO.Marshall;
using Microsoft.Practices.Composite.Events;

namespace deORO.CardReader
{
    public class NayaxMarshall : ICardReader
    {
        private static NayaxMarshall instance;
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        private static MarshallMain marshall = null;
        Task task;

        public static NayaxMarshall Instance
        {
            get
            {
                if (instance == null || marshall == null)
                {
                    instance = new NayaxMarshall();
                }

                return instance;
            }
        }


        public void Open()
        {

        }

        public void Close()
        {
            if (marshall != null)
            {
                marshall.Close();
            }
        }

        public void Reset()
        {

        }

        public NayaxMarshall()
        {
           
        }

        public void SetParams(decimal payload)
        {
            
            task = Task.Factory.StartNew(() =>
            {
                marshall = new MarshallMain();

                if (marshall.InitComm())
                {
                    marshall.Run();
                }
                else
                {
                    //marshall = null;
                    aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(
                       new EventAggregation.DeviceFailEventArgs
                       {
                           DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                           Message = "Unable to open Marshall COM Port - " + Helpers.Global.MarshallCOMPort
                       });
                }

            });
        }

        public void DataEvent(object payload)
        {

        }

        public void ErrorEvent(object payload)
        {

        }

        public void Dispose()
        {
           
        }

        public void Authorize(object payload)
        {
            if (marshall != null)
                marshall.Authorize(Convert.ToInt32(payload));
        }


    }
}
