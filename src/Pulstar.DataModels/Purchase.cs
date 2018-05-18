namespace Pulstar.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Purchase : BaseEntity
    {
        public long UniqueCode { get; set; }

        public DateTime Date { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public string DeliveryAddress { get; set; }

        public decimal Amount { get; set; }
    }
}
