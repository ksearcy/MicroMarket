using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class EventLogRepository
    {
        deOROEntities entities = new deOROEntities();

        public bool AddEventLog(event_log eventLog)
        {
            try
            {
                entities.event_log.Add(eventLog);
                return Convert.ToBoolean(entities.SaveChanges());
            }
            catch
            {
                return false;
            }
        }
        public List<event_log> GetList(string Pkid)
        {
            return entities.event_log.Where(x => x.pkid == Pkid).ToList();
        }

        public List<event_log> GetList(DateTime? lastSync = null)
        {
            return entities.event_log.Where(x => x.created_date_time >= lastSync).ToList();
        }
    }
}
