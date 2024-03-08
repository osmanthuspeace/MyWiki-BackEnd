using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyWiki.Data;
using MyWiki.Entity.PictureEntity.Repository;
using MyWiki.Models;
using MyWiki.Persistence;
using MyWiki.Service.EntryService;
using MyWiki.Service.Interface;
using MyWiki.Service.PictureService;
using MyWiki.Service.UserService;
using Newtonsoft.Json;
using NLog.Extensions.Logging;
using Serilog;

namespace MyWiki.Extensions;

public static class WebApplicationBuilderExtension
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        var connectString = builder.Configuration.GetConnectionString("WikiContext");
        builder.Services.AddDbContext<WikiContext>(o => o.UseNpgsql(connectString));

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost:6379"; // 使用你的 Redis 服务器地址
            options.InstanceName = "SampleInstance_";
        });

        builder.Services.AddMemoryCache();
        builder.Logging.AddSerilog();
        builder.Services.AddLogging(l =>
        {
            // l.AddConsole();
            l.AddNLog();
            // l.SetMinimumLevel(LogLevel.Warning);
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowSpecificOrigin",
                policyBuilder =>
                    policyBuilder
                        .WithOrigins("http://localhost:8080")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
            );
        }); //配置同源策略

        builder.Services.AddScoped<IEntryDataProvider, EntryDataProvider>();
        builder.Services.AddScoped<IUserDataProvider, UserDataProvider>();
        builder.Services.AddScoped<IPictureProvider, PictureProvider>();

        builder.Services.AddScoped<IPictureRepository, PictureRepository>();

        builder
            .Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters //指定要验证什么，验证的标准是什么，以及用于验证签名的密钥是什么
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    // ValidateLifetime = true,
                    ValidIssuer = JwtToken.Issuer,
                    ValidAudience = JwtToken.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(JwtToken.Secret)
                    )
                };
                o.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() != typeof(SecurityTokenExpiredException))
                            return Task.CompletedTask;
                        context.Response.Headers.Append("Token-Expired", "true");
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync(
                            JsonConvert.SerializeObject(new { error = "The token is out-date" })
                        );
                    }
                    // 可以添加其他事件处理...
                };
            });
        builder.Services.AddAuthorization();
        builder.Services.ConfigureExceptionHandlers();
    }
}
