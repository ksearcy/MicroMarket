using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class SyncDataRepository
    {
        deOROEntities entities = new deOROEntities();

        public sync_data GetLastUpload()
        {
            var sync = entities.sync_data.AsNoTracking().Where(x=>x.type == "Upload").SingleOrDefault();

            if (sync != null)
                return sync;
            else
                return null;
        }

        public sync_data GetLastDownload()
        {
            var sync = entities.sync_data.AsNoTracking().Where(x=>x.type == "Download").SingleOrDefault();

            if (sync != null)
                return sync;
            else
                return null;
        }

        public void UpdateUpload(DateTime dateTime, string status = null)
        {
            var sync = entities.sync_data.Where(x => x.type == "Upload").SingleOrDefault();

            if (sync != null)
            {
                if (status != null)
                    sync.status = status;

                if (status != null && status != "Failed")
                {
                    sync.date_time = dateTime;
                }

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
