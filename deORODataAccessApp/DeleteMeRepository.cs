using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class DeleteMeRepository
    {
        deOROEntities entities = new deOROEntities();

        public bool Add(string firstName, string lastName, string email, string address, string city, string state, string zip,
                        string phone, decimal amountToRefund, string userPkId, int createdId)
        {
            try
            {
                UserRepository userRepo = new UserRepository();
                userRepo.DeleteUser(userPkId);

                users_deleted deleted = new users_deleted();
                deleted.pkid = Guid.NewGuid().ToString();
                deleted.first_name = firstName;
                deleted.last_name = lastName;
                deleted.email = email;
                deleted.address = address;
                deleted.city = city;
                deleted.state = state;
                deleted.zip = zip;
                deleted.phone = phone;
                deleted.created_date_time = DateTime.Now;
                deleted.amount_to_refund = amountToRefund;
                deleted.userpkid = userPkId;
                deleted.created_by_id = createdId;

                entities.users_deleted.Add(deleted);
                return Convert.ToBoolean(entities.SaveChanges());

            }
            catch
            {
                return false;
            }
        }

        public List<users_deleted> GetList(DateTime? lastSync = null)
        {
            return entities.users_deleted.Where(x => x.created_date_time >= lastSync).ToList();
        }
    }
}
