using System.ComponentModel.DataAnnotations;

namespace MyWiki.Entity.PictureEntity;

public class Picture
{
    private string _pictureUrl;

    [Key]
    public int PictureId { get; set; }

    public Uri PictureUrl
    {
        get => new(_pictureUrl);
        set => _pictureUrl = value.ToString();
    }
}
