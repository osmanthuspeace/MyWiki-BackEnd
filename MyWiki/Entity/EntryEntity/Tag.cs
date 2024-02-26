using System.ComponentModel.DataAnnotations;

namespace MyWiki.Entity.EntryEntity;

public class Tag
{
    [Key]
    public int? TagId { get; set; }
    public string TagName { get; set; }

    public ICollection<Entry>? Entries { get; set; }
}