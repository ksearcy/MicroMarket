using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deORODataAccessApp.DataAccess
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

        public bool IsServiceCompleted()
        {
            return Convert.ToBoolean(entities.location_service.Single(x => x.id == 2).completed);
        }

        public bool SetServiceStarted(string userPkId)
        {

            var service1 = entities.location_service.SingleOrDefault(x => x.id == 1);

            if (service1 != null)
            {
                service1.pkid = Guid.NewGuid().ToString();
                service1.completed = 1;
                service1.created_date_time = DateTime.Now;
                service1.userpkid = userPkId;

                entities.Entry(service1).State = System.Data.EntityState.Modified;                    
            }

            var service2 = entities.location_service.SingleOrDefault(x => x.id == 2);

            if (service2 != null)
            {
                service2.completed = 0;
                entities.Entry(service2).State = System.Data.EntityState.Modified;
            }

            return Convert.ToBoolean(entities.SaveChanges());
            
        }

        public bool SetServiceCompleted(string userPkId)
        {
            var service1 = entities.location_service.SingleOrDefault(x => x.id == 2);

            if (service1 != null)
            {
                service1.pkid = Guid.NewGuid().ToString();
                service1.completed = 1;
                service1.created_date_time = DateTime.Now;
                service1.userpkid = userPkId;

                entities.Entry(service1).State = System.Data.EntityState.Modified;                    
            }

            var service2 = entities.location_service.SingleOrDefault(x => x.id == 1);

            if (service2 != null)
            {
                service2.completed = 0;
                entities.Entry(service2).State = System.Data.EntityState.Modified;                    
            }

            return Convert.ToBoolean(entities.SaveChanges());
        }

        public void ResetService()
        {
            ItemRepository repo1 = new ItemRepository();
            ItemSnapshotRepository repo2 = new ItemSnapshotRepository();

            var snapshotItems = repo2.GetAll(DateTime.Now);

            foreach (var snapshotItem in snapshotItems)
            {
                var item = repo1.GetItem(snapshotItem.itemid.Value);

                if (item != null)
                {
                    item.manufacturerid = snapshotItem.manufacturerid;
                    item.categoryid = snapshotItem.categoryid;
                    item.discountid = snapshotItem.discountid;
                    item.upc = snapshotItem.upc;
                    item.name = snapshotItem.name;
                    item.barcode = snapshotItem.barcode;
                    item.description = snapshotItem.description;
                    item.quantity = snapshotItem.quantity;
                    item.stale = snapshotItem.stale;
                    item.@short = snapshotItem.@short;
                    item.count = snapshotItem.count;
                    item.unitcost = snapshotItem.unitcost;
                    item.avgshelflife = snapshotItem.avgshelflife;
                    item.pickorder = snapshotItem.pickorder;
                    item.is_taxable = snapshotItem.is_taxable;
                    item.price = snapshotItem.price;
                    item.tax = snapshotItem.tax;
                    item.price_tax_included = snapshotItem.price_tax_included;
                    item.tax_percent = snapshotItem.tax_percent;
                    item.crv = snapshotItem.crv;
                    item.has_barcode = snapshotItem.has_barcode;
                    item.image = snapshotItem.image;
                    item.itemgroupid = snapshotItem.itemgroupid;

                    entities.Entry(item).State = System.Data.EntityState.Modified;                    
                }
            }

            foreach (var service in entities.location_service)
            {
                service.userpkid = null;
                service.created_date_time = null;
                service.completed = 0;
                service.pkid = null;

                entities.Entry(service).State = System.Data.EntityState.Modified; 
            }

            entities.SaveChanges();

            //Delete snapshot data
            repo2.Delete(DateTime.Now);
        }

        public List<location_service> GetList(DateTime? lastSync = null)
        {
            return entities.location_service.Where(x=>x.created_date_time >=lastSync).ToList();
        }
    }
}
