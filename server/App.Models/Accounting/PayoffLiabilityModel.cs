using App.Framework.Models;
using System;

namespace App.Models.Accounting
{
    public partial record PayoffLiabilitySearchModel : BaseNopModel
    {
        public int TraderId { get; set; }
        public DateTime Period { get; set; }
    }
    public partial record PayoffLiabilityListModel : BaseNopModel
    {
    }
    public partial record PayoffLiabilityModel : BaseNopModel
    {
        public int PayoffLiabilityTypeId { get; set; }
        public int PayoffLiabilityCategoryTypeId { get; set; }
        public decimal January { get; set; }
        public decimal February { get; set; }
        public decimal March { get; set; }
        public decimal April { get; set; }
        public decimal May { get; set; }
        public decimal June { get; set; }
        public decimal July { get; set; }
        public decimal August { get; set; }
        public decimal September { get; set; }
        public decimal October { get; set; }
        public decimal November { get; set; }
        public decimal December { get; set; }

        public string PayoffLiabilityTypeName { get; set; }
        public string PayoffLiabilityCategoryTypeName { get; set; }
    }
    public partial record PayoffLiabilityTableModel : BaseNopModel
    {
    }
    public partial record PayoffLiabilityFactorModel : BaseNopModel
    {
        public int PayoffLiabilityFactorTypeId { get; set; }
        public decimal January { get; set; }
        public decimal February { get; set; }
        public decimal March { get; set; }
        public decimal April { get; set; }
        public decimal May { get; set; }
        public decimal June { get; set; }
        public decimal July { get; set; }
        public decimal August { get; set; }
        public decimal September { get; set; }
        public decimal October { get; set; }
        public decimal November { get; set; }
        public decimal December { get; set; }

        public string PayoffLiabilityFactorTypeName { get; set; }
    }
    public partial record PayoffLiabilityFactorTableModel : BaseNopModel
    {
    }
    public class PayoffLiabilityResult
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public int Period { get; set; }
        public decimal Value { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
    public class PayoffLiabilityWorkersResult
    {
        public string Name { get; set; }
    }
    public class PayoffLiabilityItem
    {
        public decimal GeneralLiquidity { get; set; }
        public decimal ImmediateLiquidity { get; set; }
        public decimal CashFlow { get; set; }
        public decimal AverageReceivablesCollectionTime { get; set; }
        public decimal StockRecycling { get; set; }
        public decimal PaymentTimeOfShortTermLiabilities { get; set; }
        public decimal DefenseTimePeriod { get; set; }
    }
}