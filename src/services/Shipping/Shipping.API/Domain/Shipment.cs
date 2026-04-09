using System.ComponentModel.DataAnnotations;

namespace Shipping.Domain;

public class Shipment
{
    [Key]
    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }

    public DeliveryMethod DeliveryMethod { get; private set; }

    public int TotalItems { get; private set; }

    public string Address { get; private set; } = string.Empty;

    public decimal Width { get; private set; }
    public decimal Height { get; private set; }
    public decimal Depth { get; private set; }
    public decimal Weight { get; private set; }

    public string PickupAt { get; private set; } = string.Empty;

    public decimal ShipFee { get; private set; }

    public ShippingStatus Status { get; private set; } = ShippingStatus.Preparing;

    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? DeliveredAt { get; private set; }
    public DateTimeOffset? FailedAt { get; private set; }
    public string? FailureReason { get; private set; }

    public List<ShipmentTracking> Trackings { get; private set; } = [];

    // For EF Core
    private Shipment() { }

    public Shipment(
        Guid id,
        Guid orderId,
        DeliveryMethod deliveryMethod,
        int totalItems,
        string address,
        decimal shipFee,
        string pickupAt,
        decimal width,
        decimal height,
        decimal depth,
        decimal weight,
        DateTimeOffset nowUtc)
    {
        if (id == Guid.Empty) throw new ArgumentException("ShipmentId cannot be empty.", nameof(id));
        if (orderId == Guid.Empty) throw new ArgumentException("OrderId cannot be empty.", nameof(orderId));
        if (totalItems < 0) throw new ArgumentOutOfRangeException(nameof(totalItems));
        if (shipFee < 0) throw new ArgumentOutOfRangeException(nameof(shipFee));

        Id = id;
        OrderId = orderId;
        DeliveryMethod = deliveryMethod;
        TotalItems = totalItems;
        Address = address ?? string.Empty;
        ShipFee = shipFee;
        PickupAt = pickupAt ?? string.Empty;
        Width = width;
        Height = height;
        Depth = depth;
        Weight = weight;
        CreatedAt = nowUtc;

        Status = ShippingStatus.Preparing;
        AddTracking(ShippingStatus.Preparing, nowUtc, "Shipment created");
    }

    public bool CanTransitionTo(ShippingStatus next)
    {
        if (Status == next) return true;

        return Status switch
        {
            ShippingStatus.Preparing => next is ShippingStatus.Shipped,
            ShippingStatus.Shipped => next is ShippingStatus.InTransit or ShippingStatus.DeliveryFailed,
            ShippingStatus.InTransit => next is ShippingStatus.OutForDelivery or ShippingStatus.DeliveryFailed,
            ShippingStatus.OutForDelivery => next is ShippingStatus.Delivered or ShippingStatus.DeliveryFailed,
            ShippingStatus.DeliveryFailed => next is ShippingStatus.Returned,
            ShippingStatus.Delivered => false,
            ShippingStatus.Returned => false,
            ShippingStatus.Rejected => next is ShippingStatus.Returned,
            _ => false
        };
    }

    public void Start(DateTimeOffset startedAtUtc)
    {
        TransitionTo(ShippingStatus.Shipped, startedAtUtc, "Shipping started");
        StartedAt ??= startedAtUtc;
    }

    public void MarkInTransit(DateTimeOffset occurredAtUtc, string? note = null) =>
        TransitionTo(ShippingStatus.InTransit, occurredAtUtc, note ?? "In transit");

    public void MarkOutForDelivery(DateTimeOffset occurredAtUtc, string? note = null) =>
        TransitionTo(ShippingStatus.OutForDelivery, occurredAtUtc, note ?? "Out for delivery");

    public void MarkDelivered(DateTimeOffset deliveredAtUtc, string? note = null)
    {
        TransitionTo(ShippingStatus.Delivered, deliveredAtUtc, note ?? "Delivered");
        DeliveredAt = deliveredAtUtc;
        FailureReason = null;
        FailedAt = null;
    }

    public void Fail(DateTimeOffset failedAtUtc, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Failure reason is required.", nameof(reason));
        TransitionTo(ShippingStatus.DeliveryFailed, failedAtUtc, $"Delivery failed: {reason}");
        FailedAt = failedAtUtc;
        FailureReason = reason;
    }

    public void MarkReturned(DateTimeOffset occurredAtUtc, string? note = null) =>
        TransitionTo(ShippingStatus.Returned, occurredAtUtc, note ?? "Returned");

    public void TransitionTo(ShippingStatus next, DateTimeOffset occurredAtUtc, string? description = null)
    {
        if (!CanTransitionTo(next))
            throw new InvalidOperationException($"Invalid shipping status transition: {Status} -> {next}.");

        Status = next;
        AddTracking(next, occurredAtUtc, description ?? string.Empty);
    }

    private void AddTracking(ShippingStatus status, DateTimeOffset occurredAtUtc, string description)
    {
        Trackings.Add(new ShipmentTracking(
            id: Guid.NewGuid(),
            shipmentId: Id,
            timeStamp: occurredAtUtc,
            status: status,
            description: description));
    }
}

public enum DeliveryMethod
{
    Standard = 0,
    Express = 1,
    SameDay = 2
}

public enum ShippingStatus
{
    Preparing = 0,
    Shipped = 1,
    InTransit = 2,
    OutForDelivery = 3,
    Delivered = 4,
    DeliveryFailed = 5,
    Rejected = 6,
    Returned = 7
}