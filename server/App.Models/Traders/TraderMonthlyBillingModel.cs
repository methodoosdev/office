using App.Framework.Models;

namespace App.Models.Traders
{
    public partial record TraderMonthlyBillingSearchModel : BaseSearchModel
    {
        public TraderMonthlyBillingSearchModel() : base("year") { }
    }
    public partial record TraderMonthlyBillingListModel : BasePagedListModel<TraderMonthlyBillingModel>
    {
    }
    public partial record TraderMonthlyBillingModel : BaseNopEntityModel
    {
        public int TraderId { get; set; }
        public int Year { get; set; }
        public int Amount { get; set; }
    }
    public partial record TraderMonthlyBillingFormModel : BaseNopModel
    {
    }
}