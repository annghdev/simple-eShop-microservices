using Kernel;
using Microsoft.EntityFrameworkCore;
using Order.IntegrationEvents;
using Wolverine;

namespace Order.Messaging;

public static class ShippingStartedHandler
{
    public static async Task Handle(
        ShippingStarted evt,
        OrderDbContext db,
        CancellationToken ct)
    {
        var order = await db.Orders
            .Include(o=>o.Logs)
            .FirstOrDefaultAsync(o=>o.Id == evt.OrderId)
            ?? throw new NotFoundException($"Order with ID {evt.OrderId} not found.");
   
        order.MarkShipped();
    }
}
