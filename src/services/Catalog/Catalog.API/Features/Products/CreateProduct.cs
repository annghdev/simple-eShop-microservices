using Marten;
using Wolverine.Http;

namespace Catalog.API.Features;

public record CreateProductCommand(string Name);
public class CreateProduct
{
    [WolverinePost("products")]
    public static Product Post(CreateProductCommand cmd, IDocumentSession session)
    {
        var product = new Product
        {
            Id = Guid.CreateVersion7(),
            Name = cmd.Name,
        };

        session.Store(product);
        return product;
    }
}
