using App.Core;
using App.Core.Domain.Customers;
using App.Core.Domain.Forums;
using App.Core.Domain.Media;
using App.Core.Infrastructure;
using App.Data;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Customers;
using App.Models.Traders;
using App.Services;
using App.Services.Common;
using App.Services.Customers;
using App.Services.Directory;
using App.Services.Employees;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Media;
using App.Services.Messages;
using App.Services.Stores;
using App.Services.Traders;
using App.Web.Framework.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Customers
{
    /// <summary>
    /// Represents the customer model factory implementation
    /// </summary>
    public partial class CustomerModelFactory : ICustomerModelFactory
    {
        #region Fields

        private readonly CustomerSettings _customerSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ForumSettings _forumSettings;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly ICountryService _countryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IEmployeeService _employeeService;
        private readonly ITraderService _traderService;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGeoLookupService _geoLookupService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly MediaSettings _mediaSettings;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CustomerModelFactory(
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            ForumSettings forumSettings,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IModelFactoryService modelFactoryService,
            ICountryService countryService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IEmployeeService employeeService,
            ITraderService traderService,
            IRepository<Customer> customerRepository,
            IDateTimeHelper dateTimeHelper,
            IGenericAttributeService genericAttributeService,
            IGeoLookupService geoLookupService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IStoreContext storeContext,
            IStoreService storeService,
            MediaSettings mediaSettings,
            IWorkContext workContext)
        {
            _customerSettings = customerSettings;
            _dateTimeSettings = dateTimeSettings;
            _forumSettings = forumSettings;
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _modelFactoryService = modelFactoryService;
            _countryService = countryService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _employeeService = employeeService;
            _traderService = traderService;
            _customerRepository = customerRepository;
            _dateTimeHelper = dateTimeHelper;
            _genericAttributeService = genericAttributeService;
            _geoLookupService = geoLookupService;
            _localizationService = localizationService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _storeContext = storeContext;
            _storeService = storeService;
            _mediaSettings = mediaSettings;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare customer activity log search model
        /// </summary>
        /// <param name="searchModel">Customer activity log search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer activity log search model</returns>
        protected virtual CustomerActivityLogSearchModel PrepareCustomerActivityLogSearchModel(CustomerActivityLogSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        #endregion

        #region Methods

        public virtual async Task<CustomerSearchModel> PrepareCustomerSearchModelAsync(CustomerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //search registered customers by default
            var registeredRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
            if (registeredRole != null)
                searchModel.SelectedCustomerRoleIds.Add(registeredRole.Id);

            //prepare available customer roles
            //await _aclSupportedModelFactory.PrepareModelCustomerRolesAsync(searchModel);

            //prepare page parameters
            //searchModel.State = new GridState("email", "asc");
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.CustomerModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        private async Task<IPagedList<CustomerModel>> GetPagedListAsync(CustomerSearchModel searchModel)
        {
            var query = _customerService.GetAllCustomersByRoles(searchModel.SelectedCustomerRoleIds.ToArray())
            .SelectAwait(async customer =>
            {
                //fill in model values from the entity
                var customerModel = customer.ToModel<CustomerModel>();

                var trader = await _traderService.GetTraderByIdAsync(customerModel.TraderId);

                //convert dates to the user time
                customerModel.Email = await _customerService.IsRegisteredAsync(customer)
                        ? customer.Email
                        : "guest@guest"; // await _localizationService.GetResourceAsync("Admin.Customers.Guest");
                customerModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(customer.CreatedOnUtc, DateTimeKind.Utc);
                customerModel.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(customer.LastActivityDateUtc, DateTimeKind.Utc);

                //fill in additional values (not existing in the entity)
                customerModel.CustomerRoleNames = string.Join(", ",
                    (await _customerService.GetCustomerRolesAsync(customer)).Select(role => role.Name));
                customerModel.NickName = customer.NickName;
                customerModel.SystemName = customer.SystemName;

                customerModel.EmployeeName = (await _employeeService.GetEmployeeByIdAsync(customerModel.EmployeeId))?.FullName() ?? "";
                customerModel.TraderName = trader?.ToTraderFullName() ?? "";

                return customerModel;
            });

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.Email.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.NickName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.SystemName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.EmployeeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.TraderName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.Where(c => c.CustomerRoleNames.Contains(NopCustomerDefaults.RegisteredRoleName));

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<CustomerListModel> PrepareCustomerListModelAsync(CustomerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var customers = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new CustomerListModel().PrepareToGrid(searchModel, customers);

            return model;
        }

        public virtual async Task<CustomerModel> PrepareCustomerModelAsync(CustomerModel model, Customer customer)
        {
            if (customer != null)
            {
                //fill in model values from the entity
                model ??= new CustomerModel();

                model.Id = customer.Id;
                model.AllowSendingOfPrivateMessage = await _customerService.IsRegisteredAsync(customer) &&
                    _forumSettings.AllowPrivateMessages;
                model.AllowSendingOfWelcomeMessage = await _customerService.IsRegisteredAsync(customer) &&
                    _customerSettings.UserRegistrationType == UserRegistrationType.AdminApproval;
                model.AllowReSendingOfActivationMessage = await _customerService.IsRegisteredAsync(customer) && !customer.Active &&
                    _customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation;

                model.Email = customer.Email;
                model.Username = customer.Email;
                model.AdminComment = customer.AdminComment;
                model.Active = customer.Active;
                model.TimeZoneId = customer.TimeZoneId;
                //model.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(customer.CreatedOnUtc, DateTimeKind.Utc);
                //model.LastActivityDate = await _dateTimeHelper.ConvertToUserTimeAsync(customer.LastActivityDateUtc, DateTimeKind.Utc);
                model.CreatedOn = customer.CreatedOnUtc;
                model.LastActivityDate = customer.LastActivityDateUtc;
                model.LastIpAddress = customer.LastIpAddress;
                model.SelectedCustomerRoleIds = (await _customerService.GetCustomerRoleIdsAsync(customer)).ToList();
                model.RegisteredInStore = (await _storeService.GetAllStoresAsync())
                    .FirstOrDefault(store => store.Id == customer.RegisteredInStoreId)?.Name ?? string.Empty;

                //prepare model newsletter subscriptions
                if (!string.IsNullOrEmpty(customer.Email))
                {
                    model.SelectedNewsletterSubscriptionStoreIds = await (await _storeService.GetAllStoresAsync())
                        .WhereAwait(async store => await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id) != null)
                        .Select(store => store.Id).ToListAsync();
                }
                //custom
                //model.AvatarUrl = ?
                model.NickName = customer.NickName;
                model.EmployeeId = customer.EmployeeId;
                model.TraderId = customer.TraderId;
                model.SystemName = customer.SystemName;
                model.NumPosts = customer.NumPosts;
                model.FirstName = customer.FirstName;
                model.LastName = customer.LastName;
            }

            //set default values for the new model
            if (customer == null)
            {
                //precheck Registered Role as a default role while creating a new customer through admin
                var registeredRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
                if (registeredRole != null)
                    model.SelectedCustomerRoleIds.Add(registeredRole.Id);

                model.Active = true;
                //model.CreatedOn = DateTime.UtcNow;
                //model.LastActivityDate = DateTime.UtcNow;
                //model.CreatedOn = DateTime.UtcNow;//await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.UtcNow, DateTimeKind.Utc);
            }

            return model;
        }

        public virtual async Task PrepareCustomerModelHelperAsync(CustomerModelHelper model)
        {
            //prepare available vendors
            model.AvailableEmployees = await _modelFactoryService.GetAllEmployeesAsync();
            model.AvailableTraders = await _modelFactoryService.GetAllTradersAsync();

            //prepare available time zones
            model.AvailableTimeZones = await _modelFactoryService.GetAllTimeZonesAsync();

            //prepare available countries and states
            model.AvailableCountries = await _modelFactoryService.GetAllCountriesAsync();

            model.AvailableCustomerRoles = await _modelFactoryService.GetAllCustomerRolesAsync();
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<CustomerModel>(1, nameof(CustomerModel.Email), ColumnType.RouterLink),
                ColumnConfig.Create<CustomerModel>(9, nameof(CustomerModel.AdminComment)),
                ColumnConfig.Create<CustomerModel>(2, nameof(CustomerModel.NickName)),
                ColumnConfig.Create<CustomerModel>(3, nameof(CustomerModel.SystemName)),
                ColumnConfig.Create<CustomerModel>(4, nameof(CustomerModel.EmployeeName)),
                ColumnConfig.Create<CustomerModel>(5, nameof(CustomerModel.TraderName)),
                ColumnConfig.Create<CustomerModel>(8, nameof(CustomerModel.LastIpAddress)),
                ColumnConfig.Create<CustomerModel>(9, nameof(CustomerModel.Active), ColumnType.Checkbox)
            };

            return columns;
        }

        public virtual async Task<CustomerFormModel> PrepareCustomerFormModelAsync(CustomerFormModel formModel, CustomerModelHelper model)
        {
            //prepare item text
            var choiceLabel = await _localizationService.GetResourceAsync("App.Common.Choice");

            var availableCustomerRoles = await _modelFactoryService.GetAllCustomerRolesAsync();

            var _availableCustomerRoles = availableCustomerRoles
                .Select(x => new SelectionList
                {
                    Label = x.Label,
                    Value = x.Label
                }).ToList();

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<CustomerModel>(nameof(CustomerModel.Email), FieldType.Text, markAsRequired: true),
                FieldConfig.Create<CustomerModel>(nameof(CustomerModel.Password), FieldType.TextButton),
                FieldConfig.Create<CustomerModel>(nameof(CustomerModel.LastName), FieldType.Text),
                FieldConfig.Create<CustomerModel>(nameof(CustomerModel.FirstName), FieldType.Text),
                FieldConfig.Create<CustomerModel>(nameof(CustomerModel.NickName), FieldType.Text, markAsRequired: true),
                FieldConfig.Create<CustomerModel>(nameof(CustomerModel.SystemName), FieldType.Select, markAsRequired: true, options: _availableCustomerRoles),
                FieldConfig.Create<CustomerModel>(nameof(CustomerModel.EmployeeId), FieldType.Select, options: model.AvailableEmployees),
                //FieldConfig.Create<CustomerModel>(nameof(CustomerModel.TraderId), FieldType.Select, options: model.AvailableTraders),
                FieldConfig.Create<CustomerModel>(nameof(CustomerModel.TraderId), FieldType.GridSelect, options: model.AvailableTraders, placeholder: choiceLabel),

                FieldConfig.Create<CustomerModel>(nameof(CustomerModel.SelectedCustomerRoleIds), FieldType.MultiSelect, markAsRequired: true, options: availableCustomerRoles),

                FieldConfig.Create<CustomerModel>(nameof(CustomerModel.Active), FieldType.Checkbox),
                FieldConfig.Create<CustomerModel>(nameof(CustomerModel.AdminComment), FieldType.Textarea),
                FieldConfig.Create<CustomerModel>(nameof(CustomerModel.TimeZoneId), FieldType.Select, options: model.AvailableTimeZones),

                FieldConfig.Create<CustomerModel>(nameof(CustomerModel.LastIpAddress), FieldType.Text, _readonly: true),
                FieldConfig.Create<CustomerModel>(nameof(CustomerModel.CreatedOn), FieldType.Date, _readonly: true),
                FieldConfig.Create<CustomerModel>(nameof(CustomerModel.LastActivityDate), FieldType.Date, _readonly: true),
                FieldConfig.Create<CustomerModel>(nameof(CustomerModel.RegisteredInStore), FieldType.Text, _readonly: true)
            };

            var panels = new List<Dictionary<string, object>>()
            {
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Common.About"), true, "col-12 md:col-6", fields)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.CustomerModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", panels);

            return formModel;
        }

        public virtual async Task<CustomerDialogFormModel> PrepareCustomerDialogFormModelAsync(CustomerDialogFormModel formModel)
        {
            var customers = await _customerService.Table
                .Where(x => !string.IsNullOrEmpty(x.Email))
                .Select(x => new SelectionItemList(x.Id, x.Email))
                .OrderBy(x => x.Label)
                .ToListAsync();

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<CustomerDialogModel>(nameof(CustomerDialogModel.ParentId), FieldType.GridSelect, options: customers, min: 1, required: true)
            };

            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }

        #endregion
    }
}