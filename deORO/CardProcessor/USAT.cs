using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using deORO.EventAggregation;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;
using IDTechSDK;

namespace deORO.CardProcessor
{
    public class USAT : ICreditCardProcessor
    {
        private ePortConnect.ecPortTypeClient client = new ePortConnect.ecPortTypeClient("ecHttpSoap12Endpoint", Global.CreditcardProcessorUrl);
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        //public async Task ProcessSale(CreditCardData cardData, decimal amount, string transactionDetails, string zipCode = "")
        //{
        //    try
        //    {
        //        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
        //        System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>
        //        {
        //            return true;
        //        };


        //        long tranId = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / 1000;

        //        ePortConnect.ECAuthResponse response;

        //        string encryptedTrack;
        //        string additionalInfo;


        //        if (Global.CardReaderMake == deORO.Helpers.Enum.CreditReaderMake.OTI.ToString())
        //        {
        //            encryptedTrack = BitConverter.ToString(cardData.Track2EncryptedData).Replace("-", string.Empty);
        //            additionalInfo = System.Text.Encoding.ASCII.GetString(cardData.AdditionalSecurityInformation);
        //        }
        //        else
        //        {
        //            if (Global.Encoding.ToLower() == "ascii")
        //            {
        //                encryptedTrack = System.Text.Encoding.ASCII.GetString(cardData.Track2EncryptedData);
        //                additionalInfo = System.Text.Encoding.ASCII.GetString(cardData.AdditionalSecurityInformation);
        //            }
        //            else
        //            {
        //                encryptedTrack = BitConverter.ToString(cardData.Track2EncryptedData).Replace("-", string.Empty);
        //                additionalInfo = BitConverter.ToString(cardData.AdditionalSecurityInformation).Replace("-", string.Empty);
        //            }
        //        }

        //        if (Global.RunMode.ToLower() == "debug")
        //        {
        //            System.Windows.Forms.MessageBox.Show(encryptedTrack);
        //            System.Windows.Forms.MessageBox.Show(additionalInfo);
        //        }

        //        await Task.Run(() =>
        //        {
        //            //response = client.chargeV3_1(Global.CreditcardProcessorUserName, 
        //            //                             Global.CreditcardProcessorPassword, Global.CardReaderSerialNumber,
        //            //                             tranId, Convert.ToInt64(amount * 100), Global.CardReaderType, 35,
        //            //                             encryptedTrack,
        //            //                             additionalInfo,
        //            //                             Global.CardType, "Q", transactionDetails);

        //            response = client.chargeV3_1(Global.CreditcardProcessorUserName,
        //                                         Global.CreditcardProcessorPassword, Global.CardReaderSerialNumber,
        //                                         tranId, Convert.ToInt64(amount * 100), Global.CardReaderType, cardData.EncryptedDataLength,
        //                                         encryptedTrack,
        //                                         additionalInfo,
        //                                         Global.CardType, "Q", transactionDetails);

        //            if (response != null)
        //            {
        //                if (Global.RunMode.ToLower() == "debug")
        //                    System.Windows.Forms.MessageBox.Show(Global.CreditcardProcessorUserName + " " +
        //                                                         Global.CreditcardProcessorPassword + " " +
        //                                                         response.returnCode.ToString());

