using Contracts.Protos.ShippingInfo;
using Grpc.Core;
using Wolverine.Http;

namespace Order.GrpcServices;

public interface IGetShippingInfoCaller
{
    Task<GetShippingInfoByOrderIdResponse> GetByOrderId(Guid orderId, CancellationToken ct = default);
}

public sealed class GetShippingInfoCaller(ShippingInfoGrpc.ShippingInfoGrpcClient client) : IGetShippingInfoCaller
{
    public async Task<GetShippingInfoByOrderIdResponse> GetByOrderId(Guid orderId, CancellationToken ct = default)
    {
        try
        {
            var req = new GetShippingInfoByOrderIdRequest { OrderId = orderId.ToString() };
            return await client.GetByOrderIdAsync(req, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            throw new Exception($"Shipping gRPC failed with status {ex.StatusCode}: {ex.Status.Detail}");
        }
    }
}

public record TestGetShippingInfoRequest(Guid OrderId);

public static class TestGetShippingInfoEndpoint
{
    [WolverinePost("/order/internal-test/shipping-info")]
    public static async Task<GetShippingInfoByOrderIdResponse> Test(
        TestGetShippingInfoRequest request,
        IGetShippingInfoCaller caller,
        CancellationToken ct)
    {
        return await caller.GetByOrderId(request.OrderId, ct);
    }
}

