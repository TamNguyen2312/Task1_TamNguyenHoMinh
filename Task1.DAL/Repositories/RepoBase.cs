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
        protected readonly MasterContext _context;
        private DbSet<T> _dbset;
        public RepoBase()
        {
            _context = new MasterContext();
            _dbset = _context.Set<T>();
        }

        protected DbSet<T> DbSet
        {
            get 
            {
                if (_dbset != null)
                {
                    return _dbset;
                }

                _dbset = _context.Set<T>();
                return _dbset;
            }
        }

        public async Task CreateAsync(T entity)
        {
            _dbset.AddAsync(entity);
            await SaveAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            if (_context.Entry<T>(entity).State == EntityState.Detached)
            {
                _dbset.Attach(entity);
            }
            _dbset.Remove(entity);
            await SaveAsync();
        }

        private IQueryable<T> Get(Expression<Func<T, bool>> predicate = null, bool tracked = true, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = DbSet;

            if(!tracked)
            {
                query.AsNoTracking();
            }

            includeProperties = includeProperties.Distinct().ToArray();
            Expression<Func<T, object>>[] array = includeProperties;
            if(includeProperties?.Any() ?? false)
            {
                foreach (var navigationProperty in array)
                {
                    query = query.Include(navigationProperty);
                }
            }

            if(predicate != null)
            {
                query.Where(predicate);
            }

            return query;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {

            if (_context.Entry<T>(entity).State == EntityState.Detached)
            {
                _dbset.Attach(entity);
            }
            _dbset.Update(entity);
            await SaveAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null, bool tracked = true, params Expression<Func<T, object>>[] includeProperties)
        {
           return await Get(predicate, tracked, includeProperties).ToListAsync();
        }

        public async Task<T> GetSingle(Expression<Func<T, bool>> predicate = null, bool tracked = true, params Expression<Func<T, object>>[] includeProperties)
        {
            return await Get(predicate, tracked, includeProperties).FirstOrDefaultAsync();
        }
    }
}
