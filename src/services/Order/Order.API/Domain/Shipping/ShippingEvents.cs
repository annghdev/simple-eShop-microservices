namespace Order.Domain;

public record ShipmentReadyForShip(Guid ShipmentId, Guid OrderId);
public record ShippingStarted(Guid ShipmentId, Guid OrderId);
public record ShippingDelivered(Guid ShipmentId, Guid OrderId);
public record ShippingFailed(Guid ShipmentId, Guid OrderId, string Reason);
public record ShipmentReturned(Guid ShipmentId, Guid OrderId);