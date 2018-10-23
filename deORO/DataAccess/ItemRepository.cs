using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.Models;

namespace deORO.DataAccess
{
    public class ItemRepository
    {
        deOROEntities entities = new deOROEntities();

        public item GetItem(string barcode)
        {
            var items = (from e in entities.items
                         where e.barcode.Equals(barcode)
                         select e);

            if (items.Count() > 0)
            {
                return items.ToList().ElementAt(0);
            }

            return null;
        }

        public item GetItem(int itemId)
        {
            return entities.items.SingleOrDefault(x => x.id == itemId);
        }

        public List<item> GetItemsByCategory(int categoryid, int page = 1, int pageSize = 8)
        {
            return entities.items.Where(x => x.categoryid == categoryid && x.has_barcode != 1).OrderBy(x => x.id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<item> GetItemsByDiscount(int discountid, int page = 1, int pageSize = 8)
        {
            return entities.items.Where(x => x.discountid == discountid).OrderBy(x => x.id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<item> GetList(DateTime? lastSync = null)
        {
            return entities.items.Where(x => x.barcode != "ACCOUNT_REFILL_BARCODE").ToList();
        }


        public bool AddItem(item item)
        {
            try
            {
                entities.items.Add(item);
                return Convert.ToBoolean(entities.SaveChanges());
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateItem(item item)
        {
            try
            {
                return Convert.ToBoolean(entities.SaveChanges());
            }
            catch
            {
                return false;
            }
        }

        public int UpdateItems(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                string barcode = dr["barcode"].ToString();
                var item = entities.items.Where(x => x.barcode == barcode).FirstOrDefault();
                
                if (item!=null)
                {
                    item.quantity = item.quantity.HasValue ? item.quantity + 
                                    Convert.ToInt32(dr["quantity_to_refill"]) : Convert.ToInt32(dr["quantity_to_refill"]);
                }

            }

            return entities.SaveChanges();
        }

        public void UpdateItemsQuantity(List<ShoppingCartItem> shoppingItems)
        {
            shoppingItems.ForEach(x =>
            {
                if (x.BarCode != "ACCOUNT_REFILL_BARCODE")
                {
                    var items = entities.items.Where(y => y.barcode == x.BarCode);

                    if (items.Count() > 0)
                    {
                        item item = items.ToList().ElementAt(0);

                        if (item != null)
                            item.quantity -= 1;
                    }
                   
                }
            });

            entities.SaveChanges();
        }

        public void DeleteItemsNotIn(DataTable dt)
        {
            List<item> items = FindItemsNotIn(dt);

            foreach (item item in items)
            {
                if (item != null)
                    Delete(item);
            }

        }

        private List<item> FindItemsNotIn(DataTable dt)
        {
            return entities.items.Select(
                delegate(item y)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (y.barcode == row["barcode"].ToString())
                            return null;
                    }
                    return y;
                }
             ).ToList();
        }

        public void Save(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                item item1 = dr.ConvertToEntity<item>();
                item item2 = GetItem(item1.id);

                if (item2 != null)
                {
                    Extensions.CopyPropertyValues(item1, item2, new string[] { "id", "quantity" });
                }
                else
                {
                    entities.items.Add(item1);
                }
            }
            entities.SaveChanges();

            DeleteItemsNotIn(dt); ;
        }

        public void Delete(DataTable dt)
        {
            Delete(dt);

            foreach (item c in entities.items)
            {
                bool exists = false;
                foreach (DataRow row in dt.Rows)
                {
                    if (c.id == Convert.ToInt32(row["id"]))
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    entities.items.Remove(c);
            }

            entities.SaveChanges();
        }

        public bool Delete(item item)
        {
            if (item != null)
            {
                entities.items.Remove(item);
                return Convert.ToBoolean(entities.SaveChanges());
            }
            else
            {
                return false;
            }
        }

        public bool Delete(int itemId)
        {
            var item = entities.items.SingleOrDefault(x => x.id == itemId);

            if (item != null)
            {
                entities.items.Remove(item);
                entities.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
