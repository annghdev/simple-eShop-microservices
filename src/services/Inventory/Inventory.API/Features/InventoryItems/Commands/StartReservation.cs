namespace Inventory.Features.InventoryItems;

public record StartReservationCommand(Guid Id, int Quantity, Guid OrderId);

public static class StartReservationHandler
{
    [AggregateHandler]
    public static async Task<object> Handle(StartReservationCommand cmd, InventoryItem item, IMessageBus bus)
    {
        if (item.Available < cmd.Quantity)
        {
            var sucess = new ReservationFailed(cmd.Id, cmd.Quantity, cmd.OrderId);
            await bus.PublishAsync(sucess);
            return sucess;
        }
        var failed = new ReservationSucceeded(cmd.Id, cmd.Quantity, cmd.OrderId);
        await bus.PublishAsync(failed);
        return failed;
    }
}
