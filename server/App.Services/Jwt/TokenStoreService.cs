using App.Core.Configuration;
using App.Core.Domain.Customers;
using App.Services.Customers;
using System;
using System.Threading.Tasks;

namespace App.Services.Jwt
{
    public interface ITokenStoreService
    {
        Task AddCustomerTokenAsync(CustomerToken userToken);
        Task AddCustomerTokenAsync(Customer customer, string refreshTokenSerial, string accessToken, string refreshTokenSourceSerial);
        Task<CustomerToken> FindTokenAsync(string refreshTokenValue);
        Task DeleteTokenAsync(string refreshTokenValue);
        Task RevokeCustomerBearerTokensAsync(string customerIdValue, string refreshTokenValue);
    }

    public class TokenStoreService : ITokenStoreService
    {
        private readonly AppSettings _appSettings;
        private readonly ISecurityService _securityService;
        private readonly ICustomerTokenService _customerTokenService;
        private readonly ITokenFactoryService _tokenFactoryService;

        public TokenStoreService(AppSettings appSettings,
            ISecurityService securityService,
            ICustomerTokenService customerTokenService,
            ITokenFactoryService tokenFactoryService)
        {
            _appSettings = appSettings;
            _securityService = securityService;
            _customerTokenService = customerTokenService;
            _tokenFactoryService = tokenFactoryService;
        }

        public async Task AddCustomerTokenAsync(CustomerToken userToken)
        {
            var config = _appSettings.Get<BearerTokenConfig>();

            if (!config.AllowMultipleLoginsFromTheSameCustomer)
            {
                await _customerTokenService.InvalidateCustomerTokensAsync(userToken.CustomerId);
            }

            await _customerTokenService.DeleteTokensWithSameRefreshTokenSourceAsync(userToken.RefreshTokenIdHashSource);
            await _customerTokenService.InsertCustomerTokenAsync(userToken);
        }

        public async Task AddCustomerTokenAsync(Customer customer, string refreshTokenSerial, string accessToken, string refreshTokenSourceSerial)
        {
            var config = _appSettings.Get<BearerTokenConfig>();

            var now = DateTimeOffset.UtcNow;
            var token = new CustomerToken
            {
                CustomerId = customer.Id,
                // Refresh token handles should be treated as secrets and should be stored hashed
                RefreshTokenIdHash = _securityService.GetSha256Hash(refreshTokenSerial),
                RefreshTokenIdHashSource = string.IsNullOrWhiteSpace(refreshTokenSourceSerial) ?
                                           null : _securityService.GetSha256Hash(refreshTokenSourceSerial),
                AccessTokenHash = _securityService.GetSha256Hash(accessToken),
                RefreshTokenExpiresDateTime = now.AddMinutes(config.RefreshTokenExpirationMinutes),
                AccessTokenExpiresDateTime = now.AddMinutes(config.AccessTokenExpirationMinutes)
            };
            await AddCustomerTokenAsync(token);
        }

        public async Task DeleteTokenAsync(string refreshTokenValue)
        {
            var token = await FindTokenAsync(refreshTokenValue);
            if (token != null)
            {
                await _customerTokenService.DeleteCustomerTokenAsync(token);
            }
        }

        public async Task RevokeCustomerBearerTokensAsync(string customerIdValue, string refreshTokenValue)
        {
            var config = _appSettings.Get<BearerTokenConfig>();

            if (!string.IsNullOrWhiteSpace(customerIdValue) && int.TryParse(customerIdValue, out int customerId))
            {
                if (config.AllowSignoutAllCustomerActiveClients)
                {
                    await _customerTokenService.InvalidateCustomerTokensAsync(customerId);
                }
            }

            if (!string.IsNullOrWhiteSpace(refreshTokenValue))
            {
                var refreshTokenSerial = _tokenFactoryService.GetRefreshTokenSerial(refreshTokenValue);
                if (!string.IsNullOrWhiteSpace(refreshTokenSerial))
                {
                    var refreshTokenIdHashSource = _securityService.GetSha256Hash(refreshTokenSerial);
                    await _customerTokenService.DeleteTokensWithSameRefreshTokenSourceAsync(refreshTokenIdHashSource);
                }
            }

            await _customerTokenService.DeleteExpiredTokensAsync();
        }

        public async Task<CustomerToken> FindTokenAsync(string refreshTokenValue)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenValue))
                return null;

            var refreshTokenSerial = _tokenFactoryService.GetRefreshTokenSerial(refreshTokenValue);
            if (string.IsNullOrWhiteSpace(refreshTokenSerial))
                return null;

            var refreshTokenIdHash = _securityService.GetSha256Hash(refreshTokenSerial);
            var customerToken = await _customerTokenService.FindTokenAsync(refreshTokenIdHash);
            return customerToken;
        }
    }
}