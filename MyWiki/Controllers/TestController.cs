using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using MyWiki.Data;
using MyWiki.Entity.CategoryEntity;
using MyWiki.Entity.EntryEntity;

namespace MyWiki.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TestController(
    WikiContext context,
    ILogger<TestController> logger,
    IMemoryCache memoryCache,
    IDistributedCache cache
) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> TestCategory() // 改为返回Task<IActionResult>
    {
        var categories = await context.Categories.ToListAsync(); // 一次性加载所有分类
        var root = categories.FirstOrDefault(c => c.ParentCategoryId == null);
        Console.WriteLine(root.CategoryName);
        if (root != null)
            TestPrintTree(1, root, categories); // 传递所有分类到递归方法中
        return Ok(); // 应该返回一个ActionResult
    }

    private static void TestPrintTree(int level, Category parent, IEnumerable<Category> categories)
    {
        // 在内存中查找子分类，避免多次查询数据库
        var enumerable = categories as Category[] ?? categories.ToArray();
        var children = enumerable.Where(c => c.ParentCategoryId == parent.CategoryId);
        foreach (var child in children)
        {
            Console.WriteLine(new string('\t', level) + child.CategoryName);
            TestPrintTree(level + 1, child, enumerable);
        }
    }

    [ResponseCache(Duration = 20)]
    [HttpGet]
    public DateTime Now(int id)
    {
        return DateTime.Now;
    }

    [HttpGet]
    public async Task<Entry?> TestCache(int id)
    {
        logger.LogWarning("开始使用缓存");
        Random r = new();
        var item = await memoryCache.GetOrCreateAsync(
            "id为" + id + "的Entry",
            async e =>
            {
                // e.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);

                e.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5 + r.Next(1, 10));
                logger.LogWarning("缓存中没有，从数据库中取数据");
                return await context.Entries.FirstOrDefaultAsync(entry => entry.EntryId == id);
            }
        );
        logger.LogWarning("返回数据");
        return item;
    }

    [HttpGet]
    public async Task<string?> TestRedis(int id)
    {
        logger.LogWarning("开始使用缓存");
        Random r = new();
        var value = await cache.GetStringAsync("entry" + id);
        if (value == null)
        {
            logger.LogWarning("缓存未命中，重新从数据库中查找");
            var entry = await context.Entries.FirstOrDefaultAsync(e => e.EntryId == id);
            if (entry == null)
                throw new Exception("failed");
            value = JsonSerializer.Serialize(entry);
            await cache.SetStringAsync(
                "entry" + id,
                value,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // 缓存过期时间
                }
            );
        }

        JsonSerializer.Deserialize<Entry>(value);

        logger.LogWarning("返回数据");
        return value;
    }
}
