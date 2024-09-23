using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Task1.Util.Queries;

namespace Task1.DAL.Repositories
{
    public interface IRepoBase<T> where T : class
    {
        public Task<T> CreateAsync(T entity);
        public Task UpdateAsync(T entity);
        public Task DeleteAsync(T entity);
        public IQueryable<T> Get(QueryOptions<T> options);

		public Task<IEnumerable<T>> GetAllAsync(QueryOptions<T> options);
        public Task<T> GetSingleAsync(QueryOptions<T> options);

    }
}
