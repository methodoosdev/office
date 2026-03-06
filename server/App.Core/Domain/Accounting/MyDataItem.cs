using System;

namespace App.Core.Domain.Accounting
{
    public partial class MyDataItem : BaseEntity
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

        public int VatCategoryId { get; set; } // Κατηγορία ΦΠΑ
        //public int TaxType { get; set; }
        public int TaxCategoryId { get; set; }
        public string ProductCode { get; set; } // Κωδ.Είδους
        public int VatId { get; set; } // ΦΠΑ
        public int CurrencyId { get; set; } // Νόμισμα
    }
}
