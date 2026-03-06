using App.Framework.Models;
using System;

namespace App.Models.Accounting
{
    public partial record VatCalculationSearchModel : BaseNopModel
    {
        public int TraderId { get; set; }
        public int Year {  get; set; }
    }
    public partial record VatCalculationListModel : BasePagedListModel<VatCalculationModel>
    {
    }
    public partial record VatCalculationModel : BaseNopEntityModel
    {
        public string Period { get; set; }
        public decimal Sales { get; set; }
        public decimal Purchases { get; set; }
        public decimal CreditBalance { get; set; }
        public decimal NewBalance { get; set; }
        public decimal ToPay { get; set; }
        public decimal InTransfer { get; set; }
        public decimal ToReturn { get; set; }
    }
    public partial record VatCalculationTableModel : BaseNopModel
    {
    }
    public class VatCalculationResult
    {
        public string AccountingCode { get; set; }
        public int Period { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
    public class VatCalculationPeriodoi
    {
        public int Year { get; set; }
        public int Period { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}