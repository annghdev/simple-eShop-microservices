using Microsoft.EntityFrameworkCore;
using Order.Domain;
namespace Order;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Order> Orders { get; set; }
}
