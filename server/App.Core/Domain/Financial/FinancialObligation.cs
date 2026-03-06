using System;

namespace App.Core.Domain.Financial
{
    public class FinancialObligation : BaseEntity
    {
        public int TraderId { get; set; }
        public string Institution { get; set; }
        public string PaymentType { get; set; }
        public decimal PaymentValue { get; set; } 
        public string PaymentIdentity { get; set; }
        public DateTime PaymentDate { get; set; }
        public bool IsSent { get; set; }

        //migration
        public int CustomerId { get; set; }
        public int Period { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
