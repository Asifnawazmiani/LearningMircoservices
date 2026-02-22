using Infrastructure.Persistence;
using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure;

public class InventoryDbContext : BaseDbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
    }

    public DbSet<ProductInventoryEntity> ProductInventories => Set<ProductInventoryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductInventoryEntity>(entity =>
        {
            entity.ToTable("ProductInventories");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ProductId).IsRequired();
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Sku).IsRequired().HasMaxLength(50);
            entity.Property(e => e.QuantityOnHand).IsRequired();
            entity.Property(e => e.ReservedQuantity).IsRequired();
            entity.Property(e => e.ReorderLevel).IsRequired();
            entity.Property(e => e.ReorderQuantity).IsRequired();
            entity.Property(e => e.Location).IsRequired().HasMaxLength(100);

            entity.Ignore(e => e.AvailableQuantity);

            entity.HasIndex(e => e.ProductId).IsUnique();
            entity.HasIndex(e => e.Sku).IsUnique();
            entity.HasIndex(e => e.Location);
        });
    }
}
