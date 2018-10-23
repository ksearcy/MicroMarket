using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORODataAccessApp.Models;

namespace deORODataAccessApp.DataAccess
{
    public class ComboDiscountRepository
    {
        deOROEntities entities = new deOROEntities();

        public int GetDiscountId(int itemid)
        {
            ComboDiscountDetailRepository repo2 = new ComboDiscountDetailRepository();

            foreach (var c in GetAll())
            {
                var details = repo2.GetAll(c.id);

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
                else
                {
                    //Combo Discount has only 1 Category
                    continue;
                }
            }

            return 0;
        }

        public bool IsComboDiscountApplicable(int discountId, List<ShoppingCartItem> list)
        {
            ComboDiscountDetailRepository repo = new ComboDiscountDetailRepository();

            combo_discount discount = GetDiscount(discountId);
            List<ComboDiscountDetail> details = new List<ComboDiscountDetail>();

            details = (from o in repo.GetAll(discountId)
                       select new ComboDiscountDetail
                       {
                           entityId = o.entityid,
                       }).ToList();

            if (discount.category == "Item")
            {
                try
                {
                    for (int i = 0; i <= details.Count; i++)
                    {
                        var detail = details[i];

                        foreach (var item in list)
                        {
                            if (item.Id == detail.entityId)
                            {
                                details.Remove(detail);
                                i--;
                            }
                        }

                    }
                }
                catch
                {
                }

                if (details.Count == 0)
                {
                    return true;
                }
            }
            else if (discount.category == "Item Category")
            {

                try
                {
                    for (int i = 0; i <= details.Count; i++)
                    {
                        var detail = details[i];

                        foreach (var item in list)
                        {
                            if (item.Categoryid == detail.entityId)
                            {
                                details.Remove(detail);
                                i--;
                                break;
                            }
                        }

                    }
                }
                catch
                {
                }

                if (details.Count == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public Discount GetDiscount(DateTime when, int discountId, decimal price)
        {
            var d = entities.combo_discount.AsNoTracking().SingleOrDefault(x => x.id == discountId && x.is_active == 1);

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
                        discount.Percent = d.percent.Value;
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

        public bool IsValid(combo_discount d)
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

        public void Save(DataTable dt)
        {
            Delete(dt);

            foreach (DataRow dr in dt.Rows)
            {
                combo_discount d1 = dr.ConvertToEntity<combo_discount>();
                combo_discount d2 = GetDiscount(d1.id);

                if (d2 != null)
                {
                    Extensions.CopyPropertyValues(d1, d2, new string[] { "id" });
                    entities.Entry(d2).State = EntityState.Modified;
                }
                else
                {
                    entities.combo_discount.Add(d1);
                }
            }
            entities.SaveChanges();
        }

        public combo_discount GetDiscount(int id)
        {
            return entities.combo_discount.SingleOrDefault(x => x.id == id);
        }

        public List<combo_discount> GetAll()
        {
            return entities.combo_discount.OrderByDescending(x => x.category).ToList();
        }

        public int GetActiveDiscountsCount()
        {
            int count = 0;
            entities.combo_discount.ToList().ForEach(x => count = count + Convert.ToInt32(IsValid(x)));
            return count;
        }

        public List<ComboDiscount> GetActiveDiscounts(int page = 1, int pageSize = 1)
        {
            var discounts = entities.combo_discount.OrderBy(x => x.id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            List<ComboDiscount> list = new List<ComboDiscount>();

            foreach (var d in discounts)
            {
                if (IsValid(d))
                {
                    string detail = "";
                    if (d.category == "Item Category")
                    {
                        var categories = (from a in entities.combo_discount_detail
                                          from c in entities.categories
                                          where a.entityid == c.id
                                          select c.name).ToList();

                        categories.ForEach(x => detail += x + "\r\n");
                    }
                    else if (d.category == "Item")
                    {
                        var items = (from a in entities.combo_discount_detail
                                     from i in entities.items
                                     where a.entityid == i.id
                                     select i.name).ToList();

                        items.ForEach(x => detail += x + "\r\n");
                    }

                    list.Add(new ComboDiscount()
                    {
                         title = d.type == "PERCENT" ? d.percent + " %" : "$ " + d.amount,
                         description = d.description,
                         detail = detail
                    });
                    
                }
            }

            return list;
        }

        public void Delete(DataTable dt)
        {
            foreach (combo_discount c in entities.combo_discount)
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
                    entities.combo_discount.Remove(c);
            }

            entities.SaveChanges();
        }
    }

    struct ComboDiscountDetail
    {
        public int entityId { get; set; }
        public bool applied { get; set; }
    }
}
