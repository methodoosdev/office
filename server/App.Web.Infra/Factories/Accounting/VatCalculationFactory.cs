using App.Core;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Accounting;
using App.Services.Configuration;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Infra.Queries.Accounting;
using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Accounting.Factories
{
    public partial interface IVatCalculationFactory
    {
        Task<IList<VatCalculationModel>> PrepareVatBCalculationListAsync(string connection, int companyId, bool prosvasis, int traderId, int fiscalYear);
        Task<IList<VatCalculationModel>> PrepareVatCalculationListAsync(string connection, int companyId, int traderId, int fiscalYear);
        Task<VatCalculationSearchModel> PrepareVatCalculationSearchModelAsync(VatCalculationSearchModel searchModel, List<SelectionItemList> years);
        Task<VatCalculationTableModel> PrepareVatCalculationTableModelAsync(VatCalculationTableModel tableModel);
    }
    public partial class VatCalculationFactory: IVatCalculationFactory
    {
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        public VatCalculationFactory(
            IFieldConfigService fieldConfigService,
            IAppDataProvider dataProvider,
            ILocalizationService localizationService, 
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _fieldConfigService = fieldConfigService;
            _dataProvider = dataProvider;
            _localizationService = localizationService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        public virtual async Task<IList<VatCalculationModel>> PrepareVatBCalculationListAsync(string connection, int companyId, bool prosvasis, int traderId, int fiscalYear)
        {
            var query = prosvasis ? VatCalculationQuery.Prosvasis : VatCalculationQuery.SoftOne_B;
            var dataList = new List<VatCalculationModel>();

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
            var pYear = new LinqToDB.Data.DataParameter("pYear", fiscalYear);

            var results = await _dataProvider.QueryAsync<VatCalculationResult>(connection, query, pCompanyId, pYear);

            var toReturnPrevious = 0; //results.Where(x => x.AccountingCode.IsLike("54.02.03.????.9900*")).Sum(k => k.Debit);
            var inTransferPrevious = results.Where(x => x.AccountingCode.IsLike("54.02.03.????.0000*")).Sum(k => k.Debit);

            dataList.Add(new VatCalculationModel { InTransfer = inTransferPrevious * -1, ToReturn = toReturnPrevious });

            var periods = new Dictionary<int, int[]>
            {
                [1] = new int[] { 1, 2, 3 },
                [2] = new int[] { 4, 5, 6 },
                [3] = new int[] { 7, 8, 9 },
                [4] = new int[] { 10, 11, 12 }
            };

            for (int i = 1; i <= 4; i++)
            {
                var period = periods[i];
                var format = string.Format("54.02.03.????.99{0}*", period.Last().ToString("0#"));

                var toReturnPeriod = results.Where(x => x.AccountingCode.IsLike(format) && period.Contains(x.Period)).Sum(k => k.Debit);

                //Εσοδα - Πωλησεις
                var sales = results.Where(x => x.AccountingCode.IsLike("54.02.02.7*") && period.Contains(x.Period)).Sum(k => k.Debit);
                //Εξοδα - Αγορες
                var expenses = results.Where(x => x.AccountingCode.IsLike("54.02.02.2*") && period.Contains(x.Period)).Sum(k => k.Debit);
                //Δαπανες
                var outlays = results.Where(x => x.AccountingCode.IsLike("54.02.02.6*") && period.Contains(x.Period)).Sum(k => k.Debit);
                //Παγια
                var assets = results.Where(x => x.AccountingCode.IsLike("54.02.02.1*") && period.Contains(x.Period)).Sum(k => k.Debit);

                var model = new VatCalculationModel();
                model.Period = await _localizationService.GetResourceAsync(DateLocaleResources.GetLocalePeriod(i));

                model.Sales = sales;
                model.Purchases = expenses + outlays + assets;
                model.CreditBalance = inTransferPrevious;
                var newBalance = sales - expenses - outlays - assets - inTransferPrevious;
                model.NewBalance = newBalance;
                model.ToPay = newBalance > 0 ? newBalance : 0;
                inTransferPrevious = newBalance < 0 ? Math.Abs(newBalance) - toReturnPeriod : 0;
                model.InTransfer = inTransferPrevious * -1;
                model.ToReturn = toReturnPeriod;

                dataList.Add(model);
            }

            return dataList;
        }

        public virtual async Task<IList<VatCalculationModel>> PrepareVatCalculationListAsync(string connection, int companyId, int traderId, int fiscalYear)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
            var pYear = new LinqToDB.Data.DataParameter("pYear", fiscalYear);

            var periodoi = await _dataProvider.QueryAsync<VatCalculationPeriodoi>(connection, VatCalculationQuery.Periodoi, pCompanyId, pYear);

            var dataList = new List<VatCalculationModel>();

            DateTime firstDayOfYear = periodoi.First(x => x.Period == 1).FromDate;//new DateTime(year, 1, 1);
            DateTime lastDayOfYear = periodoi.First(x => x.Period == 12).ToDate;//new DateTime(year, 12, 31);

            var schema = await _dataProvider.QuerySimpleAsync<int>(connection, VatCalculationQuery.FiscalPeriod, pCompanyId, pYear);

            var pSchema = new LinqToDB.Data.DataParameter("pSchema", schema);
            var pStartingDate = new LinqToDB.Data.DataParameter("pStartingDate", firstDayOfYear);
            var pEndingDate = new LinqToDB.Data.DataParameter("pEndingDate", lastDayOfYear);

            var refunds = await _dataProvider.QueryAsync<VatCalculationResult>(connection, VatCalculationQuery.Refund_SoftOne_C,
                pCompanyId, pSchema, pStartingDate, pEndingDate);

            var results = (await _dataProvider.QueryAsync<VatCalculationResult>(connection, VatCalculationQuery.SoftOne_C,
                pCompanyId, pSchema, pStartingDate, pEndingDate)).ToList();

            var toReturnPrevious = 0; //results.Where(x => x.AccountingCode.IsLike("54.02.03.????.9912*") && x.Period == 0).Sum(k => k.Credit - k.Debit);
            var inTransferPrevious = results.Where(x => x.AccountingCode.IsLike("54.02.03.????.0001*") && x.Period == 0).Sum(k => k.Credit - k.Debit);

            dataList.Add(new VatCalculationModel { InTransfer = inTransferPrevious * -1, ToReturn = toReturnPrevious });

            var culture = CultureInfo.CurrentCulture;

            var sales1 = results.Where(x => x.AccountingCode.IsLike("54.02.02.7*") && x.Period == 1).Sum(k => k.Debit - k.Credit);

            for (int month = 1; month <= 12; month++)
            {
                var toReturnPeriod = refunds.Where(x => x.Period == month).Sum(k => k.Credit - k.Debit);

                //Εσοδα - Πωλησεις
                var sales = results.Where(x => x.AccountingCode.IsLike("54.02.02.7*") && x.Period == month).Sum(k => k.Debit - k.Credit);
                //Εξοδα - Αγορες
                var expenses = results.Where(x => x.AccountingCode.IsLike("54.02.02.2*") && x.Period == month).Sum(k => k.Credit - k.Debit);
                //Δαπανες
                var outlays = results.Where(x => x.AccountingCode.IsLike("54.02.02.6*") && x.Period == month).Sum(k => k.Credit - k.Debit);
                //Παγια
                var assets = results.Where(x => x.AccountingCode.IsLike("54.02.02.1*") && x.Period == month).Sum(k => k.Credit - k.Debit);

                var model = new VatCalculationModel();
                model.Period = culture.DateTimeFormat.GetMonthName(periodoi.First(x => x.Period == month).FromDate.Month);

                model.Sales = sales;
                model.Purchases = expenses + outlays + assets;
                model.CreditBalance = inTransferPrevious;
                var newBalance = sales - expenses - outlays - assets - inTransferPrevious;
                model.NewBalance = newBalance;
                model.ToPay = newBalance > 0 ? newBalance : 0;
                inTransferPrevious = newBalance < 0 ? Math.Abs(newBalance) - toReturnPeriod : 0;
                model.InTransfer = inTransferPrevious * -1;
                model.ToReturn = toReturnPeriod;

                dataList.Add(model);
            }

            return dataList;
        }

        public virtual async Task<VatCalculationTableModel> PrepareVatCalculationTableModelAsync(VatCalculationTableModel tableModel)
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<VatCalculationModel>(2, nameof(VatCalculationModel.Period)),
                ColumnConfig.Create<VatCalculationModel>(3, nameof(VatCalculationModel.Sales), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<VatCalculationModel>(4, nameof(VatCalculationModel.Purchases), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<VatCalculationModel>(5, nameof(VatCalculationModel.CreditBalance), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<VatCalculationModel>(6, nameof(VatCalculationModel.NewBalance), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<VatCalculationModel>(7, nameof(VatCalculationModel.ToPay), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<VatCalculationModel>(8, nameof(VatCalculationModel.InTransfer), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<VatCalculationModel>(9, nameof(VatCalculationModel.ToReturn), ColumnType.Decimal, width: 120, style: textAlign)
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.VatCalculationModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }

        public async Task<VatCalculationSearchModel> PrepareVatCalculationSearchModelAsync(VatCalculationSearchModel searchModel, List<SelectionItemList> years)
        {
            var left = new List<Dictionary<string, object>>() 
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<VatCalculationSearchModel>(nameof(VatCalculationSearchModel.TraderId)) 
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<VatCalculationSearchModel>(nameof(VatCalculationSearchModel.Year), FieldType.Select, options: years)
            };

            var fields = (searchModel.TraderId > 0)
                ? FieldConfig.CreateFields(new string[] { "col-12 md:col-3" }, right)
                : FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-3" }, left, right);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return searchModel;
        }
    }
}