using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OTI.Comm;
using OTI.Utilities;
using OTI.Framework.Saturn;
using OTI.Framework.Saturn.Choices;
using deORO.CardProcessor;
using Microsoft.Practices.Composite.Events;
using deORO.EventAggregation;

namespace deORO.CardReader
{
    public class OTI : ICardReader
    {
        Commands saturn;
        CommDll comDll = null;
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        private static OTI instance;

        public static OTI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new OTI();
                }
                return instance;
            }
        }

        public OTI()
        {
            Open();
            Init();
            Close();
        }

        private void Init()
        {
            try
            {
                if (saturn != null)
                {
                    saturn.Do(ConfigTags.ResetPCD, null);
                }
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

        void Events_TransactionPerformed(List<TLV> tlvList, byte[] clearingTemplate)
        {
            CreditCardData data = new CreditCardData();

            foreach (TLV item in tlvList)
            {
                if (item.intTag == (int)ConfigTags.EncryptionTemplate)
                {
                    //data.Track2EncryptedData = Encoding.ASCII.GetBytes(item.strValue);
                    data.Track2EncryptedData = clearingTemplate;
                    data.EncryptedDataLength = clearingTemplate.Length;
                }
                else if (item.intTag == (int)ConfigTags.Ksn)
                {
                    data.AdditionalSecurityInformation = Encoding.ASCII.GetBytes(item.strValue);
                }
               
            }

            aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Publish(data);
        }
        
        void saturn_PcdEventHandler(global::OTI.Framework.Saturn.Model.ReceivedFrame frame)
        {

        }

        void Events_PICCRemoved(TLV tlv)
        {

        }

        void Events_PollEmvTerminated(TLV[] tlv)
        {

        }

        void Events_PendingDataReceived(System.Collections.IList list)
        {
            //CreditCardData data = new CreditCardData();

            //foreach (TLV item in list)
            //{
            //    if (item.intTag == (int)ConfigTags.EncryptionTemplate)
            //    {
            //        data.Track2EncryptedData = Encoding.ASCII.GetBytes(item.strValue);
            //        data.EncryptedDataLength = data.Track2EncryptedData.Length;
            //    }
            //    else if (item.intTag == (int)ConfigTags.Ksn)
            //    {
            //        data.AdditionalSecurityInformation = Encoding.ASCII.GetBytes(item.strValue);
            //    }
            //}

            //aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Publish(data);

        }

        public void DataEvent(object payload)
        {
            
        }

        public void ErrorEvent(object payload)
        {
            
        }

        public void Dispose()
        {
            try
            {
                if (saturn != null)
                {
                    saturn.Do(ConfigTags.StopMacro, null);
                    saturn.Events.PendingDataReceived -= Events_PendingDataReceived;
                    saturn.Events.PollEmvTerminated -= Events_PollEmvTerminated;
                    saturn.Events.PICCRemoved -= Events_PICCRemoved;
                    saturn = null;
                }
            }
            catch { }

            try
            {
                ClosePort(comDll);
            }
            catch { }

        }

        private bool OpenPort(byte aPort, out CommDll aComDll)
        {
            aComDll = null;
            CommDll.CommSettings cs = new CommDll.CommSettings();
            cs.BaudRate = CommDll.BaudRateChoice.Baud115200;
            cs.DataBits = CommDll.DataBitsChoice.DataBits8;
            cs.Parity = CommDll.ParityChoice.Parity_None;
            cs.StopBits = CommDll.StopBitsChoice.StopBit_1;

            CommDll c = new CommDll();
            if (!c.OpenRS232(aPort, cs))
            {
                c.Close();
                return false;
            }

            aComDll = c;
            return true;
        }

        private void ClosePort(CommDll aComDll)
        {
            if (aComDll != null)
                aComDll.Close();
        }
        
        public void SetParams(decimal payload)
        {
            try
            {
                Open();

                string amount = payload.ToString().Replace(".", "").PadRight(3, '0').PadLeft(12, '0');
                string date = DateTime.Now.Date.ToString("yyMMdd");
                string time = DateTime.Now.ToString("hhmmss");

                List<TLV> pollEmvParams = new List<TLV>()
                {
                    new TLV((int)ConfigTags.AmountAuthorized,amount),
                    new TLV((int)ConfigTags.TransactionType,"00"),	
                    new TLV((int)ConfigTags.TransactionDate,date),	
                    new TLV((int)ConfigTags.TransactionTime,time),	
                    new TLV((int)ConfigTags.TransactionCurrencyCode,"08 40"),
                    new TLV((int)ConfigTags.EncryptionKeyVariant,"04")
                };

                saturn.PollEMVCmd(pollEmvParams);
            }
            catch(Exception ex)
            {
                aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish(ex.Message);
            }
        }
        
        public void Open()
        {
            if (saturn == null)
            {
                if (!OpenPort(Convert.ToByte(Helpers.Global.CardReaderCOMPort), out comDll))
                {
                    aggregator.GetEvent<EventAggregation.DeviceOpenFailEvent>().Publish(
                        new DeviceFailEventArgs()
                        {
                            DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                            Message = "Unable to Open OTI Reader"
                        });

                }
                else
                {
                    saturn = new Commands(comDll);
                    saturn.Events.PendingDataReceived += Events_PendingDataReceived;
                    saturn.Events.PollEmvTerminated += Events_PollEmvTerminated;
                    saturn.Events.PICCRemoved += Events_PICCRemoved;
                    saturn.PcdEventHandler += saturn_PcdEventHandler;
                    saturn.Events.TransactionPerformed += Events_TransactionPerformed;
                }
            }
        }

        public void Close()
        {
            try
            {
                if (saturn != null)
                {
                    try
                    {
                        saturn.Do(ConfigTags.StopMacro, null);
                    }
                    catch { }
                    try
                    {
                        ClosePort(comDll);
                    }
                    catch { }
                    saturn = null;
                }
            }
            catch { }
        }

        public void Reset()
        {

        }

        public void Authorize(object payload)
        {
            
        }


    }
}
