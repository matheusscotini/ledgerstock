using LedgerStock.Domain.Entities;
using LedgerStock.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LedgerStock.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(120);

            entity.Property(p => p.Sku)
                .IsRequired()
                .HasMaxLength(40);

            entity.HasIndex(p => p.Sku)
                .IsUnique();

            entity.Property(p => p.Description)
                .HasMaxLength(500);

            entity.Property(p => p.Category)
                .HasMaxLength(80);

            entity.Property(p => p.Price)
                .HasPrecision(18, 2);

            entity.Property(p => p.MinimumStock)
                .IsRequired();

            entity.Property(p => p.IsActive)
                .HasDefaultValue(true);
        });

        builder.Entity<StockMovement>(entity =>
        {
            entity.ToTable("StockMovements");

            entity.HasKey(m => m.Id);

            entity.Property(m => m.Quantity)
                .IsRequired();

            entity.Property(m => m.Reason)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(m => m.Notes)
                .HasMaxLength(500);

            entity.Property(m => m.PerformedByUserId)
                .IsRequired();

            entity.Property(m => m.Type)
                .IsRequired();

            entity.HasOne(m => m.Product)
                .WithMany(p => p.Movements)
                .HasForeignKey(m => m.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}