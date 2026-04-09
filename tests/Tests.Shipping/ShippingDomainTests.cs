using FluentAssertions;
using Shipping.Domain;
using Tests.Common;

namespace Tests.Shipping;

public class ShippingDomainTests
{
    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void Shipment_ShouldCreatePreparingTrackingOnCreation()
    {
        var now = DateTimeOffset.UtcNow;
        var shipmentId = Guid.CreateVersion7();
        var orderId = Guid.CreateVersion7();

        var shipment = new Shipment(
            id: shipmentId,
            orderId: orderId,
            deliveryMethod: DeliveryMethod.Standard,
            totalItems: 2,
            address: "District 1",
            shipFee: 10,
            pickupAt: "Warehouse A",
            width: 1.5m,
            height: 2m,
            depth: 3m,
            weight: 0.5m,
            nowUtc: now);

        shipment.Status.Should().Be(ShippingStatus.Preparing);
        shipment.Trackings.Should().NotBeEmpty();
        shipment.Trackings[0].ShipmentId.Should().Be(shipmentId);
        shipment.Trackings[0].Status.Should().Be(ShippingStatus.Preparing);
    }

    [Fact]
    [Trait("Category", TestCategories.Functional)]
    public void Shipment_ShouldStoreDimensionsAndAddress()
    {
        var shipment = new Shipment(
            id: Guid.CreateVersion7(),
            orderId: Guid.CreateVersion7(),
            deliveryMethod: DeliveryMethod.Express,
            totalItems: 1,
            address: "District 1",
            shipFee: 0,
            pickupAt: "Warehouse A",
            width: 1.5m,
            height: 2m,
            depth: 3m,
            weight: 0.5m,
            nowUtc: DateTimeOffset.UtcNow);

        shipment.Address.Should().Be("District 1");
        shipment.Weight.Should().Be(0.5m);
        shipment.Width.Should().BePositive();
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void Shipment_ShouldRejectInvalidStatusTransition()
    {
        var shipment = new Shipment(
            id: Guid.CreateVersion7(),
            orderId: Guid.CreateVersion7(),
            deliveryMethod: DeliveryMethod.Standard,
            totalItems: 1,
            address: "District 1",
            shipFee: 0,
            pickupAt: "Warehouse A",
            width: 1,
            height: 1,
            depth: 1,
            weight: 1,
            nowUtc: DateTimeOffset.UtcNow);

        var act = () => shipment.MarkDelivered(DateTimeOffset.UtcNow);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void Shipment_ShouldAllowHappyPathToDelivered()
    {
        var shipment = new Shipment(
            id: Guid.CreateVersion7(),
            orderId: Guid.CreateVersion7(),
            deliveryMethod: DeliveryMethod.SameDay,
            totalItems: 1,
            address: "District 1",
            shipFee: 0,
            pickupAt: "Warehouse A",
            width: 1,
            height: 1,
            depth: 1,
            weight: 1,
            nowUtc: DateTimeOffset.UtcNow);

        var now = DateTimeOffset.UtcNow;
        shipment.Start(now);
        shipment.MarkInTransit(now);
        shipment.MarkOutForDelivery(now);
        shipment.MarkDelivered(now);

        shipment.Status.Should().Be(ShippingStatus.Delivered);
        shipment.DeliveredAt.Should().NotBeNull();
    }
}

