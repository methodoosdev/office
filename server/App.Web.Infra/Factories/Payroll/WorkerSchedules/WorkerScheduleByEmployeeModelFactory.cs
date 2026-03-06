using App.Core;
using App.Core.Domain.Common;
using App.Core.Domain.Payroll;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Common;
using App.Models.Payroll;
using App.Models.Traders;
using App.Services;
using App.Services.Employees;
using App.Services.Localization;
using App.Services.Offices;
using App.Services.Payroll;
using App.Services.Traders;
using App.Web.Infra.Queries.Payroll;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Payroll.WorkerSchedules
{
    public partial interface IWorkerScheduleByEmployeeModelFactory
    {
        Task<WorkerScheduleSearchModel> PrepareWorkerScheduleSearchModelAsync(WorkerScheduleSearchModel searchModel);
        Task<WorkerScheduleListModel> PrepareWorkerScheduleListModelAsync(WorkerScheduleSearchModel searchModel);
        Task<WorkerScheduleModel> PrepareWorkerScheduleModelAsync(WorkerScheduleModel model, WorkerSchedule workerSchedule, string connection, int traderId);
        Task<WorkerScheduleFormModel> PrepareWorkerScheduleFormModelAsync(WorkerScheduleFormModel formModel);
        Task<WorkerScheduleFilterFormModel> PrepareWorkerScheduleFilterFormModelAsync(WorkerScheduleFilterFormModel workerScheduleFilterFormModel);
    }
    public partial class WorkerScheduleByEmployeeModelFactory : IWorkerScheduleByEmployeeModelFactory
    {
        private readonly ITraderService _traderService;
        private readonly IEmployeeService _employeeService;
        private readonly IWorkerScheduleService _workerScheduleService;
        private readonly IWorkerScheduleWorkerService _workerScheduleWorkerService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly IAppDataProvider _dataProvider;
        private readonly IPersistStateService _persistStateService;
        private readonly ILocalizationService _localizationService;

        public WorkerScheduleByEmployeeModelFactory(
            ITraderService traderService,
            IEmployeeService employeeService,
            IWorkerScheduleService workerScheduleService,
            IWorkerScheduleWorkerService workerScheduleWorkerService,
            IModelFactoryService modelFactoryService,
            IAppDataProvider dataProvider,
            IPersistStateService persistStateService,
            ILocalizationService localizationService)
        {
            _traderService = traderService;
            _employeeService = employeeService;
            _workerScheduleService = workerScheduleService;
            _workerScheduleWorkerService = workerScheduleWorkerService;
            _modelFactoryService = modelFactoryService;
            _dataProvider = dataProvider;
            _persistStateService = persistStateService;
            _localizationService = localizationService;
        }

        private async Task<IPagedList<WorkerScheduleModel>> GetPagedListAsync(WorkerScheduleSearchModel searchModel, WorkerScheduleFilterModel filterModel, bool filterExist)
        {
            var scheduleTypes = await WorkerScheduleType.Weekly.ToSelectionItemListAsync();
            var scheduleModeTypes = await WorkerScheduleModeType.Waiting.ToSelectionItemListAsync();

            var query = _workerScheduleService.Table
                .SelectAwait(async x =>
                {
                    var trader = await _traderService.GetTraderByIdAsync(x.TraderId);
                    var employee = await _employeeService.GetEmployeeByIdAsync(x.EmployeeId);

                    var model = x.ToModel<WorkerScheduleModel>();

                    model.Description = $"{model.PeriodFromDate.ToString("dd/MM/yyyy")} - {model.PeriodToDate.ToString("dd/MM/yyyy")}";
                    model.TraderName = trader.ToTraderFullName() ?? "";
                    model.EmployeeName = employee?.FullName() ?? "";
                    model.WorkerScheduleTypeName = scheduleTypes.FirstOrDefault(a => a.Value == x.WorkerScheduleTypeId)?.Label ?? "";
                    model.WorkerScheduleModeTypeName = scheduleModeTypes.FirstOrDefault(a => a.Value == x.WorkerScheduleModeTypeId)?.Label ?? "";

                    return model;
                });

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.Description.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.TraderName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.EmployeeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.WorkerScheduleTypeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.WorkerScheduleModeTypeName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            if (filterExist)
            {
                if (filterModel.TraderId > 0)
                    query = query.Where(c => c.TraderId == filterModel.TraderId);

                if (filterModel.WorkerScheduleModeTypeId.Count > 0)
                {
                    var scheduleModeType = (WorkerScheduleModeType)filterModel.WorkerScheduleModeTypeId.Sum(x => x);
                    query = query.Where(x => scheduleModeType.HasFlag((WorkerScheduleModeType)x.WorkerScheduleModeTypeId));
                }
            }

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<WorkerScheduleSearchModel> PrepareWorkerScheduleSearchModelAsync(WorkerScheduleSearchModel searchModel)
        {
            var persistState = await _persistStateService.GetModelInstance<WorkerScheduleSearchModel>();

            if (persistState.Exist)
                return persistState.Model;

            //prepare page parameters
            searchModel.SetGridPageSize(); searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.WorkerScheduleModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<WorkerScheduleListModel> PrepareWorkerScheduleListModelAsync(WorkerScheduleSearchModel searchModel)
        {
            var filterState = await _persistStateService.GetModelInstance<WorkerScheduleFilterModel>();

            //get customer roles
            var workerSchedules = await GetPagedListAsync(searchModel, filterState.Model, filterState.Exist);

            //prepare grid model
            var model = new WorkerScheduleListModel().PrepareToGrid(searchModel, workerSchedules);
            model.FilterExist = filterState.Exist;

            return model;
        }

        public virtual async Task<WorkerScheduleModel> PrepareWorkerScheduleModelAsync(WorkerScheduleModel model, WorkerSchedule workerSchedule, string connection, int traderId)
        {
            if (workerSchedule != null)
            {
                //fill in model values from the entity
                model ??= workerSchedule.ToModel<WorkerScheduleModel>();

                var workers = await _workerScheduleWorkerService.GetAllWorkerScheduleWorkersAsync(workerSchedule.Id);
                var scheduleModeTypes = await WorkerScheduleModeType.Waiting.ToSelectionItemListAsync();

                model.Workers = workers.Select(x => x.WorkerId).ToList();
                model.WorkerCardNames = string.Join(", ", workers.Select(x => x.WorkerCardName).ToArray());
                model.WorkerScheduleModeTypeName = scheduleModeTypes.FirstOrDefault(a => a.Value == model.WorkerScheduleModeTypeId)?.Label ?? "";
            }

            if (workerSchedule == null)
            {
                var date = DateTime.UtcNow;
                var trader = await _traderService.GetTraderByIdAsync(traderId);

                model.TraderId = traderId;
                model.PeriodFromDate = date.Date;
                model.PeriodToDate = date.Date;
                model.DeliveryDate = date;
                model.SubmitDate = date;
                model.WorkerScheduleTypeId = (int)WorkerScheduleType.Weekly;
                model.WorkerScheduleModeTypeId = (int)WorkerScheduleModeType.Waiting;
                model.Workers = (await GetWorkersAsync(connection, trader.HyperPayrollId)).Select(x => x.Value).ToList();
            }

            return model;
        }

        private async Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var wait = await _localizationService.GetResourceAsync("App.Common.Wait");
            var submit = await _localizationService.GetResourceAsync("App.Common.Submit");
            var cancel = await _localizationService.GetResourceAsync("App.Common.Cancel");
            var schedulesSubmit = await _localizationService.GetResourceAsync("App.Common.SchedulesSubmit");
            var schedulesCheck = await _localizationService.GetResourceAsync("App.Common.SchedulesCheck");
            var excel = await _localizationService.GetResourceAsync("App.Common.Excel");
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var centerHeader = new Dictionary<string, string> { ["justify-content"] = "center" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<WorkerScheduleModel>(0, nameof(WorkerScheduleModel.Id), width: 70, style: centerAlign, headerStyle: centerHeader),
                ColumnConfig.Create<WorkerScheduleModel>(1, nameof(WorkerScheduleModel.TraderName)),
                ColumnConfig.Create<WorkerScheduleModel>(2, nameof(WorkerScheduleModel.Description), ColumnType.RouterLink),
                ColumnConfig.Create<WorkerScheduleModel>(3, nameof(WorkerScheduleModel.PeriodFromDate), ColumnType.Date, hidden: true),
                ColumnConfig.Create<WorkerScheduleModel>(4, nameof(WorkerScheduleModel.PeriodToDate), ColumnType.Date, hidden: true),
                ColumnConfig.Create<WorkerScheduleModel>(5, nameof(WorkerScheduleModel.DeliveryDate), ColumnType.Date),
                ColumnConfig.Create<WorkerScheduleModel>(6, nameof(WorkerScheduleModel.SubmitDate), ColumnType.Date),
                ColumnConfig.Create<WorkerScheduleModel>(7, nameof(WorkerScheduleModel.Protocol), hidden: true),
                ColumnConfig.Create<WorkerScheduleModel>(8, nameof(WorkerScheduleModel.WorkerScheduleTypeName)),
                ColumnConfig.Create<WorkerScheduleModel>(9, nameof(WorkerScheduleModel.WorkerScheduleModeTypeName)),
                ColumnConfig.Create<WorkerScheduleModel>(10, nameof(WorkerScheduleModel.EmployeeName), hidden: true),
                ColumnConfig.CreateButton<WorkerScheduleModel>(11, ColumnType.RowButton, "waiting", "tertiary",
                    wait, centerAlign, centerAlign, hidden: true),
                ColumnConfig.CreateButton<WorkerScheduleModel>(12, ColumnType.RowButton, "submit", "warning",
                    submit, centerAlign, centerAlign),
                ColumnConfig.CreateButton<WorkerScheduleModel>(13, ColumnType.RowButton, "cancel", "error",
                    cancel, centerAlign, centerAlign, hidden: true),
                ColumnConfig.CreateButton<WorkerScheduleModel>(14, ColumnType.RowButton, "schedule", "info",
                    schedulesSubmit, centerAlign, centerAlign),
                ColumnConfig.CreateButton<WorkerScheduleModel>(15, ColumnType.RowButton, "check", "success",
                    schedulesCheck, centerAlign, centerAlign),
                ColumnConfig.CreateButton<WorkerScheduleModel>(16, ColumnType.RowButton, "excel", "light",
                    excel, centerAlign, centerAlign)
            };

            return columns;
        }

        public virtual async Task<WorkerScheduleFormModel> PrepareWorkerScheduleFormModelAsync(WorkerScheduleFormModel formModel)
        {
            var employees = await _modelFactoryService.GetAllEmployeesAsync();
            var scheduleTypes = await WorkerScheduleType.Weekly.ToSelectionItemListAsync();

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.WorkerCardNames), FieldType.Textarea, _readonly: true, rows: 5),
                FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.PeriodFromDate), FieldType.Date, _readonly: true),
                FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.PeriodToDate), FieldType.Date, _readonly: true),
                FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.DeliveryDate), FieldType.Date, _readonly: true),
                FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.SubmitDate), FieldType.Date),
                FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.Protocol), FieldType.Text),
                FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.Notes), FieldType.Textarea),
                FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.WorkerScheduleTypeId), FieldType.Select, options: scheduleTypes),
                FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.EmployeeId), FieldType.Select, options: employees),
                FieldConfig.Create<WorkerScheduleModel>(nameof(WorkerScheduleModel.WorkerScheduleModeTypeName), FieldType.Text, _readonly: true)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.WorkerScheduleModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }

        private async Task<IList<SelectionItemList>> GetWorkersAsync(string connection, int companyId)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
            var results = await _dataProvider.QueryAsync<WorkerScheduleQueryResult>(connection, WorkerScheduleQuery.Workers, pCompanyId);
            return results.Select(x => new SelectionItemList { Value = x.WorkerId, Label = x.WorkerCardName }).ToList();
        }

        public async Task<WorkerScheduleFilterFormModel> PrepareWorkerScheduleFilterFormModelAsync(WorkerScheduleFilterFormModel filterFormModel)
        {
            var traders = await _modelFactoryService.GetAllTradersAsync();
            var scheduleModeTypes = await WorkerScheduleModeType.Waiting.ToSelectionItemListAsync();

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerScheduleFilterModel>(nameof(WorkerScheduleFilterModel.TraderId), FieldType.GridSelect, options: traders)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerScheduleFilterModel>(nameof(WorkerScheduleFilterModel.WorkerScheduleModeTypeId), FieldType.MultiSelect, options: scheduleModeTypes)
            };

            var saveState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerScheduleFilterModel>("SaveState", FieldType.Button, themeColor: "primary",
                label: await _localizationService.GetResourceAsync("App.Common.SaveState"))
            };

            var removeState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerScheduleFilterModel>("RemoveState", FieldType.Button, themeColor: "warning",
                label: await _localizationService.GetResourceAsync("App.Common.RemoveState"))
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6", "col-12 md:col-2", "col-12 md:col-2" }, left, right, saveState, removeState);

            filterFormModel.CustomProperties.Add("fields", fields);

            return filterFormModel;
        }
    }
}