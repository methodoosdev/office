using App.Framework.Models;

namespace App.Models.Assignment
{
    public partial record AssignmentPrototypeActionSearchModel : BaseSearchModel
    {
        public AssignmentPrototypeActionSearchModel() : base("displayOrder") { }
    }
    public partial record AssignmentPrototypeActionListModel : BasePagedListModel<AssignmentPrototypeActionModel>
    {
    }
    public partial record AssignmentPrototypeActionModel : BaseNopEntityModel // Πρότυπη ενέργεια υπεύθυνου
    {
        public string Name { get; set; } // *Ενέργεια
        public string Description { get; set; } // Περιγραφή
        public int AssignmentReasonId { get; set; } // Αιτία
        public int DepartmentId { get; set; } // *Τμήμα
        public int DisplayOrder { get; set; } // Κατάταξη

        public string DepartmentName { get; set; } // Τμήμα
        public string AssignmentReasonName { get; set; } // Αιτία
    }
    public partial record AssignmentPrototypeActionFormModel : BaseNopModel
    {
    }
}