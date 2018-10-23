using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.Helpers
{
    public class Enum
    {
        public enum AuthenticationMode
        {
            UserCredentials, FingerPrint, Barcode, HIDCard
        }

        public enum Event
        {
            EmailChanged, PasswordChanged, FingerPrintsChanged, AccountRefilled, ShoppingPayment, UserDeleted, UserCreated
        }

        public enum DeviceType
        {
            BarcodeScanner, CreditCardReader, FingerPrintReader, Bill, Coin, None
        }

        public enum CreditCardProcessor
        {
            USAT, Heartland
        }

        public enum PaymentMethod
        {
            BillPay, CoinPay, CreditCardPay, BillRefill, CoinRefill, CreditCardRefill, MyAccountPay, MyPayrollPay, Reward
        }

        public enum CommunicationType
        {
            MEI, MDB
        }

    }
}
