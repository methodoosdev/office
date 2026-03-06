using App.Core;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Payroll;
using App.Services.Configuration;
using App.Services.Localization;
using App.Services.Payroll;
using App.Web.Infra.Queries.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Payroll.WorkerLeave
{
    public partial interface IWorkerLeaveFactory
    {
        Task<WorkerLeaveSearchModel> PrepareWorkerLeaveSearchModelAsync(WorkerLeaveSearchModel searchModel);
        Task<WorkerLeaveTableModel> PrepareWorkerLeaveTableModelAsync(WorkerLeaveTableModel workerLeaveTableModel);
        Task<IList<WorkerLeaveModel>> PrepareWorkerLeaveListAsync(WorkerLeaveSearchModel searchModel, Trader trader, string connection);
    }
    public partial class WorkerLeaveFactory : IWorkerLeaveFactory
    {
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IWorkerLeaveDetailService _workerLeaveDetailService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public WorkerLeaveFactory(
            IFieldConfigService fieldConfigService,
            IWorkerLeaveDetailService workerLeaveDetailService,
            IAppDataProvider dataProvider,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _fieldConfigService = fieldConfigService;
            _workerLeaveDetailService = workerLeaveDetailService;
            _dataProvider = dataProvider;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<WorkerLeaveSearchModel> PrepareWorkerLeaveSearchModelAsync(WorkerLeaveSearchModel searchModel)
        {
            var trader = await _workContext.GetCurrentTraderAsync();

            var periodos = DateTime.Now.ToUtcRelative();
            searchModel.To = new DateTime(periodos.Year, periodos.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            searchModel.TraderId = trader?.Id ?? 0;

            var left = new List<Dictionary<string, object>>() 
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<WorkerLeaveSearchModel>(nameof(WorkerLeaveSearchModel.TraderId), FieldConfigType.Payroll) 
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerLeaveSearchModel>(nameof(WorkerLeaveSearchModel.To), FieldType.Date)
            };

            var fields = trader != null
                ? FieldConfig.CreateFields(new string[] { "col-12 md:col-3" }, right)
                : FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-3" }, left, right);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return searchModel;
        }

        public virtual async Task<IList<WorkerLeaveModel>> PrepareWorkerLeaveListAsync(WorkerLeaveSearchModel searchModel, Trader trader, string connection)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", trader.HyperPayrollId);
            var pYear = new LinqToDB.Data.DataParameter("pYear", searchModel.To.Year);
            var pTo = new LinqToDB.Data.DataParameter("pTo", searchModel.To.ToString("yyyyMMdd"));

            var workerLeaveDetails = await _workerLeaveDetailService.GetAllWorkerLeaveDetailsAsync(trader.Id);
            var list = await _dataProvider.QueryAsync<WorkerLeaveModel>(connection, WorkerLeaveQuery.LeaveDays, pCompanyId, pTo, pYear);

            foreach (var workerLeaveDetail in workerLeaveDetails)
            {
                //var item = list.FirstOrDefault(x => x.WorkerId == workerLeaveDetail.WorkerId);
                //if (item != null)
                //{
                //    item.DaysLeft = workerLeaveDetail.DaysLeft;
                //}

                // Gets only the days left from last year
                if (workerLeaveDetail.Year == searchModel.To.Year - 1)
                {
                    var item = list.FirstOrDefault(x => x.WorkerId == workerLeaveDetail.WorkerId);
                    if (item != null)
                    {
                        item.DaysLeft = workerLeaveDetail.DaysLeft;
                    }
                }
            }

            foreach (var worker in list)
            {
                worker.TotalDaysLeft = worker.Deserved + worker.DaysLeft - worker.DaysTaken;
            }

            return list;
        }

        public virtual async Task<WorkerLeaveTableModel> PrepareWorkerLeaveTableModelAsync(WorkerLeaveTableModel tableModel)
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<WorkerLeaveModel>(1, nameof(WorkerLeaveModel.LastName)),
                ColumnConfig.Create<WorkerLeaveModel>(2, nameof(WorkerLeaveModel.FirstName)),
                ColumnConfig.Create<WorkerLeaveModel>(3, nameof(WorkerLeaveModel.Vat)),
                ColumnConfig.Create<WorkerLeaveModel>(4, nameof(WorkerLeaveModel.HireDate), ColumnType.Date, style : centerAlign),
                ColumnConfig.Create<WorkerLeaveModel>(5, nameof(WorkerLeaveModel.Deserved), style : rightAlign),
                ColumnConfig.Create<WorkerLeaveModel>(7, nameof(WorkerLeaveModel.DaysLeft), style : rightAlign),
                ColumnConfig.Create<WorkerLeaveModel>(6, nameof(WorkerLeaveModel.DaysTaken), style : rightAlign),
                ColumnConfig.Create<WorkerLeaveModel>(6, nameof(WorkerLeaveModel.TotalDaysLeft), style : rightAlign),
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.WorkerLeaveModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }
    }
}
