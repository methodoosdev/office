using System;

namespace App.Core.Infrastructure.Dtos.Financial
{
    public class FinancialObligationDto
    {
        public int Id { get; set; }
        public int TraderId { get; set; }
        public string Institution { get; set; }      // Φορέας 
        public string PaymentType { get; set; }      //Είδος πληρωμής
        public decimal PaymentValue { get; set; }    // Ποσό οφειλής
        public string PaymentIdentity { get; set; }  // Ταυτότητα πληρωμής
        public DateTime PaymentDate { get; set; }     // Ημερομηνία πληρωμής
        public bool IsSent { get; set; }
        public int Period { get; set; }
    }
}