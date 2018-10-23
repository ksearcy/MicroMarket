using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using deORO.Communication;
using deORO.EventAggregation;
using Microsoft.Practices.Composite.Events;
using deORO.Helpers;

namespace deORO.MDB
{
    public struct CoinInfo
    {
        public int Value;
        public int Currency;
        public byte Type;
        public byte Route;
    }

    public class NoteInfo
    {
        public byte Index;
        public byte Type;
        public byte Route;
        public decimal Value;
        public bool Visible;
    }

    public class TubeInfo
    {

        public int Number { get; set; }
        public int CoinCount { get; set; }
        public bool IsFull { get; set; }
        public decimal Amount { get; set; }
    }

    public class StackerInfo
    {
        public int BillCount { get; set; }
        public bool IsFull { get; set; }
    }

    public class CoincoMDB : deORO.Communication.ICommunicationType
    {
        public delegate void CallBackHandlerCoin(ushort EvtCode, byte[] hDev, int DataSize, int pData);
        public delegate void CallBackHandlerBill(ushort EvtCode, byte[] hDev, int DataSize, int pData);
        public delegate void CallBackCoinPayoutHandler(ushort EvtCode, byte[] hDev, int DataSize, int pData);

        private static CallBackHandlerBill billHandler = new CallBackHandlerBill(CallBackBill);
        private static CallBackHandlerCoin coinHandler = new CallBackHandlerCoin(CallBackCoin);
        private static CallBackCoinPayoutHandler coinPayoutHandler = new CallBackCoinPayoutHandler(CallBackCoinPayout);

        private static readonly IEventAggregator aggregator = deORO.EventAggregation.deOROEventAggregator.GetEventAggregator();

        private const int maxTubes = 7;
        private const int maxBills = 7;

        private static List<decimal> coins = new List<decimal>();

        #region Coin
        [DllImport("cai_mdb_w32.dll")]
        public static extern int CHGR_Open(byte lPortNum, byte[] pCtx, CallBackHandlerCoin pFunc);

        [DllImport("cai_mdb_w32.dll")]
        public static extern long CHGR_Close(IntPtr hDev);

        [DllImport("cai_mdb_w32.dll")]
        public static extern long CHGR_Reset(IntPtr hDev);

        [DllImport("cai_mdb_w32.dll")]
        public static extern int CHGR_IsOnline(IntPtr hDev);

        [DllImport("cai_mdb_w32.dll")]
        public static extern long CHGR_GetIdent(IntPtr hDev, long pIdent);

        [DllImport("cai_mdb_w32.dll")]
        public static extern long CHGR_GetNumTypes(IntPtr hDev);

        [DllImport("cai_mdb_w32.dll")]
        public static extern int CHGR_GetCoinInfo(IntPtr hDev, int Num, ref CoinInfo pInfo);

        [DllImport("cai_mdb_w32.dll")]
        public static extern int CHGR_GetCoinStatus(IntPtr hDev, int Num, ref int pCnt, ref byte pFlags);

        [DllImport("cai_mdb_w32.dll")]
        public static extern int CHGR_UpdateStatus(IntPtr hDev);

        [DllImport("cai_mdb_w32.dll")]
        public static extern long CHGR_CoinEnable(IntPtr hDev, byte Num, byte EnMask);

        [DllImport("cai_mdb_w32.dll")]
        public static extern long CHGR_Payout(IntPtr hDev, long Value, long Count, CallBackHandlerCoin pFunc, long pCtx);

        [DllImport("cai_mdb_w32.dll")]
        public static extern int CHGR_ValuePayout(IntPtr hDev, int Value, CallBackCoinPayoutHandler pFunc, byte[] pCtx);
        #endregion

        #region Bill
        [DllImport("cai_mdb_w32.dll")]
        public static extern int BVAL_Open(byte lPortNum, byte[] pCtx, CallBackHandlerBill pFunc);

        [DllImport("cai_mdb_w32.dll")]
        public static extern long BVAL_Close(IntPtr hDev);

        [DllImport("cai_mdb_w32.dll")]
        public static extern long BVAL_Reset(IntPtr hDev);


