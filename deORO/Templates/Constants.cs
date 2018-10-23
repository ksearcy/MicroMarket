using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.Templates
{
    class Constants
    {
        private static string receipt = "\x001B\x001Da\x0001[If loaded.. Stored Logo 1 goes here]\n\x001B\x001Cp\x0001\0Star Clothing Boutique\n1150 King Georges Post Rd.\nEdison, NJ 08837\n\n\x001B\x001Da\0\x001BD\x0002\x0010\"\0Date: 12/31/2010 \t Time: 9:10 PM\n------------------------------------------------ \n\x001BESALE\n\x001BF300678566 \t  PLAN T-SHIRT\t         10.99\n300692003 \t  BLACK DENIM\t         29.99\n300651148 \t  BLUE DENIM\t         29.99\n300642980 \t  STRIPE DRESS\t         49.99\n300638471 \t  BLACK BOOT\t         35.99\n\nSubtotal \t\t        156.95Tax \t\t         00.00------------------------------------------------ \nTotal\x0006\t\x001Bi\x0001\x0001         $156.95\n\x001Bi\0\0------------------------------------------------ \nCharge\n$159.95\nVisa XXXX-XXXX-XXXX-0123\n\n\x001B4Refunds and Exchanges\x001B5\nWithin \x001B-\x000130 days\x001B-\0 with receipt\nAnd tags attached\n\n\x001B\x001Da\x0001\x001Bb\x0006\x0002\x0002 12ab34cd56\x001E\n\x001Bd\x0002\a";
        public const string CenterAlignment = "\x001B\x001Da\x0001";
        public const string LeftAlignment = "\x001B\x001Da\0";
        public const string Tab = "\t";
        public const string BoldStart = "\x001BE";
        public const string BoldEnd = "\x001BF";
        public const string BlackWhiteInvertStart = "\x001B4";
        public const string BlackWhiteInvertEnd = "\x001B5";
        public const string UnderlineStart = "\x001B-\x0001";
        public const string UnderlineEnd = "\x001B-\0";
        public const string OpenCashDrawer = "\a";
        public const string Cut = "\x001Bd\x0002";
        public const int MaxProductDescriptionLength = 35;
        public const string Added = "Added";
        public const string RechargeKiosk = "Kiosk";
        public const string Balance = "Balance";
        public const string HouseAccountBalance = "House Account Balance";
        public const string Name = "Name";
        public const string Transaction = "Trans";
        public const string Thankyou = "Thank you";
        public const string DateTimeFormat = "hh:mm tt MMMM dd yyyy";
        public const string Total = "Total";
        public const string Tax = "Tax";
        public const string Deposit = "Deposit";
        public const string Discount = "Discount";
        public const string AdditionalDiscount = "Additional Discount";
        public const string CreditCardConvenienceFee = "Convenience Fee";
        public const string Coupon = "Coupon";
        public const string CurrencyFormat = "c";
        public const string CashOutReceipt = "Cashout Receipt";
        public const string Location = "Location";
        public const string TotalAmount = "Total Amount";
        public const string CashMeter = "Cash Meter";
        public const string EarnedPoints = "Earned Points";
        public const string TotalPoints = "Total Points";
        public const string PromotionalDiscount = "Promotion";
        public const string DashedLine = "------------------------------------------------";

        public static string HorizontalTab(int length)
        {
            switch (length)
            {
                case 0:
                    return "\x001BD0\0";
                case 1:
                    return "\x001BD/\0";
                case 2:
                    return "\x001BD.\0";
                case 3:
                    return "\x001BD-\0";
                case 4:
                    return "\x001BD,\0";
                case 5:
                    return "\x001BD+\0";
                case 6:
                    return "\x001BD*\0";
                case 7:
                    return "\x001BD)\0";
                case 8:
                    return "\x001BD(\0";
                case 9:
                    return "\x001BD'\0";
                case 10:
                    return "\x001BD&\0";
                case 11:
                    return "\x001BD%\0";
                case 12:
                    return "\x001BD$\0";
                case 13:
                    return "\x001BD#\0";
                default:
                    return "\x001BD0\0";
            }
        }
    }
}
