using App.Core;
using App.Core.Domain.Customers;
using App.Core.Domain.Security;
using App.Core.Events;
using App.Models.Traders;
using App.Services.Authentication;
using App.Services.Customers;
using App.Services.Employees;
using App.Services.Jwt;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Traders;
using App.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class AccountController : BasePublicController
    {
        private readonly CaptchaSettings _captchaSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly IAntiForgeryCookieService _antiforgeryCookieService;
        private readonly ITokenFactoryService _tokenFactoryService;
        private readonly ITokenStoreService _tokenStoreService;
        private readonly IJwtAuthenticationService _authenticationService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerPermissionService _customerPermissionService;
        private readonly ICustomerService _customerService;
        private readonly ITraderService _traderService;
        private readonly IEmployeeService _employeeService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        public AccountController(
            CaptchaSettings captchaSettings,
            CustomerSettings customerSettings,
            IAntiForgeryCookieService antiforgeryCookieService,
            ITokenFactoryService tokenFactoryService,
            ITokenStoreService tokenStoreService,
            IJwtAuthenticationService authenticationService,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerPermissionService customerPermissionService,
            ICustomerService customerService,
            ITraderService traderService,
            IEmployeeService employeeService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            ILanguageService languageService,
            ILogger logger,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _captchaSettings = captchaSettings;
            _customerSettings = customerSettings;
            _antiforgeryCookieService = antiforgeryCookieService;
            _tokenFactoryService = tokenFactoryService;
            _tokenStoreService = tokenStoreService;
            _authenticationService = authenticationService;
            _customerRegistrationService = customerRegistrationService;
            _customerPermissionService = customerPermissionService;
            _customerService = customerService;
            _traderService = traderService;
            _employeeService = employeeService;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _languageService = languageService;
            _logger = logger;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [HttpPost]
        //[ValidateCaptcha]
        public virtual async Task<IActionResult> Login([FromBody] LoginModel model, string returnUrl, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                var customerUserName = model.Username?.Trim();
                var customerEmail = model.Email?.Trim();
                var userNameOrEmail = _customerSettings.UsernamesEnabled ? customerUserName : customerEmail;

                var loginResult = await _customerRegistrationService.ValidateCustomerAsync(userNameOrEmail, model.Password);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled
                                ? await _customerService.GetCustomerByUsernameAsync(customerUserName)
                                : await _customerService.GetCustomerByEmailAsync(customerEmail);

                            return await _customerRegistrationService.SignInCustomerAsync(customer, returnUrl, model.RememberMe);
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("App.WrongCredentials.CustomerNotExist"));
                        break;
                    case CustomerLoginResults.Deleted:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("App.WrongCredentials.Deleted"));
                        break;
                    case CustomerLoginResults.NotActive:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("App.WrongCredentials.NotActive"));
                        break;
                    case CustomerLoginResults.NotRegistered:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("App.WrongCredentials.NotRegistered"));
                        break;
                    case CustomerLoginResults.LockedOut:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("App.WrongCredentials.LockedOut"));
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("App.Errors.WrongCredentials"));
                        break;
                }
            }

            //If we got this far, something failed, redisplay form
            var err = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault();
            return BadRequest(err.ErrorMessage);
        }

        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {
            var refreshTokenValue = model?.RefreshToken;
            if (string.IsNullOrWhiteSpace(refreshTokenValue))
                return BadRequest("RefreshToken is not set.");

            var token = await _tokenStoreService.FindTokenAsync(refreshTokenValue);
            if (token == null)
                return Unauthorized("03");

            var customer = await _customerService.GetCustomerByIdAsync(token.CustomerId);
            var result = await _tokenFactoryService.CreateJwtTokensAsync(customer);
            await _tokenStoreService.AddCustomerTokenAsync(customer, result.RefreshTokenSerial, result.AccessToken, _tokenFactoryService.GetRefreshTokenSerial(refreshTokenValue));

            _antiforgeryCookieService.RegenerateAntiForgeryCookies(result.Claims);

            return Ok(new { access_token = result.AccessToken, refresh_token = result.RefreshToken });
        }

        [AllowAnonymous]
        public virtual async Task<bool> Logout(string refreshToken)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var customerIdValue = claimsIdentity?.FindFirst(ClaimTypes.UserData)?.Value;

            // The Jwt implementation does not support "revoke OAuth token" (logout) by design.
            // Delete the user's tokens from the database (revoke its bearer token)
            await _tokenStoreService.RevokeCustomerBearerTokensAsync(customerIdValue, refreshToken);

            _antiforgeryCookieService.DeleteAntiForgeryCookies();

            //standard logout 
            await _authenticationService.SignOutAsync();

            if (int.TryParse(customerIdValue, out int customerId))
            {
                var customer = await _customerService.GetCustomerByIdAsync(customerId);
                if (customer == null)
                    return false;

                //raise logged out event       
                //await _eventPublisher.PublishAsync(new CustomerLoggedOutEvent(customer));
            }

            return true;
        }

        [AllowAnonymous]
        public virtual async Task<IActionResult> GetAppInfo(string localeId)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var language = await _workContext.GetWorkingLanguageAsync();
            var currency = await _workContext.GetWorkingCurrencyAsync();
            var languages = await _languageService.GetAllLanguagesAsync();
            var menus = (await _customerPermissionService.GetCustomerPermissionsByCustomerIdAsync(customer.Id))
                .Select(x => $"{x.Area}:{x.Controller}:{x.Action}").ToList();

            var trader = await _traderService.GetTraderByIdAsync(customer.TraderId);
            var employee = await _employeeService.GetEmployeeByIdAsync(customer.EmployeeId);

            var data = new
            {
                language,
                currency,
                languages,
                menus,
                trader = trader == null ? null : new
                {
                    id = trader.Id,
                    fullName = trader.ToTraderFullName(),
                    bookId = trader.CategoryBookTypeId
                },
                employee = employee == null ? null : new
                {
                    id = employee.Id,
                    fullName = employee.FullName(),
                    email = employee.EmailContact
                }
            };

            return Json(data);
        }
    }
}