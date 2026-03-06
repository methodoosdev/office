namespace App.Core.Domain.Traders
{
    public partial class TraderKad : BaseEntity
    {
        public int GroupId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool Activity { get; set; }
        public int TraderId { get; set; }
    }
}
