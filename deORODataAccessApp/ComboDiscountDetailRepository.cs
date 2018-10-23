using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class ComboDiscountDetailRepository
    {
        deOROEntities entities = new deOROEntities();

        public void Save(DataTable dt)
        {
            Delete(dt);

            foreach (DataRow dr in dt.Rows)
            {
                combo_discount_detail d1 = dr.ConvertToEntity<combo_discount_detail>();
                combo_discount_detail d2 = GetDiscountDetail(d1.id);

                if (d2 != null)
                {
                    Extensions.CopyPropertyValues(d1, d2, new string[] { "id" });
                    entities.Entry(d2).State = EntityState.Modified;
                }
                else
                {
                    entities.combo_discount_detail.Add(d1);
                }
            }
            entities.SaveChanges();
        }

        public combo_discount_detail GetDiscountDetail(int id)
        {
            return entities.combo_discount_detail.SingleOrDefault(x => x.id == id);
        }

        public List<combo_discount_detail> GetAll(int id)
        {
            return entities.combo_discount_detail.Where(x => x.combodiscountid == id).ToList();
        }

        public void Delete(DataTable dt)
        {
            foreach (combo_discount_detail c in entities.combo_discount_detail)
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
                    entities.combo_discount_detail.Remove(c);
            }

            entities.SaveChanges();
        }
    }
}
