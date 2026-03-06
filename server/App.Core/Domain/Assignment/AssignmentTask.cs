using System;

namespace App.Core.Domain.Assignment
{
    public partial class AssignmentTask : BaseEntity // Εντολή ανάθεσης
    {
        public string Name { get; set; } // Εντολή
        public string Description { get; set; } // Περιγραφή
        public int AssignmentReasonId { get; set; } // Αιτία
        public int TraderId { get; set; } // Συναλλασσόμενος
        public int AssignorId { get; set; } //* Εντολέας
        public DateTime CreatedDate { get; set; } // Ημερ/νία δημιουργίας
        public DateTime ExpiryDate { get; set; } //* Λήξη προθεσμίας
        public bool Rejection { get; set; } // Απόρριψη
    }
}
