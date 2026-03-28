namespace Order.Domain;

public class Basket
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? GuestId { get; set; }
    public List<BasketItem> Items { get; set; } = [];
}

public class BasketItem
{
    public Guid ProductId { get; set; }
    public Guid? VariantId { get; set; }
    public int Quantity { get; set; }
}
