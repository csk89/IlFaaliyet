using System;
using System.Linq;
using PagedList;
using Turkok.Core.Repository;
using Turkok.Model;

namespace Turkok.Core.Service.CrudService
{
    public class BaseCrudService<T> : ICrudService<T> where T : BaseEntity, new()
    {
        private readonly IRepository<T> _repository;

        public BaseCrudService(IRepository<T> repository )
        {
            _repository = repository;
        } 

        public T Create(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }

            var instance = _repository.Add(item);

            return instance;
        }

        public T Find(int id)
        {
            var item = _repository.Find(id);

            if (item == null)
            {
                throw  new NullReferenceException();
            }

            return item;
        }

        public T Update(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }

            var instance = _repository.Update(item);

            return instance;
        }

        public T Delete(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }

            var instance = _repository.Remove(item);

            return instance;
        }

        public IQueryable<T> GetQueryable()
        {
            var items = _repository.Table().OrderByDescending(x => x.Id);

            return items;
        } 

        public IPagedList<T> GetPageOf(int pagenumber, int pagesize)
        {
            var items = _repository.Table().OrderByDescending(x => x.Id);

            var result = items.ToPagedList(pagenumber, pagesize);

            return result;
        }

        public IQueryable<T> GetTable()
        {
            var table = _repository.Table();

            return table;
        }
    }
}
