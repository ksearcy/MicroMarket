using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Practices.Composite.Events;

namespace deORO.Marshall
{
    public class MarshallMain
    {
        public BlockingCollection<MarshallMessage> marshallQueue;
        public MarshallHal machineSerialPort;
        public StartUpStateMachine paringStateMachine;
        public VendStateMachine vendStateMachine;
        public MachineContext machineContext;
        public int amount;

        public MarshallHal MachineSerialPort
        {
            get
            {
                return this.machineSerialPort;
            }
        }

        //public MarshallMain(NayaxInterface nayaxInterface)
        //{
        //    this.nayaxInterface = nayaxInterface;
        //    this.marshallQueue = new BlockingCollection<MarshallMessage>(4);
        //    this.machineSerialPort = new MarshallHal(this.marshallQueue);
        //    this.paringStateMachine = new StartUpStateMachine(this);
        //    this.vendStateMachine = new VendStateMachine(this);
        //    this.machineContext = this.paringStateMachine;
        //}

        public MarshallMain()
        {
            this.marshallQueue = new BlockingCollection<MarshallMessage>(4);
            this.machineSerialPort = new MarshallHal(this.marshallQueue);
            this.paringStateMachine = new StartUpStateMachine(this);
            this.vendStateMachine = new VendStateMachine(this);
            this.machineContext = this.paringStateMachine;
        }

        public bool InitComm()
        {
            int i = 0;

            while (!MachineSerialPort.Open(Helpers.Global.MarshallCOMPort))
            {
                if (i <= 2)
                {
                    try
                    {
                        MachineSerialPort.Close();
                        Helpers.DisableHardware.DisableDevice(x => x.Contains(Helpers.Global.HardwareId), true);
                        System.Threading.Thread.Sleep(500);
                        Helpers.DisableHardware.DisableDevice(x => x.Contains(Helpers.Global.HardwareId), false);
                        System.Threading.Thread.Sleep(500);
                    }
                    catch { }
                    i++;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        //public void CloseComm()
        //{
        //    this.MachineSerialPort.Close();
        //}

        public void Close()
        {
            if (MachineSerialPort != null)
                this.MachineSerialPort.Close();

            if (paringStateMachine != null)
                paringStateMachine.doCleanUp();

            if (vendStateMachine != null)
                vendStateMachine.doCleanUp();

            marshallQueue = null;
            vendStateMachine = null;
            machineContext = null;
        }

        public void InitMachine()
        {
            //this.InitComm();
            this.machineSerialPort.InitThreads();

            MarshallMessage message = new MarshallInternalMessage(MarshallInternalMessage.INIT_PAIRING);
            try
            {
                this.marshallQueue.Add(message);
            }
            catch (Exception e)
            {

            }
        }

        public void EnableReader()
        {
            machineSerialPort.sendMarshallMessage(new ReaderEnableMessage());
        }

        public void DisableReader()
        {
            machineSerialPort.sendMarshallMessage(new ReaderDisableMessage());
            amount = 0;
        }

        public void Authorize(int amount)
        {
            this.amount = amount;
            MarshallMessage.beginSession = true;
            byte[] bytes = BitConverter.GetBytes(amount);
            VendRequestMessage mdbVendRequest = new VendRequestMessage(bytes);
            machineSerialPort.sendMarshallMessage(mdbVendRequest);
        }

        public void Run()
        {
            this.InitMachine();

            MarshallMessage message;

            while (true)
            {
                if (!this.MachineSerialPort.IsOpen())
                    break;

                try
                {
                    if (this.marshallQueue.TryTake(out message, 1800))
                    {
                        if (message is MarshallInternalMessage)
                        {
                            MarshallInternalMessage intMessage = (MarshallInternalMessage)message;

                            switch (intMessage.OpCode)
                            {
                                case MarshallInternalMessage.INIT_PAIRING:
                                    this.machineContext = paringStateMachine;
                                    break;
                                case MarshallInternalMessage.RESTART:
                                    this.machineContext = paringStateMachine;
                                    this.machineContext.setState(paringStateMachine.initComm);
                                    this.machineContext.getState().doAction(message);
                                    break;
                                case MarshallInternalMessage.KEEP_ALIVE_MODE:
                                    this.machineContext = paringStateMachine;
                                    this.machineContext.setState(paringStateMachine.keepAliveMode);
                                    this.machineContext.getState().doAction(message);
                                    break;
                                case MarshallInternalMessage.IDLE_MODE:
                                    this.machineContext = paringStateMachine;
                                    this.machineContext.setState(paringStateMachine.idle);
                                    break;
                                case MarshallInternalMessage.VEND_MODE:
                                    this.machineContext = paringStateMachine;
                                    this.machineContext.setState(vendStateMachine.vendRequest);
                                    break;
                                case MarshallInternalMessage.VEND_INTERNAL:
                                    this.machineContext.getState().doAction(message);
                                    break;
                                case MarshallInternalMessage.PRODUCT_SELECTED:
                                    if (this.machineContext.getState() == vendStateMachine.vendRequest)
                                        this.machineContext.doAction(message);
                                    break;
                                case MarshallInternalMessage.VEND_CANCELLED:
                                    if ((this.machineContext.getState() == vendStateMachine.vendRequest) || (this.machineContext.getState() == vendStateMachine.vendSuccess))
                                        this.machineContext.doAction(message);
                                    break;
                                case MarshallInternalMessage.PRODUCT_TAKEN:
                                    if (this.machineContext.getState() == vendStateMachine.vendSuccess)
                                        this.machineContext.doAction(message);
                                    break;
                                case MarshallInternalMessage.PRODUCT_FAILURE:
                                    if (this.machineContext.getState() == vendStateMachine.vendSuccess)
                                        this.machineContext.doAction(message);
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            this.machineContext.getState().doAction(message);


                        }
                    }
                    else
                    {

                        //if (message == null)
                        //    return;

                        //Console.WriteLine("Communication lost with AMIT, closing and init machine again");

                        //this.MachineSerialPort.sendMarshallMessage(new MarshallFirmwareInfoMessage());
                        //this.machineContext.setState(paringStateMachine.waitForConfig);

                        if (this.machineSerialPort.IsOpen())
                        {
                            this.MachineSerialPort.sendMarshallMessage(new MarshallFirmwareInfoMessage());
                            this.machineContext.setState(paringStateMachine.waitForConfig);

                            this.machineContext = paringStateMachine;
                            this.machineContext.setState(paringStateMachine.initComm);
                        }
                        else
                        {
                            break;
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
