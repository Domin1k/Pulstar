namespace Pulstar.Web.Infrastructure.Extensions
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Pulstar.Data;
    using Pulstar.Data.Extensions;
    using Pulstar.DataModels;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder ConfigureInitialServiceSetup(this IApplicationBuilder app, IHostingEnvironment env)
        {
            const string DefaultImagePath = "{0}\\wwwroot\\images\\defaultImg.jpg";
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<PulstarDbContext>();
                context.Database.Migrate();

                if (env.IsDevelopment())
                {
                    Task.Run(async () =>
                    {
                        await context.EnsureSeedCategories();
                        await context.EnsureSeedGames(string.Format(DefaultImagePath, env.ContentRootPath));
                    })
                    .GetAwaiter()
                    .GetResult();

                    var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
                    var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                    Task.Run(async () => await roleManager.EnsureSeedDefaultUserWithRolesData(userManager))
                        .GetAwaiter()
                        .GetResult();
                }

                return app;
            }
        }
    }
}
