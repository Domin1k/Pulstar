namespace Pulstar.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class Purchase : BaseEntity<int>
    {
        public long UniqueCode { get; set; }

        public DateTime Date { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();

        public string UserId { get; set; }

        public User User { get; set; }

        public string DeliveryAddress { get; set; }

        public decimal Amount { get; set; }
    }
}
