using MyWiki.Entity;

namespace MyWiki.Data.Repositories;

public interface IPictureRepository
{
    Task<Picture> UploadPictureAsync(IFormFile pic, string uploadPath);
    Task<string> GetPictureUrlByIdAsync(int id);
}