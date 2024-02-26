using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyWiki.Application;
using MyWiki.Data;
using MyWiki.Entity.UserEntity;
using MyWiki.Service.DtoService;

namespace MyWiki.Service.UserService;

public class UserDataProvider(WikiContext context):IUserDataProvider
{
    //注册
    public async Task<string> Register(LoginDto loginDto)
    {
        var hashedPassword = PasswordHasher.HashPassword(loginDto.Password, out var salt);
        var user = new User
        {
            Name = loginDto.Name,
            HashedPassword = hashedPassword,
            Salt = salt,
            Role=await GetOrCreateRole("User")
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return "Register success";
    }
    
    //登录
    public async Task<string> Login(LoginDto loginDto)
    {
        var existUser = await context.Users
            .Include(user => user.Role)
            .FirstOrDefaultAsync(u => u.Name == loginDto.Name);
        if (existUser is null) throw new Exception("The user is not exist");
        var isPasswordValid =
            PasswordHasher.VerifyPassword(loginDto.Password, existUser.HashedPassword, existUser.Salt);
        if (!isPasswordValid) throw new Exception("The account or password is incorrect");
        var user = new User
            {
                Name = loginDto.Name,
                Role = existUser.Role
            };
        var token = JwtGenerator.GenerateJwt(user);
        return token;
    }

    public async Task<string> SetAdmin(string name)
    {
        var targetUser=await context.Users
            .Include(user => user.Role)
            .FirstOrDefaultAsync(u => u.Name == name);
        if (targetUser == null) throw new Exception($"There is no user by the name of {name}");
        if (targetUser.Role.RoleName == "Admin") return "Already an administrator";
        var adminUser = await GetOrCreateRole("Admin");
        targetUser.Role=adminUser;
        await context.SaveChangesAsync();
        return "User Authority have been changed to administrator";
    }
    
    public async Task<IEnumerable<string>> GetUsersByRole(string roleName)
    {
        var users = await context.Users
            .Include(u=>u.Role)
            .Where(u => u.Role.RoleName == roleName)
            .ToListAsync();
        var userNames = users.Select(u => u.Name);
        if(users.IsNullOrEmpty())throw new Exception("No one belongs to this role");
        return userNames;
    }

    public async Task<List<DisplayUsersDto>> GetUsers()
    {
        var users =await context.Users.
                Include(u=>u.Role)
                .ToListAsync();
        return users.Select(u => new DisplayUsersDto
        {
            Name= u.Name,
            RoleName=u.Role.RoleName 
        }).ToList();
    }

    public async Task<bool> CheckUserName(string name)
    {
        if (name.IsNullOrEmpty()) return false;
        var existUser = await context.Users.FirstOrDefaultAsync(u => u.Name == name);
        if (existUser!=null) throw new Exception("该用户名已存在");
        return false;
    }
    
    private async Task<Role> GetOrCreateRole(string roleName)
    {
        var existingRole = await context.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
        if (existingRole != null) return existingRole;
        var newRole = new Role { RoleName = roleName };
        context.Roles.Add(newRole);
        await context.SaveChangesAsync();
        return newRole;
    }
}