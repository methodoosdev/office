using App.Framework.Models;

namespace App.Models.Accounting
{
    public partial record MedicalExamSearchModel : BaseNopModel
    {
    }
    public partial record MedicalExamModel : BaseNopModel
    {
        public string Exam { get; set; } // Περιγραφή εξέτασης
        public string Category { get; set; } // Κατηγορία
        public decimal Symmetochi { get; set; } // Συμμετοχή
        public decimal Foreas { get; set; } // Φορέας
        public decimal Price { get; set; } // Αξία
        public int Count { get; set; } // Πλήθος
    }
    public partial record MedicalExamTableModel : BaseNopModel
    {
    }
}