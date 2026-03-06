using App.Framework.Models;

namespace App.Models.Logging
{
    public partial record ActivityLogTypeSearchModel : BaseSearchModel
    {
        public ActivityLogTypeSearchModel() : base("systemKeyword") { }
    }
    public partial record ActivityLogTypeListModel : BasePagedListModel<ActivityLogTypeModel>
    {
    }
    public partial record ActivityLogTypeModel : BaseNopEntityModel
    {
        public string SystemKeyword { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
    }
    public partial record ActivityLogTypeFormModel : BaseNopModel
    {
    }
}