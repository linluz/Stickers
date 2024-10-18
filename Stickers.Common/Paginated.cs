namespace Stickers.Common
{
    /// <summary>
    /// 分页信息
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Paginated<TEntity> where TEntity : IEntity
    {
        /// <summary>
        /// 当前页所含数据
        /// </summary>
        public List<TEntity> Item { get; set; } = [];
        
        /// <summary>
        /// 页数
        /// </summary>
        public int PageIndex { get; set; } = -1;

        /// <summary>
        /// 页尺寸
        /// </summary>
        public int PageSize { get; set; } = -1;

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; } = -1;

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; } = -1;
    }
}
