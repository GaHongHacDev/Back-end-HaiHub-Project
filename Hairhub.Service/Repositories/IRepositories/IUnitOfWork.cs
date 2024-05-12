using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Repositories.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        int Commit();

        Task<int> CommitAsync();

        IGenericRepository<TEntity> GetRepository<TEntity>()
           where TEntity : class;
    }
}
