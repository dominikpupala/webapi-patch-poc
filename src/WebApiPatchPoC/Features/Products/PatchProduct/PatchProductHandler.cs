using OneOf;
using OneOf.Types;
using WebApiPatchPoC.Features.Products.Common;
using WebApiPatchPoC.Features.Products.Domain.Workflows;

namespace WebApiPatchPoC.Features.Products.PatchProduct;

internal sealed class PatchProductHandler(IProductRepository productRepository)
{
    public sealed record Command(
        string Sku,
        OneOf<string, Unknown> Name,
        OneOf<string, Unknown> ImgUri,
        OneOf<decimal, Unknown> Price,
        OneOf<string, None, Unknown> Description);

    public async Task<OneOf<Success, NotFound>> Handle(Command command)
    {
        var product = await productRepository.GetBySku(command.Sku);
        if (product is null)
        {
            return new NotFound();
        }

        command
            .Description
            .Match<OneOf<string?, Unknown>>(value => value, none => default(string), unknown => unknown)
            .Switch(
                value =>
                {
                    var (updatedProduct, events) = UpdateDescriptionWorkflow.ChangeDescription(product, value);
                    product = updatedProduct;

                    // TODO: Log or persist events here
                },
                unknown => { });

        command.Name.Switch(
            value => throw new NotImplementedException("Updating Name is not yet supported"),
            unknown => { });

        command.ImgUri.Switch(
            value => throw new NotImplementedException("Updating ImgUri is not yet supported"),
            unknown => { });

        command.Price.Switch(
            value => throw new NotImplementedException("Updating Price is not yet supported"),
            unknown => { });

        var saved = await productRepository.Save(product);

        return saved ? new Success() : new NotFound();
    }
}
