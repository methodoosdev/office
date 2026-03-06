using App.Core;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Traders;
using App.Services.Localization;
using App.Services.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Traders
{
    public partial interface ITraderMonthlyBillingModelFactory
    {
        Task<TraderMonthlyBillingSearchModel> PrepareTraderMonthlyBillingSearchModelAsync(TraderMonthlyBillingSearchModel searchModel);
        Task<TraderMonthlyBillingListModel> PrepareTraderMonthlyBillingListModelAsync(TraderMonthlyBillingSearchModel searchModel, int parentId);
        Task<TraderMonthlyBillingModel> PrepareTraderMonthlyBillingModelAsync(TraderMonthlyBillingModel model, TraderMonthlyBilling traderMonthlyBilling);
        Task<TraderMonthlyBillingFormModel> PrepareTraderMonthlyBillingFormModelAsync(TraderMonthlyBillingFormModel formModel);
    }
    public partial class TraderMonthlyBillingModelFactory : ITraderMonthlyBillingModelFactory
    {
        private readonly ITraderMonthlyBillingService _traderMonthlyBillingService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public TraderMonthlyBillingModelFactory(ITraderMonthlyBillingService traderMonthlyBillingService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _traderMonthlyBillingService = traderMonthlyBillingService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<TraderMonthlyBillingSearchModel> PrepareTraderMonthlyBillingSearchModelAsync(TraderMonthlyBillingSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.TraderMonthlyBillingModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        private async Task<IPagedList<TraderMonthlyBillingModel>> GetPagedListAsync(TraderMonthlyBillingSearchModel searchModel, int traderId)
        {
            var query = _traderMonthlyBillingService.Table.AsEnumerable()
                .Where(x => x.TraderId == traderId)
                .Select(x => x.ToModel<TraderMonthlyBillingModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                //query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<TraderMonthlyBillingListModel> PrepareTraderMonthlyBillingListModelAsync(TraderMonthlyBillingSearchModel searchModel, int parentId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var traderMonthlyBillings = await GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new TraderMonthlyBillingListModel().PrepareToGrid(searchModel, traderMonthlyBillings);

            return model;
        }

        public virtual Task<TraderMonthlyBillingModel> PrepareTraderMonthlyBillingModelAsync(TraderMonthlyBillingModel model, TraderMonthlyBilling traderMonthlyBilling)
        {
            if (traderMonthlyBilling != null)
            {
                //fill in model values from the entity
                model ??= traderMonthlyBilling.ToModel<TraderMonthlyBillingModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderMonthlyBillingModel>(1, nameof(TraderMonthlyBillingModel.Year), ColumnType.Numeric),
                ColumnConfig.Create<TraderMonthlyBillingModel>(2, nameof(TraderMonthlyBillingModel.Amount), ColumnType.Numeric)
            };

            return columns;
        }

        public virtual async Task<TraderMonthlyBillingFormModel> PrepareTraderMonthlyBillingFormModelAsync(TraderMonthlyBillingFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderMonthlyBillingModel>(nameof(TraderMonthlyBillingModel.Year), FieldType.Numeric),
                FieldConfig.Create<TraderMonthlyBillingModel>(nameof(TraderMonthlyBillingModel.Amount), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.TraderMonthlyBillingModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}