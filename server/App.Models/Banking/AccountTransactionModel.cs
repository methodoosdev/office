using App.Framework.Models;
using System;

namespace App.Models.Banking
{
    public partial record AccountTransactionSearchModel : BaseSearchModel
    {
        public AccountTransactionSearchModel() : base("date") { }
    }
    public partial record AccountTransactionListModel : BasePagedListModel<AccountTransactionModel>
    {
    }
    public partial record AccountTransactionModel : BaseNopEntityModel
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
        public string RelatedName { get; set; }
    }
}