using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyWiki.Exceptions.Exceptions;

namespace MyWiki.Exceptions.ExceptionHandlers;

public sealed class DataBaseNotFoundExceptionHandler : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case DataBaseNotFoundException:
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                httpContext.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Detail = exception.Message,
                    Title = "Not Found"
                }, cancellationToken);
                return ValueTask.FromResult(true);
            case InvalidOperationException { Message: "" }:
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                httpContext.Response.WriteAsJsonAsync(
                    new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "Not Found",
                        Detail = exception.Message,
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5"
                    },
                    cancellationToken
                );
                return ValueTask.FromResult(true);
            default:
                return ValueTask.FromResult(false);
        }
    }
}