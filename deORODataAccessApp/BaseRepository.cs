using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class BaseRepository
    {
        public DateTime? lastSync;

        public BaseRepository(DateTime lastSync)
        {
            this.lastSync = lastSync;
        }
    }
}
