using App.Framework.Models;

namespace App.Models.Employees
{
    public partial record DepartmentSearchModel : BaseSearchModel
    {
        public DepartmentSearchModel() : base("displayOrder") { }
    }
    public partial record DepartmentListModel : BasePagedListModel<DepartmentModel>
    {
    }
    public partial record DepartmentModel : BaseNopEntityModel
    {
        public string SystemName { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public string Background { get; set; }
        public string Color { get; set; }
    }
    public partial record DepartmentFormModel : BaseNopModel
    {
    }
}