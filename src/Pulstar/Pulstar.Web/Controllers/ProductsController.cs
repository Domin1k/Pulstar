namespace Pulstar.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Pulstar.Common.Enums;
    using Pulstar.Data.Models;
    using Pulstar.Models.Products;
    using Pulstar.Services.Interfaces;
    using Pulstar.Web.Infrastructure.Extensions;
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

        [HttpGet("games/{game}/{criteria?}/{order?}")]
        public async Task<IActionResult> Games(string game, string criteria, string order)
        {
            IEnumerable<ProductListingModel> products = null;
            try
            {
                var expression = GetCriteria(criteria);
                var orderType = GetOrderType(order);
                products = await GetProducts(game, expression, orderType);
                if (products == null)
                {
                    TempData.AddErrorMessage($"There are no products for {game}");
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData.AddErrorMessage(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                TempData.AddErrorMessage(ex.Message);
            }

            return View(ProductsViewName, products ?? Enumerable.Empty<ProductListingModel>());
        }

        [HttpGet("consoles/{console}/{criteria?}/{order?}")]
        public async Task<IActionResult> Consoles(string console, string criteria, string order)
        {
            IEnumerable<ProductListingModel> products = null;
            try
            {
                var expression = GetCriteria(criteria);
                var orderType = GetOrderType(order);
                products = await GetProducts(console, expression, orderType);
                if (products == null)
                {
                    TempData.AddErrorMessage($"There are no products for {console}");
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData.AddErrorMessage(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                TempData.AddErrorMessage(ex.Message);
            }

            return View(ProductsViewName, products ?? Enumerable.Empty<ProductListingModel>());
        }

        [HttpGet("accessories/{accessory}/{criteria?}/{order?}")]
        public async Task<IActionResult> Accessories(string accessory, string criteria, string order)
        {
            IEnumerable<ProductListingModel> products = null;
            try
            {
                var expression = GetCriteria(criteria);
                var orderType = GetOrderType(order);
                products = await GetProducts(accessory, expression, orderType);
                if (products == null)
                {
                    TempData.AddErrorMessage($"There are no products for {accessory}");
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData.AddErrorMessage(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                TempData.AddErrorMessage(ex.Message);
            }

            return View(ProductsViewName, products ?? Enumerable.Empty<ProductListingModel>());
        }

        [HttpGet("details/{id}")]
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

        private async Task<IEnumerable<ProductListingModel>> GetProducts(string categoryName, Expression<Func<Product, object>> orderPredicate, OrderType orderType)
        {
            if (string.IsNullOrEmpty(categoryName))
            {
                return null;
            }

            ViewBag.CategoryName = categoryName;
            return await _productService.All(categoryName, orderPredicate, orderType);
        }

        private Expression<Func<Product, object>> GetCriteria(string inputCriteria)
        {
            switch (inputCriteria?.ToLowerInvariant())
            {
                case null:
                case "":
                    return p => p.Id;
                case "price": return p => p.Price;
                case "discount": return p => p.Discount;
                default: throw new NotSupportedException($"{inputCriteria} is not supporter sorting criteria.");
            }
        }

        private OrderType GetOrderType(string inputOrder)
        {
            if (string.IsNullOrEmpty(inputOrder))
            {
                return OrderType.Ascending;
            }

            return (OrderType)Enum.Parse(typeof(OrderType), inputOrder, true);
        }
    }
}
