using System.Collections.Generic;

namespace App.Models.Financial
{
    public class PiraeusBankToken
    {
        public string token_type { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string refresh_token { get; set; }
        public string metadata { get; set; }
    }

    public class PiraeusBankAccounts
    {
        public IList<PiraeusBankAccount> Accounts { get; set; }
    }

    public class PiraeusBankAccount
    {
        public string Type { get; set; }
        public string TypeDescription { get; set; }
        public string Iban { get; set; }
        public string Number { get; set; }
        public decimal OverdraftAmount { get; set; }
        public string HolderName { get; set; }
        public string Alias { get; set; }
        public string ArrangementId { get; set; }
        public decimal Balance { get; set; }
        public bool CanDebit { get; set; }
        public bool CanViewStatement { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }
        public decimal AccountingBalance { get; set; }
        public string AccountId { get; set; }
    }
    public class PiraeusBankAccountTransactions
    {
        public IList<PiraeusBankAccountTransaction> AccountTransactions { get; set; }
        public PiraeusBankPagingInfo PagingInfo { get; set; }
    }

    public class PiraeusBankAccountTransaction
    {
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string TransactionReference { get; set; }
        public string Comments { get; set; }
        public string UniqueReference { get; set; }
        public string TransactionId { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionType { get; set; }
        public string Description { get; set; }
        public string PostDate { get; set; }
        public string ValueDate { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal RunningBalance { get; set; }
    }
    public class PiraeusBankPagingInfo
    {
        public int RowIndex { get; set; }
        public int MaxRows { get; set; }
        public bool CountRows { get; set; }
        public int TotalRowsCount { get; set; }
        public string NextPagePositioningKey { get; set; }
    }
}
