using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.DataAccess
{
    public class CashDispenseRepository
    {
        deOROEntities entities = new deOROEntities();

        public bool AddCashDispense(cash_dispense cashDispense)
        {
            try
            {
                entities.cash_dispense.Add(cashDispense);
                return Convert.ToBoolean(entities.SaveChanges());
            }
            catch
            {
                return false;
            }
        }

        public List<cash_dispense> GetList(DateTime? lastSync = null)
        {
            return entities.cash_dispense.Where(x => x.created_date_time >= lastSync).ToList();
        }
    }
}
