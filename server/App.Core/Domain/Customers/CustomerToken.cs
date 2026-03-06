using System;

namespace App.Core.Domain.Customers
{
    public partial class CustomerToken : BaseEntity
    {
        public string AccessTokenHash { get; set; }

        public DateTimeOffset AccessTokenExpiresDateTime { get; set; }

        public string RefreshTokenIdHash { get; set; }

        public string RefreshTokenIdHashSource { get; set; }

        public DateTimeOffset RefreshTokenExpiresDateTime { get; set; }

        public int CustomerId { get; set; }
    }
}
