using MyWiki.Exceptions.ExceptionHandlers;

namespace MyWiki.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection ConfigureExceptionHandlers(
        this IServiceCollection services
    )
    {
        services.AddExceptionHandler<DataBaseNotFoundExceptionHandler>();
        return services;
    }
}