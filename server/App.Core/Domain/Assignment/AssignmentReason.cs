namespace App.Core.Domain.Assignment
{
    public partial class AssignmentReason : BaseEntity // Αιτία
    {
        public string Description { get; set; } // Αιτία
        public int DisplayOrder { get; set; } // Κατάταξη
    }
}
