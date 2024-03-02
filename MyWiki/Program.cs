using MyWiki.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();


var app = builder.Build();

app.ConfigureApplication();

// app.UseHttpsRedirection();//无需更改特性，强制使用 HTTPS
app.Run();

/*
dotnet ef migrations add Init
dotnet ef database update
*/

//tree -I "bin|obj|uploads|Migrations|wwwroot"

//dotnet tool update --global dotnet-ef
