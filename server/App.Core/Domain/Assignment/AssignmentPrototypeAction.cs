namespace App.Core.Domain.Assignment
{
    public partial class AssignmentPrototypeAction : BaseEntity // Πρότυπη ενέργεια υπευθύνου
    {
        public string Name { get; set; } // Ενέργεια
        public string Description { get; set; } // Περιγραφή
        public int AssignmentReasonId { get; set; } // Αιτία
        public int DepartmentId { get; set; } // Τμήμα
        public int DisplayOrder { get; set; } // Κατάταξη
    }
}
