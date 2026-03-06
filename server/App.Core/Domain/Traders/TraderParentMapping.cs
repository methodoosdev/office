namespace App.Core.Domain.Traders
{
    public partial class TraderParentMapping : BaseEntity
    {
        public int TraderId { get; set; }
        public int ParentId { get; set; }
    }
}