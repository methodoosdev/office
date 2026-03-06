using App.Framework.Models;
using System;

namespace App.Models.Accounting
{
    public partial record PeriodicityItemsSearchModel : BaseNopModel
    {
        public int TraderId { get; set; }
        public DateTime Period { get; set; }
    }
    public partial record PeriodicityItemsListModel : BaseNopModel
    {
    }
    public partial record PeriodicityItemsModel : BaseNopModel
    {
        public string Paragraph { get; set; }
        public string PeriodicityItemTypeName { get; set; }
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
    }
    public partial record PeriodicityItemsTableModel : BaseNopModel
    {
    }

    public class RealEstateRentModel
    {
        public decimal Price { get; set; }
        public int Periodos { get; set; }
        public bool Credit { get; set; }
    }
}