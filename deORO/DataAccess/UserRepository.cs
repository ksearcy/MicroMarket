using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.DataAccess
{
    public class UserRepository
    {
        deOROEntities entities = new deOROEntities();

        public List<user> GetList(DateTime? lastSync = null)
        {
            return entities.users.ToList();
        }

        public user GetUser(int userId)
        {
            return entities.users.SingleOrDefault(x => x.id == userId);
        }

        public bool AddUser(user user)
        {
            try
            {
                entities.users.Add(user);
                return Convert.ToBoolean(entities.SaveChanges());
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateUser(user user)
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

        public bool DeleteUser(int userId)
        {
            var user = entities.users.SingleOrDefault(x => x.id == userId);

            if (user != null)
            {
                try
                {
                    user.is_active = 0;
                    user.account_balance = 0.0m;
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

        public bool DeleteUser(string userPkId)
        {
            var user = entities.users.SingleOrDefault(x => x.pkid == userPkId);

            if (user != null)
            {
                try
                {
                    user.is_active = 0;
                    user.account_balance = 0.0m;
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
