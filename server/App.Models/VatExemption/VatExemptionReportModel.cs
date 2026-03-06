using App.Framework.Models;
using System;

namespace App.Models.VatExemption
{
    public partial record VatExemptionReportSearchModel : BaseSearchModel
    {
        public VatExemptionReportSearchModel() : base("createdDate", "desc") { }
    }
    public partial record VatExemptionReportListModel : BasePagedListModel<VatExemptionReportModel>
    {
    }
    public partial record VatExemptionReportModel : BaseNopEntityModel
    {
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Protocol { get; set; }
        public string FileName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int VatExemptionApprovalId { get; set; }
        public int TraderId { get; set; }
        //
        public string ApprovalNumber { get; set; }
        public decimal ApprovalLimit { get; set; }
        public string ApprovalDoy { get; set; }
        public string CreatedDateValue { get; set; }
        public string TraderName { get; set; }
    }
    public partial record VatExemptionReportFormModel : BaseNopModel
    {
    }
}