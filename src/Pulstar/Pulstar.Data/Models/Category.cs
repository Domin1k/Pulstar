namespace Pulstar.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Pulstar.Common.Enums;

    public class Category : BaseEntity<int>
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; }

        public CategoryType Type { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
    }
}