        [DllImport("cai_mdb_w32.dll")]
        public static extern int BVAL_IsOnline(IntPtr hDev);

        [DllImport("cai_mdb_w32.dll")]
        public static extern long BVAL_GetIdent(IntPtr hDev, long pIdent);

        [DllImport("cai_mdb_w32.dll")]
        public static extern long BVAL_GetNumTypes(IntPtr hDev);

        [DllImport("cai_mdb_w32.dll")]
        public static extern long BVAL_GetNoteInfo(IntPtr hDev, long Num, long pInfo);

        [DllImport("cai_mdb_w32.dll")]
        public static extern long BVAL_GetNoteStatus(IntPtr hDev, long Num, long pCnt, long pFlags);

        [DllImport("cai_mdb_w32.dll")]
        public static extern long BVAL_UpdateStatus(IntPtr hDev);

        [DllImport("cai_mdb_w32.dll")]
        public static extern int BVAL_GetStackerCapacity(IntPtr hDev, ref int pCnt);

        [DllImport("cai_mdb_w32.dll")]
        public static extern int BVAL_GetStackerCount(IntPtr hDev, ref int pCnt, ref byte pFlags);

        [DllImport("cai_mdb_w32.dll")]
        public static extern int BVAL_NoteEnable(IntPtr hDev, byte Num, byte EnMask);

        [DllImport("cai_mdb_w32.dll")]
        public static extern long BVAL_Escrow(IntPtr hDev, long Stack, CallBackHandlerBill pFunc, long pCtx);


        #endregion

        #region CAI
        [DllImport("cai_mdb_w32.dll")]
        public static extern long CAI_MDB_Start();

        [DllImport("cai_mdb_w32.dll")]
        public static extern long CAI_MDB_Stop();
        #endregion

        private static CoincoMDB coinco;
        private IntPtr coinHandle;
        private IntPtr billHandle;
        private static decimal change;
        private static int noteTypeCount;
        private static List<NoteInfo> listNoteInfo = new List<NoteInfo>();

        public static ICommunicationType GetMDB()
        {
            if (coinco == null)
                coinco = new CoincoMDB();

            return coinco;
        }

        public void InitBill()
        {
            byte[] b = new byte[1];
            b[0] = 1;

            billHandle = new IntPtr(BVAL_Open(0, b, billHandler));
            GC.KeepAlive(billHandle);

            Global.CoincoBillLastEnableDateTime = DateTime.Now;
          
        }

        public void InitCoin()
        {
            byte[] b = new byte[1];
            b[0] = 0;

            coinHandle = new IntPtr(CHGR_Open(0, b, coinHandler));
            GC.KeepAlive(coinHandler);

            Global.CoincoCoinLastEnableDateTime = DateTime.Now;
        }

        public void CloseBill()
        {
            BVAL_Close(billHandle);
        }

        public void CloseCoin()
        {
            CHGR_Close(coinHandle);
        }

        private CoincoMDB()
        {
            CAI_MDB_Start();
        }

        public CoinAndBillStatusEventArgs GetCoinAndBillStatus()
        {
            StackerInfo stacker = GetBillCount();
            List<TubeInfo> tubes = GetCoinCount();
            return new CoinAndBillStatusEventArgs { Stacker = stacker, Tubes = tubes };
        }

        private List<TubeInfo> GetCoinCount()
        {
            List<TubeInfo> tubes = new List<TubeInfo>();

            for (int i = 0; i <= maxTubes; i++)
            {

                int iCount = 0;
                byte pFlags = new byte();
                CHGR_GetCoinStatus(coinHandle, i, ref iCount, ref pFlags);

                TubeInfo tube = new TubeInfo();
                tube.CoinCount = iCount;
                tube.Number = i + 1;
                try
                {
                    tube.Amount = GetCoinInfo(coinHandle, i).Value;
                }
                catch { }

                if ((pFlags & 0x02) != 0)
                    tube.IsFull = true;
                else
                    tube.IsFull = false;

                tubes.Add(tube);
            }

            return tubes;
        }

        private static Nullable<decimal> GetCoinInfo(IntPtr handle, int iCoin)
        {
            CoinInfo info = new CoinInfo();
            CHGR_GetCoinInfo(handle, iCoin, ref info);

            if (info.Value != 0)
                return decimal.Divide(info.Value, 10000);
            else
                return null;
        }

