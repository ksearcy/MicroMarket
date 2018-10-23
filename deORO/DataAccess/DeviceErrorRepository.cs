using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.DataAccess
{
    public class DeviceErrorRepository
    {
        deOROEntities entities = new deOROEntities();

        public bool AddDeviceError(device_error deviceError)
        {
            try
            {
                entities.device_error.Add(deviceError);
                return Convert.ToBoolean(entities.SaveChanges());
            }
            catch
            {
                return false;
            }
        }

        public List<device_error> GetList(DateTime? lastSync = null)
        {
            return entities.device_error.Where(x => x.created_date_time >= lastSync).ToList();
        }
    }
}
