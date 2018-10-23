using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.DataAccess
{
    public class LocationServiceRepository
    {
        deOROEntities entities = new deOROEntities();

        public bool AddLocationService(location_service locationService)
        {
            try
            {
                entities.location_service.Add(locationService);
                return Convert.ToBoolean(entities.SaveChanges());
            }
            catch
            {
                return false;
            }
        }

        public bool IsServicedStated()
        {
            return Convert.ToBoolean(entities.location_service.Single(x => x.id == 1).completed);
        }

        public bool SetServiceStarted(string userPkId)
        {

            var serivce = entities.location_service.SingleOrDefault(x => x.id == 1);

            if (serivce != null)
            {
                serivce.pkid = Guid.NewGuid().ToString();
                serivce.completed = 1;
                serivce.created_date_time = DateTime.Now;
                serivce.userpkid = userPkId;
                entities.SaveChanges();

                return true;
            }

            return false;
        }

        public bool SetServiceCompleted(string userPkId)
        {
            var serivce = entities.location_service.SingleOrDefault(x => x.id == 2);

            if (serivce != null)
            {
                serivce.pkid = Guid.NewGuid().ToString();
                serivce.completed = 1;
                serivce.created_date_time = DateTime.Now;
                serivce.userpkid = userPkId;
                entities.SaveChanges();

                return true;
            }

            return false;
        }

        public void ResetService()
        {
            foreach (var service in entities.location_service)
            {
                service.userpkid = null;
                service.created_date_time = null;
                service.completed = 0;
                service.pkid = null;
            }

            entities.SaveChanges();
        }

        public List<location_service> GetList(DateTime? lastSync = null)
        {
            return entities.location_service.Where(x=>x.created_date_time >=lastSync).ToList();
        }
    }
}