        private StackerInfo GetBillCount()
        {
            int iCount = 0;
            byte pFlags = new byte();
            BVAL_GetStackerCount(billHandle, ref iCount, ref pFlags);

            StackerInfo stacker = new StackerInfo();
            stacker.BillCount = iCount;

            if ((pFlags & 0x02) != 0)
                stacker.IsFull = true;
            else
                stacker.IsFull = false;

            return stacker;
        }

        public static void CallBackBill(ushort EvtCode, byte[] hDev, int DataSize, int pData)
        {
            System.Diagnostics.Debug.WriteLine("Bill " + EvtCode.ToString());
            byte[] data = new byte[DataSize];
            IntPtr source = new IntPtr(pData);
            Marshal.Copy(source, data, 0, DataSize);
            switch (EvtCode)
            {
                case 2:
                    {
                        CAI_MDB_Start();
                        System.Threading.Thread.Sleep(1000);
                        break;
                    }
                case 128:
                    {
                        noteTypeCount = data[0];

                        for (int i = 0; i < noteTypeCount; i++)
                        {
                            listNoteInfo.Add(new NoteInfo());
                        }
                        break;
                    }
                case 129:
                    {

                        if (data[7] == 255 && data[8] == 255 && data[9] == 255 && data[10] == 255)
                        {
                            NoteInfo info = listNoteInfo[data[0]];
                            info.Index = data[0];
                            info.Value = 0;
                        }
                        else
                        {
                            int noteValue = data[7] * 16777216;
                            noteValue = noteValue + data[8] * 65536;
                            noteValue = noteValue + data[9] * 256;
                            noteValue = noteValue + data[10];
                            noteValue = noteValue / 10000;

                            NoteInfo info = listNoteInfo[data[0]];
                            info.Index = data[0];
                            info.Value = noteValue;
                        }

                        break;
                    }
                case 135:
                    {
                        if (data[2] == 0)
                        {
                            int noteValue = data[4] * 65536;
                            noteValue = noteValue + data[5] * 256;
                            noteValue = noteValue + data[6];
                            noteValue = noteValue / 10000;

                            aggregator.GetEvent<EventAggregation.BillAcceptedEvent>().Publish(new CashEventArgs() { Amount = noteValue, Routing = "Stacked" });
                        }
                        break;
                    }
                case 148:
                    {
                        aggregator.GetEvent<EventAggregation.BillCheatingEvent>().Publish(null);
                        break;
                    }
                case 176:
                    {
                        aggregator.GetEvent<EventAggregation.BillJamEvent>().Publish(null);
                        break;
                    }
            }
        }

        public void EnableBills(decimal amountDue = 0, string notesSet = "", string transactionType = "Purchase")
        {

            string CAI_CONNECTED = BVAL_UpdateStatus(billHandle).ToString();

            if (CAI_CONNECTED == "8295350159626207140")
            {
                CAI_MDB_Start();
                System.Threading.Thread.Sleep(1000);
            }

            int BillValidatorStatus = BVAL_IsOnline(billHandle);

            if (BillValidatorStatus == 0)
            {
                if (DateTime.Now > Global.CoincoBillLastEnableDateTime.AddSeconds(60))
                {

                    InitBill();

                    int mx = 500, i = 0;
                    while (BillValidatorStatus == 0 && i<mx)
                    {
                        BillValidatorStatus = BVAL_IsOnline(billHandle);
                        i += 1;
                    }
                }
                else
                {

                    DialogViewService.ShowAutoCloseDialog(
                      "Cash Devices",
                      "Loading cash devices, please wait 10 seconds and try again."
                      );

                    aggregator.GetEvent<EventAggregation.CoincoCashDevicesDisabled>().Publish("Enabling Devices");

                }

            }
  
            
            if (amountDue == 0)
            {
                EnableBill(1);
                EnableBill(2);
                EnableBill(5);
                EnableBill(10);
                EnableBill(20);
                EnableBill(50);
                EnableBill(100);
            }
            else if (amountDue > 0 && amountDue <= 10)
            {
                EnableBill(1);
                EnableBill(2);
                EnableBill(5);
                EnableBill(10);

                if (amountDue >= Helpers.Global.Enable20WhenAmountIsGreaterOrEqualTo)
                {
                    EnableBill(20);
                }
            }
            //else if (amountDue > 10 && amountDue <= 20)
            else if (amountDue > 10)
            {
                EnableBill(1);
                EnableBill(2);
                EnableBill(5);
                EnableBill(10);
                EnableBill(20);
            }
            //else if (amountDue > 20 && amountDue <= 50)
            //{
            //    EnableBill(1);
            //    EnableBill(2);
            //    EnableBill(5);
            //    EnableBill(10);
            //    EnableBill(20);
            //    EnableBill(50);
            //}
            //else if (amountDue > 50)
            //{
            //    EnableBill(1);
            //    EnableBill(2);
            //    EnableBill(5);
            //    EnableBill(10);
            //    EnableBill(20);
            //    EnableBill(50);
            //    EnableBill(100);
            //}

        }

