using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class TransactionErrorRepository
    {
        deOROEntities entities = new deOROEntities();

        public bool AddItem(transaction_error item)
        {
            try
            {
                entities.transaction_error.Add(item);
                return Convert.ToBoolean(entities.SaveChanges());
            }
            catch
            {
                return false;
            }
        }


        public List<transaction_error> GetList(DateTime? lastSync = null)
        {
            return entities.transaction_error.Where(x => x.created_date_time >= lastSync).ToList();
        }
    }
}
