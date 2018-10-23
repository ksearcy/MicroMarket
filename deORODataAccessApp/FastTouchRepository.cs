using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORODataAccessApp.Models;

namespace deORODataAccessApp
{
    public class FastTouchRepository
    {
        deOROEntities entities = new deOROEntities();

        public int GetFastTouchItemCount()
        {
            var count = (from i in entities.items
                         from f in entities.fast_touch
                         where i.id == f.itemid
                         select new FastTouch
                         {
                             barcode = i.barcode,
                             id = f.id,
                             image = f.image,
                             itemid = i.id,
                             order = f.order.Value,
                             category = f.category

                         }).Count();

            return count;
        }

        public List<FastTouch> GetFastTouchItems(int page, int pageSize)
        {
            var items = (from i in entities.items
                         from f in entities.fast_touch
                         where i.id == f.itemid
                         select new FastTouch
                         {
                             barcode = i.barcode,
                             id = f.id,
                             image = f.image,
                             itemid = i.id,
                             order = f.order.Value,
                             category = f.category

                         }).Distinct().OrderBy(x => x.order).Skip((page - 1) * pageSize).Take(pageSize);

            return items.ToList();

        }
    }
}
