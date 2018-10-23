using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using deORO.Communication;
using deORO.EventAggregation;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;

namespace deORO.MDB
{
    public class E2CMDB : ICommunicationType
    {
        private static E2CMDB e2c;
        private EventDrivenTCPClient client = null;
        readonly static IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();
        private List<TubeInfo> listInfo = new List<TubeInfo>();
        private decimal remainderChange = 0;
        private decimal totalCoinValue = 0;
        private decimal lowestCoin = 0;
        private int billHolding = 0;
        private int coinHolding = 0;
        //private decimal[,] coinArray = new decimal[7, 2] { { 0.05m, 0 }, { 0.10m, 0 }, { 0.25m, 0 }, { 1, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } };
        private bool creditCardTransactionFailed = false;

        List<decimal> notes = new List<decimal>();
        List<decimal> coins = new List<decimal>();

        bool billDispensed = true;
        bool coinDispensed = true;

        public static ICommunicationType GetMDB()
        {
            if (e2c == null)
                e2c = new E2CMDB();
            else
            {
                e2c.listInfo.Clear();
                e2c.notes.Clear();
                e2c.coins.Clear();

                e2c.billDispensed = true;
                e2c.coinDispensed = true;

                e2c.totalCoinValue = 0;
                e2c.remainderChange = 0;
                e2c.creditCardTransactionFailed = false;

                if (e2c.client.ConnectionState != EventDrivenTCPClient.ConnectionStatus.Connected)
                    e2c.Connect();

            }

            return e2c;
        }

        private void Connect()
        {
            try
            {
                IPAddress ipAddress = Dns.GetHostAddresses("127.0.0.1")[0];
                client = new EventDrivenTCPClient(ipAddress, 713);
                client.AutoReconnect = false;
                client.Connect();
                client.DataReceived += client_DataReceived;
                client.ConnectionStatusChanged += client_ConnectionStatusChanged;

            }
            catch (Exception ex)
            {
                aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(
                new DeviceFailEventArgs()
                {
                    DeviceType = Helpers.Enum.DeviceType.E2C,
                    Message = string.Format(ex.Message)
                });
            }
        }

        private E2CMDB()
        {
            Connect();
        }

        void client_ConnectionStatusChanged(EventDrivenTCPClient sender, EventDrivenTCPClient.ConnectionStatus status)
        {
            if (client.ConnectionState != EventDrivenTCPClient.ConnectionStatus.Connected)
            {
                aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(
                new DeviceFailEventArgs()
                {
                    DeviceType = Helpers.Enum.DeviceType.E2C,
                    Message = string.Format("Unable to Connect to iSocket")
                });
            }
        }

        public byte[] PrepareE2CCommand(string command)
        {
            string commandLenHex = command.Length.ToString("X4");
            string hex = BitConverter.ToString(Encoding.Default.GetBytes(command)).Replace("-", "");
            string finalCommand = "00" + commandLenHex + hex + "0000";

            return HexStringToBytes(finalCommand);
        }