        private void EnableBill(decimal value)
        {
            try
            {
                int BillValidatorStatus = BVAL_IsOnline(billHandle);

                int mx = 100, i = 0;
                while (BillValidatorStatus == 0 && i < mx)
                {
                    BillValidatorStatus = BVAL_IsOnline(billHandle);
                    i += 1;
                }

                int BillsSettingStatus = -95;
              
                BillsSettingStatus =  BVAL_NoteEnable(billHandle, listNoteInfo.Find(x => x.Value == value).Index, 1);
              
            }
            catch { }
        }

        public void DisableCoins()
        {
            for (int i = 0; i <= maxTubes; i++)
            {
                CHGR_CoinEnable(coinHandle, (byte)i, 0);
            }
        }

        public void DisableBills()
        {
            for (int i = 0; i <= maxBills; i++)
            {
                BVAL_NoteEnable(billHandle, (byte)i, 0);
            }
        }

        public void EnableCoins()
        {
            string CAI_CONNECTED = CHGR_UpdateStatus(coinHandle).ToString();

            if (CAI_CONNECTED == "8295350159626207140")
            {
                CAI_MDB_Start();
                System.Threading.Thread.Sleep(1000);
            }

            int CoinPayoutStatus = CHGR_IsOnline(coinHandle);

            if (CoinPayoutStatus == 0)
            {
                if (DateTime.Now > Global.CoincoCoinLastEnableDateTime.AddSeconds(60))
                {

                    InitCoin();

                    int mx = 500, i = 0;
                    while (CoinPayoutStatus == 0 && i > mx)
                    {
                        CoinPayoutStatus = CHGR_IsOnline(coinHandle);
                        i += 1;
                    }
                }
                else
                {

                    DialogViewService.ShowAutoCloseDialog(
                      "Cash Devices",
                      "Loading cash devices, please wait 10 seconds and try again."
                      );

                    aggregator.GetEvent<EventAggregation.CoincoCashDevicesDisabled>().Publish("Enabling Devices");

                }

            }


            
            int mx2 = 50, j= 0;
            while (CoinPayoutStatus == 0 && j < mx2)
            {
                CoinPayoutStatus = CHGR_IsOnline(coinHandle);
                j += 1;
            }
            for (int i = 0; i <= maxTubes; i++)
            {
                CHGR_CoinEnable(coinHandle, (byte)i, 1);
            }
        }

