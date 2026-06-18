using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessObjects;

public class MyStoreContext : DbContext
{
    public MyStoreContext(DbContextOptions<MyStoreContext> options) : base(options) { }

    public DbSet<AccountMember> AccountMembers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // All tables live in the [store] schema – keeps multiple projects
        // isolated in the shared MonsterASP.NET database instance.
        modelBuilder.HasDefaultSchema("lab2");

        modelBuilder.Entity<AccountMember>(e =>
        {
            e.ToTable("AccountMember");
            e.HasKey(a => a.MemberID);
            e.HasIndex(a => a.EmailAddress)
             .IsUnique()
             .HasDatabaseName("UQ_AccountMember_Email");
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable("Categories");
            e.HasIndex(c => c.CategoryName)
             .IsUnique()
             .HasDatabaseName("UQ_Categories_Name");
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.HasOne(p => p.Category)
             .WithMany(c => c.Products)
             .HasForeignKey(p => p.CategoryID)
             .HasConstraintName("FK_Products_Categories")
             .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(p => p.CategoryID)
             .HasDatabaseName("IX_Products_CategoryID");
        });
    }
}
