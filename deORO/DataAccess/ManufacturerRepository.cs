using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORO.DataAccess
{
    public class ManufacturerRepository
    {
        deOROEntities entities = new deOROEntities();

        public List<manufacturer> GetManufacturers()
        {
            return entities.manufacturers.AsNoTracking().ToList();
        }
    }
}
