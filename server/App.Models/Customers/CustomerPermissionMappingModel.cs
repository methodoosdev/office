using App.Framework.Models;
using System.Collections.Generic;

namespace App.Models.Customers
{
    public partial record CustomerPermissionMappingModel : BaseNopModel
    {
        public CustomerPermissionMappingModel()
        {
            AvailableCustomerPermissions = new List<CustomerPermissionModel>();
            AvailableCustomers = new List<CustomerSecurityModel>();
            Allowed = new Dictionary<string, IDictionary<int, bool>>();
        }

        public IList<CustomerPermissionModel> AvailableCustomerPermissions { get; set; }

        public IList<CustomerSecurityModel> AvailableCustomers { get; set; }

        public IDictionary<string, IDictionary<int, bool>> Allowed { get; set; }
    }
}