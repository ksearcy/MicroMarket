using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
{
    public class ManufacturerRepository
    {
        deOROEntities entities = new deOROEntities();

        public List<manufacturer> GetManufacturers()
        {
            //return entities.manufacturers.AsNoTracking().ToList();

            return entities.manufacturers.AsNoTracking().OrderBy(x => x.name).ToList();
        }
    }
}
