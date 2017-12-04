namespace Pulstar.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using Pulstar.Common.Enums;

    public class Product : BaseEntity<int>
    {
        [Required]
        [MinLength(2)]
        public string Title { get; set; }

        [Required]
        [MinLength(2)]
        public string Description { get; set; }

        [Required]
        [MinLength(2)]
        public string Model { get; set; }

        [Required]
        [MinLength(2)]
        public string Manufacturer { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        
        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
