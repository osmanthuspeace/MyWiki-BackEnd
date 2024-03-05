using MyWiki.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ConfigureLogger();

builder.ConfigureServices();

var app = builder.Build();

app.ConfigureApplication();

app.Run();

/*
dotnet ef migrations add Init
dotnet ef database update
*/

//tree -I "bin|obj|uploads|Migrations|wwwroot"

//dotnet tool update --global dotnet-ef