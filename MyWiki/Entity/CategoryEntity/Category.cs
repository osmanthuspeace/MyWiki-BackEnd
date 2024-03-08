using System.ComponentModel.DataAnnotations;
using MyWiki.Entity.EntryEntity;

namespace MyWiki.Entity.CategoryEntity;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    public string CategoryName { get; set; }

    public string? Description { get; set; }

    public int? ParentCategoryId { get; set; }
    public Category ParentCategory { get; set; }
    public List<Category> ChildrenCategories { get; set; } = [];

    public List<Entry> Entries { get; set; } = [];
}
