namespace Inventory.Features.InventoryItems;

public record ReleaseReservationCommand(Guid Id, int Quantity, Guid OrderId);

public static class ReleaseStockHandler
{
    [AggregateHandler]
    public static ReservationReleased Handle(ReleaseReservationCommand cmd, InventoryItem item)
    {
        return new ReservationReleased(cmd.Id, cmd.Quantity, cmd.OrderId);
    }
}