namespace Inventory.Features.Warehouses;

public record CreateWarehouseCommand(string Name, decimal Latitude, decimal Longitude);

public static class CreateWarehouseHandler
{
    public static WarehouseCreated Handle(CreateWarehouseCommand cmd, IDocumentSession session)
    {
        var warehouse = Warehouse.Create(cmd.Name, cmd.Latitude, cmd.Longitude);
        session.Store(warehouse);
        return new WarehouseCreated(warehouse.Id, warehouse.Name, warehouse.Latitude, warehouse.Longitude);
    }
}

