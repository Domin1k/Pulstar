namespace Pulstar.Web
{
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Pulstar.Common;
    using Pulstar.Data;
    using Pulstar.Data.Extensions;
    using Pulstar.Data.Models;
    using Pulstar.Services;
    using Pulstar.Services.Interfaces;
    using Pulstar.Web.Services;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PulstarDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>(opts =>
                    {
                        opts.Password.RequireDigit = false;
                        opts.Password.RequiredLength = 4;
                        opts.Password.RequireLowercase = false;
                        opts.Password.RequireUppercase = false;
                        opts.Password.RequireNonAlphanumeric = false;
                    })
                .AddEntityFrameworkStores<PulstarDbContext>()
                .AddDefaultTokenProviders();

            services.AddAutoMapper();

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IPurchaseService, PurchaseService>();
            services.AddTransient<IUserAccountService, UserAccountService>();
            services.AddTransient<IUserService, UserService>();

            services.AddMvc();
            services.AddSession();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();

                // TODO
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = serviceScope.ServiceProvider.GetService<PulstarDbContext>();
                    context.Database.Migrate();

                    Task.Run(async () =>
                            {
                                await context.EnsureSeedCategories();
                                await context.EnsureSeedGames($"{env.ContentRootPath}\\wwwroot\\images\\defaultImg.jpg");
                            })
                        .GetAwaiter()
                        .GetResult();

                    var userManager = serviceScope.ServiceProvider.GetService<UserManager<User>>();
                    var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

                    Task.Run(async () => await roleManager.EnsureSeedDefaultUserWithRolesData(userManager))
                        .GetAwaiter()
                        .GetResult();
                }
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "areas",
                  template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
