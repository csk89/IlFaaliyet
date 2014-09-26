using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Turkok.Core.Repository
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly IDbContext _context;
        private bool _disposed;

        public BaseRepository(IDbContext context)
        {
            _context = context;
        }

        private IDbSet<TEntity> Collection
        {
            get { return _context.Set<TEntity>(); }
        }

        public IQueryable<TEntity> Table()
        {
            return Collection;
        }

        public TEntity Find(int id)
        {
            TEntity entity = Collection.Find(id);

            if (ReferenceEquals(entity, null))
            {
                return default(TEntity);
            }

            return entity;
        }

        public int Upsert(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }
            
            Collection.AddOrUpdate(entity);
            int result =  _context.SaveChanges();
             
            return result;
        }

        public TEntity Add(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            TEntity addedEntity = Collection.Add(entity);

            _context.SaveChanges();

            return addedEntity;
        }

        public TEntity Remove(int id)
        {
            TEntity entity = Find(id);

            if (entity == null)
            {
                throw new NullReferenceException("Entity is null/Not in collection");
            }

            return Remove(entity);
        }

        public TEntity Remove(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            TEntity removedEntity = Collection.Remove(entity);

            _context.SaveChanges();

            return removedEntity;
        }

        public TEntity Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            Collection.Attach(entity);

            _context.Entry(entity).State = EntityState.Modified;

            _context.SaveChanges();

            return entity;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}