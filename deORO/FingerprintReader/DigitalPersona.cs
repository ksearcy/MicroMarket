using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPUruNet;
using Microsoft.Practices.Composite.Events;
using deORO.EventAggregation;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;

namespace deORO.FingerprintReader
{
    public class DigitalPersona : IDisposable
    {

        private List<Fmd> fingerPrints = new List<Fmd>();
        private List<string> userNames = new List<string>();
        private deOROMembershipProvider membership = new deOROMembershipProvider();
        private DPCtlUruNet.IdentificationControl identification = null;

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        private Reader reader = null;

        public DigitalPersona()
        {
            LoadFingerPrints();
        }

        public void Init()
        {
            ReaderCollection readerCollection = ReaderCollection.GetReaders();

            if (readerCollection.Count > 0)
            {
                try
                {
                    reader = readerCollection[0];
                    identification = new DPCtlUruNet.IdentificationControl(reader, fingerPrints,
                                     Helpers.Global.FingerprintThresholdScore, 10, Constants.CapturePriority.DP_PRIORITY_EXCLUSIVE);
                    identification.Name = "dpIdentificationCtrl";
                    identification.OnIdentify += identification_OnIdentify;
                    identification.StartIdentification();
                    
                }
                catch (Exception ex)
                {
                    aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(new DeviceFailEventArgs()
                    {
                        DeviceType = Helpers.Enum.DeviceType.FingerPrintReader,
                        Message = ex.Message + " " + ex.StackTrace
                    });
                }
            }
        }

        public void LoadFingerPrints()
        {
            object[] o = membership.GetAllUsersForFmdIdentification();
            userNames = (List<string>)o[1];
            fingerPrints = (List<Fmd>)o[3];

        }

        void identification_OnIdentify(DPCtlUruNet.IdentificationControl IdentificationControl, IdentifyResult IdentificationResult)
        {
            List<string> unFound = new List<string>();

            try
            {
                if (Helpers.Global.DemoMode)
                {
                    aggregator.GetEvent<EventAggregation.FingerPrintLoginSuccessfulEvent>().Publish(userNames[0]);
                    return;
                }
            }
            catch { }


            if (IdentificationResult.Indexes.Length <= 0)
            {
                //aggregator.GetEvent<EventAggregation.LoginFailEvent>().Publish(deORO.Helpers.Enum.AuthenticationMode.FingerPrint);
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new EventAggregation.TransactionErrorEventArgs
                {
                    DeviceType = Helpers.Enum.DeviceType.Login,
                    Event = "Login Failed. Invalid Fingerprint",
                    ShoppingCartPkid = "FingerPrint",
                    Code = "ZLN"
                });
            }
            else
            {
                for (int i = 0; i < IdentificationResult.Indexes.Length; i++)
                {
                    if (!unFound.Contains(userNames[IdentificationResult.Indexes[i][0]])) // FMD index = 0
                    {
                        unFound.Add(userNames[IdentificationResult.Indexes[i][0]]);
                    }
                }
            }

            if (unFound.Count >= 1)
            {
                aggregator.GetEvent<EventAggregation.FingerPrintLoginSuccessfulEvent>().Publish(unFound[0]);
            }
        }

        public void Dispose()
        {
            if (identification != null)
            {
                reader.Reset();
                identification.StopIdentification();
                identification.OnIdentify -= identification_OnIdentify;
                identification = null;
            }
        }
    }
}
