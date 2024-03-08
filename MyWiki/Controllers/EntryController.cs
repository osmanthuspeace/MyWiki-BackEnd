using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWiki.Entity.EntryEntity;
using MyWiki.Entity.TagEntity;
using MyWiki.Models.Dtos;
using MyWiki.Service.Interface;

namespace MyWiki.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class EntryController(
    IEntryDataProvider entryDataProviderService,
    ILogger<EntryController> logger
) : ControllerBase
{
    // GET: 通过Id获取词条
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Entry>> GetEntryById(int id)
    {
        try
        {
            return Ok(await entryDataProviderService.GetEntryById(id));
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    //GET: 通过Title获取词条
    [HttpGet("{title}")]
    public async Task<ActionResult<List<Entry>>> GetEntriesByTitle(
        string title,
        int page = 1,
        int pageSize = 10
    )
    {
        try
        {
            return Ok(await entryDataProviderService.GetEntriesByTitle(title, page, pageSize));
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    //GET: 通过Tags获取词条
    [HttpGet]
    public async Task<ActionResult<ICollection<Entry>>> GetEntriesByTags(
        [FromQuery] List<string>? tagNames,
        int page = 1,
        int pageSize = 10
    )
    {
        try
        {
            return Ok(await entryDataProviderService.GetEntriesByTags(tagNames, page, pageSize));
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    //GET: 通过Category获取词条
    [HttpGet]
    public async Task<ActionResult<ICollection<Entry>>> GetEntriesByCategory(
        string categoryName,
        int page = 1,
        int pageSize = 10
    )
    {
        try
        {
            return Ok(
                await entryDataProviderService.GetEntriesByCategory(categoryName, page, pageSize)
            );
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    //GET: 展示所有词条
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Entry>>> GetEntries(int page = 1, int pageSize = 10)
    {
        try
        {
            return Ok(await entryDataProviderService.GetEntries(page, pageSize));
        }
        catch (Exception ex)
        {
            return NotFound();
        }
    }

    //POST: 新建词条
    [HttpPost]
    public async Task<ActionResult<Entry>> PostEntry(EntryDto entryDto)
    {
        try
        {
            return Ok(await entryDataProviderService.PostEntry(entryDto));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //PUT: 编辑词条
    [HttpPut]
    public async Task<IActionResult> UpdateEntry(EntryDto entryDto)
    {
        try
        {
            return Ok(await entryDataProviderService.UpdateEntry(entryDto));
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    //DELETE: 删除词条
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteEntry(string title)
    {
        var staff = User.FindFirstValue(ClaimTypes.Role);
        if (staff is not "Admin")
            return Unauthorized("Only administrators have permission to delete entries");
        try
        {
            return Ok(await entryDataProviderService.DeleteEntry(title));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public IActionResult GetEntryTotal()
    {
        try
        {
            return Ok(entryDataProviderService.GetEntryTotal());
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<Tag>>> GetTags()
    {
        try
        {
            return Ok(await entryDataProviderService.GetTags());
        }
        catch (Exception e)
        {
            return NotFound(e);
        }
    }
}
