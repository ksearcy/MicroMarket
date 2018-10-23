using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp
{
    public class SubsidyDetailRepository
    {
        deOROEntities entities = new deOROEntities();


        public List<subsidy_detail> GetAll(int id)
        {
            return entities.subsidy_detail.Where(x=>x.subsidyid == id).ToList();
        }

        public void Save(DataTable dt)
        {
            Delete(dt);

            foreach (DataRow dr in dt.Rows)
            {
                subsidy_detail d1 = dr.ConvertToEntity<subsidy_detail>();
                subsidy_detail d2 = GetSubsidyDetails(d1.id);

                if (d2 != null)
                {
                    Extensions.CopyPropertyValues(d1, d2, new string[] { "id" });
                    entities.Entry(d2).State = EntityState.Modified;
                }
                else
                {
                    entities.subsidy_detail.Add(d1);
                }
            }
            entities.SaveChanges();
        }

        public subsidy_detail GetSubsidyDetails(int id)
        {
            return entities.subsidy_detail.SingleOrDefault(x => x.id == id);
        }

        public void Delete(DataTable dt)
        {
            foreach (subsidy_detail c in entities.subsidy_detail)
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
                    entities.subsidy_detail.Remove(c);
            }

            entities.SaveChanges();
        }
    }
}
