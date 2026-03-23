namespace Order.IntegrationEvents;

public record OrderPlaced(
    Guid OrderId,
    decimal Amount
    ); // ==> Reserve Inventory and Init Payment

public record OrderConfirmed(Guid OrderId); // ==> Commit stock
public record OrderCancelledBeforeConfirm(Guid OrderId); // ==> Release stock
public record OrderCancelledAfterConfirm(Guid OrderId); // ==> Restock
