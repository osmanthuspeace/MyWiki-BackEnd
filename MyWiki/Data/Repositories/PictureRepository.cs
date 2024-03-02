using Microsoft.EntityFrameworkCore;
using MyWiki.Entity;

namespace MyWiki.Data.Repositories;

public class PictureRepository(WikiContext context) : IPictureRepository
{
    public async Task<Picture> UploadPictureAsync(IFormFile pic, string uploadPath)
    {
        if (pic.Length == 0) throw new InvalidOperationException("Please upload a picture.");

        var picName = $"{Guid.NewGuid()}{Path.GetExtension(pic.FileName)}";
        var picPath = Path.Combine(uploadPath, picName);

        await using (var stream = new FileStream(picPath, FileMode.Create))
        {
            await pic.CopyToAsync(stream);
        }

        var picture = new Picture { PictureUrl = picPath };
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        return picture;
    }

    public async Task<string> GetPictureUrlByIdAsync(int id)
    {
        var picture = await context.Pictures.FirstOrDefaultAsync(p => p.PictureId == id);

        if (picture == null) throw new InvalidOperationException("There is no picture for this ID.");

        return picture.PictureUrl;
    }
}