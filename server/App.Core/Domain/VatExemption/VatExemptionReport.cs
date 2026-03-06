using System;

namespace App.Core.Domain.VatExemption
{
    public partial class VatExemptionReport : BaseEntity
    {
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Protocol { get; set; }
        public string FileName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int VatExemptionApprovalId { get; set; }
        public int TraderId { get; set; }
    }
}
