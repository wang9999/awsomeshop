using AWSomeShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AWSomeShop.API.Infrastructure.DbContext;

public class AWSomeShopDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public AWSomeShopDbContext(DbContextOptions<AWSomeShopDbContext> options) : base(options)
    {
    }

    // Employee related
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;

    // Product related
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductImage> ProductImages { get; set; } = null!;
    public DbSet<ProductTag> ProductTags { get; set; } = null!;

    // Order related
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    public DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;

    // Points related
    public DbSet<PointsTransaction> PointsTransactions { get; set; } = null!;

    // Admin related
    public DbSet<Administrator> Administrators { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Employee configuration
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.EmployeeNumber).IsUnique();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EmployeeNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Department).HasMaxLength(100);
        });

        // Administrator configuration
        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
        });

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Points);
            // 配置乐观锁
            entity.Property(e => e.RowVersion)
                  .IsRowVersion()
                  .IsConcurrencyToken();
        });

        // ProductImage configuration
        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ImageUrl).HasMaxLength(500).IsRequired();
            entity.HasOne(e => e.Product)
                  .WithMany(p => p.Images)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ProductTag configuration
        modelBuilder.Entity<ProductTag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TagName).HasMaxLength(50).IsRequired();
            entity.HasOne(e => e.Product)
                  .WithMany(p => p.Tags)
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ShoppingCart configuration
        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.EmployeeId, e.ProductId }).IsUnique();
            entity.HasOne(e => e.Employee)
                  .WithMany(emp => emp.ShoppingCarts)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OrderNumber).IsUnique();
            entity.Property(e => e.OrderNumber).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasOne(e => e.Employee)
                  .WithMany(emp => emp.Orders)
                  .HasForeignKey(e => e.EmployeeId);
            entity.HasOne(e => e.Address)
                  .WithMany()
                  .HasForeignKey(e => e.AddressId);
        });

        // OrderItem configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductName).HasMaxLength(200).IsRequired();
            entity.HasOne(e => e.Order)
                  .WithMany(o => o.Items)
                  .HasForeignKey(e => e.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Address configuration
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReceiverName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.ReceiverPhone).HasMaxLength(20).IsRequired();
            entity.Property(e => e.DetailAddress).HasMaxLength(500).IsRequired();
            entity.HasOne(e => e.Employee)
                  .WithMany(emp => emp.Addresses)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // PointsTransaction configuration
        modelBuilder.Entity<PointsTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.ExpiryDate);
            entity.HasOne(e => e.Employee)
                  .WithMany(emp => emp.PointsTransactions)
                  .HasForeignKey(e => e.EmployeeId);
        });

        // Notification configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.HasIndex(e => e.IsRead);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasOne(e => e.Employee)
                  .WithMany()
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AdminName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Action).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EntityType).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Action);
            entity.HasIndex(e => e.EntityType);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasOne(e => e.Admin)
                  .WithMany()
                  .HasForeignKey(e => e.AdminId);
        });
    }
}
