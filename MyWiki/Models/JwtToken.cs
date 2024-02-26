namespace MyWiki.Models;

//储存Token中的信息
public abstract class JwtToken
{
    public const string Issuer = "WebAPI";//颁发者        
    public const string Audience = "Client";//接收者    
    public const string Secret = "2E-9D-12-A1-61-32-7B-4D-F3-E4-5C-22-1A-87-8B-EE-84-0B-D8-3D-2A-9F-71-CA-3A-B2-4F-7E-03-6F-53-19";//签名秘钥        
    public const int AccessExpiration = 30;//AccessToken过期时间（分钟）
    
}