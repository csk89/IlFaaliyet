using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Turkok.Core.Repository
{
    public interface IDbContext
    {
        IDbSet<T> Set<T>() where T : class;
        DbEntityEntry<T> Entry<T>(T entity) where T : class;
        int SaveChanges();
        void Dispose();
    }
}
