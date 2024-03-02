using MyWiki.Data.Repositories;
using MyWiki.Entity;
using MyWiki.Service.Interface;

// 引入仓储层的命名空间

namespace MyWiki.Service.PictureService;

//重点在于IPictureRepository pictureRepository这一段,这个变量可以引用任何实现了 IPictureRepository 接口的对象，提高了代码的抽象层次。
public class PictureProvider(IPictureRepository pictureRepository, IWebHostEnvironment hostEnvironment)
    : IPictureProvider
{
    public async Task<Picture> UploadPicture(IFormFile pic)
    {
        var rootPath = hostEnvironment.WebRootPath;
        var uploadPath = Path.Combine(rootPath, "uploads");

        return await pictureRepository.UploadPictureAsync(pic, uploadPath);
    }

    public async Task<string> GetPictureById(int id)
    {
        return await pictureRepository.GetPictureUrlByIdAsync(id);
    }
}