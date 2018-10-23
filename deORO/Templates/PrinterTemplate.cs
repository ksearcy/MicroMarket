using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.Helpers;
using deORODataAccessApp.DataAccess;
using deORODataAccessApp.Models;
using System.Collections.ObjectModel;
using deORODataAccessApp;

namespace deORO.Templates
{
    class PrinterTemplate
    {
        public string SendPaymentComplete(string shoppingCartPkId, string paymentMethod, List<ShoppingCartItem> items)
        {
            decimal Total = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append("------------------------------------------------");
            sb.AppendLine();
            sb.Append("\x001BE");
            sb.Append("Item");
            sb.Append("\x001BF");
            sb.AppendLine();
            foreach (ShoppingCartItem item in items)
            {
                var str1 = item.Name.Length > 35 ? item.Name.Substring(0, 35) + "..." : item.Name;
                var priceTaxIncluded = item.Price + (item.Price * (item.TaxPercent / 100));
                Total = Total + priceTaxIncluded;
                var discount = item.DiscountPrice;
                var subsidy = item.SubsidyPrice;
                var string1 = priceTaxIncluded.ToString("c");
                var string2 = discount.ToString("c");
                var string3 = subsidy.ToString("c");
                var format = "{0}lb @ {1}/lb";
                //var str2 = string.Format(format, (object) string1);
                sb.Append(str1);
                sb.Append(Constants.HorizontalTab(string1.Length));
                sb.Append("\t");
                sb.Append(string1);
                sb.AppendLine();
                sb.Append("        ");
                //sb.Append(str2);
                sb.AppendLine();
            }
            sb.Append("TOTAL");
            sb.Append(Constants.HorizontalTab("TOTAL".Length));
            sb.Append("\t");
            sb.Append(Total.ToString("c"));
            sb.AppendLine();
            sb.Append("------------------------------------------------");
            sb.AppendLine("Payment Method:" + paymentMethod);
            sb.AppendLine("Transaction Id:" + shoppingCartPkId);
            //<ESC> <GS> y S 0 <STX> <ESC> <GS> y S 1 <NUL> <ESC> <GS> y S 2 <ETX> <ESC> <GS> y D 1 <NUL> <FS> <NUL> h t t p : / / w w w . S t a r M i c r o n i c s . c o m <LF> <ESC> <GS> y P 
            //sb.Append("\x1b\x1d\x79\x53\x30" + model);
            //sb.Append("\x1b\x1d\x79\x53\x31" + correctionLvl);
            //sb.Append("\x1b\x1d\x79\x53\x32" + cellSize);
            //sb.Append("\x1b\x1d\x79\x44\x31\x00" + shoppingCartPkId + "\x00" + "\x0a");
            //sb.Append("\x1b\x1d\x79\x50");

            //sb.AppendLine();
            Global.ShoppingCartItemsForPrint.Clear();
            Global.PaymentMethodForPrint = "";
            Global.ShoppingCartIdForPrint = "";
            return sb.ToString();
        }

        public string CustomerInfo()
        {
            StringBuilder sb = new StringBuilder();
            var location = Global.LocationName;
            var customer = Global.CustomerName;
            sb.Append("\x1b\x1d\x61\x0");
            sb.AppendLine();
            sb.AppendLine("Customer: " + customer);
            sb.AppendLine("Location: " + location);
            sb.AppendLine();
            return sb.ToString();
        }

        private static void Total(Decimal total, StringBuilder sb)
        {
            string @string = total.ToString("c");
            sb.Append("Total");
            sb.Append(Constants.HorizontalTab(@string.Length));
            sb.Append("\t");
            sb.Append(@string);
            sb.AppendLine();
        }


