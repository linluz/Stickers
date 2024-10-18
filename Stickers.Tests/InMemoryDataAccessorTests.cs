using Stickers.DataAccess.InMemory;

namespace Stickers.Tests;

public class InMemoryDataAccessorTests
{
    internal IEnumerable<MyEntity> Entities(int count)
    {
        for (int i = 1; i <= count; i++)
            yield return new MyEntity { Id = i, Name = $"Test{i}" };
    }

    [SetUp]
    public void Setup()
    {
    }
    /// <summary>
    /// 测试添加数据
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task AddTest()
    {
        var acc = new InMemoryDataAccessor();
        var entity = new MyEntity { Name = "Test" };
        var id = await acc.AddAsync(entity);

        Assert.Multiple(() =>
        {
            Assert.That(entity.Id, Is.EqualTo(1));
            Assert.That(id, Is.EqualTo(1));
        });
    }
    /// <summary>
    /// 测试移除数据成功
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task Remove1Test()
    {
        var acc = new InMemoryDataAccessor(Entities(3));
        var result = await acc.RemoveByIdAsync<MyEntity>(1);
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// 测试移除数据失败
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task Remove2Test()
    {
        var acc = new InMemoryDataAccessor(Entities(3));
        var result = await acc.RemoveByIdAsync<MyEntity>(4);
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// 按ID获取成功
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task GetById1Test()
    {
        var acc = new InMemoryDataAccessor(Entities(3));
        var result = await acc.GetByIdAsync<MyEntity>(2);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Test2"));
        });
    }

    /// <summary>
    /// 按ID获取失败
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task GetById2Test()
    {
        var acc = new InMemoryDataAccessor(Entities(3));
        var result = await acc.GetByIdAsync<MyEntity>(4);
        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// 测试获取分页数据1
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task GetPaginated1Test()
    {
        var acc = new InMemoryDataAccessor(Entities(10));
        var result = await acc.GetPaginatedEntitiesAsync<MyEntity, object>(
            x => x.Name, false, 3);
        Assert.Multiple(() =>
        {
            Assert.That(result.PageIndex, Is.EqualTo(0));
            Assert.That(result.PageSize, Is.EqualTo(3));
            Assert.That(result.TotalCount, Is.EqualTo(10));
            Assert.That(result.Item.Count, Is.EqualTo(3));
            Assert.That(result.Item[0].Name, Is.EqualTo("Test9"));
            Assert.That(result.Item[1].Name, Is.EqualTo("Test8"));
            Assert.That(result.Item[2].Name, Is.EqualTo("Test7"));
        });
    }

    /// <summary>
    /// 测试获取分页数据2 一页不完整,带过滤表达式
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task GetPaginated2Test()
    {
        var acc = new InMemoryDataAccessor(Entities(10));
        var result = await acc.GetPaginatedEntitiesAsync<MyEntity, object>(
            x => x.Name,
            false,
            3,
            1,
            x => x.Id % 2 == 0);
        Assert.Multiple(() =>
        {
            Assert.That(result.PageIndex, Is.EqualTo(1));
            Assert.That(result.PageSize, Is.EqualTo(3));
            Assert.That(result.TotalCount, Is.EqualTo(5));
            Assert.That(result.Item.Count, Is.EqualTo(2));
            Assert.That(result.Item[0].Name, Is.EqualTo("Test2"));
            Assert.That(result.Item[1].Name, Is.EqualTo("Test10"));
        });
    }

    /// <summary>
    /// 更新数据测试
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task UpdateTest()
    {
        var acc = new InMemoryDataAccessor(Entities(10));
        var updated = new MyEntity { Name = "daxnet" };
        var result = await acc.UpdateAsync(5, updated);
        var res = await acc.GetByIdAsync<MyEntity>(5);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(res, Is.Not.Null);
            Assert.That(res!.Name, Is.EqualTo("daxnet"));
        });
    }

    /// <summary>
    /// 存在测试
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task ExistsTest()
    {
        var acc = new InMemoryDataAccessor(Entities(10));
        var result = await acc.ExistsAsync<MyEntity>(x => x.Name == "Test7");
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// 不存在测试
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task NotExistsTest()
    {
        var acc = new InMemoryDataAccessor(Entities(10));
        var result = await acc.ExistsAsync<MyEntity>(x => x.Name == "daxnet");
        Assert.That(result, Is.False);
    }
}