using App.Core;
using App.Framework.Models;
using System.Collections.Generic;

namespace App.Models.Customers
{
    public partial record CustomerPermissionSearchModel : BaseSearchModel
    {
        public CustomerPermissionSearchModel() : base("systemName") { }
    }
    public partial record CustomerPermissionListModel : BasePagedListModel<CustomerPermissionModel>
    {
    }
    public partial record CustomerPermissionModel : BaseNopEntityModel
    {
        public CustomerPermissionModel() 
        {
            AvailableControllerNames = new List<SelectionList>();
        }

        public string Name { get; set; }

        public string SystemName { get; set; }

        public string Category { get; set; }

        public string Menu { get; set; }

        public string Area { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public List<SelectionList> AvailableControllerNames { get; set; }

        public int Used { get; set; }
    }
    public partial record CustomerPermissionFormModel : BaseNopModel
    {
	}
	public class CustomerPermissionsByCustomersModel
	{
		public ICollection<int> CustomerPermissions { get; set; }
		public ICollection<int> Customers { get; set; }
	}
}