using Kernel;
using Microsoft.EntityFrameworkCore;
using Order.InternalCalls;

namespace Order.Features.Orders;

public record GetOrderReservationDetailsQuery(Guid OrderId);

public record OrderReservationDetailsResponse(
    Guid OrderId,
    ReservationStatus ReservationStatus,
    List<OrderItemReservationDto> Items);

public record OrderItemReservationDto(Guid InventoryItemId, int Quantity);

public static class GetOrderReservationDetailsHandler
{
    public static async Task<OrderReservationDetailsResponse> Handle(
        GetOrderReservationDetailsQuery query,
        OrderDbContext db,
        CancellationToken ct)
    {
        var order = await db.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Reservations)
            .FirstOrDefaultAsync(o => o.Id == query.OrderId, ct)
            ?? throw new NotFoundException($"Order with ID {query.OrderId} not found.");

        var reservationItems = order.Items
            .SelectMany(oi => oi.Reservations.Select(r => new OrderItemReservationDto(r.InventoryItemId, r.Quantity)))
            .ToList();

        return new OrderReservationDetailsResponse(
            OrderId: order.Id,
            ReservationStatus: order.ReservationStatus,
            Items: reservationItems);
    }
}

