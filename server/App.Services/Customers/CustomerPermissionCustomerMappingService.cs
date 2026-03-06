using App.Core.Domain.Customers;
using App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Customers
{
    public partial interface ICustomerPermissionCustomerMappingService
    {
        Task<IList<CustomerPermission>> GetCustomerPermissionsByCustomerIdAsync(int customerId);
        Task<IList<Customer>> GetCustomersByCustomerPermissionIdAsync(int customerPermissionId);
        Task RemoveCustomerPermissionCustomerAsync(CustomerPermission customerPermission, Customer customer);
        Task InsertCustomerPermissionCustomerAsync(CustomerPermission customerPermission, Customer customer);
        Task RemoveCustomerPermissionsFromCustomerAsync(Customer customer);
        Task InsertCustomerPermissionsFromCustomerAsync(Customer from, Customer to);
    }
    public partial class CustomerPermissionCustomerMappingService : ICustomerPermissionCustomerMappingService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerPermission> _customerPermissionRepository;
        private readonly IRepository<CustomerPermissionCustomerMapping> _customerPermissionCustomerMappingRepository;

        public CustomerPermissionCustomerMappingService(
            IRepository<Customer> customerRepository,
            IRepository<CustomerPermission> customerPermissionRepository,
            IRepository<CustomerPermissionCustomerMapping> customerPermissionCustomerMappingRepository)
        {
            _customerRepository = customerRepository;
            _customerPermissionRepository = customerPermissionRepository;
            _customerPermissionCustomerMappingRepository = customerPermissionCustomerMappingRepository;
        }

        public virtual async Task<IList<CustomerPermission>> GetCustomerPermissionsByCustomerIdAsync(int customerId)
        {
            return await _customerPermissionRepository.GetAllAsync(query =>
            {
                return from cp in query
                       join cpc in _customerPermissionCustomerMappingRepository.Table on cp.Id equals cpc.CustomerPermissionId
                       where cpc.CustomerId == customerId
                       select cp;
            });
        }

        public virtual async Task<IList<Customer>> GetCustomersByCustomerPermissionIdAsync(int customerPermissionId)
        {
            return await _customerRepository.GetAllAsync(query =>
            {
                return from c in query
                       join cpc in _customerPermissionCustomerMappingRepository.Table on c.Id equals cpc.CustomerId
                       where cpc.CustomerPermissionId == customerPermissionId
                       select c;
            });
        }

        public virtual async Task RemoveCustomerPermissionCustomerAsync(CustomerPermission customerPermission, Customer customer)
        {
            if (customerPermission == null)
                throw new ArgumentNullException(nameof(customerPermission));

            if (customer is null)
                throw new ArgumentNullException(nameof(customer));

            if (await _customerPermissionCustomerMappingRepository.Table
                .FirstOrDefaultAsync(x => x.CustomerPermissionId == customerPermission.Id && x.CustomerId == customer.Id) is CustomerPermissionCustomerMapping mapping)
            {
                await _customerPermissionCustomerMappingRepository.DeleteAsync(mapping);
            }
        }

        public virtual async Task InsertCustomerPermissionCustomerAsync(CustomerPermission customerPermission, Customer customer)
        {
            if (customerPermission is null)
                throw new ArgumentNullException(nameof(customerPermission));

            if (customer is null)
                throw new ArgumentNullException(nameof(customer));

            if (await _customerPermissionCustomerMappingRepository.Table
                .FirstOrDefaultAsync(x => x.CustomerPermissionId == customerPermission.Id && x.CustomerId == customer.Id) is null)
            {
                var mapping = new CustomerPermissionCustomerMapping
                {
                    CustomerPermissionId = customerPermission.Id,
                    CustomerId = customer.Id
                };

                await _customerPermissionCustomerMappingRepository.InsertAsync(mapping);
            }
        }

        public virtual async Task RemoveCustomerPermissionsFromCustomerAsync(Customer customer)
        {
            if (customer is null)
                throw new ArgumentNullException(nameof(customer));

            var customerPermissions = await _customerPermissionCustomerMappingRepository.Table
                .Where(x => x.CustomerId == customer.Id)
                .ToListAsync();

            await _customerPermissionCustomerMappingRepository.DeleteAsync(customerPermissions);
        }

        public virtual async Task InsertCustomerPermissionsFromCustomerAsync(Customer from, Customer to)
        {
            if (from is null)
                throw new ArgumentNullException(nameof(from));

            if (to is null)
                throw new ArgumentNullException(nameof(to));

            var mapping = new List<CustomerPermissionCustomerMapping>();

            var customerPermissionMappings = await _customerPermissionCustomerMappingRepository.Table
                .Where(x => x.CustomerId == from.Id)
                .ToListAsync();

            foreach (var customerPermissionMapping in customerPermissionMappings)
            {
                var item = new CustomerPermissionCustomerMapping
                {
                    CustomerPermissionId = customerPermissionMapping.CustomerPermissionId,
                    CustomerId = to.Id
                };

                mapping.Add(item);
            }

            await _customerPermissionCustomerMappingRepository.InsertAsync(mapping);
        }
    }
}