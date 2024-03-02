using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyWiki.Entity.UserEntity;

//用户
public class User
{
    [Key] public int Id { get; set; }

    [MaxLength(255)] public string Name { get; set; } = string.Empty;

    public string HashedPassword { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public int? RoleId { get; set; } // 外键属性

    [JsonIgnore] public Role Role { get; set; } // 导航属性
}