        public string TestPrint()
        {

            string buffer = "\x1b\x1d\x61\x1";             //Center Alignment - Refer to Pg. 3-29
            buffer = buffer + "\x5B" + "If loaded.. Logo1 goes here" + "\x5D\n";
            buffer = buffer + "\x1B\x1C\x70\x1\x0";          //Stored Logo Printing - Refer to Pg. 3-38
            buffer = buffer + "Star Clothing Boutique\n";
            buffer = buffer + "1150 King Georges Post Rd.\n";
            buffer = buffer + "Edison, NJ 08837\n\n";
            buffer = buffer + "\x1b\x1d\x61\x0";             //Left Alignment - Refer to Pg. 3-29
            buffer = buffer + "\x1b\x44\x2\x10\x22\x0";      //Setting Horizontal Tab - Pg. 3-27
            buffer = buffer + "Date: 12/31/2008 " + "\x9" + " Time: 9:10 PM\n";      //Moving Horizontal Tab - Pg. 3-26
            buffer = buffer + "------------------------------------------------ \n";
            buffer = buffer + "\x1b\x45";                    //Select Emphasized Printing - Pg. 3-14
            buffer = buffer + "SALE\n";
            buffer = buffer + "\x1b\x46";                    //Cancel Emphasized Printing - Pg. 3-14
            buffer = buffer + "300678566 " + "\x9" + "  PLAN T-SHIRT" + "\x9" + "         10.99\n";
            buffer = buffer + "300692003 " + "\x9" + "  BLACK DENIM" + "\x9" + "         29.99\n";
            buffer = buffer + "300651148 " + "\x9" + "  BLUE DENIM" + "\x9" + "         29.99\n";
            buffer = buffer + "300642980 " + "\x9" + "  STRIPE DRESS" + "\x9" + "         49.99\n";
            buffer = buffer + "300638471 " + "\x9" + "  BLACK BOOT" + "\x9" + "         35.99\n\n";
            buffer = buffer + "Subtotal " + "\x9" + "" + "\x9" + "        156.95";
            buffer = buffer + "Tax " + "\x9" + "" + "\x9" + "" + "         00.00";
            buffer = buffer + "------------------------------------------------ \n";
            buffer = buffer + "Total" + "\x6" + "" + "\x9" + "\x1b\x69\x1\x1" + "         $156.95\n";    //Character Expansion - Pg. 3-10
            buffer = buffer + "\x1b\x69\x0\x0";                                                          //Cancel Expansion - Pg. 3-10
            buffer = buffer + "------------------------------------------------ \n";
            buffer = buffer + "Charge\n" + "$159.95\n";
            buffer = buffer + "Visa XXXX-XXXX-XXXX-0123\n\n";
            buffer = buffer + "\x1b\x34" + "Refunds and Exchanges" + "\x1b\x35\n";                       //Specify/Cencel White/Black Invert - Pg. 3-16
            buffer = buffer + "Within " + "\x1b\x2d\x1" + "30 days" + "\x1b\x2d\x0" + " with receipt\n"; //Specify/Cancel Underline Printing - Pg. 3-15
            buffer = buffer + "And tags attached\n\n";
            buffer = buffer + "\x1b\x1d\x61\x1";             //Center Alignment - Refer to Pg. 3-29
            buffer = buffer + "\x1b\x62\x6\x2\x2" + " 12ab34cd56" + "\x1e\n";             //Barcode - Pg. 3-39 - 3-40
            buffer = buffer + "\x1b\x64\x02";                                            //Cut  - Pg. 3-41
            buffer = buffer + "\x7";

            return buffer;

        }

        public string UserBalance()
        {
            StringBuilder sb = new StringBuilder();
            var balance = Global.User.AccountBalance.ToString("c");
            var credit = Global.CreditToAccount.ToString("c");
            var amountPaid = Global.AmountPaid.ToString("c");
            sb.Append("\x1b\x1d\x61\x1");
            sb.AppendLine();
            sb.Append("------------------------------------------------");
            sb.AppendLine();
            sb.Append(Constants.HorizontalTab(credit.Length));
            sb.AppendLine("Credit to Account: " + credit);
            sb.Append(Constants.HorizontalTab(balance.Length));
            sb.AppendLine("\x1b\x34" + "Account Balance: " + balance + "\x1b\x35\n");
            sb.AppendLine();
            sb.Append("------------------------------------------------");
            sb.AppendLine();
            return sb.ToString();
        }

        public string AccountRefill(decimal PreviousAccountBalance, decimal NewAccountBalance, decimal RefillAmount)
        {
            StringBuilder sb = new StringBuilder();
            string balance = NewAccountBalance.ToString("c");
            string previousbalance = PreviousAccountBalance.ToString("c");
            string refillamountforaccount = RefillAmount.ToString("c");
            sb.Append("\x1b\x1d\x61\x1");
            sb.AppendLine();
            sb.Append("------------------------------------------------");
            sb.AppendLine();
            sb.Append(Constants.HorizontalTab(refillamountforaccount.Length));
            sb.AppendLine("Refilled Amount: " + refillamountforaccount);
            sb.AppendLine();
            sb.Append(Constants.HorizontalTab(previousbalance.Length));
            sb.AppendLine("Previous Account Balance: " + previousbalance);
            sb.Append(Constants.HorizontalTab(balance.Length));
            sb.AppendLine("\x1b\x34" + "New Account Balance: " + balance + "\x1b\x35\n");
            sb.AppendLine();
            sb.Append("------------------------------------------------");
            sb.AppendLine();
            return sb.ToString();
        }


