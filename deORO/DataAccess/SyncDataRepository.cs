using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.DataAccess
{
    public class SyncDataRepository
    {
        deOROEntities entities = new deOROEntities();

        public DateTime? GetLastSuccessfulUpload()
        {
            var sync = entities.sync_data.AsNoTracking().Where(x => x.status == "Success" && x.type == "Upload").SingleOrDefault();

            if (sync != null)
                return sync.date_time;
            else
                return null;
        }

        public DateTime? GetLastSuccessfulDownload()
        {
            var sync = entities.sync_data.AsNoTracking().Where(x => x.status == "Success" && x.type == "Download").SingleOrDefault();

            if (sync != null)
                return sync.date_time;
            else
                return null;
        }

        public void UpdateUpload(DateTime dateTime, string status = null)
        {
            var sync = entities.sync_data.Where(x => x.type == "Upload").SingleOrDefault();

            if (sync != null)
            {
                sync.date_time = dateTime;
                if(status!=null)
                    sync.status = status;

                entities.SaveChanges();
            }
        }

        public void UpdateDownload(DateTime dateTime, string status = null)
        {
            var sync = entities.sync_data.Where(x => x.type == "Download").SingleOrDefault();

            if (sync != null)
            {
                sync.date_time = dateTime;
                if (status != null)
                    sync.status = status;

                entities.SaveChanges();
            }
        }
    }
}
