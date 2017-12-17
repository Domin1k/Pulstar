namespace Pulstar.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class PulstarDbContext : IdentityDbContext<User>
    {
        public PulstarDbContext(DbContextOptions<PulstarDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Store> Stores { get; set; }

        public DbSet<CreditCard> CreditCards { get; set; }

        public DbSet<Purchase> Purchases { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(cp => cp.CategoryId);

            builder.Entity<User>()
                .HasMany(u => u.Purchases)
                .WithOne(p => p.User)
                .HasForeignKey(pu => pu.UserId);

            builder.Entity<User>()
                .HasMany(u => u.CreditCards)
                .WithOne(c => c.Owner)
                .HasForeignKey(cu => cu.OwnerId);

            builder.Entity<Purchase>()
                .HasMany(p => p.Products)
                .WithOne(pr => pr.Purchase)
                .HasForeignKey(pp => pp.PurchaseId);
        }
    }
}
