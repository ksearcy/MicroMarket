using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using deORO.EventAggregation;
using Microsoft.Practices.Composite.Events;

namespace deORO.Marshall
{
    public class VendRequest : IState
    {

        VendStateMachine machine;

        public VendRequest(VendStateMachine machine)
        {
            this.machine = machine;
        }


        public void doAction(MarshallMessage message)
        {
            this.machine.doVendRequest(message);
        }
    }

    public class VendSuccess : IState
    {

        VendStateMachine machine;

        public VendSuccess(VendStateMachine machine)
        {
            this.machine = machine;
        }


        public void doAction(MarshallMessage message)
        {
            this.machine.doVendSuccess(message);
        }
    }

    public class CompleteSession : IState
    {

        VendStateMachine machine;

        public CompleteSession(VendStateMachine machine)
        {
            this.machine = machine;
        }


        public void doAction(MarshallMessage message)
        {
            this.machine.doSessionComplete(message);
        }
    }

    public class EndSession : IState
    {

        VendStateMachine machine;

        public EndSession(VendStateMachine machine)
        {
            this.machine = machine;
        }


        public void doAction(MarshallMessage message)
        {
            this.machine.doEndSession(message);
        }
    }


    public class VendStateMachine : MachineContext
    {
        public VendRequest vendRequest;
        public VendSuccess vendSuccess;
        public CompleteSession completeSession;
        public EndSession endSession;


        VendRequestMessage mdbVendRequest;
        MarshallCardDataMessage cardDataMessage;
        VendSuccessMessage mdbVendSuccess;
        VendFailurelMessage mdbVendFailure;
        VendCancelMessage mdbVendCancel;
        SessionCompleteMessage sessionCompleteMessage;
        MarshallResponeTxMessage marshallResponeTxMessage;
        MarshallMain marshall;
        Timer timer;
        bool timerOn = false;

        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        List<string> messages = new List<string>();

        public VendStateMachine(MarshallMain marshall_)
        {  //Constructor

            endSession = new EndSession(this);
            completeSession = new CompleteSession(this);
            vendSuccess = new VendSuccess(this);
            vendRequest = new VendRequest(this);



            this.marshall = marshall_;
            this.mdbVendSuccess = new VendSuccessMessage();
            this.sessionCompleteMessage = new SessionCompleteMessage();
            this.marshallResponeTxMessage = new MarshallResponeTxMessage();
            this.mdbVendFailure = new VendFailurelMessage();
            this.mdbVendCancel = new VendCancelMessage();

            messages.Clear();
            this.setState(vendRequest);
        }

        public void initMachine()
        {
            this.setState(vendRequest);
        }

        public void doCleanUp()
        {
            if (timer != null)
            {
                timer.Stop();
            }
        }

        public bool MarshallMessageType(MarshallMessage message)
        {
            if (message is MarshallProtocolMessage)
                return true;
            else
                return false;
        }