        void client_DataReceived(EventDrivenTCPClient sender, object data)
        {
            string response = data.ToString().Substring(6);
            response = response.Replace("\0", "");
            System.Diagnostics.Debug.WriteLine("client_DataReceived " + response);

            if (response.Contains("Note Reader: Cash Holding") || response.Contains("Note Reader Cash Holding"))
            {
                string amount = Regex.Match(response, @"\d+", RegexOptions.IgnorePatternWhitespace).Value;

                if (amount != "")
                {
                    client.Send(PrepareE2CCommand("NoteReader:Accept"));
                }
            }
            //Error: Note Reader <MDB reset> NAK reply
            //Error: Unrecognised Note Reader status message
            else if (response.Contains("Error: Unrecognised Note Reader status message") || response.Contains("Error: Note Reader <MDB reset> NAK reply") || response.Contains("Error: Note inhibited"))
            {
                System.Threading.Thread.Sleep(1000);
                client.Send(PrepareE2CCommand("NoteReader:Disable"));
                client.Send(PrepareE2CCommand("NoteReader:Reset"));
                client.Send(PrepareE2CCommand("NoteReader:Enable"));
            }
            //Error: Note Recycler <MDB reset> NAK reply
            //Error: Unrecognised Note Recycler status message
            else if (response.Contains("Error: Unrecognised Note Recycler status message") ||
                     response.Contains("Error: Note Recycler <MDB reset> NAK reply"))
            {
                System.Threading.Thread.Sleep(1000);
                client.Send(PrepareE2CCommand("NoteRecycler:Disable"));
                client.Send(PrepareE2CCommand("NoteRecycler:Reset"));
                client.Send(PrepareE2CCommand("NoteRecycler:Enable"));
            }

            //Error: Coin Changer <MDB reset> NAK reply
            //Error: Coin Changer non specific dispenser error
            //Error: Unrecognised Coin Changer status message CoinChanger:Reset
            // Warning: Coin Changer thread running too slow
            else if (response.Contains("Error: Unrecognised Coin Changer status message") ||
                     response.Contains("Error: Coin Changer non specific dispenser error")
                     || response.Contains("Error: Coin Changer <MDB reset> NAK reply"))
            {

                System.Threading.Thread.Sleep(1000);
                client.Send(PrepareE2CCommand("CoinChanger:Disable"));
                client.Send(PrepareE2CCommand("CoinChanger:Reset"));
                client.Send(PrepareE2CCommand("CoinChanger:Enable"));
            }
            //Error: Note Recycler <payout amount> NACK response
            else if (response.Contains("Error: Note Recycler <payout amount> NACK response"))
            {
                client.Send(PrepareE2CCommand("NoteRecycler:Enable"));
                client.Send(PrepareE2CCommand("NoteRecycler:Reset"));
                System.Threading.Thread.Sleep(8000);
                client.Send(PrepareE2CCommand("NoteRecycler:Disable"));
            }


            //Error: Coin Hopper <payout amount> NACK response
            else if (response.Contains("Error: Coin Hopper <payout amount> NACK response"))
            {
                client.Send(PrepareE2CCommand("CoinHopper:Enable"));
                client.Send(PrepareE2CCommand("CoinHopper:Reset"));
                System.Threading.Thread.Sleep(8000);
                client.Send(PrepareE2CCommand("CoinHopper:Disable"));
            }
            //Error: Cashless Pay: end session out of sequence
            //Error: Cashless Pay <MDB reset> NAK reply
            //Error: Unrecognised Cashless Pay status message CoinChanger:Reset
            //Cashless Pay: Failed to enable (device initialisation)
            else if (response.Contains("Error: Unrecognised Cashless Pay status message") ||
                     response.Contains("Error: Cashless Pay <MDB reset> NAK reply")
                     || response.Contains("Error: Cashless Pay: end session out of sequence"))
            {
                client.Send(PrepareE2CCommand("CashlessPay:Reset"));
                //How long does it take for the VUI to reset in milseconds?
                System.Threading.Thread.Sleep(1000);
                client.Send(PrepareE2CCommand("CashlessPay:Enable"));
            }
            else if (response.Contains("Unexpected device reset"))
            {
                System.Threading.Thread.Sleep(500);
                client.Send(PrepareE2CCommand("CashlessPay:Enable"));
                System.Threading.Thread.Sleep(500);
            }
            else if (response.Contains("Note Recycler: Cash Holding") ||
                     response.Contains("Note Recycler Cash Holding"))
            {
                string amount = Regex.Match(response, @"\d+", RegexOptions.IgnorePatternWhitespace).Value;

                if (amount != "")
                {
                    client.Send(PrepareE2CCommand("NoteRecycler:Accept"));
                }
            }
            else if (response.Contains("Note Reader Cash In") || response.Contains("Note Recycler Cash In") || response.Contains("Note Recycler: Cash In"))
            {
                string amount = Regex.Match(response, @"\d+", RegexOptions.IgnorePatternWhitespace).Value;

            //    //Added variable billHolding for recycler routing split
            //else if (response.Contains("Note Reader Cash In") || response.Contains("Note Recycler Cash In") || response.Contains("Note Recycler: Cash In"))
            //{
            //    billHolding = Regex.Match(response, @"\d+", RegexOptions.IgnorePatternWhitespace).Value;

            //}
            //else if (response.Contains("Note Reader stacking") || response.Contains("Note Recycler stacked") || response.Contains("Note Recycler: stacked"))
            //{
            //    string amount = billHolding;
                if (amount != "")
                {
                    aggregator.GetEvent<EventAggregation.BillAcceptedEvent>()
                        .Publish(new CashEventArgs() { Amount = Convert.ToDecimal(amount, System.Globalization.CultureInfo.InvariantCulture), Routing = "Stacked" });
                }
            }
            else if (response.Contains("Coin Changer Cash In") && response.Contains("to tube"))
            {
                string amount = Regex.Match(response, @"\d+(\.\d+)?", RegexOptions.IgnorePatternWhitespace).Value;

                if (amount != "")
                {
                    aggregator.GetEvent<EventAggregation.CoinAcceptedEvent>()
                        .Publish(new CashEventArgs() { Amount = Convert.ToDecimal(amount, System.Globalization.CultureInfo.InvariantCulture), Routing = "Tube" });
                }
            }
            else if (response.Contains("Coin Changer Cash In") && response.Contains("to cashbox"))
            {
                string amount = Regex.Match(response, @"\d+(\.\d+)?", RegexOptions.IgnorePatternWhitespace).Value;

                if (amount != "")
                {
                    aggregator.GetEvent<EventAggregation.CoinAcceptedEvent>()
                        .Publish(new CashEventArgs() { Amount = Convert.ToDecimal(amount, System.Globalization.CultureInfo.InvariantCulture), Routing = "CashBox" });
                }
            }
            else if (response.Contains("Card inserted"))
            {
                aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Publish(null);
            }
            else if (response.Contains("Error: Note inhibited"))
            {
                System.Threading.Thread.Sleep(1000);
                client.Send(PrepareE2CCommand("NoteReader:Disable"));
                client.Send(PrepareE2CCommand("NoteReader:Reset"));
                client.Send(PrepareE2CCommand("NoteReader:Enable"));
                aggregator.GetEvent<EventAggregation.NoteReaderInhibitedEvent>().Publish(null);
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                {
                    Event = "NoteReaderInhibited" + " - " + response,
                    DeviceType = Helpers.Enum.DeviceType.Bill,
                    Amount = (decimal) 0.0,
                    Code = "ENK"
                });

            }
            else if (response.Contains("Device waiting for vend request"))
            {
                //aggregator.GetEvent<EventAggregation.CreditCardWaitingForVendApprovalEvent>().Publish(null); 
                //aggregator.GetEvent<EventAggregation.CreditCardReaderDataEvent>().Publish(null);
            }
            else if (response.Contains("Vend approved"))
            {
                string amount = Regex.Match(response, @"\d+(\.\d+)?", RegexOptions.IgnorePatternWhitespace).Value;
                client.Send(PrepareE2CCommand("CashlessPay:VendSuccess"));

                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Approved");
            }
            else if (response.Contains("Vend request failed"))
            {
                System.Threading.Thread.Sleep(2000);
                aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);

