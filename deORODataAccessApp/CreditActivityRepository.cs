using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORODataAccessApp.Models;

namespace deORODataAccessApp
{
    public class CreditActivityRepository
    {
        deOROEntities entities = new deOROEntities();

        public void Add(credit_activity activity)
        {
            if (entities.credit_activity.Where(x => x.expiry_date == activity.expiry_date && x.amount == activity.amount && x.userpkid == activity.userpkid && x.creditid == activity.creditid).Count() == 0)
                entities.credit_activity.Add(activity);
        }

        public void Edit(credit_activity activity)
        {
            entities.Entry(activity).State = System.Data.EntityState.Modified;
        }

        public credit_activity GetCreditActivity(int id)
        {
            return entities.credit_activity.Where(x=>x.id == id).SingleOrDefault();
        }

        public List<CreditActivity> GetAll(string userpkid)
        {
            DateTime date = DateTime.Now.Date;

            var credits = (from c1 in entities.credit_activity
                           from c2 in entities.credits
                           where c1.creditid == c2.id && c1.userpkid == userpkid && c1.expiry_date >= date 
                           && c1.credit_claimed == null && c2.is_active == 1
                           select new CreditActivity
                           {
                               Selected = false,
                               Amount = c2.amount.Value,
                               Description = c2.description,
                               Expiry = c1.expiry_date.Value,
                               Id = c1.id

                           }).ToList();

            return credits;
        }

        public List<credit_activity> GetList(DateTime? lastSync = null)
        {
            return entities.credit_activity.ToList();
        }

        public void Save()
        {
            entities.SaveChanges();
        }
    }
}
