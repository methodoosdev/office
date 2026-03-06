using App.Framework.Models;
using System;

namespace App.Models.Accounting
{
    public partial record TraderChargeSearchModel : BaseNopModel
    {
        public int TraderId { get; set; }
    }
    public partial record TraderChargeModel : BaseNopModel
    {
        public int Year { get; set; }
        public decimal Turnover { get; set; }
        public decimal BeforeTaxes { get; set; }
        public decimal Assets { get; set; }
        public int Personnel { get; set; }
        public int Amount { get; set; }
    }
    public partial record TraderChargeTableModel : BaseNopModel
    {
    }
    public class TraderChargeQueryResult
    {
        public int Year { get; set; }
        public string Code { get; set; }
        public decimal Total { get; set; }
        public int CodePrefix { get; set; }
    }
    public class EmployersPerYearQueryResult
    {
        public int EmployerId { get; set; }
    }
}