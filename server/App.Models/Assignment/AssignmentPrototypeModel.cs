using App.Framework.Models;

namespace App.Models.Assignment
{
    public partial record AssignmentPrototypeSearchModel : BaseSearchModel
    {
        public AssignmentPrototypeSearchModel() : base("displayOrder") { }
    }
    public partial record AssignmentPrototypeListModel : BasePagedListModel<AssignmentPrototypeModel>
    {
    }
    public partial record AssignmentPrototypeModel : BaseNopEntityModel // Πρότυπη εντολή
    {
        public string Name { get; set; } // *Εντολή
        public string Description { get; set; } // Περιγραφή
        public bool InActive { get; set; } // Απόκρυψη
        public int DisplayOrder { get; set; } // Κατάταξη
    }
    public partial record AssignmentPrototypeFormModel : BaseNopModel
    {
    }
}