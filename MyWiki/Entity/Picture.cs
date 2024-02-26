using System.ComponentModel.DataAnnotations;

namespace MyWiki.Entity;

public class Picture
{
    [Key]
    public int PictureId { get; set; }
    public string PictureUrl { get; set; }
}