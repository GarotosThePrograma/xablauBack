using Microsoft.EntityFrameworkCore;
using xablau.Entities;

namespace xablau.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Favorite> Favorites { get; set; } = null!;
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<CartItem> CartItems { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(user => user.Email).IsUnique();

            entity.Property(user => user.Role)
                .HasConversion<string>();
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasIndex(favorite => new { favorite.UserId, favorite.ProductId })
                .IsUnique();

            entity.HasOne(favorite => favorite.User)
                .WithMany()
                .HasForeignKey(favorite => favorite.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(favorite => favorite.Product)
                .WithMany()
                .HasForeignKey(favorite => favorite.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasIndex(cart => cart.UserId).IsUnique();

            entity.HasOne(cart => cart.User)
                .WithMany()
                .HasForeignKey(cart => cart.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasIndex(item => new { item.CartId, item.ProductId })
                .IsUnique();

            entity.HasOne(item => item.Cart)
                .WithMany(cart => cart.Items)
                .HasForeignKey(item => item.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(item => item.Product)
                .WithMany()
                .HasForeignKey(item => item.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(order => order.User)
                .WithMany()
                .HasForeignKey(order => order.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasOne(item => item.Order)
                .WithMany(order => order.Items)
                .HasForeignKey(item => item.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
