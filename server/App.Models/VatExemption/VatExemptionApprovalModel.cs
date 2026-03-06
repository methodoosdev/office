using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Models.VatExemption
{
    public partial record VatExemptionApprovalSearchModel : BaseSearchModel
    {
        public VatExemptionApprovalSearchModel() : base("expiryDate", "desc") { }
    }
    public partial record VatExemptionApprovalListModel : BasePagedListModel<VatExemptionApprovalModel>
    {
    }
    public partial record VatExemptionApprovalModel : BaseNopEntityModel
    {
        public VatExemptionApprovalModel() 
        {
            KendoUpload = new List<object>();
        }

        public string ApprovalNumber { get; set; }
        public string ApprovalProtocol { get; set; }
        public decimal Limit { get; set; }
        public string Doy { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? StartingDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool Active { get; set; }
        public int TraderId { get; set; }
        //
        public string CreatedDateValue { get; set; }
        public string StartingDateValue { get; set; }
        public string ExpiryDateValue { get; set; }
        public string TraderName { get; set; }
        public List<object> KendoUpload { get; set; }
    }
    public partial record VatExemptionApprovalFormModel : BaseNopModel
    {
    }
}