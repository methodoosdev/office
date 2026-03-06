using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Models.Payroll
{
    public partial record ApdSubmissionSearchModel : BaseNopModel
    {
        public ApdSubmissionSearchModel()
        {
            SelectedKeys = new List<int>();
        }

        public IList<int> SelectedKeys { get; set; }
        public DateTime Period { get; set; }
        public string Progress { get; set; }
    }

    public partial record ApdSubmissionModel : BaseNopModel // Αποδεικτικό υποβολής ΑΠΔ
    {
        public string SubmissionNumber { get; set; } // Αριθμός υποβολής
        public string Type { get; set; } // Τύπος Δήλωσης 01 Κανονική
        public string Ame { get; set; } // ΑΜΕ
        public string Surname { get; set; } // Επωνυμία
        public string Vat { get; set; } // ΑΦΜ
        public string Period { get; set; } // Περίοδος
        public string Month { get; set; } // Μήνας
        public string Year { get; set; } // Έτος
        public string Amoe { get; set; } // ΑΜΟΕ
        public int TotalInsuranceDays { get; set; } // Σύνολο ημερών ασφάλισης
        public decimal TotalEarnings { get; set; } // Σύνολο αποδοχών
        public decimal TotalContributions { get; set; } // Σύνολο εισφορών
        public string SubmissionDate { get; set; } // Ημερομηνία υποβολής
        public string Tpte { get; set; } // ΤΠΤΕ
        public bool Submitted { get; set; } // Υποβλήθηκε
        public string Error { get; set; } // Σφάλμα
    }
    public partial record ApdSubmissionTableModel : BaseNopModel
    {
    }
}