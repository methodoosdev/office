namespace App.Core.Domain.Traders
{
    public partial class TraderWebBankingIban : BaseEntity
    {
        public int TraderId { get; set; }
        public int TraderWebBankingAccountId { get; set; }
        public int BankAccountTypeId { get; set; }
        public string Iban { get; set; }
    }
}
