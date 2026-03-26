using Catalog.IntegrationEvents;
using Wolverine;
using Wolverine.Http;

namespace Catalog.Features.Products;

public record PublishDraftCommand(Guid DraftId);
public static class PublishDraftHandler
{
    public static ProductPublished Handle(PublishDraftCommand cmd)
    {
        var evt = new ProductPublished(cmd.DraftId, "ABC", "abc.com", []);
        return evt;
    }
}

public static class PublishDraftEndpoint
{
    [WolverinePost("products")]
    public static async Task<IResult> Post(IMessageBus bus)
    {
        await bus.SendAsync(new PublishDraftCommand(Guid.CreateVersion7()));
        return Results.Ok();
    }
}
