using App.Framework.Models;

namespace App.Models.Banking
{
    public partial record CardListItemSearchModel : BaseSearchModel
    {
        public CardListItemSearchModel() : base("bankBIC") { }
    }
    public partial record CardListItemListModel : BasePagedListModel<CardListItemModel>
    {
    }
    public partial record CardListItemModel : BaseNopEntityModel
    {
        public string Alias { get; set; } // Ψευδώνυμο
        public double CreditBalance { get; set; } // Πισ.Υπόλοιπο
        public string Kind { get; set; } // Τύπος
        public double CreditLine { get; set; } // Γραμμή
        public string CardHolderNameLatin { get; set; } // Δικαιούχος
        public string ResourceId { get; set; } // Αναγνωριστικό
        public string BankBIC { get; set; } // BIC τράπεζας
        public string Number { get; set; } // Αρ.Κάρτας
        public string ProductName { get; set; } // Προιόν
    }
}