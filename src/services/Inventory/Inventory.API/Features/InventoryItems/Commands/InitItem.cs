namespace Inventory.Features.InventoryItems;

public record InitItemCommand(Guid Id, Guid ProductId, Guid? VariantId, Guid WarehouseId);

public static class InitItemHandler
{
    [AggregateHandler]
    public static object Handle(InitItemCommand cmd, InventoryItem item)
    {
        return new ItemInitialized(cmd.Id, cmd.ProductId, cmd.VariantId, cmd.WarehouseId);
    }
}