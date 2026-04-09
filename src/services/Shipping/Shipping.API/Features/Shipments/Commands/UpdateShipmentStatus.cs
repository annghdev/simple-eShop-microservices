using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shipping.Domain;
using Shipping.Persistence;
using Wolverine;
using Wolverine.Http;

namespace Shipping.API.Features.Shipments.Commands;

public record UpdateShipmentStatusCommand(
    Guid ShipmentId,
    ShippingStatus Status,
    string? Description = null,
    bool Force = false);

public record UpdateShipmentStatusResult(Guid ShipmentId, Guid OrderId, ShippingStatus Status);

public sealed class UpdateShipmentStatusCommandValidator : AbstractValidator<UpdateShipmentStatusCommand>
{
    public UpdateShipmentStatusCommandValidator()
    {
        RuleFor(x => x.ShipmentId).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public static class UpdateShipmentStatusHandler
{
    public static async Task<UpdateShipmentStatusResult> Handle(
        UpdateShipmentStatusCommand cmd,
        ShippingDbContext db,
        CancellationToken ct)
    {
        var shipment = await db.Shipments
            .Include(x => x.Trackings)
            .FirstOrDefaultAsync(x => x.Id == cmd.ShipmentId, ct)
            ?? throw new InvalidOperationException($"Shipment with ID {cmd.ShipmentId} not found.");

        var now = DateTimeOffset.UtcNow;

        if (cmd.Force)
        {
            // Force mode still records an audit tracking entry.
            shipment.TransitionTo(cmd.Status, now, cmd.Description ?? "Forced status update");
        }
        else
        {
            // Use the explicit domain methods where possible for clearer intent.
            switch (cmd.Status)
            {
                case ShippingStatus.Shipped:
                    if (shipment.StartedAt is null)
                        shipment.Start(now);
                    break;
                case ShippingStatus.InTransit:
                    shipment.MarkInTransit(now, cmd.Description);
                    break;
                case ShippingStatus.OutForDelivery:
                    shipment.MarkOutForDelivery(now, cmd.Description);
                    break;
                case ShippingStatus.Delivered:
                    shipment.MarkDelivered(now, cmd.Description);
                    break;
                case ShippingStatus.DeliveryFailed:
                    shipment.Fail(now, cmd.Description ?? "Manual failure");
                    break;
                case ShippingStatus.Returned:
                    shipment.MarkReturned(now, cmd.Description);
                    break;
                default:
                    shipment.TransitionTo(cmd.Status, now, cmd.Description);
                    break;
            }
        }

        await db.SaveChangesAsync(ct);

        return new UpdateShipmentStatusResult(shipment.Id, shipment.OrderId, shipment.Status);
    }
}

public static class UpdateShipmentStatusEndpoint
{
    [WolverinePut("/shippings/{id:guid}/status")]
    public static async Task<IResult> Put(Guid id, UpdateShipmentStatusCommand body, IMessageBus bus, CancellationToken ct)
    {
        var cmd = body with { ShipmentId = id };
        var result = await bus.InvokeAsync<UpdateShipmentStatusResult>(cmd, ct);
        return Results.Ok(result);
    }
}

