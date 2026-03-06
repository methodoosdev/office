using System.Collections.Generic;

namespace App.Core.Infrastructure.Dtos.Accounting
{
    public class ListingF4Result
    {
        public string Error { get; set; } // Σφάλμα
        public string Group { get; set; }
        public string CountryCode { get; set; } // Προθεμα Κ-Μ
        public string Vat { get; set; } // Αρ.Φορ.Μητρώου ΦΠΑ
        public string VatNumber { get; set; } // ΦΠΑ
        public decimal Goods { get; set; } // Ενδοκ.Παραδόσεων αγαθών
        public decimal TriangleExchange { get; set; } // Τριγωνικές συναλλαγές
        public decimal Services { get; set; } // Ενδοκ.παροχών υπηρεσιών
        public decimal Products4200 { get; set; } // Ενδοκ.Παραδόσεων - Καθεστώς 4200 
    }

    public class ListingF4Data
    {
        public int TraderId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public IList<ListingF4Result> Data { get; set; }
    }

}