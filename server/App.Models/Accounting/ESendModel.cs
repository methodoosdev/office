using App.Framework.Models;
using System;

namespace App.Models.Accounting
{
    public partial record ESendSearchModel : BaseNopModel
    {
        public int TraderId { get; set; }
        public DateTime Period { get; set; }
        public bool NotSoftOne { get; set; }
    }
    public partial record ESendModel : BaseNopModel
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
    public partial record ESendTableModel : BaseNopModel
    {
    }
    //public partial record ESendItem
    //{
    //    public DateTime Date { get; set; } // ΗΜΕΡΟΜΗΝΙΑ
    //    public string Doc { get; set; } // ΠΑΡΑΣΤΑΤΙΚΟ
    //    public string Payment { get; set; } // ΤΡΟΠΟΣ ΠΛΗΡΩΜΗΣ
    //    public string Currency { get; set; } // ΝΟΜΙΣΜΑ
    //    public decimal Value { get; set; } // ΚΑΘΑΡΗ ΑΞΙΑ
    //    public string Code { get; set; } // ΛΟΓΑΡΙΑΣΜΟΣ
    //    public string DocId { get; set; } // ΣΕΙΡΑ
    //    public string CustomerCode { get; set; } // ΚΩΔΙΚΟΣ ΠΕΛΑΤΗ
    //    public string Number { get; set; } // ΦΟΡΟΛΟΓΙΚΟΣ ΑΡΙΘΜΟΣ
    //    public string Index { get; set; } // Α/Α ΑΡΘΡΟΥ
    //}
}