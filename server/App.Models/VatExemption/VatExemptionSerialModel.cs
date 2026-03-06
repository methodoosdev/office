using App.Framework.Models;

namespace App.Models.VatExemption
{
    public partial record VatExemptionSerialSearchModel : BaseSearchModel
    {
        public VatExemptionSerialSearchModel() : base("serialName") { }
    }
    public partial record VatExemptionSerialListModel : BasePagedListModel<VatExemptionSerialModel>
    {
    }
    public partial record VatExemptionSerialModel : BaseNopEntityModel
    {
        public int SerialNo { get; set; }
        public string SerialName { get; set; }
        public string SerialNameDescription { get; set; }
        public decimal Limit { get; set; }
        public bool Manuscript { get; set; }
        public string Description { get; set; }
        public int VatExemptionApprovalId { get; set; }
        public int TraderId { get; set; }
        //
        public string ApprovalNumber { get; set; }
        public decimal ApprovalLimit { get; set; }
        public bool ApprovalActive { get; set; }
        public string TraderName { get; set; }

    }
    public partial record VatExemptionSerialFormModel : BaseNopModel
    {
    }
}