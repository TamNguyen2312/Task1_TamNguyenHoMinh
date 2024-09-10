using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Task1.DAL.Repositories
{
    public class RepoBase<T> : IRepoBase<T> where T : class
    {
        private readonly PubsContext _context;
        protected readonly DbSet<T> _dbSet;
        public RepoBase(PubsContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }


        public async Task CreateAsync(T entity)
        {
            _dbSet.AddAsync(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            if (_context.Entry<T>(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

        private IQueryable<T> Get(Expression<Func<T, bool>> predicate = null,
                                  Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                  bool tracked = true,
                                  params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;

            if (!tracked)
            {
                query.AsNoTracking();
            }

            includeProperties = includeProperties.Distinct().ToArray();
            Expression<Func<T, object>>[] array = includeProperties;
            if (includeProperties?.Any() ?? false)
            {
                foreach (var navigationProperty in array)
                {
                    query = query.Include(navigationProperty);
                }
            }

            if (predicate != null)
            {
                query.Where(predicate);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }


            return query;
        }

        public async Task UpdateAsync(T entity)
        {

            if (_context.Entry<T>(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Update(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null,
                                                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                bool tracked = true,
                                                params Expression<Func<T, object>>[] includeProperties)
        {
            return await Get(predicate, orderBy, tracked, includeProperties).ToListAsync();
        }

        public async Task<T> GetSingle(Expression<Func<T, bool>> predicate = null,
                                                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                bool tracked = true,
                                                params Expression<Func<T, object>>[] includeProperties)
        {
            return await Get(predicate, orderBy, tracked, includeProperties).FirstOrDefaultAsync();
        }
    }
}
