using Wolverine.Http.Marten;

namespace Inventory.Features.InventoryItems;

public static class GetItemEndpoint
{
    [WolverineGet("inventory/items/{id}")]
    public static InventoryItem GetById([Document] InventoryItem item)
        => item;
}
