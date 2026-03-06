using App.Core;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Accounting;
using App.Services.Configuration;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Infra.Queries;
using App.Web.Infra.Queries.Accounting;
using App.Web.Infra.SqlQueries;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Accounting.Factories
{
    public partial interface IAggregateAnalysisFactory
    {
        Task<AggregateAnalysisSearchModel> PrepareAggregateAnalysisSearchModelAsync(AggregateAnalysisSearchModel searchModel, List<SelectionItemList> years, List<SelectionItemList> periods);
        Task<IList<AggregateAnalysisModel>> PrepareAggregateAnalysisOfMonthAsync(string connection, int companyId, AggregateAnalysisSearchModel searchModel);
        Task<IList<AggregateAnalysisModel>> PrepareAggregateAnalysisOfPeriodAsync(string connection, int companyId, AggregateAnalysisSearchModel searchModel);
        Task<AggregateAnalysisTableModel> PrepareAggregateAnalysisTableModelAsync(AggregateAnalysisTableModel tableModel);
        Task<AggregateAnalysisTotalTableModel> PrepareAggregateAnalysisTotalTableModelAsync(AggregateAnalysisTotalTableModel tableModel);
    }
    public partial class AggregateAnalysisFactory : IAggregateAnalysisFactory
    {
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ISoftoneQueryFactory _softoneQueryFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        public AggregateAnalysisFactory(
            IFieldConfigService fieldConfigService,
            IAppDataProvider dataProvider,
            ISoftoneQueryFactory softoneQueryFactory,
            ILocalizationService localizationService, 
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _fieldConfigService = fieldConfigService;
            _dataProvider = dataProvider;
            _softoneQueryFactory = softoneQueryFactory;
            _localizationService = localizationService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        private async Task<IList<AggregateAnalysisItem>> GetAccountingCodesAsync(string connection, string query, string code, params DataParameter[] parameters)
        {
            var prms = parameters.ToList();
            prms.Add(new LinqToDB.Data.DataParameter("pCode", code));
            parameters = prms.ToArray();

            var list = await _dataProvider.QueryAsync<AggregateAnalysisItem>(connection, query, parameters);

            return list;
        }

        private async Task<decimal> GetAggregateAnalysisPeriodsAsync(
            string connection, int companyId, int schema, int year, string code, int fromPeriod, int toPeriod)
        {
            var pSchema = new LinqToDB.Data.DataParameter("pSchema", schema);
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
            var pYear = new LinqToDB.Data.DataParameter("pYear", year);
            var pCode = new LinqToDB.Data.DataParameter("pCode", code);

            var pFromPeriod = new LinqToDB.Data.DataParameter("pFromPeriod", fromPeriod);
            var pToPeriod = new LinqToDB.Data.DataParameter("pToPeriod", toPeriod);

            var value = await _dataProvider
                .QuerySimpleAsync<decimal>(connection, SoftOneQuery.AggregateAnalysisPeriods,
                    pSchema, pCompanyId, pYear, pCode, pFromPeriod, pToPeriod);

            return value;
        }

        public virtual async Task<IList<AggregateAnalysisModel>> PrepareAggregateAnalysisOfMonthAsync(string connection, int companyId, AggregateAnalysisSearchModel searchModel)
        {
            var fiscalYear = (await _softoneQueryFactory.GetCompanyYearUsesAsync(connection, companyId, searchModel.Year)).First();
            var currentPeriod = searchModel.Period;

            var periods = await _softoneQueryFactory.GetPeriodsPerYearAsync(connection, companyId, fiscalYear.Year);

            periods.TryGetValue(currentPeriod, out var startingPeriod);
            periods.TryGetValue(currentPeriod, out var endingPeriod);

            var pSchema = new LinqToDB.Data.DataParameter("pSchema", fiscalYear.Schema);
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
            var pFromDate = new LinqToDB.Data.DataParameter("pFromDate", startingPeriod.From.Date);
            var pToDate = new LinqToDB.Data.DataParameter("pToDate", endingPeriod.To.Date);

            //orders
            var ordersList = await _dataProvider.QueryAsync<AggregateAnalysisItem>(connection, SoftOneQuery.Orders, pCompanyId, pFromDate, pToDate);

            //receipts
            var receiptsList = await _dataProvider.QueryAsync<AggregateAnalysisItem>(connection, SoftOneQuery.Receipts, pCompanyId, pFromDate, pToDate);

            //payments
            var paymentsList = await _dataProvider.QueryAsync<AggregateAnalysisItem>(connection, SoftOneQuery.Payments, pCompanyId, pFromDate, pToDate);

            //2*, 30, 50, 62, 63, 64, 65, 67, 70, 
            var purchasesList = await GetAccountingCodesAsync(connection, SoftOneQuery.AggregateAnalysisV2, "2%", pSchema, pCompanyId, pFromDate, pToDate);

            var customersList = await GetAccountingCodesAsync(connection, SoftOneQuery.AggregateAnalysisV2, "30%", pSchema, pCompanyId, pFromDate, pToDate);

            var suppliersList = await GetAccountingCodesAsync(connection, SoftOneQuery.AggregateAnalysisV2, "50%", pSchema, pCompanyId, pFromDate, pToDate);

            var expensesList = await GetAccountingCodesAsync(connection, SoftOneQuery.AggregateAnalysisV2, "6%", pSchema, pCompanyId, pFromDate, pToDate);

            var salesList = await GetAccountingCodesAsync(connection, SoftOneQuery.AggregateAnalysisV2, "70%", pSchema, pCompanyId, pFromDate, pToDate);

            var results = ordersList.Concat(receiptsList).Concat(paymentsList).Concat(purchasesList)
                .Concat(customersList).Concat(suppliersList).Concat(expensesList).Concat(salesList).ToList();
            var dates = results.Select(x => x.Date).Distinct().OrderBy(o => o).ToList();
            
            var modelList = new List<AggregateAnalysisModel>();

            var previeusYear = await _softoneQueryFactory.CheckPrevieusYearExistsAsync(connection, companyId, searchModel.Year - 1);

            decimal customersBalance = 0;
            decimal suppliersBalance = 0;

            if (previeusYear.Exists)
            {
                // Take all periods from previeus year with apografi
                customersBalance += await GetAggregateAnalysisPeriodsAsync(
                    connection, companyId, previeusYear.Schema, searchModel.Year - 1, "30%", 0, 12);

                suppliersBalance += await GetAggregateAnalysisPeriodsAsync(
                    connection, companyId, previeusYear.Schema, searchModel.Year - 1, "50%", 0, 12);
            }
            else
            {
                // Take only apografi from current year
                customersBalance += await GetAggregateAnalysisPeriodsAsync(
                    connection, companyId, fiscalYear.Schema, searchModel.Year, "30%", 0, 0);

                suppliersBalance += await GetAggregateAnalysisPeriodsAsync(
                    connection, companyId, fiscalYear.Schema, searchModel.Year, "50%", 0, 0);
            }

            var _model = new AggregateAnalysisModel { Date = "Απογραφή", CustomerBalance = customersBalance, SuppliersBalance = suppliersBalance };
            modelList.Add(_model);

            if (currentPeriod > 1)
            {
                var customersTransfer = await GetAggregateAnalysisPeriodsAsync(
                    connection, companyId, fiscalYear.Schema, searchModel.Year, "30%", 1, currentPeriod - 1);

                var suppliersTransfer = await GetAggregateAnalysisPeriodsAsync(
                    connection, companyId, fiscalYear.Schema, searchModel.Year, "50%", 1, currentPeriod - 1);

                modelList.Add(new AggregateAnalysisModel { Date = "Από μεταφορά", CustomerBalance = customersTransfer, SuppliersBalance = suppliersTransfer });

                customersBalance += customersTransfer;
                suppliersBalance += suppliersTransfer;
            }

            foreach (var date in dates)
            {
                //filter by date
                var list = results.Where(d => d.Date == date).ToList();

                var model = new AggregateAnalysisModel { Date = (await _dateTimeHelper.ConvertToUserTimeAsync(date)).ToString("dd/MM/yyyy") };

                //sales
                var sales = list.Where(x => x.Id.IsLike("70.??.11*")).Select(x => x.Total).Sum();

                //salesExports
                var salesPatterns = new [] { "70.??.12*", "70.??.13*" };
                var salesExports = list.Where(x => x.Id.IsLike(salesPatterns)).Select(x => x.Total).Sum();

                //salesReturns
                var salesReturns = list.Where(x => x.Id.IsLike(new [] { "70.??.2*", "70.??.3*" })).Select(x => x.Total).Sum();

                //salesTotal
                var salesTotal = sales + salesExports + salesReturns;

                //orders
                var orders = list.Where(x => x.Id.Equals("Orders")).Select(x => x.Total).Sum();

                //purchases
                var purchases = list.Where(x => x.Id.IsLike("2?.?2.?1*")).Select(x => x.Total).Sum();

                //purchasesImports
                var purchasesImports = list.Where(x => x.Id.IsLike(new [] { "2?.?2.2*", "2?.?2.3*" }) ).Select(x => x.Total).Sum();

                //purchasesReturns
                var purchasesReturns = list.Where(x => x.Id.IsLike(new [] { "2?.?3*", "2?.?4*" }) ).Select(x => x.Total).Sum();

                //purchasesTotal
                var purchasesTotal = purchases + purchasesImports + purchasesReturns;

                //customerBalance
                customersBalance += list.Where(x => x.Id.IsLike("30*")).Select(x => x.Total).Sum();

                //suppliersBalance
                suppliersBalance += list.Where(x => x.Id.IsLike("50*")).Select(x => x.Total).Sum();

                //warehouseBalance
                var warehouseBalance = 0m;

                //receipts
                var receipts = list.Where(x => x.Id.Equals("Receipts")).Select(x => x.Total).Sum();

                //payments
                var payments = list.Where(x => x.Id.Equals("Payments")).Select(x => x.Total).Sum();

                //expenses
                var expenses = list.Where(x => x.Id.IsLike(new [] { "62*", "63*", "64*", "65*", "67*" })).Select(x => x.Total).Sum();

                model.Sales = sales;
                model.SalesExports = salesExports;
                model.SalesReturns = salesReturns;
                model.SalesTotal = salesTotal;
                model.Orders = orders;
                model.Purchases = purchases;
                model.PurchasesImports = purchasesImports;
                model.PurchasesReturns = purchasesReturns;
                model.PurchasesTotal = purchasesTotal;
                model.CustomerBalance = customersBalance;
                model.SuppliersBalance = suppliersBalance;
                model.WarehouseBalance = warehouseBalance;
                model.Receipts = receipts;
                model.Payments = payments;
                model.Expenses = expenses;

                modelList.Add(model);
            }

            var totalModel = new AggregateAnalysisModel
            {
                Sales = modelList.Select(x => x.Sales).Sum(),
                SalesExports = modelList.Select(x => x.SalesExports).Sum(),
                SalesReturns = modelList.Select(x => x.SalesReturns).Sum(),
                SalesTotal = modelList.Select(x => x.SalesTotal).Sum(),
                Orders = modelList.Select(x => x.Orders).Sum(),
                Purchases = modelList.Select(x => x.Purchases).Sum(),
                PurchasesImports = modelList.Select(x => x.PurchasesImports).Sum(),
                PurchasesReturns = modelList.Select(x => x.PurchasesReturns).Sum(),
                PurchasesTotal = modelList.Select(x => x.PurchasesTotal).Sum(),
                CustomerBalance = modelList.Select(x => x.CustomerBalance)?.LastOrDefault() ?? 0,
                SuppliersBalance = modelList.Select(x => x.SuppliersBalance)?.LastOrDefault() ?? 0,
                WarehouseBalance = modelList.Select(x => x.WarehouseBalance).Sum(),
                Receipts = modelList.Select(x => x.Receipts).Sum(),
                Payments = modelList.Select(x => x.Payments).Sum(),
                Expenses = modelList.Select(x => x.Expenses).Sum()
            };
            modelList.Add(totalModel);

            return modelList;
        }

        public virtual async Task<IList<AggregateAnalysisModel>> PrepareAggregateAnalysisOfPeriodAsync(string connection, int companyId, AggregateAnalysisSearchModel searchModel)
        {
            var fiscalYear = (await _softoneQueryFactory.GetCompanyYearUsesAsync(connection, companyId, searchModel.Year)).First();
            var currentPeriod = searchModel.Period;

            var periods = await _softoneQueryFactory.GetPeriodsPerYearAsync(connection, companyId, fiscalYear.Year);

            periods.TryGetValue(1, out var startingPeriod);
            periods.TryGetValue(currentPeriod, out var endingPeriod);

            var pSchema = new LinqToDB.Data.DataParameter("pSchema", fiscalYear.Schema);
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
            var pFromDate = new LinqToDB.Data.DataParameter("pFromDate", startingPeriod.From.Date);
            var pToDate = new LinqToDB.Data.DataParameter("pToDate", endingPeriod.To.Date);

            //orders
            var ordersList = await _dataProvider.QueryAsync<AggregateAnalysisItem>(connection, SoftOneQuery.Orders, pCompanyId, pFromDate, pToDate);

            //receipts
            var receiptsList = await _dataProvider.QueryAsync<AggregateAnalysisItem>(connection, SoftOneQuery.Receipts, pCompanyId, pFromDate, pToDate);

            //payments
            var paymentsList = await _dataProvider.QueryAsync<AggregateAnalysisItem>(connection, SoftOneQuery.Payments, pCompanyId, pFromDate, pToDate);

            var modelList = new List<AggregateAnalysisModel>();

            var previeusYear = await _softoneQueryFactory.CheckPrevieusYearExistsAsync(connection, companyId, fiscalYear.Year - 1);

            decimal customersBalance = 0;
            decimal suppliersBalance = 0;

            if (previeusYear.Exists)
            {
                // Take all periods from previeus year with apografi
                customersBalance += await GetAggregateAnalysisPeriodsAsync(
                    connection, companyId, previeusYear.Schema, fiscalYear.Year - 1, "30%", 0, 12);

                suppliersBalance += await GetAggregateAnalysisPeriodsAsync(
                    connection, companyId, previeusYear.Schema, fiscalYear.Year - 1, "50%", 0, 12);
            }
            else
            {
                // Take only apografi from current year
                customersBalance += await GetAggregateAnalysisPeriodsAsync(
                    connection, companyId, fiscalYear.Schema, fiscalYear.Year, "30%", 0, 0);

                suppliersBalance += await GetAggregateAnalysisPeriodsAsync(
                    connection, companyId, fiscalYear.Schema, fiscalYear.Year, "50%", 0, 0);
            }

            var from = (await _dateTimeHelper.ConvertToUserTimeAsync(startingPeriod.From.Date)).ToString("dd/MM/yyyy");
            var to = (await _dateTimeHelper.ConvertToUserTimeAsync(endingPeriod.To.Date)).ToString("dd/MM/yyyy");

            var model = new AggregateAnalysisModel { Date = $"{from} - {to}" };

            var builder = new AggregateAnalysisPeriodsSql();

            //sales
            var sales = builder.Get(
                connection, companyId, fiscalYear.Schema, fiscalYear.Year, 1, currentPeriod, new[] { "70.__.11%" });

            //salesExports
            var salesExports = builder.Get(
                connection, companyId, fiscalYear.Schema, fiscalYear.Year, 1, currentPeriod, new[] { "70.__.12%", "70.__.13%" });

            //salesReturns
            var salesReturns = builder.Get(
                connection, companyId, fiscalYear.Schema, fiscalYear.Year, 1, currentPeriod, new[] { "70.__.2%", "70.__.3%" });

            //salesTotal
            var salesTotal = sales + salesExports + salesReturns;

            //orders
            var orders = ordersList.Where(x => x.Id.Equals("Orders")).Sum(x => x.Total);

            //purchases
            var purchases = builder.Get(
                connection, companyId, fiscalYear.Schema, fiscalYear.Year, 1, currentPeriod, new[] { "2_._2._1%" });

            //purchasesImports
            var purchasesImports = builder.Get(
                connection, companyId, fiscalYear.Schema, fiscalYear.Year, 1, currentPeriod, new[] { "2_._2.2%", "2_._2.3%" });

            //purchasesReturns
            var purchasesReturns = builder.Get(
                connection, companyId, fiscalYear.Schema, fiscalYear.Year, 1, currentPeriod, new[] { "2_._3%", "2_._4%" });

            //purchasesTotal
            var purchasesTotal = purchases + purchasesImports + purchasesReturns;

            //customerBalance
            customersBalance += builder.Get(
                connection, companyId, fiscalYear.Schema, fiscalYear.Year, 1, currentPeriod, new[] { "30%" });

            //suppliersBalance
            suppliersBalance += builder.Get(
                connection, companyId, fiscalYear.Schema, fiscalYear.Year, 1, currentPeriod, new[] { "50%" });

            //warehouseBalance
            var warehouseBalance = 0m;

            //receipts
            var receipts = receiptsList.Where(x => x.Id.Equals("Receipts")).Sum(x => x.Total);

            //payments
            var payments = paymentsList.Where(x => x.Id.Equals("Payments")).Sum(x => x.Total);

            //expenses
            var expenses = builder.Get(
                connection, companyId, fiscalYear.Schema, fiscalYear.Year, 1, currentPeriod, new[] { "62%", "63%", "64%", "65%", "67%" });

            model.Sales = sales;
            model.SalesExports = salesExports;
            model.SalesReturns = salesReturns;
            model.SalesTotal = salesTotal;
            model.Orders = orders;
            model.Purchases = purchases;
            model.PurchasesImports = purchasesImports;
            model.PurchasesReturns = purchasesReturns;
            model.PurchasesTotal = purchasesTotal;
            model.CustomerBalance = customersBalance;
            model.SuppliersBalance = suppliersBalance;
            model.WarehouseBalance = warehouseBalance;
            model.Receipts = receipts;
            model.Payments = payments;
            model.Expenses = expenses;

            modelList.Add(model);

            return modelList;
        }

        public virtual async Task<AggregateAnalysisSearchModel> PrepareAggregateAnalysisSearchModelAsync(AggregateAnalysisSearchModel searchModel, List<SelectionItemList> years, List<SelectionItemList> periods)
        {
            var left = new List<Dictionary<string, object>>()
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<AggregateAnalysisSearchModel>(nameof(AggregateAnalysisSearchModel.TraderId), FieldConfigType.WithCategoryBookC)
            };

            var right1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AggregateAnalysisSearchModel>(nameof(AggregateAnalysisSearchModel.Year), FieldType.Select, options: years)
            };

            var right2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AggregateAnalysisSearchModel>(nameof(AggregateAnalysisSearchModel.Period), FieldType.Select, options: periods)
            };

            var fields = (searchModel.TraderId > 0)
                ? FieldConfig.CreateFields(new string[] { "col-12 md:col-3", "col-12 md:col-3" }, right1, right2)
                : FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-3", "col-12 md:col-3" }, left, right1, right2);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return searchModel;
        }

        public virtual async Task<AggregateAnalysisTableModel> PrepareAggregateAnalysisTableModelAsync(AggregateAnalysisTableModel tableModel)
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<AggregateAnalysisModel>(1, nameof(AggregateAnalysisModel.Date), ColumnType.Date),
                ColumnConfig.Create<AggregateAnalysisModel>(2, nameof(AggregateAnalysisModel.Sales), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisModel>(3, nameof(AggregateAnalysisModel.SalesExports), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisModel>(4, nameof(AggregateAnalysisModel.SalesReturns), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisModel>(5, nameof(AggregateAnalysisModel.SalesTotal), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisModel>(6, nameof(AggregateAnalysisModel.Orders), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisModel>(7, nameof(AggregateAnalysisModel.Purchases), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisModel>(8, nameof(AggregateAnalysisModel.PurchasesImports), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisModel>(9, nameof(AggregateAnalysisModel.PurchasesReturns), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisModel>(10, nameof(AggregateAnalysisModel.PurchasesTotal), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisModel>(11, nameof(AggregateAnalysisModel.CustomerBalance), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisModel>(12, nameof(AggregateAnalysisModel.SuppliersBalance), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisModel>(13, nameof(AggregateAnalysisModel.WarehouseBalance), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisModel>(14, nameof(AggregateAnalysisModel.Receipts), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisModel>(15, nameof(AggregateAnalysisModel.Payments), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisModel>(16, nameof(AggregateAnalysisModel.Expenses), ColumnType.Decimal, width: 120, style: textAlign)
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.AggregateAnalysisModel.Title"));
            tableModel.CustomProperties.Add("title1", await _localizationService.GetResourceAsync("App.Models.AggregateAnalysisModel.Title1"));
            tableModel.CustomProperties.Add("title2", await _localizationService.GetResourceAsync("App.Models.AggregateAnalysisModel.Title2"));
            tableModel.CustomProperties.Add("title3", await _localizationService.GetResourceAsync("App.Models.AggregateAnalysisModel.Title3"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }

        public virtual async Task<AggregateAnalysisTotalTableModel> PrepareAggregateAnalysisTotalTableModelAsync(AggregateAnalysisTotalTableModel tableModel)
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<AggregateAnalysisTotalModel>(2, nameof(AggregateAnalysisTotalModel.Cash), ColumnType.Decimal, width: 60, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisTotalModel>(3, nameof(AggregateAnalysisTotalModel.Bank), ColumnType.Decimal, width: 60, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisTotalModel>(3, nameof(AggregateAnalysisTotalModel.Term), ColumnType.Decimal, width: 60, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisTotalModel>(3, nameof(AggregateAnalysisTotalModel.Else), ColumnType.Decimal, width: 60, style: textAlign),
                ColumnConfig.Create<AggregateAnalysisTotalModel>(4, nameof(AggregateAnalysisTotalModel.Vat), ColumnType.Decimal, width: 60, style: textAlign)
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.AggregateAnalysisModel.Title1"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }
    }
}