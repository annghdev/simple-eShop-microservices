using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Order.IntegrationEvents;
using Shipping.Domain;
using Shipping.IntegrationEvents;
using Shipping.Persistence;
using Wolverine;
using Wolverine.Http;

namespace Shipping.API.Features.Shipments.Commands;

public record StartShippingCommand(
    Guid OrderId,
    DeliveryMethod DeliveryMethod = DeliveryMethod.Standard,
    string? Address = null,
    int TotalItems = 0,
    decimal ShipFee = 0,
    string? PickupAt = null,
    decimal Width = 0,
    decimal Height = 0,
    decimal Depth = 0,
    decimal Weight = 0,
    bool Simulate = true);

public record StartShippingResult(Guid ShipmentId, Guid OrderId, ShippingStatus Status);

public sealed class StartShippingCommandValidator : AbstractValidator<StartShippingCommand>
{
    public StartShippingCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.TotalItems).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ShipFee).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Width).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Height).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Depth).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Weight).GreaterThanOrEqualTo(0);
    }
}

public static class StartShippingHandler
{
    public static async Task<StartShippingResult> Handle(
        StartShippingCommand cmd,
        ShippingDbContext db,
        IMessageBus bus,
        CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        Shipment? shipment = await db.Shipments
            .Include(x => x.Trackings)
            .FirstOrDefaultAsync(x => x.OrderId == cmd.OrderId, ct);

        var publishStarted = false;

        if (shipment is null)
        {
            shipment = new Shipment(
                id: Guid.NewGuid(),
                orderId: cmd.OrderId,
                deliveryMethod: cmd.DeliveryMethod,
                totalItems: cmd.TotalItems,
                address: cmd.Address ?? string.Empty,
                shipFee: cmd.ShipFee,
                pickupAt: cmd.PickupAt ?? string.Empty,
                width: cmd.Width,
                height: cmd.Height,
                depth: cmd.Depth,
                weight: cmd.Weight,
                nowUtc: now);

            db.Shipments.Add(shipment);
        }

        var wasAlreadyStarted = shipment.StartedAt is not null || shipment.Status != ShippingStatus.Preparing;

        if (!wasAlreadyStarted)
        {
            shipment.Start(now);
            publishStarted = true;
        }

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // Another concurrent start likely won the unique OrderId race. Reload and proceed idempotently.
            shipment = await db.Shipments
                .Include(x => x.Trackings)
                .FirstAsync(x => x.OrderId == cmd.OrderId, ct);

            publishStarted = false;
        }

        if (publishStarted)
        {
            await bus.PublishAsync(new ShippingStarted(shipment.Id, shipment.OrderId, now));
        }

        if (cmd.Simulate)
        {
            // Kick off simulation (implemented in later step)
            await bus.SendAsync(new StartFakeDeliverySimulation(shipment.Id));
        }

        return new StartShippingResult(shipment.Id, shipment.OrderId, shipment.Status);
    }
}

public static class StartShippingEndpoint
{
    [WolverinePost("/shippings/start")]
    public static async Task<IResult> Post(StartShippingCommand cmd, IMessageBus bus, CancellationToken ct)
    {
        var result = await bus.InvokeAsync<StartShippingResult>(cmd, ct);
        return Results.Ok(result);
    }
}

/// <summary>
/// Local message used to begin fake delivery simulation.
/// </summary>
public record StartFakeDeliverySimulation(Guid ShipmentId);

public static class AutoStartShippingOnOrderConfirmedHandler
{
    public static async Task Handle(
        OrderConfirmed evt,
        IMessageBus bus,
        CancellationToken ct)
    {
        // Keep it lightweight and deterministic enough for dev: pick a method based on simple weights.
        var method = ChooseMethod();
        var cmd = new StartShippingCommand(evt.OrderId, DeliveryMethod: method, Simulate: true);
        await bus.InvokeAsync(cmd, ct);
    }

    private static DeliveryMethod ChooseMethod()
    {
        var r = Random.Shared.Next(0, 100);
        return r switch
        {
            < 60 => DeliveryMethod.Standard,
            < 90 => DeliveryMethod.Express,
            _ => DeliveryMethod.SameDay
        };
    }
}

