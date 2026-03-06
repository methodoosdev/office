using App.Framework.Models;

namespace App.Models.Traders
{
    public partial record TraderGroupSearchModel : BaseSearchModel
    {
        public TraderGroupSearchModel() : base("displayOrder") { }
    }
    public partial record TraderGroupListModel : BasePagedListModel<TraderGroupModel>
    {
    }
    public partial record TraderGroupModel : BaseNopEntityModel
    {
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
    public partial record TraderGroupFormModel : BaseNopModel
    {
    }
}