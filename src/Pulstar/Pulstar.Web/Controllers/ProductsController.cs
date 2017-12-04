namespace Pulstar.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Pulstar.Services.Interfaces;

    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IActionResult Games()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Consoles()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Accessories()
        {
            return View();
        }
    }
}
