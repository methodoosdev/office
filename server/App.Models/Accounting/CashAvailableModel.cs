using App.Framework.Models;
using System;

namespace App.Models.Accounting
{
    public partial record CashAvailableSearchModel : BaseNopModel
    {
        public int TraderId { get; set; }
        public int Year { get; set; }
        public int Period { get; set; }
    }
    public partial record CashAvailableModel : BaseNopModel
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public decimal Total { get; set; }
        public string Type { get; set; }
    }
    public partial record CashAvailableTableModel : BaseNopModel
    {
    }
}