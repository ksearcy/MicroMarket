using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using deORODataAccess.DTO;

namespace deORODataAccess
{
    public class AlertSubscriptionRepository : GenericRepository<deORO_MasterEntities, alert_subscription>
    {
        public AlertSubscriptionRepository()
        {

        }

        public AlertSubscriptionRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }
    }


    public class AlertRepository : GenericRepository<deORO_MasterEntities, alert>
    {
        public AlertRepository()
        {

        }

        public AlertRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public AlertDTO FindBy(int id)
        {
            var alert = (from a in Context.alerts
                         from m in Context.metrics
                         where a.metricid == m.id && a.id == id
                         select new AlertDTO
                         {
                             id = a.id,
                             name = a.name,
                             metricid = m.id,
                             frequeny = a.frequency,
                             period = a.period,
                             status = a.status,
                             query = m.query,
                             date_range = m.date_range,
                             last_run_date = a.last_run_date,
                             next_run_date = a.next_run_date,
                             report_type = a.report_type

                         }).SingleOrDefault();

            return alert;
        }

        public List<AlertDTO> GetAll(int customerid, int status = 1)
        {
            var alerts = (from a in Context.alerts
                          from m in Context.metrics
                          from s in Context.alert_subscription
                          where a.metricid == m.id && a.is_active == status && s.customerid == customerid
                          select new AlertDTO
                          {
                              id = a.id,
                              name = a.name,
                              metricid = m.id,
                              frequeny = a.frequency,
                              period = a.period,
                              status = a.status,
                              query = m.query,
                              date_range = m.date_range,
                              last_run_date = a.last_run_date,
                              next_run_date = a.next_run_date

                          }).ToList();

            return alerts;
        }
    }


    public class LocationSubsidyRepository : GenericRepository<deORO_MasterEntities, location_subsidy>
    {
        public LocationSubsidyRepository()
        {

        }

        public LocationSubsidyRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public void AddRemoveSubsidies(int locationid, int[] ids)
        {
            Delete(x => x.locationid == locationid);
            Save();

            if (ids != null)
            {
                foreach (int id in ids)
                {
                    location_subsidy d = new location_subsidy();
                    d.locationid = locationid;
                    d.subsidyid = id;
                    Context.location_subsidy.Add(d);
                }
            }

            Save();
        }
    }

    public class SubsidyRepository : GenericRepository<deORO_MasterEntities, subsidy>
    {
        public SubsidyRepository()
        {

        }

        public SubsidyRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }


        public void Edit(subsidy entity, int[] ids)
        {
            SubsidyDetailRepository repo = new SubsidyDetailRepository();
            repo.Delete(x => x.subsidyid == entity.id);
            repo.Save();

            if (ids != null)
            {
                foreach (int i in ids)
                {
                    subsidy_detail detail = new subsidy_detail();
                    detail.subsidyid = entity.id;
                    detail.entityid = i;
                    repo.Add(detail);
                }

                repo.Save();
            }

            base.Edit(entity);

        }


        public List<subsidy> GetAllByCustomerLocation()
        {
            var subsidies = (from d in Context.subsidies
                             from l in Context.location_subsidy
                             where d.id == l.subsidyid && l.locationid == locationId
                             select d);

            return subsidies.ToList();
        }
    }


    public class SubsidyDetailRepository : GenericRepository<deORO_MasterEntities, subsidy_detail>
    {
        public SubsidyDetailRepository()
        {

        }

        public SubsidyDetailRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public void Add(int subsidyid, int[] ids)
        {
            foreach (int i in ids)
            {
                subsidy_detail detail = new subsidy_detail();
                detail.subsidyid = subsidyid;
                detail.entityid = i;

                base.Add(detail);
            }
        }

        public List<subsidy_detail> GetAllByCustomerLocation()
        {
            var subsidyDetails = (from o in Context.subsidy_detail
                                  from d in Context.subsidies
                                  from l in Context.location_subsidy
                                  where d.id == l.subsidyid && l.locationid == locationId && o.subsidyid == d.id
                                  select o).ToList();

            return subsidyDetails;
        }
    }

    public class ComboDiscountRepository : GenericRepository<deORO_MasterEntities, combo_discount>
    {
        public ComboDiscountRepository()
        {

        }

        public ComboDiscountRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public List<combo_discount> GetAllByCustomerLocation()
        {
            var comboDiscounts = (from d in Context.combo_discount
                                  from l in Context.location_combo_discount
                                  where d.id == l.combodiscountid && l.locationid == locationId
                                  select d).ToList();

            return comboDiscounts;
        }


        public void Edit(combo_discount entity, int[] ids)
        {
            ComboDiscountDetailRepository repo = new ComboDiscountDetailRepository();
            repo.Delete(x => x.combodiscountid == entity.id);
            repo.Save();

            if (ids != null)
            {
                foreach (int i in ids)
                {
                    combo_discount_detail detail = new combo_discount_detail();
                    detail.combodiscountid = entity.id;
                    detail.entityid = i;
                    repo.Add(detail);
                }

                repo.Save();
            }

            base.Edit(entity);

        }
    }

    public class LocationCreditRepository : GenericRepository<deORO_MasterEntities, location_credit>
    {
        public LocationCreditRepository()
        {

        }

        public LocationCreditRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public List<location_credit> GetAllByCustomerLocation()
        {
            var list = (from o in Context.location_credit
                        from l in Context.locations
                        where l.id == o.locationid
                        select o).ToList();

            return list;
        }

        public List<DTO.LocationCreditDTO> GetAll(int id)
        {
            var credits = (from l in Context.locations
                           from c in Context.customers
                           from lc in Context.location_credit
                           where lc.customerid == c.id && lc.locationid == l.id && l.id == id
                           select new DTO.LocationCreditDTO
                           {
                               id = lc.id,
                               customerid = c.id,
                               customername = c.name,
                               locationid = l.id,
                               locationame = l.name,
                               is_active = (byte?)lc.is_active.Value ?? 0,
                               amount = (decimal?)lc.amount.Value ?? 0,
                               created_by = lc.created_by,
                               created_date_time = lc.created_date_time,
                               description = lc.description,
                               end_date = lc.end_date,
                               expiry = lc.expiry,
                               interval = lc.interval,
                               effective_date = lc.effective_date,
                               type = lc.type,

                           }).ToList();

            return credits;
        }

        public DTO.LocationCreditDTO GetSingleById(int id)
        {
            var credit = (from l in Context.locations
                          from c in Context.customers
                          from lc in Context.location_credit
                          where lc.customerid == c.id && lc.locationid == l.id && lc.id == id
                          select new DTO.LocationCreditDTO
                          {
                              id = lc.id,
                              customerid = c.id,
                              customername = c.name,
                              locationid = l.id,
                              locationame = l.name,
                              is_active = (byte?)lc.is_active.Value ?? 0,
                              amount = (decimal?)lc.amount.Value ?? 0,
                              created_by = lc.created_by,
                              created_date_time = lc.created_date_time,
                              description = lc.description,
                              end_date = lc.end_date,
                              expiry = lc.expiry,
                              interval = lc.interval,
                              effective_date = lc.effective_date,
                              type = lc.type,

                          }).SingleOrDefault();


            return credit;
        }
    }

    public class LocationCreditUserRepository : GenericRepository<deORO_MasterEntities, location_credit_user>
    {
        public LocationCreditUserRepository()
        {

        }

        public LocationCreditUserRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }


        public List<location_credit_user> GetAllByCustomerLocation()
        {
            var list = (from o in Context.location_credit
                        from c in Context.location_credit_user
                        from l in Context.locations
                        where l.id == o.locationid && c.creditid == o.id
                        select c).ToList();

            return list;
        }

        public void AddRemoveUsers(int creditid, string[] userids)
        {
            Delete(x => x.creditid == creditid);
            Save();

            if (userids != null)
            {
                foreach (string id in userids)
                {
                    location_credit_user d = new location_credit_user();
                    d.creditid = creditid;
                    d.userpkid = id;
                    Context.location_credit_user.Add(d);
                }
            }

            Save();
        }
    }

    public class ComboDiscountDetailRepository : GenericRepository<deORO_MasterEntities, combo_discount_detail>
    {
        public ComboDiscountDetailRepository()
        {

        }

        public ComboDiscountDetailRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public void Add(int discountid, int[] ids)
        {
            foreach (int i in ids)
            {
                combo_discount_detail detail = new combo_discount_detail();
                detail.combodiscountid = discountid;
                detail.entityid = i;

                base.Add(detail);
            }
        }

        public List<combo_discount_detail> GetAllByCustomerLocation()
        {
            var comboDetail = (from o in Context.combo_discount_detail
                               from d in Context.combo_discount
                               from l in Context.location_combo_discount
                               where d.id == l.combodiscountid && l.locationid == locationId
                               && o.combodiscountid == d.id
                               select o).ToList();

            return comboDetail;
        }
    }

    public class LocationComboDiscountRepository : GenericRepository<deORO_MasterEntities, location_combo_discount>
    {
        public LocationComboDiscountRepository()
        {

        }

        public LocationComboDiscountRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public void AddRemoveDiscounts(int locationid, int[] ids)
        {
            Delete(x => x.locationid == locationid);
            Save();

            if (ids != null)
            {
                foreach (int id in ids)
                {
                    location_combo_discount d = new location_combo_discount();
                    d.locationid = locationid;
                    d.combodiscountid = id;
                    Context.location_combo_discount.Add(d);
                }
            }

            Save();
        }

    }

    public class LocationServiceRepository : GenericRepository<deORO_MasterEntities, location_service>
    {
        public LocationServiceRepository()
        {

        }

        public LocationServiceRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public override void Save(DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {
                string pkid = dr["pkid"].ToString();

                if (Context.location_service.Where(x => x.pkid == pkid).SingleOrDefault() == null)
                {
                    var rec = dr.ConvertToEntity<location_service>();

                    Add(rec);
                    rec.locationid = locationId;
                    rec.customerid = customerId;
                }
            }

            try
            {
                Save();
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Success", "Success");
            }
            catch (Exception ex)
            {
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Failed", ex.Message);
            }

        }


        public List<DTO.LocationServiceDTO> GetLocationServices(int locationid, int costumerid)
        {
            var locationservices = (from l in Context.location_service
                             from c in Context.locations
                             from d in Context.customers
                                    where l.locationid == c.id && l.customerid == d.id && c.id == locationid && d.id == costumerid
                             select new DTO.LocationServiceDTO
                             {
                                 id = l.id,
                                 pkid = l.pkid,
                                 customerid = l.customerid,
                                 locationid = l.locationid,
                                 userpkid = l.userpkid,
                                 comments = l.comments,
                                 created_date_time = l.created_date_time
                             }).ToList();

            return locationservices;
        }


    }

    public class LocationCreditActivityRepository : GenericRepository<deORO_MasterEntities, location_credit_activity>
    {
        public LocationCreditActivityRepository()
        {

        }

        public LocationCreditActivityRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public List<DTO.LocationCreditActivityDTO> GetAll(int id)
        {
            var activity = (from c in Context.customers
                            from l in Context.locations
                            from u in Context.users
                            from a in Context.location_credit_activity
                            where a.customerid == c.id && a.locationid == l.id && a.userpkid == u.pkid && a.creditid == id
                            select new LocationCreditActivityDTO
                            {
                                amount = a.amount.Value,
                                credit_claimed = (byte?)a.credit_claimed ?? 0,
                                credit_claimed_date = a.credit_claimed_date,
                                customerid = c.id,
                                customername = c.name,
                                locationame = l.name,
                                locationid = l.id,
                                username = u.username,
                                barcode = u.barcode,
                                expiry_date = a.expiry_date,

                                id = a.id
                            }).ToList();

            return activity;
        }
    }

    public class CashCounterRepository : GenericRepository<deORO_MasterEntities, cash_counter>
    {
        public CashCounterRepository()
        {

        }

        public CashCounterRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public CashAccountabilityDTO GetCashAccountability(string pkid)
        {
            string sql = @"select
                            'Cash Collected' as source,
                            sum(CASE WHEN d.cash_type='Coin' THEN d.amount ELSE 0 END) as coins,
                            cast(sum(CASE WHEN d.amount=1 AND d.cash_type='Bill' THEN 1 ELSE 0 END) as int) as ones,
                            cast(sum(CASE WHEN d.amount=2  THEN 1 ELSE 0 END)as int) as twos,
                            cast(sum(CASE WHEN d.amount=5  THEN 1 ELSE 0 END)as int) as fives,
                            cast(sum(CASE WHEN d.amount=10 THEN 1 ELSE 0 END)as int) as tens,
                            cast(sum(CASE WHEN d.amount=20 THEN 1 ELSE 0 END)as int) as twenties,
                            cast(sum(CASE WHEN d.amount=50 THEN 1 ELSE 0 END)as int) as fifties,
                            cast(sum(CASE WHEN d.amount=100 THEN 1 ELSE 0 END)as int) as hundreds
                            from cash_counter d WHERE d.cashcollectionpkid = '{0}'";

//            string sql = @"select
//                            'Cash Collected' as source,
//                            (SELECT SUM([amount]) FROM [payment]  WHERE source like '%Coin%' AND routing='CashBox'AND locationid=(SELECT TOP 1 [locationid]  FROM [deORO_SantaClarita].[dbo].[cash_counter] where cashcollectionpkid='{0}') AND created_date_time > (SELECT TOP 1 DATEADD(ss,-3,[created_date_time])  FROM [deORO_SantaClarita].[dbo].[cash_counter] where cashcollectionpkid='{0}' order by created_date_time) AND created_date_time < (SELECT TOP 1 DATEADD(ss,3,[created_date_time])  FROM [deORO_SantaClarita].[dbo].[cash_counter] where cashcollectionpkid='{0}' order by created_date_time DESC)) as coins,
//                            cast(sum(CASE WHEN d.amount=1 AND d.cash_type='Bill' THEN 1 ELSE 0 END) as int) as ones,
//                            cast(sum(CASE WHEN d.amount=2  THEN 1 ELSE 0 END)as int) as twos,
//                            cast(sum(CASE WHEN d.amount=5  THEN 1 ELSE 0 END)as int) as fives,
//                            cast(sum(CASE WHEN d.amount=10 THEN 1 ELSE 0 END)as int) as tens,
//                            cast(sum(CASE WHEN d.amount=20 THEN 1 ELSE 0 END)as int) as twenties,
//                            cast(sum(CASE WHEN d.amount=50 THEN 1 ELSE 0 END)as int) as fifties,
//                            cast(sum(CASE WHEN d.amount=100 THEN 1 ELSE 0 END)as int) as hundreds
//                            from cash_counter d WHERE d.cashcollectionpkid = '{0}'";

            return Context.Database.SqlQuery<CashAccountabilityDTO>(string.Format(sql, pkid)).SingleOrDefault();
        }

        public override void Save(DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {
                string pkid = dr["pkid"].ToString();

                if (Context.cash_counter.Where(x => x.pkid == pkid).SingleOrDefault() == null)
                {
                    var rec = dr.ConvertToEntity<cash_counter>();

                    Add(rec);
                    rec.locationid = locationId;
                    rec.customerid = customerId;
                }
            }

            try
            {
                Save();
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Success", "Success");
            }
            catch (Exception ex)
            {
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Failed", ex.Message);
            }

        }
    }

    public class CashDispenseRepository : GenericRepository<deORO_MasterEntities, cash_dispense>
    {
        public CashDispenseRepository()
        {

        }

        public CashDispenseRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public override void Save(DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {
                string pkid = dr["pkid"].ToString();

                if (Context.cash_dispense.Where(x => x.pkid == pkid).SingleOrDefault() == null)
                {
                    var rec = dr.ConvertToEntity<cash_dispense>();

                    Add(rec);
                    rec.locationid = locationId;
                    rec.customerid = customerId;
                }
            }

            try
            {
                Save();
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Success", "Success");
            }
            catch (Exception ex)
            {
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Failed", ex.Message);
            }

        }
    }

    public class PaymentRepository : GenericRepository<deORO_MasterEntities, payment>
    {
        public PaymentRepository()
        {

        }

        public PaymentRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public List<payment> GetAllPayments(string pkid)
        {
            return Context.payments.Where(x => x.shoppingcartpkid == pkid).ToList();
        }

        public override void Save(DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {
                string pkid = dr["pkid"].ToString();

                if (Context.payments.Where(x => x.pkid == pkid).SingleOrDefault() == null)
                {
                    var rec = dr.ConvertToEntity<payment>();

                    Add(rec);
                    rec.locationid = locationId;
                    rec.customerid = customerId;
                }
            }

            try
            {
                Save();
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Success", "Success");
            }
            catch (Exception ex)
            {
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Failed", ex.Message);
            }

        }

    }


    public class PlanogramItemRepository : GenericRepository<deORO_MasterEntities, planogram_item>
    {
        public PlanogramItemRepository()
        {

        }

        public PlanogramItemRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public void AddRemoveItems(int planogramid, List<int> itemids)
        {
            Delete(x => x.planogramid == planogramid);
            Save();


            foreach (int itemid in itemids)
            {
                planogram_item item = new planogram_item();
                item.itemid = itemid;
                item.planogramid = planogramid;
                Add(item);
            }

            Save();
        }
    }

    public class PlanogramRepository : GenericRepository<deORO_MasterEntities, planogram>
    {

        public PlanogramRepository()
        {

        }

        public PlanogramRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public List<planogram> GetAllByCustomerLocation()
        {
            var planograms = (from o in Context.planograms
                              where o.locationid == locationId
                              select o).ToList();


            return planograms;
        }
    }

    public class CashCollectionRepository : GenericRepository<deORO_MasterEntities, cash_collection>
    {
        public CashCollectionRepository()
        {

        }

        public CashCollectionRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public new List<CashCollectionDTO> GetAll()
        {
            var collections = (from cc in Context.cash_collection
                               from l in Context.locations
                               from c in Context.customers
                               where cc.locationid == l.id && cc.customerid == c.id
                               select new CashCollectionDTO
                               {
                                   customerid = c.id,
                                   customername = c.name,
                                   locationid = l.id,
                                   locationname = l.name,
                                   dateTime = cc.created_date_time.Value,
                                   id = cc.id,
                                   pkid = cc.pkid
                               }).ToList();

            return collections;
        }


        public override void Save(DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {
                string pkid = dr["pkid"].ToString();

                if (Context.cash_collection.Where(x => x.pkid == pkid).SingleOrDefault() == null)
                {
                    var rec = dr.ConvertToEntity<cash_collection>();

                    Add(rec);
                    rec.locationid = locationId;
                    rec.customerid = customerId;
                }
            }

            try
            {
                Save();
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Success", "Success");
            }
            catch (Exception ex)
            {
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Failed", ex.Message);
            }

        }
    }

    public class CashStatusRepository : GenericRepository<deORO_MasterEntities, cash_status>
    {
        public CashStatusRepository()
        {

        }

        public CashStatusRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public override void Save(DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {
                string pkid = dr["pkid"].ToString();

                if (Context.cash_status.Where(x => x.pkid == pkid).SingleOrDefault() == null)
                {
                    var rec = dr.ConvertToEntity<cash_status>();

                    Add(rec);
                    rec.locationid = locationId;
                    rec.customerid = customerId;
                }
            }

            try
            {
                Save();
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Success", "Success");
            }
            catch (Exception ex)
            {
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Failed", ex.Message);
            }

        }
    }

    public class TransactionErrorRepository : GenericRepository<deORO_MasterEntities, transaction_error>
    {
        public TransactionErrorRepository()
        {

        }

        public TransactionErrorRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public override void Save(DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {
                string pkid = dr["pkid"].ToString();

                if (Context.transaction_error.Where(x => x.pkid == pkid).SingleOrDefault() == null)
                {
                    var rec = dr.ConvertToEntity<transaction_error>();

                    Add(rec);
                    rec.locationid = locationId;
                    rec.customerid = customerId;
                }
            }

            try
            {
                Save();
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Success", "Success");
            }
            catch (Exception ex)
            {
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Failed", ex.Message);
            }
        }



        public List<LocationDashboardErrorDTO> GetErrors(int locationid, DateTime fromDate, DateTime toDate)
        {
            if (locationid != 0)
            {
                var data = (from c in Context.transaction_error
                            where c.locationid == locationid && c.created_date_time >= fromDate && c.created_date_time < toDate
                            group c by new
                            {
                                c.source
                            } into g
                            select new LocationDashboardErrorDTO
                            {
                                source = g.Key.source,
                                count = g.Count(),
                            }).ToList();

                return data;
            }
            else
            {
                var data = (from c in Context.transaction_error
                            where c.created_date_time >= fromDate && c.created_date_time < toDate
                            group c by new
                            {
                                c.source
                            } into g
                            select new LocationDashboardErrorDTO
                            {
                                source = g.Key.source,
                                count = g.Count(),
                            }).ToList();

                return data;
            }
        }
    }
    public class EventLogRepository : GenericRepository<deORO_MasterEntities, event_log>
    {
        public EventLogRepository()
        {

        }

        public EventLogRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public override void Save(DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {
                string pkid = dr["pkid"].ToString();

                if (Context.event_log.Where(x => x.pkid == pkid).SingleOrDefault() == null)
                {
                    var rec = dr.ConvertToEntity<event_log>();

                    Add(rec);
                    rec.locationid = locationId;
                    rec.customerid = customerId;
                }
            }

            try
            {
                Save();
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Success", "Success");
            }
            catch (Exception ex)
            {
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Failed", ex.Message);
            }
        }
    }

    public class DeviceErrorRepository : GenericRepository<deORO_MasterEntities, device_error>
    {
        public DeviceErrorRepository()
        {

        }

        public DeviceErrorRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public override void Save(DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {
                string pkid = dr["pkid"].ToString();

                if (Context.device_error.Where(x => x.pkid == pkid).SingleOrDefault() == null)
                {
                    var rec = dr.ConvertToEntity<device_error>();

                    Add(rec);
                    rec.locationid = locationId;
                    rec.customerid = customerId;
                }
            }

            try
            {
                Save();
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Success", "Success");
            }
            catch (Exception ex)
            {
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Failed", ex.Message);
            }
        }

        public List<LocationDashboardErrorDTO> GetErrors(int locationid, DateTime fromDate, DateTime toDate)
        {
            if (locationid != 0)
            {
                var data = (from c in Context.device_error
                            where c.locationid == locationid && c.created_date_time >= fromDate && c.created_date_time < toDate
                            group c by new
                            {
                                c.source
                            } into g
                            select new LocationDashboardErrorDTO
                            {
                                source = g.Key.source,
                                count = g.Count(),
                            }).ToList();

                return data;
            }
            else
            {
                var data = (from c in Context.device_error
                            where c.created_date_time >= fromDate && c.created_date_time < toDate
                            group c by new
                            {
                                c.source
                            } into g
                            select new LocationDashboardErrorDTO
                            {
                                source = g.Key.source,
                                count = g.Count(),
                            }).ToList();

                return data;
            }
        }
    }

    public class WebUserRepository : GenericRepository<deORO_MasterEntities, webuser>
    {
        public WebUserRepository()
        {

        }
    }


    public class CashReconciliationRepository : GenericRepository<deORO_MasterEntities, cash_reconciliation>
    {

        public CashReconciliationRepository()
        {

        }

        public CashAccountabilityDTO GetCashAccountability(string pkid)
        {
            string sql = @"select
                            'Cash Reconcilled' as source,
                            ISNULL(c.coin_total,0) as coins,
                            ISNULL(c.c1_total,0) as ones,
                            ISNULL(c.c2_total,0) as twos,
                            ISNULL(c.c5_total,0) as fives,
                            ISNULL(c.c10_total,0) as tens,
                            ISNULL(c.c20_total,0) as twenties,
                            ISNULL(c.c50_total,0) as fifties,
                            ISNULL(c.c100_total,0) as hundreds
                            from cash_reconciliation c WHERE c.cashcollectionpkid = '{0}'";

            return Context.Database.SqlQuery<CashAccountabilityDTO>(string.Format(sql, pkid)).SingleOrDefault();
        }

    }


    public class DriverRepsoitory : GenericRepository<deORO_MasterEntities, driver>
    {

        public DriverRepsoitory()
        {

        }
    }

    public class ScheduleDetailItemRepository : GenericRepository<deORO_MasterEntities, schedule_detail_item>
    {
        public ScheduleDetailItemRepository()
        {

        }

        public ScheduleDetailItemRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        //public void AddItems(int id, int[] categories, int[] planograms)
        //public void AddItems(int id, int locationid, int[] categories, int[] planograms)
        public void AddItems(int id, int locationid, string categories, string planograms)
        {
            string sql = @" SELECT 0 as id, schedule_detail.id as scheduledetailid, location_item.itemid, 
                            location_item.quantity as quantity_at_schedule,
                            round((location_item.par - (CASE WHEN ISNULL(location_item.quantity, 0) 
                         < 0 THEN 0 ELSE location_item.quantity END)) / item.count,0) * item.count as quantity_to_refill, '' as tote, '' as status, null as over_under
                            from location_item
                            INNER JOIN item ON location_item.itemid = item.id
                            FULL OUTER JOIN category ON item.subcategoryid = category.id
                            INNER JOIN location ON location_item.locationid = location.id
                            INNER JOIN schedule_detail ON location_item.locationid = schedule_detail.locationid
                            WHERE par-(CASE WHEN ISNULL(location_item.quantity, 0) 
                         < 0 THEN 0 ELSE location_item.quantity END) >= item.count AND par > 0 AND (par - (CASE WHEN ISNULL(location_item.quantity, 0) 
                         < 0 THEN 0 ELSE location_item.quantity END)) >= ((cast(ISNULL(category.depletion_level, 1) as float) / 100) * par) AND scheduleid = {0} AND location.id = {1} {2}";


            string sql1 = "";
            if (categories != null && categories != "null")
            {
                sql1 += " AND item.subcategoryid NOT IN ({0}) ";
                //sql1 = string.Format(sql1, string.Join(",", categories));
                sql1 = string.Format(sql1, categories);
            }

            if (planograms != null && planograms != "null")
            {
                sql1 += " AND item.id NOT IN (SELECT itemid FROM planogram_item WHERE planogramid IN ({0})) ";
                //sql1 = string.Format(sql1, string.Join(",", planograms));
                sql1 = string.Format(sql1, planograms);
            }

            sql = string.Format(sql, id, locationid, sql1);
            var detailItems = Context.Database.SqlQuery<schedule_detail_item>(sql);

            detailItems.ToList().ForEach(x => Add(x));

            //Save();
        }

        public void UpdateOverUnder(int id, DataSet ds)
        {
            var scheduledItems = FindBy(x => x.scheduledetailid == id);

            foreach (var si in scheduledItems)
            {
                var rows = ds.Tables[0].Select("id = " + si.itemid);

                if (rows.Count() > 0)
                {
                    si.over_under = Convert.ToInt32(rows[0]["short"]);
                    Edit(si);
                }
            }

            Save();
        }

        public List<DTO.ScheduleDetailItemDTO> GetAllForPrint(int id)
        {
            //            string sql = @" select item.name, item.upc, item.barcode,schedule_detail_item.quantity_at_schedule,         
            //                            schedule_detail_item.quantity_to_refill 
            //                            from 
            //                            schedule_detail_item
            //                            inner join 
            //                            schedule_detail on schedule_detail.id = schedule_detail_item.scheduledetailid
            //                            inner join
            //                            location_item on schedule_detail_item.itemid = location_item.itemid
            //                            inner join
            //                            item on schedule_detail_item.itemid = item.id
            //                            where
            //                            schedule_detail_item.scheduledetailid = {0}";


            string sql = @"SELECT   dbo.schedule_detail_item.id, dbo.category.name as category, dbo.item.name, dbo.item.upc, dbo.item.barcode, 
                           dbo.schedule_detail_item.quantity_at_schedule, dbo.schedule_detail_item.quantity_to_refill,schedule_detail_item.tote,
                           schedule_detail_item.status,schedule_detail_item.over_under     
                           FROM dbo.schedule_detail_item INNER JOIN
                           dbo.location_item ON dbo.schedule_detail_item.itemid = dbo.location_item.itemid INNER JOIN
                           dbo.location ON dbo.location_item.locationid = dbo.location.id INNER JOIN
                           dbo.item ON dbo.location_item.itemid = dbo.item.id INNER JOIN
                           dbo.schedule_detail ON dbo.schedule_detail_item.scheduledetailid = dbo.schedule_detail.id AND 
                           dbo.location.id = dbo.schedule_detail.locationid LEFT OUTER JOIN
                           dbo.category ON dbo.item.categoryid = dbo.category.id
                           WHERE (dbo.schedule_detail.id = {0}) ORDER BY dbo.category.name ,item.pickorder asc";

            return Context.Database.SqlQuery<DTO.ScheduleDetailItemDTO>(String.Format(sql, id)).ToList();
        }
    }

    public class ScheduleDetailRepository : GenericRepository<deORO_MasterEntities, schedule_detail>
    {
        public ScheduleDetailRepository()
        {

        }

        public ScheduleDetailRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public int? GetActiveScheduleId()
        {
            DateTime shortDate = DateTime.Now.Date;
            var schedules = from sd in Context.schedule_detail
                            from s in Context.schedules
                            where sd.scheduleid == s.id && sd.status == "Scheduled" && sd.locationid == locationId
                            && sd.customerid == customerId
                            orderby sd.id ascending
                            select new
                            {
                                sd.id,
                                s.date
                            };

            foreach (var s in schedules)
            {
                if (s.date.Value.ToShortDateString() == DateTime.Now.Date.ToShortDateString())
                {
                    return s.id;
                }
            }

            return 0;
        }

        public int UpdateActiveScheduleStatus()
        {
            string shortDateString = DateTime.Now.Date.ToShortDateString();
            ScheduleDetailItemRepository itemRepo = new ScheduleDetailItemRepository();

            var schedules = (from sd in Context.schedule_detail
                             from s in Context.schedules
                             where sd.scheduleid == s.id && sd.status == "Scheduled" && sd.locationid == locationId
                             && sd.customerid == customerId
                             && DateTime.Now.Year == s.date.Value.Year && DateTime.Now.Month == s.date.Value.Month && DateTime.Now.Day == s.date.Value.Day
                             orderby sd.id descending
                             select new
                             {
                                 sd.id,
                                 s.date

                             }).ToList();


            if (schedules.Count > 0)
            {
                foreach (var s in schedules)
                {
                    //if (s.date.Value.ToShortDateString() == DateTime.Now.Date.ToShortDateString())
                    //{
                    var scheduleDetail = GetSingleById(x => x.id == s.id);
                    scheduleDetail.status = "Serviced";
                    scheduleDetail.status_updated_date_time = DateTime.Now;
                    Edit(scheduleDetail);
                    Save();

                    return s.id;

                    //}
                }
            }

            return 0;
        }


        public new List<DTO.ScheduleDetailDTO> GetAll()
        {
            //            string sql = @" SELECT customer.id as customerid,customer.name as customername, 
            //                            location.id as locationid ,location.name as locationname, 
            //                            0 as id, 0 as driverid , '' as drivername, '' as status, ISNULL(sub.count,0) as count, CAST(0 as BIT) AS selected
            //                            FROM customer
            //                            INNER JOIN location on location.customerid = customer.id
            //                            LEFT OUTER JOIN
            //                            (
            //                            SELECT customerid, locationid, count(*) as count from location_item
            //                            INNER JOIN item ON location_item.itemid = item.id
            //                            WHERE (par - quantity) >= count
            //                            group by customerid, locationid
            //                            ) sub
            //                            ON 
            //                            sub.customerid = location.customerid
            //                            AND
            //                            sub.locationid = location.id";

            string sql = @"SELECT customer.id as customerid,customer.name as customername, 
                                        location.id as locationid ,location.name as locationname, 
                                        0 as id, 0 as userid , '' as username, '' as status, ISNULL(sub.count,0) as count, CAST(0 as BIT) AS selected
                                        FROM customer
                                        INNER JOIN location on location.customerid = customer.id
                                        LEFT OUTER JOIN
                                        (
            								SELECT location_item.customerid, location_item.locationid, count(*) as count from location_item
            								INNER JOIN item ON location_item.itemid = item.id
            								FULL OUTER JOIN category ON item.subcategoryid = category.id
            								INNER JOIN location ON location_item.locationid = location.id
            								WHERE par-quantity != 0 AND par > 0 AND
                                            par - quantity >= ((cast(ISNULL(category.depletion_level, 1) as float) / 100) * par)
                                        group by location_item.customerid, location_item.locationid
                                        ) sub
                                        ON 
                                        sub.customerid = location.customerid
                                        AND
                                        sub.locationid = location.id";

            //wE DIDNT ADD THE END TO CLOSE 

//            string sql = @" SELECT customer.id as customerid,customer.name as customername, 
//                            location.id as locationid ,location.name as locationname, 
//                            0 as id, 0 as userid , '' as username, '' as status, ISNULL(sub.count,0) as count, CAST(0 as BIT) AS selected
//                            FROM customer
//                            INNER JOIN location on location.customerid = customer.id
//                            LEFT OUTER JOIN
//                            (
//								SELECT location_item.customerid, location_item.locationid, count(*) as count from location_item
//								INNER JOIN item ON location_item.itemid = item.id
//								FULL OUTER JOIN category ON item.subcategoryid = category.id
//								INNER JOIN location ON location_item.locationid = location.id
//								WHERE par-(CASE WHEN ISNULL(quantity, 0) < 0 THEN 0 ELSE quantity END) != 0 AND par > 0 AND
//                                (par - (CASE WHEN ISNULL(quantity, 0) < 0 THEN 0 ELSE quantity END)) >= ((cast(ISNULL(category.depletion_level, 1) as float) / 100) * par)
//                            group by location_item.customerid, location_item.locationid
//                            ) sub
//                            ON 
//                            sub.customerid = location.customerid
//                            AND
//                            sub.locationid = location.id";


            var scheduleDetails = Context.Database.SqlQuery<DTO.ScheduleDetailDTO>(sql);
            return scheduleDetails.ToList();
        }

        public int ApplyFilter(int locationid, string categories, string planograms)
        {
            string sql1 = " AND locationid = {0} ";
            sql1 = string.Format(sql1, locationid);

            if (categories != null)
            {
                sql1 += " AND item.subcategoryid NOT IN ({0}) ";
                //sql1 = string.Format(sql1, string.Join(",", categories));
                sql1 = string.Format(sql1, categories);
            }

            if (planograms != null)
            {
                sql1 += " AND item.id NOT IN (SELECT itemid FROM planogram_item WHERE planogramid IN ({0})) ";
                //sql1 = string.Format(sql1, string.Join(",", planograms));
                sql1 = string.Format(sql1, planograms);
            }

            string sql2 = @"SELECT count(*) as count from location_item
							INNER JOIN item ON location_item.itemid = item.id
							FULL OUTER JOIN category ON item.subcategoryid = category.id
							INNER JOIN location ON location_item.locationid = location.id
							WHERE par-quantity != 0 AND par > 0 AND
                            (par - ISNULL(quantity, 0)) >= ((cast(ISNULL(category.depletion_level, 1) as float) / 100) * par) {0}";


            return Convert.ToInt32(Context.Database.ExecuteScalar(string.Format(sql2, sql1)));
        }

        public List<DTO.ScheduleDetailDTO> GetAll(int id)
        {
            //            string sql = @" SELECT customer.id as customerid,customer.name as customername, 
            //                            location.id as locationid ,location.name as locationname, 
            //                            ISNULL(sub.id,0) AS id, ISNULL(sub.driverid,0) AS driverid, '' as drivername, sub.status, ISNULL(sub.count,0) as count,
            //                            CAST(ISNULL(sub.selected,0) as BIT) AS selected
            //                            FROM customer
            //                            INNER JOIN location on location.customerid = customer.id
            //                            LEFT OUTER JOIN
            //                            (
            //                            SELECT id,driverid,customerid,locationid,status,count, 1 as selected
            //                            FROM schedule_detail WHERE scheduleid = {0}
            //                            ) sub
            //                            ON 
            //                            sub.customerid = location.customerid
            //                            AND
            //                            sub.locationid = location.id";


            //            string sql = @"SELECT customer.id as customerid,customer.name as customername, 
            //                            location.id as locationid ,location.name as locationname, 
            //                            ISNULL(sub.id,0) AS id, ISNULL(sub.driverid,0) AS driverid, '' as username, sub.status, 
            //                            CAST(ISNULL(sub.selected,0) as BIT) as selected,
            //                            CASE CAST(ISNULL(sub.selected,0) as BIT) 
            //							WHEN '0' THEN (SELECT count(*) as count from location_item
            //								           INNER JOIN item ON location_item.itemid = item.id
            //								           INNER JOIN category ON item.categoryid = category.id
            //								           INNER JOIN location ON location_item.locationid = location.id AND sub.locationid = location.id
            //								           INNER JOIN customer ON location.customerid = customer.id AND sub.customerid = customer.id
            //                                           WHERE par-quantity != 0 AND 
            //								           (par - quantity) >= ((category.depletion_level / 100) * par))
            //							WHEN '1' THEN ISNULL(sub.count,0)
            //                            END AS count, sub.excluded_categories,sub.excluded_planograms
            //                            FROM customer
            //                            INNER JOIN location on location.customerid = customer.id
            //                            LEFT OUTER JOIN
            //                            (
            //                            SELECT id,driverid,customerid,locationid,status,count, 1 as selected, excluded_categories, excluded_planograms 
            //                            FROM schedule_detail WHERE scheduleid = {0}
            //                            ) sub
            //                            ON 
            //                            sub.customerid = location.customerid
            //                            AND
            //                            sub.locationid = location.id";


            string sql = @"select c.id as customerid, c.name as customername,l.id as locationid,l.name as locationname, 
                            ISNULL(s.id,0) AS id, ISNULL(s.driverid,0) AS driverid, '' as username, s.status, 
                            CASE 
	                            WHEN s.id is null THEN CAST(0 as BIT)
	                            ELSE CAST(1 as BIT) 
                            END as selected,
                            CASE 
	                            WHEN count is null 
		                            THEN 
			                            (
				                            SELECT count(*) as count from location_item
				                            INNER JOIN item ON location_item.itemid = item.id
				                            FULL OUTER JOIN category ON item.subcategoryid = category.id
				                            WHERE par-ISNULL(quantity, 0) != 0 AND (par - ISNULL(quantity, 0)) <= ((ISNULL(category.depletion_level, 1) / 100) * par)
				                            AND location_item.customerid = c.id AND location_item.locationid = l.id
			                            )
	                            ELSE count
                            END as count,
                            s.excluded_categories, s.excluded_planograms
                            from location l
                            left outer join
                            schedule_detail s
                            on l.id = s.locationid AND s.scheduleid = {0}
                            left outer join
                            customer c 
                            on c.id = l.customerid";

            return Context.Database.SqlQuery<DTO.ScheduleDetailDTO>(string.Format(sql, id)).ToList();
        }

        public List<DTO.ScheduleDetailDTO> GetAllForPrint(int id)
        {
            string sql = @" select customer.name as customername,location.name as locationname, schedule_detail.id, scheduleid, 
                            [user].username as username
                            from schedule_detail
                            inner join
                            location on location.id = schedule_detail.locationid
                            inner join	
                            customer on customer.id = schedule_detail.customerid
                            inner join
                            [user] on [user].pkid = schedule_detail.driverid
                            where scheduleid = {0} ";

            var scheduleDetails = Context.Database.SqlQuery<DTO.ScheduleDetailDTO>(string.Format(sql, id));
            return scheduleDetails.ToList();
        }

        public void AddRemoveItems(List<DTO.ScheduleDetailDTO> items)
        {
            ScheduleDetailItemRepository repo = new ScheduleDetailItemRepository();

            DeleteItemsNotSelected(items);

            foreach (DTO.ScheduleDetailDTO item in items)
            {
                if (item.selected)
                {
                    var detail = GetSingleById(x => x.id == item.id);

                    if (detail != null)
                    {
                        detail.driverid = item.driverid;

                        if (detail.status != item.status)
                        {
                            detail.status = item.status;
                            detail.status_updated_date_time = DateTime.Now;
                        }

                        detail.count = item.count;
                        detail.excluded_planograms = item.excluded_planograms.Replace(";", ",") ?? null;
                        detail.excluded_categories = item.excluded_categories.Replace(";", ",") ?? null;

                        Edit(detail, x => x.id == detail.id);
                    }
                    else
                    {
                        detail = new schedule_detail();
                        detail.scheduleid = item.scheduleid;
                        detail.driverid = item.driverid;
                        detail.customerid = item.customerid;
                        detail.locationid = item.locationid;
                        detail.status = item.status;
                        detail.status_updated_date_time = DateTime.Now;
                        detail.count = item.count;
                        detail.excluded_planograms = item.excluded_planograms.Replace(";", ",") ?? null;
                        detail.excluded_categories = item.excluded_categories.Replace(";", ",") ?? null;

                        Add(detail);
                    }
                }
            }

            Save();

        }

        public void DeleteItemsNotSelected(List<DTO.ScheduleDetailDTO> items)
        {
            List<schedule_detail> details = FindItemsNotSelected(items);

            foreach (schedule_detail d in details)
            {
                if (d != null)
                    Delete(d);
            }

            Save();
        }

        private List<schedule_detail> FindItemsNotSelected(List<DTO.ScheduleDetailDTO> items)
        {
            return Context.schedule_detail.Select(
                delegate(schedule_detail item)
                {
                    foreach (var v in items)
                    {
                        if (item.id == v.id && (!v.selected))
                            return item;
                    }
                    return null;
                }
             ).ToList();
        }
    }

    public class ScheduleRepository : GenericRepository<deORO_MasterEntities, schedule>
    {

        public ScheduleRepository()
        {

        }
    }

    public class LocationItemDeletedRepository : GenericRepository<deORO_MasterEntities, location_item_deleted>
    {
        public LocationItemDeletedRepository()
        {

        }

        public LocationItemDeletedRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }
    }

    public class LocationItemRepository : GenericRepository<deORO_MasterEntities, location_item>
    {
        public LocationItemRepository()
        {

        }

        public LocationItemRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public override void Save(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                int itemId = Convert.ToInt32(row["id"]);
                var locationItem = GetSingleById(x => x.itemid == itemId && x.locationid == locationId && x.customerid == customerId);

                if (locationItem != null)
                {
                    locationItem.quantity = row["quantity"] is DBNull ? 0 : Convert.ToInt32(row["quantity"]);
                    Edit(locationItem);
                }
            }

            try
            {
                Save();
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Success", "Success");
            }
            catch (Exception ex)
            {
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Failed", ex.Message);
            }
        }
        public DataTable GetAllLocationItemToExport()
        {
            var locitems = from li in Context.location_item
                        from c in Context.customers
                        .Where(x => x.id == li.customerid).DefaultIfEmpty()
                        from l in Context.locations
                        .Where(y => y.id == li.locationid).DefaultIfEmpty()
                        select new
                        {
                            id = li.id,
                            customerid = c.name,
                            locationid = l.name,
                            itemid = li.itemid,
                            discountid = (int?)li.discountid.Value ?? 0,
                            is_taxable = (int?)li.is_taxable.Value ?? 0,
                            price = li.price,
                            tax = (int?)li.tax.Value ?? 0,
                            price_tax_included = (decimal?)li.price_tax_included.Value ?? 0,
                            tax_percent = (decimal?)li.tax_percent.Value ?? 0,
                            crv = (decimal?)li.crv.Value ?? 0,
                            par = (int?)li.par.Value ?? 0,
                            quantity = (int?)li.quantity.Value ?? 0,
                            depletion_level = (int?)li.depletion_level.Value ?? 0,
                            created_date_time = li.created_date_time,
                            is_active = (byte?)li.is_active.Value ?? 0,
                        };

            return locitems.ToList().ToDataTable();
        }

        public DataTable GetLocationItemToExportById(int? location_id)
        {
            var locitems = from li in Context.location_item
                           .Where(x => x.locationid == location_id)
                           from c in Context.customers
                           .Where(x => x.id == li.customerid).DefaultIfEmpty()
                           from l in Context.locations
                           .Where(y => y.id == li.locationid).DefaultIfEmpty()
                           from i in Context.items
                          .Where(y => y.id == li.itemid).DefaultIfEmpty()
                           select new
                           {
                               id = li.id,
                               customerid = c.name,
                               locationid = l.name,
                               itemid = li.itemid,
                               itemname = i.name,
                               discountid = (int?)li.discountid.Value ?? 0,
                               is_taxable = (int?)li.is_taxable.Value ?? 0,
                               price = li.price,
                               tax = (int?)li.tax.Value ?? 0,
                               price_tax_included = (decimal?)li.price_tax_included.Value ?? 0,  
                               unitcost = (decimal?)i.unitcost ?? 0,
                               tax_percent = (decimal?)li.tax_percent.Value ?? 0,
                               crv = (decimal?)li.crv.Value ?? 0,
                               par = (int?)li.par.Value ?? 0,
                               quantity = (int?)li.quantity.Value ?? 0,
                               depletion_level = (int?)li.depletion_level.Value ?? 0,
                               created_date_time = li.created_date_time,
                               is_active = (byte?)li.is_active.Value ?? 0
                               
                              
                           };

            return locitems.ToList().ToDataTable();
        }
        
        public void UpdateParQuantity(int id, int par, int quantity)
        {
            var l = GetSingleById(x => x.id == id);

            if (l != null)
            {
                l.par = par;
                l.quantity = quantity;

                Edit(l);
                Save();
            }
        }

        public void UpdatePrice(int id, decimal price, decimal taxPercent, decimal crv)
        {
            var l = GetSingleById(x => x.id == id);

            if (l != null)
            {
                l.price = price;
                l.tax_percent = taxPercent;
                l.tax = (l.price ?? 0) * (taxPercent * 0.01m);
                l.crv = crv;
                l.price_tax_included = (l.price ?? 0) + (l.tax ?? 0) + (l.crv ?? 0);

                Edit(l);
                Save();
            }
        }

        public void AddRemoveItems(int locationId, int[] itemIds)
        {
            ItemRepository repo = new ItemRepository();
            this.customerId = Context.locations.Where(x => x.id == locationId).SingleOrDefault().customerid.Value;
            this.locationId = locationId;

            DateTime now = DateTime.Now;

            if (itemIds != null)
            {
                DeleteItemsNotIn(itemIds);

                foreach (int id in itemIds)
                {
                    var locationItem = Context.location_item.SingleOrDefault(x => x.itemid == id && x.locationid == locationId);
                    if (locationItem == null)
                    {
                        var item = repo.GetSingleById(x => x.id == id);
                        location_item litem = new location_item();
                        //Do not delete items from location table -- added by Kevin
                        if (item != null && litem.itemid != 0)
                        {
                            
                            litem.locationid = locationId;
                            litem.itemid = id;
                            litem.customerid = this.customerId;
                            litem.is_taxable = item.is_taxable;
                            litem.price = item.price;
                            litem.tax = item.tax;
                            litem.price_tax_included = item.price_tax_included;
                            litem.crv = item.crv;
                            litem.tax_percent = item.tax_percent;
                            litem.created_date_time = now;

                            Add(litem);
                        }
                    }
                }

                Save();
            }
            else
            {
                DeleteItemsNotIn(new int[] { });
            }
        }

        public void DeleteItemsNotIn(int[] itemIds)
        {
            try
            {

                List<location_item> existingItems = (from p in Context.location_item
                                                     where itemIds.Contains(p.itemid.Value)
                                                     select p).ToList();

                List<location_item> items = Context.location_item.Where(x => x.customerid == customerId && x.locationid == locationId).ToList().Except(existingItems).ToList();
                DateTime now = DateTime.Now;

                foreach (location_item item in items)
                {
                    if (item != null)
                    {
                        location_item_deleted deleted = new location_item_deleted();
                        deleted.customerid = item.customerid;
                        deleted.locationid = item.locationid;
                        deleted.itemid = item.itemid;
                        deleted.created_date_time = now;
                        Context.location_item_deleted.Add(deleted);

                        Delete(item);
                    }


                }

                Save();
            }
            catch { }

        }

        public int GetAdjustedPar(int locationid, int itemid)
        {
            string sql = @"select
                           case when sub.count > sub.adjusted THEN sub.par ELSE sub.adjusted END as adjusted
                           from
                           (
                                select 
                                o.itemid,i.name,o.barcode , l.service_interval, li.par, count(*) / l.service_interval as average_sales, c.depletion_level,i.count,
                                cast(round(((count(*) / l.service_interval) + cast(par * c.depletion_level as decimal(8,2))/100),0)as int) as adjusted
                                from shoppingcartdetail as o
                                inner join
                                location_item as li on li.itemid = o.itemid
                                inner join
                                location as l on l.id = o.locationid
                                inner join
                                item as i on i.id = li.itemid
                                inner join
                                category as c on i.categoryid = c.id
                                where
                                o.created_date_time between
                                dateadd(""d"",-30,getdate())
                                and 
                                getdate()
                                and 
                                l.id = {0} and o.itemid = {1} and li.locationid = l.id 
                                group by o.itemid,o.barcode, li.par, service_interval, c.depletion_level,i.name,i.count
                          ) sub";


            var par = Context.Database.ExecuteScalar(string.Format(sql, locationid, itemid));

            try
            {
                return Convert.ToInt32(par);
            }
            catch
            {
                return 0;
            }
        }

        public List<LocationItemDTO> GetAdjustedPar(int locationid)
        {
            string sql = @"select sub.itemid,
                           case when sub.count > sub.adjusted THEN sub.par ELSE sub.adjusted END as adjusted_par
                           from
                           (
                                select 
                                o.itemid,i.name,o.barcode , l.service_interval, li.par, count(*) / l.service_interval as average_sales, c.depletion_level,i.count,
                                cast(round(((count(*) / l.service_interval) + cast(par * c.depletion_level as decimal(8,2))/100),0)as int) as adjusted
                                from shoppingcartdetail as o
                                inner join
                                location_item as li on li.itemid = o.itemid
                                inner join
                                location as l on l.id = o.locationid
                                inner join
                                item as i on i.id = li.itemid
                                inner join
                                category as c on i.categoryid = c.id
                                where
                                o.created_date_time between
                                dateadd(""d"",-30,getdate())
                                and 
                                getdate()
                                and 
                                l.id = {0} and li.locationid = l.id 
                                group by o.itemid,o.barcode, li.par, service_interval, c.depletion_level,i.name,i.count
                          ) sub";


            var par = Context.Database.SqlQuery<LocationItemDTO>(string.Format(sql, locationid));

            return par.ToList();

        }

        public void UpdateRecommendedPar(int locationid, int[] itemids)
        {
            if (itemids == null || itemids.Length == 0)
            {
                LocationItemRepository repo = new LocationItemRepository();
                var items = GetAdjustedPar(locationid);

                foreach (var i in items)
                {
                    var item = repo.GetSingleById(x => x.itemid == i.itemid && x.locationid == locationid);

                    if (item != null && i.adjusted_par != null && item.par != i.adjusted_par)
                    {
                        item.par = i.adjusted_par;
                        repo.Edit(item);
                    }
                }

                repo.Save();
            }
            else
            {
                foreach (var id in itemids)
                {
                    var par = GetAdjustedPar(locationid, id);
                }
            }

        }

        public List<DTO.LocationItemDTO> GetAll(int locationid)
        {
            var locationItems = (from i in Context.location_item
                                 from d in Context.discounts.Where(x => x.id == i.discountid).DefaultIfEmpty()
                                 //from s in Context.subsidies.Where(y => y.id == i.subsidyid).DefaultIfEmpty()
                                 from l in Context.locations
                                 from c in Context.customers
                                 from t in Context.items
                                 where i.customerid == c.id && i.locationid == l.id && i.itemid == t.id & i.locationid == locationid
                                 let countCategory =
                                 (
                                    from e in Context.combo_discount
                                    from w in Context.combo_discount_detail.Where(y => y.combodiscountid == e.id && y.entityid == t.subcategoryid && e.category == "Item Category")
                                    select e
                                 ).Count()
                                 let countItem =
                                 (
                                    from e in Context.combo_discount
                                    from r in Context.combo_discount_detail.Where(y => y.combodiscountid == e.id && y.entityid == t.id && e.category == "Item")
                                    select e
                                 ).Count()

                                 select new DTO.LocationItemDTO
                                  {
                                      id = i.id == null ? 0 : i.id,
                                      customername = c.name,
                                      locationame = l.name,
                                      locationid = l.id,
                                      itemid = t.id,
                                      itemname = t.name,
                                      discountname = d.description,
                                      is_taxable = (byte?)i.is_taxable ?? 0,
                                      price = (decimal?)i.price ?? 0,
                                      price_tax_included = (decimal?)i.price_tax_included ?? 0,
                                      tax = (decimal?)i.tax ?? 0,
                                      tax_percent = (decimal?)i.tax_percent ?? 0,
                                      crv = (decimal?)i.crv ?? 0,
                                      depletion_level = (int?)i.depletion_level ?? 0,
                                      par = (int?)i.par ?? 0,
                                      quantity = (int?)i.quantity ?? 0,
                                      barcode = t.barcode,
                                      upc = t.upc,
                                      combodiscount = countCategory + countItem,
                                      unitcost = (decimal?)t.unitcost ?? 0
                                      //subsidyname = s.description

                                  }).ToList();

            return locationItems;
        }

        public List<DTO.LocationItemDTO> GetAllDiscountedItems(int locationid, int discountid)
        {
            var locationItems = (from i in Context.location_item
                                 from t in Context.items
                                 where i.locationid == locationid && i.discountid == discountid && i.itemid == t.id
                                 select new DTO.LocationItemDTO
                                 {
                                     itemid = t.id,
                                     itemname = t.name
                                 }).ToList();

            return locationItems;
        }

        public List<DTO.LocationItemDTO> GetAll(int locationid, int[] ids)
        {
            var locationItems = (from i in Context.location_item
                                 from t in Context.items
                                 where i.itemid == t.id && ids.Contains(i.itemid.Value) && i.locationid == locationid
                                 select new DTO.LocationItemDTO
                                 {
                                     id = i.id == null ? 0 : i.id,
                                     itemid = t.id,
                                     itemname = t.name,
                                     par = (int?)i.par ?? 0,
                                     quantity = (int?)i.quantity ?? 0,
                                     image = t.image,
                                     price_tax_included = (decimal?)i.price_tax_included ?? 0,
                                     locationid = (int?)i.locationid ?? 0,
                                     html = "",

                                 }).ToList();

            return locationItems;
        }

        public DTO.LocationItemDTO GetParQuantity(int id)
        {
            var item = (from i in Context.location_item
                        where i.id == id
                        select new DTO.LocationItemDTO
                        {
                            par = (int?)i.par ?? 0,
                            quantity = (int?)i.quantity ?? 0,
                            price = (decimal?)i.price ?? 0,
                            tax_percent = (decimal?)i.tax_percent ?? 0,
                            crv = (decimal?)i.crv ?? 0,
                            price_tax_included = (decimal?)i.price_tax_included ?? 0,
                            tax = (decimal?)i.tax ?? 0,

                        }).SingleOrDefault();

            return item;
        }

        public string GetSalesData(int locationid, int itemid, int days)
        {
            DateTime fromDate = DateTime.Now.AddDays(-(days + 1));
            DateTime ToDate = DateTime.Now.AddDays(-1);

            var data = (from s in Context.shoppingcartdetails
                        where s.created_date_time >= fromDate && s.created_date_time <= ToDate
                        && s.locationid == locationid && s.itemid == itemid
                        select s).GroupBy(x => SqlFunctions.DatePart("d", x.created_date_time)).Select(y => new
                        {
                            key = y.Key,
                            count = y.Count()
                        });

            int[] dayArray = new int[32];

            if (data.Count() > 0)
            {

                foreach (var v in data)
                {
                    dayArray[v.key.Value] = v.count;
                }
            }

            string values = "";

            //for (int i = fromDate.Day; i <= ToDate.Day; i++)
            for (int i = 30 - days; i <= 30; i++)
            {
                values += dayArray[i] + ",";
            }

            values = values.Substring(0, values.Length - 1);
            //values += " -->";

            return values;
            //}
            //else
            //{

            //}

        }

        public string GetSalesDataGroupedIntoWeekDay(int locationid, int itemid, int days)
        {
            DateTime fromDate = DateTime.Now.AddDays(-(days + 1));
            DateTime ToDate = DateTime.Now.AddDays(-1);

            var data = (from s in Context.shoppingcartdetails
                        where s.created_date_time >= fromDate && s.created_date_time <= ToDate
                        && s.locationid == locationid && s.itemid == itemid
                        select s).GroupBy(x => SqlFunctions.DatePart("weekday", x.created_date_time)).Select(y => new
                        {
                            key = y.Key,
                            count = y.Count()
                        });

            int[] weekArray = { 0, 0, 0, 0, 0, 0, 0 };

            if (data.Count() > 0)
            {

                foreach (var v in data)
                {
                    weekArray[v.key.Value - 1] = v.count;
                }

                string values = "<!-- ";

                for (int i = 0; i < weekArray.Length; i++)
                {
                    values += weekArray[i] + ",";
                }

                values = values.Substring(0, values.Length - 1);
                values += " -->";

                return values;
            }
            else
            {
                return "No Data";
            }

        }


        public void UpdateDiscount(int locationid, int discountid, int[] itemids)
        {

            if (itemids == null)
            {
                Context.location_item.ToList().ForEach(x =>
                {
                    x.discountid = null;
                    Edit(x);
                });

                Save();
                return;
            }

            foreach (int i in itemids)
            {
                var item = Context.location_item.Where(x => x.locationid == locationid && x.itemid == i).SingleOrDefault();

                if (item != null)
                {
                    item.discountid = discountid;
                    Edit(item);
                }
            }

            Save();
        }
    }

    public class DiscountRepository : GenericRepository<deORO_MasterEntities, discount>
    {
        public DiscountRepository()
        {

        }

    }

    public class MetricRepository : GenericRepository<deORO_MasterEntities, metric>
    {
        public MetricRepository()
        {

        }

    }

    public class WidgetRepository : GenericRepository<deORO_MasterEntities, widget>
    {
        public WidgetRepository()
        {

        }
    }

    public class DashboardRepository : GenericRepository<deORO_MasterEntities, dashboard>
    {
        public DashboardRepository()
        {

        }
    }

    public class CategoryRepository : GenericRepository<deORO_MasterEntities, category>
    {
        public CategoryRepository()
        {

        }

        public List<category> GetCategories()
        {
            return FindBy(x => x.parentid == null).ToList();
        }

        public List<category> GetSubCategories(int id = 0)
        {
            return FindBy(x => x.parentid != null).ToList();
        }

        public new List<DTO.CategoryDTO> GetAll()
        {
            var categories = from c1 in Context.categories
                             from c2 in Context.categories
                             .Where(x => x.id == c1.parentid)
                             .DefaultIfEmpty()
                             select new DTO.CategoryDTO
                             {
                                 id = c1.id,
                                 code = c1.code,
                                 name = c1.name,
                                 pick_order = (int?)c1.pick_order.Value ?? 0,
                                 description = c1.description,
                                 parentid = (int?)c2.id ?? 0,
                                 parentname = c2.name ?? "",
                                 depletion_level = (int?)c1.depletion_level ?? 0,
                                 image = c1.image
                             };

            return categories.ToList();
        }

        public DTO.CategoryDTO GetSingleById(int id)
        {
            return GetAll().SingleOrDefault(x => x.id == id);
        }
    }

    public class LocationRepository : GenericRepository<deORO_MasterEntities, location>
    {
        public LocationRepository()
        {

        }
        public LocationRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public void UpdateLastServicedDate()
        {
            var location = GetSingleById(x => x.id == locationId);

            if (location != null)
            {
                location.last_service_date_time = DateTime.Now;
            }
            Edit(location);
            Save();
        }

        public List<CashStatusDTO> GetLocationCashStatus(int locationId)
        {
            string sql = @"select description,amount,cast(count as int) as count,
                         case is_full when 1 Then 'Yes' ELSE 'No' end as IsFull
                         from cash_status
                         where locationid = {0} and 
                         created_date_time = (select max(created_date_time) from cash_status where locationid = {0})";

            var locationCashStatus = Context.Database.SqlQuery<DTO.CashStatusDTO>(string.Format(sql, locationId));
            return locationCashStatus.ToList();
        }

        public bool ValidateUser(string userName, string password)
        {
        
            var location = Context.locations.Where(x => x.username == userName && x.password == password);

            if (location.Count() > 0)
                return true;
            else
                return false;
        }

        public List<DTO.LocationDTO> GetAll(int id)
        {
            var locations = (from l in Context.locations
                             from c in Context.customers
                             where l.customerid == c.id && c.id == id
                             select new DTO.LocationDTO
                             {
                                 id = l.id,
                                 customerid = c.id,
                                 customername = c.name,
                                 name = l.name,
                                 code = l.code,
                                 is_active = (byte?)l.is_active.Value ?? 0,
                                 address = l.address,
                                 city = l.city,
                                 state = l.state,
                                 zip = l.zip,
                                 phone = l.phone,
                                 fax = l.fax,
                                 email_address = l.email_address,
                                 password = l.password,
                                 username = l.username,
                                 camera_feed_path = l.camera_feed_path,
                                 users_shared = (byte?)l.users_shared.Value ?? 0
                             }).ToList();

            return locations;
        }

        public new List<DTO.LocationDTO> GetAll()
        {
            var locations = (from l in Context.locations
                             from c in Context.customers
                             where l.customerid == c.id
                             select new DTO.LocationDTO
                             {
                                 id = l.id,
                                 customerid = c.id,
                                 customername = c.name,
                                 name = l.name,
                                 code = l.code,
                                 is_active = (byte?)l.is_active.Value ?? 0,
                                 address = l.address,
                                 city = l.city,
                                 state = l.state,
                                 zip = l.zip,
                                 phone = l.phone,
                                 fax = l.fax,
                                 email_address = l.email_address,
                                 password = l.password,
                                 username = l.username,
                                 driverid = l.driverid,
                                 service_interval = (int?)l.service_interval.Value ?? 0,
                                 last_service_date_time = (DateTime?)l.last_service_date_time.Value ?? null,
                                 camera_feed_path = l.camera_feed_path,
                                 users_shared = (byte?)l.users_shared.Value ?? 0
                             }).ToList();

            return locations;
        }

        public DataTable GetServicedRoutes()
        {

             var servicedRoutes = (from l in Context.locations
                             from ls in Context.location_service
                             where ls.locationid == l.id && ls.comments == "Service Completed" && ls.created_date_time > DateTime.Today
                             select new
                             {
                                 route = l.city,
                             }).GroupBy(x => x.route).ToList();

             return servicedRoutes.ToList().ToDataTable();
        }

        public DTO.LocationDTO GetSingleById(int id)
        {
            return GetAll().SingleOrDefault(x => x.id == id);
        }

        public DTO.LocationDashboardDTO GetDashboardData(int id, string fromDate, string toDate)
        {
            ShoppingCartDetailRepository repo1 = new ShoppingCartDetailRepository();
            TransactionErrorRepository repo2 = new TransactionErrorRepository();
            DeviceErrorRepository repo3 = new DeviceErrorRepository();

            DateTime fromDT = DateTime.Parse(fromDate);
            DateTime toDT = DateTime.Parse(toDate).AddDays(1);

            LocationDashboardDTO data = new LocationDashboardDTO();

            data.last30Top5 = repo1.GetTopSalesByItemCount(id, DateTime.Now.AddDays(-30), DateTime.Now, 5);
            data.last7Top5 = repo1.GetTopSalesByItemCount(id, DateTime.Now.AddDays(-7), DateTime.Now, 5);
            data.dateRangeTop5 = repo1.GetTopSalesByItemCount(id, fromDT, toDT, 5);

            data.last30Bottom5 = repo1.GetBottomSalesByItemCount(id, DateTime.Now.AddDays(-30), DateTime.Now, 5);
            data.last7Bottom5 = repo1.GetBottomSalesByItemCount(id, DateTime.Now.AddDays(-7), DateTime.Now, 5);
            data.dateRangeBottom5 = repo1.GetBottomSalesByItemCount(id, fromDT, toDT, 5);

            data.transactionErrors = repo2.GetErrors(id, fromDT, toDT);
            data.deviceErrors = repo3.GetErrors(id, fromDT, toDT);

            return data;
        }
    }

    public class CustomerRepository : GenericRepository<deORO_MasterEntities, customer>
    {
        public CustomerRepository()
        {

        }
    }


    public class ManufacutrerRepository : GenericRepository<deORO_MasterEntities, manufacturer>
    {

        public ManufacutrerRepository()
        {

        }

        public ManufacutrerRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }
    }

    public class SynclogRepository : GenericRepository<deORO_MasterEntities, synclog>
    {
        public SynclogRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }
    }

    public class UserRepository : GenericRepository<deORO_MasterEntities, user>
    {

        public bool UsersSharedAcrossLocations { get; set; }

        public UserRepository()
        {

        }

        public UserRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public List<UserDTO> GetAll(int customerid, int locationid, byte usersshared)
        {
            if (usersshared == 1)
                return GetAllWhereLocationIsNull(customerid);
            else
                return GetAll(customerid, locationid);
        }

        public List<UserDTO> GetAll(int customerid, int locationid)
        {
            var users = (from l in Context.locations
                         from c in Context.customers
                         from d in Context.users
                         where c.id == d.customerid && d.locationid == l.id && d.locationid == locationid && c.id == customerid
                         select new DTO.UserDTO
                         {
                             id = d.id,
                             customerid = c.id,
                             customername = c.name,
                             locationid = l.id,
                             locationname = l.name,
                             first_name = d.first_name,
                             last_name = d.last_name,
                             email = d.email,
                             username = d.username,
                             lastlogindate = (DateTime?)d.lastlogindate.Value ?? null,
                             last_updated_on = (DateTime?)d.last_updated_on.Value ?? null,
                             password = d.password,
                             pkid = d.pkid,
                             sync_vector = (int?)d.sync_vector ?? 0,
                             //===Replace commentes because we need the prefix "deORO_" in some users
                             //barcode = d.barcode.Replace("deORO_", "") 
                             is_active = (byte?)d.is_active ?? 1,
                             is_superuser = (byte?)d.is_superuser ?? 0,
                             is_staff = (byte?)d.is_staff ?? 0,
                             is_lockedout = (byte?)d.is_lockedout ?? 0,
                             is_approved = (byte?)d.is_approved ?? 1,
                             enrolled_fmd1 = d.enrolled_fmd1,
                             finger_id1 = d.finger_id1,
                             enrolled_fmd2 = d.enrolled_fmd2,
                             finger_id2 = d.finger_id2,
                             enrolled_fmd3 = d.enrolled_fmd3,
                             finger_id3 = d.finger_id3,
                             enrolled_fmd4 = d.enrolled_fmd4,
                             finger_id4 = d.finger_id4,
                             salt = d.salt,
                             barcode = d.barcode,
                             gender = d.gender,
                             dob = d.dob,
                             account_balance = (decimal?)d.account_balance.Value ?? 0,
                             lastaccountbalancechangedamount = (decimal?)d.lastaccountbalancechangedamount.Value ?? 0,
                             lastaccountbalancechangeddescription = d.lastaccountbalancechangeddescription,
                             lastaccountbalancechangeddate = (DateTime?)d.lastaccountbalancechangeddate ?? null,
                             payroll_balance = (decimal?)d.payroll_balance.Value ?? 0,    
                             lastpayrollbalancechangedamount = (decimal?)d.lastpayrollbalancechangedamount.Value ?? 0,
                             lastpayrollbalancechangeddescription = d.lastpayrollbalancechangeddescription,
                             lastpayrollbalancechangeddate = (DateTime?)d.lastpayrollbalancechangeddate ?? null

                         }).ToList();

            return users;
        }

        public List<UserDTO> GetAllWhereLocationIsNull(int customerid)
        {
            var users = (from c in Context.customers
                         from d in Context.users
                         where c.id == d.customerid && d.locationid == 0 && c.id == customerid
                         select new DTO.UserDTO
                         {
                             id = d.id,
                             customerid = c.id,
                             customername = c.name,                          
                             locationname = "Multiple Location User",
                             first_name = d.first_name,
                             last_name = d.last_name,
                             email = d.email,
                             username = d.username,
                             lastlogindate = (DateTime?)d.lastlogindate.Value ?? null,
                             last_updated_on = (DateTime?)d.last_updated_on.Value ?? null,
                             password = d.password,
                             pkid = d.pkid,
                             sync_vector = (int?)d.sync_vector ?? 0,
                             //===Replace commentes because we need the prefix "deORO_" in some users
                             //barcode = d.barcode.Replace("deORO_", "") 
                             is_active = (byte?)d.is_active ?? 1,
                             is_superuser = (byte?)d.is_superuser ?? 0,
                             is_staff = (byte?)d.is_staff ?? 0,
                             is_lockedout = (byte?)d.is_lockedout ?? 0,
                             is_approved = (byte?)d.is_approved ?? 1,
                             enrolled_fmd1 = d.enrolled_fmd1,
                             finger_id1 = (int?)d.finger_id1,
                             enrolled_fmd2 = d.enrolled_fmd2,
                             finger_id2 = (int?)d.finger_id2,
                             enrolled_fmd3 = d.enrolled_fmd3,
                             finger_id3 = (int?)d.finger_id3,
                             enrolled_fmd4 = d.enrolled_fmd4,
                             finger_id4 = (int?)d.finger_id4,
                             salt = d.salt,
                             barcode = d.barcode,
                             gender = d.gender,
                             dob = d.dob,
                             account_balance = (decimal?)d.account_balance.Value ?? 0,
                             lastaccountbalancechangedamount = (decimal?)d.lastaccountbalancechangedamount.Value ?? 0,
                             lastaccountbalancechangeddescription = d.lastaccountbalancechangeddescription,
                             lastaccountbalancechangeddate = (DateTime?)d.lastaccountbalancechangeddate ?? null,
                             payroll_balance = (decimal?)d.payroll_balance.Value ?? 0,    
                             lastpayrollbalancechangedamount = (decimal?)d.lastpayrollbalancechangedamount.Value ?? 0,
                             lastpayrollbalancechangeddescription = d.lastpayrollbalancechangeddescription,
                             lastpayrollbalancechangeddate = (DateTime?)d.lastpayrollbalancechangeddate ?? null

                         }).ToList();

            return users;
        }

        public List<user> GetAllWhereLocationIsNull()
        {
            var users = (from d in Context.users
                         where d.locationid == 0 && d.customerid == customerId
                         select d).ToList();

            return users;
        }



        public user GetLastUser()
        {
           
           var users = Context.users.OrderByDescending(x => x.id).FirstOrDefault();

           return users;
        }

        public user GetSingleById(int id)
        {
            return GetAll().SingleOrDefault(x => x.id == id);
        }

        //Sync User Addition
        public List<user> GetSingleByPkId(string pkid)
        {
            var users = (from d in Context.users
                         where d.pkid == pkid
                         select d).ToList();

            return users;
        }



    }
    public class UserDeletedRepository : GenericRepository<deORO_MasterEntities, users_deleted>
    {

        public UserDeletedRepository()
        {

        }

        public UserDeletedRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public new List<DTO.DeletedUserDTO> GetAll()
        {
            var users = (from l in Context.locations
                         from c in Context.customers
                         from d in Context.users_deleted
                         where c.id == d.customerid && d.locationid == l.id
                         select new DTO.DeletedUserDTO
                         {
                             id = d.id,
                             customerid = c.id,
                             customername = c.name,
                             locationid = l.id,
                             locationname = l.name,
                             first_name = d.first_name,
                             last_name = d.last_name,
                             amount_to_refund = (decimal?)d.amount_to_refund ?? 0,
                             email = d.email,
                             address = d.address,
                             city = d.city,
                             state = d.state,
                             zip = d.zip,
                             phone = d.phone,
                             refund_cleared = (byte?)d.refund_cleared ?? 0,
                             refund_processed = (byte?)d.refund_processed ?? 0

                         }).ToList();

            return users;
        }

        public override void Save(DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {
                string pkid = dr["pkid"].ToString();

                if (Context.users_deleted.Where(x => x.pkid == pkid).SingleOrDefault() == null)
                {
                    var rec = dr.ConvertToEntity<users_deleted>();

                    Add(rec);
                    rec.locationid = locationId;
                    rec.customerid = customerId;
                }
            }

            try
            {
                Save();
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Success", "Success");
            }
            catch (Exception ex)
            {
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Failed", ex.Message);
            }

        }
    }

    public class AccountBalanceHistoryRepository : GenericRepository<deORO_MasterEntities, accountbalancehistory>
    {

        public AccountBalanceHistoryRepository() { }

        public AccountBalanceHistoryRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public List<accountbalancehistory> GetHistories(string userpkid, string fromDate, string toDate)
        {
            DateTime fromDt = DateTime.Parse(fromDate);
            DateTime toDt = DateTime.Parse(toDate).AddDays(1);


            var histories = (from h in Context.accountbalancehistories
                             where h.userpkid == userpkid && h.created_date_time > fromDt && h.created_date_time < toDt
                             select h).OrderByDescending(x => x.created_date_time).ToList();

            return histories;
        }

        public override void Save(DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {
                string pkid = dr["pkid"].ToString();

                if (Context.accountbalancehistories.Where(x => x.pkid == pkid).SingleOrDefault() == null)
                {
                    var rec = dr.ConvertToEntity<accountbalancehistory>();

                    Add(rec);
                    rec.locationid = locationId;
                    rec.customerid = customerId;
                }
            }

            try
            {
                Save();
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Success", "Success");
            }
            catch (Exception ex)
            {
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Failed", ex.Message);
            }

        }
    }

    public class ShoppingCartDetailRepository : GenericRepository<deORO_MasterEntities, shoppingcartdetail>
    {
        public ShoppingCartDetailRepository() { }

        public ShoppingCartDetailRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public override void Save(DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {
                string pkid = dr["pkid"].ToString();

                if (Context.shoppingcartdetails.Where(x => x.pkid == pkid).SingleOrDefault() == null)
                {
                    var rec = dr.ConvertToEntity<shoppingcartdetail>();

                    Add(rec);
                    rec.locationid = locationId;
                    rec.customerid = customerId;
                }
            }

            try
            {
                Save();
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Success", "Success");
            }
            catch (Exception ex)
            {
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Failed", ex.Message);
            }
        }

        public List<ShoppingCartItemDTO> GetShoppingCartDetails(string pkid)
        {
            var items = (from s in Context.shoppingcarts
                         from d in Context.shoppingcartdetails
                         from i in Context.items
                         where s.pkid == d.shoppingcartpkid && s.pkid == pkid && d.itemid == i.id
                         select new ShoppingCartItemDTO
                         {
                             barcode = d.barcode,
                             itemname = i.name,
                             tax = d.tax.Value,
                             price = d.price.Value,
                             price_tax_included = d.price_tax_included.Value

                         }).ToList();

            return items;
        }


        public List<LocationDashboardItemDTO> GetTopSalesByItemCount(int locationid, DateTime fromDate, DateTime toDate, int take)
        {
            if (locationid != 0)
            {
                var data = (from s in Context.shoppingcartdetails
                            from i in Context.items
                            where s.created_date_time >= fromDate && s.created_date_time < toDate && s.locationid == locationid && i.id == s.itemid
                            group s by i into g
                            let totalSaleCount =
                                     (
                                        from d in Context.shoppingcartdetails.Where(x => x.created_date_time >= fromDate && x.created_date_time < toDate && x.locationid == locationid)
                                        select d.id
                                     ).Count()
                            let totalSaleAmount =
                                     (
                                        from d in Context.shoppingcartdetails.Where(x => x.created_date_time >= fromDate && x.created_date_time < toDate && x.locationid == locationid)
                                        select d.price_tax_included
                                     ).Sum()
                            select new LocationDashboardItemDTO
                            {
                                itemname = g.Key.name,
                                count = g.Count(),
                                amount = g.Sum(c => c.price_tax_included.Value),
                                totalSaleCount = totalSaleCount,
                                totalSaleAmount = totalSaleAmount.Value,
                            }).OrderByDescending(x => x.count).Take(take).ToList();

                return data;
            }
            else
            {
                var data = (from s in Context.shoppingcartdetails
                            from i in Context.items
                            where s.created_date_time >= fromDate && s.created_date_time < toDate && i.id == s.itemid
                            group s by i into g
                            let totalSaleCount =
                                     (
                                        from d in Context.shoppingcartdetails.Where(x => x.created_date_time >= fromDate && x.created_date_time < toDate)
                                        select d.id
                                     ).Count()
                            let totalSaleAmount =
                                     (
                                        from d in Context.shoppingcartdetails.Where(x => x.created_date_time >= fromDate && x.created_date_time < toDate)
                                        select d.price_tax_included
                                     ).Sum()
                            select new LocationDashboardItemDTO
                            {
                                itemname = g.Key.name,
                                count = g.Count(),
                                amount = g.Sum(c => c.price_tax_included.Value),
                                totalSaleCount = totalSaleCount,
                                totalSaleAmount = totalSaleAmount.Value,
                            }).OrderByDescending(x => x.count).Take(take).ToList();

                return data;
            }


        }

        public List<LocationDashboardItemDTO> GetBottomSalesByItemCount(int locationid, DateTime fromDate, DateTime toDate, int take)
        {
            if (locationid != 0)
            {
                var data = (from s in Context.shoppingcartdetails
                            from i in Context.items
                            where s.created_date_time >= fromDate && s.created_date_time < toDate && s.locationid == locationid && i.id == s.itemid
                            group s by i into g
                            let totalSaleCount =
                                     (
                                        from d in Context.shoppingcartdetails.Where(x => x.created_date_time >= fromDate && x.created_date_time < toDate && x.locationid == locationid)
                                        select d.id
                                     ).Count()
                            let totalSaleAmount =
                                     (
                                        from d in Context.shoppingcartdetails.Where(x => x.created_date_time >= fromDate && x.created_date_time < toDate && x.locationid == locationid)
                                        select d.price_tax_included
                                     ).Sum()
                            select new LocationDashboardItemDTO
                            {
                                itemname = g.Key.name,
                                count = g.Count(),
                                amount = g.Sum(c => c.price_tax_included.Value),
                                totalSaleCount = totalSaleCount
                            }).OrderBy(x => x.count).Take(take).ToList();

                return data;
            }
            else
            {
                var data = (from s in Context.shoppingcartdetails
                            from i in Context.items
                            where s.created_date_time >= fromDate && s.created_date_time < toDate && i.id == s.itemid
                            group s by i into g
                            let totalSaleCount =
                                     (
                                        from d in Context.shoppingcartdetails.Where(x => x.created_date_time >= fromDate && x.created_date_time < toDate)
                                        select d.id
                                     ).Count()
                            let totalSaleAmount =
                                     (
                                        from d in Context.shoppingcartdetails.Where(x => x.created_date_time >= fromDate && x.created_date_time < toDate)
                                        select d.price_tax_included
                                     ).Sum()
                            select new LocationDashboardItemDTO
                            {
                                itemname = g.Key.name,
                                count = g.Count(),
                                amount = g.Sum(c => c.price_tax_included.Value),
                                totalSaleCount = totalSaleCount
                            }).OrderBy(x => x.count).Take(take).ToList();

                return data;
            }
        }
    }

    public class ShoppingCartRepository : GenericRepository<deORO_MasterEntities, shoppingcart>
    {
        public ShoppingCartRepository() { }

        public ShoppingCartRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }


        public ShoppingCartDTO GetShoppingCart(string pkid)
        {
            ShoppingCartDetailRepository repo1 = new ShoppingCartDetailRepository();
            PaymentRepository repo2 = new PaymentRepository();

            var detail = (from c in Context.customers
                          from l in Context.locations.Where(z => z.customerid == c.id)
                          from s in Context.shoppingcarts.Where(x => c.id == x.customerid && l.id == x.locationid && x.pkid == pkid)
                          from u in Context.users.Where(y => y.pkid == s.userpkid).DefaultIfEmpty()
                          select new ShoppingCartDTO
                          {
                              pkid = s.pkid,
                              username = u.username,
                              firstname = u.first_name,
                              lastname = u.last_name,
                              created_date_time = s.created_date_time.Value,
                              customername = c.name,
                              locationname = l.name,
                          }).SingleOrDefault();

            return detail;
        }

        public override void Save(DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {
                string pkid = dr["pkid"].ToString();

                if (Context.shoppingcarts.Where(x => x.pkid == pkid).SingleOrDefault() == null)
                {
                    var rec = dr.ConvertToEntity<shoppingcart>();

                    Add(rec);
                    rec.locationid = locationId;
                    rec.customerid = customerId;
                }
            }

            try
            {
                Save();
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Success", "Success");
            }
            catch (Exception ex)
            {
                SaveToLog(string.Format("Upload Table Name - {0}, Rows = {1}", dt.TableName, dt.Rows.Count), "Failed", ex.Message);
            }

        }

        public List<ShoppingCartDTO> GetShoppingCarts(string userpkid, string fromDate, string toDate)
        {

            DateTime fromDt = DateTime.Parse(fromDate);
            DateTime toDt = DateTime.Parse(toDate).AddDays(1);

            var carts = (from s in Context.shoppingcarts
                         from u in Context.users
                         where s.created_date_time >= fromDt && s.created_date_time < toDt && s.userpkid == u.pkid && u.pkid == userpkid
                         let cartTotal =
                                 (
                                    from d in Context.shoppingcartdetails.Where(x => x.shoppingcartpkid == s.pkid)
                                    select d.price_tax_included
                                 ).Sum()
                         let itemCount =
                         (
                            from d in Context.shoppingcartdetails.Where(x => x.shoppingcartpkid == s.pkid)
                            select d.price_tax_included
                         ).Count()
                         select new ShoppingCartDTO
                         {
                             pkid = s.pkid,
                             created_date_time = s.created_date_time,
                             itemcount = itemCount,
                             price_tax_included = cartTotal.Value
                         }).OrderByDescending(x => x.created_date_time).ToList();

            return carts;
        }


    }

    public class ItemRepository : GenericRepository<deORO_MasterEntities, item>
    {
        public ItemRepository() { }

        public ItemRepository(int customerId, int locationId)
            : base(customerId, locationId)
        {

        }

        public new List<DTO.ItemDTO> GetAll()
        {
            var items = from i in Context.items
                        from m in Context.manufacturers
                        .Where(x => x.id == i.manufacturerid).DefaultIfEmpty()
                        from c1 in Context.categories
                        .Where(y => y.id == i.subcategoryid).DefaultIfEmpty()
                        from c2 in Context.categories
                        .Where(z => z.id == c1.parentid).DefaultIfEmpty()
                        select new deORODataAccess.DTO.ItemDTO
                        {
                            id = i.id,
                            manufacturerid = (int?)m.id ?? 0,
                            manufacturername = m.name,
                            categoryid = (int?)c2.id ?? 0,
                            categoryname = c2.name,
                            subcategoryid = (int?)c1.id ?? 0,
                            subcategoryname = c1.name,
                            upc = i.upc,
                            name = i.name,
                            barcode = i.barcode,
                            description = i.description,
                            count = (int?)i.count.Value ?? 0,
                            unitcost = (decimal?)i.unitcost.Value ?? 0,
                            avgshelflife = i.avgshelflife,
                            pickorder = (int?)i.pickorder.Value ?? 0,
                            is_taxable = (byte?)i.is_taxable.Value ?? 0,
                            price = (decimal?)i.price.Value ?? 0,
                            tax = (decimal?)i.tax.Value ?? 0,
                            price_tax_included = (decimal?)i.price_tax_included.Value ?? 0,
                            tax_percent = (decimal?)i.tax_percent.Value ?? 0,
                            has_barcode = (byte?)i.has_barcode.Value ?? 0,
                            image = i.image,
                            crv = (decimal?)i.crv.Value ?? 0,
                            is_active = (byte?)i.is_active.Value ?? 0,
                        };

            return items.ToList();
        }

        public void BatchEdit(int? manufactureid = null, int? categoryid = null, int? subcategoryid = null, byte? taxable = null,
                              decimal? unitcost = null, int? count = null, decimal? price = null, decimal? taxpercent = null, decimal? crv = null,
                              int[] itemids = null)
        {
            for (int i = 0; i < itemids.Count(); i++)
            {
                int id = itemids[i];
                item item = FindBy(x => x.id == id).SingleOrDefault();

                if (item != null)
                {
                    item.manufacturerid = manufactureid ?? item.manufacturerid;
                    item.categoryid = categoryid ?? item.categoryid;

                    if (categoryid == null)
                    {
                        item.subcategoryid = subcategoryid ?? item.subcategoryid;
                    }
                    else
                    {
                        item.subcategoryid = subcategoryid;
                    }

                    item.is_taxable = taxable ?? item.is_taxable;
                    item.unitcost = unitcost ?? item.unitcost;
                    item.price = price ?? item.price;
                    item.tax_percent = taxpercent ?? item.tax_percent;
                    item.crv = crv ?? item.crv;
                    item.count = count ?? item.count;

                    if (item.tax_percent != null)
                    {
                        item.tax = (item.price ?? 0) * (item.tax_percent * 0.01m);
                        item.price_tax_included = (item.price ?? 0) + (item.crv ?? 0) + (item.tax ?? 0);
                    }

                    Edit(item);
                }
            }


        }

        public DataTable GetAllToExport()
        {
            var items = from i in Context.items
                        from m in Context.manufacturers
                        .Where(x => x.id == i.manufacturerid).DefaultIfEmpty()
                        from c1 in Context.categories
                        .Where(y => y.id == i.subcategoryid).DefaultIfEmpty()
                        from c2 in Context.categories
                        .Where(z => z.id == c1.parentid).DefaultIfEmpty()
                        select new
                        {
                            id = i.id,
                            manufacturer = m.name,
                            categoryname = c2.name,
                            subcategoryname = c1.name,
                            upc = i.upc,
                            name = i.name,
                            barcode = i.barcode,
                            description = i.description,
                            count = (int?)i.count.Value ?? 0,
                            unitcost = (decimal?)i.unitcost.Value ?? 0,
                            avgshelflife = i.avgshelflife,
                            pickorder = (int?)i.pickorder.Value ?? 0,
                            is_taxable = (byte?)i.is_taxable.Value ?? 0,
                            price = (decimal?)i.price.Value ?? 0,
                            tax = (decimal?)i.tax.Value ?? 0,
                            price_tax_included = (decimal?)i.price_tax_included.Value ?? 0,
                            tax_percent = (decimal?)i.tax_percent.Value ?? 0,
                            crv = (decimal?)i.crv.Value ?? 0,
                            has_barcode = (byte?)i.has_barcode.Value ?? 0,
                            is_active = (byte?)i.is_active.Value ?? 0,
                        };

            return items.ToList().ToDataTable();
        }

        

        public List<DTO.ItemDTO> GetAll(int active)
        {
            var items = from i in Context.items.AsNoTracking().Where(a => a.is_active == active)
                        from m in Context.manufacturers
                        .Where(x => x.id == i.manufacturerid).DefaultIfEmpty()
                        from c1 in Context.categories
                        .Where(y => y.id == i.categoryid).DefaultIfEmpty()
                        from c2 in Context.categories
                        .Where(z => z.id == i.subcategoryid).DefaultIfEmpty()
                        select new deORODataAccess.DTO.ItemDTO
                        {
                            id = i.id,
                            manufacturerid = (int?)m.id ?? 0,
                            manufacturername = m.name,
                            categoryid = (int?)c1.id ?? 0,
                            categoryname = c1.name,
                            subcategoryid = (int?)c2.id ?? 0,
                            subcategoryname = c2.name,
                            upc = i.upc,
                            name = i.name,
                            barcode = i.barcode,
                            description = i.description,
                            count = (int?)i.count.Value ?? 0,
                            unitcost = (decimal?)i.unitcost.Value ?? 0,
                            avgshelflife = i.avgshelflife,
                            pickorder = (int?)i.pickorder.Value ?? 0,
                            is_taxable = (byte?)i.is_taxable.Value ?? 0,
                            price = (decimal?)i.price.Value ?? 0,
                            tax = (decimal?)i.tax.Value ?? 0,
                            price_tax_included = (decimal?)i.price_tax_included.Value ?? 0,
                            tax_percent = (decimal?)i.tax_percent.Value ?? 0,
                            has_barcode = (byte?)i.has_barcode.Value ?? 0,
                            image = i.image,
                            crv = (decimal?)i.crv.Value ?? 0,
                            is_active = (byte?)i.is_active.Value ?? 0,
                        };

            return items.ToList();
        }

        public DTO.ItemDTO GetSingleById(int id)
        {
            var item = (from i in Context.items.AsNoTracking().Where(x => x.id == id)
                        from m in Context.manufacturers
                        .Where(x => x.id == i.manufacturerid).DefaultIfEmpty()
                        from c1 in Context.categories
                        .Where(y => y.id == i.categoryid).DefaultIfEmpty()
                        from c2 in Context.categories
                        .Where(z => z.id == i.subcategoryid).DefaultIfEmpty()
                        select new deORODataAccess.DTO.ItemDTO
                        {
                            id = i.id,
                            manufacturerid = (int?)m.id ?? 0,
                            manufacturername = m.name,
                            categoryid = (int?)c1.id ?? 0,
                            categoryname = c1.name,
                            subcategoryid = (int?)c2.id ?? 0,
                            subcategoryname = c2.name,
                            upc = i.upc,
                            name = i.name,
                            barcode = i.barcode,
                            description = i.description,
                            count = (int?)i.count.Value ?? 0,
                            unitcost = (decimal?)i.unitcost.Value ?? 0,
                            avgshelflife = i.avgshelflife,
                            pickorder = (int?)i.pickorder.Value ?? 0,
                            is_taxable = (byte?)i.is_taxable.Value ?? 0,
                            price = (decimal?)i.price.Value ?? 0,
                            tax = (decimal?)i.tax.Value ?? 0,
                            price_tax_included = (decimal?)i.price_tax_included.Value ?? 0,
                            tax_percent = (decimal?)i.tax_percent.Value ?? 0,
                            has_barcode = (byte?)i.has_barcode.Value ?? 0,
                            image = i.image,
                            crv = (decimal?)i.crv.Value ?? 0,
                            is_active = (byte?)i.is_active.Value ?? 0,
                        }).SingleOrDefault();

            return item;
        }

        public List<item> GetAllByCustomerLocation()
        {
            var items = (from i in Context.items
                         from l in Context.location_item
                         where l.customerid == customerId && l.locationid == locationId
                         && l.itemid == i.id
                         select new
                         {
                             id = i.id,
                             manufacturerid = i.manufacturerid,
                             categoryid = i.subcategoryid,
                             discountid = l.discountid,
                             upc = i.upc,
                             name = i.name,
                             barcode = i.barcode,
                             description = i.description,
                             count = i.count,
                             unitcost = i.unitcost,
                             avgshelflife = i.avgshelflife,
                             pickorder = i.pickorder,
                             is_taxable = l.is_taxable,
                             price = l.price,
                             tax = l.tax,
                             price_tax_included = l.price_tax_included,
                             tax_percent = l.tax_percent,
                             has_barcode = i.has_barcode,
                             image = i.image,
                             crv = l.crv

                         }).ToList().Select(x => new item
                         {
                             id = x.id,
                             manufacturerid = x.manufacturerid,
                             categoryid = x.categoryid,
                             discountid = x.discountid,
                             upc = x.upc,
                             name = x.name,
                             barcode = x.barcode,
                             description = x.description,
                             count = x.count,
                             unitcost = x.unitcost,
                             avgshelflife = x.avgshelflife,
                             pickorder = x.pickorder,
                             is_taxable = x.is_taxable,
                             price = x.price,
                             tax = x.tax,
                             price_tax_included = x.price_tax_included,
                             tax_percent = x.tax_percent,
                             has_barcode = x.has_barcode,
                             image = x.image,
                             crv = x.crv
                         });

            return items.ToList();
        }
    }
}