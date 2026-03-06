using App.Framework.Models;

namespace App.Models.Traders
{
    public partial record AccountingWorkSearchModel : BaseSearchModel
    {
        public AccountingWorkSearchModel() : base("displayOrder") { }
    }
    public partial record AccountingWorkListModel : BasePagedListModel<AccountingWorkModel>
    {
    }
    public partial record AccountingWorkModel : BaseNopEntityModel
    {
        public string SortDescription { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public int Price { get; set; }
    }
    public partial record AccountingWorkFormModel : BaseNopModel
    {
    }
}