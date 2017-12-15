namespace Pulstar.Web.Areas.Admin.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Pulstar.Common.Constants;
    using Pulstar.Services.Interfaces;
    using Pulstar.Web.Areas.Admin.Models.Users;
    using Pulstar.Web.Infrastructure.Constants;
    using Pulstar.Web.Infrastructure.Extensions;
    using Pulstar.Web.Models.ProductsViewModels;

    [Area(WebContants.AdminArea)]
    [Authorize(Roles = AppConstants.Administrator + "," + AppConstants.Manager)]
    public class AdminsController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;

        public AdminsController(RoleManager<IdentityRole> roleManager, IUserService userService)
        {
            _roleManager = roleManager;
            _userService = userService;
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
        public async Task<IActionResult> EditProduct(ProductViewModel input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }
            
            // TODO
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddDiscount(int productId, double discount)
        {
            if (productId <= 0 || (discount <= 0 || discount >= 100))
            {
                TempData.AddErrorMessage("Discount must be in range [1..99] and product must be existing.");
                return Ok();
            }

            // TODO
            return View();
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
