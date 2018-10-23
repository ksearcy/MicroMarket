using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.EventAggregation;

namespace deORO.Communication
{
    public interface ICommunicationType : IDisposable
    {
        void InitCoin();
        void InitBill();
        void InitCreditCard();
        void DispenseChange(decimal change);
        CoinAndBillStatusEventArgs GetCoinAndBillStatus();
        void EnableBills(decimal amountDue = 0, string noteSet = "", string transactionType = "Purchase");
        void EnableCoins();
        void EnableCreditCard();
        void DisableCreditCard();
        void AuthorizeAmount(decimal amount);
        void ResetCoinHopper();
        void ResetBillRecycler();
        void ResetMDBBill();
        void ResetMDBCoin();
        void CloseDevices();
        void Close();
    }
}
