using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp
{
    public class CreditUserRepository
    {
        deOROEntities entities = new deOROEntities();

        public void Save(DataTable dt)
        {
            Delete();

            foreach (DataRow dr in dt.Rows)
            {
                credit_user d1 = dr.ConvertToEntity<credit_user>();
                entities.credit_user.Add(d1);
            }

            entities.SaveChanges();
        }

        public credit GetCreditDetail(int id)
        {
            return entities.credits.SingleOrDefault(x => x.id == id);
        }

        public List<credit_user> GetAll(int id=0)
        {
            if (id == 0)
            {
                return entities.credit_user.ToList();
            }
            else
            {
                return entities.credit_user.Where(x => x.creditid == id).ToList();
            }
        }

        public void Delete()
        {
            foreach (credit_user c in entities.credit_user)
            {
                entities.credit_user.Remove(c);
            }

            entities.SaveChanges();
        }
    }
}
