using App.Core;
using App.Core.Domain.Payroll;
using App.Core.Domain.Traders;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Accounting;
using App.Services.Common;
using App.Services.Configuration;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Infra.Queries.Common;
using App.Web.Infra.Queries.Payroll;
using DocumentFormat.OpenXml.Bibliography;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Common.Factories
{
    public partial interface ITraderChargeFactory
    {
        Task<TraderChargeSearchModel> PrepareTraderChargeSearchModelAsync(TraderChargeSearchModel searchModel);
        Task<IList<TraderChargeModel>> PrepareTraderChargeListAsync(TraderConnectionResult traderConnection, string payrollConnection);
        Task<TraderChargeTableModel> PrepareTraderChargeTableModelAsync(TraderChargeTableModel tableModel);
    }
    public class TraderChargeFactory : ITraderChargeFactory
    {
        private readonly IFieldConfigService _fieldConfigService;
        private readonly ITraderMonthlyBillingService _traderMonthlyBillingService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public TraderChargeFactory(
            IFieldConfigService fieldConfigService,
            ITraderMonthlyBillingService traderMonthlyBillingService,
            IAppDataProvider provider,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _fieldConfigService = fieldConfigService;
            _traderMonthlyBillingService = traderMonthlyBillingService;
            _dataProvider = provider;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<int> CountEmployees(string connection, int year, int companyId)
        {
            var pYear = new LinqToDB.Data.DataParameter("pYear", year);
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
            var employers = await _dataProvider.QueryAsync<EmployersPerYearQueryResult>(connection, PayrollQuery.EmployersPerYear, pYear, pCompanyId);

            return employers.Count();
        }

        private async Task<int> GetTraderMonthlyBillings(int traderId, int year, int amount)
        {
            var list = await _traderMonthlyBillingService.GetAllTraderMonthlyBillingsAsync(traderId, year);
            var item = list.FirstOrDefault();

            if (item is null)
            {
                item = new TraderMonthlyBilling
                {
                    TraderId = traderId,
                    Year = year,
                    Amount = amount
                };

                await _traderMonthlyBillingService.InsertTraderMonthlyBillingAsync(item);
            }
            else if(item.Amount == 0)
            {
                item.Amount = amount;
            }

                return item.Amount;
        }

        public virtual async Task<IList<TraderChargeModel>> PrepareTraderChargeListAsync(TraderConnectionResult traderConnection, string payrollConnection)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", traderConnection.CompanyId);
            var pSchema = new LinqToDB.Data.DataParameter("pSchema", traderConnection.AccountingSchema);

            var connectionQuery = new TraderChargeQuery(traderConnection.LogistikiProgramTypeId, traderConnection.CategoryBookTypeId).Get();

            var result = await _dataProvider.QueryAsync<TraderChargeQueryResult>(traderConnection.Connection, 
                connectionQuery, pCompanyId, pSchema);

            var list = new List<TraderChargeModel>();
            var years = result.Select(x => x.Year).Distinct().ToList();

            foreach (var year in years)
            {
                var assets = result.Where(x => x.Year == year && x.CodePrefix == 1).Sum(s => s.Total);
                var sales = result.Where(x => x.Year == year && x.CodePrefix == 7).Sum(s => s.Total);
                var expenses = result.Where(x => x.Year == year && x.CodePrefix == 6).Sum(s => s.Total);
                var other = result.Where(x => x.Year == year && x.CodePrefix == 8).Sum(s => s.Total);

                var purchases1 = result.Where(x => x.Year == year && x.Code.IsLike("2?.01*")).Sum(s => s.Total);
                var purchases2 = result.Where(x => x.Year == year && x.Code.IsLike("2?.02*")).Sum(s => s.Total);
                var purchases3 = result.Where(x => x.Year == year && x.Code.IsLike("2?.03*")).Sum(s => s.Total);
                var purchases4 = result.Where(x => x.Year == year && x.Code.IsLike("2?.04*")).Sum(s => s.Total);
                var purchases5 = result.Where(x => x.Year == year && x.Code.IsLike("2?.05*")).Sum(s => s.Total);
                var purchases6 = result.Where(x => x.Year == year && x.Code.IsLike("2?.06*")).Sum(s => s.Total);
                var purchases = purchases1 + purchases2 + purchases3 + purchases4 + purchases5 - purchases6;

                var model = new TraderChargeModel 
                {
                    Year = year,
                    Turnover = sales,
                    BeforeTaxes = sales - purchases - expenses - other,
                    Assets = assets,
                    Personnel = await CountEmployees(payrollConnection, year, traderConnection.HyperPayrollId),
                    Amount = await GetTraderMonthlyBillings(traderConnection.TraderId, year, traderConnection.TraderPayment)
                };
                list.Add(model);
            }

            return list;
        }

        public virtual async Task<TraderChargeSearchModel> PrepareTraderChargeSearchModelAsync(TraderChargeSearchModel searchModel)
        {
            var left = new List<Dictionary<string, object>>() 
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<TraderChargeSearchModel>(nameof(TraderChargeSearchModel.TraderId), FieldConfigType.WithCategoryBooks) 
            };

            var right = new List<Dictionary<string, object>>()
            {
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-3" }, left, right);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return searchModel;
        }

        public virtual async Task<TraderChargeTableModel> PrepareTraderChargeTableModelAsync(TraderChargeTableModel tableModel)
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderChargeModel>(1, nameof(TraderChargeModel.Year)),
                ColumnConfig.Create<TraderChargeModel>(2, nameof(TraderChargeModel.Turnover), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<TraderChargeModel>(3, nameof(TraderChargeModel.BeforeTaxes), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<TraderChargeModel>(3, nameof(TraderChargeModel.Assets), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<TraderChargeModel>(3, nameof(TraderChargeModel.Personnel), ColumnType.Numeric, style: rightAlign),
                ColumnConfig.Create<TraderChargeModel>(3, nameof(TraderChargeModel.Amount), ColumnType.Numeric, style: rightAlign),
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.TraderChargeModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }
    }
}