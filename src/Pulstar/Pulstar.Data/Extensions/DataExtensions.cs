namespace Pulstar.Data.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Pulstar.Common.Constants;
    using Pulstar.Common.Enums;
    using Pulstar.Data.Models;

    public static class DataExtensions
    {
        public static async Task EnsureSeedDefaultUserWithRolesData(this RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            const string password = "Secr3dP4ss";
            const string adminEmail = "admin@mysite.com";

            if (!await roleManager.RoleExistsAsync(AppConstants.Administrator))
            {
                var role = new IdentityRole
                {
                    Name = AppConstants.Administrator,
                };

                await roleManager.CreateAsync(role);
                var admin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                };

                if ((await userManager.CreateAsync(admin, password)).Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, AppConstants.Administrator);
                }
            }
        }

        public static async Task EnsureSeedCategories(this PulstarDbContext context)
        {
            if (context.Categories.Any())
            {
                return;
            }

            var consoles = new List<string> { "PS3", "PS4", "XBox One" };
            var accessories = new List<string> { "iOS", "PlayStation VR", "Switch", "PS4", "Xbox One", "PC", "PS3", "Xbox 360", "WiiU", "PS", "Vita", "3DS", "PSP", "PS2", "Wii", "NDS Nintendo", "Classic" };
            var games = new List<string> { "PlayStation VR", "PS4", "Xbox One", "PC", "PS3", "Xbox 360", "WiiU", "PS Vita", "Switch", "3DS", "PSP", "PS2", "WiiNDSМобилниCard & Board" };

            consoles
                .ForEach(i =>
                {
                    var category = new Category
                    {
                        Name = i,
                        Type = CategoryType.Console,
                    };
                    context.Categories.Add(category);
                });

            accessories
                .ForEach(i =>
                {
                    var category = new Category
                    {
                        Name = i,
                        Type = CategoryType.Accessory,
                    };
                    context.Categories.Add(category);
                });

            games
               .ForEach(i =>
               {
                   var category = new Category
                   {
                       Name = i,
                       Type = CategoryType.Game,
                   };
                   context.Categories.Add(category);
               });

            await context.SaveChangesAsync();
        }
    }
}
