using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyWiki.Application;
using MyWiki.Service.DtoService;

namespace MyWiki.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class UserController(IUserDataProvider userDataProvider) : ControllerBase
{
    //POST: 注册
    [HttpPost]
    public async Task<ActionResult<string>> Register(LoginDto loginDto)
    {
        try
        {
            return Ok(await userDataProvider.Register(loginDto));

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    //POST: 设置管理员
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<string>> SetAdmin(string name)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (role is not "Admin") return Unauthorized("Only administrators have permission to set administer");
        try
        {
            return Ok(await userDataProvider.SetAdmin(name));
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    //POST: 登录
    [HttpPost]
    public async Task<ActionResult<string>> Login(LoginDto loginDto)
    {
        try
        {
            return Ok(await userDataProvider.Login(loginDto));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    //GET: 用过Role查找用户
    [HttpGet]
    public async Task<ActionResult<IEnumerable<string>>> GetUsersByRole(string roleName)
    {
        try
        {
            return Ok(await userDataProvider.GetUsersByRole(roleName));
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    //GET: 展示所有用户
    [HttpGet]
    public async Task<ActionResult<List<DisplayUsersDto>>> GetUsers()
    {
        try
        {
            return Ok(await userDataProvider.GetUsers());
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //GET: 检查用户名是否重复
    [HttpGet]
    public async Task<ActionResult<bool>> CheckUserName(string name)
    {
        try
        {
            return Ok(await userDataProvider.CheckUserName(name));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);//重复，则不能用这个用户名
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<DisplayUsersDto>> GetUserInfoByName(string name)
    {
        try
        {
            return Ok(await userDataProvider.GetUserInfoByName(name));
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
    }
    
    [HttpPost]
    [Authorize]
    public IActionResult CheckToken()
    {
        var staff = User.FindFirstValue(ClaimTypes.Role);
        if (staff == "Admin") return Ok();
        return Ok(staff);
    }
    
    //测试身份验证
    [HttpPost]//用get会返回405
    [Authorize]
    public ActionResult TestJwtVerification()
    {
        var userRole = User.FindFirstValue(ClaimTypes.Role); //这样可以获取到声明中的值
        return userRole switch
        {
            "Admin" => Ok("You are the Admin, and you have permission to delete entries and enable other to be the admin"),
            "User" => Ok("You are common users, and you have permission to add and edit entries"),
            "Visitor" => Ok("You are visitors, and you have permission to view entries"),
            _ => Ok("401")
        };
    }
}
/*
{
   "name": "oygp",
   "password": "123456"
}
*/