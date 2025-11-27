using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;
using System.Text.Json;
using WebApiPatchPoC.Filters;
using WebApiPatchPoC.Utility;

namespace WebApiPatchPoC.Features.Products.PatchProduct;

internal static class PatchProductEndpoint
{
    public sealed record Request(string Sku, JsonElement PatchDocument);

    public sealed class PatchProductValidator : AbstractValidator<Request>
    {
        public PatchProductValidator()
        {
            RuleFor(x => x.Sku)
                .NotEmpty()
                .WithMessage("SKU is required")

                .MaximumLength(50)
                .WithMessage("SKU cannot exceed 50 characters")

                .Must(sku => !string.IsNullOrWhiteSpace(sku))
                .WithMessage("SKU cannot be empty or whitespace")

                .Matches(@"^[\x20-\x7E]*$")
                .WithMessage("SKU can only contain ASCII printable characters")

                .Must(sku => sku.Trim() == sku)
                .WithMessage("SKU cannot have leading or trailing whitespace");
        }
    }

    private sealed record class PatchFields
    {
        public string? Name { get; set; }
        public string? ImgUri { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
    }

    extension(IEndpointRouteBuilder endpoints)
    {
        public void MapPatchProduct()
        {
            endpoints.MapPatch("/{sku}", static async ([AsParameters] Request request, PatchProductHandler handler) =>
            {
                PatchFields patchFields;
                HashSet<string> setFields;

                try
                {
                    (patchFields, setFields) = PatchMapper.Map<PatchFields>(request.PatchDocument);
                }
                catch (Exception ex) when (ex is InvalidOperationException or JsonException)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid patch document",
                        detail: ex.Message);
                }

                if (!ValidatePatchFields(patchFields, setFields, out var errors))
                {
                    return Results.ValidationProblem(errors);
                }

                var command = CreateCommand(request.Sku, patchFields, setFields);

                var result = await handler.Handle(command);

                return result.Match(
                    success => Results.NoContent(),
                    notFound => Results.NotFound());
            })
            .WithName("PatchProduct")
            .WithSummary("Partially update a product using JSON Merge Patch")
            .WithDescription("Updates product fields using JSON Merge Patch (RFC 7396). Any product field can be updated except SKU. Fields not present are not updated. To clear a field, explicitly set it to null.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .WithRequestValidation<Request>();
        }
    }

    private static bool ValidatePatchFields(PatchFields patchFields, HashSet<string> setFields, out IDictionary<string, string[]> errors)
    {
        var validator = new InlineValidator<PatchFields>();

        if (setFields.Contains(nameof(PatchFields.Name)))
        {
            validator
                .RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name cannot be empty")

                .MaximumLength(200)
                .WithMessage("Name must not exceed 200 characters");
        }

        if (setFields.Contains(nameof(PatchFields.ImgUri)))
        {
            validator
                .RuleFor(x => x.ImgUri)
                .NotEmpty()
                .WithMessage("ImgUri cannot be empty")

                .MaximumLength(500)
                .WithMessage("ImgUri must not exceed 500 characters")

                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("ImgUri must be a valid absolute URI");
        }

        if (setFields.Contains(nameof(PatchFields.Price)))
        {
            validator
                .RuleFor(x => x.Price)
                .NotNull()
                .WithMessage("Price cannot be null")

                .GreaterThanOrEqualTo(0)
                .WithMessage("Price must be greater than or equal to 0");
        }

        if (setFields.Contains(nameof(PatchFields.Description)))
        {
            validator.When(x => x.Description is not null, () =>
            {
                validator.RuleFor(x => x.Description)
                    .MaximumLength(2000)
                    .WithMessage("Description must not exceed 2000 characters");
            });
        }

        var validationResult = validator.Validate(patchFields);

        if (!validationResult.IsValid)
        {
            errors = validationResult.ToDictionary();
            return false;
        }

        errors = new Dictionary<string, string[]>();
        return true;
    }

    private static PatchProductHandler.Command CreateCommand(string sku, PatchFields patchFields, HashSet<string> setFields)
    {
        var name = MapField(setFields, nameof(PatchFields.Name), patchFields.Name);
        var imgUri = MapField(setFields, nameof(PatchFields.ImgUri), patchFields.ImgUri);
        var price = MapField(setFields, nameof(PatchFields.Price), patchFields.Price ?? 0);

        var description = setFields.Contains(nameof(PatchFields.Description))
            ? patchFields.Description is null
                ? OneOf<string, None, Unknown>.FromT1(new None())
                : OneOf<string, None, Unknown>.FromT0(patchFields.Description)
            : OneOf<string, None, Unknown>.FromT2(new Unknown());

        return new(sku, name, imgUri, price, description);
    }

    private static OneOf<T, Unknown> MapField<T>(HashSet<string> setFields, string fieldName, T? value)
    {
        return setFields.Contains(fieldName)
            ? OneOf<T, Unknown>.FromT0(value!)
            : OneOf<T, Unknown>.FromT1(new Unknown());
    }
}
