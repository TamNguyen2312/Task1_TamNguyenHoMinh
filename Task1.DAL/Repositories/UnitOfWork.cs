using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Task1.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PubsContext _masterContext;
        private readonly IServiceProvider _serviceProvider;
        private IDbContextTransaction _transaction;
        public UnitOfWork(PubsContext masterContext, IServiceProvider serviceProvider)
        {
            _masterContext = masterContext;
            _serviceProvider = serviceProvider;
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
            return _serviceProvider.GetRequiredService<IRepoBase<T>>();
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
