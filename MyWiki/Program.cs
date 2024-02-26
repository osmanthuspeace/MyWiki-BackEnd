using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyWiki.Application;
using MyWiki.Data;
using MyWiki.Models;
using MyWiki.Service.EntryService;
using MyWiki.Service.PictureService;
using MyWiki.Service.UserService;

var builder = WebApplication.CreateBuilder(args);
var connectString = builder.Configuration.GetConnectionString("WikiContext");
builder.Services.AddControllers().AddJsonOptions(o=>
    o.JsonSerializerOptions.ReferenceHandler=ReferenceHandler.IgnoreCycles);
builder.Services.AddDbContext<WikiContext>(o =>
    o.UseNpgsql(connectString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policyBuilder => policyBuilder.WithOrigins("http://localhost:8080")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});//配置同源策略

builder.Services.AddScoped<IEntryDataProvider,EntryDataProvider>();
builder.Services.AddScoped<IUserDataProvider,UserDataProvider>();
builder.Services.AddScoped<IPictureProvider,PictureProvider>();

builder.Services.AddAuthentication(options=>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer( o =>
{
    o.TokenValidationParameters = new TokenValidationParameters//指定要验证什么，验证的标准是什么，以及用于验证签名的密钥是什么
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = JwtToken.Issuer,
        ValidAudience = JwtToken.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtToken.Secret))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseHttpsRedirection();//无需更改特性，强制使用 HTTPS
app.UseAuthorization();

app.UseCors("AllowSpecificOrigin");

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();

/*
dotnet ef migrations add Init
dotnet ef database update
*/

//tree -I "bin|obj|uploads|Migrations|wwwroot"

//dotnet tool update --global dotnet-ef
