using System.ComponentModel.DataAnnotations;
using MyWiki.Entity.EntryEntity;

namespace MyWiki.Entity.TagEntity;

public class Tag
{
    [Key]
    public int? TagId { get; set; }

    public string TagName { get; set; }

    public ICollection<Entry>? Entries { get; set; }
}
