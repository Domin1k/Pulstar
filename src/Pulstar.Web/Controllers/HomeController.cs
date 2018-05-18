namespace Pulstar.Web.Controllers
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Pulstar.Common.Enums;
    using Pulstar.Services.Interfaces;
    using Pulstar.Web.Infrastructure.Constants;
    using Pulstar.Web.Models;

    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet(RouteConstants.SearchTerm)]
        public async Task<IActionResult> Index(string searchTerm)
        {
            var products = string.IsNullOrEmpty(searchTerm)
                ? await _productService.All(null, p => p.Discount, OrderType.Descending, WebContants.NumberOfTopDiscounts)
                : await _productService.All(p => p.Title == searchTerm || p.Model == searchTerm || p.Category.Name == searchTerm, p => p.Discount, OrderType.Descending, WebContants.NumberOfTopDiscounts);
            TempData["searchTerm"] = searchTerm;
            return View(products);
        }
        
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
