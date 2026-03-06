using App.Core;
using App.Core.Domain.Payroll;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Payroll;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Payroll.WorkerSchedules
{
    public partial interface IWorkerScheduleShiftByTraderModelFactory
    {
        Task<WorkerScheduleShiftSearchModel> PrepareWorkerScheduleShiftSearchModelAsync(WorkerScheduleShiftSearchModel searchModel);
        Task<WorkerScheduleShiftListModel> PrepareWorkerScheduleShiftListModelAsync(WorkerScheduleShiftSearchModel searchModel);
        Task<WorkerScheduleShiftModel> PrepareWorkerScheduleShiftModelAsync(WorkerScheduleShiftModel model, WorkerScheduleShift workerScheduleShift, int traderId);
        Task<WorkerScheduleShiftFormModel> PrepareWorkerScheduleShiftFormModelAsync(WorkerScheduleShiftFormModel formModel);
        Task<string> PrepareWorkerScheduleShiftModelValidationAsync(WorkerScheduleShiftModel model, int breakLimit);
    }
    public partial class WorkerScheduleShiftByTraderModelFactory : IWorkerScheduleShiftByTraderModelFactory
    {
        private readonly IWorkerScheduleShiftService _WorkerScheduleShiftService;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        public WorkerScheduleShiftByTraderModelFactory(
            IWorkerScheduleShiftService WorkerScheduleShiftService,
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _WorkerScheduleShiftService = WorkerScheduleShiftService;
            _localizationService = localizationService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        private async Task<IPagedList<WorkerScheduleShiftModel>> GetPagedListAsync(WorkerScheduleShiftSearchModel searchModel)
        {
            var trader = await _workContext.GetCurrentTraderAsync();

            var query = _WorkerScheduleShiftService.Table.AsEnumerable()
                .Where(a => a.TraderId == trader.Id)
                .Select(x => x.ToModel<WorkerScheduleShiftModel>()).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<WorkerScheduleShiftSearchModel> PrepareWorkerScheduleShiftSearchModelAsync(WorkerScheduleShiftSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.WorkerScheduleShiftModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<WorkerScheduleShiftListModel> PrepareWorkerScheduleShiftListModelAsync(WorkerScheduleShiftSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var WorkerScheduleShifts = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new WorkerScheduleShiftListModel().PrepareToGrid(searchModel, WorkerScheduleShifts);

            return model;
        }

        public virtual Task<WorkerScheduleShiftModel> PrepareWorkerScheduleShiftModelAsync(WorkerScheduleShiftModel model, WorkerScheduleShift workerScheduleShift, int traderId)
        {
            if (workerScheduleShift != null)
            {
                //fill in model values from the entity
                model ??= workerScheduleShift.ToModel<WorkerScheduleShiftModel>();
            }

            if (workerScheduleShift == null)
            {
                var date = new DateTime(new DateTime(2000, 2, 2, 0, 0, 0, 0).Ticks, DateTimeKind.Utc);

                model.TraderId = traderId;
                model.NonstopFromDate = date;
                model.NonstopToDate = date;
                model.SplitFromDate = date;
                model.SplitToDate = date;
                model.BreakNonstopFromDate = date;
                model.BreakNonstopToDate = date;
                model.BreakNonstop2FromDate = date;
                model.BreakNonstop2ToDate = date;
                model.BreakSplitFromDate = date;
                model.BreakSplitToDate = date;
                model.OvertimeFromDate = date;
                model.OvertimeToDate = date;
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<WorkerScheduleShiftModel>(1, nameof(WorkerScheduleShiftModel.DisplayOrder)),
                ColumnConfig.Create<WorkerScheduleShiftModel>(3, nameof(WorkerScheduleShiftModel.Description), ColumnType.RouterLink),
                ColumnConfig.Create<WorkerScheduleShiftModel>(4, nameof(WorkerScheduleShiftModel.NonstopFromDate), ColumnType.Time),
                ColumnConfig.Create<WorkerScheduleShiftModel>(5, nameof(WorkerScheduleShiftModel.NonstopToDate), ColumnType.Time),
                ColumnConfig.Create<WorkerScheduleShiftModel>(6, nameof(WorkerScheduleShiftModel.SplitFromDate), ColumnType.Time),
                ColumnConfig.Create<WorkerScheduleShiftModel>(7, nameof(WorkerScheduleShiftModel.SplitToDate), ColumnType.Time),
                ColumnConfig.Create<WorkerScheduleShiftModel>(8, nameof(WorkerScheduleShiftModel.BreakNonstopFromDate), ColumnType.Time),
                ColumnConfig.Create<WorkerScheduleShiftModel>(9, nameof(WorkerScheduleShiftModel.BreakNonstopToDate), ColumnType.Time),
                ColumnConfig.Create<WorkerScheduleShiftModel>(8, nameof(WorkerScheduleShiftModel.BreakSplitFromDate), ColumnType.Time),
                ColumnConfig.Create<WorkerScheduleShiftModel>(9, nameof(WorkerScheduleShiftModel.BreakSplitToDate), ColumnType.Time)
            };

            return columns;
        }

        public virtual async Task<WorkerScheduleShiftFormModel> PrepareWorkerScheduleShiftFormModelAsync(WorkerScheduleShiftFormModel formModel)
        {
            var min = new DateTime(new DateTime(2000, 2, 2, 0, 0, 0, 0).Ticks, DateTimeKind.Utc);
            var max = new DateTime(new DateTime(2000, 2, 2, 23, 59, 59, 999).Ticks, DateTimeKind.Utc);

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerScheduleShiftModel>(nameof(WorkerScheduleShiftModel.DisplayOrder), FieldType.Numeric),
                FieldConfig.Create<WorkerScheduleShiftModel>(nameof(WorkerScheduleShiftModel.Description), FieldType.Text, markAsRequired: true),
                FieldConfig.Create<WorkerScheduleShiftModel>(nameof(WorkerScheduleShiftModel.NonstopFromDate), FieldType.Time, minDate: min, maxDate: max),
                FieldConfig.Create<WorkerScheduleShiftModel>(nameof(WorkerScheduleShiftModel.NonstopToDate), FieldType.Time, minDate: min, maxDate: max),
                FieldConfig.Create<WorkerScheduleShiftModel>(nameof(WorkerScheduleShiftModel.BreakNonstopFromDate), FieldType.Time, minDate: min, maxDate: max),
                FieldConfig.Create<WorkerScheduleShiftModel>(nameof(WorkerScheduleShiftModel.BreakNonstopToDate), FieldType.Time, minDate: min, maxDate: max),
                FieldConfig.Create<WorkerScheduleShiftModel>(nameof(WorkerScheduleShiftModel.BreakNonstop2FromDate), FieldType.Time, minDate: min, maxDate: max),
                FieldConfig.Create<WorkerScheduleShiftModel>(nameof(WorkerScheduleShiftModel.BreakNonstop2ToDate), FieldType.Time, minDate: min, maxDate: max),
                FieldConfig.Create<WorkerScheduleShiftModel>(nameof(WorkerScheduleShiftModel.IsSplit), FieldType.Checkbox)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerScheduleShiftModel>(nameof(WorkerScheduleShiftModel.SplitFromDate), FieldType.Time, disableExpression: "!model.isSplit", minDate: min, maxDate: max),
                FieldConfig.Create<WorkerScheduleShiftModel>(nameof(WorkerScheduleShiftModel.SplitToDate), FieldType.Time, disableExpression: "!model.isSplit", minDate: min, maxDate: max),
                FieldConfig.Create<WorkerScheduleShiftModel>(nameof(WorkerScheduleShiftModel.BreakSplitFromDate), FieldType.Time, disableExpression: "!model.isSplit", minDate: min, maxDate: max),
                FieldConfig.Create<WorkerScheduleShiftModel>(nameof(WorkerScheduleShiftModel.BreakSplitToDate), FieldType.Time, disableExpression: "!model.isSplit", minDate: min, maxDate: max),
                //FieldConfig.Create<WorkerScheduleShiftModel>(nameof(WorkerScheduleShiftModel.OvertimeFromDate), FieldType.Time, minDate: min, maxDate: max),
                //FieldConfig.Create<WorkerScheduleShiftModel>(nameof(WorkerScheduleShiftModel.OvertimeToDate), FieldType.Time, minDate: min, maxDate: max)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.WorkerScheduleShiftModel.EditForm.Title"));

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6" }, left, right);
            formModel.CustomProperties.Add("fields", fields);

            return formModel;
        }

        public virtual async Task<string> PrepareWorkerScheduleShiftModelValidationAsync(WorkerScheduleShiftModel model, int breakLimit)
        {
            double getHours(double value)
            {
                return Math.Floor(value);
            }

            double getMinutes(double value)
            {
                var hours = Math.Floor(value);
                return 60 * (value - hours);
            }

            string getFromMinutes(int minutes)
            {
                var time = TimeSpan.FromMinutes(minutes);
                return string.Format("{0:#00}:{1:#00}", (int)time.TotalHours, time.Minutes);
            }

            string getTimeString(double value)
            {
                return string.Format("{0:#00}:{1:#00}", getHours(value), getMinutes(value));
            }

            bool getBreakItemError(double dailyBreak, double dailyTotalHours)
            {
                var totalHours = getHours(dailyTotalHours);
                var totalBreak = getHours(dailyBreak) * 60 + getMinutes(dailyBreak);

                if (totalHours >= 4 && totalBreak < 15)
                {
                    return true;
                }
                return false;
            }

            string error = null;
            var dailyBreakNonstop = (model.BreakNonstopToDate - model.BreakNonstopFromDate).TotalHours;
            var dailyBreakNonstop2 = (model.BreakNonstop2ToDate - model.BreakNonstop2FromDate).TotalHours;
            var dailyBreakSplit = (model.BreakSplitToDate - model.BreakSplitFromDate).TotalHours;
            var dailyNonstop = (model.NonstopToDate - model.NonstopFromDate).TotalHours - dailyBreakNonstop;
            var dailySplit = (model.SplitToDate - model.SplitFromDate).TotalHours - dailyBreakSplit;
            var dailyBreak = dailyBreakNonstop + dailyBreakNonstop2 + dailyBreakSplit;
            var dailyTotalHours = dailyNonstop + dailySplit - dailyBreak;

            var dailyTotalHoursError = getHours(dailyTotalHours) > 8;

            if (dailyTotalHoursError)
            {
                var titleError = await _localizationService.GetResourceAsync("App.Errors.DailyTotalHoursError");
                error = $"{titleError} - {getTimeString(dailyTotalHours)} > {string.Format("{0:#00}:{1:#00}", 8, 0)}";

                return error;
            }

            var totalBreak = getHours(dailyBreak) * 60 + getMinutes(dailyBreak);
            var breakItemError = getBreakItemError(dailyBreak, dailyTotalHours);
            if (breakItemError)
            {

                var titleError = await _localizationService.GetResourceAsync("App.Errors.DailyBreakError");
                error = $"{titleError} - {getTimeString(totalBreak)} < {string.Format("{0:#00}:{1:#00}", 0, 15)}";
            }

            if (totalBreak > breakLimit)
            {
                var titleError = await _localizationService.GetResourceAsync("App.Errors.BreakLimitError");
                var format = await _localizationService.GetResourceAsync("App.Errors.BreakLimitFormat");
                error = $"{titleError} - {string.Format(format, getFromMinutes((int)totalBreak), getFromMinutes(breakLimit))}";
            }

            return error;
        }
    }
}