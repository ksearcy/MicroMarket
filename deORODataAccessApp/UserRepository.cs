using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class UserRepository
    {
        deOROEntities entities = new deOROEntities();

        public List<user> GetList(DateTime? lastSync = null)
        {
            return entities.users.ToList();
        }

        public List<user> GetList(string pkid)
        {
            return entities.users.Where(x => x.pkid == pkid).ToList();
        }


        public user GetUser(int userId)
        {
            return entities.users.AsNoTracking().SingleOrDefault(x => x.id == userId);
        }

        public user GetUserByUsername(string username)
        {
            return entities.users.AsNoTracking().SingleOrDefault(x => x.username == username);
        }

        public user GetUserByEmail(string email)
        {
            return entities.users.AsNoTracking().SingleOrDefault(x => x.email == email);
        }

        public user GetLastUser()
        {

            return entities.users.OrderByDescending(x => x.id).FirstOrDefault();
        }

        public bool EmailExists(string email)
        {
            return Convert.ToBoolean(entities.users.Where(x => x.email == email).Count());
        }

        public user GetUser(string pkid)
        {
            return entities.users.AsNoTracking().SingleOrDefault(x => x.pkid == pkid);
        }

        public void Save(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                user serverCopy = row.ConvertToEntity<user>();
                user localCopy = GetUser(serverCopy.pkid);

                if (localCopy != null)
                {
                    //if ((serverCopy.last_updated_on > localCopy.last_updated_on) || (localCopy.last_updated_on == null))
                    //{
                    //    user_snapshot snapshot = new user_snapshot();
                    //    Extensions.CopyPropertyValues(localCopy, snapshot, new string[] { "id" });
                    //    entities.user_snapshot.Add(snapshot);

                    //    Extensions.CopyPropertyValues(serverCopy, localCopy, new string[] { "id" });
                    //    entities.Entry(localCopy).State = EntityState.Modified;
                    //}

                    if ((serverCopy.sync_vector > localCopy.sync_vector) || (localCopy.sync_vector == null))
                    {
                        user_snapshot snapshot = new user_snapshot();
                        Extensions.CopyPropertyValues(localCopy, snapshot, new string[] { "id" });
                        entities.user_snapshot.Add(snapshot);

                        Extensions.CopyPropertyValues(serverCopy, localCopy, new string[] { "id" });
                        entities.Entry(localCopy).State = EntityState.Modified;
                    }

                }
                else
                {
                    entities.users.Add(serverCopy);
                }
            }

            entities.SaveChanges();
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

        public bool UpdateUser(user user, bool useCurrentDateForLastUpdatedDate = true)
        {
            try
            {
                if (useCurrentDateForLastUpdatedDate)
                    user.last_updated_on = DateTime.Now;              

                entities.Entry<user>(user).State = EntityState.Modified;
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
                    user.lastaccountbalancechangeddate = DateTime.Now;
                    user.lastaccountbalancechangeddescription = "User Deleted";
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
                    user.lastaccountbalancechangeddate = DateTime.Now;
                    user.lastaccountbalancechangeddescription = "User Deleted";
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
