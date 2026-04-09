using Microsoft.EntityFrameworkCore;
using Shipping.Domain;

namespace Shipping.Persistence;

public class ShippingDbContext : DbContext
{
    public ShippingDbContext(DbContextOptions<ShippingDbContext> options) : base(options)
    {        
    }

    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<ShipmentTracking> Trackings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Shipment>(b =>
        {
            b.HasIndex(x => x.OrderId).IsUnique();
            b.Property(x => x.DeliveryMethod).HasConversion<int>();
            b.Property(x => x.Status).HasConversion<int>();

            b.HasMany(x => x.Trackings)
                .WithOne(x => x.Shipment)
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ShipmentTracking>(b =>
        {
            b.HasIndex(x => x.ShipmentId);
            b.Property(x => x.Status).HasConversion<int>();
        });
    }
}
