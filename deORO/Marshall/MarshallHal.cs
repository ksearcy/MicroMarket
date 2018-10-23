using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Collections.Specialized;
using System.Collections.Concurrent;

namespace deORO.Marshall
{
    public class MarshallHal : IDisposable
    {
        private SerialPort _serialPort;
        private object serialPortLock = new object();

        public SerialPort SerialPort
        {
            get
            {
                return _serialPort;
            }
            private set
            {
                this._serialPort = value;
            }
        }

        object marshalllock = new object();
        public object MarshallLock { get { return this.marshalllock; } }
        public BlockingCollection<MarshallMessage> MarshallQueue
        {
            get
            {
                return this.marshallQueue;
            }
        }
        public static string[] PortNames
        {
            get
            {
                return SerialPort.GetPortNames();
            }
        }

        public static bool PortsAvailable()
        {
            return PortNames != null && PortNames.Length > 0;
        }
        public bool IsOpen()
        {
            try
            {
                return this.SerialPort.IsOpen;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Open(string portName)
        {
            this._serialPort = new SerialPort(portName);

            try
            {
                this.SerialPort.BaudRate = 115200;
                this.SerialPort.DataBits = 8;
                this.SerialPort.StopBits = StopBits.One;
                this.SerialPort.Parity = Parity.None;
                this.SerialPort.DtrEnable = true;
                this.SerialPort.RtsEnable = true;
                this.SerialPort.Open();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public void Close()
        {
            try
            {
                //System.Windows.Forms.MessageBox.Show("close");
                if (this.SerialPort != null)
                    this.SerialPort.Close();
                this.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void InitThreads()
        {
            this.Rx = new SerialRead(this);
            rxThread = new Thread(this.Rx.Run);
            rxThread.Start();

            this.Tx = new SerialWrite(this);
            txThread = new Thread(this.Tx.Run);
            txThread.Start();
        }

        private Thread rxThread;
        private Thread txThread;

        public SerialWrite Tx;
        public SerialRead Rx;

        private BlockingCollection<MarshallMessage> marshallQueue;

        public MarshallHal(BlockingCollection<MarshallMessage> marshallQueue)
        {
            this.marshallQueue = marshallQueue;
        }
        public void sendInternalMessage(byte type)
        {
            MarshallInternalMessage systemMessage = new MarshallInternalMessage(type);

            try
            {
                this.marshallQueue.Add(systemMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        public void sendMdbInternalMessage(MarshallProtocolMessage message)
        {
            try
            {
                this.marshallQueue.Add(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void sendMarshallMessage(MarshallMessage message)
        {
            try
            {
                Tx.marshallTx.Add((MarshallProtocolMessage)message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.SerialPort != null)
                {
                    this.SerialPort.Dispose();
                    this.SerialPort = null;
                }

                try
                {
                    if (txThread != null)
                        this.txThread.Abort();
                }
                catch { }

                try
                {
                    if (rxThread != null)
                        this.rxThread.Abort();
                }
                catch { }
            }
        }

        public void Dispose()
        {
            //System.Windows.Forms.MessageBox.Show("Dispose");
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
    public class SerialRead
    {
        public SerialRead(MarshallHal ma)
        {
            this.ma = ma;
        }

        MarshallProtocolMessage message;
        public MarshallHal ma;
        byte[] rxBuffer = null;
        private short getEncodedLength(int offset)
        {

            short length = 0;
            byte[] encodedLength = new byte[2];

            encodedLength[0] = rxBuffer[offset];
            encodedLength[1] = rxBuffer[offset + 1];
            length = Utils.byteArrToShort(encodedLength, 0);
            return length;
        }

        public void Run(object state)
        {
            while (true)
            {
                if (ma.SerialPort == null)
                    return;
                try
                {
                    if (this.ma.IsOpen())
                    {
                        Thread.Sleep(150);
                        int bytesToread = 0, bytesRead = 0;

                        lock (this.ma.MarshallLock)
                        {
                            Console.WriteLine("Locking on recv");
                            bytesToread = this.ma.SerialPort.BytesToRead;
                            this.rxBuffer = new byte[bytesToread];
                            bytesRead = this.ma.SerialPort.Read(this.rxBuffer, 0, bytesToread);
                        }

                        if ((this.rxBuffer != null) && (bytesRead >= 11))
                        {
                            int totalLength = rxBuffer.Length;
                            int offset = 0;
                            short encLength = 0;

                            while (offset < totalLength)
                            {
                                encLength = getEncodedLength(offset);
                                if (encLength == 0)
                                    break;
                                if (totalLength - offset < 2)
                                    break;
                                if (totalLength < encLength + 2)
                                {
                                    Console.WriteLine("Encoded length: {0}", encLength);
                                    Console.WriteLine("Total length: {0}", totalLength);
                                    break;
                                }

                                byte[] rawMessage = new byte[encLength + 2];
                                Array.Copy(rxBuffer, offset, rawMessage, 0, encLength + 2);
                                offset += (encLength + 2);

                                message = MarshallProtocolMessage.factory(rawMessage);
                                if (message != null)
                                {
                                    try
                                    {
                                        string type = message.GetType().ToString().Replace("NayaxConsoleTester.", string.Empty);
                                        string buff = BitConverter.ToString(rawMessage, 0);

                                        string fmt = string.Format("Receiving {0} {1}", type, buff);

                                        System.Diagnostics.Debug.WriteLine(fmt);
                                        Console.WriteLine(fmt);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.ToString());
                                    }


                                    ma.MarshallQueue.Add(message);
                                }
                                else
                                    Console.WriteLine("MarshallProtocolMessage null");
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    break;
                }
            }
        }

    }
    public class SerialWrite
    {
        public MarshallHal ma;
        public BlockingCollection<MarshallProtocolMessage> marshallTx;

        public SerialWrite(MarshallHal ma)
        {
            this.ma = ma;
            this.marshallTx = new BlockingCollection<MarshallProtocolMessage>(2);
        }

        public void Run()
        {
            while (true)
            {
                if (ma.SerialPort == null)
                    return;
                try
                {
                    MarshallProtocolMessage message = this.marshallTx.Take();

                    byte[] txBuffer = message.buildTxBuffer();

                    //string fmt = string.Format("Writing message {0}, opcode: {1}", message.GetType().ToString().Replace("NayaxConsoleTester.", string.Empty), message.OpCode);

                    string type = message.GetType().ToString().Replace("NayaxConsoleTester.", string.Empty);
                    string buff = BitConverter.ToString(txBuffer, 0);

                    string fmt = string.Format("Sending {0} {1}", type, buff);
                    Console.WriteLine(fmt);

                    lock (this.ma.MarshallLock)
                    {
                        Console.WriteLine("Locking on send");

                        if (ma.SerialPort != null && ma.SerialPort.IsOpen)
                            ma.SerialPort.Write(txBuffer, 0, txBuffer.Length);
                        else
                            break;
                    }

                    Console.Write("TX: ");
                    for (int i = 0; i < txBuffer.Length; i++)
                    {
                        Console.Write("{0:X2} ", txBuffer[i]);
                    }
                    Console.WriteLine("\n");


                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
