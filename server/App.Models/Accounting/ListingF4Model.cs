using App.Framework.Models;
using System;

namespace App.Models.Accounting
{
    public partial record ListingF4SearchModel : BaseNopModel
    {
        public int TraderId { get; set; }
        public DateTime Period { get; set; }
    }
    public partial record ListingF4Model : BaseNopModel
    {
    }
    public partial record ListingF4TableModel : BaseNopModel
    {
    }
    public class ListingF4QueryResult
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