using Catalog.IntegrationEvents;

namespace Inventory.Messaging;

/// <summary>
/// Initialize InventoryItems
/// </summary>
public static class ProductPublishedHandler
{
    public static async Task<object[]> Handle(ProductPublished e, IQuerySession qSession, IDocumentSession dSession)
    {
        var warehouseIds = await qSession.Query<Warehouse>().Select(w => w.Id).ToListAsync();
        if (warehouseIds.Count == 0)
            throw new Exception("Empty Warehouse list");

        List<ItemInitialized> initItemEvents = [];

        if (e.Variants.Count > 0)
        {
            foreach (var wId in warehouseIds)
            {
                initItemEvents.AddRange(e.Variants
                    .Select(v => new ItemInitialized(Guid.CreateVersion7(), e.ProductId, v.VariantId, wId)));
            }
        }
        else
        {
            foreach (var wId in warehouseIds)
            {
                var item = new ItemInitialized(Guid.CreateVersion7(), e.ProductId, null, wId);
                initItemEvents.Add(item);
            }
        }

        foreach (var evt in initItemEvents)
        {
            dSession.Events.Append(evt.Id, evt);
        }

        return [.. initItemEvents];
    }
}

public static class SimulateConsumeProductPublished
{
    [WolverinePost("inventory/simulate/product-published/without-variant")]
    public static (IResult, ProductPublished) PostWithoutVariants()
    {
        var evt = new ProductPublished(Guid.CreateVersion7(), "ABC", "abc.com", []);
        return (Results.Ok(evt), evt);
    }

    [WolverinePost("inventory/simulate/product-published/with-variants")]
    public static (IResult, ProductPublished) PostWhitVariants()
    {
        List<VariantInfo> variants =
        [
            new VariantInfo(Guid.NewGuid(), "Variant1", "variant1 image"),
            new VariantInfo(Guid.NewGuid(), "Variant2", "variant2 image"),
            new VariantInfo(Guid.NewGuid(), "Variant3", "variant3 image"),
        ];
        var evt = new ProductPublished(Guid.CreateVersion7(), "XYZ", "xyz.com", variants);
        return (Results.Ok(evt), evt);
    }
}
