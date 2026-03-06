using App.Core;
using App.Core.Domain.Customers;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Customers;
using App.Services.Customers;
using App.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Customers
{
    /// <summary>
    /// Represents the customer role model factory implementation
    /// </summary>
    public partial class CustomerRoleModelFactory : ICustomerRoleModelFactory
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public CustomerRoleModelFactory(ICustomerService customerService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _customerService = customerService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        private async Task<IPagedList<CustomerRoleModel>> GetPagedListAsync(CustomerRoleSearchModel searchModel)
        {
            var query = _customerService.CustomerRoleTable.AsEnumerable()
                .Select(x => x.ToModel<CustomerRoleModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.Name.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.SystemName.ContainsIgnoreCase(searchModel.QuickSearch));
            }
            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        /// <summary>
        /// Prepare customer role search model
        /// </summary>
        /// <param name="searchModel">Customer role search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer role search model
        /// </returns>
        public virtual async Task<CustomerRoleSearchModel> PrepareCustomerRoleSearchModelAsync(CustomerRoleSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.CustomerRoleModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        /// <summary>
        /// Prepare paged customer role list model
        /// </summary>
        /// <param name="searchModel">Customer role search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer role list model
        /// </returns>
        public virtual async Task<CustomerRoleListModel> PrepareCustomerRoleListModelAsync(CustomerRoleSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            ////get customer roles
            var customerRoles = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new CustomerRoleListModel().PrepareToGrid(searchModel, customerRoles);

            return model;
        }

        /// <summary>
        /// Prepare customer role model
        /// </summary>
        /// <param name="model">Customer role model</param>
        /// <param name="customerRole">Customer role</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer role model
        /// </returns>
        public virtual Task<CustomerRoleModel> PrepareCustomerRoleModelAsync(CustomerRoleModel model, CustomerRole customerRole, bool excludeProperties = false)
        {
            if (customerRole != null)
            {
                //fill in model values from the entity
                model ??= customerRole.ToModel<CustomerRoleModel>();
            }

            //set default values for the new model
            if (customerRole == null)
                model.Active = true;

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<CustomerRoleModel>(1, nameof(CustomerRoleModel.Name), ColumnType.RouterLink),
                ColumnConfig.Create<CustomerRoleModel>(2, nameof(CustomerRoleModel.SystemName)),
                ColumnConfig.Create<CustomerRoleModel>(7, nameof(CustomerRoleModel.Active), ColumnType.Checkbox),
                ColumnConfig.Create<CustomerRoleModel>(8, nameof(CustomerRoleModel.IsSystemRole), ColumnType.Checkbox),
                ColumnConfig.Create<CustomerRoleModel>(9, nameof(CustomerRoleModel.EnablePasswordLifetime), ColumnType.Checkbox)
            };

            return columns;
        }

        public virtual async Task<CustomerRoleFormModel> PrepareCustomerRoleFormModelAsync(CustomerRoleFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<CustomerRoleModel>(nameof(CustomerRoleModel.Name), FieldType.Text),
                FieldConfig.Create<CustomerRoleModel>(nameof(CustomerRoleModel.SystemName), FieldType.Text),
                FieldConfig.Create<CustomerRoleModel>(nameof(CustomerRoleModel.Active), FieldType.Checkbox),
                FieldConfig.Create<CustomerRoleModel>(nameof(CustomerRoleModel.IsSystemRole), FieldType.Checkbox),
                FieldConfig.Create<CustomerRoleModel>(nameof(CustomerRoleModel.EnablePasswordLifetime), FieldType.Checkbox)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.CustomerRoleModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }

        #endregion
    }
}