using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class CashStatusRepository
    {
        deOROEntities entities = new deOROEntities();

        public bool AddCashStatus(cash_status cashStatus)
        {
            try
            {
                entities.cash_status.Add(cashStatus);
                return Convert.ToBoolean(entities.SaveChanges());
            }
            catch
            {
                return false;
            }
        }

        public List<cash_status> GetList(DateTime? lastSync = null)
        {
            return entities.cash_status.Where(x => x.created_date_time >= lastSync).ToList();
        }
    }
}
