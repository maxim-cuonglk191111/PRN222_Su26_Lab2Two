using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessObjects;

public class MyStoreContext : DbContext
{
    public MyStoreContext(DbContextOptions<MyStoreContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<AccountMember> AccountMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("lab2");

        modelBuilder.Entity<AccountMember>(entity =>
        {
            entity.ToTable("AccountMember", "lab2");
            entity.HasKey(a => a.MemberId);
            entity.Property(a => a.MemberId).ValueGeneratedNever();
            entity.Property(a => a.MemberPassword).IsRequired().HasMaxLength(100);
            entity.Property(a => a.MemberRole).IsRequired();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category", "lab2");
            entity.HasKey(c => c.CategoryId);
            entity.Property(c => c.CategoryId).UseIdentityColumn();
            entity.Property(c => c.CategoryName).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product", "lab2");
            entity.HasKey(p => p.ProductId);
            entity.Property(p => p.ProductId).UseIdentityColumn();
            entity.Property(p => p.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(p => p.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(p => p.UnitsInStock).IsRequired();

            entity.HasOne(p => p.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(p => p.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
