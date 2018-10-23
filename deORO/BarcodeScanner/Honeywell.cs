using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using deORO.Helpers;
using Microsoft.PointOfService;
using Microsoft.Practices.Composite.Events;
using deORO.EventAggregation;

namespace deORO.BarcodeScanner
{
    public class Honeywell
    {
        private DeviceInfo deviceInfo;
        private Scanner posScanner;

        public event EventHandler<ObjectArrEventArgs> DeviceScanner_DataEvent;
        public event EventHandler<ObjectArrEventArgs> DeviceScanner_ErrorEvent;
        public event EventHandler<ObjectArrEventArgs> DeviceScanner_StatusUpdateEvent;

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        public ISynchronizeInvoke EventSyncInvoke { get; set; }

        private static Honeywell instance;

        public static Honeywell Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Honeywell();
                }
                return instance;
            }
        }

        //public static Honeywell GetBarcodeScanner()
        //{
        //    if (honeywell == null)
        //    {
        //        honeywell = new Honeywell();
        //    }
            
        //    return honeywell;
        //}

        private Honeywell()
        {
            Init();
        }

        private string subscriber;

        public string Subscriber
        {
            get { return subscriber; }
            set { subscriber = value; }
        }

         private void RaiseEvent_ScannerDataEvent(object[] eventData)
        {
            ObjectArrEventArgs e = new ObjectArrEventArgs(eventData);
            EventHandler<ObjectArrEventArgs> evnt = this.DeviceScanner_DataEvent;

            // check for no subscribers
            if (evnt == null)
                return;

            if (EventSyncInvoke == null)
                evnt(this, e);
            else
                EventSyncInvoke.Invoke(evnt, new object[] { this, e });
        }

        private void RaiseEvent_ScannerErrorEvent(object[] eventData)
        {
            ObjectArrEventArgs e = new ObjectArrEventArgs(eventData);
            EventHandler<ObjectArrEventArgs> evnt = this.DeviceScanner_ErrorEvent;

            // check for no subscribers
            if (evnt == null)
                return;

            if (EventSyncInvoke == null)
                evnt(this, e);
            else
                EventSyncInvoke.Invoke(evnt, new object[] { this, e });
        }

        private void RaiseEvent_ScannerStatusUpdateEvent(object[] eventData)
        {
            ObjectArrEventArgs e = new ObjectArrEventArgs(eventData);
            EventHandler<ObjectArrEventArgs> evnt = this.DeviceScanner_StatusUpdateEvent;

            // check for no subscribers
            if (evnt == null)
                return;

            if (EventSyncInvoke == null)
                evnt(this, e);
            else
                EventSyncInvoke.Invoke(evnt, new object[] { this, e });
        }

        private void Init()
        {
            PosExplorer posExplorer = new PosExplorer();
            deviceInfo = posExplorer.GetDevice(DeviceType.Scanner, Global.BarcodeReaderDeviceName);

            if (deviceInfo != null)
            {
                try
                {
                    PosDevice posDevice = posExplorer.CreateInstance(deviceInfo);
                    posScanner = (Scanner)posDevice;

                    posScanner.StatusUpdateEvent += posScanner_StatusUpdateEvent;
                    posScanner.ErrorEvent += posScanner_ErrorEvent;
                    posScanner.DataEvent += posScanner_DataEvent;

                    try
                    {
                        if (posScanner.Claimed)
                        {
                            posScanner.DeviceEnabled = false;
                            posScanner.Release();
                            posScanner.Close();
                        }
                    }
                    catch { }

                    posScanner.Open();
                    posScanner.Claim(1000);
                    posScanner.DeviceEnabled = true;
                    posScanner.DataEventEnabled = true;
                    posScanner.DecodeData = true;
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

        void posScanner_DataEvent(object sender, DataEventArgs e)
        {
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

                //Dispose();
                //Init();

                if (Subscriber == "Local")
                {
                    aggregator.GetEvent<EventAggregation.BarcodeScannerDataLocalEvent>().Publish(text.Replace("\0", " "));
                    //RaiseEvent_ScannerDataEvent(new object[] { "Scanner", Global.BarcodeReaderDeviceName, text.Replace("\0", " ") });
                }
                else
                {
                    aggregator.GetEvent<EventAggregation.BarcodeScannerDataGlobalEvent>().Publish(text.Replace("\0", " "));
                }

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
     
        public void Dispose()
        {
            if (posScanner != null)
            {
                posScanner.StatusUpdateEvent -= posScanner_StatusUpdateEvent;
                posScanner.ErrorEvent -= posScanner_ErrorEvent;
                posScanner.DataEvent -= posScanner_DataEvent;

                if (posScanner.DeviceEnabled)
                    posScanner.DeviceEnabled = false;

                try
                {
                    posScanner.Release();
                }
                catch { }

                try
                {
                    posScanner.Close();
                }
                catch { }

                posScanner = null;
            }
        }
    }
}