        public string Footer()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("\x001B\x001Da\x0001");
            sb.Append("\x001BE");
            sb.Append("Thank you");
            sb.Append("\x001BF");
            sb.AppendLine();
            sb.AppendLine();
            sb.Append(DateTime.Now.ToString("hh:mm tt MMMM dd yyyy"));
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("\x001Bd\x0002");
            sb.Append("\a");
            return sb.ToString();
        }


        public string CashCollection(string collectionPkId, string user, List<cash_counter> records)
        {
            int BillDenomination = 0;
            decimal Amount = 0;
            decimal CoinsTotalAmount = 0;
            int CoinsTotalCount = 0;
            decimal BillsTotalAmount = 0;
            int Bill1TotalCount = 0;
            int Bill2TotalCount = 0;
            int Bill5TotalCount = 0;
            int Bill10TotalCount = 0;
            int Bill20TotalCount = 0;
            
            foreach (cash_counter cashcount in records)
            {
              
                Amount = Convert.ToDecimal(cashcount.amount);

                if (cashcount.cash_type == "Bill")
                {
                    BillDenomination = Convert.ToInt32(cashcount.amount);

                    switch (BillDenomination)
                    {
                        case 1:
                            Bill1TotalCount += 1;
                            BillsTotalAmount += Amount;
                            break;
                        case 2:
                            Bill2TotalCount += 1;
                            BillsTotalAmount += Amount;
                            break;
                        case 5:
                            Bill5TotalCount += 1;
                            BillsTotalAmount += Amount;
                            break;
                        case 10:
                            Bill10TotalCount += 1;
                            BillsTotalAmount += Amount;
                            break;
                        case 20:
                            Bill20TotalCount += 1;
                            BillsTotalAmount += Amount;
                            break;
                        default:
                            break;
                    }
                }
                else if (cashcount.cash_type == "Coin")
                {
                    CoinsTotalCount += 1;
                    CoinsTotalAmount += Amount;
                }

            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.Append("------------------------------------------------");
            //sb.AppendLine("Collection ID: " + collectionPkId);
            //we must build in how long the data is into the data set command
            string strLen = Char.ConvertFromUtf32("Test".Length);
            //Set QR data    
            sb.AppendLine("\x1b\x1d\x79\x44\x31\x00" + strLen + "\x00" + "Test" + "\x0a");
            sb.AppendLine("UserID: " + user);           
            sb.Append("------------------------------------------------");
            sb.AppendLine();

            sb.Append("Coin Count:");
            sb.Append(Constants.HorizontalTab("Coin Count:".Length));
            sb.Append("\t");
            sb.Append(CoinsTotalCount.ToString());
            sb.AppendLine();
            sb.Append("        ");
            sb.AppendLine();

            sb.Append("Coins Total Amount: ");
            sb.Append(Constants.HorizontalTab("Coins Total Amount:".Length));
            sb.Append("\t");
            sb.Append(CoinsTotalAmount.ToString("c"));
            sb.AppendLine();
            sb.Append("        ");
            sb.AppendLine();

            sb.Append("$1s:");
            sb.Append(Constants.HorizontalTab("$1s:".Length));
            sb.Append("\t");
            sb.Append(Bill1TotalCount.ToString());
            sb.AppendLine();
            sb.Append("        ");
            sb.AppendLine();

            sb.Append("$2s:");
            sb.Append(Constants.HorizontalTab("$2s:".Length));
            sb.Append("\t");
            sb.Append(Bill2TotalCount.ToString());
            sb.AppendLine();
            sb.Append("        ");
            sb.AppendLine();

            sb.Append("$5s:");
            sb.Append(Constants.HorizontalTab("$5s:".Length));
            sb.Append("\t");
            sb.Append(Bill5TotalCount.ToString());
            sb.AppendLine();
            sb.Append("        ");
            sb.AppendLine();

            sb.Append("$10s:");
            //sb.Append(Constants.HorizontalTab("$10s:".Length));
            sb.Append("\t");
            sb.Append(Bill10TotalCount.ToString());
            sb.AppendLine();
            sb.Append("        ");
            sb.AppendLine();

            sb.Append("$20s:");
            //sb.Append(Constants.HorizontalTab("$20s:".Length));
            sb.Append("\t");
            sb.Append(Bill20TotalCount.ToString());
            sb.AppendLine();
            sb.Append("        ");
            sb.AppendLine();

            sb.Append("Bills Total Amount: ");
            sb.Append(Constants.HorizontalTab("Bills Total Amount:".Length));
            sb.Append("\t");
            sb.Append(BillsTotalAmount.ToString("c"));
            sb.AppendLine();
            sb.Append("        ");
            sb.AppendLine();
            sb.Append("------------------------------------------------");
            sb.AppendLine();

   
            //this.Bills(data.Balance, data.CashElements, receipt);

            return sb.ToString();
        }

        //public static void Bills(string cashMeter, List<CashElement> cashElements, StringBuilder sb)
        //{
        //    sb.AppendFormat("{0} : {1}", (object)"Cash Meter", (object)cashMeter);
        //    sb.AppendLine();
        //    string @string = cashElements.Sum<CashElement>((Func<CashElement, int>)(b => b.Value)).ToString("C");
        //    sb.AppendFormat("{0}: {1}", (object)"Total Amount", (object)@string);
        //    sb.AppendLine();
        //    foreach (CashElement cashElement in cashElements)
        //    {
        //        sb.AppendFormat("#{0}: {1}", (object)cashElement.Type.Value, (object)cashElement.Number);
        //        sb.AppendLine();
        //    }
        //    sb.AppendLine();
        //}

        //public void Market(string market, StringBuilder receipt)
        //{
        //    receipt.Append("\x001B\x001Da\x0001");
        //    receipt.AppendLine(market);
        //    receipt.AppendLine();
        //    receipt.AppendLine();
        //}

        //    public void CustomerName(string customerName, StringBuilder receipt)
        //    {
        //        receipt.AppendFormat("{0}: {1}", (object)"Name", (object)customerName);
        //        receipt.AppendLine();
        //    }

        //    public void Transaction(string transactionId, StringBuilder receipt)
        //    {
        //        receipt.Append("\x001B\x001Da\0");
        //        receipt.AppendFormat("{0}: {1}", (object)"Trans", (object)transactionId);
        //        receipt.AppendLine();
        //    }

        //    private static void ShoppingCart(IShoppingCart shoppingCart, StringBuilder receipt)
        //    {
        //        receipt.Append("------------------------------------------------");
        //        receipt.AppendLine();
        //        receipt.Append("\x001BE");
        //        receipt.Append("Item");
        //        receipt.Append("\x001BF");
        //        receipt.AppendLine();
        //        foreach (OrderProduct orderProduct in order.Products.Where<OrderProduct>((Func<OrderProduct, bool>)(o => o.IsProduct)))
        //        {
        //            string str1 = orderProduct.Product.Name.Length > 35 ? orderProduct.Product.Name.Substring(0, 35) + "..." : orderProduct.Product.Name;
        //            bool? isPayByWeight = orderProduct.Product.IsPayByWeight;
        //            int num1;
        //            Decimal num2;
        //            string string1;
        //            string str2;
        //            if (num1 == 0)
        //            {
        //                num2 = (Decimal)orderProduct.QTY * orderProduct.Product.Price;
        //                string1 = num2.ToString("c");
        //                string format = "{0} @ {1}";
        //                // ISSUE: variable of a boxed type
        //                __Boxed<int> local = (ValueType)orderProduct.QTY;
        //                num2 = orderProduct.Product.Price;
        //                string string2 = num2.ToString("C");
        //                str2 = string.Format(format, (object)local, (object)string2);
        //            }
        //            else
        //            {
        //                num2 = orderProduct.Product.PayByWeightQuantity * orderProduct.Product.Price;
        //                string1 = num2.ToString("c");
        //                string format = "{0}lb @ {1}/lb";
        //                // ISSUE: variable of a boxed type
        //                __Boxed<Decimal> local = (ValueType)orderProduct.Product.PayByWeightQuantity;
        //                num2 = orderProduct.Product.Price;
        //                string string2 = num2.ToString("C");
        //                str2 = string.Format(format, (object)local, (object)string2);
        //            }
        //            receipt.Append(str1);
        //            receipt.Append(Constants.HorizontalTab(string1.Length));
        //            receipt.Append("\t");
        //            receipt.Append(string1);
        //            receipt.AppendLine();
        //            receipt.Append("        ");
        //            receipt.Append(str2);
        //            receipt.AppendLine();
        //        }
        //        receipt.Append("------------------------------------------------");
        //        receipt.AppendLine();
        //    }

        //    public void Added(string amountAdded, StringBuilder receipt)
        //    {
        //        receipt.Append("Added");
        //        receipt.Append(Constants.HorizontalTab(amountAdded.Length));
        //        receipt.Append("\t");
        //        receipt.Append(amountAdded);
        //        receipt.AppendLine();
        //    }
        //    public void Balance(string balance, StringBuilder receipt)
        //    {
        //        receipt.Append("Balance");
        //        receipt.Append(Constants.HorizontalTab(balance.Length));
        //        receipt.Append("\t");
        //        receipt.Append(balance);
        //        receipt.AppendLine();
        //    }
        //    internal static void Footer(StringBuilder receipt)
        //    {
        //        receipt.AppendLine();
        //        receipt.Append("\x001B\x001Da\x0001");
        //        receipt.Append("\x001BE");
        //        receipt.Append("Thank you");
        //        receipt.Append("\x001BF");
        //        receipt.AppendLine();
        //        receipt.AppendLine();
        //        receipt.Append(DateTime.Now.ToString("hh:mm tt MMMM dd yyyy"));
        //        receipt.AppendLine();
        //        receipt.AppendLine();
        //        receipt.AppendLine();
        //        receipt.Append("\x001Bd\x0002");
        //        receipt.Append("\a");
        //    }

        //    private string GetUsableTransactionId(PrintObject data)
        //    {
        //        if (data.Order != null)
        //            return data.Order.TransactionId != Guid.Empty ? data.Order.TransactionId.ToString() : data.TransactionId;
        //        return data.TransactionId;
        //    }

        //    private static void Tax(Decimal tax, StringBuilder receipt)
        //    {
        //        if (!(tax != new Decimal(0)))
        //            return;
        //        string @string = tax.ToString("c");
        //        receipt.Append("Tax");
        //        receipt.Append(Constants.HorizontalTab(@string.Length));
        //        receipt.Append("\t");
        //        receipt.Append(@string);
        //        receipt.AppendLine();
        //    }

        //    private static void Discount(Decimal discount, StringBuilder receipt)
        //    {
        //        if (!(discount != new Decimal(0)))
        //            return;
        //        string @string = discount.ToString("c");
        //        receipt.Append("Discount");
        //        receipt.Append(Constants.HorizontalTab(@string.Length));
        //        receipt.Append("\t");
        //        receipt.Append(@string);
        //        receipt.AppendLine();
        //    }

        //    public void Subsidy(Decimal subsidy, StringBuilder receipt)
        //    {
        //        if (!(subsidy != new Decimal(0)))
        //            return;
        //        string @string = subsidy.ToString("c");
        //        receipt.Append("Promotion");
        //        receipt.Append(Constants.HorizontalTab(@string.Length));
        //        receipt.Append("\t");
        //        receipt.Append(@string);
        //        receipt.AppendLine();
        //    }

        //    public void CashoutHeader(StringBuilder receipt)
        //    {
        //        receipt.Append("\x001B\x001Da\0");
        //        receipt.Append("Cashout Receipt");
        //        receipt.AppendLine();
        //    }

        //    public void LocationName(string locationName, StringBuilder receipt)
        //    {
        //        receipt.AppendFormat("{0}: {1}", (object)"Location", (object)locationName);
        //        receipt.AppendLine();
        //    }

        //    private static void Total(Decimal total, StringBuilder receipt)
        //    {
        //        string @string = total.ToString("c");
        //        receipt.Append("Total");
        //        receipt.Append(Constants.HorizontalTab(@string.Length));
        //        receipt.Append("\t");
        //        receipt.Append(@string);
        //        receipt.AppendLine();
        //    }
        //    public void Bills(string cashMeter, List<CashElement> cashElements, StringBuilder receipt)
        //    {
        //        receipt.AppendFormat("{0} : {1}", (object)"Cash Meter", (object)cashMeter);
        //        receipt.AppendLine();
        //        string @string = cashElements.Sum<CashElement>((Func<CashElement, int>)(b => b.Value)).ToString("C");
        //        receipt.AppendFormat("{0}: {1}", (object)"Total Amount", (object)@string);
        //        receipt.AppendLine();
        //        foreach (CashElement cashElement in cashElements)
        //        {
        //            receipt.AppendFormat("#{0}: {1}", (object)cashElement.Type.Value, (object)cashElement.Number);
        //            receipt.AppendLine();
        //        }
        //        receipt.AppendLine();
        //    }
    }

}
