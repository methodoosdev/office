using App.Core;
using App.Core.Domain.Employees;
using App.Core.Domain.Payroll;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Services;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Common.Models.Payroll;
using App.Web.Infra.Queries.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Payroll
{
    public partial interface IFmyContributionModelFactory
    {
        Task<FmyContributionSearchModel> PrepareFmyContributionSearchModelAsync(FmyContributionSearchModel searchModel);
        Task<IList<FmyContributionModel>> PrepareFmyContributionListModelAsync(FmyContributionSearchModel searchModel, string connection);
        Task<FmyContributionTableModel> PrepareFmyContributionTableModelAsync(FmyContributionTableModel tableModel, FmyContributionSearchModel searchModel);
    }

    public partial class FmyContributionModelFactory : IFmyContributionModelFactory
    {
        private readonly IModelFactoryService _modelFactoryService;
        private readonly ILocalizationService _localizationService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ITraderService _traderService;
        private readonly ITraderEmployeeMappingService _traderEmployeeMappingService;

        public FmyContributionModelFactory(
            IModelFactoryService modelFactoryService,
            ILocalizationService localizationService,
            IAppDataProvider dataProvider,
            ITraderService traderService,
            ITraderEmployeeMappingService traderEmployeeMappingService)
        {
            _modelFactoryService = modelFactoryService;
            _localizationService = localizationService;
            _dataProvider = dataProvider;
            _traderService = traderService;
            _traderEmployeeMappingService = traderEmployeeMappingService;   
        }

        public async Task<FmyContributionSearchModel> PrepareFmyContributionSearchModelAsync(FmyContributionSearchModel searchModel)
        {
            var traders = await _modelFactoryService.GetAllTradersAsync(FieldConfigType.Payroll);

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<FmyContributionSearchModel>(nameof(FmyContributionSearchModel.EmployerIds), FieldType.MultiSelectAll, options: traders)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<FmyContributionSearchModel>(nameof(FmyContributionSearchModel.Period), FieldType.MonthDate),
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-6 md:col-6", "col-12 md:col-3" }, left, right);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            searchModel.Period = DateTime.UtcNow.AddMonths(-1);

            return searchModel;
        }

        public async Task<IList<FmyContributionModel>> PrepareFmyContributionListModelAsync(FmyContributionSearchModel searchModel, string connection)
        {
            var fmyItems = new List<FmyContributionModel>();

            var traderIds = searchModel.EmployerIds.ToList();

            var employers = await _dataProvider.QueryAsync<EmployerLookupItem>(connection, PayrollQuery.EmployerLookupItem);

            var traders = await _traderService.GetTradersByIdsAsync(traderIds.ToArray());
            traders = traders.Where(x => x.HyperPayrollId > 0).ToList();

            foreach (var trader in traders)
            {
                var companyId = trader.HyperPayrollId;

                var companyName = employers.Where(x => x.CompanyId == companyId).FirstOrDefault()?.FullName() ?? string.Empty;

                var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
                var pMonth = new LinqToDB.Data.DataParameter("pMonth", searchModel.Period.Month);
                var pYear = new LinqToDB.Data.DataParameter("pYear", searchModel.Period.Year);

                var list = await _dataProvider.QueryAsync<FmyContributionResult>(connection, FmyContributionQuery.Fmy, pCompanyId, pYear, pMonth);
                var total = new FmyContributionResult { TotalFmy = list.Sum(s => s.TotalFmy) };

                var christmasPresent = new FmyContributionResult { TotalFmy = list.Where(x => x.Periodos.Equals(13)).Sum(s => s.TotalFmy) };

                var easterPresent = new FmyContributionResult { TotalFmy = list.Where(x => x.Periodos.Equals(14)).Sum(s => s.TotalFmy) };

                var model = new FmyContributionResult();
                var lastChristmasPresent = new FmyContributionResult();
                var easterPresentMay = new FmyContributionResult();

                var employees = await _traderEmployeeMappingService.GetEmployeesByTraderIdAsync(trader?.Id ?? 0);

                fmyItems.Add(new FmyContributionModel
                {
                    CompanyName = companyName,
                    TotalFmy = total.TotalFmy,
                    ChristmasPresentFmy = christmasPresent.TotalFmy,
                    EasterPresentFmy = easterPresent.TotalFmy,
                    EmployeeName = string.Join(", ", employees.Where(x => x.DepartmentId == 3).Select(s => s.FullName()).ToList())
                });
            }

            fmyItems = fmyItems.OrderBy(x => x.CompanyName).ToList();

            return fmyItems;
        }

        public async Task<FmyContributionTableModel> PrepareFmyContributionTableModelAsync(FmyContributionTableModel tableModel, FmyContributionSearchModel searchModel)
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<FmyContributionModel>(1, nameof(FmyContributionModel.CompanyName)),
                ColumnConfig.Create<FmyContributionModel>(2, nameof(FmyContributionModel.EmployeeName)),
                ColumnConfig.Create<FmyContributionModel>(3, nameof(FmyContributionModel.TotalFmy), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<FmyContributionModel>(4, nameof(FmyContributionModel.ChristmasPresentFmy), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<FmyContributionModel>(5, nameof(FmyContributionModel.EasterPresentFmy), ColumnType.Decimal, style: rightAlign)
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.FmyContributionModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }

    }
}
