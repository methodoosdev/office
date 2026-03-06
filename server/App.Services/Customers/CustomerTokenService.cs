using App.Core.Domain.Customers;
using App.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Customers
{
    public partial interface ICustomerTokenService
    {
        Task<CustomerToken> GetCustomerTokenByIdAsync(int customerTokenId);
        Task InsertCustomerTokenAsync(CustomerToken customerToken);
        Task UpdateCustomerTokenAsync(CustomerToken customerToken);
        Task DeleteCustomerTokenAsync(CustomerToken customerToken);
        Task DeleteExpiredTokensAsync();
        Task DeleteTokensWithSameRefreshTokenSourceAsync(string refreshTokenIdHashSource);
        Task InvalidateCustomerTokensAsync(int customerId);
        Task<CustomerToken> IsValidTokenAsync(string accessTokenHash, int customerId);
        Task<CustomerToken> FindTokenAsync(string refreshTokenIdHash);
    }

    public partial class CustomerTokenService : ICustomerTokenService
    {
        private readonly IRepository<CustomerToken> _customerTokenRepository;

        public CustomerTokenService(IRepository<CustomerToken> customerTokenRepository)
        {
            _customerTokenRepository = customerTokenRepository;
        }

        public virtual async Task<CustomerToken> GetCustomerTokenByIdAsync(int customerTokenId)
        {
            return await _customerTokenRepository.GetByIdAsync(customerTokenId);
        }

        public virtual async Task InsertCustomerTokenAsync(CustomerToken customerToken)
        {
            await _customerTokenRepository.InsertAsync(customerToken);
        }

        public virtual async Task UpdateCustomerTokenAsync(CustomerToken customerToken)
        {
            await _customerTokenRepository.UpdateAsync(customerToken);
        }

        public virtual async Task DeleteCustomerTokenAsync(CustomerToken customerToken)
        {
            await _customerTokenRepository.DeleteAsync(customerToken);
        }

        public virtual async Task DeleteExpiredTokensAsync()
        {
            var now = DateTimeOffset.UtcNow;
            var customerTokens = await _customerTokenRepository.Table.Where(x => x.RefreshTokenExpiresDateTime < now).ToListAsync();

            await _customerTokenRepository.DeleteAsync(customerTokens);
        }

        public virtual async Task DeleteTokensWithSameRefreshTokenSourceAsync(string refreshTokenIdHashSource)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenIdHashSource))
                return;

            var customerTokens = await _customerTokenRepository.Table
                .Where(x => x.RefreshTokenIdHashSource == refreshTokenIdHashSource || 
                    (x.RefreshTokenIdHash == refreshTokenIdHashSource && x.RefreshTokenIdHashSource == null))
                .ToListAsync();

            await _customerTokenRepository.DeleteAsync(customerTokens);
        }

        public async Task InvalidateCustomerTokensAsync(int customerId)
        {
            var customerTokens = await _customerTokenRepository.Table.Where(x => x.CustomerId == customerId).ToListAsync();

            await _customerTokenRepository.DeleteAsync(customerTokens);
        }

        public async Task<CustomerToken> IsValidTokenAsync(string accessTokenHash, int customerId)
        {
            return await _customerTokenRepository.Table.Where(x => x.AccessTokenHash == accessTokenHash && x.CustomerId == customerId)
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerToken> FindTokenAsync(string refreshTokenIdHash)
        {
            return await _customerTokenRepository.Table.Where(x => x.RefreshTokenIdHash == refreshTokenIdHash)
                .FirstOrDefaultAsync();
        }

    }
}