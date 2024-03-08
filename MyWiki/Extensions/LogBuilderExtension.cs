using Serilog;
using Serilog.Formatting.Compact;

namespace MyWiki.Extensions;

public static class LogBuilderExtension
{
    public static void ConfigureLogger(this ILoggingBuilder builder)
    {
        builder.ClearProviders();
        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            // .WriteTo.Console(new JsonFormatter())
            .WriteTo.File(
                new CompactJsonFormatter(),
                "/Users/ouyangguiping/Desktop/vueee/MyWiki/BackEnd/MyWiki/MyWiki/logs/serilogs"
            )
            .CreateLogger();
        builder.AddSerilog(logger);
    }
}
