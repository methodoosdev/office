namespace App.Core.Domain.Traders
{
    public partial class TraderMonthlyBilling : BaseEntity
    {
        public int TraderId { get; set; }
        public int Year { get; set; }
        public int Amount { get; set; }
    }
}
