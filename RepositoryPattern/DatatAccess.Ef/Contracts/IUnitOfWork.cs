using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatatAccess.Ef.Contracts
{
    public interface IUnitOfWork : IProgramAbilitySupport, IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        Task SaveChangesAsync();

        void SaveChanges();
    }
}
