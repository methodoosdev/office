using App.Framework.Models;

namespace App.Models.Assignment
{
    public partial record AssignmentReasonSearchModel : BaseSearchModel
    {
        public AssignmentReasonSearchModel() : base("displayOrder") { }
    }
    public partial record AssignmentReasonListModel : BasePagedListModel<AssignmentReasonModel>
    {
    }
    public partial record AssignmentReasonModel : BaseNopEntityModel // Αιτία
    {
        public string Description { get; set; } // Αιτία
        public int DisplayOrder { get; set; } // Κατάταξη
    }
    public partial record AssignmentReasonFormModel : BaseNopModel
    {
    }
}