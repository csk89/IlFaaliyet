using System.Linq;
using PagedList;

namespace Turkok.Core.Service.CrudService
{
    public interface ICrudService<T>
    {

        T Create(T item);
        T Find(int id);
        T Update(T item);
        T Delete(T item);
        IQueryable<T> GetQueryable();
        IPagedList<T> GetPageOf(int pagenumber, int pagesize); 
        IQueryable<T> GetTable();

    }
}
