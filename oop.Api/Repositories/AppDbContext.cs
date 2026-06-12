using oop.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace oop.Api.Repositories;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        // tell EF Core to ignore the computed property — not a real column
        modelBuilder.Entity<Order>()
            .Ignore(o => o.TotalAmount);

        modelBuilder.Entity<OrderItem>()
            .Ignore(i => i.TotalPrice);
    }
}