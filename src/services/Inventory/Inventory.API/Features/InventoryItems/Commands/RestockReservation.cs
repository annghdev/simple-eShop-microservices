namespace Inventory.Features.InventoryItems;

public record RestockReservationCommand(Guid Id, int Quantity, Guid OrderId);

public static class RestockReservationHandler
{
    [AggregateHandler]
    public static ReservationRestocked Handle(RestockReservationCommand cmd, InventoryItem item)
    {
        var evt = new ReservationRestocked(cmd.Id, cmd.Quantity, cmd.OrderId);
        return evt;
    }
}