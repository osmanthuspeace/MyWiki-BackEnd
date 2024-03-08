using MyWiki.Entity.PictureEntity;
using MyWiki.Entity.PictureEntity.Repository;
using MyWiki.Service.Interface;

// 引入仓储层的命名空间

namespace MyWiki.Service.PictureService;

//重点在于IPictureRepository pictureRepository这一句,这个变量可以引用任何实现了 IPictureRepository 接口的对象，提高了代码的抽象层次。
public class PictureProvider(
    IPictureRepository pictureRepository,
    IWebHostEnvironment hostEnvironment
) : IPictureProvider
{
    //前端判断文件是否为空
    public async Task<Picture> UploadPicture(IFormFile pic)
    {
        var rootPath = hostEnvironment.WebRootPath;
        if (!File.Exists(rootPath))
            Directory.CreateDirectory(rootPath);
        var uploadPath = Path.Combine(rootPath, "uploads");
        if (!File.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);
        var picName = $"{Guid.NewGuid()}{Path.GetExtension(pic.FileName)}";

        var picPath = Path.Combine(uploadPath, picName);

        await using (var stream = new FileStream(picPath, FileMode.Create))
        {
            await pic.CopyToAsync(stream);
        }

        var pictureUrl = new Uri(picPath, UriKind.Absolute);
        var picture = new Picture { PictureUrl = pictureUrl };

        return await pictureRepository.UploadPictureAsync(picture);
    }

    public async Task<string> GetPictureById(int id)
    {
        return await pictureRepository.GetPictureUrlByIdAsync(id);
    }

    public Task<bool> RemovePicture(int id)
    {
        throw new NotImplementedException();
    }
}
