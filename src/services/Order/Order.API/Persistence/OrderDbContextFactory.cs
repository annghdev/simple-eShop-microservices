using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Order.Persistence;

public class OrderDbContextFactory : IDesignTimeDbContextFactory<OrderDbContext>
{
    private const string DesignTimeConnectionString =
        "Host=localhost;Port=5432;Database=orderdb;Username=postgres;Password=postgres";

    public OrderDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();
        optionsBuilder.UseNpgsql(DesignTimeConnectionString);

        return new OrderDbContext(optionsBuilder.Options);
    }
}
