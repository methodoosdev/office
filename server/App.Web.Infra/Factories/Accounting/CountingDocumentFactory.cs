using App.Core;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Accounting;
using App.Services.Configuration;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Infra.Queries.Accounting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Accounting.Factories
{
    public partial interface ICountingDocumentFactory
    {
        Task<IList<CountingDocumentModel>> PrepareCountingDocumentListAsync(TraderConnectionResult connectionResult, int year);
        Task<CountingDocumentSearchModel> PrepareCountingDocumentSearchModelAsync(CountingDocumentSearchModel searchModel);
        Task<CountingDocumentTableModel> PrepareCountingDocumentTableModelAsync(CountingDocumentTableModel tableModel);
    }
    public partial class CountingDocumentFactory: ICountingDocumentFactory
    {
        private readonly IAppDataProvider _dataProvider;
        private readonly IFieldConfigService _fieldConfigService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public CountingDocumentFactory(
            IAppDataProvider dataProvider,
            IFieldConfigService fieldConfigService,
            ILocalizationService localizationService, 
            IWorkContext workContext)
        {
            _dataProvider = dataProvider;
            _fieldConfigService = fieldConfigService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<IList<CountingDocumentModel>> PrepareCountingDocumentListAsync(TraderConnectionResult connectionResult, int year)
        {
            DateTime firstDayOfYear = new DateTime(year, 1, 1);
            DateTime lastDayOfYear = new DateTime(year, 12, 31);

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pStartingDate = new LinqToDB.Data.DataParameter("pStartingDate", firstDayOfYear);
            var pEndingDate = new LinqToDB.Data.DataParameter("pEndingDate", lastDayOfYear);

            var results = await _dataProvider.QueryAsync<CountingDocumentModel>(connectionResult.Connection,
                CountingDocumentQuery.Get, pCompanyId, pStartingDate, pEndingDate);

            var dataList = new List<CountingDocumentModel>();

            foreach (var result in results)
            {
                result.DocType = await _localizationService.GetResourceAsync($"App.Models.CountingDocumentModel.Types.{result.DocType}");
                dataList.Add(result);
            }

            return dataList.OrderBy(x => x.DocType).ToList();
        }

        public virtual async Task<CountingDocumentTableModel> PrepareCountingDocumentTableModelAsync(CountingDocumentTableModel tableModel)
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<CountingDocumentModel>(1, nameof(CountingDocumentModel.DocType), hidden: true),
                ColumnConfig.Create<CountingDocumentModel>(2, nameof(CountingDocumentModel.DocName)),
                ColumnConfig.Create<CountingDocumentModel>(3, nameof(CountingDocumentModel.RecCount), ColumnType.Numeric, style: textAlign)
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.CountingDocumentModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }

        public async Task<CountingDocumentSearchModel> PrepareCountingDocumentSearchModelAsync(CountingDocumentSearchModel searchModel)
        {
            var trader = await _workContext.GetCurrentTraderAsync();

            searchModel.TraderId = trader?.Id ?? 0;
            searchModel.Periodos = DateTime.UtcNow;

            var left = new List<Dictionary<string, object>>() 
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<CountingDocumentSearchModel>(nameof(CountingDocumentSearchModel.TraderId), FieldConfigType.OnlySoftone) 
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<CountingDocumentSearchModel>(nameof(CountingDocumentSearchModel.Periodos), FieldType.YearDate)
            };

            var fields = (trader != null)
                ? FieldConfig.CreateFields(new string[] { "col-12 md:col-3" }, right)
                : FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-3" }, left, right);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return searchModel;
        }
    }
}