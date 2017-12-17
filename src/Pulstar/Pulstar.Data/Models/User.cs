namespace Pulstar.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Identity;

    public class User : IdentityUser
    {
        [MinLength(2)]
        public string FullName { get; set; }

        [MinLength(4)]
        public string Address { get; set; }

        [MinLength(2)]
        public string Country { get; set; }

        [Range(0, double.MaxValue)]
        public decimal AccountBalance { get; set; }

        public List<Purchase> Purchases { get; set; } = new List<Purchase>();

        public List<CreditCard> CreditCards { get; set; } = new List<CreditCard>();
    }
}
