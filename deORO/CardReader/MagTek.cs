using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.Helpers;
using Microsoft.PointOfService;
using Microsoft.Practices.Composite.Events;
using deORO.EventAggregation;
using deORO.CardProcessor;
using MTSCRANET;
using MTLIB;

namespace deORO.CardReader
{
    class MagTek : ICardReader
    {
        private MTSCRA m_SCRA;
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        private MTConnectionState state;
        private bool isLocked = false;

        private static MagTek instance;
        public static MagTek Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MagTek();
                }
                return instance;
            }
        }

        public MagTek()
        {
        }

        void m_SCRA_OnDeviceConnectionStateChanged(object sender, MTConnectionState state)
        {
            //m_SCRA.OnDeviceConnectionStateChanged -= m_SCRA_OnDeviceConnectionStateChanged;
            this.state = state;

            if (state == MTConnectionState.Connected)
            {
                //System.Windows.Forms.MessageBox.Show("connected");
                aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
            }
            else if (state == MTConnectionState.Error)
            {
                System.Windows.Forms.MessageBox.Show("error");
                aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
                  new DeviceFailEventArgs()
                  {
                      DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                      Message = "Error while enabling credit card reader"
                  });

                aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
            }
            else if (state == MTConnectionState.Disconnected)
            {
                System.Windows.Forms.MessageBox.Show("disconnected");
                aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
                 new DeviceFailEventArgs()
                 {
                     DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                     Message = "Credit card reader is disconnected"
                 });

                aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
            }

        }

        protected void OnDataReceived(object sender, IMTCardData cardData)
        {
            //System.Windows.MessageBox.Show("Swiped");
            //m_SCRA.OnDataReceived -= OnDataReceived;
            System.Diagnostics.Debug.WriteLine(DateTime.Now);
            System.Diagnostics.Debug.WriteLine(cardData.getTrackDecodeStatus());
            System.Diagnostics.Debug.WriteLine(this.isLocked);
            if (!this.isLocked)
                this.isLocked = true;
            else
                return;

            if (cardData.getTrackDecodeStatus() == "000000")
            {

                DataEvent(new CreditCardData
                {
                    AdditionalSecurityInformation = Encoding.ASCII.GetBytes(cardData.getKSN()),
                    Track2EncryptedData = Encoding.ASCII.GetBytes(cardData.getTrack2()),
                    EncryptedDataLength = 18
                });
            }
            else
            {
                m_SCRA.OnDataReceived += OnDataReceived;
                aggregator.GetEvent<EventAggregation.CreditCardReaderErrorEvent>().Publish(null);
            }
        }

        public void Open()
        {
            m_SCRA = new MTSCRA();
            m_SCRA.OnDeviceList += m_SCRA_OnDeviceList;
            m_SCRA.OnDataReceived += OnDataReceived;
            m_SCRA.OnDeviceConnectionStateChanged += m_SCRA_OnDeviceConnectionStateChanged;
            m_SCRA.setConnectionType(MTConnectionType.USB);
            m_SCRA.requestDeviceList(MTConnectionType.USB);
            this.isLocked = false;

        }

        void m_SCRA_OnDeviceList(object sender, MTConnectionType connectionType, List<MTDeviceInformation> deviceList)
        {
            if (deviceList.Count > 0)
            {
                m_SCRA.setAddress(deviceList[0].Address);
                m_SCRA.openDevice();
            }
            else
            {
                aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
                  new DeviceFailEventArgs()
                  {
                      DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                      Message = "Credit card reader not found."
                  });

                aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);
            }
        }

        public void Close()
        {
            if (m_SCRA != null)
            {
                m_SCRA.OnDataReceived -= OnDataReceived;
                m_SCRA.OnDeviceConnectionStateChanged -= m_SCRA_OnDeviceConnectionStateChanged;
                m_SCRA.closeDevice();
                this.isLocked = false;
            }
        }

        public void Reset()
        {

        }

        public void SetParams(decimal payload)
        {
            try
            {
                if (m_SCRA != null)
                {
                    m_SCRA.OnDataReceived -= OnDataReceived;
                    m_SCRA.OnDeviceConnectionStateChanged -= m_SCRA_OnDeviceConnectionStateChanged;
                }
                Open();


            }
            catch (Exception ex)
            {
                aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
                  new DeviceFailEventArgs()
                  {
                      DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                      Message = ex.Message + " " + ex.StackTrace
                  });

            }
        }

        public void DataEvent(object payload)
        {
            aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Publish(payload as CreditCardData);
        }

        public void ErrorEvent(object payload)
        {

        }

        public void Authorize(object payload)
        {

        }

        public void Dispose()
        {
            if (m_SCRA != null)
            {
                m_SCRA.OnDataReceived -= OnDataReceived;
                m_SCRA.OnDeviceConnectionStateChanged -= m_SCRA_OnDeviceConnectionStateChanged;
                m_SCRA.closeDevice();
                this.isLocked = false;
            }
        }


    }
}
