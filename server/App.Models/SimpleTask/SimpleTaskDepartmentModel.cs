using App.Framework.Models;

namespace App.Models.SimpleTask 
{ 
    public partial record SimpleTaskDepartmentSearchModel : BaseSearchModel
    {
        public SimpleTaskDepartmentSearchModel() : base("displayOrder") { }
    }
    public partial record SimpleTaskDepartmentListModel : BasePagedListModel<SimpleTaskDepartmentModel>
    {
    }
    public partial record SimpleTaskDepartmentModel : BaseNopEntityModel
    {
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
    public partial record SimpleTaskDepartmentFormModel : BaseNopModel
    {
    }
}