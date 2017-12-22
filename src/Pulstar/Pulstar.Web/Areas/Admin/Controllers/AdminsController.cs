namespace Pulstar.Web.Areas.Admin.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Pulstar.Common.Constants;
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
        private const string ProductDetails = "/products/details/{0}";
        private const string AdminController = "Admins";
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
            if (image.Length > AppConstants.MaximumImageSizeInBytesAllowed)
            {
                TempData.AddErrorMessage(string.Format(TempMessages.MaximumImageSizeExceeded, AppConstants.MaximumImageSizeInBytesAllowed));
                return Redirect(string.Format(ProductDetails, input.Id));
            }

            if (!ModelState.IsValid)
            {
                TempData.AddErrorMessage(TempMessages.GeneralInvalidInputData);
                return Redirect(string.Format(ProductDetails, input.Id));
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

            return Redirect(string.Format(ProductDetails, input.Id));
        }

        [HttpPost]
        public async Task<IActionResult> ManageDiscount(ManageDiscountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.AddErrorMessage(TempMessages.GeneralInvalidInputData);
                return Redirect(string.Format(ProductDetails, model.ProductId));
            }

            try
            {
                await _productService.AddDiscount(model.ProductId, model.Discount);
            }
            catch (InvalidOperationException)
            {
                TempData.AddErrorMessage(TempMessages.InvalidDiscount);
            }

            return Redirect(string.Format(ProductDetails, model.ProductId));
        }

        [Authorize(Roles = AppConstants.Administrator)]
        [HttpPost]
        public async Task<IActionResult> Manage(AddToRoleModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.AddErrorMessage(TempMessages.GeneralInvalidInputData);
            }
            else
            {
                await _userService.AssignToRole(model.UserName, model.Role);
            }            

            return RedirectToAction(nameof(AdminsController.Index), AdminController);
        }
    }
}
