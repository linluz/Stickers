using System.Linq.Expressions;
using Stickers.Common;
using Stickers.Common.DataAccess;

namespace Stickers.DataAccess.InMemory
{
    public class InMemoryDataAccessor : ISimplifiedDataAccessor
    {
        private List<IEntity> _entities = [];
       
        public InMemoryDataAccessor() { }
        
        public InMemoryDataAccessor(IEnumerable<IEntity> entities) => _entities.AddRange(entities);

        public async Task<int> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class, IEntity
            => await Task.Run(() =>
            {
                entity.Id = (_entities.LastOrDefault()?.Id) ?? 0 + 1;
                _entities.Add(entity);
                return entity.Id;
            }, cancellationToken);

        public async Task<bool> RemoveByIdAsync<TEntity>(int id, CancellationToken cancellationToken = default) where TEntity : class, IEntity
            => await Task.Run(() =>
            {
                var index = _entities.FindIndex(t => t.Id == id);
                if (index == -1)
                    return false;
                _entities.RemoveAt(index);
                return true;
            }, cancellationToken);

        public async Task<TEntity?> GetByIdAsync<TEntity>(int id, CancellationToken cancellationToken = default) where TEntity : class, IEntity
            => await Task.Run(
                () => (TEntity?)_entities.FirstOrDefault(t => t.Id == id),
                cancellationToken);

        public async Task<bool> UpdateAsync<TEntity>(int id, TEntity entity, CancellationToken cancellationToken = default) where TEntity : class, IEntity
            => await Task.Run(() =>
            {
                var index = _entities.FindIndex(t => t.Id == id);
                if (index == -1)
                    return false;
                entity.Id = id;
                _entities[index] = entity;
                return true;
            }, cancellationToken);

        public async Task<Paginated<TEntity>> GetPaginatedEntitiesAsync<TEntity, Tfield>(Expression<Func<TEntity, Tfield>> orderExpression, bool sortAscending = true,
            int pageSize = 25, int pageNumber = 0, Expression<Func<TEntity, bool>>? filterExpression = null, CancellationToken cancellationToken = default) where TEntity : class, IEntity
            => await Task.Run(() =>
            {
                var resultSet = filterExpression is not null//过滤
                    ? _entities.Cast<TEntity>().Where(filterExpression.Compile())
                    : _entities.Cast<TEntity>();
                var enumerableResultSet = resultSet.ToList();
                var totalCount = enumerableResultSet.Count;
                var orderedResultSet = sortAscending//排序
                    ? enumerableResultSet.OrderBy(orderExpression.Compile())
                    : enumerableResultSet.OrderByDescending(orderExpression.Compile());

                return new Paginated<TEntity>()
                {
                    Item = orderedResultSet.Skip(pageNumber * pageSize).Take(pageSize).ToList(),
                    PageIndex = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (totalCount - 1) / pageSize + 1
                };
            }, cancellationToken);

        public async Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken = default) where TEntity : class, IEntity
            => await Task.Run(
                () => _entities.Cast<TEntity>().Count(filterExpression.Compile()) > 0,//过滤后数量大于0
                cancellationToken);
    }
}