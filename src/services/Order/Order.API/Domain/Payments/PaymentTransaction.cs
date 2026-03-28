namespace Order.Domain;

public class PaymentTransaction
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string? ExternalCode { get; set; }
    public decimal Amount { get; set; }
    public PaymentGateway? Gateway { get; set; }
    public PaymentMethod Method { get; set; }
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    public PaymentStatus Status { get; set; }
}

public enum PaymentGateway
{
    VnPay,
    Momo,
    ZaloPay,
    Paypal
}

public enum PaymentMethod
{
    // After Ship
    COD = 0,

    // Before Ship
    Online = 1,
    BankTransfer = 2,
}

public enum PaymentStatus
{
    Initialized,
    Cancelled,
    Paid,
    Refunded,
}