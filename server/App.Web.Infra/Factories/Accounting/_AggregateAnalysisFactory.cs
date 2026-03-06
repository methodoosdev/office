using App.Core;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Accounting;
using App.Services.Configuration;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Infra.Queries.Accounting;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Accounting.Factories
{
    public partial interface _IAggregateAnalysisFactory
    {
        Task<AggregateAnalysisSearchModel> PrepareAggregateAnalysisSearchModelAsync(AggregateAnalysisSearchModel searchModel);
        Task<IList<AggregateAnalysisModel>> PrepareAggregateAnalysisListAsync(TraderConnectionResult connectionResult, int month, int year);
        Task<IList<AggregateAnalysisModel>> PrepareAggregateAnalysisProgressListAsync(TraderConnectionResult connectionResult, int month, int year);
        Task<AggregateAnalysisTableModel> PrepareAggregateAnalysisTableModelAsync(AggregateAnalysisTableModel tableModel);
        Task<AggregateAnalysisTotalTableModel> PrepareAggregateAnalysisTotalTableModelAsync(AggregateAnalysisTotalTableModel tableModel);
    }
    public partial class _AggregateAnalysisFactory : _IAggregateAnalysisFactory
    {
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        public _AggregateAnalysisFactory(
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

        private async Task<(decimal customers, decimal suplpliers)> GetAggregateAnalysisTotals(TraderConnectionResult connectionResult, DateTime firstDayOfMonth)
        {
            var lastDayOfPeriod = firstDayOfMonth.AddDays(-1);
            var firstDayOfPeriod = new DateTime(lastDayOfPeriod.Year, 1, 1, 0, 0, 0);

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pStartDate = new LinqToDB.Data.DataParameter("pStartDate", firstDayOfPeriod.ToString("yyyy/MM/dd"));
            var pEndDate = new LinqToDB.Data.DataParameter("pEndDate", lastDayOfPeriod.ToString("yyyy/MM/dd"));

            //70, 2*, 30, 50, 62, 63, 64, 65, 67
            var list = await _dataProvider.QueryAsync<AggregateAnalysisItem>(connectionResult.Connection,
                new _AggregateAnalysisQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCompanyId, pStartDate, pEndDate);

            var customerBbalance = list.Where(x => x.Id.IsLike("30*")).Select(x => x.Total).Sum();

            var suppliersBalance = list.Where(x => x.Id.IsLike("50*")).Select(x => x.Total).Sum();

            return (customerBbalance, suppliersBalance);
        }

        public virtual async Task<IList<AggregateAnalysisModel>> PrepareAggregateAnalysisListAsync(TraderConnectionResult connectionResult, int month, int year)
        {
            var firstDayOfMonth = new DateTime(year, month, 1, 0, 0, 0);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pStartDate = new LinqToDB.Data.DataParameter("pStartDate", firstDayOfMonth.ToString("yyyy/MM/dd"));
            var pEndDate = new LinqToDB.Data.DataParameter("pEndDate", lastDayOfMonth.ToString("yyyy/MM/dd"));

            //70, 2*, 30, 50, 62, 63, 64, 65, 67
            var all = await _dataProvider.QueryAsync<AggregateAnalysisItem>(connectionResult.Connection,
                new _AggregateAnalysisQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCompanyId, pStartDate, pEndDate);

            //orders
            var ordersList = await _dataProvider.QueryAsync<AggregateAnalysisItem>(connectionResult.Connection, 
                new _AggregateAnalysisOrdersQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCompanyId, pStartDate, pEndDate);

            //receipts
            var receiptsList = await _dataProvider.QueryAsync<AggregateAnalysisItem>(connectionResult.Connection, 
                new _AggregateAnalysisReceiptsQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCompanyId, pStartDate, pEndDate);

            //payments
            var paymentsList = await _dataProvider.QueryAsync<AggregateAnalysisItem>(connectionResult.Connection, 
                new _AggregateAnalysisPaymentsQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCompanyId, pStartDate, pEndDate);

            var results = all.Concat(ordersList).Concat(receiptsList).Concat(paymentsList).ToList();
            var dates = results.Select(x => x.Date).Distinct().OrderBy(o => o).ToList();

            var totals = await GetAggregateAnalysisTotals(connectionResult, firstDayOfMonth);
            if (month > 1)
            {
                var previeusYeartotals = await GetAggregateAnalysisTotals(connectionResult, new DateTime(year, 1, 1).Date);
                totals.customers += previeusYeartotals.customers;
                totals.suplpliers += previeusYeartotals.suplpliers;
            }

            var modelList = new List<AggregateAnalysisModel>();
            foreach (var date in dates)
            {
                //filter by date
                var list = results.Where(d => d.Date == date).ToList();

                var model = new AggregateAnalysisModel { Date = (await _dateTimeHelper.ConvertToUserTimeAsync(date)).ToString("dd/MM/yyyy") };

                //sales
                var sales = list.Where(x => x.Id.IsLike("70.??.11*")).Select(x => x.Total).Sum();

                //salesExports
                var salesExports = list.Where(x => x.Id.IsLike("70.??.12*") || x.Id.IsLike("70.??.13*")).Select(x => x.Total).Sum();

                //salesReturns
                var salesReturns = list.Where(x => x.Id.IsLike("70.??.2*") || x.Id.IsLike("70.??.3*")).Select(x => x.Total).Sum();

                //salesTotal
                var salesTotal = sales + salesExports + salesReturns;

                //orders
                var orders = list.Where(x => x.Id.Equals("orders")).Select(x => x.Total).Sum();

                //purchases
                var purchases = list.Where(x => x.Id.IsLike("2?.?2.?1*")).Select(x => x.Total).Sum();

                //purchasesImports
                var purchasesImports = list.Where(x => x.Id.IsLike("2?.?2.2*") || x.Id.IsLike("2?.?2.3*")).Select(x => x.Total).Sum();

                //purchasesReturns
                var purchasesReturns = list.Where(x => x.Id.IsLike("2?.?3*") || x.Id.IsLike("2?.?4*")).Select(x => x.Total).Sum();

                //purchasesTotal
                var purchasesTotal = purchases + purchasesImports + purchasesReturns;

                //customerBalance
                var customerBalance = list.Where(x => x.Id.IsLike("30*")).Select(x => x.Total).Sum() + totals.customers;
                totals.customers = customerBalance;

                //suppliersBalance
                var suppliersBalance = list.Where(x => x.Id.IsLike("50*")).Select(x => x.Total).Sum() + totals.suplpliers;
                totals.suplpliers = suppliersBalance;

                //warehouseBalance
                var warehouseBalance = 0m;

                //receipts
                var receipts = list.Where(x => x.Id.Equals("receipts")).Select(x => x.Total).Sum();

                //payments
                var payments = list.Where(x => x.Id.Equals("payments")).Select(x => x.Total).Sum();

                //expenses
                var expenses = list.Where(x => x.Id.IsLike("62*") || x.Id.IsLike("63*") || 
                    x.Id.IsLike("64*") || x.Id.IsLike("65*") || x.Id.IsLike("67*")).Select(x => x.Total).Sum();

                model.Sales = sales;
                model.SalesExports = salesExports;
                model.SalesReturns = salesReturns;
                model.SalesTotal = salesTotal;
                model.Orders = orders;
                model.Purchases = purchases;
                model.PurchasesImports = purchasesImports;
                model.PurchasesReturns = purchasesReturns;
                model.PurchasesTotal = purchasesTotal;
                model.CustomerBalance = customerBalance;
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
                CustomerBalance = modelList.Select(x => x.CustomerBalance).Last(),
                SuppliersBalance = modelList.Select(x => x.SuppliersBalance).Last(),
                WarehouseBalance = modelList.Select(x => x.WarehouseBalance).Sum(),
                Receipts = modelList.Select(x => x.Receipts).Sum(),
                Payments = modelList.Select(x => x.Payments).Sum(),
                Expenses = modelList.Select(x => x.Expenses).Sum()
            };
            modelList.Add(totalModel);

            return modelList;
        }

        public virtual async Task<IList<AggregateAnalysisModel>> PrepareAggregateAnalysisProgressListAsync(TraderConnectionResult connectionResult, int month, int year)
        {
            var firstDayOf = new DateTime(year, 1, 1).Date;
            var lastDayOf = new DateTime(year, month, 1).Date.AddMonths(1).AddDays(-1);

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pStartDate = new LinqToDB.Data.DataParameter("pStartDate", firstDayOf.ToString("yyyy/MM/dd"));
            var pEndDate = new LinqToDB.Data.DataParameter("pEndDate", lastDayOf.ToString("yyyy/MM/dd"));

            //70, 2*, 30, 50, 62, 63, 64, 65, 67
            var all = await _dataProvider.QueryAsync<AggregateAnalysisItem>(connectionResult.Connection,
                new _AggregateAnalysisQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCompanyId, pStartDate, pEndDate);

            //orders
            var ordersList = await _dataProvider.QueryAsync<AggregateAnalysisItem>(connectionResult.Connection,
                new _AggregateAnalysisOrdersQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCompanyId, pStartDate, pEndDate);

            //receipts
            var receiptsList = await _dataProvider.QueryAsync<AggregateAnalysisItem>(connectionResult.Connection,
                new _AggregateAnalysisReceiptsQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCompanyId, pStartDate, pEndDate);

            //payments
            var paymentsList = await _dataProvider.QueryAsync<AggregateAnalysisItem>(connectionResult.Connection,
                new _AggregateAnalysisPaymentsQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCompanyId, pStartDate, pEndDate);

            var results = all.Concat(ordersList).Concat(receiptsList).Concat(paymentsList).ToList();

            var modelList = new List<AggregateAnalysisModel>();

            var from = (await _dateTimeHelper.ConvertToUserTimeAsync(firstDayOf)).ToString("dd/MM/yyyy");
            var to = (await _dateTimeHelper.ConvertToUserTimeAsync(lastDayOf)).ToString("dd/MM/yyyy");

            var totals = await GetAggregateAnalysisTotals(connectionResult, firstDayOf);

            //sales
            var sales = results.Where(x => x.Id.IsLike("70.??.11*")).Select(x => x.Total).Sum();

            //salesExports
            var salesExports = results.Where(x => x.Id.IsLike("70.??.12*") || x.Id.IsLike("70.??.13*")).Select(x => x.Total).Sum();

            //salesReturns
            var salesReturns = results.Where(x => x.Id.IsLike("70.??.2*") || x.Id.IsLike("70.??.3*")).Select(x => x.Total).Sum();

            //salesTotal
            var salesTotal = sales + salesExports + salesReturns;

            //orders
            var orders = results.Where(x => x.Id.Equals("orders")).Select(x => x.Total).Sum();

            //purchases
            var purchases = results.Where(x => x.Id.IsLike("2?.?2.?1*")).Select(x => x.Total).Sum();

            //purchasesImports
            var purchasesImports = results.Where(x => x.Id.IsLike("2?.?2.2*") || x.Id.IsLike("2?.?2.3*")).Select(x => x.Total).Sum();

            //purchasesReturns
            var purchasesReturns = results.Where(x => x.Id.IsLike("2?.?3*") || x.Id.IsLike("2?.?4*")).Select(x => x.Total).Sum();

            //purchasesTotal
            var purchasesTotal = purchases + purchasesImports + purchasesReturns;

            //customerBalance
            var customerBalance = results.Where(x => x.Id.IsLike("30*")).Select(x => x.Total).Sum() + totals.customers;

            //suppliersBalance
            var suppliersBalance = results.Where(x => x.Id.IsLike("50*")).Select(x => x.Total).Sum() + totals.suplpliers;

            //warehouseBalance
            var warehouseBalance = 0m;

            //receipts
            var receipts = results.Where(x => x.Id.Equals("receipts")).Select(x => x.Total).Sum();

            //payments
            var payments = results.Where(x => x.Id.Equals("payments")).Select(x => x.Total).Sum();

            //expenses
            var expenses = results.Where(x => x.Id.IsLike("62*") || x.Id.IsLike("63*") ||
                x.Id.IsLike("64*") || x.Id.IsLike("65*") || x.Id.IsLike("67*")).Select(x => x.Total).Sum();

            var model = new AggregateAnalysisModel { Date = $"{from} - {to}" };
            model.Sales = sales;
            model.SalesExports = salesExports;
            model.SalesReturns = salesReturns;
            model.SalesTotal = salesTotal;
            model.Orders = orders;
            model.Purchases = purchases;
            model.PurchasesImports = purchasesImports;
            model.PurchasesReturns = purchasesReturns;
            model.PurchasesTotal = purchasesTotal;
            model.CustomerBalance = customerBalance;
            model.SuppliersBalance = suppliersBalance;
            model.WarehouseBalance = warehouseBalance;
            model.Receipts = receipts;
            model.Payments = payments;
            model.Expenses = expenses;

            modelList.Add(model);

            return modelList;
        }

        public virtual async Task<AggregateAnalysisSearchModel> PrepareAggregateAnalysisSearchModelAsync(AggregateAnalysisSearchModel searchModel)
        {
            var trader = await _workContext.GetCurrentTraderAsync();

            //searchModel.Periodos = DateTime.Now.ToUtcRelative();
            searchModel.TraderId = trader?.Id ?? 0;

            var left = new List<Dictionary<string, object>>()
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<AggregateAnalysisSearchModel>(nameof(AggregateAnalysisSearchModel.TraderId), FieldConfigType.WithCategoryBookC)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AggregateAnalysisSearchModel>(nameof(AggregateAnalysisSearchModel.Period), FieldType.MonthDate)
            };

            var fields = (trader != null)
                ? FieldConfig.CreateFields(new string[] { "col-12 md:col-3" }, right)
                : FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-3" }, left, right);

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