using MyWiki.Exceptions.ExceptionHandlers;

namespace MyWiki.Extensions;

public static class ServiceCollectionExtension//扩展方法只能在非泛型、非嵌套的静态类中声明
{
    public static IServiceCollection ConfigureExceptionHandlers(
        this IServiceCollection services
    )
    {
        services.AddExceptionHandler<DataBaseNotFoundExceptionHandler>();
        return services;
    }
}