using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace deORO.Marshall
{
    public class InitComm : IState
    {

        StartUpStateMachine machine;

        public InitComm(StartUpStateMachine machine_)
        {
            this.machine = machine_;
        }


        public void doAction(MarshallMessage message)
        {
            this.machine.doInitComm(message);
        }
    }
    public class WaitForDisplayMessage : IState
    {
        StartUpStateMachine machine;
        public WaitForDisplayMessage(StartUpStateMachine mac)
        {
            machine = mac;
        }

        public void doAction(MarshallMessage message)
        {
            this.machine.doWaitForDisplayMessage(message);
        }
    }
    public class WaitForReset : IState
    {

        StartUpStateMachine machine;

        public WaitForReset(StartUpStateMachine machine_)
        {
            this.machine = machine_;
        }


        public void doAction(MarshallMessage message)
        {
            this.machine.doWaitForReset(message);
        }
    }

    public class WaitForConfig : IState
    {

        StartUpStateMachine machine;

        public WaitForConfig(StartUpStateMachine machine_)
        {
            this.machine = machine_;
        }


        public void doAction(MarshallMessage message)
        {
            this.machine.doWaitForConfig(message);
        }
    }

    public class KeepAliveMode : IState
    {

        StartUpStateMachine machine;

        public KeepAliveMode(StartUpStateMachine machine_)
        {
            this.machine = machine_;
        }


        public void doAction(MarshallMessage message)
        {
            this.machine.doKeepAlive(message);
        }
    }

    public class Idle : IState
    {

        StartUpStateMachine machine;

        public Idle(StartUpStateMachine machine_)
        {
            this.machine = machine_;
        }


        public void doAction(MarshallMessage message)
        {
            this.machine.doIdle(message);
        }
    }

    public class StartUpStateMachine : MachineContext
    {
        public InitComm initComm;
        public WaitForReset waitForReset;
        public WaitForDisplayMessage waitForDisplayMessage;
        public WaitForConfig waitForConfig;
        public KeepAliveMode keepAliveMode;
        public Idle idle;
        public MarshallMain marshall;

        public MarshallDisplayMessage displayMessage;
        public MarshallFirmwareInfoMessage firmwareInfoMessage;
        public MarshallKeepAliveMessage keepAliveMessage;
        public ReaderEnableMessage readerEnableMessage;
        public ReaderDisableMessage readerDisableMessage;
        public Timer kaTimer;
        public bool isTimerOn;
        public int respCounter = 0;

        public StartUpStateMachine(MarshallMain marshall)
        {
            this.marshall = marshall;
            this.initComm = new InitComm(this);
            this.waitForReset = new WaitForReset(this);
            this.waitForConfig = new WaitForConfig(this);
            this.keepAliveMode = new KeepAliveMode(this);
            this.waitForDisplayMessage = new WaitForDisplayMessage(this);
            this.idle = new Idle(this);

            firmwareInfoMessage = new MarshallFirmwareInfoMessage();
            keepAliveMessage = new MarshallKeepAliveMessage();
            readerEnableMessage = new ReaderEnableMessage();
            readerDisableMessage = new ReaderDisableMessage();

            this.setState(waitForReset);
            isTimerOn = false;
        }

        public void doCleanUp()
        {
            if (kaTimer != null)
            {
                kaTimer.Stop();
            }
            marshall = null;
            initComm = null;
            waitForConfig = null;
            waitForReset = null;
            keepAliveMode = null;
            waitForDisplayMessage = null;
            idle = null;
        }

        public void doInitComm(MarshallMessage message)
        {
            if (isTimerOn)
            {
                kaTimer.Close();
                isTimerOn = false;
            }

            
            marshall.InitComm();
            this.setState(waitForReset);
        }

        public void doWaitForDisplayMessage(MarshallMessage message)
        {
            if (message is MarshallProtocolMessage)
            {
                MarshallProtocolMessage marshallMessage = (MarshallProtocolMessage)message;

                switch (marshallMessage.OpCode)
                {
                    case MarshallDisplayMessage.DISPLAY_MESSAGE_RESPONSE_CODE:
                        Console.WriteLine("DISPLAY MESSAGE RESPONSE CODE!!!!!!");
                        break;
                    case MarshallDisplayMessage.DISPLAY_MESSAGE_OPCODE:
                        Console.WriteLine("DISPLAY MESSAGE CODE!!!!!!");
                        break;
                    default:
                        break;
                }

                this.setState(keepAliveMode);
            }
        }
        public void doWaitForReset(MarshallMessage message)
        {
            if (message is MarshallProtocolMessage)
            {
                MarshallProtocolMessage marshallMessage = (MarshallProtocolMessage)message;

                switch (marshallMessage.OpCode)
                {
                    case MarshallResetMessage.RESET_OPCODE:
                        //Console.WriteLine("RESET OPCODE IN doWaitForReset");
                        this.marshall.MachineSerialPort.sendMarshallMessage(firmwareInfoMessage);
                        this.setState(waitForConfig);
                        break;
                    default:
                        Console.WriteLine("UNEXPECTED MESSAGE IN WAITFORRESEST!! {0}", message.GetType());
                        break;
                }
            }
            else
                Console.WriteLine("UNEXPECTED INTERNAL MESSAGE IN WAITFORRESET {0}", message.GetType());
        }
        public void doWaitForConfig(MarshallMessage message)
        {
            if (message is MarshallProtocolMessage)
            {
                MarshallProtocolMessage marshallMessage = (MarshallProtocolMessage)message;
                
                switch (marshallMessage.OpCode)
                {
                    case MarshallResetMessage.CONFIG_OPCODE:
                        //Console.WriteLine("CONFIG OPCODE IN doWaitForConfig\n");
                        this.setState(keepAliveMode);
                        this.marshall.MachineSerialPort.sendInternalMessage(MarshallInternalMessage.KEEP_ALIVE_MODE);
                        break;
                    case MarshallResetMessage.RESET_OPCODE:
                        this.marshall.MachineSerialPort.sendMarshallMessage(firmwareInfoMessage);
                        this.setState(waitForConfig);
                        break;
                    default:
                        Console.WriteLine("UNEXPECTED MESSAGE IN WAITFORCONFIG!! {0}", message.GetType());
                        break;
                }
            }
            else
                Console.WriteLine("UNEXPECTED INTERNAL MESSAGE IN WAITFORCONFIG {0}", message.GetType());
        }
        public void doKeepAlive(MarshallMessage message)
        {
            if (message is MarshallProtocolMessage)
            {
                Console.WriteLine("UNEXPECTED MARSHALL MESSAGE IN KEEPALIVE {0}", message.GetType());
            }
            if (message is MarshallInternalMessage)
            {
                MarshallInternalMessage marshallMessage = (MarshallInternalMessage)message;

                switch (marshallMessage.OpCode)
                {
                    case MarshallInternalMessage.KEEP_ALIVE_MODE:
                        //this.marshall.MachineSerialPort.sendMarshallMessage(readerDisableMessage);
                        this.marshall.MachineSerialPort.sendMarshallMessage(readerEnableMessage);

                        if (!isTimerOn)
                        {
                            kaTimer = new Timer(1000);
                            kaTimer.Elapsed += (sender, e) =>
                            {
                                if (marshall.MachineSerialPort.IsOpen())
                                    marshall.MachineSerialPort.sendMarshallMessage(keepAliveMessage);

                                //respCounter++;
                                //if (this.respCounter > 1)
                                //{
                                //    if (marshall.MachineSerialPort.IsOpen())
                                //        marshall.MachineSerialPort.sendInternalMessage(MarshallInternalMessage.RESTART);
                                //    this.respCounter = 0;
                                //    marshall.MachineSerialPort.Close();
                                //}
                            };
                            kaTimer.Start();
                            isTimerOn = true;
                        }
                        this.setState(idle);
                        break;
                    default:
                        Console.WriteLine("UNEXPECTED MESSAGE IN WAITFORRESEST!! {0}", message.GetType());
                        break;
                }
            }
            else
                Console.WriteLine("UNEXPECTED INTERNAL MESSAGE IN WAITFORRESET {0}", message.GetType());
        }
        
  
        public void doIdle(MarshallMessage message)
        {
            if (message is MarshallProtocolMessage)
            {
                MarshallProtocolMessage marshallMessage = (MarshallProtocolMessage)message;

                switch (marshallMessage.OpCode)
                {
                    case MarshallResponeRxMessage.RESPONSE_OPCODE:
                        Console.WriteLine("response in doIdle");
                        this.respCounter--;
                        break;
                    case MarshallResetMessage.RESET_OPCODE:
                        Console.WriteLine("Reset in doIdle");
                        this.setState(initComm);
                        break;
                    case MdbBeginSessionMessage.MDB_OPCODE:
                        Console.WriteLine("mdbOpcode in doIdle");
                        if (marshallMessage.ackResponse)
                        {
                            this.marshall.vendStateMachine.sendAckResponseMessage();
                            marshallMessage.ackResponse = false;
                        }
                        this.marshall.MachineSerialPort.sendInternalMessage(MarshallInternalMessage.VEND_MODE);
                        this.marshall.MachineSerialPort.sendMdbInternalMessage(marshallMessage);
                        break;
                    default:
                        Console.WriteLine("UNEXPECTED MESSAGE IN WAITFORRESEST!! {0}", message.GetType());
                        break;
                }
            }
            else
                Console.WriteLine("UNEXPECTED INTERNAL MESSAGE IN WAITFORRESET {0}", message.GetType());
        }
    }
    
}
