using System.Collections.Generic;
using System.Linq;
using Omu.ValueInjecter;

namespace Turkok.Core.Mappers
{
    public class Mapper<TEntity,TModel> : IMapper<TEntity, TModel> where TEntity : class, new() where  TModel: class, new()
    {
        public TModel ToModel(TEntity entity)
        {
            var model = new TModel();

            model.InjectFrom(entity);

            return model;
        }

        public TEntity ToEntity(TModel model)
        {
            var entity = new TEntity();

            entity.InjectFrom(model);

            return entity;
        }

        public IEnumerable<TModel> ToModels(IEnumerable<TEntity> entities)
        {
            var models = entities.Select(e => new TModel().InjectFrom(e)).Cast<TModel>();

            return models;
        }

        public IEnumerable<TEntity> ToEntities(IEnumerable<TModel> models)
        {
            var entities = models.Select(m => new TEntity().InjectFrom(m)).Cast<TEntity>();

            return entities;    
        }
          
    }
}