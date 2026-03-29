using Kernel.Interfaces;

namespace Order.IntegrationEvents;

public interface IOrderIntegrationEvent : IIntegrationEvent;

public record OrderPlaced(
    Guid OrderId,
    decimal Amount,
    string PaymentMethod,
    List<ItemReservation> Items) : IOrderIntegrationEvent; // ==> Reserve Inventory and Init Payment
public record ItemReservation(Guid InventoryItemId, int Quantity);
public record OrderConfirmed(Guid OrderId) : IOrderIntegrationEvent; // ==> Commit stock
public record OrderCancelledBeforeConfirm(Guid OrderId) : IOrderIntegrationEvent; // ==> Release stock
public record OrderCancelledAfterConfirm(Guid OrderId) : IOrderIntegrationEvent; // ==> Restock


public record PaymentSuceeeded(Guid PaymentId, Guid OrderId, decimal Amount) : IOrderIntegrationEvent; // ==> Confirm Order and mark paid
public record PaymentFailed(Guid PaymentId, Guid OrderId) : IOrderIntegrationEvent; // ==> Confirm Order and mark paid

public record ShippingStarted(Guid ShipmentId, Guid OrderId, DateTimeOffset StartedAt) : IOrderIntegrationEvent; // ==> Mark order as Shipped
public record ShipmentDelivered(Guid ShipmentId, Guid OrderId, DateTimeOffset DeliveredAt) : IOrderIntegrationEvent; // ==> Mark order as Shipped
public record ShipmentDeliveryFailed(Guid ShipmentId, Guid OrderId, DateTimeOffset OccurredAt) : IOrderIntegrationEvent; // ==> Mark order as Shipped