                if (creditCardTransactionFailed)
                    return;

                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed" + " " + response);
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                {
                    Event = "CreditCardTransactionFailed" + " - " + response,
                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                    Amount = (decimal)0.0,
                    Code = "EKE"
                });

                creditCardTransactionFailed = true;
            }
            else if (response.Contains("card not present"))
            {
                System.Threading.Thread.Sleep(2000);
                aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);

                if (creditCardTransactionFailed)
                    return;

                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed" + " " + response);
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                {
                    Event = "CreditCardTransactionFailed" + " - " + response,
                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                    Amount = (decimal)0.0,
                    Code = "EKE"
                });

                creditCardTransactionFailed = true;
            }
            else if (response.Contains("Session cancel request"))
            {
                System.Threading.Thread.Sleep(2000);
                aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);

                if (creditCardTransactionFailed)
                    return;

                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed" + " " + response);
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                {
                    Event = "CreditCardTransactionFailed" + " - " + response,
                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                    Amount = (decimal)0.0,
                    Code = "EK2X"
                });

                creditCardTransactionFailed = true;
            }
            else if (response.Contains("Vend denied"))
            {
                System.Threading.Thread.Sleep(2000);
                aggregator.GetEvent<EventAggregation.PopupCloseEvent>().Publish(null);

                if (creditCardTransactionFailed)
                    return;

                aggregator.GetEvent<EventAggregation.CreditCardTransactionEvent>().Publish("Failed" + " " + response);
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                {
                    Event = "CreditCardTransactionFailed" + " - " + response,
                    DeviceType = Helpers.Enum.DeviceType.CreditCardReader,
                    Amount = (decimal)0.0,
                    Code = "EKE"
                });

                creditCardTransactionFailed = true;
            }
            else if (response.Contains("Coin Hopper: Dispensed"))
            {
                coinDispensed = true;
                var match1 = Regex.Match(response, "\\d+(\\.\\d+)?");
                decimal amount = Convert.ToDecimal(match1.ToString(), System.Globalization.CultureInfo.InvariantCulture);

                coins.Add(amount);
                client.Send(PrepareE2CCommand("CoinHopper:Disable"));

                if (billDispensed)
                    aggregator.GetEvent<EventAggregation.CoinDispenseCompleteEvent>().Publish(new DispenseEventArgs()
                    {
                        Coins = coins,
                        CoinsValue = coins.Sum(),
                        Notes = notes,
                        NotesValue = notes.Sum()
                    });

            }
            else if (response.Contains("Note Recycler: Dispensed"))
            {
                billDispensed = true;
                var match1 = Regex.Match(response, "\\d+(\\.\\d+)?");
                decimal amount = Convert.ToDecimal(match1.ToString());

                notes.Add(amount);
                client.Send(PrepareE2CCommand("NoteRecycler:Disable"));

                if (coinDispensed)
                    aggregator.GetEvent<EventAggregation.CoinDispenseCompleteEvent>().Publish(new DispenseEventArgs()
                    {
                        Coins = coins,
                        CoinsValue = coins.Sum(),
                        Notes = notes,
                        NotesValue = notes.Sum()
                    });
            }
            else if (response.Contains("Coin Hopper: Dispense value command failed"))
            {
                aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>().Publish(0);
                client.Send(PrepareE2CCommand("CoinHopper:Disable"));
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                {
                    Event = "Coin Hopper Dispense" + " - " + response,
                    DeviceType = Helpers.Enum.DeviceType.Coin,
                    Amount = (decimal)0.0,
                    Code = "EAN"
                });
            }
            else if (response.Contains("Coin Hopper: Payout timeout"))
            {
                decimal amount = 0;
                try
                {
                    var match1 = Regex.Match(response, "\\d+(\\.\\d+)?");
                    amount = Convert.ToDecimal(match1.ToString(), System.Globalization.CultureInfo.InvariantCulture);
                }
                catch
                {
                }

                aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>().Publish(amount);
                client.Send(PrepareE2CCommand("CoinHopper:Disable"));
            }
            else if (response.Contains("Note Recycler: Dispense value command failed"))
            {
                aggregator.GetEvent<EventAggregation.NoteDispenseFailedEvent>().Publish(0);
                client.Send(PrepareE2CCommand("NoteRecycler:Disable"));
                aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(new TransactionErrorEventArgs
                {
                    Event = "CreditCardTransactionFailed" + " - " + response,
                    DeviceType = Helpers.Enum.DeviceType.Bill,
                    Amount = (decimal)0.0,
                    Code = "EAN"
                });
            }
            //else if (response.Contains("Coin Hopper Maximum Payout"))
            //{
            //    var match1 = Regex.Match(response, "\\d+(\\.\\d+)?");
            //    e2c.maxCoinPayout = Convert.ToDecimal(match1.ToString());
            //}
            //else if (response.Contains("Coin Hopper Minimum Payout"))
            //{
            //    var match1 = Regex.Match(response, "\\d+(\\.\\d+)?");
            //    e2c.minCoinPayout = Convert.ToDecimal(match1.ToString());
            //}
            //else if (response.Contains("Note Recycler Maximum Payout"))
            //{
            //    var match1 = Regex.Match(response, "\\d+(\\.\\d+)?");
            //    e2c.maxCoinPayout = Convert.ToDecimal(match1.ToString());
            //}
            //else if (response.Contains("Note Recycler Minimum Payout"))
            //{
            //    var match1 = Regex.Match(response, "\\d+(\\.\\d+)?");
            //    e2c.minBillPayout = Convert.ToDecimal(match1.ToString());
            //}
            else
            {
                if (response.StartsWith("Coin Changer: Failed to dispense, device is disabled") ||
                    response.StartsWith("Coin Changer: Failed to dispense") ||
                    response.StartsWith("Coin Changer: Invalid dispense value") ||
                    response.Contains("Coin Changer failed to get end of payout status") ||
                    response.Contains("Coin Changer: Dispense value command failed"))
                {
                    aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>().Publish(0);
                    aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(
                        new TransactionErrorEventArgs
                        {
                            Event = "CoinChanger" + " - " + response,
                            DeviceType = Helpers.Enum.DeviceType.Coin,
                            Amount = (decimal) 0.0,
                            Code = "EAN"
                        });
                }

                if (response.StartsWith("Coin Changer Last Payout Value Unpaid") ||
                    response.Contains("Coin Changer Last Payout Value Unpaid"))
                {
                    string amount;

                    var match1 = Regex.Match(response, "Coin Changer Last Payout Value Unpaid: [0-9]+");
                    amount = Regex.Match(match1.Groups[0].Value, @"\d+", RegexOptions.IgnorePatternWhitespace).Value;

                    if (Convert.ToInt16(amount) > 0)
                    {
                        aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>()
                            .Publish(Convert.ToDecimal(amount, System.Globalization.CultureInfo.InvariantCulture) / 100);
                    }
                }

                if (response.StartsWith("Coin Changer Last Payout Coins Paid") ||
                    response.Contains("Coin Changer Last Payout Coins Paid"))
                {
                    if (remainderChange == 0)
                    {
                        var match2 = Regex.Matches(response, @"[0-9]+ of \d+(\.\d+)?");
                        List<decimal> coins = new List<decimal>();

                        foreach (var m in match2)
                        {
                            string matchedString = m.ToString();
                            int coinCount =
                                Convert.ToInt32(matchedString.Substring(0, matchedString.IndexOf("of")).Trim());
                            decimal denomination =
                                Convert.ToDecimal(matchedString.Substring(matchedString.IndexOf("of") + 3).Trim());

                            for (int i = 0; i < coinCount; i++)
                            {
                                coins.Add(denomination);
                            }
                        }

                        aggregator.GetEvent<EventAggregation.CoinDispenseCompleteEvent>()
                            .Publish(new DispenseEventArgs() {Coins = coins, CoinsValue = coins.Sum()});
                    }
                    else
                    {
                        DispenseChange(remainderChange);
                    }
                }

                if (response.StartsWith("Coin Changer: Insufficient coins to dispense value") ||
                    response.Contains("Coin Changer: Insufficient coins to dispense value"))
                {
                    string amount = response
                        .Substring(response.IndexOf("Coin Changer: Insufficient coins to dispense value") + 51, 2)
                        .Trim();

                    if (Convert.ToInt16(amount) > 0)
                    {
                        aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>()
                            .Publish(Convert.ToDecimal(amount, System.Globalization.CultureInfo.InvariantCulture) / 100);
                        aggregator.GetEvent<EventAggregation.LogTransactionErrorEvent>().Publish(
                            new TransactionErrorEventArgs
                            {
                                Event = "CoinChanger" + " - " + response,
                                DeviceType = Helpers.Enum.DeviceType.Coin,
                                Amount = (decimal) 0.0,
                                Code = "EAN"
                            });
                    }
                }

                aggregator.GetEvent<EventAggregation.E2CGeneralEvent>().Publish(response);
            }

            var match3 = Regex.Matches(response, @"Coin Changer: \d+(\.\d+)?USD tube has [0-9]+ coins");
            if (match3.Count > 0)
            {
                int i = 0;

                decimal amount = 0;
                decimal coinCount = 0;
                totalCoinValue = 0;

                foreach (var m in match3)
                {

                    string matchedString = m.ToString();
                    var match4 = Regex.Matches(matchedString, @"\d+(\.\d+)?|[0-9]+");
                    if (match4.Count > 0)
                    {
                        amount = Convert.ToDecimal(match4[0].Value, System.Globalization.CultureInfo.InvariantCulture);
                        coinCount = Convert.ToInt32(match4[1].Value, System.Globalization.CultureInfo.InvariantCulture);
                        if (amount == 0.05m)
                        {
                            lowestCoin = coinCount*amount;
                        }
                        totalCoinValue += coinCount * amount;
                    }
                    i++;
                }
            }
            ////Create Coin route collection
            ////Coin Changer Coin Type 1: 1 [0.05USD] route to tube
            //var match5 = Regex.Matches(response, @"\[*\d+(\.\d+)?USD] route to tube");
            //if (match5.Count > 0)
            //{
            //    int i = 0;
            //    foreach (var m in match5)
            //    {
            //        string matchedString = m.ToString();
            //        var match6 = Regex.Matches(matchedString, @"[0-9]\d*(\.\d+)|\d");
            //        if (match6.Count > 0)
            //        //Added by Kevin: amount and coinCount and TotalCoin accessor 
            //        {
            //            string CoinValue = match6[0].Value;
            //            coinArray[i, 0] = CoinValue;
            //            i++;
            //        }

            //    }
            //}
        }


        public void InitCoin()
        {
            return;
        }

        public void InitBill()
        {
            return;
        }

        public void DispenseChange(decimal change)
        {
            if (Helpers.Global.BillDispenser && Helpers.Global.CoinHopper)
            {
                string[] split = change.ToString().Split('.');

                if (split.Length > 0)
                {
                    if (split[0] != "0")
                    {
                        billDispensed = false;
                        client.Send(PrepareE2CCommand("NoteRecycler:DispenseValue=" + split[0] + ".00USD"));
                    }

                    try
                    {
                        if (split[1] != "00")
                        {
                            coinDispensed = false;
                            client.Send(PrepareE2CCommand("CoinHopper:DispenseValue=0." + split[1] + "USD"));
                        }
                    }
                    catch { }
                }
            }
            else if (Helpers.Global.CoinHopper)
            {
                billDispensed = true;
                client.Send(PrepareE2CCommand("CoinHopper:DispenseValue=" + change + "USD"));
                System.Threading.Thread.Sleep(500);
            }
            else
            {

                if (change > 10)
                {
                    decimal tenDiv = (change / 10);

                    if (tenDiv > 1)
                    {
                        remainderChange = change - 10;

                        client.Send(PrepareE2CCommand("CoinChanger:DispenseValue=" + 10 * 100));
                        System.Threading.Thread.Sleep(500);
                    }
                    else
                    {
                        remainderChange = change % 10;

                        client.Send(PrepareE2CCommand("CoinChanger:DispenseValue=" + 10 * 100));
                        System.Threading.Thread.Sleep(500);
                    }
                }
                else
                {
                    remainderChange = 0;
                    client.Send(PrepareE2CCommand("CoinChanger:DispenseValue=" + change * 100));
                    System.Threading.Thread.Sleep(500);
                }
            }
        }

        public EventAggregation.CoinAndBillStatusEventArgs GetCoinAndBillStatus()
        {
            CoinAndBillStatusEventArgs args = new CoinAndBillStatusEventArgs();
            args.Stacker = new StackerInfo { BillCount = 0, IsFull = false };
            EnableCoins();

            if (Global.EnableDispenseChange) { System.Threading.Thread.Sleep(500); }
            args.Tubes = this.listInfo;

            CloseDevices();
            return args;
        }

        private string notesSurvived = "";


        public void EnableBills(decimal amountDue = 0, string notesSet = "", string transactionType = "Purchase")
        {
            client.Send(PrepareE2CCommand("NoteReader:Reset"));
            client.Send(PrepareE2CCommand("NoteRecycler:Reset"));
            if (notesSet != "")
            {
                if (transactionType == "Purchase")
                {
                    if (Global.CorrectChange)
                    {
                        string notesSurvived = "";
                        string[] notes = notesSet.Replace("USD", "").Split(',');
                        int iCounter = 0;

                        while (iCounter < 3)
                        {
                            if (Global.EnableDispenseChange)
                            {
                                System.Threading.Thread.Sleep(500);
                            }
                            if (totalCoinValue != 0)
                            {
                                notes.Where(x => Convert.ToInt32(x) < totalCoinValue).ToList().ForEach(y =>
                                {
                                    notesSurvived += y + "USD" + ",";
                                });
                                break;
                            }

                            iCounter++;
                        }

                        if (totalCoinValue != 0 && notesSurvived != "" && lowestCoin > 0.50m)
                        {
                            client.Send(PrepareE2CCommand("NoteReader:SetNotes=" + notesSurvived.Substring(0, notesSurvived.Length - 1)));
                            client.Send(PrepareE2CCommand("NoteReader:Enable"));
                        }
                        else if (!Global.EnableDispenseChange)
                        {
                            client.Send(PrepareE2CCommand("NoteReader:SetNotes=" + notesSet));
                            client.Send(PrepareE2CCommand("NoteReader:Enable"));
                        }
                        lowestCoin = 0;
                        System.Threading.Thread.Sleep(1000);
                    }
                    else {   
                            client.Send(PrepareE2CCommand("NoteReader:SetNotes=" + notesSet));
                            client.Send(PrepareE2CCommand("NoteReader:Enable"));                       
                    }
                    
                }
                else 
                {
                    client.Send(PrepareE2CCommand("NoteReader:SetNotes=" + notesSet));
                    client.Send(PrepareE2CCommand("NoteReader:Enable"));
                }

            }

            if (Helpers.Global.BillDispenser)
            {
                client.Send(PrepareE2CCommand("NoteRecycler:Enable"));
                if (notesSet != "")
                {
                    client.Send(PrepareE2CCommand("NoteRecycler:SetNotes=" + notesSet));
                }
                client.Send(PrepareE2CCommand("NoteRecycler:SetRecycleNote=" + Helpers.Global.RecycleNote));
                client.Send(PrepareE2CCommand("NoteRecycler:GetCounts"));
            }
        }

        public void EnableCoins()
        {
            client.Send(PrepareE2CCommand("CoinChanger:Enable"));

            if (Helpers.Global.CoinHopper)
            {
                client.Send(PrepareE2CCommand("CoinHopper:Enable"));
                client.Send(PrepareE2CCommand("CoinHopper:GetCounts"));
            }
        }

        public void CloseDevices()
        {
            client.Send(PrepareE2CCommand("NoteReader:Disable"));
            client.Send(PrepareE2CCommand("CoinChanger:Disable"));
        }

        public void Dispose()
        {
            if (client != null)
            {
                client.DataReceived -= client_DataReceived;
                client.ConnectionStatusChanged -= client_ConnectionStatusChanged;

                try
                {
                    client.Disconnect();
                }
                catch { }

                //try
                //{
                //    client.Dispose();
                //}   
                //catch { }

            }
        }

        public byte[] HexStringToBytes(string hexString)
        {
            if (hexString == null)
                throw new ArgumentNullException("hexString");
            if (hexString.Length % 2 != 0)
                throw new ArgumentException("hexString must have an even length", "hexString");
            var bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                string currentHex = hexString.Substring(i * 2, 2);
                bytes[i] = Convert.ToByte(currentHex, 16);
            }
            return bytes;
        }


        public void InitCreditCard()
        {

        }

        public void EnableCreditCard()
        {
            //client.Send(PrepareE2CCommand("CashlessPay:Reset"));
            //System.Threading.Thread.Sleep(500);
            client.Send(PrepareE2CCommand("CashlessPay:Enable"));
        }

        public void AuthorizeAmount(decimal amount)
        {
            client.Send(PrepareE2CCommand("CashlessPay:VendRequest=" + Decimal.Round(amount, 2)));
        }

        public void DisableCreditCard()
        {
            client.Send(PrepareE2CCommand("CashlessPay:Disable"));
        }


        public void Close()
        {
            try
            {
                client.Send(PrepareE2CCommand("CoinHopper:Disable"));
            }
            catch { }

            try
            {
                client.Send(PrepareE2CCommand("NoteRecycler:Disable"));
            }
            catch { }
        }


        public void ResetCoinHopper()
        {
            client.Send(PrepareE2CCommand("CoinHopper:Enable"));
            System.Threading.Thread.Sleep(1000);
            client.Send(PrepareE2CCommand("CoinHopper:Reset"));
            System.Threading.Thread.Sleep(12000);
            client.Send(PrepareE2CCommand("CoinHopper:Disable"));
            System.Threading.Thread.Sleep(1000);
        }

        public void ResetBillRecycler()
        {
            client.Send(PrepareE2CCommand("NoteRecycler:Enable"));
            System.Threading.Thread.Sleep(1000);
            client.Send(PrepareE2CCommand("NoteRecycler:Reset"));
            System.Threading.Thread.Sleep(12000);
            client.Send(PrepareE2CCommand("NoteRecycler:Disable"));
            System.Threading.Thread.Sleep(1000);
        }

        public void ResetMDBBill()
        {
            client.Send(PrepareE2CCommand("NoteReader:Reset"));
            client.Send(PrepareE2CCommand("NoteReader:Enable"));
            client.Send(PrepareE2CCommand("NoteReader:Disable"));
        }

        public void ResetMDBCoin()
        {
            client.Send(PrepareE2CCommand("CoinChanger:Reset"));
            client.Send(PrepareE2CCommand("CoinChanger:Enable"));
            client.Send(PrepareE2CCommand("CoinChanger:Disable"));
        }
    }
}
