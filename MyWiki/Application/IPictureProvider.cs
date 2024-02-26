using MyWiki.Entity;

namespace MyWiki.Application;

public interface IPictureProvider
{
    public Task<Picture> UploadPicture(IFormFile pic);
    public Task<string> GetPictureById(int id);
}