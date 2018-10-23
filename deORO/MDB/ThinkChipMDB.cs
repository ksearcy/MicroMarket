using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.EventAggregation;
using Microsoft.Practices.Composite.Events;
using SimpleUSB;
using deORO.Helpers;
using deORODataAccessApp.Models;
using deORO.Communication;

namespace deORO.MDB
{
    public class ThinkChipMDB : ICommunicationType
    {
        private static ThinkChipMDB mdb = null;

        byte[] OutputBuffer = new byte[32];
        byte[] InputBuffer = new byte[32];

        const byte INIT_MDB_DEVICES = 1;
        const byte COIN_CHANGER = 1;
        const byte BILL_ACCEPTOR = 2;
        const byte COIN_AND_BILL = 3;
        const byte CASH_INSERTED = 2;
        const byte ERROR_STATUS = 4;
        const byte CASH_IN_TUBES = 3;
        const byte CASH_IN_TUBES_EXT = 13;
        const byte CASH_IN_BILL = 6;
        const byte SET_COINCHANGER = 10;
        const byte SET_BILL = 11;
        const byte SEND_CHANGE = 9;
        const byte SETUP_COIN = 8;
        const byte SETUP_BILL = 12;
        const byte BILL_INSERTED = 5;
        const byte COIN_VALUE = 14;
        const byte SET_COINCHANGER_EXT = 15;
        const byte CASH_DISPENSED_MANUALLY = 16;
        const byte COIN_DISPENSE = 17;
        const byte DISPLAY_VAL = 18;

        byte Level = 0;
        byte Scaling = 20;
        int Country = 0;
        byte DecimalPlaces = 0;

        int ScalingBill = 5;
        byte DecimalPlacesBill = 0;
        int CountryBill = 0;
        int StackerCapacity = 0;

        private decimal change = 0.0m;

        private SimpleUSB.SimpleUSB simpleUSB1 = new SimpleUSB.SimpleUSB();
        readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();


        public static ThinkChipMDB GetMDB()
        {
            if (mdb == null)
            {
                mdb = new ThinkChipMDB();
                return mdb;
            }
            else
            {
                return mdb;
            }
        }

        private ThinkChipMDB()
        {
            simpleUSB1.GUID = new Guid("a3c4dc3e-683a-4220-9279-cdba089ea343");
            //simpleUSB1.GUID = new Guid("53d29ef7-377c-4d14-864b-eb3a85769359");

            simpleUSB1.onDeviceConnect += simpleUSB_onDeviceConnect;
            simpleUSB1.onDeviceDisconnect += simpleUSB_onDeviceDisconnect;
            simpleUSB1.onExceptionCatch += simpleUSB_onExceptionCatch;
            simpleUSB1.onReadComplete += simpleUSB_onReadComplete;

            simpleUSB1.OpenConnection();
            
        }

        public void InitBill()
        {
            OutputBuffer[0] = SET_BILL;
            OutputBuffer[1] = 1;
            simpleUSB1.WriteData(1, OutputBuffer, 8);
        }

        public void InitCoin()
        {
            OutputBuffer[0] = SET_COINCHANGER;
            OutputBuffer[1] = 1;
            simpleUSB1.WriteData(1, OutputBuffer, 8);
        }

        public void CloseBill()
        {
            OutputBuffer[0] = SET_BILL;
            OutputBuffer[1] = 0;
            simpleUSB1.WriteData(1, OutputBuffer, 8);
        }

        public void CloseCoin()
        {
            OutputBuffer[0] = SET_COINCHANGER;
            OutputBuffer[1] = 0;
            simpleUSB1.WriteData(1, OutputBuffer, 8);
        }

        public int GetCashInTubes()
        {
            OutputBuffer[0] = CASH_IN_TUBES;
            OutputBuffer[1] = 1;
            simpleUSB1.WriteData(3, OutputBuffer, 8);
            simpleUSB1.ReadBulkEndpoint(4, ref InputBuffer, 8);

            byte msb, lsb;
            int cash_in_tubes;
            double money;

            msb = InputBuffer[1];
            lsb = InputBuffer[2];
            cash_in_tubes = msb;
            cash_in_tubes <<= 8;
            cash_in_tubes |= lsb;

            money = cash_in_tubes / Scaling;

            //OutputBuffer[0] = CASH_IN_TUBES; //ACTION TO BE REALIZED
            //simpleUSB1.WriteData(1, OutputBuffer, 8);


            return 0;

        }

