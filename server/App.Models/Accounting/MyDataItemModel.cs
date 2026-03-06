using App.Framework.Models;
using App.Models.Employees;
using System;

namespace App.Models.Accounting
{
    public partial record MyDataItemSearchModel : BaseSearchModel
    {
        public MyDataItemSearchModel() : base("traderName") { }
    }
    public class MyDataItemInfoModel
    {
        public int TraderId { get; set; } = 0; // Συναλλασσόμενος
        public bool IsIssuer { get; set; } = true; // Εκδότης/Λήπτης
        public int DocTypeId { get; set; } = 4; // Τύπος 1.Πωλήσεις, 4.Αγορές, 8.Δαπάνες
    }
    public partial record MyDataItemInfoFormModel : BaseNopModel
    {
    }
    public partial record MyDataItemListModel : BasePagedListModel<MyDataItemModel>
    {
    }
    public partial record MyDataItemModel : BaseNopEntityModel
    {
        public DateTime LastDateOnUtc { get; set; } // Ημερομηνία
        public string TraderVat { get; set; } // ΑΦΜ Συναλλ/νου
        public string CounterpartVat { get; set; } // ΑΦΜ Πελάτη/Προμηθευτή
        public string InvoiceType { get; set; } // Κωδ.Παραστατικού 
        public string Series { get; set; } // Σειρά Παραστατικού 
        public int Branch { get; set; } // Υποκατάστημα
        public int PaymentMethodId { get; set; } // Τρόπος Πληρωμής
        public int VatProvisionId { get; set; } // Δ.απαλλαγής
        public int SeriesId { get; set; } // Σειρά
        public int DocTypeId { get; set; } // Τύπος 1.Πωλήσεις, 4.Αγορές, 8.Δαπάνες
        public bool IsIssuer { get; set; } // Εκδότης/Λήπτης
        //
        public DateTime LastDateOn { get; set; } // Ημερομηνία
        public string TraderName { get; set; } // Συναλλασσόμενος
        public string CounterpartName { get; set; } // ΑΦΜ Πελάτη/Προμηθευτή
        public string InvoiceTypeName { get; set; } // Παραστατικό
        public string PaymentMethodName { get; set; } // Τρόπος Πληρωμής
        public string VatProvisionName { get; set; } // Δ.απαλλαγής
        public string SeriesName { get; set; } // Σειρά
        public string DocTypeName { get; set; } // Τύπος
        public string IsIssuerName { get; set; } // Εκδότης/Λήπτης

        public int VatCategoryId { get; set; } // Κατηγορία ΦΠΑ
        //public int TaxType { get; set; }
        //public int TaxCategory { get; set; }
        public int TaxCategoryId { get; set; }
        public string ProductCode { get; set; } // Κωδ.Είδους
        public int VatId { get; set; } // ΦΠΑ
        public int CurrencyId { get; set; } // Νόμισμα
        //
        public string VatCategoryName { get; set; } // Κατηγορία ΦΠΑ
        public string ProductCodeName { get; set; } // Περ.Είδους
        public string TaxCategoryName { get; set; } // Κατηγορία εξόδου
        public string VatName { get; set; } // ΦΠΑ
        public string CurrencyName { get; set; } // Νόμισμα
    }
    public partial record MyDataItemFormModel : BaseNopModel
    {
    }

    public class MyDataExport
    {
        public int SeriesId { get; set; } // Σειρά Παραστατικού
        public DateTime CreatedOnUtc { get; set; } // Ημερομηνία
        public string Invoice { get; set; } // Παραστατικό
        public string TraderCode { get; set; } // Συναλλασσόμενος
        public string ProductCode { get; set; } // Γραμμή είδους
        public decimal Quantity { get; set; } // Ποσότητα
        public decimal NetValue { get; set; } // Αξία
        public int VatId { get; set; } // ΦΠΑ
        public int CurrencyId { get; set; } // Νόμισμα
        public int VatProvisionId { get; set; } // Δ.απαλλαγής
        public int PaymentMethodId { get; set; } // Τρόπος Πληρωμής
    }
}