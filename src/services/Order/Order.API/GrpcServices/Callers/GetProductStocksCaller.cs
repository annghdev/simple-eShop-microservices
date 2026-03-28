using Contracts.Protos.InventoryStocks;
using Grpc.Core;
using Wolverine.Http;

namespace Order.GrpcServices;

public interface IGetProductStocksCaller
{
    Task<GetProductStocksResponse> Get(GetProductStocksRequest request, CancellationToken ct = default);
}

public class GetProductStocksCaller(InventoryStockGrpc.InventoryStockGrpcClient inventoryStockClient) : IGetProductStocksCaller
{
    public async Task<GetProductStocksResponse> Get(GetProductStocksRequest request, CancellationToken ct = default)
    {
        try
        {
            return await inventoryStockClient.GetProductStocksAsync(request, cancellationToken: ct);
        }
        catch (RpcException ex)
        {
            throw new Exception($"Inventory gRPC failed with status {ex.StatusCode}: {ex.Status.Detail}");
        }
    }
}


public record TestGetProductStocksRequest(List<TestProductVariantItem> Items);
public record TestProductVariantItem(Guid ProductId, Guid? VariantId);

public static class TestGetProductStocksEndpoint
{
    [WolverinePost("/order/internal-test/product-stocks")]
    public static async Task<GetProductStocksResponse> Test(
        TestGetProductStocksRequest request,
        IGetProductStocksCaller caller,
        CancellationToken ct)
    {
        var grpcRequest = new GetProductStocksRequest();
        grpcRequest.Items.AddRange(request.Items.Select(item => new ProductVariantQuery
        {
            ProductId = item.ProductId.ToString(),
            VariantId = item.VariantId?.ToString() ?? string.Empty
        }));

        return await caller.Get(grpcRequest, ct);
    }
}
