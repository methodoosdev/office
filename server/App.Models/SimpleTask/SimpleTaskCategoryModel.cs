using App.Framework.Models;

namespace App.Models.SimpleTask
{
    public partial record SimpleTaskCategorySearchModel : BaseSearchModel
    {
        public SimpleTaskCategorySearchModel() : base("displayOrder") { }
    }
    public partial record SimpleTaskCategoryListModel : BasePagedListModel<SimpleTaskCategoryModel>
    {
    }
    public partial record SimpleTaskCategoryModel : BaseNopEntityModel
    {
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
    public partial record SimpleTaskCategoryFormModel : BaseNopModel
    {
    }
}