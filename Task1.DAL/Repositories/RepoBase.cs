﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Task1.Util.Queries;

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


        public async Task<T> CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public Task DeleteAsync(T entity)
        {
            if (_context.Entry<T>(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);

            return Task.CompletedTask;
        }

		public IQueryable<T> Get(QueryOptions<T> options)
		{
			IQueryable<T> query = _dbSet;

			if (!options.Tracked)
			{
				query = query.AsNoTracking();
			}

			if (options.IncludeProperties?.Any() ?? false)
			{
				foreach (var includeProperty in options.IncludeProperties)
				{
					query = query.Include(includeProperty);
				}
			}

			if (options.Predicate != null)
			{
				query = query.Where(options.Predicate);
			}

			if (options.OrderBy != null)
			{
				query = options.OrderBy(query);
			}

			return query;
		}

		public Task UpdateAsync(T entity)
        {

            if (_context.Entry<T>(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Update(entity);

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<T>> GetAllAsync(QueryOptions<T> options)
        {
            return await Get(options).ToListAsync();
        }

        public async Task<T> GetSingleAsync(QueryOptions<T> options)
		{
            return await Get(options).FirstOrDefaultAsync();
        }
    }
}
