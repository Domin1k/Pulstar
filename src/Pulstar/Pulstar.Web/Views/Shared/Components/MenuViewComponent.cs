namespace Pulstar.Web.Views.Shared.Components
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Pulstar.Common.Enums;
    using Pulstar.Services.Interfaces;
    using Pulstar.Web.Models.CategoryViewModels;

    public class MenuViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public MenuViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var accessories = await _categoryService.All(CategoryType.Accessory.ToString());
            var games = await _categoryService.All(CategoryType.Game.ToString());
            var consoles = await _categoryService.All(CategoryType.Console.ToString());
            var viewModel = new CategoryMenuViewModel
            {
                Games = games.Select(c => c.Name).ToList(),
                Consoles = consoles.Select(c => c.Name).ToList(),
                Accessories = accessories.Select(c => c.Name).ToList(),
            };
            return View("_MainMenu", viewModel);
        }
    }
}