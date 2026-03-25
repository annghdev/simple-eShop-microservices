using Kernel.Interfaces;

namespace Order.IntegrationEvents;

public record OrderPlaced(
    Guid OrderId,
    decimal Amount,
    string PaymentMethod,
    List<ItemReservation> Items) : IIntegrationEvent; // ==> Reserve Inventory and Init Payment
public record ItemReservation(Guid InventoryItemId, int Quantity) : IIntegrationEvent;
public record OrderConfirmed(Guid OrderId) : IIntegrationEvent; // ==> Commit stock
public record OrderCancelledBeforeConfirm(Guid OrderId) : IIntegrationEvent; // ==> Release stock
public record OrderCancelledAfterConfirm(Guid OrderId) : IIntegrationEvent; // ==> Restock
