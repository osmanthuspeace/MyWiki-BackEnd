using MyWiki.Entity.PictureEntity;

namespace MyWiki.Service.Interface;

public interface IPictureProvider
{
    public Task<Picture> UploadPicture(IFormFile pic);
    public Task<string> GetPictureById(int id);
    public Task<bool> RemovePicture(int id);
}
