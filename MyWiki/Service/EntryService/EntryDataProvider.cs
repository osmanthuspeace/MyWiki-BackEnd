using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyWiki.Data;
using MyWiki.Entity.EntryEntity;
using MyWiki.Models.Dtos;
using MyWiki.Service.Interface;

namespace MyWiki.Service.EntryService;

public class EntryDataProvider(WikiContext context):IEntryDataProvider
{
    // 通过Id获取单个词条
    public async Task<EntryDto> GetEntryById(int id)
    {
        var entry = await context.Entries
            // .Include(e=>e.Category)
            .Include(e=>e.Tags)
            .FirstOrDefaultAsync(e=>e.EntryId==id);
        if (entry == null) throw new Exception("There is no entry for this id");
        return new EntryDto
        {
            Id = entry.EntryId,
            Title = entry.Title,
            Content = entry.Content,
            CategoryName = "",
            TagNames = entry.GetTagNames().ToList()
        };
    }
    
    // 通过Title获取词条
    public async Task<List<EntryDto>> GetEntriesByTitle(string title,int page,int pageSize)
    {
        var skipCount = (page - 1) * pageSize;
        if (title.IsNullOrEmpty()) throw new Exception("please specify the title");
        var entries = await context.Entries
            .Where(entry => entry.Title.Contains(title))
            .OrderBy(e=>e)
            .Include(e=>e.Tags)
            // .Include(entry => entry.Category)
            .Skip(skipCount)
            .Take(pageSize)
            .ToListAsync();
        if(entries.Count == 0) throw new Exception("There is no entry with this title");
        return entries.Select(e => new EntryDto
        {
            Id = e.EntryId,
            Title = e.Title,
            Content = e.Content,
            CategoryName = "",
            TagNames = e.GetTagNames().ToList()
        }).ToList();
    }
    
    // 通过Tags获取词条
    public async Task<ICollection<EntryDto>> GetEntriesByTags([FromQuery]List<string>? tagNames,int page,int pageSize)
    {
        var skipCount = (page - 1) * pageSize;
        if (tagNames.IsNullOrEmpty()) throw new Exception("please specify the tags");
        var entries = await context.Entries
            .Where(entry => entry.Tags.Any(tag => tagNames!.Contains(tag.TagName)))
            .OrderByDescending(entry => entry.Tags.Count)
            // .Include(entry => entry.Category!)
            .Include(e=>e.Tags)
            .Skip(skipCount)
            .Take(pageSize)
            .ToListAsync();
        if (entries.Count==0) throw new Exception("There is no entry under these tags");
        return entries.Select(entry => new EntryDto
        {
            Id = entry.EntryId,
            Title = entry.Title,
            Content = entry.Content,
            CategoryName = "",
            TagNames = entry.GetTagNames().ToList()
        }).ToList();
    }
    
    // 通过Category获取词条
    public async Task<ICollection<EntryDto>> GetEntriesByCategory(string categoryName,int page,int pageSize)
    {

        var skipCount = (page - 1) * pageSize;
        if (categoryName.IsNullOrEmpty()) throw new Exception("please specify the category");

        var existCategory = await context.Categories
            .Where(c => c.CategoryName == categoryName)
            .ToListAsync();
        if (existCategory == null) throw new Exception("There is no such category");
        
        var entries = await context.Entries
            .Where(entry => entry.Category!.CategoryName == categoryName)
            .Include(e=>e.Tags)
            // .Include(e=>e.Category)
            .Skip(skipCount)
            .Take(pageSize)
            .ToListAsync();
        if (entries.Count==0) throw new Exception("There are no entries under this category");
        return entries.Select(entry => new EntryDto
        {
            Id = entry.EntryId,
            Title = entry.Title,
            Content = entry.Content,
            CategoryName = "",
            TagNames = entry.GetTagNames().ToList()
        }).ToList();
    }

    // 展示所有词条
    public async Task<IEnumerable<EntryDto>> GetEntries(int page,int pageSize)
    {
        var skipCount = (page - 1) * pageSize;
        var entries=await context.Entries
            // .Include(entry => entry.Category!)
            .OrderBy(e=>e)
            .Include(e=>e.Tags)
            .Skip(skipCount)
            .Take(pageSize)
            .ToListAsync();
        if(entries == null || entries.Count == 0)throw new Exception("No entry!");
        return entries.Select(entry => new EntryDto
        {
            Id = entry.EntryId,
            Title = entry.Title,
            Content = "",
            CategoryName = "",
            TagNames = entry.GetTagNames().ToList()
        }).ToList();
    }
    
