using App.Framework.Models;

namespace App.Models.Traders
{
    public partial record WorkingAreaSearchModel : BaseSearchModel
    {
        public WorkingAreaSearchModel() : base("displayOrder") { }
    }
    public partial record WorkingAreaListModel : BasePagedListModel<WorkingAreaModel>
    {
    }
    public partial record WorkingAreaModel : BaseNopEntityModel
    {
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
    public partial record WorkingAreaFormModel : BaseNopModel
    {
    }
}