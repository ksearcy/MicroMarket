using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class CategoryRepository
    {
        deOROEntities entities = new deOROEntities();

        public void AddCategory(category category)
        {
            entities.categories.Add(category);
        }

        public List<category> GetCategories()
        {
            return entities.categories.AsNoTracking().ToList();
        }

        //public List<category> GetSubCategories()
        //{
        //    return entities.categories.Where(x => x.parentid != null).ToList();
        //}

        public List<category> GetMissingItemsSubCategories(int page = 1, int pageSize = 8)
        {
            //return entities.categories.Where(x => x.parentid != 0).OrderBy(x=>x.id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var categories = (from c in entities.categories.Where(x => x.parentid != 0)
                              from i in entities.items.Where(y => y.categoryid == c.id && c.parentid != 0 && y.has_barcode != 1)
                              select c).Distinct().OrderBy(x => x.id).Skip((page - 1) * pageSize).Take(pageSize);

            return categories.ToList();

        }

        public category GetSubCategory(int categoryId)
        {
            return entities.categories.SingleOrDefault(x => x.id == categoryId && x.parentid != null);
        }

        public category GetCategory(int categoryId)
        {
            return entities.categories.SingleOrDefault(x => x.id == categoryId);
        }

        public category GetCategory(string name)
        {
            return entities.categories.SingleOrDefault(x => x.name == name && x.parentid == null);
        }

        public category GetSubCategory(string name)
        {
            return entities.categories.SingleOrDefault(x => x.name == name && x.parentid != null);
        }

        public void Save()
        {
            entities.SaveChanges();
        }

        public void Save(DataTable dt)
        {
            Delete(dt);

            foreach (DataRow dr in dt.Rows)
            {
                category d1 = dr.ConvertToEntity<category>();
                category d2 = GetCategory(d1.id);

                if (d2 != null)
                {
                    Extensions.CopyPropertyValues(d1, d2, new string[] { "id" });
                    entities.Entry(d2).State = EntityState.Modified;
                }
                else
                {
                    entities.categories.Add(d1);
                }
            }
            entities.SaveChanges();
        }

        public void Delete(DataTable dt)
        {
            foreach (category c in entities.categories)
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
                    entities.categories.Remove(c);
            }

            entities.SaveChanges();
        }
    }
}
