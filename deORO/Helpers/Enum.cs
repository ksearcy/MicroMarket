using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.Helpers
{
    public class Enum
    {
        public enum AuthenticationMode
        {
            UserCredentials, FingerPrint, Barcode
        }

        public enum Event
        {
            EmailChanged, PasswordChanged, FingerPrintsChanged, AccountRefilled, ShoppingPayment, UserDeleted, UserCreated
        }

        public enum DeviceType
        {
            BarcodeScanner, CreditCardReader, FingerPrintReader, Bill, Coin, None, E2C, Login, InventoryAdjusted, HIDReader
        }

        public enum CreditCardProcessor
        {
            USAT, Heartland, Demo, Nayax, CardKnox
        }

        public enum CreditReaderMake
        {
            MagTek, OTI, NayaxE2C, NayaxMarshall, CardKnox, POS, IDTECH
        }

        public enum BarcodeReaderMake
        {
            Honeywell, Code
        }

        public enum PaymentMethod
        {
            BillPay = 1, CoinPay = 2, CreditCardPay = 4, BillRefill = 8, CoinRefill = 16, CreditCardRefill = 32, MyAccountPay = 64, Reward = 128, ChangeAbandoned = 256, CreditAddRefill = 512, MyPayrollPay = 1024
        }

        public enum CommunicationType
        {
            MEI, MDB
        }

        public enum MDBVendor
        {
            Coinco, E2C, ThinkChip, None
        }
    }
}
