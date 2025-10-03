using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangBanLe.Models;

namespace QuanLyCuaHangBanLe.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets cho các bảng
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình User
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(50).IsRequired();
                entity.Property(e => e.Password).HasColumnName("password").HasMaxLength(255).IsRequired();
                entity.Property(e => e.FullName).HasColumnName("full_name").HasMaxLength(100);
                entity.Property(e => e.Role).HasColumnName("role").HasMaxLength(20);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            // Cấu hình Customer
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customers");
                entity.HasKey(e => e.CustomerId);
                entity.Property(e => e.CustomerId).HasColumnName("customer_id");
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(20);
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100);
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            // Cấu hình Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");
                entity.HasKey(e => e.CategoryId);
                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.CategoryName).HasColumnName("category_name").HasMaxLength(100).IsRequired();
            });

            // Cấu hình Supplier
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("suppliers");
                entity.HasKey(e => e.SupplierId);
                entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(20);
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100);
                entity.Property(e => e.Address).HasColumnName("address");
            });

            // Cấu hình Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");
                entity.HasKey(e => e.ProductId);
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.SupplierId).HasColumnName("supplier_id");
                entity.Property(e => e.ProductName).HasColumnName("product_name").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Barcode).HasColumnName("barcode").HasMaxLength(50);
                entity.Property(e => e.Price).HasColumnName("price").HasColumnType("decimal(10,2)");
                entity.Property(e => e.Unit).HasColumnName("unit").HasMaxLength(20);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                // Quan hệ với Category
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Quan hệ với Supplier
                entity.HasOne(e => e.Supplier)
                    .WithMany(s => s.Products)
                    .HasForeignKey(e => e.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Cấu hình Inventory
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.ToTable("inventory");
                entity.HasKey(e => e.InventoryId);
                entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                // Quan hệ với Product
                entity.HasOne(e => e.Product)
                    .WithOne(p => p.Inventory)
                    .HasForeignKey<Inventory>(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Cấu hình Promotion
            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.ToTable("promotions");
                entity.HasKey(e => e.PromoId);
                entity.Property(e => e.PromoId).HasColumnName("promo_id");
                entity.Property(e => e.PromoCode).HasColumnName("promo_code").HasMaxLength(50).IsRequired();
                entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(255);
                entity.Property(e => e.DiscountType).HasColumnName("discount_type").HasMaxLength(20);
                entity.Property(e => e.DiscountValue).HasColumnName("discount_value").HasColumnType("decimal(10,2)");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
                entity.Property(e => e.EndDate).HasColumnName("end_date");
                entity.Property(e => e.MinOrderAmount).HasColumnName("min_order_amount").HasColumnType("decimal(10,2)");
                entity.Property(e => e.UsageLimit).HasColumnName("usage_limit");
                entity.Property(e => e.UsedCount).HasColumnName("used_count");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20);
            });

            // Cấu hình Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");
                entity.HasKey(e => e.OrderId);
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.CustomerId).HasColumnName("customer_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.PromoId).HasColumnName("promo_id");
                entity.Property(e => e.OrderDate).HasColumnName("order_date");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20);
                entity.Property(e => e.TotalAmount).HasColumnName("total_amount").HasColumnType("decimal(10,2)");
                entity.Property(e => e.DiscountAmount).HasColumnName("discount_amount").HasColumnType("decimal(10,2)");

                // Quan hệ với Customer
                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Quan hệ với User
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Quan hệ với Promotion
                entity.HasOne(e => e.Promotion)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(e => e.PromoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Cấu hình OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("order_items");
                entity.HasKey(e => e.OrderItemId);
                entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.Price).HasColumnName("price").HasColumnType("decimal(10,2)");
                entity.Property(e => e.Subtotal).HasColumnName("subtotal").HasColumnType("decimal(10,2)");

                // Quan hệ với Order
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Quan hệ với Product
                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Cấu hình Payment
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("payments");
                entity.HasKey(e => e.PaymentId);
                entity.Property(e => e.PaymentId).HasColumnName("payment_id");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.Amount).HasColumnName("amount").HasColumnType("decimal(10,2)");
                entity.Property(e => e.PaymentMethod).HasColumnName("payment_method").HasMaxLength(50);
                entity.Property(e => e.PaymentDate).HasColumnName("payment_date");

                // Quan hệ với Order
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.Payments)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
