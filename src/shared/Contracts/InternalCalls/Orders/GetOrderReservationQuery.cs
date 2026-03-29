namespace Order.InternalCalls;

public record GetOrderReservationQuery(Guid OrderId);

public class OrderReservationInfo
{
    public Guid OrderId { get; set; }
    public ReservationStatus ReservationStatus { get; set; }
    public List<OrderReservationItem> Items { get; set; } = [];
}

public enum ReservationStatus
{
    Pending,
    Failed,
    Reserved
}

public record OrderReservationItem(Guid ItemId, int Quantity);