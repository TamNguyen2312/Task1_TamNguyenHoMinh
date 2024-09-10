using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Task1.DAL.Repositories
{
    public interface IRepoBase<T> where T : class
    {
        public Task CreateAsync(T entity);
        public Task UpdateAsync(T entity);
        public Task DeleteAsync(T entity);
        public Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null, bool tracked = true, params Expression<Func<T, object>>[] includeProperties);
        public Task<T> GetSingle(Expression<Func<T, bool>> predicate = null, bool tracked = true, params Expression<Func<T, object>>[] includeProperties);

    }
}
