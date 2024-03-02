using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MyWiki.Service;

public class PasswordHasher
{
    public static string HashPassword(string password, out string salt)
    {
        // 使用随机数生成器生成一个随机的盐
        var saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes); //使用 RandomNumberGenerator 的实例rng 生成随机字节，然后将这些字节写入 saltBytes 数组中
        }

        salt = Convert.ToBase64String(saltBytes); //将字节数组转换为对应的 Base64 字符串表示形式

        // 使用PBKDF2进行密码哈希
        var hashedPassword = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password,
                saltBytes,
                KeyDerivationPrf.HMACSHA512, //伪随机函数（PRF）：HMAC-SHA-512
                10000, //迭代次数
                256 / 8 //期望生成的哈希值的字节数
            )
        );

        return hashedPassword;
    }

    public static bool VerifyPassword(string password, string hashedPassword, string salt)
    {
        // 将存储的盐解码为字节数组
        var saltBytes = Convert.FromBase64String(salt);

        // 使用PBKDF2验证密码
        var hashedInput = Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password,
                saltBytes,
                KeyDerivationPrf.HMACSHA512,
                10000,
                256 / 8
            )
        );

        // 比较两个哈希值是否相等
        return hashedInput == hashedPassword;
    }
}