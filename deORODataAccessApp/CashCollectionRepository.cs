using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class CashCollectionRepository
    {
        deOROEntities entities = new deOROEntities();

        public bool AddCashCollection(cash_collection cashCollection)
        {
            try
            {
                entities.cash_collection.Add(cashCollection);
                return Convert.ToBoolean(entities.SaveChanges());
            }
            catch
            {
                return false;
            }
        }

        public List<cash_collection> GetList(DateTime? lastSync = null)
        {
            return entities.cash_collection.Where(x => x.created_date_time >= lastSync).ToList();
        }
                
    }
}
