using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.EventAggregation;
using deORO.Helpers;
using Microsoft.PointOfService;
using Microsoft.Practices.Composite.Events;

namespace deORO.BarcodeScanner
{
    public class POSBarcodeScanner : IBarcodeScanner
    {
        private DeviceInfo deviceInfo;
        private Scanner posScanner;

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        private static POSBarcodeScanner instance;
        public static POSBarcodeScanner Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new POSBarcodeScanner();
                }
                return instance;
            }
        }

        public POSBarcodeScanner()
        {
            //Init();
        }

        private string subscriber;

        public string Subscriber
        {
            get { return subscriber; }
            set { subscriber = value; }
        }

        private void Init()
        {
            if (posScanner == null)
            {
                PosExplorer posExplorer = new PosExplorer();
                deviceInfo = posExplorer.GetDevice(DeviceType.Scanner, Global.BarcodeReaderDeviceName);

                if (deviceInfo != null)
                {
                    try
                    {

                        posScanner = posExplorer.CreateInstance(deviceInfo) as Scanner;
                        posScanner.StatusUpdateEvent += posScanner_StatusUpdateEvent;
                        posScanner.ErrorEvent += posScanner_ErrorEvent;
                        posScanner.DataEvent += posScanner_DataEvent;
                    }
                    catch (Exception ex)
                    {
                        aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(
                        new DeviceFailEventArgs()
                        {
                            DeviceType = Helpers.Enum.DeviceType.BarcodeScanner,
                            Message = ex.Message + " " + ex.StackTrace
                        });

                    }
                }
                else
                {
                    aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(
                    new DeviceFailEventArgs()
                    {
                        DeviceType = Helpers.Enum.DeviceType.BarcodeScanner,
                        Message = string.Format("Device {0} not found", Global.BarcodeReaderDeviceName)
                    });
                }
            }
        }

        void posScanner_DataEvent(object sender, DataEventArgs e)
        {
            posScanner.DataEventEnabled = true;

            try
            {
                string text = "";
                bool flag = false;

                byte[] scanDataLabel = posScanner.ScanDataLabel;
                int i = 0;
                while (i < scanDataLabel.Length)
                {
                    switch (scanDataLabel[i])
                    {
                        case 0:
                            text += " [NUL]";
                            break;
                        case 1:
                            text += "[SOH]";
                            break;
                        case 2:
                            text += '[';
                            text += "STX]";
                            break;
                        case 3:
                            text += '[';
                            text += "ETX]";
                            break;
                        case 4:
                            text += '[';
                            text += "EOT]";
                            break;
                        case 5:
                            text += '[';
                            text += "ENQ]";
                            break;
                        case 6:
                            text += '[';
                            text += "ACK]";
                            break;
                        case 7:
                            text += '[';
                            text += "BEL]";
                            break;
                        case 8:
                            text += '[';
                            text += "BS]";
                            break;
                        case 9:
                            goto IL_472;
                        case 10:
                            text += '[';
                            text += "LF]";
                            if (flag)
                            {
                                text += "\n";
                                flag = false;
                            }
                            break;
                        case 11:
                            text += '[';
                            text += "VT]";
                            break;
                        case 12:
                            text += '[';
                            text += "FF]";
                            break;
                        case 13:
                            text += '[';
                            text += "CR]";
                            flag = true;
                            break;
                        case 14:
                            text += '[';
                            text += " SO]";
                            break;
                        case 15:
                            text += '[';
                            text += "SI]";
                            break;
                        case 16:
                            text += '[';
                            text += "DLF]";
                            break;
                        case 17:
                            text += '[';
                            text += "DC1]";
                            break;
                        case 18:
                            text += '[';
                            text += "DC2]";
                            break;
                        case 19:
                            text += '[';
                            text += "DC3]";
                            break;
                        case 20:
                            text += '[';
                            text += "DC4]";
                            break;
                        case 21:
                            text += '[';
                            text += "NAK]";
                            break;
                        case 22:
                            text += '[';
                            text += "SYN]";
                            break;
                        case 23:
                            text += '[';
                            text += "ETB]";
                            break;
                        case 24:
                            text += '[';
                            text += "CAN]";
                            break;
                        case 25:
                            text += '[';
                            text += "EM]";
                            break;
                        case 26:
                            text += '[';
                            text += "SUB]";
                            break;
                        case 27:
                            text += '[';
                            text += "ESC]";
                            break;
                        case 28:
                            text += '[';
                            text += "FS]";
                            break;
                        case 29:
                            text += '[';
                            text += "GS]";
                            break;
                        case 30:
                            text += '[';
                            text += "RS]";
                            break;
                        case 31:
                            text += '[';
                            text += "US]";
                            break;
                        default:
                            goto IL_472;
                    }
                IL_483:
                    i++;
                    continue;
                IL_472:
                    text += (char)scanDataLabel[i];
                    goto IL_483;
                }

                DataEvent(text.Replace("\0", " "));

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        void posScanner_ErrorEvent(object sender, DeviceErrorEventArgs e)
        {

        }

        void posScanner_StatusUpdateEvent(object sender, StatusUpdateEventArgs e)
        {

        }

        public void ErrorEvent(object payload)
        {

        }

        public void DataEvent(object payload)
        {
            if (Subscriber == "Local")
            {
                aggregator.GetEvent<EventAggregation.BarcodeScannerDataLocalEvent>().Publish(payload);
            }
            else
            {
                aggregator.GetEvent<EventAggregation.BarcodeScannerDataGlobalEvent>().Publish(payload);
            }
        }

        public void StatusUpdateEvent(object payload)
        {

        }

        public void Open(string subscriber)
        {
            Init();

            if (subscriber != "")
                this.Subscriber = subscriber;

            try
            {

                if (posScanner.State == ControlState.Closed)
                    posScanner.Open();
            }
            catch (Exception ex)
            {
                aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
                   new DeviceFailEventArgs()
                   {
                       DeviceType = Helpers.Enum.DeviceType.BarcodeScanner,
                       Message = ex.Message + " " + ex.StackTrace
                   });

                return;
            }

            try
            {
                if (!posScanner.Claimed)
                    posScanner.Claim(1000);
            }
            catch (Exception ex)
            {
                aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
                  new DeviceFailEventArgs()
                  {
                      DeviceType = Helpers.Enum.DeviceType.BarcodeScanner,
                      Message = ex.Message + " " + ex.StackTrace
                  });

                return;
            }

            try
            {
                if (!posScanner.DeviceEnabled)
                    posScanner.DeviceEnabled = true;
            }
            catch (Exception ex)
            {
                aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
                  new DeviceFailEventArgs()
                  {
                      DeviceType = Helpers.Enum.DeviceType.BarcodeScanner,
                      Message = ex.Message + " " + ex.StackTrace
                  });

                return;
            }

            try
            {
                if (!posScanner.DataEventEnabled)
                    posScanner.DataEventEnabled = true;
            }
            catch (Exception ex)
            {
                aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
                  new DeviceFailEventArgs()
                  {
                      DeviceType = Helpers.Enum.DeviceType.BarcodeScanner,
                      Message = ex.Message + " " + ex.StackTrace
                  });

                return;
            }

            try
            {
                if (!posScanner.DecodeData)
                    posScanner.DecodeData = true;
            }
            catch (Exception ex)
            {
                aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
                  new DeviceFailEventArgs()
                  {
                      DeviceType = Helpers.Enum.DeviceType.BarcodeScanner,
                      Message = ex.Message + " " + ex.StackTrace
                  });

                return;
            }

        }

        public void Close()
        {
            Subscriber = "";
        }

        public void Dispose()
        {
            if (posScanner != null)
            {
                try
                {
                    posScanner.DeviceEnabled = false;
                }
                catch { }

                try
                {
                    posScanner.Close();
                }
                catch { }
            }
        }
    }
}
