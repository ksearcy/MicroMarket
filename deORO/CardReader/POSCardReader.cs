using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.CardProcessor;
using deORO.EventAggregation;
using deORO.Helpers;
using Microsoft.PointOfService;
using Microsoft.Practices.Composite.Events;

namespace deORO.CardReader
{
    class POSCardReader : ICardReader
    {
        private DeviceInfo deviceInfo;
        private Msr posCardReader;
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        private static POSCardReader instance;

        public static POSCardReader Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new POSCardReader();
                }
                return instance;
            }
        }

        public POSCardReader()
        {
            //Open();
        }

        private void Init()
        {
            try
            {

                if (posCardReader == null)
                {
                    PosExplorer posExplorer = new PosExplorer();

                    foreach (DeviceInfo d in posExplorer.GetDevices(DeviceType.Msr))
                    {
                        if (d.ServiceObjectName.Equals(Global.CardReaderDeviceName))
                        {
                            deviceInfo = d;
                            break;
                        }
                    }

                    if (deviceInfo != null)
                    {
                        posCardReader = posExplorer.CreateInstance(deviceInfo) as Msr;
                    }
                    else
                    {
                        aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(
                        new DeviceFailEventArgs()
                        {
                            DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                            Message = string.Format("Device {0} not found", Global.CardReaderDeviceName)
                        });

                    }
                }

                posCardReader.ErrorEvent += posCardReader_ErrorEvent;
                posCardReader.DataEvent += posCardReader_DataEvent;
            }
            catch (Exception ex)
            {
                aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(
                new DeviceFailEventArgs()
                {
                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                    Message = ex.Message + " " + ex.StackTrace
                });
            }
        }


        void posCardReader_DataEvent(object sender, DataEventArgs e)
        {
            posCardReader.DeviceEnabled = false;

            DataEvent(new CreditCardData
            {
                AdditionalSecurityInformation = posCardReader.AdditionalSecurityInformation,
                Track2EncryptedData = posCardReader.Track2EncryptedData,
                EncryptedDataLength = posCardReader.Track2EncryptedDataLength
            });

        }

        void posCardReader_ErrorEvent(object sender, DeviceErrorEventArgs e)
        {
            ErrorEvent(e);
        }

        public void Dispose()
        {
            if (posCardReader != null)
            {
                try { if (posCardReader.DeviceEnabled) posCardReader.DeviceEnabled = false; }
                catch { }
                try { if (posCardReader.Claimed) posCardReader.Release(); }
                catch { }
                try { posCardReader.Close(); }
                catch { }
            }

        }

        public void DataEvent(object payload)
        {
            aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Publish(payload as CreditCardData);
        }

        public void ErrorEvent(object payload)
        {
            aggregator.GetEvent<EventAggregation.CreditCardReaderErrorEvent>().Publish(null);
        }

        public void SetParams(decimal payload)
        {
            Init();

            try
            {

                if (posCardReader.State == ControlState.Closed)
                    posCardReader.Open();
            }
            catch (Exception ex)
            {
                aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
                   new DeviceFailEventArgs()
                   {
                       DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                       Message = ex.Message + " " + ex.StackTrace
                   });

                return;
            }

            try
            {
                if (!posCardReader.Claimed)
                    posCardReader.Claim(1000);
            }
            catch (Exception ex)
            {
                aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
                  new DeviceFailEventArgs()
                  {
                      DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                      Message = ex.Message + " " + ex.StackTrace
                  });

                return;
            }

            try
            {
                if (!posCardReader.DeviceEnabled)
                    posCardReader.DeviceEnabled = true;
            }
            catch (Exception ex)
            {
                aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
                  new DeviceFailEventArgs()
                  {
                      DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                      Message = ex.Message + " " + ex.StackTrace
                  });

                return;
            }

            try
            {
                if (!posCardReader.DataEventEnabled)
                    posCardReader.DataEventEnabled = true;
            }
            catch (Exception ex)
            {
                aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
                  new DeviceFailEventArgs()
                  {
                      DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                      Message = ex.Message + " " + ex.StackTrace
                  });

                return;
            }

            try
            {
                if (!posCardReader.DecodeData)
                    posCardReader.DecodeData = true;
            }
            catch (Exception ex)
            {
                aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
                  new DeviceFailEventArgs()
                  {
                      DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                      Message = ex.Message + " " + ex.StackTrace
                  });

                return;
            }

        }

        public void Open()
        {
            //Init();

            //try
            //{

            //    if (posCardReader.State == ControlState.Closed)
            //        posCardReader.Open();
            //}
            //catch (Exception ex)
            //{
            //    aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
            //       new DeviceFailEventArgs()
            //       {
            //           DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
            //           Message = ex.Message + " " + ex.StackTrace
            //       });

            //    return;
            //}

            //try
            //{
            //    if (!posCardReader.Claimed)
            //        posCardReader.Claim(1000);
            //}
            //catch (Exception ex)
            //{
            //    aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
            //      new DeviceFailEventArgs()
            //      {
            //          DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
            //          Message = ex.Message + " " + ex.StackTrace
            //      });

            //    return;
            //}

            //try
            //{
            //    if (!posCardReader.DeviceEnabled)
            //        posCardReader.DeviceEnabled = true;
            //}
            //catch (Exception ex)
            //{
            //    aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
            //      new DeviceFailEventArgs()
            //      {
            //          DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
            //          Message = ex.Message + " " + ex.StackTrace
            //      });

            //    return;
            //}

            //try
            //{
            //    if (!posCardReader.DataEventEnabled)
            //        posCardReader.DataEventEnabled = true;
            //}
            //catch (Exception ex)
            //{
            //    aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
            //      new DeviceFailEventArgs()
            //      {
            //          DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
            //          Message = ex.Message + " " + ex.StackTrace
            //      });

            //    return;
            //}

            //try
            //{
            //    if (!posCardReader.DecodeData)
            //        posCardReader.DecodeData = true;
            //}
            //catch (Exception ex)
            //{
            //    aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
            //      new DeviceFailEventArgs()
            //      {
            //          DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
            //          Message = ex.Message + " " + ex.StackTrace
            //      });

            //    return;
            //}

        }

        public void Close()
        {
            if (posCardReader != null)
            {
                posCardReader.DeviceEnabled = false;
                posCardReader.ErrorEvent -= posCardReader_ErrorEvent;
                posCardReader.DataEvent -= posCardReader_DataEvent;
            }
        }

        public void Reset()
        {

        }

        public void Authorize(object payload)
        {
            //throw new NotImplementedException();
        }
                       
    }
}
