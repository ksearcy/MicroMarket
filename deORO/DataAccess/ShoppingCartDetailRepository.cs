using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.DataAccess
{
    public class ShoppingCartDetailRepository
    {
        deOROEntities entities = new deOROEntities();

        public List<shoppingcartdetail> GetList(DateTime? lastSync = null)
        {
            return entities.shoppingcartdetails.Where(x => x.created_date_time >= lastSync).ToList();
        }
    }
}
