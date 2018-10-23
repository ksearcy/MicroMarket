using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp
{
    public class ItemSnapshotRepository
    {
        deOROEntities entities = new deOROEntities();

        public void SnapshotItems(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                string barcode = row["barcode"].ToString();
                var items = entities.items.Where(x => x.barcode == barcode).ToList();

                if (items.Count() > 0)
                {
                    item item = items.ElementAt(0);

                    if (item != null)
                    {
                        item_snapshot snapshot = new item_snapshot();
                        snapshot.pkid = Guid.NewGuid().ToString();
                        snapshot.schedule_date = DateTime.Now.Date;
                        snapshot.created_date_time = DateTime.Now;

                        snapshot.itemid = item.id;
                        snapshot.manufacturerid = item.manufacturerid;
                        snapshot.categoryid = item.categoryid;
                        snapshot.discountid = item.discountid;
                        snapshot.upc = item.upc;
                        snapshot.name = item.name;
                        snapshot.barcode = item.barcode;
                        snapshot.description = item.description;
                        snapshot.quantity = item.quantity;
                        snapshot.stale = item.stale;
                        snapshot.@short = item.@short;
                        snapshot.count = item.count;
                        snapshot.unitcost = item.unitcost;
                        snapshot.avgshelflife = item.avgshelflife;
                        snapshot.pickorder = item.pickorder;
                        snapshot.is_taxable = item.is_taxable;
                        snapshot.price = item.price;
                        snapshot.tax = item.tax;
                        snapshot.price_tax_included = item.price_tax_included;
                        snapshot.tax_percent = item.tax_percent;
                        snapshot.crv = item.crv;
                        snapshot.has_barcode = item.has_barcode;
                        snapshot.image = item.image;
                        snapshot.itemgroupid = item.itemgroupid;

                        entities.item_snapshot.Add(snapshot);
                    }
                }
            }

            entities.SaveChanges();
        }

        public void Delete(DateTime date)
        {
            int count = entities.Database.ExecuteNonQuery("DELETE FROM item_snapshot WHERE schedule_date = '" + date.ToString("d") + "'");
        }

        public List<item_snapshot> GetAll(DateTime date)
        {
            return entities.Database.SqlQuery<item_snapshot>("SELECT * FROM item_snapshot WHERE schedule_date = '" + date.ToString("d") + "'").ToList();
        }
    }
}
