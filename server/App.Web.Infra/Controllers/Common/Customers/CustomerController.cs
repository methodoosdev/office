using App.Core;
using App.Core.Domain.Customers;
using App.Core.Domain.Forums;
using App.Core.Domain.Messages;
using App.Core.Events;
using App.Core.Infrastructure;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Customers;
using App.Services.Common;
using App.Services.Customers;
using App.Services.ExportImport;
using App.Services.Forums;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Messages;
using App.Services.Security;
using App.Services.Stores;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Customers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Customers
{
    public partial class CustomerController : BaseProtectController
    {
        #region Fields

        private readonly CustomerSettings _customerSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly ForumSettings _forumSettings;
        private readonly IExportToExcelService _exportToExcelService;
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IForumService _forumService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;

        #endregion

        #region Ctor

        public CustomerController(
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            EmailAccountSettings emailAccountSettings,
            ForumSettings forumSettings,
            IExportToExcelService exportToExcelService,
            ICustomerModelFactory customerModelFactory,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IEmailAccountService emailAccountService,
            IEventPublisher eventPublisher,
            IForumService forumService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IQueuedEmailService queuedEmailService,
            IStoreContext storeContext,
            IStoreService storeService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService
            )
        {
            _customerSettings = customerSettings;
            _dateTimeSettings = dateTimeSettings;
            _emailAccountSettings = emailAccountSettings;
            _forumSettings = forumSettings;
            _exportToExcelService = exportToExcelService;
            _customerModelFactory = customerModelFactory;
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _emailAccountService = emailAccountService;
            _eventPublisher = eventPublisher;
            _forumService = forumService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _queuedEmailService = queuedEmailService;
            _storeContext = storeContext;
            _storeService = storeService;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
        }

        #endregion

        #region Utilities

        protected virtual async Task<string> ValidateCustomerRolesAsync(IList<CustomerRole> customerRoles, IList<CustomerRole> existingCustomerRoles)
        {
            if (customerRoles == null)
                throw new ArgumentNullException(nameof(customerRoles));

            if (existingCustomerRoles == null)
                throw new ArgumentNullException(nameof(existingCustomerRoles));

            //check ACL permission to manage customer roles
            var rolesToAdd = customerRoles.Except(existingCustomerRoles);
            var rolesToDelete = existingCustomerRoles.Except(customerRoles);
            //if (rolesToAdd.Any(role => role.SystemName != NopCustomerDefaults.RegisteredRoleName) || rolesToDelete.Any())
            //{
            //    return await _localizationService.GetResourceAsync("App.CustomerErrors.CustomerRolesManagingError");
            //}

            //ensure a customer is not added to both 'Guests' and 'Registered' customer roles
            //ensure that a customer is in at least one required role ('Guests' and 'Registered')
            var isInGuestsRole = customerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.GuestsRoleName) != null;
            var isInRegisteredRole = customerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.RegisteredRoleName) != null;
            if (isInGuestsRole && isInRegisteredRole)
                return await _localizationService.GetResourceAsync("App.CustomerErrors.GuestsAndRegisteredRolesError");
            if (!isInGuestsRole && !isInRegisteredRole)
                return await _localizationService.GetResourceAsync("App.CustomerErrors.AddCustomerToGuestsOrRegisteredRoleError");

            //no errors
            return string.Empty;
        }

        private async Task<bool> SecondAdminAccountExistsAsync(Customer customer)
        {
            var customers = await _customerService.GetAllCustomersAsync(customerRoleIds: new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.AdministratorsRoleName)).Id });

            return customers.Any(c => c.Active && c.Id != customer.Id);
        }

        #endregion

        #region Customers

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _customerModelFactory.PrepareCustomerSearchModelAsync(new CustomerSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] CustomerSearchModel searchModel)
        {
            //prepare model
            var model = await _customerModelFactory.PrepareCustomerListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _customerModelFactory.PrepareCustomerModelAsync(new CustomerModel(), null);

            //prepare form
            var helperModel = new CustomerModelHelper();
            await _customerModelFactory.PrepareCustomerModelHelperAsync(helperModel);
            var formModel = await _customerModelFactory.PrepareCustomerFormModelAsync(new CustomerFormModel(), helperModel);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] CustomerModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Email) && await _customerService.GetCustomerByEmailAsync(model.Email) != null)
                ModelState.AddModelError(string.Empty, "Email is already registered");

            if (!string.IsNullOrWhiteSpace(model.Username) && _customerSettings.UsernamesEnabled &&
                await _customerService.GetCustomerByUsernameAsync(model.Username) != null)
            {
                ModelState.AddModelError(string.Empty, "Username is already registered");
            }

            //validate customer roles
            var allCustomerRoles = await _customerService.GetAllCustomerRolesAsync(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);
            var customerRolesError = await ValidateCustomerRolesAsync(newCustomerRoles, new List<CustomerRole>());
            if (!string.IsNullOrEmpty(customerRolesError))
            {
                ModelState.AddModelError(string.Empty, customerRolesError);
            }

            // Ensure that valid email address is entered if Registered role is checked to avoid registered customers with empty email address
            if (newCustomerRoles.Any() && newCustomerRoles.FirstOrDefault(c => c.SystemName == NopCustomerDefaults.RegisteredRoleName) != null &&
                !CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("App.CustomerErrors.ValidEmailRequiredRegisteredRole"));
            }

            if (ModelState.IsValid)
            {
                //fill entity from model
                var customer = model.ToEntity<Customer>();

                customer.Username = model.Email;
                customer.SystemName = model.SystemName;
                customer.CustomerGuid = Guid.NewGuid();
                customer.CreatedOnUtc = DateTime.UtcNow;
                customer.LastActivityDateUtc = DateTime.UtcNow;
                customer.RegisteredInStoreId = (await _storeContext.GetCurrentStoreAsync()).Id;

                await _customerService.InsertCustomerAsync(customer);

                //newsletter subscriptions
                if (!string.IsNullOrEmpty(customer.Email))
                {
                    var allStores = await _storeService.GetAllStoresAsync();
                    foreach (var store in allStores)
                    {
                        var newsletterSubscription = await _newsLetterSubscriptionService
                            .GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id);
                        if (model.SelectedNewsletterSubscriptionStoreIds != null &&
                            model.SelectedNewsletterSubscriptionStoreIds.Contains(store.Id))
                        {
                            //subscribed
                            if (newsletterSubscription == null)
                            {
                                await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = customer.Email,
                                    Active = true,
                                    StoreId = store.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                        else
                        {
                            //not subscribed
                            if (newsletterSubscription != null)
                            {
                                await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(newsletterSubscription);
                            }
                        }
                    }
                }

                //password
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    var changePassRequest = new ChangePasswordRequest(model.Email, false, _customerSettings.DefaultPasswordFormat, model.Password);
                    var changePassResult = await _customerRegistrationService.ChangePasswordAsync(changePassRequest);
                    if (!changePassResult.Success)
                    {
                        return BadRequest(changePassResult.Errors.FirstOrDefault());
                    }
                }

                //customer roles
                foreach (var customerRole in newCustomerRoles)
                {
                    //ensure that the current customer cannot add to "Administrators" system role if he's not an admin himself
                    if (customerRole.SystemName == NopCustomerDefaults.AdministratorsRoleName && !await _customerService.IsAdminAsync(await _workContext.GetCurrentCustomerAsync()))
                        continue;

                    await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = customerRole.Id });
                }

                //ensure that a customer with a employee associated is not in "Administrators" role
                //otherwise, he won't have access to other functionality in admin area
                if (await _customerService.IsAdminAsync(customer) && customer.EmployeeId > 0)
                {
                    return BadRequest(await _localizationService.GetResourceAsync("App.CustomerErrors.AdminCouldNotbeEmployee"));
                }

                //ensure that a customer with a trader associated is not in "Administrators" role
                //otherwise, he won't have access to other functionality in admin area
                if (await _customerService.IsAdminAsync(customer) && customer.TraderId > 0)
                {
                    return BadRequest(await _localizationService.GetResourceAsync("App.CustomerErrors.AdminCouldNotbeTrader"));
                }

                await _customerService.UpdateCustomerAsync(customer);

                return Json(customer.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get a customer with the specified id
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return await AccessDenied();

            //prepare model
            var model = await _customerModelFactory.PrepareCustomerModelAsync(null, customer);

            //prepare form
            var helperModel = new CustomerModelHelper();
            await _customerModelFactory.PrepareCustomerModelHelperAsync(helperModel);
            var formModel = await _customerModelFactory.PrepareCustomerFormModelAsync(new CustomerFormModel(), helperModel);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] CustomerModel model)
        {
            //try to get a customer with the specified id
            var customer = await _customerService.GetCustomerByIdAsync(model.Id);
            if (customer == null)
                return await AccessDenied();

            //validate customer roles
            var allCustomerRoles = await _customerService.GetAllCustomerRolesAsync(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);

            var customerRolesError = await ValidateCustomerRolesAsync(newCustomerRoles, await _customerService.GetCustomerRolesAsync(customer));

            if (!string.IsNullOrEmpty(customerRolesError))
            {
                ModelState.AddModelError(string.Empty, customerRolesError);
            }

            // Ensure that valid email address is entered if Registered role is checked to avoid registered customers with empty email address
            if (newCustomerRoles.Any() && newCustomerRoles.FirstOrDefault(c => c.SystemName == NopCustomerDefaults.RegisteredRoleName) != null &&
                !CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("App.CustomerErrors.ValidEmailRequiredRegisteredRole"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    customer.AdminComment = model.AdminComment;

                    //prevent deactivation of the last active administrator
                    if (!await _customerService.IsAdminAsync(customer) || model.Active || await SecondAdminAccountExistsAsync(customer))
                        customer.Active = model.Active;
                    else
                        return BadRequest(await _localizationService.GetResourceAsync("App.CustomerErrors.AdminAccountShouldExists"));

                    //email
                    if (!string.IsNullOrWhiteSpace(model.Email))
                        await _customerRegistrationService.SetEmailAsync(customer, model.Email, false);
                    else
                        customer.Email = model.Email;

                    //username
                    if (_customerSettings.UsernamesEnabled)
                    {
                        if (!string.IsNullOrWhiteSpace(model.Username))
                            await _customerRegistrationService.SetUsernameAsync(customer, model.Username);
                        else
                            customer.Username = model.Username;
                    }

                    //edit
                    customer.EmployeeId = model.EmployeeId;
                    customer.TraderId = model.TraderId;
                    //custom
                    customer.NickName = model.NickName;
                    customer.TimeZoneId = model.TimeZoneId;
                    customer.SystemName = model.SystemName;
                    customer.FirstName = model.FirstName;
                    customer.LastName = model.LastName;

                    //newsletter subscriptions
                    if (!string.IsNullOrEmpty(customer.Email))
                    {
                        var allStores = await _storeService.GetAllStoresAsync();
                        foreach (var store in allStores)
                        {
                            var newsletterSubscription = await _newsLetterSubscriptionService
                                .GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id);
                            if (model.SelectedNewsletterSubscriptionStoreIds != null &&
                                model.SelectedNewsletterSubscriptionStoreIds.Contains(store.Id))
                            {
                                //subscribed
                                if (newsletterSubscription == null)
                                {
                                    await _newsLetterSubscriptionService.InsertNewsLetterSubscriptionAsync(new NewsLetterSubscription
                                    {
                                        NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                        Email = customer.Email,
                                        Active = true,
                                        StoreId = store.Id,
                                        CreatedOnUtc = DateTime.UtcNow
                                    });
                                }
                            }
                            else
                            {
                                //not subscribed
                                if (newsletterSubscription != null)
                                {
                                    await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(newsletterSubscription);
                                }
                            }
                        }
                    }

                    var currentCustomerRoleIds = await _customerService.GetCustomerRoleIdsAsync(customer, true);

                    //customer roles
                    foreach (var customerRole in allCustomerRoles)
                    {
                        //ensure that the current customer cannot add/remove to/from "Administrators" system role
                        //if he's not an admin himself
                        if (customerRole.SystemName == NopCustomerDefaults.AdministratorsRoleName &&
                            !await _customerService.IsAdminAsync(await _workContext.GetCurrentCustomerAsync()))
                            continue;

                        if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                        {
                            //new role
                            if (currentCustomerRoleIds.All(roleId => roleId != customerRole.Id))
                                await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = customerRole.Id });
                        }
                        else
                        {
                            //prevent attempts to delete the administrator role from the user, if the user is the last active administrator
                            if (customerRole.SystemName == NopCustomerDefaults.AdministratorsRoleName && !await SecondAdminAccountExistsAsync(customer))
                            {
                                return BadRequest(await _localizationService.GetResourceAsync("App.CustomerErrors.AdminAccountShouldExists"));
                            }

                            //remove role
                            if (currentCustomerRoleIds.Any(roleId => roleId == customerRole.Id))
                                await _customerService.RemoveCustomerRoleMappingAsync(customer, customerRole);
                        }
                    }

                    //ensure that a customer with a employee associated is not in "Administrators" role
                    //otherwise, he won't have access to other functionality in admin area
                    if (await _customerService.IsAdminAsync(customer) && customer.EmployeeId > 0)
                    {
                        return BadRequest(await _localizationService.GetResourceAsync("App.CustomerErrors.AdminCouldNotbeEmployee"));
                    }

                    //ensure that a customer with a trader associated is not in "Administrators" role
                    //otherwise, he won't have access to other functionality in admin area
                    if (await _customerService.IsAdminAsync(customer) && customer.TraderId > 0)
                    {
                        return BadRequest(await _localizationService.GetResourceAsync("App.CustomerErrors.AdminCouldNotbeTrader"));
                    }

                    await _customerService.UpdateCustomerAsync(customer);

                    return Json(customer.Id);
                }
                catch
                {
                    return await BadRequestMessageAsync("App.Models.CustomerRoles.Errors.TryToEdit");
                }
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ChangePassword([FromBody] CustomerModel model)
        {
            //try to get a customer with the specified id
            var customer = await _customerService.GetCustomerByIdAsync(model.Id);
            if (customer == null)
                return await AccessDenied();

            //ensure that the current customer cannot change passwords of "Administrators" if he's not an admin himself
            if (await _customerService.IsAdminAsync(customer) && !await _customerService.IsAdminAsync(await _workContext.GetCurrentCustomerAsync()))
            {
                var error = await _localizationService.GetResourceAsync("App.CustomerErrors.OnlyAdminCanChangePassword");
                return await BadRequestMessageAsync(error);
            }

            if (!ModelState.IsValid)
                return BadRequestFromModel();

            var changePassRequest = new ChangePasswordRequest(model.Email, false, _customerSettings.DefaultPasswordFormat, model.Password);
            var changePassResult = await _customerRegistrationService.ChangePasswordAsync(changePassRequest);
            if (!changePassResult.Success)
                return await BadRequestMessageAsync(changePassResult.Errors.FirstOrDefault());

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer with the specified id
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return await AccessDenied();

            try
            {
                //prevent attempts to delete the user, if it is the last active administrator
                if (await _customerService.IsAdminAsync(customer) && !await SecondAdminAccountExistsAsync(customer))
                {
                    var error = await _localizationService.GetResourceAsync("App.CustomerErrors.AdminAccountShouldExists");
                    return await BadRequestMessageAsync(error);
                }

                //ensure that the current customer cannot delete "Administrators" if he's not an admin himself
                if (await _customerService.IsAdminAsync(customer) && !await _customerService.IsAdminAsync(await _workContext.GetCurrentCustomerAsync()))
                {
                    var error = await _localizationService.GetResourceAsync("App.CustomerErrors.OnlyAdminCanDeleteAdmin");
                    return await BadRequestMessageAsync(error);
                }

                //delete
                await _customerService.DeleteCustomerAsync(customer);

                //remove newsletter subscription (if exists)
                foreach (var store in await _storeService.GetAllStoresAsync())
                {
                    var subscription = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id);
                    if (subscription != null)
                        await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(subscription);
                }

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.CustomerRoles.Errors.TryToDelete");
            }
        }
        [HttpPost]
        public virtual async Task<IActionResult> SendWelcomeMessage(CustomerModel model)
        {
            //try to get a customer with the specified id
            var customer = await _customerService.GetCustomerByIdAsync(model.Id);
            if (customer == null)
                return await AccessDenied();

            await _workflowMessageService.SendCustomerWelcomeMessageAsync(customer, (await _workContext.GetWorkingLanguageAsync()).Id);

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReSendActivationMessage([FromBody] CustomerModel model)
        {
            //try to get a customer with the specified id
            var customer = await _customerService.GetCustomerByIdAsync(model.Id);
            if (customer == null)
                return await AccessDenied();

            //email validation message
            await _workflowMessageService.SendCustomerEmailValidationMessageAsync(customer, (await _workContext.GetWorkingLanguageAsync()).Id);

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> SendEmail([FromBody] CustomerModel model)
        {
            //try to get a customer with the specified id
            var customer = await _customerService.GetCustomerByIdAsync(model.Id);
            if (customer == null)
                return await AccessDenied();

            try
            {
                if (string.IsNullOrWhiteSpace(customer.Email))
                    throw new NopException("Customer email is empty");
                if (!CommonHelper.IsValidEmail(customer.Email))
                    throw new NopException("Customer email is not valid");
                if (string.IsNullOrWhiteSpace(model.SendEmail.Subject))
                    throw new NopException("Email subject is empty");
                if (string.IsNullOrWhiteSpace(model.SendEmail.Body))
                    throw new NopException("Email body is empty");

                var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId);
                if (emailAccount == null)
                    emailAccount = (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();
                if (emailAccount == null)
                    throw new NopException("Email account can't be loaded");
                var email = new QueuedEmail
                {
                    Priority = QueuedEmailPriority.High,
                    EmailAccountId = emailAccount.Id,
                    FromName = emailAccount.DisplayName,
                    From = emailAccount.Email,
                    ToName = customer.NickName,
                    To = customer.Email,
                    Subject = model.SendEmail.Subject,
                    Body = model.SendEmail.Body,
                    CreatedOnUtc = DateTime.UtcNow,
                    DontSendBeforeDateUtc = model.SendEmail.SendImmediately || !model.SendEmail.DontSendBeforeDate.HasValue ?
                        null : _dateTimeHelper.ConvertToUtcTime(model.SendEmail.DontSendBeforeDate.Value)
                };
                await _queuedEmailService.InsertQueuedEmailAsync(email);

                return Ok();
            }
            catch
            {
            }

            return await BadRequestMessageAsync("App.Models.CustomerRoles.Errors.TryToSendEmail");
        }

        [HttpPost]
        public virtual async Task<IActionResult> SendPm([FromBody] CustomerModel model)
        {
            //try to get a customer with the specified id
            var customer = await _customerService.GetCustomerByIdAsync(model.Id);
            if (customer == null)
                return await AccessDenied();

            try
            {
                if (!_forumSettings.AllowPrivateMessages)
                    throw new NopException("Private messages are disabled");
                if (await _customerService.IsGuestAsync(customer))
                    throw new NopException("Customer should be registered");
                if (string.IsNullOrWhiteSpace(model.SendPm.Subject))
                    throw new NopException(await _localizationService.GetResourceAsync("PrivateMessages.SubjectCannotBeEmpty"));
                if (string.IsNullOrWhiteSpace(model.SendPm.Message))
                    throw new NopException(await _localizationService.GetResourceAsync("PrivateMessages.MessageCannotBeEmpty"));

                var privateMessage = new PrivateMessage
                {
                    StoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                    ToCustomerId = customer.Id,
                    FromCustomerId = (await _workContext.GetCurrentCustomerAsync()).Id,
                    Subject = model.SendPm.Subject,
                    Text = model.SendPm.Message,
                    IsDeletedByAuthor = false,
                    IsDeletedByRecipient = false,
                    IsRead = false,
                    CreatedOnUtc = DateTime.UtcNow
                };

                await _forumService.InsertPrivateMessageAsync(privateMessage);

                return Ok();
            }
            catch
            {
            }

            return await BadRequestMessageAsync("App.Models.CustomerRoles.Errors.TryToSendPm");
        }

        #endregion

        #region Customer

        public virtual async Task<IActionResult> PrepareParentIdDialog()
        {
            //prepare form
            var formModel = await _customerModelFactory.PrepareCustomerDialogFormModelAsync(new CustomerDialogFormModel());

            return Json(formModel);

        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<IActionResult> LoadCustomerStatistics(string period)
        {
            var result = new List<object>();

            var nowDt = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
            var timeZone = await _dateTimeHelper.GetCurrentTimeZoneAsync();
            var searchCustomerRoleIds = new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName)).Id };

            var culture = new CultureInfo((await _workContext.GetWorkingLanguageAsync()).LanguageCulture);

            switch (period)
            {
                case "year":
                    //year statistics
                    var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
                    var searchYearDateUser = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
                    for (var i = 0; i <= 12; i++)
                    {
                        result.Add(new
                        {
                            date = searchYearDateUser.Date.ToString("Y", culture),
                            value = (await _customerService.GetAllCustomersAsync(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser.AddMonths(1), timeZone),
                                customerRoleIds: searchCustomerRoleIds,
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                        });

                        searchYearDateUser = searchYearDateUser.AddMonths(1);
                    }

                    break;
                case "month":
                    //month statistics
                    var monthAgoDt = nowDt.AddDays(-30);
                    var searchMonthDateUser = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
                    for (var i = 0; i <= 30; i++)
                    {
                        result.Add(new
                        {
                            date = searchMonthDateUser.Date.ToString("M", culture),
                            value = (await _customerService.GetAllCustomersAsync(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser.AddDays(1), timeZone),
                                customerRoleIds: searchCustomerRoleIds,
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                        });

                        searchMonthDateUser = searchMonthDateUser.AddDays(1);
                    }

                    break;
                case "week":
                default:
                    //week statistics
                    var weekAgoDt = nowDt.AddDays(-7);
                    var searchWeekDateUser = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
                    for (var i = 0; i <= 7; i++)
                    {
                        result.Add(new
                        {
                            date = searchWeekDateUser.Date.ToString("d dddd", culture),
                            value = (await _customerService.GetAllCustomersAsync(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser.AddDays(1), timeZone),
                                customerRoleIds: searchCustomerRoleIds,
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true)).TotalCount.ToString()
                        });

                        searchWeekDateUser = searchWeekDateUser.AddDays(1);
                    }

                    break;
            }

            return Json(result);
        }

        #endregion

        #region Export / Import

        [HttpPost, ActionName("ExportExcel")]
        [FormValueRequired("exportexcel-all")]
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<IActionResult> ExportExcelAll(CustomerSearchModel model)
        {
            var customers = await _customerService.GetAllCustomersAsync(customerRoleIds: model.SelectedCustomerRoleIds.ToArray(),
                email: model.SearchEmail,
                username: model.SearchUsername);

            try
            {
                var bytes = await _exportToExcelService.CustomerToXlsxAsync(customers);
                return File(bytes, MimeTypes.TextXlsx, "customers.xlsx");
            }
            catch
            {
                return await BadRequestMessageAsync("App.Errors.ExportExcelAll");
            }
        }

        [HttpPost]
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<IActionResult> ExportExcelSelected(string selectedIds)
        {
            var customers = new List<Customer>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                customers.AddRange(await _customerService.GetCustomersByIdsAsync(ids));
            }

            try
            {
                var bytes = await _exportToExcelService.CustomerToXlsxAsync(customers);
                return File(bytes, MimeTypes.TextXlsx, "customers.xlsx");
            }
            catch
            {
                return await BadRequestMessageAsync("App.Errors.ExportExcelSelected");
            }
        }

        //[HttpPost, ActionName("ExportXML")]
        //[FormValueRequired("exportxml-all")]
        ///// <returns>A task that represents the asynchronous operation</returns>
        //public virtual async Task<IActionResult> ExportXmlAll(CustomerSearchModel model)
        //{
        //    var customers = await _customerService.GetAllCustomersAsync(customerRoleIds: model.SelectedCustomerRoleIds.ToArray(),
        //        email: model.SearchEmail,
        //        username: model.SearchUsername);

        //    try
        //    {
        //        var xml = await _exportToExcelService.CustomerToXlsxAsync(customers);
        //        return File(Encoding.UTF8.GetBytes(xml), "application/xml", "customers.xml");
        //    }
        //    catch
        //    {
        //        return await BadRequestMessageAsync("App.Errors.ExportXmlAll");
        //    }
        //}

        //[HttpPost]
        ///// <returns>A task that represents the asynchronous operation</returns>
        //public virtual async Task<IActionResult> ExportXmlSelected(string selectedIds)
        //{
        //    var customers = new List<Customer>();
        //    if (selectedIds != null)
        //    {
        //        var ids = selectedIds
        //            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
        //            .Select(x => Convert.ToInt32(x))
        //            .ToArray();
        //        customers.AddRange(await _customerService.GetCustomersByIdsAsync(ids));
        //    }

        //    try
        //    {
        //        var xml = await _exportToExcelService.CustomerToXlsxAsync(customers);
        //        return File(Encoding.UTF8.GetBytes(xml), "application/xml", "customers.xml");
        //    }
        //    catch
        //    {
        //        return await BadRequestMessageAsync("App.Errors.ExportXmlSelected");
        //    }
        //}

        #endregion
    }
}