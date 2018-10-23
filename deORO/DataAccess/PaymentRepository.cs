using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.DataAccess
{
    public class PaymentRepository
    {
        deOROEntities entities = new deOROEntities();

        public List<payment> GetList(DateTime? lastSync = null)
        {
            return entities.payments.Where(x => x.created_date_time >= lastSync).ToList();
        }
    }
}
