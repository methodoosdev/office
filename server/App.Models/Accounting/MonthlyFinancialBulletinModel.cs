using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Models.Accounting
{
    public partial record MonthlyFinancialBulletinSearchModel : BaseNopModel
    {
        public int TraderId { get; set; }
        public DateTime Periodos { get; set; }
        public decimal ExpirationInventory { get; set; }
        public decimal ExpirationDepreciate { get; set; }
        public string Branch { get; set; }
        public bool Predictions { get; set; }
    }
    public partial record MonthlyFinancialBulletinModel : BaseNopModel
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Description { get; set; }
        public decimal PeriodCurrentYear { get; set; }
        public decimal PeriodPreviousYear { get; set; }
        public decimal PeriodDefference { get; set; }
        public decimal PeriodRate { get; set; }
        public decimal ProgressivelyCurrentYear { get; set; }
        public decimal ProgressivelyPreviousYear { get; set; }
        public decimal ProgressivelyDefference { get; set; }
        public decimal ProgressivelyRate { get; set; }
        public int DisplayOrder { get; set; }
        public int Level { get; set; }
    }
    public partial record MonthlyFinancialBulletinFormModel : BaseNopModel
    {
    }
    public partial record MonthlyFinancialBulletinTableModel : BaseNopModel
    {
    }
    public class MonthlyFinancialBulletinQuery
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public int Periodos { get; set; }
        public int Level { get; set; }
        public string Branch { get; set; }
        public decimal Total { get; set; }
    }
    public partial record MonthlyFinancialBulletinResultFormModel : BaseNopModel
    {
        public decimal ExpirationInventory { get; set; }
        public decimal NetProfitPeriod { get; set; }
        public decimal RemodelingCosts { get; set; }
        public decimal PreviousYearDamage { get; set; }
        public decimal TaxProfitPeriod { get; set; }
        public decimal TaxIncome { get; set; }
        public decimal HoldingTaxAdvance { get; set; }
        public decimal TaxAdvance { get; set; }
        public decimal PaymentPreviousYear { get; set; }
        public decimal TaxesFee { get; set; }
        public decimal AmountPayable { get; set; }
        public decimal TaxReturn { get; set; }
    }
    public partial record MonthlyFinancialBulletinRemodelingCostsQueryModel : BaseNopModel
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public int Year { get; set; }
        public decimal Rate { get; set; }
        public decimal Total { get; set; }
        public decimal Result { get; set; }
    }
    public class PrepareMonthlyFinancialBulletinModel
    {
        public IList<MonthlyFinancialBulletinModel> TreeList { get; set; }
        public IList<MonthlyFinancialBulletinRemodelingCostsQueryModel> RemodelingCostsList { get; set; }
        public MonthlyFinancialBulletinResultFormModel ResultModel { get; set; }
    }
    public class MonthlyFinancialBulletinCodes
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
    }
    public class MonthlyFinancialBulletinPdfForm
    {
        public string Description { get; set; }
        public decimal Value { get; set; }
    }
    //public class MonthlyFinancialBulletinPrint
    //{
    //    public IList<MonthlyFinancialBulletinModel> Model { get; set; }
    //    public IList<MonthlyFinancialBulletinPdfForm> Form { get; set; }
    //}
}