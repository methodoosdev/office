using App.Core;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Common;
using App.Models.Customers;
using App.Models.Traders;
using App.Services.Customers;
using App.Services.Localization;
using App.Services.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Customers
{
    public partial interface ICustomerOnlineModelFactory
    {
        Task<CustomerOnlineSearchModel> PrepareCustomerOnlineSearchModelAsync(CustomerOnlineSearchModel searchModel);
        Task<CustomerOnlineListModel> PrepareCustomerOnlineListModelAsync(CustomerOnlineSearchModel searchModel);
    }
    public partial class CustomerOnlineModelFactory : ICustomerOnlineModelFactory
    {
        private readonly ITraderService _traderService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerOnlineService _customerOnlineService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public CustomerOnlineModelFactory(ITraderService traderService,
            ICustomerService customerService,
            ICustomerOnlineService customerOnlineService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _customerService = customerService;
            _customerOnlineService = customerOnlineService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<CustomerOnlineModel>> GetPagedListAsync(CustomerOnlineSearchModel searchModel)
        {
            var query = _customerOnlineService.Table
                .SelectAwait(async x =>
                {
                    var customer = await _customerService.GetCustomerByEmailAsync(x.Email);
                    var trader = await _traderService.GetTraderByIdAsync(customer.TraderId);

                    var model = x.ToModel<CustomerOnlineModel>();
                    model.CompanyName = trader.ToTraderFullName() ?? "";
                    model.LastLoginDate = x.LastLoginDateUtc;

                    return model;
                });

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.Email.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.CompanyName.ContainsIgnoreCase(searchModel.QuickSearch)
                    );
            }

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<CustomerOnlineSearchModel> PrepareCustomerOnlineSearchModelAsync(CustomerOnlineSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.CustomerOnlineModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<CustomerOnlineListModel> PrepareCustomerOnlineListModelAsync(CustomerOnlineSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var customerOnlines = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new CustomerOnlineListModel().PrepareToGrid(searchModel, customerOnlines);

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<CustomerOnlineModel>(1, nameof(CustomerOnlineModel.Email)),
                ColumnConfig.Create<CustomerOnlineModel>(2, nameof(CustomerOnlineModel.CompanyName)),
                ColumnConfig.Create<CustomerOnlineModel>(3, nameof(CustomerOnlineModel.LastIpAddress)),
                ColumnConfig.Create<CustomerOnlineModel>(4, nameof(CustomerOnlineModel.LastLoginDate), ColumnType.DateTime, style: rightAlign),
                ColumnConfig.Create<CustomerOnlineModel>(5, nameof(CustomerOnlineModel.Online), ColumnType.Checkbox),
                ColumnConfig.Create<CustomerOnlineModel>(6, nameof(CustomerOnlineModel.Visits), style: rightAlign)
            };

            return columns;
        }
    }
}