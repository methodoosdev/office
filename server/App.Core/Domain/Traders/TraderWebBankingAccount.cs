namespace App.Core.Domain.Traders
{
    public partial class TraderWebBankingAccount : BaseEntity
    {
        public int TraderId { get; set; }
        public int BankTypeId { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int DisplayOrder { get; set; }
    }
}
