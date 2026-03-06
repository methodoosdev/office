using App.Framework.Models;

namespace App.Models.Traders
{
    public partial record TraderKadSearchModel : BaseSearchModel
    {
        public TraderKadSearchModel() : base("code") { }
    }
    public partial record TraderKadListModel : BasePagedListModel<TraderKadModel>
    {
    }
    public partial record TraderKadModel : BaseNopEntityModel
    {
        public int GroupId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool Activity { get; set; }
        public int TraderId { get; set; }
    }
}