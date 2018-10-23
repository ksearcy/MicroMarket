using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.EventAggregation;
using Microsoft.Practices.Composite.Events;

namespace deORO.CardProcessor
{
    public class CardKnox : ICreditCardProcessor
    {
        private PaymentEngine.xTransaction.Request request = null;
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        public CardKnox()
        {
            try
            {
                request = new PaymentEngine.xTransaction.Request();
            }
            catch (Exception ex)
            {

            }
        }

        public string userName
        {
            get
            {
                return "";
            }
            set
            {
                userName = value;
            }
        }

        public string password
        {
            get
            {
                return "";
            }
            set
            {
                userName = value;
            }
        }

        public string message
        {
            get
            {
                return "";
            }
            set
            {
                message = value;
            }
        }

        public void ProcessSale(CreditCardData cardData, decimal amount, string transactionDetails = "", string zipCode = "")
        {
            request.xKey = Helpers.Global.CardKnoxKey;
            request.xVersion = Helpers.Global.ApiVersion;
            request.xSoftwareName = System.Reflection.Assembly.GetEntryAssembly().FullName;
            request.xSoftwareVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();

            request.Settings.Device_Name = Helpers.Global.CardReaderDeviceName;
            request.Settings.Device_IP_Address = Helpers.Global.IP;
            request.Settings.Device_IP_Port = Helpers.Global.IPPort;

            request.xCommand = "cc:sale";
            request.xInvoice = Guid.NewGuid().ToString();
            request.xAmount = amount;

            request.Settings.Printer_Name = ""; //Ex: EPSON TM-T20"
            request.Settings.Receipt_Merchant_Disabled = false;
            request.Settings.Receipt_Customer_Disabled = false;

            request.ShowKeyPad = false;
            request.EnableKeyedEntry = true;
            request.ExitFormOnDeviceError = false;

            request.EnableSilentMode = true;
            request.EnableStoredAccount = false;

            bool MyRequire_AVS = false; //Only affects keyed transactions
            bool MyRequire_CVV = false; //Only affects keyed transactions
            bool MyEnableDeviceInsertSwipeTap = true;
            bool MyRequire_Pin = false; //TIP: Skip the signature prompt for small transactions by setting the Require_Signature parameter to false for those transactions.
            bool MyRequire_Signature = false;
            bool MyExitFormIfApproved = true; //Return control to the calling application if transaction is approved
            bool MyExitFormIfNotApproved = false; //Return control to the calling application if transaction is not approved

            var response = request.Manual(
                                            MyRequire_AVS,
                                            MyRequire_CVV,
                                            MyEnableDeviceInsertSwipeTap,
                                            MyRequire_Pin, MyRequire_Signature,
                                            MyExitFormIfApproved,
                                            MyExitFormIfNotApproved,
                                            false);


            //System.Windows.Forms.MessageBox.Show(response.xError + " " + response.xErrorCode);

            if (response.xResult == "A")
            {
                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Approved");
            }
            else if (response.xResult == "D")
            {
                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Declined");
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                {
                    Event = "CreditCardTransactionDeclined",
                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                    Amount = amount,
                    Code = "EKE"
                });
            }
            else
            {
                //aggregator.GetEvent<EventAggregation.CreditCardPaymentCancelEvent>().Publish(null);
                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed - " + response.xErrorCode  + " " + response.xError);
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                {
                    Event = "CreditCardTransactionFailed - " + response.xErrorCode + " " + response.xError,
                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                    Amount = amount,
                    Code = "EKE"
                });
            }
        }

        public void ChangePassword(string serialNumber, int updateStatus)
        {

        }
    }
}
