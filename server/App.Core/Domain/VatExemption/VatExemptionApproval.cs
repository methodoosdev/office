using System;

namespace App.Core.Domain.VatExemption
{
    public partial class VatExemptionApproval : BaseEntity
    {
        public decimal Limit { get; set; }
        public string Doy { get; set; }
        public string ApprovalNumber { get; set; }
        public string FileName { get; set; }
        public string ApprovalProtocol { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? StartingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool Active { get; set; }
        public int TraderId { get; set; }
    }
}
