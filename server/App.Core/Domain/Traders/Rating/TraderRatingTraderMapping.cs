namespace App.Core.Domain.Traders.Rating
{
    public partial class TraderRatingTraderMapping : BaseEntity
    {
        public int TraderRatingId { get; set; }
        public int TraderId { get; set; }
    }
}
