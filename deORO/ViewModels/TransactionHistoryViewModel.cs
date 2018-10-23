using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using deORO.Helpers;
using deORODataAccessApp;
using deORODataAccessApp.Models;

namespace deORO.ViewModels
{
    public class TransactionHistoryViewModel : BaseViewModel
    {
        deOROEntities entities = new deOROEntities();


        public ICommand ApplyDatesCommand { get { return new DelegateCommand(() => { GetData(); }); } }

        List<TransactionHistory> transactions;
        string filterText;

        public string FilterText
        {
            get { return filterText; }
            set
            {
                filterText = value;
                RaisePropertyChanged(() => FilterText);
                ApplyFilter();
            }
        }

        public List<TransactionHistory> Transactions
        {
            get { return transactions; }
            set { transactions = value; RaisePropertyChanged(() => Transactions); }
        }


        private DateTime fromDate;

        public DateTime FromDate
        {
            get { return fromDate; }
            set { fromDate = value; RaisePropertyChanged(() => FromDate); }
        }
        private DateTime toDate;

        public DateTime ToDate
        {
            get { return toDate; }
            set { toDate = value; RaisePropertyChanged(() => ToDate); }
        }

        public override void Init()
        {
            FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ToDate = DateTime.Now;

            GetData();

            base.Init();
        }

        private void GetData()
        {
            string sql;
            FilterText = "";
            
            if (Global.User.IsAdmin)
            {
                sql = @"SELECT SUM(shoppingcartdetail.price_tax_included) AS amount, shoppingcart.created_date_time as createddatetime, 
                         [user].username,
                         CASE WHEN shoppingcartdetail.barcode <> 'ACCOUNT_REFILL_BARCODE' THEN 'Purchase' ELSE 'Refill' END AS type
                         FROM shoppingcart INNER JOIN
                         [user] ON shoppingcart.userpkid = [user].pkid INNER JOIN
                         shoppingcartdetail ON shoppingcart.pkid = shoppingcartdetail.shoppingcartpkid WHERE shoppingcartdetail.created_date_time 
                         between '{0}' AND '{1}'
                         GROUP BY shoppingcart.created_date_time, username,
                         CASE WHEN shoppingcartdetail.barcode <> 'ACCOUNT_REFILL_BARCODE' THEN 'Purchase' ELSE 'Refill' END ORDER BY shoppingcart.created_date_time desc";
                sql = String.Format(sql, FromDate.ToString("MM/dd/yyyy"), ToDate.AddDays(1).ToString("MM/dd/yyyy"));
            }
            else
            {
                sql = @"SELECT SUM(shoppingcartdetail.price_tax_included) AS amount, shoppingcart.created_date_time as createddatetime, 
                         [user].username,
                         CASE WHEN shoppingcartdetail.barcode <> 'ACCOUNT_REFILL_BARCODE' THEN 'Purchase' ELSE 'Refill' END AS type
                         FROM shoppingcart INNER JOIN
                         [user] ON shoppingcart.userpkid = [user].pkid INNER JOIN
                         shoppingcartdetail ON shoppingcart.pkid = shoppingcartdetail.shoppingcartpkid WHERE [user].username = '{0}' 
                         AND shoppingcartdetail.created_date_time between '{1}' AND '{2}'
                         GROUP BY shoppingcart.created_date_time, username,
                         CASE WHEN shoppingcartdetail.barcode <> 'ACCOUNT_REFILL_BARCODE' THEN 'Purchase' ELSE 'Refill' END ORDER BY shoppingcart.created_date_time desc";
                sql = String.Format(sql, Global.User.UserName, FromDate.ToString("MM/dd/yyyy"), ToDate.AddDays(1).ToString("MM/dd/yyyy"));
            }

            try
            {

                Transactions = entities.Database.SqlQuery<TransactionHistory>(sql).ToList();
            }
            catch { Transactions.Clear(); }
           
        }

        private void ApplyFilter()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(Transactions);

            if (view != null)
            {
                view.Filter = ((x) =>
                {
                    if (FilterText == "") return true;

                    TransactionHistory item = x as TransactionHistory;

                    if (item.username != null)
                    {
                        if (item.username.ToLower().Contains(FilterText.ToLower()))
                            return true;
                    }

                    if (item.type != null)
                    {
                        if (item.type.ToLower().Contains(FilterText.ToLower()))
                            return true;
                    }

                    if (item.amount != null)
                    {
                        if (item.amount.ToString().Contains(FilterText.ToLower()))
                            return true;
                    }

                    if (item.createddatetime != null)
                    {
                        if (item.createddatetime.ToString().Contains(FilterText.ToLower()))
                            return true;
                    }

                    return false;
                });

                view.Refresh();

            }
        }

    }
}
