using BuyStuffOnline.DataAccess.Data;
using BuyStuffOnline.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;

namespace BuyStuffOnline.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbset;

        public Repository(ApplicationDbContext db) 
        {
            _db = db;
            this.dbset= _db.Set<T>();
        }

        public void Add(T entity)
        {
            dbset.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties, bool tracked = false)
        {
            IQueryable<T> query;
            if (tracked)
            {
                query = dbset;
            }
            else 
            {
                query = dbset.AsNoTracking();
            }

            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (string includeProp in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }

            }
            return query.FirstOrDefault();
        }

        //Category,Cover
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties=null)
        {
            IQueryable<T> query = dbset;

            if(filter!=null)
                query = query.Where(filter);


            if (!string.IsNullOrEmpty(includeProperties)) 
            {
                foreach (string includeProp in includeProperties.Split(',',StringSplitOptions.RemoveEmptyEntries)) 
                { 
                    query= query.Include(includeProp);
                }
                
            }

            return query.ToList(); 
        }

        public void Remove(T entity)
        {
            dbset.Remove(entity);
        }

        public void RemoveRange(T entity)
        {
            dbset.RemoveRange(entity);
        }
    }
}
