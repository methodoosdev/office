using App.Framework.Models;

namespace App.Models.Banking
{
    public partial record AccountListSearchModel : BaseSearchModel
    {
        public AccountListSearchModel() : base("bankBIC") { }
    }
    public partial record AccountListListModel : BasePagedListModel<AccountListModel>
    {
    }
    public partial record AccountListModel : BaseNopEntityModel
    {
        public string ResourceId { get; set; } // Αναγνωριστικό
        public string BankBIC { get; set; } // BIC τράπεζας
        public string AccountTypeCode { get; set; } // Κωδικός λογαριασμού
        public string AccountTypeValue { get; set; } // Τιμή λογαριασμού
        public decimal OverdraftLimit { get; set; } // Όριο υπερανάληψης
        public string SerialNo { get; set; } // Σειριακός αριθμός
        public string Account { get; set; } // Λογαριασμός
        public string Iban { get; set; } // Iban
        public string Currency { get; set; } // Νόμισμα
        public string Alias { get; set; } // Ψευδώνυμο
        public string Product { get; set; } // Προϊόν
        public decimal LedgerBalance { get; set; } // Λογιστικό υπόλοιπο
        public decimal AvailableBalance { get; set; } // Διαθέσιμο υπόλοιπο
    }
}