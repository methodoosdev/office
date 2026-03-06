using App.Framework.Models;

namespace App.Models.Traders
{
    public partial record TraderBranchSearchModel : BaseSearchModel
    {
        public TraderBranchSearchModel() : base("address") { }
    }
    public partial record TraderBranchListModel : BasePagedListModel<TraderBranchModel>
    {
    }
    public partial record TraderBranchModel : BaseNopEntityModel
    {
        public int GroupId { get; set; }
        public string Kind { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string Doy { get; set; }
        public int TraderId { get; set; }
    }
}