using App.Core.Domain.Customers;
using App.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Customers
{
    public partial interface ICustomerOnlineService
    {
        IQueryable<CustomerOnline> Table { get; }
        Task<CustomerOnline> GetCustomerOnlineByIdAsync(int customerOnlineId);
        Task<CustomerOnline> GetCustomerOnlineByCustomerIdAsync(int customerId);
        Task<IList<CustomerOnline>> GetAllCustomerOnlinesAsync();
        Task DeleteCustomerOnlineAsync(CustomerOnline customerOnline);
        Task InsertCustomerOnlineAsync(CustomerOnline customerOnline);
        Task UpdateCustomerOnlineAsync(CustomerOnline customerOnline);
    }
    public partial class CustomerOnlineService : ICustomerOnlineService
    {
        private readonly IRepository<CustomerOnline> _customerOnlineRepository;

        public CustomerOnlineService(
            IRepository<CustomerOnline> customerOnlineRepository)
        {
            _customerOnlineRepository = customerOnlineRepository;
        }

        public virtual IQueryable<CustomerOnline> Table => _customerOnlineRepository.Table;

        public virtual async Task<CustomerOnline> GetCustomerOnlineByIdAsync(int customerOnlineId)
        {
            return await _customerOnlineRepository.GetByIdAsync(customerOnlineId);
        }

        public virtual async Task<CustomerOnline> GetCustomerOnlineByCustomerIdAsync(int customerId)
        {
            return await _customerOnlineRepository.Table.FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }

        public virtual async Task<IList<CustomerOnline>> GetAllCustomerOnlinesAsync()
        {
            var entities = await _customerOnlineRepository.GetAllAsync(query =>
            {
                query = query.OrderBy(l => l.Id);

                return query;
            });

            return entities;
        }

        public virtual async Task DeleteCustomerOnlineAsync(CustomerOnline customerOnline)
        {
            await _customerOnlineRepository.DeleteAsync(customerOnline);
        }

        public virtual async Task InsertCustomerOnlineAsync(CustomerOnline customerOnline)
        {
            await _customerOnlineRepository.InsertAsync(customerOnline);
        }

        public virtual async Task UpdateCustomerOnlineAsync(CustomerOnline customerOnline)
        {
            await _customerOnlineRepository.UpdateAsync(customerOnline);
        }
    }
}