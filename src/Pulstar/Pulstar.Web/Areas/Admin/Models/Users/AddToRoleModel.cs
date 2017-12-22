namespace Pulstar.Web.Areas.Admin.Models.Users
{
    using System.ComponentModel.DataAnnotations;

    public class AddToRoleModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
