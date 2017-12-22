namespace Pulstar.Web.Areas.Admin.Models.Categories
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AddCategoryViewModel
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; }

        [DisplayName("Category Type")]
        public string CategoryTypeId { get; set; }

        public List<SelectListItem> CategoryTypes { get; set; }
    }
}
