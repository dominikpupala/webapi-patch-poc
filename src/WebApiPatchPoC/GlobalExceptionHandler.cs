using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApiPatchPoC;

internal sealed class GlobalExceptionHandler(IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            NotImplementedException ex => new ProblemDetails
            {
                Status = StatusCodes.Status501NotImplemented,
                Title = "Not Implemented",
                Detail = ex.Message,
                Instance = httpContext.Request.Path
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request",
                Detail = environment.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred",
                Instance = httpContext.Request.Path
            }
        };

        if (environment.IsDevelopment() && exception is not NotImplementedException)
        {
            problemDetails.Extensions["exceptionType"] = exception.GetType().FullName;
        }

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
