using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.Marshall
{
    public enum CardEntryTypeEnum
    {
        Unknown = 0,
        MSR_SWIPE = 1,         /* Mag stripes	        */
        CONTACTLESS,           /* Contactless	        */
        CONTACT,               /* Contact	            */
        MIFARE,                /* MiFare	            */
        HID,                   /* HID	                */
        NFC,                   /* NFC	                */
        CNOUS,                 /* CNOUS	            */

        MAX,

    }

    public enum CardTypeEnum
    {
        UNKOWN = 0,                 /* Unknown	            */
        PROPRIETARY,                /* Proprietary	        */
        VISA,                       /* Visa	                */
        MASTER_CARD,                /* Master Card          */
        CHINA_UNION_PAY,            /* China Union Pay	    */
        MAESTRO,                    /* Maestro  	        */
        INTERAC,                    /* Interac  	        */

        CARD_TYPE_MAX,



    }
    public class MarshallCardAuthMessage : MarshallProtocolMessage
    {
        byte[] cardType;
        byte[] cardEntryMode;
        byte[] cardUid;
        byte[] authStatus;

        public MarshallCardAuthMessage(byte[] cardType, byte[] cardEntryMode, byte[] cardUid, byte[] authStatus)
            : base(TRANSFER_DATA)
        {
            this.cardType = cardType;
            this.cardEntryMode = cardEntryMode;
            this.cardUid = cardUid;
            this.authStatus = authStatus;
        }

        public override byte[] dataToBuffer()
        {
            byte[] buff = new byte[cardType.Length + cardEntryMode.Length + cardUid.Length + authStatus.Length];
            Array.Copy(this.cardType, buff, cardType.Length);
            Array.Copy(this.cardEntryMode, buff, cardEntryMode.Length);
            Array.Copy(this.cardUid, buff, cardUid.Length);
            Array.Copy(this.authStatus, buff, authStatus.Length);

            return buff;
        }
    }
    public class MarshallCardDataMessage : MarshallProtocolMessage
    {


        public MarshallCardDataMessage(byte[] buffer)
            : base(TRANSFER_DATA)
        {
            isAckResponseRequired(buffer);

            if (buffer[8] == TRANSFER_DATA)
            {
                updateCardData(buffer);
            }
        }
        public MarshallCardDataMessage()
            : base(TRANSFER_DATA)
        {

        }

        public static MarshallProtocolMessage factory(byte[] buffer)
        {

            MarshallProtocolMessage message = null;

            if (buffer[8] == TRANSFER_DATA)
            {
                message = new MarshallCardDataMessage(buffer);
                updateCardData(buffer);
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

        public static CardTypeEnum GetCardType()
        {
            if (CardType != null && CardType.Count() > 0)
                return (CardTypeEnum)CardType[0];
            return CardTypeEnum.UNKOWN;
        }

        public static CardEntryTypeEnum GetCardEntryType()
        {
            if (CardEntryMode != null && CardEntryMode.Count() > 0)
                return (CardEntryTypeEnum)CardEntryMode[0];
            return CardEntryTypeEnum.Unknown;
        }

        public const byte TRANSFER_DATA = (byte)0x0A;
        public static byte[] TransactionID;
        public static byte[] ChooseProductTimeout;
        public static byte[] CardType;
        public static byte[] CardEntryMode;
        public static byte[] CardBIN;
        public static byte[] CardBinHash;
        public static byte[] ProprieteryCardUID;
        public static byte[] MachineAuthorizationStatus;
        public static byte[] CommStatus;
        public static byte[] CardData;

        public static void updateCardData(byte[] buffer)
        {
            const byte TRANSACTION_ID = 0x01;
            const byte CHOOSE_PRODUCT_TIMEOUT = 0x02;
            const byte CARD_TYPE = 0x03;
            const byte CARD_ENTRY_MODE = 0x04;
            const byte CARD_BIN = 0x05;
            const byte CARD_BIN_HASH = 0x06;
            const byte PROPRAIETRY_CARD_UID = 0x07;
            const byte MACHINE_AUTHORIZATION_STATUS = 0x08;
            const byte COMM_STATUS = 0x09;


            byte[] transactionID = null;
            byte[] chooseProductTimeout = null;
            byte[] cardType = null;
            byte[] cardEntryMode = null;
            byte[] cardBIN = null;
            byte[] cardBinHash = null;
            byte[] proprietyCardUID = null;
            byte[] machineAuthorizationStatus = null;
            byte[] commStatus = null;

            int index = 9;
            int type = 0;
            int typeLength = 0;

            int cardDataLength = buffer.Length - (index - 2);//- 11;

            while (index < cardDataLength)
            {

                type = buffer[index++];
                typeLength = buffer[index++];

                switch (type)
                {
                    case TRANSACTION_ID:
                        transactionID = new byte[typeLength];
                        Array.Copy(buffer, index, transactionID, 0, typeLength);
                        TransactionID = transactionID;
                        index += typeLength;
                        break;

                    case CHOOSE_PRODUCT_TIMEOUT:
                        chooseProductTimeout = new byte[typeLength];
                        Array.Copy(buffer, index, chooseProductTimeout, 0, typeLength);
                        ChooseProductTimeout = chooseProductTimeout;
                        index += typeLength;
                        break;

                    case CARD_TYPE:
                        cardType = new byte[typeLength];
                        Array.Copy(buffer, index, cardType, 0, typeLength);
                        CardType = cardType;
                        index += typeLength;
                        break;
                    case CARD_ENTRY_MODE:
                        cardEntryMode = new byte[typeLength];
                        Array.Copy(buffer, index, cardEntryMode, 0, typeLength);
                        CardEntryMode = cardEntryMode;
                        index += typeLength;
                        break;
                    case CARD_BIN:
                        cardBIN = new byte[typeLength];
                        Array.Copy(buffer, index, cardBIN, 0, typeLength);
                        CardBIN = cardBIN;
                        index += typeLength;
                        break;
                    case CARD_BIN_HASH:
                        cardBinHash = new byte[typeLength];
                        Array.Copy(buffer, index, cardBinHash, 0, typeLength);
                        CardBinHash = cardBinHash;
                        index += typeLength;
                        break;
                    case PROPRAIETRY_CARD_UID:
                        proprietyCardUID = new byte[typeLength];
                        Array.Copy(buffer, index, proprietyCardUID, 0, typeLength);
                        ProprieteryCardUID = proprietyCardUID;
                        index += typeLength;
                        break;
                    case MACHINE_AUTHORIZATION_STATUS:
                        machineAuthorizationStatus = new byte[typeLength];
                        Array.Copy(buffer, index, machineAuthorizationStatus, 0, typeLength);
                        MachineAuthorizationStatus = machineAuthorizationStatus;
                        index += typeLength;
                        break;
                    case COMM_STATUS:
                        commStatus = new byte[typeLength];
                        Array.Copy(buffer, index, commStatus, 0, typeLength);
                        CommStatus = commStatus;
                        index += typeLength;
                        break;
                    default:
                        Console.Write("unsupported type in transfer data 0x{0:x2}\n", buffer[type]);
                        break;
                }
            }


            //CardTypeEnum c = (CardTypeEnum)cardType[0];
            //CardEntryTypeEnum cc = (CardEntryTypeEnum)cardEntryMode[0];
            //string uid = BitConverter.ToString(proprietyCardUID, 0);

        }
  
        public virtual byte[] dataToBuffer()
        {
            return null;
        }

        public virtual byte ackRequired()
        {
            return ACK;
        }

    }

    internal class MarshallStatusMessage : MarshallProtocolMessage
    {

        public const byte CODE_STATUS = (byte)0x0B;

        public MarshallStatusMessage(byte[] buffer)
            : base(CODE_STATUS)
        {
            isAckResponseRequired(buffer);
        }

        
        public static MarshallProtocolMessage factory(byte[] buffer)
        {

            MarshallProtocolMessage message = null;

            if (buffer[8] == CODE_STATUS)
            {
                message = new MarshallStatusMessage(buffer);
                checkStatusMessage(buffer);
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

        public static void checkStatusMessage(byte[] buffer)
        {
            const int UNEXPECTED_ERROR = 0;
            const int TIMEOUT = 1;
            const int OUT_OF_SEQUENCE = 2;
            const int PENDING_COMMUNICATION_RECEIVED = 3;
            const int SETTELMENT_STATUS = 5;

            byte status = buffer[9];

            switch (status)
            {
                case UNEXPECTED_ERROR:
                    Console.WriteLine("\nRX: UNEXPECTED_ERROR");
                    break;
                case TIMEOUT:
                    Console.WriteLine("\nRX: TIMEOUT");
                    break;
                case OUT_OF_SEQUENCE:
                    Console.WriteLine("\nRX: OUT_OF_SEQUENCE");
                    break;
                case PENDING_COMMUNICATION_RECEIVED:
                    Console.WriteLine("\nRX: PENDING_COMMUNICATION");
                    break;
                default:
                    Console.WriteLine("\nRX: UNKNOWN STATUS CODE");
                    break;
            }
        }
    }
}
