namespace Pulstar.Data.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Pulstar.Common.Constants;
    using Pulstar.Common.Enums;
    using Pulstar.DataModels;

    public static class DataExtensions
    {
        public static async Task EnsureSeedDefaultUserWithRolesData(this RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            const string password = "Secr3dP4ss";
            const string adminEmail = "admin@mysite.com";
            const string managerEmail = "manager@mysite.com";

            await SeedUser(roleManager, userManager, AppConstants.Administrator, adminEmail, password);
            await SeedUser(roleManager, userManager, AppConstants.Manager, managerEmail, password);
        }

        public static async Task EnsureSeedGames(this PulstarDbContext context, string defaultImageFilePath)
        {
            const int gamesPerCategory = 5;

            var categories = context.Categories.Where(c => c.Type == CategoryType.Game).ToList();
            if (categories.Count == 0 || context.Products.Any())
            {
                return;
            }

            var random = new Random();
            foreach (var category in categories)
            {
                for (int i = 0; i < gamesPerCategory; i++)
                {
                    var manufacturer = i % 2 == 0 ? "Sony" : "Samsung";
                    var title = i % 2 == 0 ? "PS" : "PC";

                    var game = new Product
                    {
                        CategoryId = category.Id,
                        Manufacturer = $"{manufacturer} Enterprise",
                        Price = random.Next(20, 400),
                        Title = $"title{i}",
                        Quantity = random.Next(0, 100),
                        Discount = random.Next(0, 100),
                        Model = $"{title}{i + 1}",
                        Image = File.ReadAllBytes(defaultImageFilePath),
                        Description = @"Lorem ipsum dolor sit amet, dico congue reprimique an quo. Ex sea solum rebum dolorem. Duis malis fierent ea vis, ne his idque soluta. Prima idque debet cu vel, mea illum libris ei. Mundi elitr suscipit vel ad, accumsan repudiandae te sit. Unum luptatum vis et.",
                    };
                    context.Products.Add(game);
                }

                await context.SaveChangesAsync();
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

        private static async Task SeedUser(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, string roleName, string email, string password)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole
                {
                    Name = roleName,
                };

                await roleManager.CreateAsync(role);
                var admin = new User
                {
                    UserName = email,
                    Email = email,
                };

                if ((await userManager.CreateAsync(admin, password)).Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, roleName);
                }
            }
        }
    }
}
