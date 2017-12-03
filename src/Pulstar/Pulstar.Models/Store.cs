namespace Pulstar.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Store : BaseEntity<int>
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; }

        [Required]
        [MinLength(2)]
        public string Address { get; set; }

        [Required]
        [MinLength(2)]
        public string Phone { get; set; }

        [Required]
        [MinLength(2)]
        public string Email { get; set; }

        [Required]
        [MinLength(2)]
        public string WorkingHours { get; set; }

        public float Longitude { get; set; }

        public float Latitude { get; set; }
    }
}