        //                if (response.returnCode == 2) //Transaction Approved
        //                {
        //                    aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Approved");
        //                }
        //                else if (response.returnCode == 0) //Transaction Failed
        //                {
        //                    aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed" + " " + response.returnMessage);
        //                    aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
        //                    {
        //                        Event = "CreditCardTransactionFailed" + " - " + response.returnMessage,
        //                        DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
        //                        Amount = amount
        //                    });
        //                }
        //                else if (response.returnCode == 3) //Transaction Declined
        //                {
        //                    aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Declined" + " " + response.returnMessage);
        //                    aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
        //                    {
        //                        Event = "CreditCardTransactionDeclined" + " - " + response.returnMessage,
        //                        DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
        //                        Amount = amount
        //                    });
        //                }
        //                else
        //                {
        //                    aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
        //                    aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
        //                    {
        //                        Event = "CreditCardTransactionFailed" + " - " + response.returnMessage,
        //                        DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
        //                        Amount = amount
        //                    });
        //                }
        //            }
        //            else
        //            {
        //                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
        //                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
        //                {
        //                    Event = "CreditCardTransactionFailed - Unknown Error",
        //                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
        //                    Amount = amount,
        //                });
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {

        //        if (Global.RunMode.ToLower() == "debug")
        //            System.Windows.Forms.MessageBox.Show(Global.CreditcardProcessorUserName + " " +
        //                                                 Global.CreditcardProcessorPassword +
        //                                                 " Unknown Error " + ex.Message);

        //        aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
        //        aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
        //        {
        //            Event = "CreditCardTransactionFailed - Unknown Error -" + ex.Message,
        //            DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
        //            Amount = amount
        //        });

        //    }

        //}

        public void ChangePassword(string serialNumber, int updateStatus)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>
                {
                    return true;
                };

                var response = client.processUpdates(userName, password, serialNumber, updateStatus);

                if (response != null)
                {
                    password = response.newPassword;
                    message = response.returnMessage;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }

        public string userName { get; set; }

        public string password { get; set; }

        public string message { get; set; }


        public void ProcessSale(CreditCardData cardData, decimal amount, string transactionDetails = "", string zipCode = "")
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += (a, b) =>
                {

                    try
                    {
                        System.Threading.Thread.Sleep(1000);

                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                        System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) =>
                        {
                            return true;
                        };


                        long tranId = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / 1000;

                        ePortConnect.ECAuthResponse response;

                        string encryptedTrack;
                        string additionalInfo;


                        if (Global.CardReaderMake == deORO.Helpers.Enum.CreditReaderMake.OTI.ToString() )
                        {
                            encryptedTrack = BitConverter.ToString(cardData.Track2EncryptedData).Replace("-", string.Empty);
                            additionalInfo = System.Text.Encoding.ASCII.GetString(cardData.AdditionalSecurityInformation);

                        }else if(Global.CardReaderMake == deORO.Helpers.Enum.CreditReaderMake.IDTECH.ToString()){


                            encryptedTrack = Common.getHexStringFromBytes(cardData.Track2EncryptedData).ToUpper();
                            additionalInfo = System.Text.Encoding.ASCII.GetString(cardData.AdditionalSecurityInformation).ToUpper();

                        }
                        else
                        {
                            if (Global.Encoding.ToLower() == "ascii")
                            {
                                encryptedTrack = System.Text.Encoding.ASCII.GetString(cardData.Track2EncryptedData);
                                additionalInfo = System.Text.Encoding.ASCII.GetString(cardData.AdditionalSecurityInformation);
                            }
                            else
                            {
                                encryptedTrack = BitConverter.ToString(cardData.Track2EncryptedData).Replace("-", string.Empty);
                                additionalInfo = BitConverter.ToString(cardData.AdditionalSecurityInformation).Replace("-", string.Empty);
                            }
                        }

                        if (Global.RunMode.ToLower() == "debug")
                        {
                            //System.Windows.Forms.MessageBox.Show(encryptedTrack);
                            //System.Windows.Forms.MessageBox.Show(additionalInfo);
                        }

                        string CardType = "";

                        if (cardData.CardType != null)
                        {
                            CardType = cardData.CardType;
                        }
                        else {
                            CardType = Global.CardType;
                        }


                        response = client.chargeV3_1(Global.CreditcardProcessorUserName,
                                                     Global.CreditcardProcessorPassword, Global.CardReaderSerialNumber,
                                                     tranId, Convert.ToInt64(amount * 100), Global.CardReaderType, cardData.EncryptedDataLength,
                                                     encryptedTrack,
                                                     additionalInfo,
                                                     CardType, "Q", transactionDetails);

                     

                        if (response != null)
                        {
                            if (Global.RunMode.ToLower() == "debug")
                            {
                                System.Windows.Forms.MessageBox.Show(Global.CreditcardProcessorUserName + " " +
                                                                     Global.CreditcardProcessorPassword + " " +
                                                                     response.returnCode.ToString());
                            }

                            if (response.returnCode == 2) //Transaction Approved
                            {
                                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Approved");
                            }
                            else if (response.returnCode == 0) //Transaction Failed
                            {
                                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed" + " " + response.returnMessage);

                                if (response.returnMessage.Contains("Request failed validation: [Invalid encryptedCardDataHex: ]"))
                                {
                                    response.returnMessage = "Card data read failed, please try again.";
                                }
                                else if (response.returnMessage.Contains("There was no endpoint listening at https://ec.usatech.com:9443/soap/ec that could accept the message"))
                                {
                                    response.returnMessage = "Credit Card Transactions Server Unavailable, Please use other payment option or try later.";
                                }

                                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                                {
                                    Event = "CreditCardTransactionFailed" + " - " + response.returnMessage,
                                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                                    Amount = amount,
                                    Code = "EKE"
                                });
                            }
                            else if (response.returnCode == 3) //Transaction Declined
                            {
                                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Declined" + " " + response.returnMessage);
                                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                                {
                                    Event = "CreditCardTransactionDeclined" + " - " + response.returnMessage,
                                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                                    Amount = amount,
                                    Code = "EKE"
                                });
                            }
                            else 
                            {
                                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");

                                if (response.returnMessage.Contains("Request failed validation: [Invalid encryptedCardDataHex: ]"))
	                            {
                                    response.returnMessage = "Card data read failed, please try again.";
                             	}
                                else if (response.returnMessage.Contains("There was no endpoint listening at https://ec.usatech.com:9443/soap/ec that could accept the message"))
                                {
                                       response.returnMessage = "Credit Card Transactions Server Unavailable, Please use other payment option or try later.";
                                }

                                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                                {
                                    Event = "CreditCardTransactionFailed" + " - " + response.returnMessage,
                                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                                    Amount = amount,
                                    Code = "EKE"
                                });
                            }
                        }
                        else
                        {
                            aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
                            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                            {
                                Event = "CreditCardTransactionFailed - Unknown Error",
                                DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                                Amount = amount,
                                Code = "EKE"
                            });
                        }

                    }
                    catch (Exception ex)
                    {

                        if (Global.RunMode.ToLower() == "debug")
                        {
                            System.Windows.Forms.MessageBox.Show(Global.CreditcardProcessorUserName + " " +
                                                                 Global.CreditcardProcessorPassword +
                                                                 " Unknown Error " + ex.Message);
                        }

                        aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
                        aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                        {
                            Event = "CreditCardTransactionFailed - Unknown Error -" + ex.Message,
                            DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                            Amount = amount,
                            Code = "EKE"
                        });

                    }
                };

            bg.RunWorkerAsync();
        }
    }
}