        public void GetCashInTubesExtended()
        {
            OutputBuffer[0] = CASH_IN_TUBES_EXT;
            OutputBuffer[1] = 1;
            simpleUSB1.WriteData(3, OutputBuffer, 8);
            simpleUSB1.ReadBulkEndpoint(4, ref InputBuffer, 8);

            List<Tube> tubes = new List<Tube>();

            byte first = InputBuffer[2];
            byte second = InputBuffer[3];

            if (InputBuffer[0] == CASH_IN_TUBES_EXT && InputBuffer[1] == 1)
            {
                tubes.Add(new Tube() { Index = 1, Count = Convert.ToInt32(InputBuffer[4]), Full = first.GetBit(0) });
                tubes.Add(new Tube() { Index = 2, Count = Convert.ToInt32(InputBuffer[5]), Full = first.GetBit(1) });
                tubes.Add(new Tube() { Index = 3, Count = Convert.ToInt32(InputBuffer[6]), Full = first.GetBit(2) });
                tubes.Add(new Tube() { Index = 4, Count = Convert.ToInt32(InputBuffer[7]), Full = first.GetBit(3) });
            }

            OutputBuffer[0] = CASH_IN_TUBES_EXT;
            OutputBuffer[1] = 2;
            simpleUSB1.WriteData(3, OutputBuffer, 8);
            simpleUSB1.ReadBulkEndpoint(4, ref InputBuffer, 8);

            if (InputBuffer[0] == CASH_IN_TUBES_EXT && InputBuffer[1] == 2)
            {
                tubes.Add(new Tube() { Index = 5, Count = Convert.ToInt32(InputBuffer[2]), Full = first.GetBit(4) });
                tubes.Add(new Tube() { Index = 6, Count = Convert.ToInt32(InputBuffer[3]), Full = first.GetBit(5) });
                tubes.Add(new Tube() { Index = 7, Count = Convert.ToInt32(InputBuffer[4]), Full = first.GetBit(6) });
                tubes.Add(new Tube() { Index = 8, Count = Convert.ToInt32(InputBuffer[5]), Full = first.GetBit(7) });
                tubes.Add(new Tube() { Index = 9, Count = Convert.ToInt32(InputBuffer[6]), Full = second.GetBit(0) });
                tubes.Add(new Tube() { Index = 10, Count = Convert.ToInt32(InputBuffer[7]), Full = second.GetBit(1) });
            }

            OutputBuffer[0] = CASH_IN_TUBES_EXT;
            OutputBuffer[1] = 3;
            simpleUSB1.WriteData(3, OutputBuffer, 8);
            simpleUSB1.ReadBulkEndpoint(4, ref InputBuffer, 8);

            if (InputBuffer[0] == CASH_IN_TUBES_EXT && InputBuffer[1] == 3)
            {
                tubes.Add(new Tube() { Index = 11, Count = Convert.ToInt32(InputBuffer[2]), Full = second.GetBit(2) });
                tubes.Add(new Tube() { Index = 12, Count = Convert.ToInt32(InputBuffer[3]), Full = second.GetBit(3) });
                tubes.Add(new Tube() { Index = 13, Count = Convert.ToInt32(InputBuffer[4]), Full = second.GetBit(4) });
                tubes.Add(new Tube() { Index = 14, Count = Convert.ToInt32(InputBuffer[5]), Full = second.GetBit(5) });
                tubes.Add(new Tube() { Index = 15, Count = Convert.ToInt32(InputBuffer[6]), Full = second.GetBit(6) });
                tubes.Add(new Tube() { Index = 16, Count = Convert.ToInt32(InputBuffer[7]), Full = second.GetBit(7) });

            }

            aggregator.GetEvent<EventAggregation.CashInTubesExtendedCompleteEvent>().Publish(tubes);
        }

        public void GetCashInBill()
        {
            OutputBuffer[0] = CASH_IN_BILL;
            simpleUSB1.WriteData(1, OutputBuffer, 8);
        }

        public void DispenseChange(decimal change)
        {
            int cash_to_send;
            byte msb, lsb;
            decimal number;
            decimal scalingfloat;
            decimal numeric;

            this.change = change;

            scalingfloat = Convert.ToDecimal(Scaling, System.Globalization.CultureInfo.InvariantCulture);
            numeric = change;

            number = numeric * scalingfloat;
            cash_to_send = Convert.ToInt16(number);

            msb = (byte)(cash_to_send >> 8);
            lsb = (byte)(cash_to_send & 0xff);
            OutputBuffer[0] = SEND_CHANGE;
            OutputBuffer[1] = msb;
            OutputBuffer[2] = lsb;

            if (simpleUSB1.DeviceConnected == true)
            {
                simpleUSB1.WriteData(1, OutputBuffer, 8);
            }
        }

