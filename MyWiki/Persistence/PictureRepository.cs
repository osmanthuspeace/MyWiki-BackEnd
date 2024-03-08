using Microsoft.EntityFrameworkCore;
using MyWiki.Data;
using MyWiki.Entity.PictureEntity;
using MyWiki.Entity.PictureEntity.Repository;

namespace MyWiki.Persistence;

public class PictureRepository(WikiContext context) : IPictureRepository
{
    public async Task<Picture> UploadPictureAsync(Picture picture)
    {
        context.Pictures.Add(picture);
        await context.SaveChangesAsync();

        return picture;
    }

    public async Task<string> GetPictureUrlByIdAsync(int id)
    {
        var picture = await context.Pictures.FirstOrDefaultAsync(p => p.PictureId == id);

        if (picture == null)
            throw new InvalidOperationException("There is no picture for this ID.");

        return picture.PictureUrl.ToString();
    }

    public async Task<bool> RemovePictureAsync(int id)
    {
        var picture = await context.Pictures.FirstOrDefaultAsync(p => p.PictureId == id);

        if (picture == null)
            throw new InvalidOperationException("There is no picture for this ID.");

        context.Remove(picture);

        return true;
    }
}
