using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GoodStuffVending
{
    public interface IGenericRepository<T> : IDisposable where T : class
    {
        IQueryable<T> GetAll();
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        T GetSingleById(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Delete(T entity);
        void Edit(T entity, Func<T, bool> predicate);
        void Edit(T entity);
        void Save();
    }

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


        private T Find(string pkId)
        {
            return _entities.Set<T>().FirstOrDefault(
                delegate(T t)
                {
                    var propeties = t.GetType().GetProperties();

                    foreach (var prop in propeties)
                    {
                        if (prop.Name.Equals("pkid"))
                        {
                            if (prop.GetValue(t).ToString().Equals(pkId))
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
            var propeties = o.GetType().GetProperties();

            foreach (var prop in propeties)
            {
                if (prop.Name.Equals("pkid"))
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
    }



    public class InventoryRepository : GenericRepository<deORO_SourceEntities, Inventory>
    {
        public List<string> GetCategories()
        {
            var categories = (from c in Context.Inventories
                              select c.category).Distinct().ToList<string>();

            return categories;
        }
    }

    public class LocationRepository : GenericRepository<deORO_SourceEntities, Location>
    {
    }

    public class TransactionHistoryRepository : GenericRepository<deORO_SourceEntities, TransactionHistory>
    {
    }

    public class StockRepository : GenericRepository<deORO_SourceEntities, Stock>
    {
    }

    public class UserRepository : GenericRepository<deORO_SourceEntities,aspnet_Users>
    {
        public new List<DTO.UserDTO> GetAll()
        {
            var users = (from a in Context.aspnet_Users
                         from r in Context.aspnet_UsersInRoles.Where(y=>y.UserId ==a.UserId)
                         //from u in Context.Users.Where(x => x.phone == a.UserName).DefaultIfEmpty()
                         from m in Context.aspnet_MoMoney.Where(z=>z.UserId == a.UserId).DefaultIfEmpty().Take(1)
                         select new DTO.UserDTO
                         {
                             userName = a.UserName,
                             accountBalance = m.CurrentCash,
                             password = a.UserName,
                             createdDate = a.LastActivityDate,
                             modifiedDate = a.lastModified,
                             role = r.RoleId

                         }).ToList();

            return users;
        }
    }

}
