using App.Framework.Models;

namespace App.Models.SimpleTask
{
    public partial record SimpleTaskNatureSearchModel : BaseSearchModel
    {
        public SimpleTaskNatureSearchModel() : base("displayOrder") { }
    }
    public partial record SimpleTaskNatureListModel : BasePagedListModel<SimpleTaskNatureModel>
    {
    }
    public partial record SimpleTaskNatureModel : BaseNopEntityModel
    {
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
    public partial record SimpleTaskNatureFormModel : BaseNopModel
    {
    }
}