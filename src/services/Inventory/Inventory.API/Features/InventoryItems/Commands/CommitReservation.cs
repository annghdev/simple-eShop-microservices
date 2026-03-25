namespace Inventory.Features.InventoryItems;

public record CommitReservationCommand(Guid Id, int Quantity, Guid OrderId);

public static class CommitReservationHandler
{
    [AggregateHandler]
    public static ReservationCommitted Handle(CommitReservationCommand cmd, InventoryItem item)
    {
        return new ReservationCommitted(cmd.Id, cmd.Quantity, cmd.OrderId);
    }
}