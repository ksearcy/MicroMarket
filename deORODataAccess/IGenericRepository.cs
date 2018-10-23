using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Data;

namespace deORODataAccess
{
    public interface IGenericRepository<T> : IDisposable where T : class
    {
        IQueryable<T> GetAll();
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        T GetSingleById(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Delete(T entity);
        void Delete(Func<T, bool> predicate);
        void Edit(T entity, Func<T, bool> predicate);
        void Edit(T entity);
        void Edit(T entity, string columnName, string columnType, object value);
        void Save();
    }


}