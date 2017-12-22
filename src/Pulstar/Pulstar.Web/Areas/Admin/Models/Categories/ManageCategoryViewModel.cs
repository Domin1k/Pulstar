namespace Pulstar.Web.Areas.Admin.Models.Categories
{
    using System.ComponentModel.DataAnnotations;

    public class ManageCategoryViewModel
    {
        [Required]
        public string Name { get; set; }

        public string CategoryType { get; set; }
    }
}
