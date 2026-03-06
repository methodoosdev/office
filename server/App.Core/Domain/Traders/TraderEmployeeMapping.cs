namespace App.Core.Domain.Traders
{
    public partial class TraderEmployeeMapping : BaseEntity
    {
        public int TraderId { get; set; }
        public int EmployeeId { get; set; }
    }
}