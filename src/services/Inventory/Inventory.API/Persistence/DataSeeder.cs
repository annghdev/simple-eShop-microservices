namespace Inventory.Persistence;

public class DataSeeder(IDocumentSession session)
{
    private static readonly Guid Warehouse1Id = Guid.Parse("11111111-3333-3333-3333-333333333333");
    private static readonly Guid Warehouse2Id = Guid.Parse("22222222-3333-3333-3333-333333333333");

    private static readonly Guid ProductId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private static readonly Guid Variant1Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
    private static readonly Guid Variant2Id = Guid.Parse("55555555-5555-5555-5555-555555555555");

    private static readonly Guid InventoryItem1Id = Guid.Parse("11111111-1111-3333-3333-333333333333");
    private static readonly Guid InventoryItem2Id = Guid.Parse("11111111-2222-3333-3333-333333333333");

    public async Task SeedAsync()
    {
        if (await session.LoadAsync<Warehouse>(Warehouse1Id) is null)
        {
            session.Store(new Warehouse(Warehouse1Id, "Warehouse 1", 1, 1));
        }

        if (await session.LoadAsync<Warehouse>(Warehouse2Id) is null)
        {
            session.Store(new Warehouse(Warehouse2Id, "Warehouse 2", 1.5m, 1.5m));
        }

        if (await session.Events.AggregateStreamAsync<InventoryItem>(InventoryItem1Id) is null)
        {
            session.Events.Append(InventoryItem1Id, new ItemInitialized(
                InventoryItem1Id,
                ProductId,
                Variant1Id,
                Warehouse1Id));
        }

        if (await session.Events.AggregateStreamAsync<InventoryItem>(InventoryItem2Id) is null)
        {
            session.Events.Append(InventoryItem2Id, new ItemInitialized(
                InventoryItem2Id,
                ProductId,
                Variant2Id,
                Warehouse2Id));
        }

        await session.SaveChangesAsync();
    }
}
