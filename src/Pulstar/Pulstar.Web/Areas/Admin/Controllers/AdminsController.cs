namespace Pulstar.Web.Areas.Admin.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Pulstar.Common.Constants;
    using Pulstar.Common.Enums;
    using Pulstar.Common.Helpers;
    using Pulstar.Models.Products;
    using Pulstar.Services.Interfaces;
    using Pulstar.Web.Areas.Admin.Models.Users;
    using Pulstar.Web.Extensions;
    using Pulstar.Web.Infrastructure.Constants;
    using Pulstar.Web.Infrastructure.Extensions;
    using Pulstar.Web.Models.ProductsViewModels;

    [Area(WebContants.AdminArea)]
    [Authorize(Roles = AppConstants.Administrator + "," + AppConstants.Manager)]
    public class AdminsController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        private readonly IProductService _productService;

        public AdminsController(RoleManager<IdentityRole> roleManager, IUserService userService, IProductService productService)
        {
            _roleManager = roleManager;
            _userService = userService;
            _productService = productService;
        }

        [Authorize(Roles = AppConstants.Administrator)]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToListAsync();
            var users = await _userService.All();
            var model = users
                .Select(u => new ManageUserViewModel
                {
                    UserName = u.UserName,
                    Email = u.Email,
                    Roles = roles,
                })
                .ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductViewModel input, IFormFile image)
        {
            if (!ModelState.IsValid)
            {
                TempData.AddErrorMessage("Some of the inputs contain invalid data.");
            }

            var productModel = new ProductModel
            {
                Description = input.Description,
                Manufacturer = input.Manufacturer,
                Model = input.Model,
                Price = input.Price,
                Discount = input.Discount,
                Quantity = input.Quantity,
                Title = input.Title,
            };
            var byteImage = await image.ToByteArrayAsync();

            await _productService.EditProduct(input.Id, productModel, byteImage);

            return Redirect($"/products/details/{input.Id}");
        }

        [HttpPost]
        public async Task<IActionResult> ManageDiscount(ManageDiscountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.AddErrorMessage("Some of the inputs contain invalid data.");
            }

            try
            {
                await _productService.AddDiscount(model.ProductId, model.Discount);
            }
            catch (Exception)
            {
                TempData.AddErrorMessage("Discount must be in range [1..99] and product must be existing.");
                throw;
            }

            return Redirect($"/products/details/{model.ProductId}");
        }

        [Authorize(Roles = AppConstants.Administrator)]
        public async Task<IActionResult> Manage(string userName, string role)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(role))
            {
                return BadRequest();
            }

            await _userService.AssignToRole(userName, role);

            return View();
        }
    }
}
