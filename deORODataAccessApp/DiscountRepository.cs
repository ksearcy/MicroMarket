using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using deORODataAccessApp.Models;

namespace deORODataAccessApp.DataAccess
{
    public class DiscountRepository
    {
        deOROEntities entities = new deOROEntities();

        public Discount GetDiscount(DateTime when, int discountId, decimal price)
        {
            var d = entities.discounts.AsNoTracking().SingleOrDefault(x => x.id == discountId && x.is_active == 1);

            if (d == null)
                return null;

            bool exit = false;

            Discount discount = new Discount();
            discount.Description = d.description;

            switch (when.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    exit = d.monday == 0 ? true : false;
                    break;
                case DayOfWeek.Tuesday:
                    exit = d.tuesday == 0 ? true : false;
                    break;
                case DayOfWeek.Wednesday:
                    exit = d.wednesday == 0 ? true : false;
                    break;
                case DayOfWeek.Thursday:
                    exit = d.thursday == 0 ? true : false;
                    break;
                case DayOfWeek.Friday:
                    exit = d.friday == 0 ? true : false;
                    break;
                case DayOfWeek.Saturday:
                    exit = d.saturday == 0 ? true : false;
                    break;
                case DayOfWeek.Sunday:
                    exit = d.sunday == 0 ? true : false;
                    break;
            }

            if (exit)
            {
                return null;
            }

            DateTime dateToCompare = new DateTime(when.Year, when.Month, when.Day);

            if (d.date_from != null && d.date_from.Value > dateToCompare)
            {
                return null;
            }

            if (d.date_to != null && d.date_to < dateToCompare)
            {
                return null;
            }

            try
            {
                if (d.time_from != null && d.time_from != "")
                {
                    TimeSpan fromTime = TimeSpan.Parse(d.time_from);
                    if (fromTime >= when.TimeOfDay)
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }

            try
            {
                if (d.time_to != null && d.time_to != "")
                {
                    TimeSpan toTime = TimeSpan.Parse(d.time_to);
                    if (toTime <= when.TimeOfDay)
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }

            switch (d.type)
            {
                case "PERCENT":
                    {
                        discount.Percent = d.percent ?? 0;
                        discount.Amount = price * ((d.percent ?? 0) * 0.01m);
                        return discount;
                    }
                case "AMOUNT":
                    {
                        discount.Amount = d.amount.Value;
                        return discount;
                    }
            }

            return null;
        }

        public bool IsValid(discount d)
        {
            DateTime when = DateTime.Now;
            bool exit = false;

            if (!(d.is_active.HasValue && d.is_active == 1))
                return false;
           
            switch (when.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    exit = d.monday == 0 ? true : false;
                    break;
                case DayOfWeek.Tuesday:
                    exit = d.tuesday == 0 ? true : false;
                    break;
                case DayOfWeek.Wednesday:
                    exit = d.wednesday == 0 ? true : false;
                    break;
                case DayOfWeek.Thursday:
                    exit = d.thursday == 0 ? true : false;
                    break;
                case DayOfWeek.Friday:
                    exit = d.friday == 0 ? true : false;
                    break;
                case DayOfWeek.Saturday:
                    exit = d.saturday == 0 ? true : false;
                    break;
                case DayOfWeek.Sunday:
                    exit = d.sunday == 0 ? true : false;
                    break;
            }

            if (exit)
            {
                return false;
            }

            DateTime dateToCompare = new DateTime(when.Year, when.Month, when.Day);

            if (d.date_from != null && d.date_from.Value > dateToCompare)
            {
                return false;
            }

            if (d.date_to != null && d.date_to < dateToCompare)
            {
                return false;
            }

            try
            {
                if (d.time_from != null && d.time_from != "")
                {
                    TimeSpan fromTime = TimeSpan.Parse(d.time_from);
                    if (fromTime >= when.TimeOfDay)
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }

            try
            {
                if (d.time_to != null && d.time_to != "")
                {
                    TimeSpan toTime = TimeSpan.Parse(d.time_to);
                    if (toTime <= when.TimeOfDay)
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public List<discount> GetDiscounts(int page = 1, int pageSize = 4)
        {
            //return entities.discounts.AsNoTracking().ToList();
            List<discount> listDiscounts = new List<discount>();

            try
            {
                var discounts = entities.discounts.AsNoTracking().Where(x => x.is_active == 1).OrderBy(x => x.id).Skip((page - 1) * pageSize).Take(pageSize).ToList();

                discounts.ForEach(x =>
                    {
                        if (IsValid(x))
                        {
                            listDiscounts.Add(x);
                        }
                    });
            }
            catch { }

            return listDiscounts;
        }

        public discount GetDiscount(int id)
        {
            return entities.discounts.SingleOrDefault(x => x.id == id);
        }

        public bool AddDiscount(discount discount)
        {
            try
            {
                entities.discounts.Add(discount);
                return Convert.ToBoolean(entities.SaveChanges());
            }
            catch
            {
                return false;
            }
        }

        public void Save(DataTable dt)
        {
            Delete(dt);

            foreach (DataRow dr in dt.Rows)
            {
                discount d1 = dr.ConvertToEntity<discount>();
                discount d2 = GetDiscount(d1.id);

                if (d2 != null)
                {
                    Extensions.CopyPropertyValues(d1, d2, new string[] { "id" });
                    entities.Entry(d2).State = EntityState.Modified;
                }
                else
                {
                    entities.discounts.Add(d1);
                }
            }
            entities.SaveChanges();
        }

        public void Delete(DataTable dt)
        {
            foreach (discount c in entities.discounts)
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
                    entities.discounts.Remove(c);
            }

            entities.SaveChanges();
        }

        public bool UpdateDiscount(discount discount)
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

        //Todo: Cascade Deletes
        public bool DeleteDiscount(int discountId)
        {
            var discount = entities.discounts.SingleOrDefault(x => x.id == discountId);

            if (discount != null)
            {
                try
                {
                    entities.discounts.Remove(discount);
                    entities.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
