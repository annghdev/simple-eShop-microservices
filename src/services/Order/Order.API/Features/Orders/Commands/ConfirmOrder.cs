using Kernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Order.IntegrationEvents;
using Wolverine;
using Wolverine.Http;

namespace Order.API.Features;

public record ConfirmOrderCommand(Guid OrderId, string ConfirmBy);
public static class ConfirmOrderHandler
{
    public static async Task Handle(
        ConfirmOrderCommand cmd,
        OrderDbContext db,
        IMessageBus bus,
        CancellationToken ct)
    {
        var order = await db.Orders
            .Include(o => o.Logs)
            .FirstOrDefaultAsync(o => o.Id == cmd.OrderId)
            ?? throw new NotFoundException($"Order with ID {cmd.OrderId} not found.");

        order.ConfirmManually(cmd.ConfirmBy);

        await bus.PublishAsync(new OrderConfirmed(order.Id));
    }
}

public static class ConfirmOrderEndpoint
{
    [WolverinePut("/orders/{id}/confirm")]
    //[Authorize(Policy = "CanConfirmOrder")]
    public static IResult Put(Guid id, IMessageBus bus, HttpContext httpContext, CancellationToken ct)
    {
        string userId = "Staff123"; // simulate getting user ID from the authenticated user
        var cmd = new ConfirmOrderCommand(id, userId);
        bus.InvokeAsync(cmd, ct);
        return Results.Ok();
    }
}