namespace App.Core.Domain.Assignment
{
    public partial class AssignmentPrototype : BaseEntity // Πρότυπη εντολή ανάθεσης
    {
        public string Name { get; set; } // Εντολή
        public string Description { get; set; } // Περιγραφή
        public bool InActive { get; set; } // Απόκρυψη
        public int DisplayOrder { get; set; } // Κατάταξη
    }
}
