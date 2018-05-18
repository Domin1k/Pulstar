namespace Pulstar.Data
{
    using DataModels;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

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
    }
}
