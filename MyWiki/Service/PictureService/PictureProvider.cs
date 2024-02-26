using Microsoft.EntityFrameworkCore;
using MyWiki.Application;
using MyWiki.Data;
using MyWiki.Entity;

namespace MyWiki.Service.PictureService;

public class PictureProvider(WikiContext context,IWebHostEnvironment hostEnvironment):IPictureProvider
{
    //POST:上传图片
    public async Task<Picture> UploadPicture(IFormFile pic)
    {
        if (pic.Length == 0) throw new Exception("please upload a picture");
        var rootPath = hostEnvironment.WebRootPath;
        var picName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(pic.FileName)}";
        var picPath = Path.Combine(rootPath, "uploads", picName);
        await using var stream = new FileStream(picPath, FileMode.Create);
        await pic.CopyToAsync(stream);
        var picture = new Picture { PictureUrl = picPath }; 
        context.Pictures.Add(picture); 
        await context.SaveChangesAsync();
        return picture;//Save之后，Id会被数据库自动填充
    }
    //GET:根据Id获取图片
    public async Task<string> GetPictureById(int id)
    {
        var existPicture = await context.Pictures.FirstOrDefaultAsync(p => p.PictureId == id);
        if (existPicture == null) throw new Exception("There is no picture for this id");
        return existPicture.PictureUrl;
    }
}