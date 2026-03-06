namespace App.Core.Domain.VatExemption
{
    public partial class VatExemptionSerial : BaseEntity
    {
        public int SerialNo { get; set; }
        public string SerialName { get; set; }
        public string SerialNameDescription { get; set; }
        public decimal Limit { get; set; }
        public bool Manuscript { get; set; }
        public string Description { get; set; }
        public int VatExemptionApprovalId { get; set; }
        public int TraderId { get; set; }
    }
}
