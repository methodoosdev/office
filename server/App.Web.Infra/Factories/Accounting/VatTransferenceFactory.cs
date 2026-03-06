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
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Accounting.Factories
{
    public partial interface IVatTransferenceFactory
    {
        Task<VatTransferenceSearchModel> PrepareVatTransferenceSearchModelAsync(VatTransferenceSearchModel searchModel);
        Task<IList<VatTransferenceModel>> PrepareVatTransferenceListAsync(TraderConnectionResult connectionResult, VatTransferenceSearchModel searchModel);
        Task<VatTransferenceTableModel> PrepareVatTransferenceTableModelAsync(VatTransferenceTableModel tableModel);
    }

    public class VatTransferenceFactory : IVatTransferenceFactory
    {
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly INumberHelper _numberHelper;
        private readonly IWorkContext _workContext;
        public VatTransferenceFactory(
            IFieldConfigService fieldConfigService,
            IAppDataProvider provider,
            ILocalizationService localizationService,
            INumberHelper numberHelper,
            IWorkContext workContext)
        {
            _fieldConfigService = fieldConfigService;
            _dataProvider = provider;
            _localizationService = localizationService;
            _numberHelper = numberHelper;
            _workContext = workContext;
        }

        public virtual async Task<IList<VatTransferenceModel>> PrepareVatTransferenceListAsync(TraderConnectionResult connectionResult, VatTransferenceSearchModel searchModel)
        {
            IList<VatTransferenceModel> list = new List<VatTransferenceModel>();
            IList<VatTransferenceResult> results = new List<VatTransferenceResult>();

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pPeriodFrom = new LinqToDB.Data.DataParameter("pPeriodFrom", searchModel.PeriodFrom.Month);
            var pPeriodTo = new LinqToDB.Data.DataParameter("pPeriodTo", searchModel.PeriodTo.Month);
            var pYear = new LinqToDB.Data.DataParameter("pYear", searchModel.PeriodTo.Year);



            results = await _dataProvider.QueryAsync<VatTransferenceResult>(connectionResult.Connection, VatTransferenceQuery.VatTransference, pCompanyId, pPeriodFrom, pPeriodTo, pYear);


            var vatList = results.GroupBy(x => x.Code).Select(x => new { Name = x.Key, Value = x.Max(f => f.Vat) });
            foreach (var item2 in vatList)
            {
                VatTransferenceModel vatTransferenceItem = new VatTransferenceModel();
               
                vatTransferenceItem.Vat = results.Where(x=>x.Code == item2.Name).Select(x=>x.Vat.Substring(x.Vat.LastIndexOf(' ') + 1)).FirstOrDefault();
                vatTransferenceItem.Sales = results.Where(x => x.Type == 0 && x.Code == item2.Name).Sum(x => x.REMAINVAL);
                vatTransferenceItem.Expenses = results.Where(x => x.Type == 1 && x.Code == item2.Name).Sum(x => x.REMAINVAL);
                vatTransferenceItem.Difference = vatTransferenceItem.Sales - vatTransferenceItem.Expenses;
                if (vatTransferenceItem.Difference >= 0)
                {
                    var percentage = vatTransferenceItem.Sales > 0 ? 1 - decimal.Round((vatTransferenceItem.Difference / vatTransferenceItem.Sales), 2, MidpointRounding.AwayFromZero) : 1 ;
                    vatTransferenceItem.Percentage = percentage * 100;

                }
                else
                {
                    var percentage = vatTransferenceItem.Sales > 0 ? 1 + decimal.Round((vatTransferenceItem.Difference / vatTransferenceItem.Sales), 2, MidpointRounding.AwayFromZero): 1;
                    vatTransferenceItem.Percentage = -percentage * 100;
                }
                list.Add(vatTransferenceItem);
            }

            return list;
        }

        public virtual async Task<VatTransferenceSearchModel> PrepareVatTransferenceSearchModelAsync(VatTransferenceSearchModel searchModel)
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            var date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            searchModel.PeriodFrom = firstDayOfMonth;
            searchModel.PeriodTo = lastDayOfMonth;
            searchModel.TraderId = trader?.Id ?? 0;

            var left = new List<Dictionary<string, object>>()
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<VatTransferenceSearchModel>(nameof(VatTransferenceSearchModel.TraderId), FieldConfigType.IndividualLegal)
            };
            var center = new List<Dictionary<string, object>>()
            {
                 FieldConfig.Create<VatTransferenceSearchModel>(nameof(VatTransferenceSearchModel.PeriodFrom),FieldType.MonthDate, className: "col-12"),
            };

            var right = new List<Dictionary<string, object>>()
            {

                FieldConfig.Create<VatTransferenceSearchModel>(nameof(VatTransferenceSearchModel.PeriodTo), FieldType.MonthDate, className: "col-12")
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-3", "col-12 md:col-3" }, left, center, right);


            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return searchModel;

        }

        public virtual async Task<VatTransferenceTableModel> PrepareVatTransferenceTableModelAsync(VatTransferenceTableModel tableModel)
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<VatTransferenceModel>(2, nameof(VatTransferenceModel.Vat)),
                ColumnConfig.Create<VatTransferenceModel>(3, nameof(VatTransferenceModel.Sales),  ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<VatTransferenceModel>(4, nameof(VatTransferenceModel.Expenses),  ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<VatTransferenceModel>(5, nameof(VatTransferenceModel.Difference),  ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<VatTransferenceModel>(6, nameof(VatTransferenceModel.Percentage), ColumnType.Percent, style: rightAlign)

            };
            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.VatTransferenceModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }

    }
}
