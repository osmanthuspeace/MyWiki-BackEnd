using MyWiki.Models.Dtos;

namespace MyWiki.Service.Interface;

public interface IUserDataProvider
{
    public Task<string> Register(LoginDto loginDto);
    public Task<string> Login(LoginDto loginDto);
    public Task<string> SetAdmin(string name);
    public Task<IEnumerable<string>> GetUsersByRole(string roleName);
    public Task<List<DisplayUsersDto>> GetUsers();
    public Task<bool> CheckUserName(string name);
    public Task<DisplayUsersDto> GetUserInfoByName(string name);
}