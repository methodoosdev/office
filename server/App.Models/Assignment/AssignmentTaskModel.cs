using App.Framework.Models;
using System;

namespace App.Models.Assignment
{
    public partial record AssignmentTaskFilterModel : BaseNopModel
    {
        public string AssignmentTaskName { get; set; } = null; // *Εντολή
        public int TraderId { get; set; } = 0; // *Συναλλασσόμενος
        public int EmployeeId { get; set; } = 0; // *Υπεύθυνος
        public int? AssignmentActionStatusTypeId { get; set; } = null; // Κατάσταση ενέργειας
        public int? AssignmentActionPriorityTypeId { get; set; } = null; // Προτεραιότητα ενέργειας
        public int AssignorId { get; set; } = 0; //* Εντολέας
        public int AssignmentReasonId { get; set; } = 0; // Αιτία
        public DateTime? ExpiryFrom { get; set; } = null; // Λήξη προθεσμίας
        public DateTime? ExpiryTo { get; set; } = null; // Λήξη προθεσμίας
    }
    public partial record AssignmentTaskFilterFormModel : BaseNopModel
    {
    }
    public partial record AssignmentTaskSearchModel : BaseSearchModel
    {
        public AssignmentTaskSearchModel() : base("expiryDate") { }
    }
    public partial record AssignmentTaskListModel : BasePagedListModel<AssignmentTaskModel>
    {
    }
    public partial record AssignmentTaskModel : BaseNopEntityModel // Εντολή ανάθεσης
    {
        public string Name { get; set; } // *Εντολή
        public string Description { get; set; } // Περιγραφή
        public int AssignmentReasonId { get; set; } // Αιτία
        public int TraderId { get; set; } // *Συναλλασσόμενος
        public int AssignorId { get; set; } //* Εντολέας
        public DateTime CreatedDate { get; set; } // Ημερ/νία δημιουργίας
        public DateTime ExpiryDate { get; set; } // Λήξη προθεσμίας
        public bool Rejection { get; set; } // Απόρριψη
        
        public int AssignmentPrototypeId { get; set; } // *Εντολή
        public string AssignmentReasonName { get; set; } // Αιτία
        public string TraderName { get; set; } // Συναλλασσόμενος
        public string AssignmentTaskStatus { get; set; } // Κατάσταση
        public string AssignorName { get; set; } //* Εντολέας
    }
    public partial record AssignmentTaskFormModel : BaseNopModel
    {
    }
}