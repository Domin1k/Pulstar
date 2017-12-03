namespace Pulstar.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Category : BaseEntity<int>
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
    }
}
