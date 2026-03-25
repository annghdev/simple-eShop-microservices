namespace Inventory.Messaging;

public static class WarehouseCreatedHandler
{
    public static async Task Handle(WarehouseCreated e)
    {
        //Step 1: query all productIds from Projection

        //Step 2: divide into multiple batch

        //Step 3: publish batch

        //Step 4: handle batch
    }
}
