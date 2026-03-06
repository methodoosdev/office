using System;

namespace App.Core.Domain.Traders
{
    public partial class TraderInfo : BaseEntity
    {
        public int Gravity { get; set; }
        public string SortDescription { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TraderId { get; set; }
    }
}
