namespace Inventory.Features.InventoryItems;

public record TransferStockCommand(Guid Id, int Quantity, Guid ToWarehouseId);

public static class TransferStockHandler
{
    [AggregateHandler]
    public static async Task<StockTransfered> Handle(TransferStockCommand cmd, InventoryItem item, IDocumentSession session)
    {
        var destItem = await session.Query<InventoryItem>()
            .FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.VariantId == item.VariantId && i.WarehouseId == cmd.ToWarehouseId)
            ?? throw new ArgumentException("Destination Item stock not found");

        var transferEvent = new StockTransfered(item.Id, cmd.Quantity, cmd.ToWarehouseId);

        var destItemReceiveEvent = new StockReceived(destItem.Id, cmd.Quantity);

        session.Events.Append(destItem.Id, destItemReceiveEvent);

        return transferEvent;
    }
}

public static class TranferStockEndpoint
{
    [WolverinePut("inventory/items/{id}/transfer")]
    public static async Task<IResult> Put(Guid id, TransferStockCommand cmd, IMessageBus bus)
    {
        await bus.InvokeAsync(cmd);
        return Results.Ok();
    }

    [WolverinePost("inventory/simulate/transfer")]
    public static async Task<IResult> Test(IMessageBus bus, IDocumentSession session)
    {
        Guid ProductId = Guid.NewGuid();
        Guid Warehouse1Id = Guid.NewGuid();
        Guid Warehouse2Id = Guid.NewGuid();
        var initItem1 = new InitItemCommand(Guid.NewGuid(), ProductId, null, Warehouse1Id);
        var initItem2 = new InitItemCommand(Guid.NewGuid(), ProductId, null, Warehouse2Id);
        await bus.InvokeAsync(initItem1);
        await bus.InvokeAsync(initItem2);
        var adjustItem1 = new AdjustStockCommand(initItem1.Id, 10);
        var adjustItem2 = new AdjustStockCommand(initItem2.Id, 10);
        await bus.InvokeAsync(adjustItem1);
        await bus.InvokeAsync(adjustItem2);

        var transfer = new TransferStockCommand(initItem1.Id, 5, Warehouse2Id);
        await bus.InvokeAsync(transfer);

        var item1 = await session.Events.AggregateStreamAsync<InventoryItem>(initItem1.Id);
        var item2 = await session.Events.AggregateStreamAsync<InventoryItem>(initItem2.Id);

        bool result = item1?.Available == 5 && item2?.Available == 15;

        return Results.Ok(result);
    }
}