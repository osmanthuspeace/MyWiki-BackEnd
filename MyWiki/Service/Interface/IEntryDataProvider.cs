using Microsoft.AspNetCore.Mvc;
using MyWiki.Entity.EntryEntity;
using MyWiki.Models.Dtos;

namespace MyWiki.Service.Interface;

public interface IEntryDataProvider
{
    public Task<EntryDto> GetEntryById(int id);
    public Task<List<EntryDto>> GetEntriesByTitle(string title, int page, int pageSize);
    public Task<ICollection<EntryDto>> GetEntriesByTags([FromQuery] List<string>? tagNames, int page, int pageSize);
    public Task<ICollection<EntryDto>> GetEntriesByCategory(string categoryName, int page, int pageSize);
    public Task<IEnumerable<EntryDto>> GetEntries(int page, int pageSize);
    public Task<string> PostEntry(EntryDto entryDto);
    public Task<string> UpdateEntry(EntryDto entryDto);
    public Task<string> DeleteEntry(string title);
    public int GetEntryTotal();
    public Task<List<Tag>> GetTags();
}