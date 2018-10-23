using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORODataAccessApp.DataAccess;
using deORODataAccessApp.Models;

namespace deORODataAccessApp
{
    public class SubsidyRepository
    {
        deOROEntities entities = new deOROEntities();

        public int GetSubsidyId(int itemid)
        {
            SubsidyDetailRepository repo1 = new SubsidyDetailRepository();

            foreach (var c in GetAll())
            {
                var details = repo1.GetAll(c.id);

                if (details.Count > 0)
                {
                    if (c.category == "Item Category")
                    {
                        foreach (var d in details)
                        {
                            ItemRepository repo3 = new ItemRepository();
                            var i = repo3.GetItemsByCategory(d.entityid);

                            foreach (var item in i)
                            {
                                if (item.id == itemid)
                                {
                                    return c.id;
                                }
                            }
                        }
                    }
                    else if (c.category == "Item")
                    {
                        foreach (var d in details)
                        {
                            if (d.entityid == itemid)
                            {
                                return c.id;
                            }
                        }
                    }
                }
            }

            return 0;
        }

        public Subsidy GetSubsidy(int id, decimal price)
        {
            var d = entities.subsidies.AsNoTracking().SingleOrDefault(x => x.id == id && x.is_active == 1);

            if (d == null)
                return null;

            Subsidy subsidy = new Subsidy();
            subsidy.Description = d.description;
                        
            switch (d.type)
            {
                case "PERCENT":
                    {
                        subsidy.Percent = d.percent.Value;
                        subsidy.Amount = price * ((d.percent ?? 0) * 0.01m);
                        return subsidy;
                    }
                case "AMOUNT":
                    {
                        subsidy.Amount = d.amount.Value;
                        return subsidy;
                    }
            }

            return null;
        }

        public List<subsidy> GetAll()
        {
            return entities.subsidies.OrderByDescending(x => x.category).ToList();
        }

        public void Save(DataTable dt)
        {
            Delete(dt);

            foreach (DataRow dr in dt.Rows)
            {
                subsidy d1 = dr.ConvertToEntity<subsidy>();
                subsidy d2 = GetSubsidy(d1.id);

                if (d2 != null)
                {
                    Extensions.CopyPropertyValues(d1, d2, new string[] { "id" });
                    entities.Entry(d2).State = EntityState.Modified;
                }
                else
                {
                    entities.subsidies.Add(d1);
                }
            }
            entities.SaveChanges();
        }

        public subsidy GetSubsidy(int id)
        {
            return entities.subsidies.SingleOrDefault(x => x.id == id);
        }

        public void Delete(DataTable dt)
        {
            foreach (subsidy c in entities.subsidies)
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
                    entities.subsidies.Remove(c);
            }

            entities.SaveChanges();
        }
    }
}
