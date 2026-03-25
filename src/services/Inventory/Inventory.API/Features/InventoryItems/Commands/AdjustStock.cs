namespace Inventory.Features.InventoryItems;

public record AdjustStockCommand(Guid Id, int Quantity);

public static class AdjustStockHandler
{
    [AggregateHandler]
    public static object Handle(AdjustStockCommand cmd, InventoryItem item)
    {
        var evt = item.AdjustStock(cmd.Quantity);
        return evt;
    }
}

public static class AdjustStockEndpoint
{
    [WolverinePost("inventory/items/adjust")]
    public static (IResult, AdjustStockCommand) Post(AdjustStockCommand cmd)
    {
        return (Results.Ok(), cmd);
    }
}