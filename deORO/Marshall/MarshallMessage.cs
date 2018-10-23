using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.Marshall
{

    public abstract class MarshallMessage
    {
        public static bool beginSession = false;
    }

    public class MarshallInternalMessage : MarshallMessage
    {

        public const byte INIT_PAIRING = 0x01;
        public const byte KEEP_ALIVE_MODE = 0x02;
        public const byte RESTART = 0x03;
        public const byte VEND_MODE = 0x04;
        public const byte VEND_INTERNAL = 0x08;
        public const byte IDLE_MODE = 0x05;
        public const byte PRODUCT_SELECTED = 0x0A;
        public const byte PRODUCT_TAKEN = 0x0B;
        public const byte PRODUCT_FAILURE = 0x0C;
        public const byte VEND_CANCELLED = 0X0D;


        internal byte opcode;
        internal byte subOpcode;

        public MarshallInternalMessage(byte opcode_)
        {
            this.opcode = opcode_;
        }
        public MarshallInternalMessage(byte opcode_, byte subOpcode_)
        {
            this.opcode = opcode_;
            this.subOpcode = subOpcode_;
        }
        public virtual byte OpCode
        {
            get
            {
                return opcode;
            }
        }

        public virtual byte SubOpcode
        {
            get
            {
                return subOpcode;
            }
        }
    }

    public class MarshallProtocolMessage : MarshallMessage
    {

        protected internal const int HEADER_SIZE = 11;
        protected internal const int CRC16_SIZE = 2;
        protected internal const int OPTIONS = 2;
        protected internal const int ID = 3;
        protected internal const int SOURCE = 4;
        protected internal const int SOURCE_LSB = 5;
        protected internal const int DEST = 6;
        protected internal const int DEST_LSB = 7;
        protected internal const int CODE = 8;

        public const byte RESET_OPCODE = 0x01;
        public const byte CONFIG_OPCODE = 0x06;

        public static readonly byte TRANSFER_DATA = (byte)0x0A;
        public const byte RESPONSE_OPCODE = 0x00;

        public const byte ACK = 0x01;
        public const byte ACK_REQUIRED = 0x01;

        public const byte MDB_OPCODE = unchecked((byte)0x80);
        public const byte VEND_CMD = (byte)0x13;

        //public readonly byte[] PRODUCT_PRICE = new byte[] { 0x00, 0x05, 0x00, 0x06, 0x00, 0x07, 0x00, 0x08 };
        //public readonly byte[] PRODUCT_CODE = new byte[] { 0x01, 0x01, 0x01, 0x02, 0x02, 0x01, 0x02, 0x02 };



        internal static byte source = 0;
        internal static byte sourceLSB = 0;
        internal static byte dest = 0;
        internal static byte destLSB = 0;

        internal byte opcode;
        internal byte mdbOpcode;
        internal static byte packetId = unchecked((byte)0xBB);
        internal byte[] mdbData;
        public static byte counter = 0;

        internal bool ackResponse = false;

        public MarshallProtocolMessage(byte opcode_)
        {
            opcode = opcode_;

        }

        public MarshallProtocolMessage(byte opcode_, byte mdbOpcode_)
        {
            opcode = opcode_;
            mdbOpcode = mdbOpcode_;

        }
        public MarshallProtocolMessage(byte opcode_, byte[] data)
        {
            opcode = opcode_;
            mdbData = data;
        }

        public static MarshallProtocolMessage factory(byte[] buffer)
        {

            MarshallProtocolMessage message = null;


            if (!correctCrc(buffer))
            {
                return null;
            }

            Console.Write("RX: ");
            for (int j = 0; j < buffer.Length; j++)
            {
                Console.Write("{0:X2} ", buffer[j]);
            }
            Console.WriteLine("\n");


            message = MarshallConfigMessage.factory(buffer); //RX
            if (message != null)
            {
                return message;
            }

            message = MarshallResetMessage.factory(buffer); //RX
            if (message != null)
            {
                return message;
            }

            message = MarshallResponeRxMessage.factory(buffer); //RX
            if (message != null)
            {
                return message;
            }

            message = MdbBeginSessionMessage.factory(buffer); //RX
            if (message != null)
            {
                return message;
            }

            message = MdbCancelSessionMessage.factory(buffer); //RX
            if (message != null)
            {
                return message;
            }

            message = MdbVendApproveMessage.factory(buffer); //RX
            if (message != null)
            {
                return message;
            }

            message = MdbEndSessionMessage.factory(buffer); //RX
            if (message != null)
            {
                return message;
            }

            message = MarshallCardDataMessage.factory(buffer); //RX
            if (message != null)
            {
                return message;
            }


            message = MdbVendDeniedMessage.factory(buffer); //RX
            if (message != null)
            {
                return message;
            }

            message = MarshallStatusMessage.factory(buffer); //RX
            if (message != null)
            {
                return message;
            }

            return message;
        }

        public virtual byte PacketId
        {
            set
            {
                packetId = value;
            }
            get
            {
                return packetId;
            }
        }


        public virtual byte DataLength
        {
            get
            {
                return 0;
            }
        }

        public virtual byte[] buildTxBuffer() //(byte source, byte sourceLSB, byte dest, byte destLSB)
        {
            int dataLength = 0;
            byte[] messageData = dataToBuffer();
            if (messageData == null)
            {
                dataLength = 0;
            }
            else
            {
                dataLength = messageData.Length;
            }

            short length = (short)(dataLength + HEADER_SIZE);

            byte[] decLength = new byte[2];
            Utils.shortToByteArray(decLength, 0, (short)(length - 2));

            byte[] message = new byte[length];

            message[0] = decLength[0];
            message[1] = decLength[1];

            message[CODE] = opcode;
            if (message[CODE] == RESPONSE_OPCODE)
            {
                message[ID] = packetId;
                message[OPTIONS] = 0x00;
            }
            else
            {
                message[OPTIONS] = ackRequired();
                message[ID] = counter;
                if (counter != unchecked((byte)0xFF))
                {
                    counter++; //update counter
                }
                else
                {
                    counter = 0x00; //Max counter value
                }
            }

            message[SOURCE] = source;
            message[SOURCE_LSB] = sourceLSB;
            message[DEST] = dest;
            message[DEST_LSB] = destLSB;

            //Append data 
            if (dataLength > 0)
            {
                Array.Copy(messageData, 0, message, 9, dataLength);
            }
            //Append CRC
            byte[] rcvCRC = new byte[2];
            short crc;
            crc = Utils.calcCRC(message, message.Length - 2, Utils.SEED_CCITT);
            Utils.shortToByteArray(rcvCRC, 0, crc);
            Array.Copy(rcvCRC, 0, message, message.Length - 2, 2);

            return message;
        }

        public virtual byte[] dataToBuffer()
        {

            return null;
        }

        public static bool correctCrc(byte[] message)
        {
            short crc, rcvCRC;
            int messageLength = message.Length;

            crc = Utils.calcCRC(message, message.Length - 2, Utils.SEED_CCITT);
            byte[] decCRC = new byte[] { message[messageLength - 2], message[messageLength - 1] };
            rcvCRC = Utils.byteArrToShort(decCRC, 0);
            if (rcvCRC != crc)
            {
                Console.WriteLine("Rx CRC Error\n");
                return false;
            }
            return true;
        }

        public virtual byte ackRequired()
        {
            return ACK;
        }

        public virtual byte OpCode
        {
            get
            {
                return opcode;
            }
        }
        public virtual byte MdbOpcode
        {
            get
            {
                return mdbOpcode;
            }
        }

    }

    //===========================================================
    // 				RECEIVED MESSAGES FROM AMIT
    // ===========================================================
    public class MarshallResetMessage : MarshallProtocolMessage
    {

        public new const byte RESET_OPCODE = 0x01;

        public MarshallResetMessage()
            : base(RESET_OPCODE)
        {
        }

        public static MarshallProtocolMessage factory(byte[] buffer)
        {

            MarshallProtocolMessage message = null;

            if (buffer[8] == RESET_OPCODE)
            {
                message = new MarshallResetMessage();
            }

            return message;
        }
    }


    public class MarshallResponeRxMessage : MarshallProtocolMessage
    {

        public new const byte RESPONSE_OPCODE = 0x00;
        public const byte ACK_OK = 0x00;

        public MarshallResponeRxMessage()
            : base(RESPONSE_OPCODE)
        {
        }

        public static MarshallProtocolMessage factory(byte[] buffer)
        {
            MarshallProtocolMessage message = null;

            if (buffer[8] == RESPONSE_OPCODE)
            {
                message = new MarshallResponeRxMessage();
            }

            return message;
        }

    }

    public class MdbBeginSessionMessage : MarshallProtocolMessage
    {

        public const byte MDB_OPCODE = 0x80;
        public static readonly byte BEGIN_SESSION = 0x03;

        public MdbBeginSessionMessage(byte[] buffer)
            : base(MDB_OPCODE, BEGIN_SESSION)
        {
            isAckResponseRequired(buffer);
        }


        public static MarshallProtocolMessage factory(byte[] buffer)
        {

            MarshallProtocolMessage message = null;

            if ((buffer[8] == MDB_OPCODE) && (buffer[9] == BEGIN_SESSION))
            {
                message = new MdbBeginSessionMessage(buffer);
            }

            return message;
        }

        public override byte MdbOpcode
        {
            get
            {
                return BEGIN_SESSION;
            }
        }

        public virtual void isAckResponseRequired(byte[] buffer)
        {
            if ((buffer[OPTIONS] & 0x01) == ACK_REQUIRED)
            {
                packetId = buffer[ID];
                ackResponse = true;
            }
        }

        public override byte[] dataToBuffer()
        {
            return null;
        }

        public override byte ackRequired()
        {
            return ACK;
        }

    }

    public class MdbVendApproveMessage : MarshallProtocolMessage
    {

        public const byte VEND_APPROVED = (byte)0x05;

        public MdbVendApproveMessage(byte[] buffer)
            : base(MDB_OPCODE, VEND_APPROVED)
        {
            isAckResponseRequired(buffer);
        }

        public static MarshallProtocolMessage factory(byte[] buffer)
        {

            MarshallProtocolMessage message = null;

            if ((buffer[8] == MDB_OPCODE) && (buffer[9] == VEND_APPROVED))
            {
                message = new MdbVendApproveMessage(buffer);
            }

            return message;
        }
        public override byte MdbOpcode
        {
            get
            {
                return VEND_APPROVED;
            }
        }

        public virtual void isAckResponseRequired(byte[] buffer)
        {
            if ((buffer[OPTIONS] & 0x01) == ACK_REQUIRED)
            {
                packetId = buffer[ID];
                ackResponse = true;
            }
        }

        public override byte[] dataToBuffer()
        {
            return null;
        }

        public override byte ackRequired()
        {
            return ACK;
        }
    }

    public class MdbVendDeniedMessage : MarshallProtocolMessage
    {

        public static readonly byte VEND_DENIED = (byte)0x06;

        public MdbVendDeniedMessage(byte[] buffer)
            : base(MDB_OPCODE, VEND_DENIED)
        {
            isAckResponseRequired(buffer);
        }

        public static MarshallProtocolMessage factory(byte[] buffer)
        {

            MarshallProtocolMessage message = null;

            if ((buffer[8] == MDB_OPCODE) && (buffer[9] == VEND_DENIED))
            {
                message = new MdbVendDeniedMessage(buffer);
            }

            return message;
        }
        public override byte MdbOpcode
        {
            get
            {
                return VEND_DENIED;
            }
        }

        public virtual void isAckResponseRequired(byte[] buffer)
        {
            if ((buffer[OPTIONS] & 0x01) == ACK_REQUIRED)
            {
                packetId = buffer[ID];
                ackResponse = true;
            }
        }

        public override byte[] dataToBuffer()
        {
            return null;
        }

        public override byte ackRequired()
        {
            return ACK;
        }

    }

    public class MdbCancelSessionMessage : MarshallProtocolMessage
    {

        public static readonly byte CANCEL_SESSION = (byte)0x04;

        public MdbCancelSessionMessage(byte[] buffer)
            : base(MDB_OPCODE, CANCEL_SESSION)
        {
            isAckResponseRequired(buffer);
        }

        public static MarshallProtocolMessage factory(byte[] buffer)
        {

            MarshallProtocolMessage message = null;

            if ((buffer[8] == MDB_OPCODE) && (buffer[9] == CANCEL_SESSION))
            {
                message = new MdbCancelSessionMessage(buffer);
            }

            return message;
        }

        public virtual void isAckResponseRequired(byte[] buffer)
        {
            if ((buffer[OPTIONS] & 0x01) == ACK_REQUIRED)
            {
                packetId = buffer[ID];
                ackResponse = true;
            }
        }

        public override byte[] dataToBuffer()
        {
            return null;
        }

        public override byte ackRequired()
        {
            return ACK;
        }
    }


    public class MdbEndSessionMessage : MarshallProtocolMessage
    {

        public static readonly byte END_SESSION = (byte)0x07;

        public MdbEndSessionMessage(byte[] buffer)
            : base(MDB_OPCODE, END_SESSION)
        {
            isAckResponseRequired(buffer);
        }

        public static MarshallProtocolMessage factory(byte[] buffer)
        {

            MarshallProtocolMessage message = null;

            if ((buffer[8] == MDB_OPCODE) && (buffer[9] == END_SESSION))
            {
                message = new MdbEndSessionMessage(buffer);
            }

            return message;
        }

        public virtual void isAckResponseRequired(byte[] buffer)
        {
            if ((buffer[OPTIONS] & 0x01) == ACK_REQUIRED)
            {
                packetId = buffer[ID];
                ackResponse = true;
            }
        }

        public override byte[] dataToBuffer()
        {
            return null;
        }

        public override byte ackRequired()
        {
            return ACK;
        }
    }


    public class MarshallConfigMessage : MarshallProtocolMessage
    {

        public new const byte CONFIG_OPCODE = 0x06;

        public MarshallConfigMessage()
            : base(CONFIG_OPCODE)
        {
        }

        public static MarshallProtocolMessage factory(byte[] buffer)
        {

            MarshallProtocolMessage message = null;

            if (buffer[8] == CONFIG_OPCODE)
            {
                message = new MarshallConfigMessage();

                source = (byte)buffer[DEST];
                sourceLSB = (byte)buffer[DEST_LSB];
                dest = (byte)buffer[SOURCE];
                destLSB = (byte)buffer[SOURCE_LSB];
                int msgLength = buffer.Length - CRC16_SIZE;
                byte[] lengthField = buffer.CopyOfRange(0, 1).ToArray();//Arrays.copyOfRange(buffer, 0, 1);
                short length = Utils.byteArrToShort(lengthField, 0);

                updateConfigData(buffer);
            }
            return message;

        }
        public override byte DataLength
        {
            get
            {
                return 1;
            }
        }

        public override byte[] dataToBuffer()
        {
            byte[] buffer = null;
            return buffer;
        }

        public enum Language
        {
            Unknown = -1,
            English = 0
        }

        public static int PollInterval;
        public static int CountryCode;

        public static void updateConfigData(byte[] buffer)
        {

            byte machineID;
            byte[] currency = new byte[3];
            byte[] country = new byte[3];
            byte[] amitSerialNum = new byte[16];
            byte[] pollInterval = new byte[4];
            byte[] protocolVer = new byte[2];
            byte language;
            byte[] maxPacketLen = new byte[2];

            int index = 9;

            // Protocol version
            protocolVer = buffer.CopyOfRange(index, index + 2).ToArray(); //Arrays.copyOfRange(buffer, index, index + 2);
            string ProtocolVersion = string.Format("{0}.{1}", protocolVer[0], protocolVer[1]);

            // Device id/Machine id
            index += 2;
            machineID = buffer[index];
            //string MachineId = machineID.ToString();

            // Poll interval
            index += 1;
            pollInterval = buffer.CopyOfRange(index, index + 4).ToArray();// Arrays.copyOfRange(buffer, index, index + 4);
            //PollInterval = BitConverter.ToInt32(pollInterval, 0);

            // Language
            index += 4;
            language = buffer[index];
            //Language lang = Language.Unknown;
            //lang = (Language)language;

            // Country
            index += 1;
            country = buffer.CopyOfRange(index, index + 3).ToArray();// Arrays.copyOfRange(buffer, index, index + 3);
            //CountryCode = BitConverter.ToInt32(country.Concat(new byte[] { 0 }).ToArray(), 0);

            // Currency
            index += 3;
            currency = buffer.CopyOfRange(index, index + 3).ToArray(); // Arrays.copyOfRange(buffer, index, index + 3);

            //string c = BitConverter.ToString(currency, 0);


            // Serial number
            index += 3; //point on serial number
            byte decimalPlace = buffer[index];
            index += 1;
            amitSerialNum = buffer.CopyOfRange(index, index + 16).ToArray(); //Arrays.copyOfRange(buffer, index, index + 16);

            // Max packet len
            index += 16;
            maxPacketLen = buffer.CopyOfRange(index, index + 2).ToArray(); //Arrays.copyOfRange(buffer, index, index + 2);
            
        }
    }

    //===========================================================
    //			TRANSSMIT MESSAGES FROM MACHINE
    //===========================================================


    public class MarshallDisplayMessage : MarshallProtocolMessage
    {

        public const byte DISPLAY_MESSAGE_OPCODE = 0x08;
        public const byte DISPLAY_MESSAGE_RESPONSE_CODE = 0x09;
        private readonly string Message;

        public MarshallDisplayMessage(string Message)
            : base(DISPLAY_MESSAGE_OPCODE)
        {
            this.Message = Message;
        }
        public override byte[] dataToBuffer()
        {
            var buffer = new byte[1 + Message.Length];

            Array.Copy(new[] { DISPLAY_MESSAGE_OPCODE }, 0, buffer, 0, 1);
            Array.Copy(Encoding.UTF8.GetBytes(Message), 0, buffer, 1, Message.Length);

            return buffer;
        }

        public override byte ackRequired()
        {
            return ACK;
        }
    }

    public class MarshallKeepAliveMessage : MarshallProtocolMessage
    {

        public const byte KEEP_ALIVE_OPCODE = 0x07;

        public MarshallKeepAliveMessage()
            : base(KEEP_ALIVE_OPCODE)
        {
        }

        public override byte[] dataToBuffer()
        {
            return null;
        }

        public override byte ackRequired()
        {
            return ACK;
        }

    }

    public class MarshallResponeTxMessage : MarshallProtocolMessage
    {

        public new static readonly byte RESPONSE_OPCODE = (byte)0x00;
        public const byte ACK_OK = 0x00;


        public MarshallResponeTxMessage()
            : base(RESPONSE_OPCODE)
        {
        }

        public override byte[] dataToBuffer()
        {
            byte[] ack = new byte[] { ACK_OK };
            return ack;
        }

        public override byte ackRequired()
        {
            return 0x00;
        }
    }

    public class ReaderEnableMessage : MarshallProtocolMessage
    {

        public new static readonly byte MDB_OPCODE = unchecked((byte)0x80);
        private static readonly byte READER_CMD = (byte)0x14;
        private static readonly byte READER_ENABLE = (byte)0x01;

        internal byte[] mdbSubCommand = new byte[2];

        public ReaderEnableMessage()
            : base(MDB_OPCODE)
        {
            mdbSubCommand[0] = READER_CMD;
            mdbSubCommand[1] = READER_ENABLE;
        }

        public override byte[] dataToBuffer()
        {
            return mdbSubCommand;
        }

        public override byte ackRequired()
        {
            return ACK;
        }
    }

    public class ReaderDisableMessage : MarshallProtocolMessage
    {

        public new static readonly byte MDB_OPCODE = unchecked((byte)0x80);
        private static readonly byte READER_CMD = (byte)0x14;
        private static readonly byte READER_DISABLE = (byte)0x00;

        internal byte[] mdbCommand = new byte[2];

        public ReaderDisableMessage()
            : base(MDB_OPCODE)
        {
            mdbCommand[0] = READER_CMD;
            mdbCommand[1] = READER_DISABLE;
        }

        public override byte[] dataToBuffer()
        {
            return mdbCommand;
        }

        public override byte ackRequired()
        {
            return ACK;
        }
    }

    public class VendRequestMessage : MarshallProtocolMessage
    {

        public static readonly byte VEND_REQUEST = (byte)0x00;

        internal byte[] mdbCommand = new byte[6];
        public VendRequestMessage()
            : base(MDB_OPCODE)
        {

        }
        public VendRequestMessage(byte[] price)
            : base(MDB_OPCODE)
        {
            mdbCommand[0] = (byte)VEND_CMD;
            mdbCommand[1] = VEND_REQUEST;

            // Price
            mdbCommand[2] = price[0];
            mdbCommand[3] = price[1];
            // Code
            mdbCommand[4] = 0xff;
            mdbCommand[5] = 0xff;

        }

        public override byte[] dataToBuffer()
        {
            return mdbCommand;
        }

        public override byte ackRequired()
        {
            return ACK;
        }

    }

    public class VendSuccessMessage : MarshallProtocolMessage
    {

        public static readonly byte VEND_SUCCESS = (byte)0x02;

        internal byte[] mdbCommand = new byte[2];

        public VendSuccessMessage()
            : base(MDB_OPCODE)
        {
            mdbCommand[0] = (byte)VEND_CMD;
            mdbCommand[1] = VEND_SUCCESS;
        }

        public override byte[] dataToBuffer()
        {
            return mdbCommand;
        }

        public override byte ackRequired()
        {
            return ACK;
        }
    }

    public class VendCancelMessage : MarshallProtocolMessage
    {

        public static readonly byte VEND_CANCEL = (byte)0x01;

        internal byte[] mdbCommand = new byte[2];

        public VendCancelMessage()
            : base(MDB_OPCODE)
        {
            mdbCommand[0] = (byte)VEND_CMD;
            mdbCommand[1] = VEND_CANCEL;
        }

        public override byte[] dataToBuffer()
        {
            return mdbCommand;
        }

        public override byte ackRequired()
        {
            return ACK;
        }
    }

    public class VendFailurelMessage : MarshallProtocolMessage
    {

        public static readonly byte VEND_FAILURE = (byte)0x03;

        internal byte[] mdbCommand = new byte[2];

        public VendFailurelMessage()
            : base(MDB_OPCODE)
        {
            mdbCommand[0] = (byte)VEND_CMD;
            mdbCommand[1] = VEND_FAILURE;
        }

        public override byte[] dataToBuffer()
        {
            return mdbCommand;
        }

        public override byte ackRequired()
        {
            return ACK;
        }
    }

    public class SessionCompleteMessage : MarshallProtocolMessage
    {

        public static readonly byte SESSION_COMPLETE = (byte)0x04;

        internal byte[] mdbCommand = new byte[2];

        public SessionCompleteMessage()
            : base(MDB_OPCODE)
        {
            mdbCommand[0] = (byte)VEND_CMD;
            mdbCommand[1] = SESSION_COMPLETE;
        }

        public override byte[] dataToBuffer()
        {
            return mdbCommand;

        }

        public override byte ackRequired()
        {
            return ACK;
        }
    }

    public class MarshallFirmwareInfoMessage : MarshallProtocolMessage
    {

        //        firmwareInfo.PeripheralType = 0x00; // None
        //firmwareInfo.PeripheralSubType = 0x01;  // VPOS
        //firmwareInfo.PeripheralCapabilities = 0x8000; // 2 bytes
        //firmwareInfo.PeripheralModel = new byte[] { 0x50, 0x75, 0x72, 0x6B, 0x75, 0x72, 0x00 };  //Encoding.Default.GetBytes("Purkur\0");
        //firmwareInfo.PeripheralSerialNum = new byte[] { 0x53, 0x65, 0x72, 0x69, 0x61, 0x6C, 0x5F, 0x33, 0x30, 0x37, 0x38, 0x38, 0x00 };  //Encoding.Default.GetBytes("Serial_30788\0");
        //firmwareInfo.PeripheralAppSwVer = new byte[] { 0x50, 0x75, 0x72, 0x6B, 0x75, 0x72, 0x20, 0x56, 0x65, 0x72, 0x31, 0x00 };  //Encoding.Default.GetBytes("Purkur Ver1\0");

        public const byte FIRMWARE_INFO_OPCODE = 0x05;

        private const int _MAJOR_SW_VER = 0x00;
        private const int _MINOR_SW_VER = 0x09;
        private const int MACHINE_TYPE = 0x00; // None  
        private const int MACHINE_SUBTYPE = 0x01; // VPOS

        private const string MACHINE_MODEL = "ENTERMODEL\0";
        private const string MACHINE_SERIAL_NUMBER = "ENTERSERIAL\0";
        private const string MACHINE_SW_VERSION = "ENTERVERSION\0";

        internal byte[] MachineParam;
        internal byte[] InvData;
        internal string machineData;

        private string machineModel;
        private string machineSerNumber;
        private string machineSwVersion;

        public MarshallFirmwareInfoMessage()
            : base(FIRMWARE_INFO_OPCODE)
        {

            MachineParam = new byte[6];

            MachineParam[0] = _MAJOR_SW_VER;
            MachineParam[1] = _MINOR_SW_VER;
            MachineParam[2] = MACHINE_TYPE;
            MachineParam[3] = MACHINE_SUBTYPE;
            MachineParam[4] = 0x80; //capabilities
            MachineParam[5] = 0x00; //capabilities

            //"\0" or '\0' null terminated
            machineModel = MACHINE_MODEL;
            machineSerNumber = MACHINE_SERIAL_NUMBER;
            machineSwVersion = MACHINE_SW_VERSION;
        }

        public override byte DataLength
        {
            get
            {
                return 1;
            }
        }


        public override byte[] dataToBuffer()
        {
            int dataLength = machineModel.Length + machineSerNumber.Length + machineSwVersion.Length;

            var buffer = new byte[MachineParam.Length + dataLength];
            int ind = 0;

            Array.Copy(MachineParam, 0, buffer, 0, MachineParam.Length);
            ind += MachineParam.Length;

            Array.Copy(Encoding.UTF8.GetBytes(machineModel), 0, buffer, ind, machineModel.Length);
            ind += machineModel.Length;

            Array.Copy(Encoding.UTF8.GetBytes(machineSerNumber), 0, buffer, ind, machineSerNumber.Length);
            ind += machineSerNumber.Length;

            Array.Copy(Encoding.UTF8.GetBytes(machineSwVersion), 0, buffer, ind, machineSwVersion.Length);
            
            return buffer;
        }
        public override byte ackRequired()
        {
            return 0x00;
        }
    }

}
