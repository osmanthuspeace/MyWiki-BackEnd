using Microsoft.AspNetCore.Mvc;
using MyWiki.Service.Interface;

namespace MyWiki.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class PictureController(IPictureProvider pictureProvider, ILogger<PictureController> logger) : ControllerBase
{
    //POST:上传图片
    [HttpPost]
    public async Task<ActionResult<string>> UploadPicture(IFormFile pic)
    {
        try
        {
            return Ok(await pictureProvider.UploadPicture(pic));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "error");
            // 返回一个通用的服务器错误响应
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An unexpected error occurred." });
        }
    }

    //GET:通过Id获取图片
    [HttpGet]
    public async Task<ActionResult<string>> GetPictureById(int id)
    {
        try
        {
            return Ok(await pictureProvider.GetPictureById(id));
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}