        void simpleUSB_onReadComplete()
        {

            simpleUSB1.ReadInterruptEndpoint(ref InputBuffer, 8);

            if (InputBuffer[0] == ERROR_STATUS)
            {

            }

            if (InputBuffer[0] == INIT_MDB_DEVICES)
            {
                if (InputBuffer[1] == COIN_CHANGER || InputBuffer[1] == BILL_ACCEPTOR || InputBuffer[1] == COIN_AND_BILL)
                {

                }
                else
                {
                    aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(new DeviceFailEventArgs()
                    {
                        DeviceType = Helpers.Enum.DeviceType.None,
                        Message = "No devices found to connect"
                    });
                }
            }

            if (InputBuffer[0] == CASH_INSERTED)
            {
                byte msb, lsb;
                int cash;
                byte routing;
                double realcash;

                msb = InputBuffer[1];
                lsb = InputBuffer[2];
                routing = InputBuffer[3];

                cash = msb;
                cash <<= 8;
                cash |= lsb;

                realcash = Convert.ToDouble(cash);
                realcash /= 20;

                //aggregator.GetEvent<EventAggregation.CoinAcceptedEvent>().Publish(realcash);

                string routingMessage = "Disabled";
                if (routing.ToString() == "0")
                {
                    routingMessage = "CashBox";
                }
                else if (routing.ToString() == "1")
                {
                    routingMessage = "Tubes";
                }

                aggregator.GetEvent<EventAggregation.CoinAcceptedEvent>().Publish(new CashEventArgs()
                {
                    Amount = Convert.ToDecimal(realcash, System.Globalization.CultureInfo.InvariantCulture),
                    Routing = routingMessage
                });
            }

            if (InputBuffer[0] == BILL_INSERTED)
            {
                byte msb, lsb;
                byte routing;
                int cash_inserted = 0;
                double cash;
                double cash_float;
                double scaling_float;

                msb = InputBuffer[1];
                lsb = InputBuffer[2];

                routing = InputBuffer[3];

                cash_inserted = msb;
                cash_inserted <<= 8;
                cash_inserted |= lsb;

                cash_float = Convert.ToDouble(cash_inserted);
                scaling_float = Convert.ToDouble(ScalingBill);
                cash = cash_float;

                //aggregator.GetEvent<EventAggregation.BillAcceptedEvent>().Publish(cash);
                aggregator.GetEvent<EventAggregation.BillAcceptedEvent>().Publish(new CashEventArgs()
                {
                    Amount = Convert.ToDecimal(cash, System.Globalization.CultureInfo.InvariantCulture),
                    Routing = routing.ToString() == "0" ? "Stacked" : "N/A"
                });
            }

            if (InputBuffer[0] == CASH_IN_TUBES)
            {
                byte msb, lsb;
                int cash_in_tubes;
                double money;

                msb = InputBuffer[1];
                lsb = InputBuffer[2];
                cash_in_tubes = msb;
                cash_in_tubes <<= 8;
                cash_in_tubes |= lsb;

                money = cash_in_tubes / Scaling;

                aggregator.GetEvent<EventAggregation.CashInTubesCompleteEvent>().Publish(money);

            }

            if (InputBuffer[0] == CASH_IN_BILL)
            {
                byte msb, lsb;
                int cash_in_bill = 0;

                msb = InputBuffer[1];
                lsb = InputBuffer[2];

                cash_in_bill = msb;
                cash_in_bill <<= 8;
                cash_in_bill |= lsb;

                aggregator.GetEvent<EventAggregation.CashInBillsCompleteEvent>().Publish(cash_in_bill);
            }

            if (InputBuffer[0] == SEND_CHANGE)
            {
                if (InputBuffer[1] != 1)
                {
                    // aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>().Publish(change);
                }
                else
                {
                    // aggregator.GetEvent<EventAggregation.CoinDispenseCompleteEvent>().Publish(change);
                }
            }
        }

        void simpleUSB_onExceptionCatch()
        {

        }

        void simpleUSB_onDeviceDisconnect()
        {
            simpleUSB1.StopReadingInterruptEndpoint();
        }

        void simpleUSB_onDeviceConnect()
        {
            simpleUSB1.StartReadingInterruptEndPoint(2, 8);

            OutputBuffer[0] = INIT_MDB_DEVICES;
            simpleUSB1.WriteData(1, OutputBuffer, 8);
        }

        public void Dispose()
        {
            this.CloseCoin();
            this.CloseBill();
        }


        public CoinAndBillStatusEventArgs GetCoinAndBillStatus()
        {
            return null;
        }

        public void EnableBills(decimal amountDue = 0, string notesSet = "", string transactionType = "Purchase")
        {
            OutputBuffer[0] = SET_BILL;
            OutputBuffer[1] = 1;
            simpleUSB1.WriteData(1, OutputBuffer, 8);
        }

        public void EnableCoins()
        {
            OutputBuffer[0] = SET_COINCHANGER;
            OutputBuffer[1] = 1;
            simpleUSB1.WriteData(1, OutputBuffer, 8);
        }

        public void CloseDevices()
        {

            CloseBill();
            CloseCoin();
        }

        public void InitCreditCard()
        {
            
        }

        public void EnableCreditCard()
        {
            
        }

        public void AuthorizeAmount(decimal amount)
        {
            
        }

        public void CompleteTransaction()
        {
            
        }

        public void DisableCreditCard()
        {
            
        }


        public void Close()
        {
            //throw new NotImplementedException();
        }


        public void ResetCoinHopper()
        {
            throw new NotImplementedException();
        }

        public void ResetBillRecycler()
        {
            throw new NotImplementedException();
        }

     
        public void ResetMDBCoin()
        {
            throw new NotImplementedException();
        }

        public void ResetMDBBill()
        {
            throw new NotImplementedException();
        }


    }
}
