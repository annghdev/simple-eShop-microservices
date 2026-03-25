namespace Inventory.Features.InventoryItems;

public record ReceiveStockCommand(Guid Id, int Quantity);

public static class ReceiveStockHandler
{
    [AggregateHandler]
    public static StockReceived Handle(ReceiveStockCommand cmd, InventoryItem item)
    {
        var evt = item.Receive(cmd.Quantity);
        return evt;
    }
}

public static class ReceiveStockEndpoint
{
    [WolverinePut("inventory/items/{id}/receive")]
    public static (IResult, ReceiveStockCommand) Put(Guid id, ReceiveStockCommand cmd)
    {
        return (Results.Ok(), cmd);
    }
}