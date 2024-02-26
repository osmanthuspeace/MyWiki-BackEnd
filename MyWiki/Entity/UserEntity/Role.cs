using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyWiki.Entity.UserEntity;

public class Role
{
    [Key]
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    [JsonIgnore]
    public ICollection<User> Users { get; set; } = [];
}