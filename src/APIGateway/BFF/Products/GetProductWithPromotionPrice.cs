using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Wolverine.Http;

namespace APIGateway.BFF.Products;

public class GetProductWithPromotionPrice
{
    [WolverineGet("bff/products")]
    [EnableRateLimiting("sliding")]
    //[Authorize]
    public static IResult Get()
    {
        return Results.Ok();
    }
}
