using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.DataAccess
{
    public class TransactionErrorRepository
    {
        deOROEntities entities = new deOROEntities();

        public bool AddItem(transactionerror item)
        {
            try
            {
                entities.transactionerrors.Add(item);
                return Convert.ToBoolean(entities.SaveChanges());
            }
            catch
            {
                return false;
            }
        }


        public List<transactionerror> GetList(DateTime? lastSync = null)
        {
            return entities.transactionerrors.Where(x => x.created_date_time >= lastSync).ToList();
        }
    }
}
