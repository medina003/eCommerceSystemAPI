using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eCommerceRESTfulAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace eCommerceRESTfulAPI.Infrastructure.Data.DbContexts
{
    public class eCommerceSystemDbContext : DbContext
    {
        public eCommerceSystemDbContext(DbContextOptions<eCommerceSystemDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(256);
                entity.Property(u => u.PasswordHash)
                    .IsRequired();
                entity.Property(u => u.Role)
                    .IsRequired();
                entity.HasIndex(u => u.Email)
                    .IsUnique();
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.UserId)
                    .IsRequired();
                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(c => c.Orders)
                    .WithOne(o => o.Customer)
                    .HasForeignKey(o => o.CustomerId);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name)
                    .IsRequired();
                entity.Property(c => c.Description)
                    .IsRequired(false);
                entity.HasMany(c => c.Localizations)
            .WithOne()
            .HasForeignKey(cl => cl.CategoryId);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name)
                    .IsRequired();
                entity.Property(p => p.Description)
                    .IsRequired(false);
                entity.Property(p => p.Price)
                    .IsRequired();
                entity.Property(p => p.StockQuantity)
                    .IsRequired();
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId);
                entity.HasMany(p => p.Localizations)
            .WithOne()
            .HasForeignKey(pl => pl.ProductId);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.OrderDate)
                    .IsRequired();
                entity.HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CustomerId);
                entity.Property(o => o.Status)
                    .IsRequired();
                 entity.Property(o => o.TotalPrice)
            .HasColumnType("decimal(18,2)") 
            .HasDefaultValue(0);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);
                entity.Property(oi => oi.Quantity)
                    .IsRequired();
                entity.Property(oi => oi.UnitPrice)
                    .IsRequired();
                entity.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId);
                entity.HasOne(oi => oi.Product)
                    .WithMany()
                    .HasForeignKey(oi => oi.ProductId);
            });
        }
    }
}
