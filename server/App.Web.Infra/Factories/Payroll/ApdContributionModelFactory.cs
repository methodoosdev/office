using App.Core;
using App.Core.Domain.Payroll;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Services;
using App.Services.Helpers;
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
    public partial interface IApdContributionModelFactory
    {
        Task<ApdContributionSearchModel> PrepareApdContributionSearchModelAsync(ApdContributionSearchModel searchModel);
        Task<IList<ApdContributionModel>> PrepareApdContributionListModelAsync(ApdContributionSearchModel searchModel, string connection);
        Task<ApdContributionTableModel> PrepareApdContributionTableModelAsync(ApdContributionTableModel tableModel, ApdContributionSearchModel searchModel);
    }

    public partial class ApdContributionModelFactory : IApdContributionModelFactory
    {
        private readonly IModelFactoryService _modelFactoryService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ITraderService _traderService;
        private readonly ITraderEmployeeMappingService _traderEmployeeMappingService;
        private readonly ITraderKadService _traderKadService;

        public ApdContributionModelFactory(
            IModelFactoryService modelFactoryService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IAppDataProvider dataProvider,
            ITraderService traderService,
            ITraderEmployeeMappingService traderEmployeeMappingService,
            ITraderKadService traderKadService)
        {
            _modelFactoryService = modelFactoryService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _dataProvider = dataProvider;
            _traderService = traderService;
            _traderEmployeeMappingService = traderEmployeeMappingService;
            _traderKadService = traderKadService;
        }

        public async Task<ApdContributionSearchModel> PrepareApdContributionSearchModelAsync(ApdContributionSearchModel searchModel)
        {
            var traders = await _modelFactoryService.GetAllTradersAsync(FieldConfigType.Payroll);

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ApdContributionSearchModel>(nameof(ApdContributionSearchModel.EmployerIds), FieldType.MultiSelectAll, options: traders)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ApdContributionSearchModel>(nameof(ApdContributionSearchModel.Period), FieldType.MonthDate),
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-3" }, left, right);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            searchModel.Period = DateTime.UtcNow.AddMonths(-1);

            return searchModel;
        }

        public async Task<IList<ApdContributionModel>> PrepareApdContributionListModelAsync(ApdContributionSearchModel searchModel, string connection)
        {
            var date = await _dateTimeHelper.ConvertToUserTimeAsync(searchModel.Period);

            var description = "Βρέθηκαν διαφορές στους υπαλλήλους: {0}";

            var apdItems = new List<ApdContributionModel>();

            var traderIds = searchModel.EmployerIds.ToList();

            var employers = await _dataProvider.QueryAsync<EmployerLookupItem>(connection, PayrollQuery.EmployerLookupItem);

            var traders = await _traderService.GetTradersByIdsAsync(traderIds.ToArray());
            traders = traders.Where(x => x.HyperPayrollId > 0).ToList();

            foreach (var trader in traders)
            {
                //Έλεγχος αν η επιχείρηση είναι φραμακείο
                var kads = await _traderKadService.GetAllTraderKadsAsync(trader.Id);
                var isPharmacy = kads.Any(k => k.Code.StartsWith("4773") && k.Activity);

                var companyId = trader.HyperPayrollId;
                var companyName = employers.Where(x => x.CompanyId == companyId).FirstOrDefault()?.FullName() ?? string.Empty;

                var month = date.Month;
                var year = date.Year;

                var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
                var pMonth = new LinqToDB.Data.DataParameter("pMonth", month);
                var pYear = new LinqToDB.Data.DataParameter("pYear", year);

                var list = await _dataProvider.QueryAsync<ApdContributionResult>(connection, ApdContributionQuery.EfkaTeka, pCompanyId, pYear, pMonth);
                //var total = new ApdContributionResult { EfkaEmployer = list.Sum(s => s.EfkaEmployer), Teka = list.Sum(s => s.Teka) };

                var totalEfkaEmployer = list.Sum(s => s.EfkaEmployer);
                var totalTeka = list.Sum(s => s.Teka);
                //var christmasPresent = new ApdContributionResult { EfkaEmployer = list.Where(x => x.Periodos.Equals(13)).Sum(s => s.EfkaEmployer), Teka = list.Where(x => x.Periodos.Equals(13)).Sum(s => s.Teka) };
                //var easterPresent = new ApdContributionResult { EfkaEmployer = list.Where(x => x.Periodos.Equals(14)).Sum(s => s.EfkaEmployer), Teka = list.Where(x => x.Periodos.Equals(14)).Sum(s => s.Teka) };

                //var model = new ApdContributionResult();
                //var lastChristmasPresent = new ApdContributionResult();
                //var easterPresentMay = new ApdContributionResult();

                var lastChristmasPresentEfkaEmployer = 0m;
                var lastChristmasPresentTeka = 0m;
                var easterPresentMayEfkaEmployer = 0m;
                var easterPresentMayTeka = 0m;
                decimal efkaEmployer = 0m;
                decimal teka = 0m;

                if (month.Equals(1))
                {
                    var pLastYear = new LinqToDB.Data.DataParameter("pYear", date.Year - 1);
                    var pLastMonth = new LinqToDB.Data.DataParameter("pMonth", 12);
                    var lastYearTotal = await _dataProvider.QueryAsync<ApdContributionResult>(connection, ApdContributionQuery.EfkaTeka, pCompanyId, pLastYear, pLastMonth);

                    lastChristmasPresentEfkaEmployer = lastYearTotal.Where(x => x.Periodos.Equals(13)).Sum(s => s.EfkaEmployer);
                    lastChristmasPresentTeka = lastYearTotal.Where(x => x.Periodos.Equals(13)).Sum(s => s.Teka);

                    efkaEmployer = totalEfkaEmployer + lastChristmasPresentEfkaEmployer;
                    teka = totalTeka + lastChristmasPresentTeka;
                }
                else if (month.Equals(4))
                {
                    var easterPresentEfkaEmployer = list.Where(x => x.Periodos.Equals(14)).Sum(s => s.EfkaEmployer);
                    var easterPresentTeka = list.Where(x => x.Periodos.Equals(14)).Sum(s => s.Teka);

                    efkaEmployer = totalEfkaEmployer - easterPresentEfkaEmployer;
                    teka = totalTeka - easterPresentTeka;
                }
                else if (month.Equals(5))
                {
                    var lastMonth = new LinqToDB.Data.DataParameter("pMonth", 4);
                    var aprilTotal = await _dataProvider.QueryAsync<ApdContributionResult>(connection, ApdContributionQuery.EfkaTeka, pCompanyId, pYear, lastMonth);

                    easterPresentMayEfkaEmployer = aprilTotal.Where(x => x.Periodos.Equals(14)).Sum(s => s.EfkaEmployer);
                    easterPresentMayTeka = aprilTotal.Where(x => x.Periodos.Equals(14)).Sum(s => s.Teka);

                    efkaEmployer = totalEfkaEmployer + easterPresentMayEfkaEmployer;
                    teka = totalTeka + easterPresentMayTeka;
                }
                else if (month.Equals(12))
                {
                    var christmasPresentTeka = list.Where(x => x.Periodos.Equals(13)).Sum(s => s.Teka);
                    var christmasPresentEfkaEmployer = list.Where(x => x.Periodos.Equals(13)).Sum(s => s.EfkaEmployer);

                    efkaEmployer = totalEfkaEmployer - christmasPresentEfkaEmployer;
                    teka = totalTeka - christmasPresentTeka;
                }

                var workerDiffList = new List<string>();

                if (list.Count > 0)
                {
                    var employees = list.Select(x => new EmployeeHelper()
                    {
                        EmployeeId = x.EmployeeId,
                        FullName = x.FullName
                    }).Distinct().ToList();

                    var currentWorkers = await _dataProvider.QueryAsync<EmployeeHelper>(connection, ApdContributionQuery.Employers, pCompanyId, pYear, pMonth);

                    foreach (var worker in currentWorkers)
                    {
                        if (!employees.Any(x => x.EmployeeId == worker.EmployeeId))
                            workerDiffList.Add(worker.FullName);
                    }

                    foreach (var employee in employees)
                    {
                        if (!currentWorkers.Any(x => x.EmployeeId == employee.EmployeeId))
                            workerDiffList.Add(employee.FullName);
                    }
                }

                if (totalTeka > 0 || totalEfkaEmployer > 0)
                {
                    var employees = await _traderEmployeeMappingService.GetEmployeesByTraderIdAsync(trader?.Id ?? 0);

                    apdItems.Add(new ApdContributionModel
                    {
                        CompanyName = companyName,
                        TotalEfka = efkaEmployer > 0 ? efkaEmployer : totalEfkaEmployer,
                        TotalTeka = isPharmacy ? 0 : teka > 0 ? teka : totalTeka,
                        ChristmasPresentEfka = lastChristmasPresentEfkaEmployer,
                        ChristmasPresentTeka = isPharmacy ? 0 : lastChristmasPresentTeka,
                        EasterPresentEfka = easterPresentMayEfkaEmployer,
                        EasterPresentTeka = isPharmacy ? 0 : easterPresentMayTeka,
                        Notes = workerDiffList.Count > 0 ? string.Format(description, string.Join(", ", workerDiffList.Distinct())) : null,
                        EmployeeName = string.Join(", ", employees.Where(x => x.DepartmentId == 3).Select(s => s.FullName()).ToList())
                    });
                }
            }

            apdItems = apdItems.OrderBy(x => x.CompanyName).ToList();

            return apdItems;
        }

        public async Task<ApdContributionTableModel> PrepareApdContributionTableModelAsync(ApdContributionTableModel tableModel, ApdContributionSearchModel searchModel)
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ApdContributionModel>(1, nameof(ApdContributionModel.CompanyName)),
                ColumnConfig.Create<ApdContributionModel>(8, nameof(ApdContributionModel.EmployeeName)),
                ColumnConfig.Create<ApdContributionModel>(2, nameof(ApdContributionModel.TotalEfka), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<ApdContributionModel>(3, nameof(ApdContributionModel.TotalTeka), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<ApdContributionModel>(4, nameof(ApdContributionModel.ChristmasPresentEfka), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<ApdContributionModel>(5, nameof(ApdContributionModel.ChristmasPresentTeka), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<ApdContributionModel>(6, nameof(ApdContributionModel.EasterPresentEfka), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<ApdContributionModel>(7, nameof(ApdContributionModel.EasterPresentTeka), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<ApdContributionModel>(8, nameof(ApdContributionModel.Notes))
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ApdContributionModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }
    }
}
