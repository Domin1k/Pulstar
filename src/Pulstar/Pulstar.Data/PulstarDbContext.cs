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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(cp => cp.CategoryId);

            builder.Entity<User>()
                .HasMany(u => u.Products);

            builder.Entity<User>()
                .HasMany(u => u.CreditCards)
                .WithOne(c => c.Owner)
                .HasForeignKey(cu => cu.OwnerId);
        }
    }
}
