using System;

namespace App.Core.Domain.Payroll
{
    public partial class ApdTeka : BaseEntity // Έλεγχος ΑΠΔ-ΤΕΚΑ
    {
        public int TraderId { get; set; } // Συναλλασσόμενος
        public int CompanyId { get; set; } // Επωνυμία
        public int? EmployeeId { get; set; } // Υπεύθυνος
        public bool CheckWorkers { get; set; } // Έλεγχος Εργ.με Μισθ.κατάσταση
        public decimal Contributions { get; set; } // Εισφορές
        public decimal Subsidiary { get; set; } // Επικουρικό
        public decimal Apd { get; set; } // ΑΠΔ
        public decimal Teka { get; set; } // ΤΕΚΑ
        public decimal ApdSubmit { get; set; } // ΑΠΔ Υποβολής
        public decimal TekaSubmit { get; set; } // ΤΕΚΑ Υποβολής
        public DateTime? ApdSubmitDateOnUtc { get; set; } // Ημερ.Υποβολής ΑΠΔ
        public DateTime? TekaSubmitDateOnUtc { get; set; } // Ημερ.Υποβολής ΤΕΚΑ
        public DateTime? InfoDateOnUtc { get; set; } // Ημερ.Ενημέρωσης
        public string Notes { get; set; } // Παρατηρήσεις
        public int Period { get; set; } // Περίοδος
        public int Year { get; set; } // Έτος
        public decimal DxApd { get; set; } // ΔΧ.ΑΠΔ
        public decimal DxTeka { get; set; } // ΔΧ.ΤΕΚΑ
        public decimal DpApd { get; set; } // ΔΠ.ΑΠΔ
        public decimal DpTeka { get; set; } // ΔΠ.ΤΕΚΑ
        public string WorkersError { get; set; } // Σφάλμα εργαζομένων
        public string ApdError { get; set; } // Σφάλμα ΑΠΔ
        public string TekaError { get; set; } // Σφάλμα ΤΕΚΑ
    }
}
