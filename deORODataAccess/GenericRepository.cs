using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace deORODataAccess
{

    public abstract class GenericRepository<C, T> : IGenericRepository<T>
        where T : class,new()
        where C : DbContext, new()
    {
        private C _entities = new C();
        protected int customerId = 0;
        protected int locationId = 0;

        protected C Context
        {

            get { return _entities; }
            set { _entities = value; }
        }

        public GenericRepository()
        {

        }

        public GenericRepository(int customerId, int locationId)
        {
            this.customerId = customerId;
            this.locationId = locationId;
        }

        public virtual IQueryable<T> GetAll()
        {
            IQueryable<T> query = _entities.Set<T>().AsNoTracking();
            return query;
        }

        public IQueryable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _entities.Set<T>().Where(predicate);
            return query;
        }

        public T GetSingleById(System.Linq.Expressions.Expression<Func<T, bool>> predicateId)
        {
            var query = GetAll().Where(predicateId).FirstOrDefault();
            return query;
        }

        public virtual void Add(T entity)
        {

            _entities.Set<T>().Add(entity);
        }

        public virtual void Delete(T entity)
        {

            _entities.Set<T>().Remove(entity);
        }

        public virtual void Delete(T entity, Func<T, bool> predicate)
        {
            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                var set = Context.Set<T>();
                T attachedEntity = set.Local.SingleOrDefault(predicate);
                if (attachedEntity != null)
                {
                    var attachedEntry = Context.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(entity);
                }
                else
                {
                    entry.State = EntityState.Deleted; // This should attach entity
                }
            }
            _entities.Set<T>().Remove(entity);
        }

        public void Delete(Func<T, bool> predicate)
        {
            var entities = Context.Set<T>().Where(predicate);

            foreach (var entity in entities)
            {
                _entities.Set<T>().Remove(entity);
            }
        }

        public virtual void Edit(T entity)
        {
            _entities.Entry(entity).State = System.Data.EntityState.Modified;

        }

        public virtual void Edit(T entity, Func<T, bool> predicate)
        {
            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                var set = Context.Set<T>();
                T attachedEntity = set.Local.SingleOrDefault(predicate);
                if (attachedEntity != null)
                {
                    var attachedEntry = Context.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(entity);
                }
                else
                {
                    entry.State = EntityState.Modified; // This should attach entity
                }
            }


        }

        public virtual void Save()
        {
            _entities.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {

            if (!this.disposed)
                if (disposing)
                    _entities.Dispose();

            this.disposed = true;
        }

        public void Dispose()
        {

            Dispose(true);
            GC.SuppressFinalize(this);
        }

           
        public virtual void Save(DataTable dt)
        {

            foreach (DataRow dr in dt.Rows)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();

                T rec1 = dr.ConvertToEntity<T>();
                object pkid = _entities.Entry(rec1).Property("pkid").CurrentValue;
                T rec2 = Find(pkid.ToString());
                
                if (rec2 != null)
                {
                    Extensions.CopyPropertyValues(rec1, rec2, new string[] { "id" });
                    _entities.Entry(rec2).Property("locationid").CurrentValue = locationId;
                    _entities.Entry(rec2).Property("customerid").CurrentValue = customerId;
                    _entities.Entry(rec2).State = EntityState.Modified;
                }
                else
                {
                    Add(rec1);
                    _entities.Entry(rec1).Property("locationid").CurrentValue = locationId;
                    _entities.Entry(rec1).Property("customerid").CurrentValue = customerId;
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

        protected void SaveToLog(string description, string type, string message)
        {
            synclog log = new synclog();
            log.customerid = customerId;
            log.locationid = locationId;
            log.description = description;
            log.created_date_time = DateTime.Now;
            log.type = type;
            log.message = message;

            Context.Set<synclog>().Add(log);
            Save();
        }

        //private T Find(string pkId)
        //{
        //    return _entities.Set<T>().FirstOrDefault(
        //        delegate(T t)
        //        {

        //            //if (t.GetType().GetProperty("pkid").GetValue(t).ToString().Equals(pkId))
        //            //{
        //            //    return true;
        //            //}
        //            //else
        //            //{
        //            //    return false;
        //            //}
        //            var propeties = t.GetType().GetProperties();

        //            foreach (var prop in propeties)
        //            {
        //                if (prop.Name.Equals("pkid"))
        //                {
        //                    if (prop.GetValue(t).ToString().Equals(pkId))
        //                        return true;
        //                    else
        //                        return false;
        //                }
        //            }

        //            return false;
        //        }
        //     );
        //}

        protected T Find(string pkId)
        {
            return _entities.Set<T>().FirstOrDefault(
                delegate(T t)
                {

                    if (t.GetType().GetProperty("pkid").GetValue(t).ToString().Equals(pkId))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                    //var propeties = t.GetType().GetProperties();

                    //foreach (var prop in propeties)
                    //{
                    //    if (prop.Name.Equals("pkid"))
                    //    {
                    //        if (prop.GetValue(t).ToString().Equals(pkId))
                    //            return true;
                    //        else
                    //            return false;
                    //    }
                    //}

                    //return false;
                }
             );
        }


        private T Find(int id)
        {
            return _entities.Set<T>().FirstOrDefault(
               delegate(T t)
               {
                   var propeties = t.GetType().GetProperties();

                   foreach (var prop in propeties)
                   {
                       if (prop.Name.Equals("id"))
                       {
                           if (prop.GetValue(t).ToString().Equals(id))
                               return true;
                           else
                               return false;
                       }
                   }

                   return false;
               }
            );
        }

        private string GetPkId(object o)
        {
            //var propeties = o.GetType().GetProperties();

            //foreach (var prop in propeties)
            //{
            //    if (prop.Name.Equals("pkid"))
            //    {
            //        return prop.GetValue(o).ToString();
            //    }
            //}

            return "";
        }

        private string GetId(object o)
        {
            var propeties = o.GetType().GetProperties();

            foreach (var prop in propeties)
            {
                if (prop.Name.Equals("id"))
                {
                    return prop.GetValue(o).ToString();
                }
            }

            return "";
        }

        public string GetConnectionString()
        {
            return Context.Database.Connection.ConnectionString;
        }


        public void Edit(T entity, string columnName, string columnType, object value)
        {
            try
            {
                if (value.ToString() == "3.0000000000000027E-2")
                    value = "0.03";
                else if (value.ToString() == "4.0000000000000036E-2")
                    value = "0.04";
                else if (value.ToString() == "5.0000000000000044E-2" || value.ToString() == "4.9999999999999933E-2")
                    value = "0.05";
                else if (value.ToString() == "6.0000000000000053E-2")
                    value = "0.06";
                else if (value.ToString() == "7.0000000000000007E-2" || value.ToString() == "6.999999999999984E-2")
                    value = "0.07";
                else if (value.ToString() == "8.0000000000000071E-2")
                    value = "0.08";
                else if (value.ToString() == "9.000000000000008E-2")
                    value = "0.09";
                else if (value.ToString() == "9.9999999999999867E-2" || value.ToString() == "1.0000000000000009E-2")
                    value = "0.10";

                Type type = Type.GetType(columnType);
                _entities.Entry(entity).Property(columnName).CurrentValue = Convert.ChangeType(value, type);
            }
            catch
            {
                throw;
            }
        }

    }
}
