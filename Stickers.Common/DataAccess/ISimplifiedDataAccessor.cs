using System.Linq.Expressions;

namespace Stickers.Common.DataAccess
{
    /// <summary>
    /// 简易数据读取接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ISimplifiedDataAccessor
    {
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>新增数据的ID</returns>
        Task<int> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        /// <summary>
        /// 按ID移除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns>是否删除成功</returns>
        Task<bool> RemoveByIdAsync<TEntity>(int id, CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        /// <summary>
        /// 按ID获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns>获取的数据</returns>
        Task<TEntity?> GetByIdAsync<TEntity>(int id, CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        /// <summary>
        /// 按ID更新数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns>是否更新成功</returns>
        Task<bool> UpdateAsync<TEntity>(int id, TEntity entity, CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        /// <summary>
        /// 获取数据分页
        /// </summary>
        /// <typeparam name="Tfield"></typeparam>
        /// <param name="orderExpression">排序表达式</param>
        /// <param name="sortAscending">升序排序</param>
        /// <param name="pageSize">页尺寸</param>
        /// <param name="pageNumber">页数</param>
        /// <param name="filterExpression">过滤表达式</param>
        /// <returns>分页数据</returns>
        Task<Paginated<TEntity>> GetPaginatedEntitiesAsync<TEntity, Tfield>(Expression<Func<TEntity, Tfield>> orderExpression, bool sortAscending = true, int pageSize = 25, int pageNumber = 0, Expression<Func<TEntity, bool>>? filterExpression = null, CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        /// <summary>
        /// 按过滤条件判断是否有数据存在
        /// </summary>
        /// <param name="filterExpression">过滤条件</param>
        /// <returns>数据是否存在</returns>
        Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> filterExpression, CancellationToken cancellationToken = default) where TEntity : class, IEntity;
    }
}