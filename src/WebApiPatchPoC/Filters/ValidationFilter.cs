using FluentValidation;
using WebApiPatchPoC;

namespace WebApiPatchPoC.Filters;

internal class ValidationFilter<TRequest>(IValidator<TRequest> validator) : IEndpointFilter
{
    private readonly IValidator<TRequest> _validator = validator;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (_validator is null)
        {
            return await next(context);
        }

        var request = context.Arguments.OfType<TRequest>().First();
        var validationResult = await _validator.ValidateAsync(request, context.HttpContext.RequestAborted);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        return await next(context);
    }
}

internal static class ValidationFilterExtensions
{
    extension(RouteHandlerBuilder builder)
    {
        public RouteHandlerBuilder WithRequestValidation<TRequest>()
            => builder
            .AddEndpointFilter<ValidationFilter<TRequest>>()
            .ProducesValidationProblem();
    }
}
