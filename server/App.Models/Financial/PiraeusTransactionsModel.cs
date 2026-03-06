using App.Framework.Models;
using System;

namespace App.Models.Financial
{
    public partial record PiraeusTransactionsSearchModel : BaseNopModel
    {
        public int TraderId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
    public partial record PiraeusTransactionsModel : BaseNopModel
    {
        public string Date { get; set; } // ΗΜΕΡΟΜΗΝΙΑ
        public string Doc { get; set; } // ΠΑΡΑΣΤΑΤΙΚΟ
        public string Payment { get; set; } // ΤΡΟΠΟΣ ΠΛΗΡΩΜΗΣ
        public string Currency { get; set; } // ΝΟΜΙΣΜΑ
        public decimal Value { get; set; } // ΚΑΘΑΡΗ ΑΞΙΑ
        public string Code { get; set; } // ΛΟΓΑΡΙΑΣΜΟΣ
        public string DocId { get; set; } // ΣΕΙΡΑ
        public string CustomerCode { get; set; } // ΚΩΔΙΚΟΣ ΠΕΛΑΤΗ
        public string Number { get; set; } // ΦΟΡΟΛΟΓΙΚΟΣ ΑΡΙΘΜΟΣ
        public string Index { get; set; } // Α/Α ΑΡΘΡΟΥ
    }
    public partial record PiraeusTransactionsFormModel : BaseNopModel
    {
    }
    public partial record PiraeusTransactionsTableModel : BaseNopModel
    {
    }
}