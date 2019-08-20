using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Uplift2.DataAccess.Data.Repository.IRepository;

namespace Uplift2.DataAccess.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        //Declare Data access variables.
        protected readonly DbContext Context;
        internal DbSet<T> dbSet;


        public Repository(DbContext context)
        {
            //Initilize dbSet(Using dependancy injection)
            Context = context;

            this.dbSet = context.Set<T>();
        }
           
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(int id)
        {
            return dbSet.Find(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null)
        {
            //Declare query variable
            IQueryable<T> query = dbSet;

            //Implementing the filter
            if (filter != null)
            {
                query = query.Where(filter);
            }

            //Check if there's any included properties.
            if (includeProperties != null)
            {
                //will be separated by a comma
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            //Check order by
            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }

            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProperties = null)
        {
            //Declare query variable
            IQueryable<T> query = dbSet;

            //Implementing the filter
            if (filter != null)
            {
                query = query.Where(filter);
            }

            //Check if there's any included properties.
            if (includeProperties != null)
            {
                //will be separated by a comma
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query.FirstOrDefault();
        }

        public void Remove(int id)
        {
            T entityToRemove = dbSet.Find(id);
            Remove(entityToRemove);
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }
    }
}
