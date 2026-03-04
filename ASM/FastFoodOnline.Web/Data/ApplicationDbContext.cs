using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FastFoodOnline.Web.Models;

namespace FastFoodOnline.Web.Data
{
    // Kế thừa IdentityDbContext để tích hợp sẵn các bảng Identity (Users, Roles, Claims...)
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Danh sách các bảng trong database
        public DbSet<Category> Categories { get; set; } // Danh mục món ăn
        public DbSet<Food> Foods { get; set; }          // Món ăn
        public DbSet<Combo> Combos { get; set; }        // Combo
        public DbSet<ComboItem> ComboItems { get; set; } // Chi tiết món trong Combo
        public DbSet<Cart> Carts { get; set; }          // Giỏ hàng
        public DbSet<CartItem> CartItems { get; set; }  // Chi tiết giỏ hàng
        public DbSet<Order> Orders { get; set; }        // Đơn hàng
        public DbSet<OrderItem> OrderItems { get; set; } // Chi tiết đơn hàng

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Cần gọi base.OnModelCreating để Identity cấu hình các bảng của nó trước
            base.OnModelCreating(builder);

            builder.Entity<OrderItem>().Ignore(oi => oi.LineTotal);


            // Cấu hình khóa chính phức hợp cho bảng ComboItem (ComboId + FoodId)
            builder.Entity<ComboItem>()
                .HasKey(ci => new { ci.ComboId, ci.FoodId });

            // Đảm bảo Slug của Category là duy nhất (Unique Index)
            builder.Entity<Category>()
                .HasIndex(c => c.Slug)
                .IsUnique();

            // Cấu hình các mối quan hệ (Relationships)

            // Relationship: CartItem -> Food
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Food)
                .WithMany()
                .HasForeignKey(ci => ci.FoodId)
                .IsRequired(false); // FoodId có thể null nếu item là Combo

            // Relationship: CartItem -> Combo
            builder.Entity<CartItem>()
                .HasOne(ci => ci.Combo)
                .WithMany()
                .HasForeignKey(ci => ci.ComboId)
                .IsRequired(false); // ComboId có thể null nếu item là Food lẻ

            // Relationship: OrderItem -> Food
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Food)
                .WithMany()
                .HasForeignKey(oi => oi.FoodId)
                .IsRequired(false);

            // Relationship: OrderItem -> Combo
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Combo)
                .WithMany()
                .HasForeignKey(oi => oi.ComboId)
                .IsRequired(false);

            // FIX CẢNH BÁO DECIMAL (Màu vàng)
            // Tự động set type decimal(18,2) cho tất cả các cột tiền tệ
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?));

                foreach (var property in properties)
                {
                    builder.Entity(entityType.Name).Property(property.Name).HasColumnType("decimal(18,2)");
                }
            }
        }
    }
}