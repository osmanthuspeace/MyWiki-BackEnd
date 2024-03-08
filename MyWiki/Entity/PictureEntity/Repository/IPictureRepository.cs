namespace MyWiki.Entity.PictureEntity.Repository;

public interface IPictureRepository
{
    Task<Picture> UploadPictureAsync(Picture picture);
    Task<string> GetPictureUrlByIdAsync(int id);
    Task<bool> RemovePictureAsync(int id);
}
