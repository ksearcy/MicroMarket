using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.CardProcessor;
using Microsoft.Practices.Composite.Events;
using deORO.EventAggregation;
using IDTechSDK;
using deORO.Helpers;
using System.Threading;
using OTI.Utilities;

namespace deORO.CardReader
{
    public class IDTECH : ICardReader
    {
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        private static IDTECH instance;

        private decimal Amount;

        string TransactionType;

        public static IDTECH Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new IDTECH();
                }

                return instance;
            }
        }

        IDTECH()
        {
            Init();
        }

        public void Init()
        {
            IDT_Vendi.setCallback(MessageCallBack);

            IDT_Vendi.useUSB();

            //================= ACTUAL SDK VERSION 1.2.22.0==========
            //string version = IDT_Vendi.SDK_Version();

        }

        public void SetParams(decimal payload)
        {

            try
            {
                TransactionType = null;
                Amount = payload;
                byte[] additionalTags = new byte[] { };
                RETURN_CODE rt = IDT_Vendi.SharedController.ctls_startTransaction(Convert.ToDouble(Amount), 0, 2, 0, 40, additionalTags);
                if (rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                {

                    if (rt != RETURN_CODE.RETURN_CODE_MISSING_DLL)
                    {
                        if (rt != RETURN_CODE.RETURN_CODE_BUSY)
                        {

                            try
                            {
                                IDT_Vendi.SharedController.ctls_cancelTransaction();
                                rt = IDT_Vendi.SharedController.ctls_startTransaction(Convert.ToDouble(Amount), 0, 2, 0, 40, additionalTags);
                                if (rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                                {
                                    aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                                    {
                                        Event = "CreditCardTransactionFailed" + " - " + rt,
                                        DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                                        Amount = (decimal)0.0,
                                        Code = "EKE"
                                    });

                                    aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish(rt.ToString());
                                }
                                else
                                {
                                    DialogViewService.ShowDialog("Credit Card", "Please hit OK and swipe your card");
                                }

                            }
                            catch (Exception ex)
                            {
                                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                                {
                                    Event = "CreditCardTransactionFailed" + " - " + ex.ToString(),
                                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                                    Amount = (decimal)0.0,
                                    Code = "EKE"
                                });

                                aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish(ex.ToString());
                                throw;
                            }

                        }
                        else
                        {

                            Global.IDTECHReaderBusyCount += 1;

                            if (Global.IDTECHReaderBusyCount != 3)
                            {
                                try
                                {
                                    IDT_Vendi.SharedController.ctls_cancelTransaction();
                                    rt = IDT_Vendi.SharedController.ctls_startTransaction(Convert.ToDouble(Amount), 0, 2, 0, 40, additionalTags);
                                    if (rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                                    {

                                        aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                                        {
                                            Event = "CreditCardTransactionFailed" + " - " + rt,
                                            DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                                            Amount = (decimal)0.0,
                                            Code = "EKE"
                                        });

                                        this.Reset();
                                        aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Reader busy, select credit card option again");

                                    }
                                    else
                                    {
                                        DialogViewService.ShowDialog("Credit Card", "Please hit OK and swipe your card");
                                    }

                                }
                                catch (Exception ex)
                                {
                                    aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                                    {
                                        Event = "CreditCardTransactionFailed" + " - " + ex.ToString(),
                                        DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                                        Amount = (decimal)0.0,
                                        Code = "EKE"
                                    });

                                    this.Reset();
                                    aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Reader busy, select credit card option again");

                                    throw;
                                }
                            }
                            else
                            {

                                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                                {
                                    Event = "CreditCardTransactionFailed" + " - App Shut Down Because of Busy Reader",
                                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                                    Amount = (decimal)0.0,
                                    Code = "EKE"
                                });
                                
                                App.Current.Shutdown();

                            }

                        }
                    }
                    else
                    {

                        aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish("Credit card reader not connected");
                    }

                }

            }
            catch (Exception ex)
            {
                aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish(ex.Message);
            }
        }

        public void Open()
        {

        }

        public void Close()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (TransactionType != "Swipe")
            {
                try
                {
                    Amount = 0;
                    IDT_Vendi.SharedController.ctls_cancelTransaction();
                }
                catch
                { }
            }
            else
            {
                Amount = 0;
            }


        }

        public void Reset()
        {
            IDT_Vendi.setCallback(MessageCallBack);

            IDT_Vendi.useUSB();
        }

        public void Authorize(object payload)
        {

        }

        public void DataEvent(object payload)
        {

        }

        public void ErrorEvent(object payload)
        {

        }

        private void MessageCallBack(IDTechSDK.IDT_DEVICE_Types type, DeviceState state, byte[] data, IDTTransactionData cardData, EMV_Callback emvCallback, RETURN_CODE transactionResultCode)
        {
            switch (state)
            {
                case DeviceState.ToConnect:
                    //A connection attempt is starting for IDT_DEVICE_TYPES type
                    break;
                case DeviceState.DefaultDeviceTypeChange:
                    //The SDK is changing the default device to IDT_DEVICE_TYPES type
                    break;
                case DeviceState.Connected:
                    //A connection has been made to IDT_DEVICE_TYPES type
                    break;
                case DeviceState.Disconnected:
                    //A disconnection has occured with IDT_DEVICE_TYPES type
                    aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                    {
                        Event = "CreditCardTransactionFailed" + " - " + "A disconnection has occured with IDT_DEVICE_TYPES  type",
                        DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                        Amount = (decimal)0.0,
                        Code = "EKE"
                    });
                    break;
                case DeviceState.ConnectionFailed:
                    //A connection attempt has failed for IDT_DEVICE_TYPES type
                    aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                    {
                        Event = "CreditCardTransactionFailed" + " - " + "A connection attempt has failed for IDT_DEVICE_TYPES type",
                        DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                        Amount = (decimal)0.0,
                        Code = "EKE"
                    });
                    break;
                case DeviceState.TransactionData:
                    //Transaction data is beign returned in IDTTransactionData cardData
                    displayCardData(cardData);
                    break;
                case DeviceState.DataReceived:
                    //Low-level data received  for IDT_DEVICE_TYPES type
                    //SetOutputTextLog(" IN: " + Common.getHexStringFromBytes(data));
                    break;
                case DeviceState.DataSent:
                    //Low-level data sent  for IDT_DEVICE_TYPES type
                    // SetOutputTextLog(" OUT: " + Common.getHexStringFromBytes(data));
                    break;
                case DeviceState.CommandTimeout:
                    //DialogViewService.ShowDialog("Credit Card", "Timeout");
                    //aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                    //{
                    //    Event = "CreditCardTransactionFailed" + " - " + "Timeout",
                    //    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                    //    Amount = (decimal)0.0,
                    //    Code = "EKE"
                    //});
                    break;
                case DeviceState.CardAction:
                    if (data != null & data.Length > 0)
                    {
                        CARD_ACTION action = (CARD_ACTION)data[0];
                        StringBuilder sb = new StringBuilder(" ");
                        if ((action & CARD_ACTION.CARD_ACTION_INSERT) == CARD_ACTION.CARD_ACTION_INSERT) DialogViewService.ShowDialog("Card Action Request:", "INSERT");
                        if ((action & CARD_ACTION.CARD_ACTION_REINSERT) == CARD_ACTION.CARD_ACTION_REINSERT) DialogViewService.ShowDialog("Card Action Request:", "REINSERT");
                        if ((action & CARD_ACTION.CARD_ACTION_REMOVE) == CARD_ACTION.CARD_ACTION_REMOVE) DialogViewService.ShowDialog("Card Action Request:", "REMOVE");
                        if ((action & CARD_ACTION.CARD_ACTION_SWIPE) == CARD_ACTION.CARD_ACTION_SWIPE) DialogViewService.ShowDialog("Card Action Request:", "SWIPE");
                        if ((action & CARD_ACTION.CARD_ACTION_SWIPE_AGAIN) == CARD_ACTION.CARD_ACTION_SWIPE_AGAIN) DialogViewService.ShowDialog("Card Action Request:", "SWIPE AGAIN");
                        if ((action & CARD_ACTION.CARD_ACTION_TAP) == CARD_ACTION.CARD_ACTION_TAP) DialogViewService.ShowDialog("Card Action Request", "TAP");
                        if ((action & CARD_ACTION.CARD_ACTION_TAP_AGAIN) == CARD_ACTION.CARD_ACTION_TAP_AGAIN) DialogViewService.ShowDialog("Card Action Request", "TAP AGAIN");
                        // SetOutputText(sb.ToString() + "\n");
                    }
                    break;
                case DeviceState.MSRDecodeError:
                    //Awaiting a swipe for IDT_DEVICE_TYPES type
                    //DialogViewService.ShowDialog("Credit Card", "Card Data Swipe Error");
                    //aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                    //{
                    //    Event = "CreditCardTransactionFailed" + " - " + "Card Data Swipe Error",
                    //    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                    //    Amount = (decimal)0.0,
                    //    Code = "EKE"
                    //});
                    break;
                case DeviceState.SwipeTimeout:
                    //Waiting for swipe timed out
                    //DialogViewService.ShowDialog("Credit Card", "Swipe Timeout");
                    //aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                    //{
                    //    Event = "CreditCardTransactionFailed" + " - " + "Credit Card Swipe Timeout",
                    //    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                    //    Amount = (decimal)0.0,
                    //    Code = "EKE"
                    //});
                    break;
                case DeviceState.TransactionCancelled:
                    //    //Transaction has been cancelled
                    //    aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                    //    {
                    //        Event = "CreditCardTransactionFailed" + " - " + "Transaction has been cancelled",
                    //        DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                    //        Amount = (decimal)0.0,
                    //        Code = "EKE"
                    //    });
                    break;
                case DeviceState.TransactionFailed:
                    //Transaction failed to complete
                    //DialogViewService.ShowDialog("Credit Card", "Transaction Failed, Please try again");
                    //aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                    //{
                    //    Event = "CreditCardTransactionFailed" + " - " + "Transaction Failed",
                    //    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                    //    Amount = (decimal)0.0,
                    //    Code = "EKE"
                    //});
                    break;
                case DeviceState.EMVCallback:
                    //Callback during EMV transaction retrieved from EMV_Callback emvCallback
                    //aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                    //{
                    //    Event = "CreditCardTransactionFailed" + " - " + "Callback during EMV transaction retrieved from EMV_Callback emvCallback",
                    //    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                    //    Amount = (decimal)0.0,
                    //    Code = "EKE"
                    //});
                    break;
                default:
                    break;
            }
        }

        private void displayCardData(IDTTransactionData cardData)
        {
            bool CtlsKSNAvailable = false;

            if (cardData.msr_KSN != null)
            {
                CreditCardData data = new CreditCardData();

                data.CardType = "C";

                TransactionType = "Swipe";

                data.Track2EncryptedData = cardData.msr_encTrack2;

                if (cardData.msr_track2Length != null && cardData.msr_track2Length != 0)
                {
                    data.EncryptedDataLength = cardData.msr_track2Length;
                }
                else
                {
                    data.EncryptedDataLength = 38;
                }

                data.AdditionalSecurityInformation = Encoding.ASCII.GetBytes(Common.getHexStringFromBytes(cardData.msr_KSN));

                aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Publish(data);


            }
            else if (cardData.emv_unencryptedTags != null)
            {

                Dictionary<string, string> UnecryptedTags = Common.processTLVUnencrypted(cardData.emv_unencryptedTags);

                foreach (KeyValuePair<string, string> UnecryptedTagKey in UnecryptedTags)
                {
                    switch (UnecryptedTagKey.Key)
                    {
                        case "FFEE12":
                            CtlsKSNAvailable = true;
                            break;
                        default:
                            break;
                    }

                }

                if (CtlsKSNAvailable == true)
                {
                    CreditCardData data = new CreditCardData();

                    data.CardType = "R";

                    TransactionType = "Ctls";

                    foreach (KeyValuePair<string, string> UnecryptedTagKey in UnecryptedTags)
                    {
                        switch (UnecryptedTagKey.Key)
                        {
                            case "FFEE12":
                                data.AdditionalSecurityInformation = Encoding.ASCII.GetBytes(UnecryptedTagKey.Value);
                                break;
                            case "DFEF4C":
                                byte[] LengthTagArray = Common.getByteArray(UnecryptedTagKey.Value);
                                int LengthTagArrayBytePosition = 0;
                                foreach (byte LengthTagArrayElement in LengthTagArray)
                                {
                                    LengthTagArrayBytePosition += 1;
                                    if (LengthTagArrayBytePosition == 2)
                                    {
                                        int CtlsEncryptedDataLength = Convert.ToInt32(LengthTagArrayElement.ToString());

                                        if (CtlsEncryptedDataLength != null && CtlsEncryptedDataLength != 0)
                                        {
                                            data.EncryptedDataLength = CtlsEncryptedDataLength;
                                        }
                                        else
                                        {

                                            data.EncryptedDataLength = 40;

                                        }

                                    }
                                }
                                break;
                            case "DFEF4D":
                                data.Track2EncryptedData = Common.getByteArray(UnecryptedTagKey.Value);
                                break;
                            default:
                                break;
                        }

                    }


                    aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Publish(data);

                }
                else
                {

                    try
                    {

                        byte[] additionalTags = new byte[] { };
                        RETURN_CODE rt = IDT_Vendi.SharedController.ctls_startTransaction(Convert.ToDouble(Amount), 0, 2, 0, 40, additionalTags);
                        if (rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                        {
                            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                            {
                                Event = "CreditCardTransactionFailed" + " - " + "Card Data Swipe Error, Please Try Again",
                                DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                                Amount = (decimal)0.0,
                                Code = "EKE"
                            });
                        }

                        DialogViewService.ShowDialog("Credit Card", "Failed Data Reading, Please try again");

                    }
                    catch (Exception)
                    {
                        try
                        {
                            IDT_Vendi.SharedController.ctls_cancelTransaction();
                            byte[] additionalTags = new byte[] { };
                            RETURN_CODE rt = IDT_Vendi.SharedController.ctls_startTransaction(Convert.ToDouble(Amount), 0, 2, 0, 40, additionalTags);
                            if (rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                            {
                                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                                {
                                    Event = "CreditCardTransactionFailed" + " - " + "Card Data Swipe Error, Please Try Again",
                                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                                    Amount = (decimal)0.0,
                                    Code = "EKE"
                                });

                            }

                            DialogViewService.ShowDialog("Credit Card", "Failed Data Reading, Please try again");


                        }
                        catch (Exception ex)
                        {
                            aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish("Reader busy, select credit card option again");
                            throw;
                        }


                        throw;
                    }

                }

            }
            else
            {
                try
                {

                    RETURN_CODE rt = IDT_Vendi.SharedController.ctls_cancelTransaction();
                    if (rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
                    {
                        aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                        {
                            Event = "CreditCardTransactionFailed" + " - " + rt,
                            DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                            Amount = (decimal)0.0,
                            Code = "EKE"
                        });

                    }

                    aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish("Reader busy, select credit card option again");


                }
                catch (Exception ex)
                {
                    aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish("Reader busy, select credit card option again");
                    throw;
                }
            }
        }

        private string tlvToValues(byte[] tlv)
        {
            string text = "";
            Dictionary<string, string> dict = Common.processTLVUnencrypted(tlv);
            foreach (KeyValuePair<string, string> kvp in dict) text += kvp.Key + ": " + kvp.Value + "\r\n";
            return text;
        }

        //private void displayCardData(IDTTransactionData cardData)
        //{


        //    if (cardData.captureCardType == CAPTURE_CARD_TYPE.CAPTURE_CARD_TYPE_CTLS_EMV || cardData.captureCardType == CAPTURE_CARD_TYPE.CAPTURE_CARD_TYPE_CTLS_MSD)
        //    {
        //        if (cardData.emv_resultCode == EMV_RESULT_CODE.EMV_RESULT_CODE_GO_ONLINE_CTLS)
        //        {

        //            if (cardData.emv_unencryptedTags != null)
        //            {

        //                CreditCardData data = new CreditCardData();

        //                data.CardType = "R";

        //                Dictionary<string, string> UnecryptedTags = Common.processTLVUnencrypted(cardData.emv_unencryptedTags);
        //                foreach (KeyValuePair<string, string> UnecryptedTagKey in UnecryptedTags)
        //                {
        //                    switch (UnecryptedTagKey.Key)
        //                    {
        //                        case "FFEE12":
        //                            data.AdditionalSecurityInformation = Encoding.ASCII.GetBytes(UnecryptedTagKey.Value);
        //                            break;
        //                        case "DFEF4C":
        //                            byte[] LengthTagArray = Common.getByteArray(UnecryptedTagKey.Value);
        //                            int LengthTagArrayBytePosition = 0;
        //                            foreach (byte LengthTagArrayElement in LengthTagArray)
        //                            {
        //                                LengthTagArrayBytePosition += 1;
        //                                if (LengthTagArrayBytePosition == 2)
        //                                {
        //                                    data.EncryptedDataLength = Convert.ToInt32(LengthTagArrayElement.ToString());
        //                                }
        //                            }
        //                            break;
        //                        case "DFEF4D":
        //                            data.Track2EncryptedData = Common.getByteArray(UnecryptedTagKey.Value);
        //                            break;
        //                        default:
        //                            break;
        //                    }

        //                }





        //                aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Publish(data);


        //            }
        //            else
        //            {

        //                try
        //                {

        //                    byte[] additionalTags = new byte[] { 0x8E, 0x5A };
        //                    RETURN_CODE rt = IDT_Vendi.SharedController.ctls_startTransaction(Amount, 0, 2, 0, 180, additionalTags);
        //                    if (rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
        //                    {
        //                        aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish(rt.ToString());
        //                        aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
        //                        {
        //                            Event = "CreditCardTransactionFailed" + " - " + "Card Data Swipe Error, Please Try Again",
        //                            DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
        //                            Amount = (decimal)0.0,
        //                            Code = "EKE"
        //                        });
        //                    }

        //                    //DialogViewService.ShowDialog("Credit Card", "Card Data Swipe Error, Please Try Again");
        //                }
        //                catch (Exception)
        //                {
        //                    try
        //                    {
        //                        IDT_Vendi.SharedController.ctls_cancelTransaction();

        //                        byte[] additionalTags = new byte[] { 0x8E, 0x5A };
        //                        RETURN_CODE rt = IDT_Vendi.SharedController.ctls_startTransaction(Amount, 0, 2, 0, 180, additionalTags);
        //                        if (rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
        //                        {
        //                            aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish(rt.ToString());
        //                            aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
        //                            {
        //                                Event = "CreditCardTransactionFailed" + " - " + "Card Data Swipe Error, Please Try Again",
        //                                DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
        //                                Amount = (decimal)0.0,
        //                                Code = "EKE"
        //                            });
        //                        }

        //                        //DialogViewService.ShowDialog("Credit Card", "Card Data Swipe Error, Please Try Again");

        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish(ex.Message);

        //                        throw;
        //                    }


        //                    throw;
        //                }


        //            }


        //        }
        //        else
        //        {

        //            try
        //            {

        //                byte[] additionalTags = new byte[] { 0x8E, 0x5A };
        //                RETURN_CODE rt = IDT_Vendi.SharedController.ctls_startTransaction(Amount, 0, 2, 0, 180, additionalTags);
        //                if (rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
        //                {
        //                    aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish(rt.ToString());
        //                    aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
        //                    {
        //                        Event = "CreditCardTransactionFailed" + " - " + "Card Data Swipe Error, Please Try Again",
        //                        DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
        //                        Amount = (decimal)0.0,
        //                        Code = "EKE"
        //                    });
        //                }

        //                //DialogViewService.ShowDialog("Credit Card", "Card Data Swipe Error, Please Try Again");
        //            }
        //            catch (Exception)
        //            {
        //                try
        //                {
        //                    IDT_Vendi.SharedController.ctls_cancelTransaction();

        //                    byte[] additionalTags = new byte[] { 0x8E, 0x5A };
        //                    RETURN_CODE rt = IDT_Vendi.SharedController.ctls_startTransaction(Amount, 0, 2, 0, 180, additionalTags);
        //                    if (rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
        //                    {
        //                        aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish(rt.ToString());
        //                        aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
        //                        {
        //                            Event = "CreditCardTransactionFailed" + " - " + "Card Data Swipe Error, Please Try Again",
        //                            DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
        //                            Amount = (decimal)0.0,
        //                            Code = "EKE"
        //                        });
        //                    }

        //                    //DialogViewService.ShowDialog("Credit Card", "Card Data Swipe Error, Please Try Again");

        //                }
        //                catch (Exception ex)
        //                {
        //                    aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish(ex.Message);

        //                    throw;
        //                }


        //                throw;
        //            }


        //        }
        //    }
        //    else if (cardData.captureCardType == CAPTURE_CARD_TYPE.CAPTURE_CARD_TYPE_MSR)
        //    {

        //        if (cardData.msr_encTrack2 != null)
        //        {

        //            CreditCardData data = new CreditCardData();

        //            data.CardType = "C";

        //            data.Track2EncryptedData = cardData.msr_encTrack2;
        //            data.EncryptedDataLength = cardData.msr_track2Length;
        //            data.AdditionalSecurityInformation = Encoding.ASCII.GetBytes(Common.getHexStringFromBytes(cardData.msr_KSN));

        //            aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Publish(data);
        //        }
        //        else
        //        {

        //            try
        //            {

        //                byte[] additionalTags = new byte[] { 0x8E, 0x5A };
        //                RETURN_CODE rt = IDT_Vendi.SharedController.ctls_startTransaction(Amount, 0, 2, 0, 180, additionalTags);
        //                if (rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
        //                {
        //                    aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish(rt.ToString());
        //                    aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
        //                    {
        //                        Event = "CreditCardTransactionFailed" + " - " + "Card Data Swipe Error, Please Try Again",
        //                        DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
        //                        Amount = (decimal)0.0,
        //                        Code = "EKE"
        //                    });
        //                }

        //            }
        //            catch (Exception)
        //            {
        //                try
        //                {
        //                    IDT_Vendi.SharedController.ctls_cancelTransaction();


        //                    byte[] additionalTags = new byte[] { 0x8E, 0x5A };
        //                    RETURN_CODE rt = IDT_Vendi.SharedController.ctls_startTransaction(Amount, 0, 2, 0, 180, additionalTags);
        //                    if (rt != RETURN_CODE.RETURN_CODE_DO_SUCCESS)
        //                    {
        //                        aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish(rt.ToString());
        //                        aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
        //                        {
        //                            Event = "CreditCardTransactionFailed" + " - " + "Card Data Swipe Error, Please Try Again",
        //                            DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
        //                            Amount = (decimal)0.0,
        //                            Code = "EKE"
        //                        });
        //                    }


        //                }
        //                catch (Exception ex)
        //                {
        //                    aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish(ex.Message);

        //                    throw;
        //                }


        //                throw;
        //            }


        //        }
        //    }
        //    else
        //    {
        //        aggregator.GetEvent<EventAggregation.CreditCardSetParamsFailEvent>().Publish("Please Select Credit Card Option Again");
        //    }
        //}




    }

}
