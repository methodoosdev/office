using App.Core;
using App.Core.Domain.Customers;
using App.Data;
using App.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Customers
{
    public partial class CustomerPermissionService : ICustomerPermissionService
    {
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<CustomerPermission> _customerPermissionRepository;
        private readonly IRepository<CustomerPermissionCustomerMapping> _customerPermissionCustomerMappingRepository;
        private readonly IWorkContext _workContext;

        public CustomerPermissionService(ICustomerService customerService,
            ILocalizationService localizationService,
            IRepository<CustomerPermission> customerPermissionRepository,
            IRepository<CustomerPermissionCustomerMapping> customerPermissionCustomerMappingRepository,
            IWorkContext workContext)
        {
            _customerService = customerService;
            _localizationService = localizationService;
            _customerPermissionRepository = customerPermissionRepository;
            _customerPermissionCustomerMappingRepository = customerPermissionCustomerMappingRepository;
            _workContext = workContext;
        }

        protected virtual async Task<CustomerPermission> GetCustomerPermissionBySystemNameAsync(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from pr in _customerPermissionRepository.Table
                where pr.SystemName == systemName
                orderby pr.Id
                select pr;

            var permissionRecord = await query.FirstOrDefaultAsync();
            return permissionRecord;
        }

        public virtual async Task<IList<CustomerPermission>> GetCustomerPermissionsByCustomerIdAsync(int customerId)
        {
            var query = from pr in _customerPermissionRepository.Table
                        join prcrm in _customerPermissionCustomerMappingRepository.Table on pr.Id equals prcrm.CustomerPermissionId
                        where prcrm.CustomerId == customerId
                        orderby pr.Id
                        select pr;

            return await query.ToListAsync();
        }

        public virtual IQueryable<CustomerPermission> Table => _customerPermissionRepository.Table;

        public virtual async Task<IList<CustomerPermission>> GetAllCustomerPermissionsAsync()
        {
            var permissions = await _customerPermissionRepository.GetAllAsync(query =>
            {
                return from pr in query
                    orderby pr.Name
                    select pr;
            });

            return permissions;
        }

        public virtual async Task InsertCustomerPermissionAsync(CustomerPermission customerPermission)
        {
            await _customerPermissionRepository.InsertAsync(customerPermission);
        }

        public virtual async Task InsertCustomerPermissionAsync(IList<CustomerPermission> customerPermissions)
        {
            await _customerPermissionRepository.InsertAsync(customerPermissions);
        }

        public virtual async Task<CustomerPermission> GetCustomerPermissionByIdAsync(int customerPermissionId)
        {
            return await _customerPermissionRepository.GetByIdAsync(customerPermissionId);
        }

        public virtual async Task<IList<CustomerPermission>> GetCustomerPermissionsByIdsAsync(int[] customerPermissionIds)
        {
            return await _customerPermissionRepository.GetByIdsAsync(customerPermissionIds);
        }

        public virtual async Task UpdateCustomerPermissionAsync(CustomerPermission customerPermission)
        {
            await _customerPermissionRepository.UpdateAsync(customerPermission);
        }

        public virtual async Task DeleteCustomerPermissionAsync(CustomerPermission customerPermission)
        {
            await _customerPermissionRepository.DeleteAsync(customerPermission);
        }

        public virtual async Task DeleteCustomerPermissionAsync(IList<CustomerPermission> customerPermissions)
        {
            await _customerPermissionRepository.DeleteAsync(customerPermissions);
        }

        public virtual async Task<bool> AuthorizeAsync(CustomerPermission customerPermission)
        {
            return await AuthorizeAsync(customerPermission, await _workContext.GetCurrentCustomerAsync());
        }

        public virtual async Task<bool> AuthorizeAsync(CustomerPermission customerPermission, Customer customer)
        {
            if (customerPermission == null)
                return false;

            if (customer == null)
                return false;

            return await AuthorizeAsync(customerPermission.SystemName, customer);
        }

        public virtual async Task<bool> AuthorizeAsync(string customerPermissionSystemName)
        {
            return await AuthorizeAsync(customerPermissionSystemName, await _workContext.GetCurrentCustomerAsync());
        }

        public virtual async Task<bool> AuthorizeAsync(string customerPermissionSystemName, Customer customer)
        {
            if (string.IsNullOrEmpty(customerPermissionSystemName))
                return false;

            if (await AuthorizeAsync(customerPermissionSystemName, customer.Id))
                //yes, we have such permission
                return true;

            //no permission found
            return false;
        }

        public virtual async Task<bool> AuthorizeAsync(string customerPermissionSystemName, int customerId)
        {
            if (string.IsNullOrEmpty(customerPermissionSystemName))
                return false;

            var permissions = await GetCustomerPermissionsByCustomerIdAsync(customerId);
            foreach (var permission in permissions)
                if (permission.SystemName.Equals(customerPermissionSystemName, StringComparison.InvariantCultureIgnoreCase))
                    return true;

            return false;
        }

        public virtual async Task<IList<CustomerPermissionCustomerMapping>> GetMappingByCustomerPermissionIdAsync(int customerPermissionId)
        {
            var query = _customerPermissionCustomerMappingRepository.Table;

            query = query.Where(x => x.CustomerPermissionId == customerPermissionId);

            return await query.ToListAsync();
        }

        public virtual async Task DeleteCustomerPermissionCustomerMappingAsync(int customerPermissionId, int customerId)
        {
            var mapping = _customerPermissionCustomerMappingRepository.Table
                .FirstOrDefault(prcm => prcm.CustomerId == customerId && prcm.CustomerPermissionId == customerPermissionId);
            if (mapping is null)
                return;

            await _customerPermissionCustomerMappingRepository.DeleteAsync(mapping);
        }

        public virtual async Task InsertCustomerPermissionCustomerMappingAsync(CustomerPermissionCustomerMapping customerPermissionCustomerMapping)
        {
            await _customerPermissionCustomerMappingRepository.InsertAsync(customerPermissionCustomerMapping);
        }
    }
}