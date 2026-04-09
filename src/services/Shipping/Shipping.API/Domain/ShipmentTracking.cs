using System.ComponentModel.DataAnnotations;

namespace Shipping.Domain;

public class ShipmentTracking
{
    [Key]
    public Guid Id { get; private set; }
    public Guid ShipmentId { get; private set; }
    public DateTimeOffset TimeStamp { get; private set; }
    public ShippingStatus Status { get; private set; }
    public string Description { get; private set; } = string.Empty;

    public Shipment Shipment { get; private set; } = default!;

    // For EF Core
    private ShipmentTracking() { }

    public ShipmentTracking(Guid id, Guid shipmentId, DateTimeOffset timeStamp, ShippingStatus status, string description)
    {
        if (id == Guid.Empty) throw new ArgumentException("TrackingId cannot be empty.", nameof(id));
        if (shipmentId == Guid.Empty) throw new ArgumentException("ShipmentId cannot be empty.", nameof(shipmentId));

        Id = id;
        ShipmentId = shipmentId;
        TimeStamp = timeStamp;
        Status = status;
        Description = description ?? string.Empty;
    }
}
