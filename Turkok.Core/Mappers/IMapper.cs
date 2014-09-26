using System.Collections.Generic;

namespace Turkok.Core.Mappers
{
    public interface IMapper<TEntity, TModel>
        where TEntity : class, new()
        where TModel : new()
    {
        TModel ToModel(TEntity entity);
        TEntity ToEntity(TModel model);

        IEnumerable<TModel> ToModels(IEnumerable<TEntity> entities);
        IEnumerable<TEntity> ToEntities(IEnumerable<TModel> models);
    }
}