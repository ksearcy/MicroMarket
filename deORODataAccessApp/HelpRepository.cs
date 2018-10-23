using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class HelpRepository
    {
        deOROEntities entities = new deOROEntities();

        public help GetHelp(string key)
        {
            return entities.helps.SingleOrDefault(x => x.key == key);
        }
    }
}
