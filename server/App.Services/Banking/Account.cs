using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace App.Services.Banking
{
    //
    public class AccountBeneficiaries
    {
        public List<AccountBeneficiarie> Payload { get; set; }
        public double ExecutionTime { get; set; }
    }

    public class AccountBeneficiarie
    {
        public string NameGreek { get; set; }
        public string Order { get; set; }
    }


    //Details
    public class AccountDetails
    {
        public AccountDetailsPayload Payload { get; set; }
        public double ExecutionTime { get; set; }
    }

    public class AccountDetailsPayload
    {
        public string Account { get; set; }
        public string ResourceId { get; set; }
        public string BankBIC { get; set; }
        public string Iban { get; set; }
        public AccountType AccountType { get; set; }
        public string Currency { get; set; }
        public string Alias { get; set; }
        public string Product { get; set; }
        public decimal LedgerBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal OverdraftLimit { get; set; }
    }

    public class AccountType
    {
        public string Code { get; set; }
        public string Value { get; set; }
    }

    // List
    public class AccountList
    {
        public List<AccountListPayload> Payload { get; set; }
        public double ExecutionTime { get; set; }
    }

    public class AccountListPayload
    {
        public string ResourceId { get; set; }
        public string BankBIC { get; set; }
        public AccountType AccountType { get; set; }
        public decimal OverdraftLimit { get; set; }
        public string SerialNo { get; set; }
        public string Account { get; set; }
        public string Iban { get; set; }
        public string Currency { get; set; }
        public string Alias { get; set; }
        public string Product { get; set; }
        public decimal LedgerBalance { get; set; }
        public decimal AvailableBalance { get; set; }
    }

    // Transactions
    public class AccountTransactions
    {
        public List<AccountTransaction> Payload { get; set; }
        public double ExecutionTime { get; set; }
    }

    public class AccountTransaction
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string CreditDebit { get; set; }
        public DateTime Valeur { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
        public string Trans { get; set; }
        public string TransDescription { get; set; }
        public string RelatedAccount { get; set; }
        public string RelatedName{ get; set; }
    }

    // TransactionsPaged
    public class AccountTransactionsPaged
    {
        public AccountTransactionPaged Payload { get; set; }
        public double ExecutionTime { get; set; }
    }

    public class AccountTransactionPaged
    {
        public List<AccountTransaction> Data { get; set; }
        public string PaginationToken { get; set; }
    }


}
