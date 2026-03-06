using System;

namespace App.Core.Domain.VatExemption
{
    public partial class VatExemptionDoc : BaseEntity
    {
        public decimal ApprovalLimit { get; set; }
        public string SerialName { get; set; }
        public int SerialNo { get; set; }
        public decimal SerialLimit { get; set; }
        public string ApprovalNumber { get; set; }
        public DateTime ApprovalExpiryDate { get; set; }
        public string TraderFullName { get; set; }
        public string TraderProfessionalActivity { get; set; }
        public string TraderAddress { get; set; }
        public string TraderStreetNumber { get; set; }
        public string TraderPostcode { get; set; }
        public string TraderCity { get; set; }
        public string TraderVat { get; set; }
        public string TraderDoy { get; set; }
        public string SupplierFullName { get; set; }
        public string SupplierProfessionalActivity { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierStreetNumber { get; set; }
        public string SupplierPostcode { get; set; }
        public string SupplierCity { get; set; }
        public string SupplierVat { get; set; }
        public string SupplierDoy { get; set; }

        public string Customs { get; set; }
        public decimal LimitBalance { get; set; }
        public decimal ReturnDiscount { get; set; }
        public decimal TransferFromSeries { get; set; }
        public decimal TransferToSeries { get; set; }
        public decimal AdjustedLimit { get; set; }
        public decimal CurrentTransaction { get; set; }
        public string CurrentTransactionAlphabet { get; set; }
        public decimal CurrentLimit { get; set; }
        public string CurrentLimitAlphabet { get; set; }

        public string DocumentCity { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int VatExemptionSerialId { get; set; }
        public int TraderId { get; set; }
    }
}
