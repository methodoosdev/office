using System;

namespace App.Core.Domain.Customers
{
    public partial class CustomerOnline : BaseEntity
    {
        public int CustomerId { get; set; }
        public string Email { get; set; }
        public string SystemName { get; set; }
        public string LastIpAddress { get; set; }
        public DateTime LastLoginDateUtc { get; set; }
        public bool Online { get; set; }
        public int Visits { get; set; }
    }
}
