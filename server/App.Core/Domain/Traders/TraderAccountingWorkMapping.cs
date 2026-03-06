namespace App.Core.Domain.Traders
{
    public partial class TraderAccountingWorkMapping : BaseEntity
    {
        public int TraderId { get; set; }
        public int AccountingWorkId { get; set; }
    }
}
