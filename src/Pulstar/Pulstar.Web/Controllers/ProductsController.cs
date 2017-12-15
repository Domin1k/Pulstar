namespace Pulstar.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Pulstar.Models.Products;
    using Pulstar.Services.Interfaces;
    using Pulstar.Web.Models.ProductsViewModels;

    [Route("products")]
    public class ProductsController : Controller
    {
        private const string ProductsViewName = "Products";

        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("games/{game}")]
        public async Task<IActionResult> Games(string game)
        {
            var products = await GetProducts(game);
            if (products == null)
            {
                return BadRequest();
            }

            return View(ProductsViewName, products);
        }

        [HttpGet("consoles/{console}")]
        public async Task<IActionResult> Consoles(string console)
        {
            var products = await GetProducts(console);
            if (products == null)
            {
                return BadRequest();
            }

            return View(ProductsViewName, products);
        }

        [HttpGet("accessories/{accessory}")]
        public async Task<IActionResult> Accessories(string accessory)
        {
            var products = await GetProducts(accessory);
            if (products == null)
            {
                return BadRequest();
            }

            return View(ProductsViewName, products);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var model = await _productService.ViewDetails(id);

            if (model == null)
            {
                return NotFound();
            }

            var viewModel = Mapper.Map<ProductViewModel>(model);
            return View(viewModel);
        }
        
        private async Task<IEnumerable<ProductListingModel>> GetProducts(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return null;
            }

            ViewBag.CategoryName = categoryName;
            try
            {
                return await _productService.All(categoryName);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
