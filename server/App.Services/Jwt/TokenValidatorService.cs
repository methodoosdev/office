using App.Services.Customers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App.Services.Jwt
{
    public interface ITokenValidatorService
    {
        Task ValidateAsync(TokenValidatedContext context);
        Task<bool> IsValidTokenAsync(string accessToken, int customerId);
    }

    public class TokenValidatorService : ITokenValidatorService
    {
        private readonly ISecurityService _securityService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerTokenService _customerTokenService;

        public TokenValidatorService(
            ISecurityService securityService,
            ICustomerService customerService,
            ICustomerTokenService customerTokenService)
        {
            _securityService = securityService;
            _customerService = customerService;
            _customerTokenService = customerTokenService;
        }

        public async Task ValidateAsync(TokenValidatedContext context)
        {
            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                context.Fail("This is not our issued token. It has no claims.");
                return;
            }

            var serialNumberClaim = claimsIdentity.FindFirst(ClaimTypes.SerialNumber);
            if (serialNumberClaim == null)
            {
                context.Fail("This is not our issued token. It has no serial.");
                return;
            }

            var customerIdString = claimsIdentity.FindFirst(ClaimTypes.UserData).Value;
            if (!int.TryParse(customerIdString, out int customerId))
            {
                context.Fail("This is not our issued token. It has no customer-id.");
                return;
            }

            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null || customer.CustomerGuid.ToString("N") != serialNumberClaim.Value || !customer.Active)
            {
                // customer has changed his/her password/roles/stat/IsActive
                context.Fail("This token is expired. Please login again.");
            }

            if (!(context.SecurityToken is JwtSecurityToken accessToken) || string.IsNullOrWhiteSpace(accessToken.RawData) ||
                !await IsValidTokenAsync(accessToken.RawData, customerId))
            {
                context.Fail("This token is not in our database.");
                return;
            }
        }

        public async Task<bool> IsValidTokenAsync(string accessToken, int customerId)
        {
            var accessTokenHash = _securityService.GetSha256Hash(accessToken);
            var customerToken = await _customerTokenService.IsValidTokenAsync(accessTokenHash, customerId);
            return customerToken == null ? false : customerToken.AccessTokenExpiresDateTime >= DateTimeOffset.UtcNow;
        }
    }
}