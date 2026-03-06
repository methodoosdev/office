using App.Core.Configuration;
using App.Core.Domain.Customers;
using App.Services.Customers;
using App.Services.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Jwt
{
    public interface ITokenFactoryService
    {
        Task<JwtTokensData> CreateJwtTokensAsync(Customer customer);
        string GetRefreshTokenSerial(string refreshTokenValue);
    }

    public class TokenFactoryService : ITokenFactoryService
    {
        private readonly AppSettings _appSettings;
        private readonly ISecurityService _securityService;
        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;

        public TokenFactoryService(AppSettings appSettings,
            ISecurityService securityService,
            ICustomerService customerService,
            ILogger logger)
        {
            _appSettings = appSettings;
            _securityService = securityService;
            _customerService = customerService;
            _logger = logger;
        }

        public async Task<JwtTokensData> CreateJwtTokensAsync(Customer customer)
        {
            var (accessToken, claims) = await createAccessTokenAsync(customer);
            var (refreshTokenValue, refreshTokenSerial) = createRefreshToken();
            return new JwtTokensData
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                RefreshTokenSerial = refreshTokenSerial,
                Claims = claims
            };
        }

        private (string RefreshTokenValue, string RefreshTokenSerial) createRefreshToken()
        {
            var refreshTokenSerial = _securityService.CreateCryptographicallySecureGuid().ToString().Replace("-", "");
            var config = _appSettings.Get<BearerTokenConfig>();

            var claims = new List<Claim>
            {
                // Unique Id for all Jwt tokes
                new Claim(JwtRegisteredClaimNames.Jti, _securityService.CreateCryptographicallySecureGuid().ToString(), ClaimValueTypes.String, config.Issuer),
                // Issuer
                new Claim(JwtRegisteredClaimNames.Iss, config.Issuer, ClaimValueTypes.String, config.Issuer),
                // Issued at
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64, config.Issuer),
                // for invalidation
                new Claim(ClaimTypes.SerialNumber, refreshTokenSerial, ClaimValueTypes.String, config.Issuer)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: config.Issuer,
                audience: config.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(config.RefreshTokenExpirationMinutes),
                signingCredentials: creds);
            var refreshTokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return (refreshTokenValue, refreshTokenSerial);
        }

        public string GetRefreshTokenSerial(string refreshTokenValue)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenValue))
            {
                return null;
            }

            ClaimsPrincipal decodedRefreshTokenPrincipal = null;
            var config = _appSettings.Get<BearerTokenConfig>();

            try
            {
                decodedRefreshTokenPrincipal = new JwtSecurityTokenHandler().ValidateToken(
                    refreshTokenValue,
                    new TokenValidationParameters
                    {
                        RequireExpirationTime = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Key)),
                        ValidateIssuerSigningKey = true, // verify signature to avoid tampering
                        ValidateLifetime = true, // validate the expiration
                        ClockSkew = TimeSpan.Zero // tolerance for the expiration date
                    },
                    out _
                );
            }
            catch
            {
                //_logger.ErrorAsync($"Failed to validate refreshTokenValue: `{refreshTokenValue}`.", ex);
            }

            return decodedRefreshTokenPrincipal?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value;
        }

        private async Task<(string AccessToken, IEnumerable<Claim> Claims)> createAccessTokenAsync(Customer customer)
        {
            var config = _appSettings.Get<BearerTokenConfig>();
            var claims = new List<Claim>
            {
                // Unique Id for all Jwt tokes
                new Claim(JwtRegisteredClaimNames.Jti, _securityService.CreateCryptographicallySecureGuid().ToString(), ClaimValueTypes.String, config.Issuer),
                // Issuer
                new Claim(JwtRegisteredClaimNames.Iss, config.Issuer, ClaimValueTypes.String, config.Issuer),
                // Issued at
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64, config.Issuer),
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString(), ClaimValueTypes.String, config.Issuer),
                new Claim(ClaimTypes.Name, customer.Username, ClaimValueTypes.String, config.Issuer),
                new Claim(ClaimTypes.Email, customer.Email, ClaimValueTypes.String, config.Issuer),
                new Claim("NickName", customer.NickName, ClaimValueTypes.String, config.Issuer),
                new Claim("SystemName", customer.SystemName, ClaimValueTypes.String, config.Issuer),
                // to invalidate the cookie
                new Claim(ClaimTypes.SerialNumber, customer.CustomerGuid.ToString("N"), ClaimValueTypes.String, config.Issuer),
                // custom data
                new Claim(ClaimTypes.UserData, customer.Id.ToString(), ClaimValueTypes.String, config.Issuer)
            };

            // add roles
            var roles = await _customerService.GetCustomerRolesAsync(customer);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name, ClaimValueTypes.String, config.Issuer));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: config.Issuer,
                audience: config.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(config.AccessTokenExpirationMinutes),
                signingCredentials: creds);
            return (new JwtSecurityTokenHandler().WriteToken(token), claims);
        }
    }
}