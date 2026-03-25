using Inventory.Features.InventoryItems;
using Inventory.IntegrationEvents;
using Order.IntegrationEvents;
using Wolverine.Persistence.Sagas;

namespace Inventory.Messaging;

public record InititionTimeout(Guid Id) : TimeoutMessage(30.Seconds());

public class ReservationSaga : Saga
{
    // OrderId
    public Guid Id { get; set; }
    public int TotalItems { get; set; }
    public HashSet<Guid> Reserved { get; set; } = [];
    public Dictionary<Guid, int> Quantities { get; set; } = [];

    public bool AllReserved => Reserved.Count == TotalItems;
    public bool Failed { get; set; }

    public static (ReservationSaga, object[]) Start(
        [SagaIdentityFrom("OrderId")] OrderPlaced e,
        ILogger<ReservationSaga> logger)
    {
        logger.LogInformation("Got a new Order with ID {Id}", e.OrderId);
        var saga = new ReservationSaga
        {
            Id = e.OrderId,
            TotalItems = e.Items.Count
        };
        foreach (var item in e.Items)
        {
            saga.Quantities[item.InventoryItemId] = item.Quantity;
        }
        var reserveCmds = new List<StartReservationCommand>();

        foreach (var item in e.Items)
        {
            reserveCmds.Add(new StartReservationCommand(item.InventoryItemId, item.Quantity, e.OrderId));
        }
        var inititionTimeout = new InititionTimeout(saga.Id);

        return (saga, [.. reserveCmds, inititionTimeout]);
    }

    public IEnumerable<object> Handle(
        [SagaIdentityFrom("OrderId")] ReservationSucceeded e,
        ILogger<ReservationSaga> logger)
    {
        logger.LogDebug("Order with ID {Id} reserved for Item {ItemId}", e.OrderId, e.Id);

        if (Failed)
        {
            // Compensate
            yield return new ReleaseReservationCommand(e.Id, e.Quantity, e.OrderId);
            yield break;
        }

        // Idempotent
        if (!Reserved.Add(e.Id))
            yield break;


        // All reservation success -> complete
        if (AllReserved)
        {
            logger.LogInformation("Order {Id} reservation succeeded", Id);
        }
    }

    // FAIL FAST
    public IEnumerable<object> Handle(
        [SagaIdentityFrom("OrderId")] ReservationFailed e,
        ILogger<ReservationSaga> logger)
    {
        logger.LogDebug("Order with ID {Id} reserve fail for Item {ItemId}", e.OrderId, e.Id);

        // Idempotent
        if (Failed)
            yield break;

        Failed = true;

        foreach (var cmd in BuildRollbackCommands())
        {
            yield return cmd;
        }

        yield return new InventoryReservationFailed(Id);
    }

    #region Timeout handling
    public void Handle(InititionTimeout timeout, ILogger<InititionTimeout> logger)
    {
        MarkCompleted();
    }
    #endregion

    private IEnumerable<object> BuildRollbackCommands()
    {
        foreach (var itemId in Reserved)
        {
            yield return new ReleaseReservationCommand(itemId, Quantities[itemId], Id);
        }
    }
}