        public static void CallBackCoin(ushort EvtCode, byte[] hDev, int DataSize, int pData)
        {
            System.Diagnostics.Debug.WriteLine("Coin " + EvtCode.ToString());
            byte[] data = new byte[DataSize];
            IntPtr source = new IntPtr(pData);
            Marshal.Copy(source, data, 0, DataSize);

            switch (EvtCode)
            {
                case 42:
                    {
                        if (data[0] == 1)
                        {
                            System.Diagnostics.Debug.WriteLine("Coin Inhibited by VMC");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Coin Uninhibited by VMC");
                        }

                        break;
                    }
                case 135:
                    {
                        if (data[2] == 0 || data[2] == 1)
                        {
                            decimal coinValue = data[4] * 65536;
                            coinValue = coinValue + data[5] * 256;
                            coinValue = coinValue + data[6];
                            coinValue = coinValue / 10000;

                            if (data[2] == 1)
                            {
                                aggregator.GetEvent<EventAggregation.CoinAcceptedEvent>().Publish(new CashEventArgs() { Amount = coinValue, Routing = "Tube" });
                            }
                            else
                            {
                                aggregator.GetEvent<EventAggregation.CoinAcceptedEvent>().Publish(new CashEventArgs() { Amount = coinValue, Routing = "CashBox" });
                            }
                        }
                        break;
                    }
                case 176:
                    {
                        aggregator.GetEvent<EventAggregation.CoinJamEvent>().Publish(null);
                        break;
                    }
            }
        }

        public void DispenseChange(decimal change)
        {
            int CoinPayoutStatus = CHGR_IsOnline(coinHandle);

            if (CoinPayoutStatus == 1)
            {
                MDB.CoincoMDB.change = change;
                byte[] b = new byte[1];
                b[0] = 0;

                CHGR_ValuePayout(coinHandle, Convert.ToInt32(change * 10000), coinPayoutHandler, b);
            }
            else {

                aggregator.GetEvent<EventAggregation.CoinJamEvent>().Publish("Coin Changer Offline");

            }           

        }

        public static void CallBackCoinPayout(ushort EvtCode, byte[] hDev, int DataSize, int pData)
        {
            System.Diagnostics.Debug.WriteLine(EvtCode.ToString() + "Payout");
            byte[] data = new byte[DataSize];
            IntPtr source = new IntPtr(pData);
            Marshal.Copy(source, data, 0, DataSize);

            switch (EvtCode)
            {
                case 139:
                    {
                        decimal coinValue = data[2] * 65536;
                        coinValue = coinValue + data[3] * 256;
                        coinValue = coinValue + data[4];
                        coinValue = coinValue / 10000;

                        if (coinValue != 0m)
                            coins.Add(coinValue);

                        break;
                    }
                case 140:
                    {

                        decimal coinValue = data[1] * 65536;
                        coinValue = coinValue + data[2] * 256;
                        coinValue = coinValue + data[3];
                        coinValue = coinValue / 10000;

                        if (coins.Count > 0)
                        {
                            aggregator.GetEvent<EventAggregation.CoinDispenseCompleteEvent>().Publish(
                            new DispenseEventArgs
                            {
                                Coins = coins,
                                CoinsValue = coinValue
                            });
                        }
                        else
                        {
                            aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>().Publish(change);
                        }

                        MDB.CoincoMDB.change = 0;
                        break;
                    }
                case 141:
                    {
                        if (data[0] == 3 || data[0] == 4)
                        {
                            aggregator.GetEvent<EventAggregation.CoinDispenseFailedEvent>().Publish(change);

                        }

                        MDB.CoincoMDB.change = 0;
                        break;
                    }
            }
        }

        public void Dispose()
        {
            //System.Threading.Thread t1 = new System.Threading.Thread(DisableBill);
            //t1.Start();

            //System.Threading.Thread t2 = new System.Threading.Thread(DisableCoin);
            //t2.Start();
            System.Threading.Thread.Sleep(1000);
            try { CHGR_Close(coinHandle); }
            catch { }
            try { BVAL_Close(billHandle); }
            catch { }
            try { CAI_MDB_Stop(); }
            catch { }

        }

        public void CloseDevices()
        {

            DisableBills();
            DisableCoins();
            coins.Clear();
        }


        public void InitCreditCard()
        {
            //throw new NotImplementedException();
        }

        public void EnableCreditCard()
        {
            //throw new NotImplementedException();
        }

        public void AuthorizeAmount(decimal amount)
        {
            //throw new NotImplementedException();
        }

        public void CompleteTransaction()
        {
            //throw new NotImplementedException();
        }


        public void DisableCreditCard()
        {
            //throw new NotImplementedException();
        }


        public void Close()
        {
            throw new NotImplementedException();
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
            InitCoin();
        }

        public void ResetMDBBill()
        {
            InitBill();
        }

    }
}
