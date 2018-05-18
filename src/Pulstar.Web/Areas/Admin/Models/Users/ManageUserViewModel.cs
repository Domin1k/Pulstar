namespace Pulstar.Web.Areas.Admin.Models.Users
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ManageUserViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Role { get; set; }

        public List<SelectListItem> Roles { get; set; }
    }
}
