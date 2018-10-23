using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class AccountBalanceHistoryRepository
    {
        deOROEntities entities = new deOROEntities();

        public void Add(string userPkId, decimal accountBalance, decimal amount, string description,string shoppingCartPkid = null)
        {
            accountbalancehistory history = new accountbalancehistory();
            history.pkid = Guid.NewGuid().ToString();
            history.userpkid = userPkId;
            history.shoppingcartpkid = shoppingCartPkid;
            history.account_balance = accountBalance;
            history.amount = amount;
            history.created_date_time = DateTime.Now;
            history.description = description;
            entities.accountbalancehistories.Add(history);

            entities.SaveChanges();
        }

        public List<accountbalancehistory> GetList(DateTime? lastSync = null)
        {
            return entities.accountbalancehistories.Where(x => x.created_date_time >= lastSync).ToList();
        }

        

    }
}
