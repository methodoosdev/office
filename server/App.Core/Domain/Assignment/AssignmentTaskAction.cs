using System;

namespace App.Core.Domain.Assignment
{
    public partial class AssignmentTaskAction : BaseEntity // Ενέργεια υπεύθυνου
    {
        public int AssignmentTaskId { get; set; } // Εντολή
        public string ActionName { get; set; } // Ενέργεια
        public string ActionDescription { get; set; } // Περιγραφή
        public int AssignmentActionStatusTypeId { get; set; } // Κατάσταση ενέργειας
        public int AssignmentActionPriorityTypeId { get; set; } // Προτεραιότητα ενέργειας
        public int EmployeeId { get; set; }
        public DateTime ExpiryDate { get; set; } //* Λήξη προθεσμίας
        public string Notes { get; set; } // Σχόλια
        public int DisplayOrder { get; set; } // Κατάταξη
    }
}
