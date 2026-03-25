namespace Inventory.SeedData;

public static class SeedInventoryDataEndpoint
{
    [WolverinePost("inventory/warehouses/seed")]
    public static IResult SeedWarehouses(IDocumentSession session)
    {
        var warehouse1 = new Warehouse(Guid.CreateVersion7(), "Warehouse 1", 1, 1);
        var warehouse2 = new Warehouse(Guid.CreateVersion7(), "Warehouse 2", 1.5m, 1.5m);
        Warehouse[] warehouses = [warehouse1, warehouse2];
        session.Store(warehouses);
        return Results.Ok(warehouses);
    }
}