using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MyWiki.Entity.UserEntity;
using MyWiki.Models;

namespace MyWiki.Service;

//生成JWT的Token

public abstract class JwtGenerator
{
    public static string GenerateJwt(User userInfo)
    {
        
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(JwtToken.Secret)), 
            SecurityAlgorithms.HmacSha256
            );//为JWT创建签名凭证
        
        var claims = new[]//添加声明信息，即关于用户的信息
        {
            new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
            new Claim(ClaimTypes.Name, userInfo.Name),
            new Claim(ClaimTypes.Role,userInfo.Role.RoleName),
        };
        
        var token = new JwtSecurityToken(
            issuer:JwtToken.Issuer, 
            audience:JwtToken.Audience, 
            claims:claims,//在JWT令牌中包含声明信息，用于验证身份
            expires: DateTime.Now.AddMinutes(JwtToken.AccessExpiration), 
            signingCredentials: credentials//告诉JWT生成器要使用哪个密钥和签名算法来签署生成的 JWT
        );
        return new JwtSecurityTokenHandler().WriteToken(token);//把token转换为string类型
    }
}
