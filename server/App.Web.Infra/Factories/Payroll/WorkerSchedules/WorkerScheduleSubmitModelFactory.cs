using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Payroll;
using App.Services.Localization;
using App.Services.Payroll;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Payroll.WorkerSchedules
{
    public partial interface IWorkerScheduleSubmitModelFactory
    {
        Task<WorkerScheduleSubmitModel> PrepareWorkerScheduleSubmitModelAsync(WorkerScheduleSubmitModel submitModel);
        Task<IList<WorkerScheduleDateModel>> PrepareWorkerScheduleSubmitDataModelAsync(int workerScheduleId);
    }
    public partial class WorkerScheduleSubmitModelFactory : IWorkerScheduleSubmitModelFactory
    {
        private readonly IWorkerScheduleDateService _workerScheduleDateService;
        private readonly IWorkerScheduleWorkerService _workerScheduleWorkerService;
        private readonly ILocalizationService _localizationService;

        public WorkerScheduleSubmitModelFactory(
            IWorkerScheduleDateService workerScheduleDateService,
            IWorkerScheduleWorkerService workerScheduleWorkerService,
            ILocalizationService localizationService)
        {
            _workerScheduleDateService = workerScheduleDateService;
            _workerScheduleWorkerService = workerScheduleWorkerService;
            _localizationService = localizationService;
        }

        public virtual async Task<WorkerScheduleSubmitModel> PrepareWorkerScheduleSubmitModelAsync(WorkerScheduleSubmitModel submitModel)
        {
            submitModel.Columns = CreateKendoGridColumnConfig();
            submitModel.Title = await _localizationService.GetResourceAsync("App.Models.WorkerScheduleDateModel.ListForm.Title");

            return submitModel;
        }

        public virtual async Task<IList<WorkerScheduleDateModel>> PrepareWorkerScheduleSubmitDataModelAsync(int workerScheduleId)
        {
            var employees = await _workerScheduleWorkerService.GetAllWorkerScheduleWorkersAsync(workerScheduleId);

            var query = _workerScheduleDateService.Table.AsEnumerable()
                .Where(a => a.WorkerScheduleId == workerScheduleId)
                .Select(x =>
                {
                    var model = x.ToModel<WorkerScheduleDateModel>();

                    model.WorkerCardName = employees.FirstOrDefault(x => x.WorkerId == model.WorkerId)?.WorkerCardName ?? "";
                    model.WorkerName = employees.FirstOrDefault(x => x.WorkerId == model.WorkerId)?.WorkerName ?? "";

                    return model;
                }).AsQueryable();

            query = query.OrderBy(x => x.WorkingDate);

            return await query.ToListAsync();
        }

        private Dictionary<string, ColumnConfig> CreateKendoGridColumnConfig()
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var rightJustify = new Dictionary<string, string> { ["justify-content"] = "right" };
            var columns = new Dictionary<string, ColumnConfig>
            {
                ["workerCardName"] = ColumnConfig.Create<WorkerScheduleDateModel>(1, nameof(WorkerScheduleDateModel.WorkerCardName), width: 200, hidden: true, _class: "time-column"),
                ["workingDate"] = ColumnConfig.Create<WorkerScheduleDateModel>(2, nameof(WorkerScheduleDateModel.WorkingDate), ColumnType.Date, width: 110, _class: "time-column"),
                ["nonstopFromDate"] = ColumnConfig.Create<WorkerScheduleDateModel>(3, nameof(WorkerScheduleDateModel.NonstopFromDate), ColumnType.Time, width: 110, _class: "time-column"),
                ["nonstopToDate"] = ColumnConfig.Create<WorkerScheduleDateModel>(4, nameof(WorkerScheduleDateModel.NonstopToDate), ColumnType.Time, width: 110, _class: "time-column"),
                ["splitFromDate"] = ColumnConfig.Create<WorkerScheduleDateModel>(5, nameof(WorkerScheduleDateModel.SplitFromDate), ColumnType.Time, width: 110, _class: "time-column"),
                ["splitToDate"] = ColumnConfig.Create<WorkerScheduleDateModel>(6, nameof(WorkerScheduleDateModel.SplitToDate), ColumnType.Time, width: 110, _class: "time-column"),
                ["breakNonstopFromDate"] = ColumnConfig.Create<WorkerScheduleDateModel>(7, nameof(WorkerScheduleDateModel.BreakNonstopFromDate), ColumnType.Time, width: 110, _class: "time-column"),
                ["breakNonstopToDate"] = ColumnConfig.Create<WorkerScheduleDateModel>(8, nameof(WorkerScheduleDateModel.BreakNonstopToDate), ColumnType.Time, width: 110, _class: "time-column"),
                ["breakSplitFromDate"] = ColumnConfig.Create<WorkerScheduleDateModel>(7, nameof(WorkerScheduleDateModel.BreakSplitFromDate), ColumnType.Time, width: 110, _class: "time-column"),
                ["breakSplitToDate"] = ColumnConfig.Create<WorkerScheduleDateModel>(8, nameof(WorkerScheduleDateModel.BreakSplitToDate), ColumnType.Time, width: 110, _class: "time-column"),
                ["overtimeFromDate"] = ColumnConfig.Create<WorkerScheduleDateModel>(9, nameof(WorkerScheduleDateModel.OvertimeFromDate), ColumnType.Time, width: 110, _class: "time-column", hidden: true),
                ["overtimeToDate"] = ColumnConfig.Create<WorkerScheduleDateModel>(10, nameof(WorkerScheduleDateModel.OvertimeToDate), ColumnType.Time, width: 110, _class: "time-column", hidden: true),
                ["leave"] = ColumnConfig.Create<WorkerScheduleDateModel>(11, nameof(WorkerScheduleDateModel.Leave), ColumnType.Checkbox, width: 80, _class: "time-column", style: textAlign),
                ["sickLeave"] = ColumnConfig.Create<WorkerScheduleDateModel>(12, nameof(WorkerScheduleDateModel.SickLeave), ColumnType.Checkbox, width: 80, _class: "time-column", style: textAlign),
                ["isSplit"] = ColumnConfig.Create<WorkerScheduleDateModel>(13, nameof(WorkerScheduleDateModel.IsSplit), ColumnType.Checkbox, width: 80, _class: "time-column", style: textAlign),
                ["breakNonstop2FromDate"] = ColumnConfig.Create<WorkerScheduleDateModel>(15, nameof(WorkerScheduleDateModel.BreakNonstop2FromDate), ColumnType.Time, width: 110, _class: "time-column"),
                ["breakNonstop2ToDate"] = ColumnConfig.Create<WorkerScheduleDateModel>(16, nameof(WorkerScheduleDateModel.BreakNonstop2ToDate), ColumnType.Time, width: 110, _class: "time-column"),
                ["dailyBreak"] = ColumnConfig.Create<WorkerScheduleDateModel>(14, nameof(WorkerScheduleDateModel.DailyBreak), width: 105, _class: "time-column", style: rightAlign, headerStyle: rightJustify),
                ["dailyTotalHours"] = ColumnConfig.Create<WorkerScheduleDateModel>(14, nameof(WorkerScheduleDateModel.DailyTotalHours), width: 90, _class: "time-column", style: rightAlign, headerStyle: rightJustify),
            };

            return columns;
        }
    }
}