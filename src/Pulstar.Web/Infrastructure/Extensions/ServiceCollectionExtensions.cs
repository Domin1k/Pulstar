namespace Pulstar.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using Pulstar.Data;
    using Pulstar.Data.Interfaces;
    using Pulstar.Data.Repositories;
    using Pulstar.DataModels;
    using Pulstar.Services;
    using Pulstar.Services.Interfaces;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            serviceCollection.AddScoped<ICategoryService, CategoryService>();
            serviceCollection.AddScoped<IProductService, ProductService>();
            serviceCollection.AddScoped<IPurchaseService, PurchaseService>();
            serviceCollection.AddScoped<IUserAccountService, UserAccountService>();
            serviceCollection.AddScoped<IUserService, UserService>();

            return serviceCollection;
        }

        public static IServiceCollection ConfigureIdentity(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddIdentity<User, IdentityRole>(opts =>
            {
                opts.Password.RequireDigit = false;
                opts.Password.RequiredLength = 4;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<PulstarDbContext>()
                .AddDefaultTokenProviders();

            return serviceCollection;
        }
    }
}
