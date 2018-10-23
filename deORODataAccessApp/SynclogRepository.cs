using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class SynclogRepository
    {
        deOROEntities entities = new deOROEntities();

        public void AddSynclog(synclog syncLog)
        {
            entities.synclogs.Add(syncLog);
        }

        public void Save()
        {
            entities.SaveChanges();
        }

        public List<synclog> GetLogs()
        {
            return entities.synclogs.AsNoTracking().OrderByDescending(x => x.createddatetime).ToList();
        }

    }
}
