using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class PaymentRepository
    {
        deOROEntities entities = new deOROEntities();

        public List<payment> GetList(DateTime? lastSync = null)
        {
            return entities.payments.Where(x => x.created_date_time >= lastSync).ToList();
        }

        public List<payment> GetList(string shoppingCartPkid)
        {
            return entities.payments.Where(x => x.shoppingcartpkid == shoppingCartPkid).ToList();
        }
    }
}
