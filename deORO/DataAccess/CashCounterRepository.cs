using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.DataAccess
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

        public void UpdateCashCollectionPKID(string pkid)
        {
            var list = from e in entities.cash_counter
                       where e.cashcollectionpkid == null
                       select e;
            foreach (cash_counter counter in list)
            {
                counter.cashcollectionpkid = pkid;
            }

            entities.SaveChanges();
        }
    }
}
