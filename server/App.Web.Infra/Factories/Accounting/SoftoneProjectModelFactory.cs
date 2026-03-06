using App.Core;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Framework.Models.Extensions;
using App.Models.Accounting;
using App.Services.Configuration;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Infra.Queries.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Accounting.Factories
{
    public partial interface ISoftoneProjectModelFactory
    {
        Task<SoftoneProjectInfoModel> PrepareSoftoneProjectInfoModelAsync(SoftoneProjectInfoModel infoModel);
        Task<SoftoneProjectInfoFormModel> PrepareSoftoneProjectInfoFormModelAsync(SoftoneProjectInfoFormModel infoFormModel);
        Task<SoftoneProjectSearchModel> PrepareSoftoneProjectSearchModelAsync(SoftoneProjectSearchModel searchModel);
        Task<SoftoneProjectListModel> PrepareSoftoneProjectListModelAsync(SoftoneProjectSearchModel searchModel, TraderConnectionResult connectionResult);
    }
    public partial class SoftoneProjectModelFactory : ISoftoneProjectModelFactory
    {
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public SoftoneProjectModelFactory(
            IFieldConfigService fieldConfigService,
            IDateTimeHelper dateTimeHelper,
            IAppDataProvider dataProvider,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _fieldConfigService = fieldConfigService;
            _dateTimeHelper = dateTimeHelper;
            _dataProvider = dataProvider;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<SoftoneProjectModel>> GetPagedListAsync(SoftoneProjectSearchModel searchModel, TraderConnectionResult connectionResult)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);

            var results = await _dataProvider.QueryAsync<SoftoneProjectQueryResult>(connectionResult.Connection, SoftoneProjectQuery.All, pCompanyId);

            var query = results.SelectAwait(async x => 
            {
                var model = new SoftoneProjectModel();

                var createdDate = x.CreatedDate.HasValue ? (await _dateTimeHelper
                    .ConvertToUserTimeAsync(x.CreatedDate.Value, DateTimeKind.Utc)).ToString("g") : null;

                var startingDate = x.StartingDate.HasValue ? (await _dateTimeHelper
                    .ConvertToUserTimeAsync(x.StartingDate.Value, DateTimeKind.Utc)).ToString("g") : null;

                var endingDate = x.EndingDate.HasValue ? (await _dateTimeHelper
                    .ConvertToUserTimeAsync(x.EndingDate.Value, DateTimeKind.Utc)).ToString("g") : null;

                model.Id = x.Id;
                model.Code = x.Code ?? "";
                model.Description = x.Description ?? "";
                model.Active = x.Active;
                model.Customer = x.Customer ?? "";
                model.CreatedDateValue = createdDate;
                model.StartingDateValue = startingDate;
                model.EndingDateValue = endingDate;
                model.CreatedDate = x.CreatedDate;
                model.StartingDate = x.StartingDate;
                model.EndingDate = x.EndingDate;

                return model;
            });

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.Code.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Description.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Customer.ContainsIgnoreCase(searchModel.QuickSearch)
                );
            }

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<SoftoneProjectInfoModel> PrepareSoftoneProjectInfoModelAsync(SoftoneProjectInfoModel infoModel)
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader != null)
                infoModel.TraderId = trader.Id;

            return infoModel;
        }

        public virtual async Task<SoftoneProjectInfoFormModel> PrepareSoftoneProjectInfoFormModelAsync(SoftoneProjectInfoFormModel infoFormModel)
        {
            var fields = new List<Dictionary<string, object>>() 
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<SoftoneProjectInfoModel>(nameof(SoftoneProjectInfoModel.TraderId)) 
            };

            infoFormModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return infoFormModel;
        }

        public virtual async Task<SoftoneProjectSearchModel> PrepareSoftoneProjectSearchModelAsync(SoftoneProjectSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.SoftoneProjectModel.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<SoftoneProjectListModel> PrepareSoftoneProjectListModelAsync(SoftoneProjectSearchModel searchModel, TraderConnectionResult connectionResult)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var softoneProjects = await GetPagedListAsync(searchModel, connectionResult);

            //prepare grid model
            var model = new SoftoneProjectListModel().PrepareToGrid(searchModel, softoneProjects);

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<SoftoneProjectModel>(1, nameof(SoftoneProjectModel.Code)),
                ColumnConfig.Create<SoftoneProjectModel>(2, nameof(SoftoneProjectModel.Description), ColumnType.RouterLink),
                ColumnConfig.Create<SoftoneProjectModel>(3, nameof(SoftoneProjectModel.Active), ColumnType.Checkbox),
                ColumnConfig.Create<SoftoneProjectModel>(4, nameof(SoftoneProjectModel.Customer)),
                ColumnConfig.Create<SoftoneProjectModel>(5, nameof(SoftoneProjectModel.CreatedDate), ColumnType.Date, style: textAlign),
                ColumnConfig.Create<SoftoneProjectModel>(6, nameof(SoftoneProjectModel.StartingDate), ColumnType.Date, style: textAlign),
                ColumnConfig.Create<SoftoneProjectModel>(7, nameof(SoftoneProjectModel.EndingDate), ColumnType.Date, style: textAlign)
            };

            return columns;
        }
    }
}