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
    using Pulstar.DataModels;
    using Pulstar.Models.Products;
    using Pulstar.Services.Interfaces;
    using Pulstar.Web.Infrastructure.Constants;
    using Pulstar.Web.Infrastructure.Extensions;
    using Pulstar.Web.Models.ProductsViewModels;

    [Route(RouteConstants.ProductsController)]
    public class ProductsController : Controller
    {
        private const string ProductsViewName = "Products";

        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet(RouteConstants.Games)]
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
                    TempData.AddErrorMessage(string.Format(TempMessages.NoProductsError, game));
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

        [HttpGet(RouteConstants.Consoles)]
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
                    TempData.AddErrorMessage(string.Format(TempMessages.NoProductsError, console));
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

        [HttpGet(RouteConstants.Accessories)]
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
                    TempData.AddErrorMessage(string.Format(TempMessages.NoProductsError, accessory));
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

        [HttpGet(RouteConstants.ProductsDetails)]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                TempData.AddErrorMessage(TempMessages.InvalidProduct);
                return RedirectToAction(nameof(HomeController.Index));
            }

            var model = await _productService.ViewDetails(id);

            if (model == null)
            {
                TempData.AddErrorMessage(TempMessages.InvalidProduct);
                return RedirectToAction(nameof(HomeController.Index));
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
