using Kernel.Interfaces;

namespace Inventory.IntegrationEvents;

public record InventoryReservationFailed(Guid OrderId): IIntegrationEvent; // ==> CancelOrder
