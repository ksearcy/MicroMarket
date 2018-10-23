using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp
{
    public class CreditRepository
    {
        deOROEntities entities = new deOROEntities();

        public void Save(DataTable dt)
        {
            Delete(dt);

            foreach (DataRow dr in dt.Rows)
            {
                credit d1 = dr.ConvertToEntity<credit>();
                credit d2 = GetCreditDetail(d1.id);

                if (d2 != null)
                {
                    Extensions.CopyPropertyValues(d1, d2, new string[] { "id" });
                    entities.Entry(d2).State = EntityState.Modified;
                }
                else
                {
                    entities.credits.Add(d1);
                }
            }
            entities.SaveChanges();
        }

        public credit GetCreditDetail(int id)
        {
            return entities.credits.SingleOrDefault(x => x.id == id);
        }

        public List<credit>GetAll()
        {
            return entities.credits.Where(x => x.is_active == 1).ToList();
        }
        
        public void Delete(DataTable dt)
        {
            foreach (credit c in entities.credits)
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
                    entities.credits.Remove(c);
            }

            entities.SaveChanges();
        }
    }
}
