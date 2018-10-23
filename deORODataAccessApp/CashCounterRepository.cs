using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class CashCounterRepository
    {
        deOROEntities entities = new deOROEntities();

        public bool AddCashCounter(cash_counter cashCounter)
        {
            try
            {
                entities.cash_counter.Add(cashCounter);
                return Convert.ToBoolean(entities.SaveChanges());
            }
            catch
            {
                return false;
            }
        }

        public List<cash_counter> GetList(DateTime? lastSync = null)
        {
                return entities.cash_counter.Where(x => x.created_date_time >= lastSync).ToList();
        }
               

        public decimal GetCashOut(string collectionPkid)
        {
            return entities.cash_counter.Where(x => x.cashcollectionpkid == collectionPkid).Sum(x => x.amount).Value;
        }

        public List<cash_counter> GetList(string[] cashCollectionPkids)
        {
            var records = (from e in entities.cash_counter
                           where e.cashcollectionpkid != null && cashCollectionPkids.Contains(e.cashcollectionpkid)
                           select e).ToList();

            return records;
        }

        public int UpdateCashCollectionPKID(string pkid)
        {
            //var list = from e in entities.cash_counter
            //           where e.cashcollectionpkid == null
            //           select e;

            //foreach (cash_counter counter in list)
            //{
            //    counter.cashcollectionpkid = pkid;
            //}
            
            //return entities.SaveChanges();

            if (pkid == null || pkid == "")
                return 0;

            string sql = string.Format("UPDATE cash_counter SET cashcollectionpkid = '{0}' WHERE cashcollectionpkid is null", pkid);
            return entities.Database.ExecuteSqlCommand(sql);
           
        }

        public List<cash_counter> GetCashCollectedList(string cashCollectionPkid)
        {
            var records = (from e in entities.cash_counter
                           where e.cashcollectionpkid != null && cashCollectionPkid.Contains(e.cashcollectionpkid)
                           select e).ToList();

            return records;
        }


    }
}
