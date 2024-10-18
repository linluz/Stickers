using System.ComponentModel.DataAnnotations;
using Stickers.Common;

namespace Stickers.WebApi.Models;

/// <summary>
/// 便签
/// </summary>
/// <param name="title">标题</param>
/// <param name="content">内容</param>
public class Sticker(string title, string content) : IEntity
{
    /// <summary>
    /// ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    [Required][StringLength(50)] public string Title { get; set; } = title;

    /// <summary>
    /// 内容
    /// </summary>
    public string Content { get; set; } = content;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 修改时间
    /// </summary>
    public DateTime? ModifiedOn { get; set; }
}