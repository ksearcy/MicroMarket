using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class PlanogramItemRepository
    {
        deOROEntities entities = new deOROEntities();

        public void Save(DataTable dt)
        {
            Delete(dt);

            foreach (DataRow dr in dt.Rows)
            {
                planogram_item d1 = dr.ConvertToEntity<planogram_item>();
                planogram_item d2 = GetPlanogramItem(d1.id);

                if (d2 != null)
                {
                    Extensions.CopyPropertyValues(d1, d2, new string[] { "id" });
                    entities.Entry(d2).State = EntityState.Modified;
                }
                else
                {
                    entities.planogram_item.Add(d1);
                }
            }
            entities.SaveChanges();
        }

        private planogram_item GetPlanogramItem(int id)
        {
            return entities.planogram_item.SingleOrDefault(x => x.id == id);
        }

        public void Delete(DataTable dt)
        {
            foreach (planogram_item c in entities.planogram_item)
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
                    entities.planogram_item.Remove(c);
            }

            entities.SaveChanges();
        }
    }
}
