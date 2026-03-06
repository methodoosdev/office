using App.Core;
using App.Core.Domain.Common;
using App.Core.Domain.Payroll;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models;
using App.Framework.Models.Extensions;
using App.Models.Common;
using App.Models.Payroll;
using App.Models.Traders;
using App.Services;
using App.Services.Customers;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Offices;
using App.Services.Payroll;
using App.Services.Traders;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Payroll.WorkerSchedules
{
    public partial interface IWorkerScheduleLogModelFactory
    {
        Task<WorkerScheduleLogSearchModel> PrepareWorkerScheduleLogSearchModelAsync(WorkerScheduleLogSearchModel searchModel);
        Task<WorkerScheduleLogListModel> PrepareWorkerScheduleLogListModelAsync(WorkerScheduleLogSearchModel searchModel);
        Task<WorkerScheduleLogModel> PrepareWorkerScheduleLogModelAsync(WorkerScheduleLogModel model, WorkerScheduleLog workerScheduleLog);
        Task<WorkerScheduleLogFormModel> PrepareWorkerScheduleLogFormModelAsync(WorkerScheduleLogFormModel formModel);
        Task<WorkerScheduleLogFilterFormModel> PrepareWorkerScheduleLogFilterFormModelAsync(WorkerScheduleLogFilterFormModel filterFormModel);
    }
    public partial class WorkerScheduleLogModelFactory : IWorkerScheduleLogModelFactory
    {
        private readonly ITraderService _traderService;
        private readonly IWorkerScheduleLogService _workerScheduleLogService;
        private readonly IWorkerScheduleService _workerScheduleService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly IPersistStateService _persistStateService;
        private readonly IWorkContext _workContext;

        public WorkerScheduleLogModelFactory(ITraderService traderService,
            IWorkerScheduleLogService workerScheduleLogService,
            IWorkerScheduleService workerScheduleService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IModelFactoryService modelFactoryService,
            IPersistStateService persistStateService,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _workerScheduleLogService = workerScheduleLogService;
            _workerScheduleService = workerScheduleService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _modelFactoryService = modelFactoryService;
            _persistStateService = persistStateService;
            _workContext = workContext;
        }

        private async Task<IPagedList<WorkerScheduleLogModel>> GetPagedListAsync(WorkerScheduleLogSearchModel searchModel, WorkerScheduleLogFilterModel filterModel, bool filterExist)
        {
            var scheduleModeTypes = await WorkerScheduleModeType.Waiting.ToSelectionItemListAsync();

            var query = _workerScheduleLogService.Table
                .SelectAwait(async x =>
                {
                    var model = x.ToModel<WorkerScheduleLogModel>();

                    var _trader = await _traderService.GetTraderByIdAsync(x.TraderId);
                    var trader = _trader.ToTraderModel();

                    var workerScheduleModeTypeId = (await _workerScheduleService.Table.FirstAsync(a => a.Id == x.WorkerScheduleId)).WorkerScheduleModeTypeId;

                    model.TraderName = trader.FullName();
                    model.SubmitDateValue = await _dateTimeHelper.ConvertToUserTimeAsync(x.SubmitDate, DateTimeKind.Utc);
                    model.WorkerScheduleModeTypeId = workerScheduleModeTypeId;
                    model.WorkerScheduleModeTypeName = scheduleModeTypes.FirstOrDefault(a => a.Value == workerScheduleModeTypeId).Label;

                    return model;
                });

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Period.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader != null)
                query = query.Where(x => x.TraderId == trader.Id);

            if (filterExist)
            {
                if (filterModel.TraderId > 0)
                    query = query.Where(x => x.TraderId == filterModel.TraderId);

                if (filterModel.WorkerScheduleModeTypeId > 0)
                    query = query.Where(x => x.WorkerScheduleModeTypeId == filterModel.WorkerScheduleModeTypeId);

                //get parameters to filter log
                var periodFromValue = filterModel.PeriodFrom.HasValue
                    ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(filterModel.PeriodFrom.Value) : null;
                var periodToValue = filterModel.PeriodTo.HasValue
                    ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(filterModel.PeriodTo.Value).AddDays(1) : null;

                if (periodFromValue.HasValue && periodToValue.HasValue && periodToValue.Value >= periodFromValue)
                    query = query.Where(x => x.SubmitDate >= periodFromValue.Value && x.SubmitDate <= periodToValue.Value);
            }

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<WorkerScheduleLogSearchModel> PrepareWorkerScheduleLogSearchModelAsync(WorkerScheduleLogSearchModel searchModel)
        {
            var persistState = await _persistStateService.GetModelInstance<WorkerScheduleLogSearchModel>();

            if (persistState.Exist)
                return persistState.Model;

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.WorkerScheduleLogModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<WorkerScheduleLogListModel> PrepareWorkerScheduleLogListModelAsync(WorkerScheduleLogSearchModel searchModel)
        {
            var filterState = await _persistStateService.GetModelInstance<WorkerScheduleLogFilterModel>();

            //get customer roles
            var workerScheduleLogs = await GetPagedListAsync(searchModel, filterState.Model, filterState.Exist);

            //prepare grid model
            var model = new WorkerScheduleLogListModel().PrepareToGrid(searchModel, workerScheduleLogs);
            model.FilterExist = filterState.Exist;

            return model;
        }

        public virtual async Task<WorkerScheduleLogModel> PrepareWorkerScheduleLogModelAsync(WorkerScheduleLogModel model, WorkerScheduleLog workerScheduleLog)
        {
            if (workerScheduleLog != null)
            {
                //fill in model values from the entity
                model ??= workerScheduleLog.ToModel<WorkerScheduleLogModel>();
                model.SubmitDateValue = await _dateTimeHelper.ConvertToUserTimeAsync(workerScheduleLog.SubmitDate, DateTimeKind.Utc);

                var trader = await _traderService.GetTraderByIdAsync(workerScheduleLog.TraderId);

                model.TraderName = trader.ToTraderFullName() ?? "";
            }

            return model;
        }

        private async Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<WorkerScheduleLogModel>(0, nameof(WorkerScheduleLogModel.WorkerScheduleId), width: 55, style: textAlign),
                ColumnConfig.Create<WorkerScheduleLogModel>(1, nameof(WorkerScheduleLogModel.Period), ColumnType.RouterLink, width: 220),
                ColumnConfig.Create<WorkerScheduleLogModel>(2, nameof(WorkerScheduleLogModel.SubmitDateValue), ColumnType.DateTime, width: 200),
                ColumnConfig.Create<WorkerScheduleLogModel>(3, nameof(WorkerScheduleLogModel.WorkerScheduleModeTypeName)),
                ColumnConfig.Create<WorkerScheduleLogModel>(4, nameof(WorkerScheduleLogModel.Notes))
            };

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null)
                columns.Add(ColumnConfig.Create<WorkerScheduleLogModel>(0, nameof(WorkerScheduleLogModel.TraderName), width: 220));

            return columns.OrderBy(x => x.Order).ToList();
        }

        public virtual async Task<WorkerScheduleLogFormModel> PrepareWorkerScheduleLogFormModelAsync(WorkerScheduleLogFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerScheduleLogModel>(nameof(WorkerScheduleLogModel.Period), FieldType.Text, _readonly: true),
                FieldConfig.Create<WorkerScheduleLogModel>(nameof(WorkerScheduleLogModel.SubmitDateValue), FieldType.DateTime, _readonly: true),
                FieldConfig.Create<WorkerScheduleLogModel>(nameof(WorkerScheduleLogModel.Notes), FieldType.Textarea)
            };

            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null)
            {
                var traderNameField = FieldConfig.Create<WorkerScheduleLogModel>(nameof(WorkerScheduleLogModel.TraderName), FieldType.Text, _readonly: true);
                fields.Insert(0, traderNameField);
            }

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.WorkerScheduleLogModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }

        public virtual async Task<WorkerScheduleLogFilterFormModel> PrepareWorkerScheduleLogFilterFormModelAsync(WorkerScheduleLogFilterFormModel filterFormModel)
        {
            var employers = await _modelFactoryService.GetAllActiveEmployersAsync();
            var scheduleModeTypes = await WorkerScheduleModeType.Waiting.ToSelectionItemListAsync();

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerScheduleLogFilterModel>(nameof(WorkerScheduleLogFilterModel.TraderId), FieldType.Select, options: employers),
                FieldConfig.Create<WorkerScheduleLogFilterModel>(nameof(WorkerScheduleLogFilterModel.WorkerScheduleModeTypeId), FieldType.Select, options: scheduleModeTypes)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerScheduleLogFilterModel>(nameof(WorkerScheduleLogFilterModel.PeriodFrom), FieldType.Date),
                FieldConfig.Create<WorkerScheduleLogFilterModel>(nameof(WorkerScheduleLogFilterModel.PeriodTo), FieldType.Date)
            };

            var saveState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerScheduleLogFilterModel>("SaveState", FieldType.Button, themeColor: "primary",
                label: await _localizationService.GetResourceAsync("App.Common.SaveState"))
            };

            var removeState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerScheduleLogFilterModel>("RemoveState", FieldType.Button, themeColor: "warning",
                label: await _localizationService.GetResourceAsync("App.Common.RemoveState"))
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6", "col-12 md:col-2", "col-12 md:col-2" }, left, right, saveState, removeState);

            filterFormModel.CustomProperties.Add("fields", fields);

            return filterFormModel;
        }

    }
}