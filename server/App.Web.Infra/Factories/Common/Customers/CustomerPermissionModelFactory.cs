using App.Core;
using App.Core.Domain.Customers;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Customers;
using App.Services.Customers;
using App.Services.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Customers
{
    public partial interface ICustomerPermissionModelFactory
    {
        IList<CustomerPermissionModel> GetCustomerPermissionModelList();
        Task<CustomerPermissionSearchModel> PrepareCustomerPermissionSearchModelAsync(CustomerPermissionSearchModel searchModel);
        Task<CustomerPermissionListModel> PrepareCustomerPermissionListModelAsync(CustomerPermissionSearchModel searchModel);
        Task<CustomerPermissionModel> PrepareCustomerPermissionModelAsync(CustomerPermissionModel model, CustomerPermission customerPermission, bool excludeProperties = false);
        Task<CustomerPermissionFormModel> PrepareCustomerPermissionFormModelAsync(CustomerPermissionFormModel formModel, CustomerPermissionModel model);
    }
    public partial class CustomerPermissionModelFactory : ICustomerPermissionModelFactory
    {
        private readonly ICustomerPermissionService _customerPermissionService;
        private readonly ICustomerPermissionCustomerMappingService _customerPermissionCustomerMappingService;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public CustomerPermissionModelFactory(
            ICustomerPermissionService customerPermissionService,
            ICustomerPermissionCustomerMappingService customerPermissionCustomerMappingService,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _customerPermissionService = customerPermissionService;
            _customerPermissionCustomerMappingService = customerPermissionCustomerMappingService;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual IList<CustomerPermissionModel> GetCustomerPermissionModelList()
        {
            var actionDescriptors = _actionDescriptorCollectionProvider.ActionDescriptors.Items
                .Select(s => new
                {
                    Area = s.RouteValues["area"],
                    Controller = s.RouteValues["controller"],
                    Action = s.RouteValues["action"]
                }).ToList();

            var customerPermission = actionDescriptors
                .Where(k => !string.IsNullOrEmpty(k.Area))
                .Select(x => new CustomerPermissionModel
                {
                    Name = $"{x.Area}, {x.Controller}, {x.Action}",
                    SystemName = $"{x.Area}:{x.Controller}:{x.Action}",
                    Area = x.Area,
                    Controller = x.Controller,
                    Action = x.Action
                }).GroupBy(g => g.SystemName).Select(n => n.First()).ToList();

            return customerPermission;
        }

        private async Task PrepareControllerNamesAsync(IList<SelectionList> items, bool withSpecialDefaultItem = true)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available
            var controllerNames = GetCustomerPermissionModelList();

            foreach (var controller in controllerNames)
            {
                items.Add(new SelectionList { Value = controller.SystemName, Label = controller.Name });
            }

            //whether to insert the first special item for the default value
            if (!withSpecialDefaultItem)
                return;

            //prepare item text
            var defaultItemText = await _localizationService.GetResourceAsync("App.Common.Choice");

            //insert this default item at first
            items.Insert(0, new SelectionList { Label = defaultItemText, Value = null });
        }

        private async Task<IPagedList<CustomerPermissionModel>> GetPagedListAsync(CustomerPermissionSearchModel searchModel)
        {
            var query = _customerPermissionService.Table
                .SelectAwait(async x =>
                {
                    var customers = await _customerPermissionCustomerMappingService.GetCustomersByCustomerPermissionIdAsync(x.Id);
                    var model = x.ToModel<CustomerPermissionModel>();

                    model.Used = customers.Count();

                    return model;
                });

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Name.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.SystemName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Area.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Controller.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Action.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<CustomerPermissionSearchModel> PrepareCustomerPermissionSearchModelAsync(CustomerPermissionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var customer = await _workContext.GetCurrentCustomerAsync();

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.CustomerPermissionModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<CustomerPermissionListModel> PrepareCustomerPermissionListModelAsync(CustomerPermissionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var customerPermissions = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new CustomerPermissionListModel().PrepareToGrid(searchModel, customerPermissions);

            return model;
        }

        public virtual async Task<CustomerPermissionModel> PrepareCustomerPermissionModelAsync(CustomerPermissionModel model, CustomerPermission customerPermission, bool excludeProperties = false)
        {
            if (customerPermission != null)
            {
                //fill in model values from the entity
                model ??= customerPermission.ToModel<CustomerPermissionModel>();
            }

            await PrepareControllerNamesAsync(model.AvailableControllerNames);

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<CustomerPermissionModel>(1, nameof(CustomerPermissionModel.Name), ColumnType.RouterLink),
                ColumnConfig.Create<CustomerPermissionModel>(2, nameof(CustomerPermissionModel.SystemName)),
                ColumnConfig.Create<CustomerPermissionModel>(3, nameof(CustomerPermissionModel.Category), hidden: true),
                ColumnConfig.Create<CustomerPermissionModel>(4, nameof(CustomerPermissionModel.Menu), hidden: true),
                ColumnConfig.Create<CustomerPermissionModel>(5, nameof(CustomerPermissionModel.Area)),
                ColumnConfig.Create<CustomerPermissionModel>(6, nameof(CustomerPermissionModel.Controller)),
                ColumnConfig.Create<CustomerPermissionModel>(7, nameof(CustomerPermissionModel.Action)),
                ColumnConfig.Create<CustomerPermissionModel>(8, nameof(CustomerPermissionModel.Used), ColumnType.Checkbox)

            };

            return columns;
        }

        public virtual async Task<CustomerPermissionFormModel> PrepareCustomerPermissionFormModelAsync(CustomerPermissionFormModel formModel, CustomerPermissionModel model)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<CustomerPermissionModel>(nameof(CustomerPermissionModel.Name), FieldType.Text),
                FieldConfig.Create<CustomerPermissionModel>(nameof(CustomerPermissionModel.SystemName), FieldType.Select, options: model.AvailableControllerNames),
                FieldConfig.Create<CustomerPermissionModel>(nameof(CustomerPermissionModel.Category), FieldType.Text),
                FieldConfig.Create<CustomerPermissionModel>(nameof(CustomerPermissionModel.Menu), FieldType.Text),
                FieldConfig.Create<CustomerPermissionModel>(nameof(CustomerPermissionModel.Area), FieldType.Text, _readonly: true),
                FieldConfig.Create<CustomerPermissionModel>(nameof(CustomerPermissionModel.Controller), FieldType.Text, _readonly: true),
                FieldConfig.Create<CustomerPermissionModel>(nameof(CustomerPermissionModel.Action), FieldType.Text, _readonly: true)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.CustomerPermissionModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}