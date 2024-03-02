namespace MyWiki.Models.Dtos;

public class EntryDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string? CategoryName { get; set; }
    public List<string> TagNames { get; set; } = [];
}