using App.Framework.Models;
using System;

namespace App.Models.Assignment
{
    public partial record AssignmentTaskActionFilterModel : BaseNopModel
    {
        public string AssignmentTaskName { get; set; } = null; // *Εντολή
        public string ActionName { get; set; } = null; // *Ενέργεια
        public int TraderId { get; set; } = 0; // *Συναλλασσόμενος
        public int? AssignmentActionStatusTypeId { get; set; } = null; // Κατάσταση ενέργειας
        public int? AssignmentActionPriorityTypeId { get; set; } = null; // Προτεραιότητα ενέργειας
        public int AssignorId { get; set; } = 0; //* Εντολέας
        public DateTime? ExpiryFrom { get; set; } = null; // Λήξη προθεσμίας
        public DateTime? ExpiryTo { get; set; } = null; // Λήξη προθεσμίας
    }
    public partial record AssignmentTaskActionFilterFormModel : BaseNopModel
    {
    }
    public partial record AssignmentTaskActionSearchModel : BaseSearchModel
    {
        public AssignmentTaskActionSearchModel() : base("expiryDate") { }
    }
    public partial record AssignmentTaskActionListModel : BasePagedListModel<AssignmentTaskActionModel>
    {
    }
    public partial record AssignmentTaskActionModel : BaseNopEntityModel // Ενέργεια υπεύθυνου
    {
        public int AssignmentTaskId { get; set; } // *Εντολή
        public string ActionName { get; set; } // *Ενέργεια
        public string ActionDescription { get; set; } // Περιγραφή
        public int AssignmentActionStatusTypeId { get; set; } // Κατάσταση ενέργειας
        public int AssignmentActionPriorityTypeId { get; set; } // Προτεραιότητα ενέργειας
        public int EmployeeId { get; set; } // *Υπεύθυνος
        public DateTime ExpiryDate { get; set; } // Λήξη προθεσμίας
        public string Notes { get; set; } // Σχόλια
        public int DisplayOrder { get; set; } // Κατάταξη

        public int AssignmentPrototypeActionId { get; set; } // Ενέργεια
        public string AssignmentActionStatusTypeName { get; set; } // Κατάσταση ενέργειας
        public string AssignmentActionPriorityTypeName { get; set; } // Προτεραιότητα ενέργειας
        public string EmployeeName { get; set; } // Υπεύθυνος
        public string DepartmentName { get; set; } // Τμήμα
        public string LetterName { get; set; }
        public string Color { get; set; }
        public string Background { get; set; }
        public string AssignmentTaskName { get; set; } // *Εντολή
        public string TraderName { get; set; } // *Συναλλασσόμενος
        public string AssignorName { get; set; } //* Εντολέας
        public int AssignorId { get; set; }
        public int TraderId { get; set; }
    }
    public partial record AssignmentTaskActionFormModel : BaseNopModel
    {
    }
}