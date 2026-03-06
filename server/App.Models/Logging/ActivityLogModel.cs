using App.Framework.Models;
using System;

namespace App.Models.Logging
{
    public partial record ActivityLogSearchModel : BaseSearchModel
    {
        public ActivityLogSearchModel() : base("activityLogTypeName") { }
    }
    public partial record ActivityLogListModel : BasePagedListModel<ActivityLogModel>
    {
    }
    public partial record ActivityLogModel : BaseNopEntityModel
    {
        public int ActivityLogTypeId { get; set; }
        public int? EntityId { get; set; }
        public string EntityName { get; set; }
        public int CustomerId { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public virtual string IpAddress { get; set; }

        public string ActivityLogTypeName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public partial record ActivityLogFormModel : BaseNopModel
    {
    }
}