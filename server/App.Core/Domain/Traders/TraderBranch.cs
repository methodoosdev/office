namespace App.Core.Domain.Traders
{
    public partial class TraderBranch : BaseEntity
    {
        public int GroupId { get; set; }
        public string Kind { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string Doy { get; set; }
        public int TraderId { get; set; }
    }
}
