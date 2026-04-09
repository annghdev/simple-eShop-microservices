using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shipping.API.Features.Shipments.Commands;
using Shipping.Domain;
using Shipping.IntegrationEvents;
using Shipping.Persistence;
using Wolverine;

namespace Shipping.API.Features.Shipments.Simulation;

public class FakeDeliverySimulationOptions
{
    public DeliveryMethodProfile Standard { get; init; } = new();
    public DeliveryMethodProfile Express { get; init; } = new() { MinStepDelaySeconds = 1, MaxStepDelaySeconds = 2, FailProbability = 0.05 };
    public DeliveryMethodProfile SameDay { get; init; } = new() { MinStepDelaySeconds = 1, MaxStepDelaySeconds = 1, FailProbability = 0.15 };

    public class DeliveryMethodProfile
    {
        public int MinStepDelaySeconds { get; init; } = 2;
        public int MaxStepDelaySeconds { get; init; } = 4;
        public double FailProbability { get; init; } = 0.10;
    }
}

public record AdvanceFakeDeliverySimulation(Guid ShipmentId);

public static class FakeDeliverySimulationHandlers
{
    public static async Task Handle(
        StartFakeDeliverySimulation cmd,
        ShippingDbContext db,
        IMessageBus bus,
        IOptions<FakeDeliverySimulationOptions> options,
        CancellationToken ct)
    {
        var shipment = await db.Shipments.FirstOrDefaultAsync(x => x.Id == cmd.ShipmentId, ct);
        if (shipment is null)
            return;

        if (IsTerminal(shipment.Status))
            return;

        // If simulation was started before the shipment got started, kick it forward once.
        var delay = RandomDelay(shipment.DeliveryMethod, options.Value);
        await bus.ScheduleAsync(new AdvanceFakeDeliverySimulation(shipment.Id), DateTimeOffset.UtcNow.Add(delay));
    }

    public static async Task Handle(
        AdvanceFakeDeliverySimulation cmd,
        ShippingDbContext db,
        IMessageBus bus,
        IOptions<FakeDeliverySimulationOptions> options,
        CancellationToken ct)
    {
        var shipment = await db.Shipments
            .Include(x => x.Trackings)
            .FirstOrDefaultAsync(x => x.Id == cmd.ShipmentId, ct);

        if (shipment is null)
            return;
        if (IsTerminal(shipment.Status))
            return;

        var now = DateTimeOffset.UtcNow;
        var profile = GetProfile(shipment.DeliveryMethod, options.Value);

        switch (shipment.Status)
        {
            case ShippingStatus.Preparing:
                // StartShipping should normally do this; keep it safe for idempotent replays.
                shipment.Start(now);
                await bus.PublishAsync(new ShippingStarted(shipment.Id, shipment.OrderId, now));
                break;

            case ShippingStatus.Shipped:
                shipment.MarkInTransit(now);
                break;

            case ShippingStatus.InTransit:
                shipment.MarkOutForDelivery(now);
                break;

            case ShippingStatus.OutForDelivery:
                if (ShouldFail(profile))
                {
                    shipment.Fail(now, "Simulated failure");
                    await bus.PublishAsync(new ShipmentDeliveryFailed(shipment.Id, shipment.OrderId, now));
                }
                else
                {
                    shipment.MarkDelivered(now);
                    await bus.PublishAsync(new ShipmentDelivered(shipment.Id, shipment.OrderId, now));
                }
                break;

            case ShippingStatus.DeliveryFailed:
                // Optionally move to Returned to close the loop.
                shipment.MarkReturned(now);
                break;

            default:
                // Unknown/legacy states: no-op to avoid invalid transitions.
                return;
        }

        await db.SaveChangesAsync(ct);

        if (!IsTerminal(shipment.Status))
        {
            var delay = RandomDelay(shipment.DeliveryMethod, options.Value);
            await bus.ScheduleAsync(new AdvanceFakeDeliverySimulation(shipment.Id), DateTimeOffset.UtcNow.Add(delay));
        }
    }

    private static bool IsTerminal(ShippingStatus status) =>
        status is ShippingStatus.Delivered or ShippingStatus.Returned;

    private static FakeDeliverySimulationOptions.DeliveryMethodProfile GetProfile(DeliveryMethod method, FakeDeliverySimulationOptions opts) =>
        method switch
        {
            DeliveryMethod.Express => opts.Express,
            DeliveryMethod.SameDay => opts.SameDay,
            _ => opts.Standard
        };

    private static TimeSpan RandomDelay(DeliveryMethod method, FakeDeliverySimulationOptions opts)
    {
        var p = GetProfile(method, opts);
        var min = Math.Max(0, p.MinStepDelaySeconds);
        var max = Math.Max(min, p.MaxStepDelaySeconds);
        var seconds = Random.Shared.Next(min, max + 1);
        return TimeSpan.FromSeconds(seconds);
    }

    private static bool ShouldFail(FakeDeliverySimulationOptions.DeliveryMethodProfile profile)
    {
        var r = Random.Shared.NextDouble();
        return r < profile.FailProbability;
    }
}