    // 新建词条
    public async Task<string> PostEntry(EntryDto entryDto)
    {
        //检查是否有已存在的词条
        var previousEntry = await context.Entries.FirstOrDefaultAsync(e => e.Title == entryDto.Title);
        
        //有，就删掉（覆盖）
        if (previousEntry != null) throw new Exception("The entry has already exist!");
        
        var tagsInDto = entryDto.TagNames;//取出请求体中的Tag列表
        var entry = new Entry
        {
            Title = entryDto.Title,
            Content = entryDto.Content,
            Category = null
        };
        
        //检查Dto的标签中是否有数据库中已经存在的标签，如果有，则把那些标签取出来
        var existingTags = await context.Tags
            .Where(tagInDataBase => tagsInDto.Contains(tagInDataBase.TagName))
            .ToListAsync(); //EF Core开始跟踪
        
        foreach (var tagInDto in tagsInDto)
        {
            //检查数据库中已经存在的标签中的哪一个是当前的tag
            var existingTag = existingTags.FirstOrDefault(tag => tag.TagName == tagInDto);
            var newTag = new Tag
            {
                TagName = tagInDto
            };
            if (existingTag == null)//如果数据库中没有这个标签，就存起来
            {
                entry.Tags.Add(newTag);
                newTag.Entries?.Add(entry);
            }
            else//如果有，则用现有的，并更新导航属性
            {
                entry.Tags.Add(existingTag);
                existingTag.Entries?.Add(entry);
            }
        }
        await context.Entries.AddAsync(entry);
        await context.SaveChangesAsync();
        return "Post success!";
    }
    
    // 编辑词条
    public async Task<string> UpdateEntry(EntryDto entryDto)
    {
        var entry = await context.Entries
            .Include(e => e.Tags)
            .FirstOrDefaultAsync(e => e.EntryId == entryDto.Id);
        if (entry == null) throw new Exception("The Entry isn't exist!");
        
        entry.Title = entryDto.Title;
        entry.Content = entryDto.Content;
        
        var tagsInDto = entryDto.TagNames;
        var existingTags = await context.Tags.ToListAsync();
        var updatedTags = new List<Tag>();
        foreach (var tagName in tagsInDto)
        {
            var tag = existingTags.FirstOrDefault(tag => tag.TagName == tagName);
            if (tag == null)
            {
                tag = new Tag { TagName = tagName };
                entry.Tags.Add(tag);
                tag.Entries?.Add(entry);
            }
            updatedTags.Add(tag);
        }
        entry.Tags.Clear();
        foreach (var tag in updatedTags)
        {
            entry.Tags.Add(tag); // 添加新的标签关联
        }
        await context.SaveChangesAsync();
        return "Update success!";
    }
    
    // 删除词条
    public async Task<string> DeleteEntry(string title)
    {
        var entry = await context.Entries.FirstOrDefaultAsync(e=>e.Title==title);
        if (entry == null) throw new Exception("There is no entry with this title");
        if(entry.ImgUrl!=null&& File.Exists(entry.ImgUrl))
            File.Delete(entry.ImgUrl);
        context.Entries.Remove(entry);
        await context.SaveChangesAsync();
        return "Delete success";
    }

    // 获取词条总数
    public int GetEntryTotal()
    {
        var total = context.Entries.Count();
        if (total == 0) throw new Exception("There is no entry in the database");
        return total;
    }

    // 获取所有标签
    public Task<List<Tag>> GetTags()
    {
        var tags = context.Tags
            .Select(t => t)
            .OrderBy(t => t.TagId)
            .ToListAsync();
        if (tags == null) throw new Exception("There is no entry");
        return tags;
    }

    private async Task<Category> GetOrCreateCategory(string categoryName)
    {
        var existCategory = await context.Categories.FirstOrDefaultAsync(c => c.CategoryName == categoryName);
        if (existCategory != null) return existCategory;
        var newCategory = new Category
        {
            CategoryName = categoryName,
            Description = ""
        };
        context.Categories.Add(newCategory);
        await context.SaveChangesAsync();
        return newCategory;
    }
}