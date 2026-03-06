using App.Framework.Models;
using System;

namespace App.Models.Customers
{
    public partial record CustomerActivityLogSearchModel : BaseSearchModel
    {
        public CustomerActivityLogSearchModel() : base("createdOn", "desc") { }
        public int CustomerId { get; set; }
    }
    public partial record CustomerActivityLogModel : BaseNopEntityModel
    {
        public int? EntityId { get; set; }
        public string EntityName { get; set; }
        public string Comment { get; set; }
        public virtual string IpAddress { get; set; }

        public string ActivityLogTypeName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public partial record CustomerActivityLogListModel : BasePagedListModel<CustomerActivityLogModel>
    {
    }
}