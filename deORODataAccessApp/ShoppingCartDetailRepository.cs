using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class ShoppingCartDetailRepository
    {
        deOROEntities entities = new deOROEntities();

        public List<shoppingcartdetail> GetList(DateTime? lastSync = null)
        {
            if (lastSync == null)
            {
                return entities.shoppingcartdetails.ToList();
            }
            else
            {
                return entities.shoppingcartdetails.Where(x => x.created_date_time >= lastSync).ToList();
            }
        }

        public List<shoppingcartdetail> GetList(string shoppingCartPkid)
        {
            return entities.shoppingcartdetails.Where(x => x.shoppingcartpkid == shoppingCartPkid).ToList();
        }
    }
}
