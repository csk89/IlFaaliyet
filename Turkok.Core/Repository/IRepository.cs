using System.Linq;

namespace Turkok.Core.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Table();
        TEntity Find(int id);
        TEntity Add(TEntity entity);
        TEntity Remove(TEntity entity);
        TEntity Remove(int id);
        TEntity Update(TEntity entity);
         
        int Upsert(TEntity entity);
    }
}