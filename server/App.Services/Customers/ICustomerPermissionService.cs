using App.Core.Domain.Customers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Customers
{
    public partial interface ICustomerPermissionService
    {
        Task<IList<CustomerPermission>> GetCustomerPermissionsByCustomerIdAsync(int customerId);

        IQueryable<CustomerPermission> Table { get; }

        Task<IList<CustomerPermission>> GetAllCustomerPermissionsAsync();

        Task InsertCustomerPermissionAsync(CustomerPermission customerPermission);

        Task InsertCustomerPermissionAsync(IList<CustomerPermission> customerPermissions);

        Task<CustomerPermission> GetCustomerPermissionByIdAsync(int customerPermissionId);

        Task<IList<CustomerPermission>> GetCustomerPermissionsByIdsAsync(int[] customerPermissionIds);

        Task UpdateCustomerPermissionAsync(CustomerPermission customerPermission);

        Task DeleteCustomerPermissionAsync(CustomerPermission customerPermission);

        Task DeleteCustomerPermissionAsync(IList<CustomerPermission> customerPermissions);

        Task<bool> AuthorizeAsync(CustomerPermission customerPermission);

        Task<bool> AuthorizeAsync(CustomerPermission customerPermission, Customer customer);

        Task<bool> AuthorizeAsync(string customerPermissionSystemName);

        Task<bool> AuthorizeAsync(string customerPermissionSystemName, Customer customer);

        Task<bool> AuthorizeAsync(string customerPermissionSystemName, int customerId);

        Task<IList<CustomerPermissionCustomerMapping>> GetMappingByCustomerPermissionIdAsync(int customerPermissionId);

        Task DeleteCustomerPermissionCustomerMappingAsync(int customerPermissionId, int customerId);

        Task InsertCustomerPermissionCustomerMappingAsync(CustomerPermissionCustomerMapping customerPermissionCustomerMapping);
    }
}