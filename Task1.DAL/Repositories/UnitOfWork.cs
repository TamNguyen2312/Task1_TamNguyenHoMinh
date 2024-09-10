using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PubsContext _masterContext;
        private IDbContextTransaction _transaction;
        private readonly Dictionary<Type, object> _repositories;
        public UnitOfWork(PubsContext masterContext)
        {
            _masterContext = masterContext;
            _repositories = new Dictionary<Type, object>();
        }
        public async Task BeginTransactionAsync()
        {
            _transaction = await _masterContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _transaction.CommitAsync();
            }
            catch
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null!;
            }
        }

        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _masterContext.Dispose();
                }
                this.disposed = true;
            }
        }

        public IRepoBase<T> GetRepo<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return _repositories[typeof(T)] as IRepoBase<T>;
            }

            var repo = new RepoBase<T>(_masterContext);
            _repositories.Add(typeof(T), repo);
            return repo;
        }

        public async Task RollBackAsync()
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null!;
        }

        public async Task SaveChangesAsync()
        {
            await _masterContext.SaveChangesAsync();
        }
    }
}
