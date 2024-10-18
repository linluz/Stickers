using System.Linq.Expressions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Stickers.Common.DataAccess;
using Stickers.WebApi.Models;

namespace Stickers.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class StickersController(ISimplifiedDataAccessor dac) : ControllerBase
{
    /// <summary>
    /// 按ID获取便签
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Sticker))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(int id)
        => await dac.GetByIdAsync<Sticker>(id) is { } sticker
            ? Ok(sticker)
            : NotFound($"未找到具有{id}的便签");

    /// <summary>
    /// 创建便签
    /// </summary>
    /// <param name="sticker"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(Sticker sticker)
    {
        var exists = await dac.ExistsAsync<Sticker>(s => s.Title == sticker.Title);
        if (exists)
            Conflict($"标题为{sticker.Title}的便签已存在");
        var id = await dac.AddAsync(sticker);
        return CreatedAtAction(nameof(GetByIdAsync), new { id }, sticker);
    }

    /// <summary>
    /// 按ID删除便签
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteByIdAsync(int id)
        => await dac.RemoveByIdAsync<Sticker>(id)
            ? NoContent()
            : NotFound($"未找到具有{id}的便签");

    /// <summary>
    /// 按ID更新便签
    /// </summary>
    /// <param name="id"></param>
    /// <param name="patchDocument"></param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateStickerAsync(int id,
        [FromBody] JsonPatchDocument<Sticker>? patchDocument)
    {
        if (patchDocument is null) return BadRequest();
        var sticker = await dac.GetByIdAsync<Sticker>(id);
        if (sticker is null) return NotFound();
        sticker.ModifiedOn = DateTime.UtcNow;
        patchDocument.ApplyTo(sticker, ModelState);
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await dac.UpdateAsync(id, sticker);
        return Ok(sticker);
    }

    /// <summary>
    /// 按条件获取便签
    /// </summary>
    /// <param name="sortField">用于排序的字段</param>
    /// <param name="ascending">是否升序排序，默认为真</param>
    /// <param name="pageSize">页尺寸，默认20</param>
    /// <param name="pageNumber">页码，默认0</param>
    /// <returns>返回分页数据</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStickerAsync(
        [FromQuery(Name = "sort")] string? sortField = null,
        [FromQuery(Name = "asc")] bool ascending = true,
        [FromQuery(Name = "size")] int pageSize = 20,
        [FromQuery(Name = "page")] int pageNumber = 0)
        => Ok(await dac.GetPaginatedEntitiesAsync(
            sortField is not null
                ? ConvertToExpression<Sticker, object>(sortField)
                : s => s.Id,
            ascending, pageSize, pageNumber));

    /// <summary>
    /// 按属性名生成排序表达式
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private static Expression<Func<TEntity, TProperty>> ConvertToExpression<TEntity, TProperty>(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentNullException($"{nameof(propertyName)}不能为null或空白。");
        var propertyInfo = typeof(TEntity).GetProperty(propertyName);
        if (propertyInfo is null) throw new ArgumentNullException($"属性{propertyName}未定义");
        var parameterExpression = Expression.Parameter(typeof(TEntity), "p");
        var memberExperession = Expression.Property(parameterExpression, propertyInfo);
        return propertyInfo.PropertyType.IsValueType
            ? Expression.Lambda<Func<TEntity, TProperty>>(Expression.Convert(memberExperession, typeof(object)), parameterExpression)
            : Expression.Lambda<Func<TEntity, TProperty>>(memberExperession, parameterExpression);
        //(object)TEntity.propertyName
    }

}