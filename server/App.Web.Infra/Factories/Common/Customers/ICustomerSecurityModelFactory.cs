using App.Models.Customers;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Customers
{
    public partial interface ICustomerSecurityModelFactory
    {
        Task<CustomerPermissionMappingModel> PrepareCustomerPermissionMappingModelAsync(CustomerPermissionMappingModel model);
    }
}