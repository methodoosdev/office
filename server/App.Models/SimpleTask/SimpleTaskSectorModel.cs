using App.Framework.Models;

namespace App.Models.SimpleTask
{
    public partial record SimpleTaskSectorSearchModel : BaseSearchModel
    {
        public SimpleTaskSectorSearchModel() : base("displayOrder") { }
    }
    public partial record SimpleTaskSectorListModel : BasePagedListModel<SimpleTaskSectorModel>
    {
    }
    public partial record SimpleTaskSectorModel : BaseNopEntityModel
    {
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
    public partial record SimpleTaskSectorFormModel : BaseNopModel
    {
    }
}