using Contracts.Protos.ShippingInfo;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Shipping.Persistence;

namespace Shipping.API.GrpcServices.Handlers;

public sealed class ShippingInfoGrpcHandler(ShippingDbContext db)
    : ShippingInfoGrpc.ShippingInfoGrpcBase
{
    public override async Task<GetShippingInfoByOrderIdResponse> GetByOrderId(
        GetShippingInfoByOrderIdRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.OrderId, out var orderId) || orderId == Guid.Empty)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "order_id is invalid"));

        var shipment = await db.Shipments
            .AsNoTracking()
            .Include(x => x.Trackings)
            .FirstOrDefaultAsync(x => x.OrderId == orderId, context.CancellationToken);

        if (shipment is null)
        {
            return new GetShippingInfoByOrderIdResponse
            {
                Found = false,
                OrderId = request.OrderId
            };
        }

        var res = new GetShippingInfoByOrderIdResponse
        {
            Found = true,
            ShipmentId = shipment.Id.ToString(),
            OrderId = shipment.OrderId.ToString(),
            Status = (int)shipment.Status,
            DeliveryMethod = (int)shipment.DeliveryMethod,
            Address = shipment.Address ?? string.Empty,
            ShipFee = (double)shipment.ShipFee,
            CreatedAtUnixMs = shipment.CreatedAt.ToUnixTimeMilliseconds(),
            StartedAtUnixMs = shipment.StartedAt?.ToUnixTimeMilliseconds() ?? 0,
            DeliveredAtUnixMs = shipment.DeliveredAt?.ToUnixTimeMilliseconds() ?? 0,
            FailedAtUnixMs = shipment.FailedAt?.ToUnixTimeMilliseconds() ?? 0,
            FailureReason = shipment.FailureReason ?? string.Empty
        };

        foreach (var t in shipment.Trackings.OrderBy(x => x.TimeStamp))
        {
            res.Tracking.Add(new ShippingTrackingItem
            {
                Status = (int)t.Status,
                TimestampUnixMs = t.TimeStamp.ToUnixTimeMilliseconds(),
                Description = t.Description ?? string.Empty
            });
        }

        return res;
    }
}

