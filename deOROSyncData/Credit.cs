using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORODataAccessApp;

namespace deOROSyncData
{
    public class Credit
    {
        public void ProcessCredits()
        {

            CreditRepository repo1 = new CreditRepository();
            CreditUserRepository repo2 = new CreditUserRepository();

            var credits = repo1.GetAll();

            foreach (var c in credits)
            {
                if (c.effective_date == null)
                {
                    c.effective_date = DateTime.Now.AddDays(-30);
                }

                if ((c.type == "Once" && c.effective_date.Value.Date.ToShortDateString() == DateTime.Today.Date.ToShortDateString())
                    ||  (c.type == "Reccuring" && c.effective_date.Value.Date.AddDays(Convert.ToDouble(c.interval)) >= DateTime.Now.Date && (c.end_date == null || c.end_date.Value.Date >= DateTime.Now.Date))
                    || (c.type == "Once" && (c.expiry != null || c.end_date != null))
                    )
                {
                    var users = repo2.GetAll(c.id);
                    if (users.Count > 0)
                    {
                        CreditActivityRepository repo3 = new CreditActivityRepository();

                        foreach (var user in users)
                        {
                            credit_activity actvity = new credit_activity();
                            actvity.pkid = Guid.NewGuid().ToString();
                            actvity.userpkid = user.userpkid;
                            actvity.creditid = c.id;
                            actvity.amount = c.amount;
                            if (c.expiry != null || c.end_date != null)
                            {
                                if (c.end_date != null)
                                {
                                    actvity.expiry_date = c.end_date;
                                }
                                else if (c.expiry != null)
                                {
                                    actvity.expiry_date = DateTime.Now.Date.AddDays(Convert.ToDouble(c.expiry));
                                }
                            }
                            else {

                                actvity.expiry_date = c.effective_date.Value.AddDays(1);
                            }
                            
                            repo3.Add(actvity);
                        }

                        repo3.Save();
                    }
                }

            }

        }
    }
}
