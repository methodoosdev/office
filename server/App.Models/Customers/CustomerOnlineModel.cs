using App.Framework.Models;
using System;

namespace App.Models.Customers
{
    public partial record CustomerOnlineSearchModel : BaseSearchModel
    {
        public CustomerOnlineSearchModel() : base("lastLoginDateUtc", "desc") { }
    }
    public partial record CustomerOnlineListModel : BasePagedListModel<CustomerOnlineModel>
    {
    }
    public partial record CustomerOnlineModel : BaseNopEntityModel
    {
        public int CustomerId { get; set; }
        public string Email { get; set; }
        public string SystemName { get; set; }
        public string LastIpAddress { get; set; }
        public DateTime LastLoginDateUtc { get; set; }
        public bool Online { get; set; }
        public int Visits { get; set; }

        //
        public string CompanyName { get; set; }
		public DateTime? LastLoginDate { get; set; }
	}
}