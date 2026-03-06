using App.Framework.Models;
using System;

namespace App.Models.Accounting
{
    public partial record ListingF5SearchModel : BaseNopModel
    {
        public int TraderId { get; set; }
        public DateTime Period { get; set; }
    }
    public partial record ListingF5Model : BaseNopModel
    {
    }
    public partial record ListingF5TableModel : BaseNopModel
    {
    }
    public class ListingF5QueryResult
    {
        public int InvId { get; set; }
        public DateTime InvDate { get; set; }
        public int Periodos { get; set; }
        public string InvSeries { get; set; }
        public string Part { get; set; }
        public string Code { get; set; }
        public decimal Value { get; set; }
        public string CountryCode { get; set; }
        public string Vat { get; set; }
        public string VatNumber { get; set; }
        public decimal ShipKind { get; set; }
    }

}