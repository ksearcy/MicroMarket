using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Practices.Composite.Events;

namespace deORO.USBRelay
{
    public class KMTronic : IDisposable
    {
        private System.IO.Ports.SerialPort serialPort = null;
        private DispatcherTimer timer1 = new DispatcherTimer();
        private DispatcherTimer timer2 = new DispatcherTimer();

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public void Init()
        {
            try
            {
                timer1.Interval = new TimeSpan(0, 0, Helpers.Global.KMtronicTimeout);
                timer1.Tick += timer1_Tick;
                timer1.IsEnabled = false;

                timer2.Interval = new TimeSpan(0, 0, Helpers.Global.KMtronicTimeout);
                timer2.Tick += timer2_Tick;
                timer2.IsEnabled = false;

                serialPort = new System.IO.Ports.SerialPort(Helpers.Global.KMtronic);
                serialPort.Open();

            }
            catch
            {

            }
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            CloseRelay1();
        }

        void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            CloseRelay2();
        }

        public void OpenRelay1()
        {
            timer1.Start();
            try
            {
                serialPort.Write(new byte[] { 0xFF, 0x01, 0x01 }, 0, 3);
            }
            catch{ }
        }

        public void OpenRelay2()
        {
            timer2.Start();
            try
            {
                serialPort.Write(new byte[] { 0xFF, 0x02, 0x01 }, 0, 3);
            }
            catch{ }
        }

        public void CloseRelay1()
        {
            try
            {
                serialPort.Write(new byte[] { 0xFF, 0x01, 0x00 }, 0, 3);
            }
            catch { }
            aggregator.GetEvent<EventAggregation.Relay1CloseEvent>().Publish(null);
        }

        public void CloseRelay2()
        {
            try
            {
                serialPort.Write(new byte[] { 0xFF, 0x02, 0x00 }, 0, 3);
            }
            catch { }
            aggregator.GetEvent<EventAggregation.Relay2CloseEvent>().Publish(null);
        }

        public void Dispose()
        {
            if (serialPort != null)
            {
                CloseRelay1();
                CloseRelay2();
                serialPort.Close();
            }

            timer1.IsEnabled = false;
            timer2.IsEnabled = false;
        }
    }
}
