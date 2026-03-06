using App.Core;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Payroll;
using App.Services.Configuration;
using App.Services.Localization;
using App.Web.Infra.Queries.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Payroll.WorkerLeave
{
    public partial interface IWorkerSickLeaveFactory
    {
        Task<WorkerSickLeaveSearchModel> PrepareWorkerSickLeaveSearchModelAsync(WorkerSickLeaveSearchModel searchModel);
        Task<WorkerSickLeaveTableModel> PrepareWorkerSickLeaveTableModelAsync(WorkerSickLeaveTableModel workerSickLeaveTableModel);
        Task<IList<WorkerSickLeaveModel>> PrepareWorkerSickLeaveListAsync(WorkerSickLeaveSearchModel searchModel, Trader trader, string connection);
    }
    public partial class WorkerSickLeaveFactory : IWorkerSickLeaveFactory
    {
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public WorkerSickLeaveFactory(
            IFieldConfigService fieldConfigService,
            IAppDataProvider dataProvider,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _fieldConfigService = fieldConfigService;
            _dataProvider = dataProvider;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<WorkerSickLeaveSearchModel> PrepareWorkerSickLeaveSearchModelAsync(WorkerSickLeaveSearchModel searchModel)
        {
            var trader = await _workContext.GetCurrentTraderAsync();

            var periodos = DateTime.Now.ToUtcRelative();
            searchModel.To = new DateTime(periodos.Year, periodos.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            searchModel.TraderId = trader?.Id ?? 0;

            var left = new List<Dictionary<string, object>>() 
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<WorkerSickLeaveSearchModel>(nameof(WorkerSickLeaveSearchModel.TraderId), FieldConfigType.Payroll) 
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerSickLeaveSearchModel>(nameof(WorkerSickLeaveSearchModel.To), FieldType.Date)
            };

            var fields = trader != null
                ? FieldConfig.CreateFields(new string[] { "col-12 md:col-3" }, right)
                : FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-3" }, left, right);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return searchModel;
        }

        public virtual async Task<IList<WorkerSickLeaveModel>> PrepareWorkerSickLeaveListAsync(WorkerSickLeaveSearchModel searchModel, Trader trader, string connection)
        {

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", trader.HyperPayrollId);
            var pTo = new LinqToDB.Data.DataParameter("pTo", searchModel.To.ToString("yyyyMMdd"));

            var list = await _dataProvider.QueryAsync<WorkerSickLeaveModel>(connection, WorkerLeaveQuery.SickDays, pCompanyId, pTo);

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
                list = list.Where(c =>
                    c.LastName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.FirstName.ContainsIgnoreCase(searchModel.QuickSearch)
                ).ToList();

            return list;
        }

        public virtual async Task<WorkerSickLeaveTableModel> PrepareWorkerSickLeaveTableModelAsync(WorkerSickLeaveTableModel tableModel)
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<WorkerSickLeaveModel>(1, nameof(WorkerSickLeaveModel.LastName)),
                ColumnConfig.Create<WorkerSickLeaveModel>(2, nameof(WorkerSickLeaveModel.FirstName)),
                ColumnConfig.Create<WorkerSickLeaveModel>(3, nameof(WorkerSickLeaveModel.Vat)),
                ColumnConfig.Create<WorkerSickLeaveModel>(4, nameof(WorkerSickLeaveModel.HireDate), ColumnType.Date, style : centerAlign),
                ColumnConfig.Create<WorkerSickLeaveModel>(5, nameof(WorkerSickLeaveModel.Deserved), style : rightAlign),
                ColumnConfig.Create<WorkerSickLeaveModel>(6, nameof(WorkerSickLeaveModel.DaysTaken), style : rightAlign),
                ColumnConfig.Create<WorkerSickLeaveModel>(7, nameof(WorkerSickLeaveModel.DaysLeft), style : rightAlign)
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.WorkerSickLeaveModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }
    }
}
