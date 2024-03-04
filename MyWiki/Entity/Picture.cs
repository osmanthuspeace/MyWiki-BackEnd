using System.ComponentModel.DataAnnotations;

namespace MyWiki.Entity;

public class Picture
{
    private string pictureUrl;
    [Key] public int PictureId { get; set; }

    public Uri PictureUrl
    {
        get => new(pictureUrl);
        set => pictureUrl = value.ToString();
    }
}