        public void doVendRequest(MarshallMessage message)
        {
            MarshallInternalMessage systemMessage;
            MarshallProtocolMessage marshallMessage;

            if (MarshallMessageType(message))
            {
                marshallMessage = (MarshallProtocolMessage)message;

                if (marshallMessage.ackResponse)
                {
                    sendAckResponseMessage();
                }

                //sbyte opCode = (sbyte)marshallMessage.OpCode;

                switch (marshallMessage.OpCode)
                {
                    case MdbBeginSessionMessage.MDB_OPCODE:
                        if (marshallMessage.MdbOpcode == MdbVendApproveMessage.VEND_APPROVED)
                        {
                            this.marshall.MachineSerialPort.sendMarshallMessage(this.mdbVendSuccess);
                            this.marshall.MachineSerialPort.sendMarshallMessage(this.sessionCompleteMessage);
                            this.setState(this.endSession);

                            if(!messages.Contains("Approved"))
                            {
                                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Approved");
                                messages.Add("Approved");
                            }
                        }
                        else if (marshallMessage.MdbOpcode == MdbEndSessionMessage.END_SESSION)
                        {
                            messages.Clear();
                            Console.WriteLine("END SESSION");
                        }
                        else if (marshallMessage.MdbOpcode == MdbBeginSessionMessage.BEGIN_SESSION)
                        {
                            if (!messages.Contains("Begin"))
                            {
                                aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Publish(null);
                                messages.Add("Begin");
                                Console.WriteLine("BEGIN SESSION - CHECKING CARD");
                            }

                            //MarshallMessage.beginSession = true;
                            //byte[] bytes = BitConverter.GetBytes(marshall.amount);
                            //this.mdbVendRequest = new VendRequestMessage(bytes);
                            ////this.mdbVendRequest = new VendRequestMessage(new byte[] { 0x00, 0x00 });
                            //this.marshall.MachineSerialPort.sendMarshallMessage(this.mdbVendRequest);

                            //Func<CardTypeEnum, bool> IsDebitOrCreditCard = p =>
                            //{
                            //    return new[]
                            //    {
                            //        CardTypeEnum.MAESTRO,
                            //        CardTypeEnum.MASTER_CARD,
                            //        CardTypeEnum.VISA,
                            //        CardTypeEnum.CHINA_UNION_PAY, // Chinese
                            //        CardTypeEnum.INTERAC // Japan
                            //    }.Contains(p);
                            //};

                            //if (MarshallCardDataMessage.GetCardType() == CardTypeEnum.PROPRIETARY)
                            //{
                            //    if (MarshallCardDataMessage.GetCardEntryType() == CardEntryTypeEnum.MIFARE)
                            //    {
                            //        if (!timerOn)
                            //        {
                            //            timer = new Timer(10000);
                            //            timer.Elapsed += (object sender, ElapsedEventArgs e) =>
                            //            {
                            //                if (this.getState() == this.vendRequest)
                            //                {
                            //                    Console.WriteLine("TIMEOUT VEND REQUEST EXPIRED");
                            //                    this.marshall.MachineSerialPort.sendInternalMessage(MarshallInternalMessage.VEND_INTERNAL);
                            //                    this.setState(completeSession);
                            //                    timer.Stop();
                            //                    timerOn = false;
                            //                }
                            //            };
                            //            timer.Start();
                            //            timerOn = true;
                            //        }

                            //        //this.mdbVendRequest = new VendRequestMessage(new byte[] { 0x00, 0x00 });
                            //        this.mdbVendRequest = new VendRequestMessage(bytes);
                            //        this.marshall.MachineSerialPort.sendMarshallMessage(this.mdbVendRequest);

                            //        byte[] authBytes = null;
                            //        bool success = true;// call web service, or db or something

                            //        if (success)
                            //        {
                            //            authBytes = new byte[] { 8, 1, 0 }; // approved
                            //        }
                            //        else
                            //        {
                            //            authBytes = new byte[] { 8, 1, 1 }; // denied
                            //        }

                            //        // Send auth message
                            //        var cardMessage = new MarshallCardAuthMessage(
                            //            MarshallCardDataMessage.CardType,
                            //            MarshallCardDataMessage.CardEntryMode,
                            //            MarshallCardDataMessage.ProprieteryCardUID,
                            //            authBytes
                            //        );

                            //        this.marshall.MachineSerialPort.sendMarshallMessage(cardMessage);

                            //        this.setState(this.vendSuccess);
                            //    }
                            //}
                            //else if (IsDebitOrCreditCard(MarshallCardDataMessage.GetCardType()))
                            //{
                            //    //// vend request
                            //    var cardMessage = new MarshallCardAuthMessage(
                            //        new byte[] { 2 },
                            //        new byte[] { 2 },
                            //        new byte[] { },
                            //        new byte[] { 8, 1, 0 });
                            //    //marshall handles validating credit cards
                            //    this.marshall.MachineSerialPort.sendMarshallMessage(cardMessage);

                            //    this.setState(this.vendSuccess);


                            //}
                            //else
                            //{
                            //    this.marshall.MachineSerialPort.sendInternalMessage(MarshallInternalMessage.VEND_INTERNAL);
                            //    this.setState(completeSession);
                            //}
                        }
                        else if (marshallMessage.MdbOpcode == MdbCancelSessionMessage.CANCEL_SESSION)
                        {
                            Console.WriteLine("VEND CANCELLED BY AMIT - SENDING SESSION COMPLETE");

                            if (timerOn)
                            {
                                timer.Stop();
                                timerOn = false;
                            }

                            this.marshall.MachineSerialPort.sendInternalMessage(MarshallInternalMessage.VEND_INTERNAL);
                            this.setState(completeSession);

                            if (!messages.Contains("Failed"))
                            {
                                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
                                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                                {
                                    Event = "CreditCardTransactionFailed",
                                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                                    Amount = marshall.amount == 0 ? 0 : marshall.amount / 100
                                });

                                messages.Add("Failed");
                            }
                        }
                        else if (marshallMessage.MdbOpcode == MdbVendDeniedMessage.VEND_DENIED)
                        {
                            Console.WriteLine("RECEIVED VEND DENIED IN VEND REQUEST");
                            this.marshall.MachineSerialPort.sendInternalMessage(MarshallInternalMessage.VEND_INTERNAL);
                            this.setState(completeSession);

                            if (!messages.Contains("Failed"))
                            {
                                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
                                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                                {
                                    Event = "CreditCardTransactionFailed",
                                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                                    Amount = marshall.amount == 0 ? 0 : marshall.amount / 100
                                });

                                messages.Add("Failed");
                            }

                        }
                        else
                        {
                            Console.WriteLine("UNEXPECTED MDB OPCODE IN VEND REQUEST {0}", message.GetType());
                        }
                        break;
                    case MarshallCardDataMessage.TRANSFER_DATA:

                        int f = 0;

                        break;
                    case MarshallResponeRxMessage.RESPONSE_OPCODE:
                        this.marshall.paringStateMachine.respCounter--;
                        break;
                    case MarshallStatusMessage.CODE_STATUS:
                        Console.WriteLine("RECEIVED STATUS IN VEND REQUEST");
                        break;
                    case MarshallResetMessage.RESET_OPCODE:
                        Console.WriteLine("RECEIVED RESET IN VEND REQUEST");
                        this.marshall.MachineSerialPort.sendMarshallMessage(new MarshallFirmwareInfoMessage());
                        this.setState(this.marshall.paringStateMachine);


                        //this.marshall.MachineSerialPort.sendInternalMessage(MarshallInternalMessage.INIT_PAIRING);
                        //this.setState(endSession);
                        //this.marshall.MachineSerialPort.sendInternalMessage(MarshallInternalMessage.RESTART); maybe this??

                        break;
                    default:
                        Console.WriteLine("UNEXPECTED MARSHALL MESSAGE IN VEND REQUEST {0}", message.GetType());
                        break;
                }
            }
            else if (!MarshallMessageType(message))
            {
                systemMessage = (MarshallInternalMessage)message;

                Console.WriteLine("Message is marshallinternalMessage, MdbOpcode: {0}, SubOpCode: {1}", systemMessage.OpCode, systemMessage.SubOpcode);

                switch (systemMessage.OpCode)
                {
                    case MarshallInternalMessage.PRODUCT_SELECTED: // DONT NEED THIS

                        //if (timerOn)
                        //{
                        //    this.timer.Stop();
                        //    this.timerOn = false;
                        //}

                        //this.mdbVendRequest = new VendRequestMessage();
                        //this.marshall.MachineSerialPort.sendMarshallMessage(this.mdbVendRequest);
                        //this.setState(this.vendSuccess);
                        break;

                    case MarshallInternalMessage.VEND_CANCELLED:
                        if (timerOn)
                        {
                            timer.Stop();
                            timerOn = false;
                        }
                        this.marshall.MachineSerialPort.sendMarshallMessage(mdbVendCancel);
                        aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
                        aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                        {
                            Event = "CreditCardTransactionFailed",
                            DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                            Amount = marshall.amount == 0 ? 0 : marshall.amount / 100
                        });
                        break;
                    default:
                        Console.WriteLine("UNEXPECTED INTERNAL MESSAGE IN DOVENDREQUEST {0}", message.GetType());
                        break;
                }
            }
        }

        public void doVendSuccess(MarshallMessage message)
        {
            MarshallInternalMessage systemMessage;
            MarshallProtocolMessage marshallMessage;

            Console.WriteLine("doVendSuccess");


            if (message is MarshallProtocolMessage)
            {
                marshallMessage = (MarshallProtocolMessage)message;

                if (marshallMessage.ackResponse)
                {
                    sendAckResponseMessage();
                }

                switch (marshallMessage.OpCode)
                {
                    case MdbVendApproveMessage.MDB_OPCODE:
                        if (marshallMessage.MdbOpcode == MdbVendApproveMessage.VEND_APPROVED)
                        {
                            // fire event
                            //this.marshall.nayaxInterface.OnAuthorize(null, Convert.ToDecimal(marshallMessage.dataToBuffer()));

                            if (!timerOn)
                            {
                                timer = new Timer(10000);
                                timer.Elapsed += (object sender, ElapsedEventArgs e) =>
                                {
                                    if (this.getState() == this.vendSuccess)
                                    {
                                        Console.WriteLine("TIMEOUT FOR TAKING PRODUCT EXPIRED");
                                        this.marshall.MachineSerialPort.sendInternalMessage(MarshallInternalMessage.PRODUCT_FAILURE);
                                        aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
                                        this.timer.Stop();
                                        this.timerOn = false;
                                    }
                                };
                                timer.Start();
                                timerOn = true;
                            }
                        }
                        else if (marshallMessage.MdbOpcode == MdbVendDeniedMessage.VEND_DENIED)
                        {
                            aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed");
                            Console.WriteLine("RECEIVED VEND DENIED");
                            this.marshall.MachineSerialPort.sendInternalMessage(MarshallInternalMessage.VEND_INTERNAL);
                            this.setState(completeSession);
                        }
                        else
                        {
                            Console.WriteLine("UNEXPECTED MDB OPCODE IN VEND SUCCESS {0}", message.GetType());
                        }
                        break;
                    case MarshallCardDataMessage.TRANSFER_DATA:
                        Console.WriteLine("CARD DATA RECEIVED");
                        break;
                    case MarshallResponeRxMessage.RESPONSE_OPCODE:
                        this.marshall.paringStateMachine.respCounter--;
                        break;
                    case MarshallResetMessage.RESET_OPCODE:
                        Console.WriteLine("RECEIVED RESET IN VEND SUCCESS");
                        break;
                    default:
                        Console.WriteLine("UNEXPECTED MARSHALL MESSAGE IN VEND SUCCESS {0}", message.GetType());
                        break;
                }
            }
        }

        public void doSessionComplete(MarshallMessage message)
        {
            Console.WriteLine("doSessionComplete");
            MarshallInternalMessage systemMessage;

            if (message is MarshallProtocolMessage)
            {
                var marshallMessage = (MarshallProtocolMessage)message;

                if (marshallMessage.OpCode == MarshallResponeRxMessage.RESPONSE_OPCODE)
                {
                    Console.WriteLine("RECEIVED RESPONSE IN SESSION COMPLETE");
                    this.marshall.paringStateMachine.respCounter--;
                }
                else if (marshallMessage.opcode == MarshallStatusMessage.CODE_STATUS)
                    ;
                else
                {
                    Console.WriteLine("UNEXPECTED MARSHALL MESSAGE IN SESSION COMPLETE {0}", message.GetType());
                }
            }

            if (message is MarshallInternalMessage)
            {
                systemMessage = (MarshallInternalMessage)message;

                if (systemMessage.OpCode == MarshallInternalMessage.VEND_INTERNAL)
                {
                    Console.WriteLine("SEND SESSION COMPLETE");
                    this.marshall.MachineSerialPort.sendMarshallMessage(sessionCompleteMessage);

                    if (!timerOn)
                    {
                        timer = new Timer(1000);
                        timer.Elapsed += (object sender, ElapsedEventArgs e) =>
                        {
                            if (this.getState() != this.endSession)
                            {
                                Console.WriteLine("TIMEOUT FOR END SESSION EXPIRED");
                                this.setState(this.endSession);
                                this.timer.Stop();
                                this.timerOn = false;
                            }
                        };
                        timer.Start();
                        timerOn = true;
                    }
                    this.setState(endSession);
                }
                else
                {
                    Console.WriteLine("UNEXPECTED SYSTEM MESSAGE IN SESSION COMPLETE {0}", message.GetType());
                }
            }
        }

        public void doEndSession(MarshallMessage message)
        {
            if (this.MarshallMessageType(message))
            {
                MarshallProtocolMessage marshallMessage = (MarshallProtocolMessage)message;

                if (marshallMessage.ackResponse)
                    sendAckResponseMessage();

                switch (marshallMessage.OpCode)
                {
                    case MdbEndSessionMessage.MDB_OPCODE:
                        if (marshallMessage.MdbOpcode == MdbEndSessionMessage.END_SESSION)
                        {
                            MarshallMessage.beginSession = false;
                            this.marshall.MachineSerialPort.sendInternalMessage(MarshallInternalMessage.IDLE_MODE);
                            timer.Stop();
                            timerOn = false;
                        }
                        else
                        {
                            Console.WriteLine("UNEXPECTED MDB OPCODE IN END_SESSION {0}", message.GetType());
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public void sendAckResponseMessage()
        {
            marshall.MachineSerialPort.sendMarshallMessage(marshallResponeTxMessage);
        }
    }
}
