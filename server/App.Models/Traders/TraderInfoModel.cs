using App.Framework.Models;
using System;

namespace App.Models.Traders
{
    public partial record TraderInfoSearchModel : BaseSearchModel
    {
        public TraderInfoSearchModel() : base("sortDescription") { }
    }
    public partial record TraderInfoListModel : BasePagedListModel<TraderInfoModel>
    {
    }
    public partial record TraderInfoModel : BaseNopEntityModel
    {
        public int Gravity { get; set; }
        public string SortDescription { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TraderId { get; set; }
    }
    public partial record TraderInfoFormModel : BaseNopModel
    {
    }
}