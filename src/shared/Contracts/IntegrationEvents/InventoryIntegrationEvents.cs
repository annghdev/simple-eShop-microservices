namespace Inventory.IntegrationEvents;

public record InventoryReservationFailed(Guid OrderId); // => CancelOrder
