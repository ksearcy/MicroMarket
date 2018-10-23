using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deOROSyncData
{
    public interface ISyncData
    {
        void Init();
        Task<int> SyncScheduledItems();
        Task<int> UpdateScheduledStatusAndItemsQuantity();
        void UploadShoppingCartDetails(string pkid);
        void DownloadUser(string pkid);
        bool UploadData();
        bool ProcessCredits();
        bool DownloadData();
        void UploadCashCollectionEvents(string userPkid = "");
        bool AdjustUnevenBalances();
        bool DownloadCredit();
        void SyncTime();
        bool UploadAccountBalances();
        bool UploadShoppingCart();
        bool CashCollection();
    }
}
