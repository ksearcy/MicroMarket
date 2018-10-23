using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;
using MPOST;
using deORO.EventAggregation;

namespace deORO.BillAcceptor
{
    public class MEI : IDisposable
    {
        private Acceptor billAcceptor;
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public void Init()
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

        void billAcceptor_OnJamDetected(object sender, EventArgs e)
        {
            aggregator.GetEvent<EventAggregation.BillJamEvent>().Publish(null);
        }

        void billAcceptor_OnStacked(object sender, EventArgs e)
        {
            if (billAcceptor.DocType == DocumentType.Bill)
            {
                //aggregator.GetEvent<EventAggregation.BillAcceptedEvent>().Publish(billAcceptor.Bill);
                aggregator.GetEvent<EventAggregation.BillAcceptedEvent>().Publish(new CashEventArgs()
                {
                    Amount = Convert.ToDecimal(billAcceptor.Bill.Value, System.Globalization.CultureInfo.InvariantCulture),
                    Routing = "Stacked"
                });
            }
        }

        void billAcceptor_OnConnected(object sender, EventArgs e)
        {
            billAcceptor.EnableAcceptance = true;
            billAcceptor.AutoStack = true;
            billAcceptor.HighSecurity = false;
            billAcceptor.DisconnectTimeout = 100;
        }

        public void Dispose()
        {
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
    }
}
