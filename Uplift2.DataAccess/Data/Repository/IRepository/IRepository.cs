using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Uplift2.DataAccess.Data.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //Get object with an Id.
        T Get(int id);

        //Get all objects with filters.
        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null
            );

        //Return one object from filtered criteria.
        T GetFirstOrDefault(
            Expression<Func<T, bool>> filter = null,
            string includeProperties = null
            );

        //Add Entity (function)
        void Add(T entity);

        //Remove an Entity (function)
        void Remove(int id);

        //Remove the whole entity.
        void Remove(T entity);
    }
}
