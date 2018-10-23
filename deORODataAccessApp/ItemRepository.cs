using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORODataAccessApp.Models;

namespace deORODataAccessApp.DataAccess
{
    public class ItemRepository
    {
        deOROEntities entities = new deOROEntities();

        public item GetItem(string barcode)
        {
            //var items = (from e in entities.items
            //             where e.barcode.Equals(barcode)
            //             select e);
            var items = entities.items.Where(x=> x.barcode.Contains(barcode));

            if (items.Count() > 0)
            {
                return items.ToList().ElementAt(0);
            }

            return null;
        }

        public item GetItem(int itemId)
        {
            return entities.items.AsNoTracking().SingleOrDefault(x => x.id == itemId);
        }

        public List<item> GetAll()
        {
            return entities.items.ToList();
        }

        public List<DamagedItem> GetAllDamaged()
        {
            List<DamagedItem> items = (from i in entities.items.Where(x => x.barcode != "ACCOUNT_REFILL_BARCODE")
                                       from c in entities.categories.Where(x => i.categoryid == x.id).DefaultIfEmpty()
                                       select new DamagedItem
                                       {
                                           Barcode = i.barcode,
                                           Category = c.name,
                                           Name = i.name,
                                           Price = i.price.Value,
                                           Crv = (decimal?)i.@crv.Value ?? 0,
                                           Selected = false
                                           }).OrderBy(x => x.Name).ToList();
                                       //}).OrderBy(x => x.Name).Take(45).ToList();

            return items;
        }

        public async Task<int> ResetOverUnderAndStale()
        {
            return await Task.Run(() =>
                {
                    entities.items.ToList().ForEach(x =>
                        {
                            x.@short = 0;
                            x.stale = 0;
                        });

                    return entities.SaveChanges();
                });
        }

        public List<item> GetItems(List<int> itemids)
        {
            List<item> items = new List<item>();

            itemids.Distinct().ToList().ForEach(x =>
                {
                    var i = entities.items.Where(y => y.id == x).SingleOrDefault();
                    if (i != null)
                    {
                        items.Add(i);
                    }
                });

            return items;
        }

        public List<item> GetItemsByCategory(int categoryid)
        {
            return entities.items.Where(x => x.categoryid == categoryid).ToList();
        }

        public List<item> GetItemsByCategory(int categoryid, int page = 1, int pageSize = 8)
        {
            return entities.items.Where(x => x.categoryid == categoryid && x.has_barcode != 1).
                                  OrderBy(x => x.id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<item> GetItemsByDiscount(int discountid, int page = 1, int pageSize = 8)
        {
            return entities.items.Where(x => x.discountid == discountid).
                                  OrderBy(x => x.id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<item> GetList(DateTime? lastSync = null)
        {
            return entities.items.Where(x => x.barcode != "ACCOUNT_REFILL_BARCODE").ToList();
        }

        public List<Item> GetList()
        {
            var items = (from e in entities.items.Where(x => x.barcode != "ACCOUNT_REFILL_BARCODE")
                         select new Item
                         {
                             barcode = e.barcode,
                             name = e.name,
                             id = e.id,
                             Quantity = (int?)e.quantity.Value ?? 0,
                             Short = (int?)e.@short.Value ?? 0,
                             Stale = (int?)e.stale.Value ?? 0
                         }).ToList();

            return items;

        }

        public List<Item> GetServicedList()
        {
            var items = (from e in entities.items.Where(x => x.barcode != "ACCOUNT_REFILL_BARCODE")
                         from i in entities.item_snapshot
                         where e.id == i.itemid && i.schedule_date.Value.Day == DateTime.Now.Day && i.schedule_date.Value.Month == DateTime.Now.Month && i.schedule_date.Value.Year == DateTime.Now.Year
                         select new Item
                         {
                             barcode = e.barcode,
                             name = e.name,
                             id = e.id,
                             Quantity = (int?)e.quantity.Value ?? 0,
                             Short = (int?)e.@short.Value ?? 0,
                             Stale = (int?)e.stale.Value ?? 0
                         }).ToList();

            return items;
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

        public void UpdateItem(item item)
        {
            entities.Entry(item).State = EntityState.Modified;
            entities.SaveChanges();
        }

        public bool UpdateItem(Item item)
        {
            try
            {
                var entity = entities.items.SingleOrDefault(x => x.id == item.id);

                entity.quantity = item.Quantity;
                entity.@short = 0;
                entity.stale = 0;

                entities.Entry(entity).State = EntityState.Modified;
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

                if (item != null)
                {
                    item.quantity = item.quantity.HasValue ? item.quantity +
                                    Convert.ToInt32(dr["quantity_to_refill"]) : Convert.ToInt32(dr["quantity_to_refill"]);

                    entities.Entry(item).State = EntityState.Modified;
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

        public void Save()
        {
            entities.SaveChanges();
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
                    entities.Entry(item2).State = EntityState.Modified;
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
