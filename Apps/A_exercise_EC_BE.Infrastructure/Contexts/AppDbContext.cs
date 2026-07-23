using A_exercise_EC_BE.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace A_exercise_EC_BE.Infrastructure.Contexts;

/// <summary>
/// 管理側と共有するPostgreSQLデータベースのコンテキスト。
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<CustomerEntity> Customers => Set<CustomerEntity>();
    public DbSet<ProductCategoryEntity> ProductCategories => Set<ProductCategoryEntity>();
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    public DbSet<ProductStockEntity> ProductStocks => Set<ProductStockEntity>();

    public DbSet<PaymentMethodEntity> PaymentMethods => Set<PaymentMethodEntity>();
    public DbSet<OrderStatusEntity> OrderStatuses => Set<OrderStatusEntity>();
    public DbSet<OrdersEntity> Orders => Set<OrdersEntity>();
    public DbSet<OrdersDetailEntity> OrdersDetails => Set<OrdersDetailEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerEntity>(entity =>
        {
            entity.HasIndex(customer => customer.CustomerUuid).IsUnique();
            entity.HasIndex(customer => customer.MailAddress).IsUnique();
            entity.HasIndex(customer => customer.Username).IsUnique();
            entity.Property(customer => customer.Name).HasMaxLength(20).IsRequired();
            entity.Property(customer => customer.Kana).HasMaxLength(20);
            entity.Property(customer => customer.Address1).HasMaxLength(100).IsRequired();
            entity.Property(customer => customer.Address2).HasMaxLength(100);
            entity.Property(customer => customer.PhoneNumber).HasMaxLength(20).IsRequired();
            entity.Property(customer => customer.MailAddress).HasMaxLength(200).IsRequired();
            entity.Property(customer => customer.Username).HasMaxLength(30).IsRequired();
            entity.Property(customer => customer.Password).HasMaxLength(255).IsRequired();
        });

        modelBuilder.Entity<ProductCategoryEntity>(entity =>
        {
            entity.HasIndex(category => category.CategoryUuid).IsUnique();
            entity.Property(category => category.Name).HasMaxLength(30).IsRequired();
        });

        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.HasIndex(product => product.ProductUuid).IsUnique();
            entity.Property(product => product.Name).HasMaxLength(100).IsRequired();
            entity.Property(product => product.ImageUrl).HasMaxLength(200);
            entity.Property(product => product.DeleteFlg).HasDefaultValue(0);

            entity.HasOne(product => product.ProductCategory)
                .WithMany(category => category.Products)
                .HasForeignKey(product => product.ProductCategoryId)
                .HasConstraintName("fk_product_category")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(product => product.ProductStock)
                .WithOne(stock => stock.Product)
                .HasForeignKey<ProductStockEntity>(stock => stock.ProductId)
                .HasConstraintName("fk_product_stock_product")
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductStockEntity>(entity =>
        {
            entity.HasIndex(stock => stock.StockUuid).IsUnique();
        });
    }
}
