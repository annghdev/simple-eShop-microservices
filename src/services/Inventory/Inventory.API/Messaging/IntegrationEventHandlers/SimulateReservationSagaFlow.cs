using Inventory.Features.InventoryItems;
using Order.IntegrationEvents;

namespace Inventory.Messaging;

public static class SimulateReservationSagaFlow
{
    [WolverinePost("inventory/simulate/place-order")]
    public static async Task<IResult> Post(IMessageBus bus)
    {
        List<InitItemCommand> initItemCmds = [];
        List<ItemReservation> orderItems = [];
        for (int i = 1; i <= 20; i++)
        {
            var initCmd = new InitItemCommand(Guid.CreateVersion7(), Guid.CreateVersion7(), null, Guid.CreateVersion7());
            initItemCmds.Add(initCmd);
            await bus.InvokeAsync(initCmd);
            var adjustStockCmd = new AdjustStockCommand(initCmd.Id, 1);
            await bus.InvokeAsync(adjustStockCmd);
            orderItems.Add(new ItemReservation(initCmd.Id, 1 + i % 2));
        }

        var order = new OrderPlaced(Guid.CreateVersion7(), 100m, "Online", orderItems);
        await bus.PublishAsync(order);

        return Results.Ok();
    }
}
