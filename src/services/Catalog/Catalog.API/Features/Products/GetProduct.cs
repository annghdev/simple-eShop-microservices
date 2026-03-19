using Wolverine.Http;
using Wolverine.Http.Marten;

namespace Catalog.API.Features;

public class GetProduct
{
    [WolverineGet("products/{id}")]
    public static Product Get([Document] Product product) => product;
}
