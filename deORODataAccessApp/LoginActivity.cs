using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp
{
    public class LoginActivity
    {
        deOROEntities entities = new deOROEntities();

        public bool AddLoginActivity(login_activity loginActivity)
        {
            try
            {
                entities.login_activity.Add(loginActivity);
                return Convert.ToBoolean(entities.SaveChanges());
            }
            catch
            {
                return false;
            }
        }
    }
}
