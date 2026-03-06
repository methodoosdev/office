using App.Framework.Models;

namespace App.Models.Offices
{
    public partial record PeriodicityItemSearchModel : BaseSearchModel
    {
        public PeriodicityItemSearchModel() : base("paragraph") { }
    }
    public partial record PeriodicityItemListModel : BasePagedListModel<PeriodicityItemModel>
    {
    }
    public partial record PeriodicityItemModel : BaseNopEntityModel
    {
        public string Paragraph { get; set; }
        public string Notes { get; set; }

        public int PeriodicityItemTypeId { get; set; }
        public string PeriodicityItemTypeName { get; set; }
    }
    public partial record PeriodicityItemFormModel : BaseNopModel
    {
    }
}