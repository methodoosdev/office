using App.Core;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Payroll;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Payroll;
using App.Services.Traders;
using App.Web.Infra.Queries.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Payroll.WorkerSchedules
{
    public partial interface IWorkerScheduleCheckModelFactory
    {
        Task<WorkerScheduleCheckModel> PrepareWorkerScheduleCheckModelAsync(WorkerScheduleCheckModel model);
        Task<IList<WorkerScheduleResult>> PrepareWorkerScheduleCheckListModelAsync(string connection, int parentId);
    }

    public partial class WorkerScheduleCheckModelFactory : IWorkerScheduleCheckModelFactory
    {
        private readonly ITraderService _traderService;
        private readonly IWorkerScheduleService _workerScheduleService;
        private readonly IWorkerScheduleDateService _workerScheduleDateService;
        private readonly IWorkerScheduleWorkerService _workerScheduleWorkerService;
        private readonly ILocalizationService _localizationService;
        private readonly IAppDataProvider _dataProvider;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        public WorkerScheduleCheckModelFactory(ITraderService traderService,
            IWorkerScheduleService workerScheduleService,
            IWorkerScheduleDateService workerScheduleDateService,
            IWorkerScheduleWorkerService workerScheduleWorkerService,
            ILocalizationService localizationService,
            IAppDataProvider dataProvider,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _workerScheduleService = workerScheduleService;
            _workerScheduleDateService = workerScheduleDateService;
            _workerScheduleWorkerService = workerScheduleWorkerService;
            _localizationService = localizationService;
            _dataProvider = dataProvider;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        public virtual async Task<WorkerScheduleCheckModel> PrepareWorkerScheduleCheckModelAsync(WorkerScheduleCheckModel model)
        {
            model.Columns = CreateKendoGridColumnConfig();
            model.DetailColumns = CreateKendoGridDetailsColumnConfig();
            model.Title = await _localizationService.GetResourceAsync("App.Models.WorkerScheduleResult.Title");

            return model;
        }

        private async Task<IList<WorkerScheduleQueryResult>> GetWorkersAsync(string connection, int companyId)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
            var workers = await _dataProvider.QueryAsync<WorkerScheduleQueryResult>(connection, WorkerScheduleQuery.Workers, pCompanyId);

            return workers;
        }

        double getHours(double value)
        {
            return Math.Floor(value / 60);
        }

        double getMinutes(double value)
        {
            var minutes = Math.Floor(value % 60);
            return minutes;
        }

        public async Task<IList<WorkerScheduleResult>> PrepareWorkerScheduleCheckListModelAsync(string connection, int parentId)
        {
            var models = await _workerScheduleDateService.GetAllWorkerScheduleDatesAsync(parentId);
            //models = models.Where(x => x.Active).ToList();

            // Οι εβδομάδες της περιόδου
            var weeks = new List<int>();
            // Οι μέρες της περιόδου
            var dates = models.Select(x => x.WorkingDate).Distinct().OrderBy(o => o).ToList();
            foreach (var date in dates)
            {
                var week = await _dateTimeHelper.GetWeekOfYear(date);
                if (!weeks.Contains(week))
                    weeks.Add(week);
            }

            // Οι πρώτη και τελευταία ημέρα της εβδομάδας
            var dict = new Dictionary<int, (DateTime first, DateTime last)>();
            foreach (var week in weeks)
            {
                var first = _dateTimeHelper.FirstDateOfWeek(week, dates.FirstOrDefault().Year);
                var last = _dateTimeHelper.LastDateOfWeek(first);
                dict.Add(week, (first, last));
            }

            // Η πρώτη ημέρα της περιόδου
            var firstDate = dict.First().Value.first;

            // Η τελευταία ημέρα της περιόδου
            var lastDate = dict.Last().Value.last;

            var workerSchedule = await _workerScheduleService.GetWorkerScheduleByIdAsync(parentId);
            var workerCards = await _workerScheduleWorkerService.GetAllWorkerScheduleWorkersAsync(workerSchedule.Id);
            var workerCardIds = workerCards.Select(x => x.WorkerId).ToList();

            // Προηγούμενες ημέρες που ανήκουν στην πρώτη εβδομάδα
            var _dataList = await _workerScheduleDateService.Table
                .Where(x =>
                    workerCardIds.Contains(x.WorkerId) &&
                    x.TraderId == workerSchedule.TraderId &&
                    x.WorkingDate >= firstDate &&
                    x.WorkingDate <= lastDate &&
                    //x.WorkerScheduleId != workerSchedule.Id &&
                    x.Active)
                .ToListAsync();

            //...todo group for multiple dates

            // Όλες οι ημέρες που ανήκουν για τον υπολογισμό
            //var _dataList = previousModel.Concat(models).ToList();

            var dataList = _dataList.Select(x => x.ToModel<WorkerScheduleDateModel>()).ToList();

            foreach (var workerScheduleDate in dataList)
            {
                var dailyBreakNonstop = (workerScheduleDate.BreakNonstopToDate - workerScheduleDate.BreakNonstopFromDate).TotalMinutes;
                var dailyBreakNonstop2 = (workerScheduleDate.BreakNonstop2ToDate - workerScheduleDate.BreakNonstop2FromDate).TotalMinutes;
                var dailyBreakSplit = (workerScheduleDate.BreakSplitToDate - workerScheduleDate.BreakSplitFromDate).TotalMinutes;
                var dailyNonstop = (workerScheduleDate.NonstopToDate - workerScheduleDate.NonstopFromDate).TotalMinutes;
                var dailySplit = (workerScheduleDate.SplitToDate - workerScheduleDate.SplitFromDate).TotalMinutes;

                dailyBreakNonstop = dailyBreakNonstop < 0 ? 24 * 60 + dailyBreakNonstop : dailyBreakNonstop;
                dailyBreakNonstop2 = dailyBreakNonstop2 < 0 ? 24 * 60 + dailyBreakNonstop2 : dailyBreakNonstop2;
                dailyBreakSplit = dailyBreakSplit < 0 ? 24 * 60 + dailyBreakSplit : dailyBreakSplit;
                dailyNonstop = dailyNonstop < 0 ? 24 * 60 + dailyNonstop : dailyNonstop;
                dailySplit = dailySplit < 0 ? 24 * 60 + dailySplit : dailySplit;

                dailyNonstop = dailyNonstop - dailyBreakNonstop - dailyBreakNonstop2;
                dailySplit = dailySplit - dailyBreakSplit;

                var dailyBreak = dailyBreakNonstop + dailyBreakNonstop2 + dailyBreakSplit;
                var dailyTotalHours = dailyNonstop + dailySplit;

                workerScheduleDate.DailyNonstop = dailyNonstop;
                workerScheduleDate.DailySplit = dailySplit;
                workerScheduleDate.DailyBreak = dailyBreak;
                workerScheduleDate.DailyTotalHours = dailyTotalHours;
            }

            var groups = dataList.GroupBy(g => new { g.WorkerVat, g.WorkingDate })
                .OrderBy(o => o.Key.WorkerVat).ThenBy(o => o.Key.WorkingDate)
                .Select(x => new
                {
                    x.Key.WorkerVat,
                    x.Key.WorkingDate,
                    Data = x.ToList()
                })
                .ToList();

            //
            var workerVats = workerCards.Select(x => x.WorkerVat).Distinct().ToList();
            var trader = await _traderService.GetTraderByIdAsync(workerSchedule.TraderId);

            //
            var workersResult = await GetWorkersAsync(connection, trader.HyperPayrollId);
            var workers = workersResult.Where(x => x.ActiveCard && workerVats.Contains(x.WorkerVat)).ToList();

            var detailsResultList = new List<WorkerScheduleDetailsResult>();
            foreach (var group in groups)
            {
                var model = new WorkerScheduleDetailsResult();
                model.WorkerVat = group.WorkerVat;
                model.WorkerName = workers.FirstOrDefault(x => x.WorkerVat == group.WorkerVat)?.WorkerName ?? "Name?";
                model.WorkingDate = group.WorkingDate;
                model.DailyNonstop = group.Data.Sum(x => x.DailyNonstop);
                model.DailySplit = group.Data.Sum(x => x.DailySplit);
                model.DailyBreak = group.Data.Sum(x => x.DailyBreak);
                model.DailyTotalHours = group.Data.Sum(x => x.DailyTotalHours);

                model.DailyNonstopValue = string.Format("{0:#00}:{1:#00}", getHours(model.DailyNonstop), getMinutes(model.DailyNonstop));
                model.DailySplitValue = string.Format("{0:#00}:{1:#00}", getHours(model.DailySplit), getMinutes(model.DailySplit));
                model.DailyBreakValue = string.Format("{0:#00}:{1:#00}", getHours(model.DailyBreak), getMinutes(model.DailyBreak));
                model.DailyTotalHoursValue = string.Format("{0:#00}:{1:#00}", getHours(model.DailyTotalHours), getMinutes(model.DailyTotalHours));

                model.Leave = group.Data.Any(x => x.Leave);
                model.SickLeave = group.Data.Any(x => x.SickLeave);

                detailsResultList.Add(model);
            }

            var resultList = new List<WorkerScheduleResult>();
            foreach (var worker in workers)
            {
                foreach (var week in dict)
                {
                    var first = week.Value.first;
                    var last = week.Value.last;

                    var weekList = detailsResultList.Where(x =>
                        x.WorkingDate >= first &&
                        x.WorkingDate <= last &&
                        x.WorkerVat == worker.WorkerVat).ToList();

                    var leave = weekList.Any(x => x.Leave);
                    var sickLeave = weekList.Any(x => x.SickLeave);
                    var weeklyTotalHours = weekList.Sum(x => x.DailyTotalHours);
                    var workingDays = weekList.Where(x => x.DailyTotalHours > 0 && (!x.Leave || !x.SickLeave)).Count();

                    var result = new WorkerScheduleResult
                    {
                        WorkerName = $"{worker.WorkerName} - {worker.WorkerVat}",
                        Period = $"({await _dateTimeHelper.GetWeekOfYear(first)}) {first.ToString("dd/MM/yyyy")} - {last.ToString("dd/MM/yyyy")}",
                        Leave = leave,
                        SickLeave = sickLeave,
                        WorkingHours = worker.WorkingHours,
                        WeeklyTotalHours = weeklyTotalHours,
                        WorkingHoursValue = string.Format("{0:#00}:{1:#00}", getHours(worker.WorkingHours * 60), getMinutes(worker.WorkingHours * 60)),
                        WeeklyTotalHoursValue = string.Format("{0:#00}:{1:#00}", getHours(weeklyTotalHours), getMinutes(weeklyTotalHours)),
                        MaxFortyHoursPerWeekError = weeklyTotalHours > 40 * 60,
                        MaxSixDaysPerWeekError = workingDays > 6,
                        ContractChangeError = leave || sickLeave || !weeklyTotalHours.Equals(worker.WorkingHours * 60),
                        Details = weekList
                    };

                    resultList.Add(result);
                }
            }

            return resultList;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<WorkerScheduleResult>(1, nameof(WorkerScheduleResult.WorkerName)),
                ColumnConfig.Create<WorkerScheduleResult>(1, nameof(WorkerScheduleResult.Period), hidden: true),
                ColumnConfig.Create<WorkerScheduleResult>(1, nameof(WorkerScheduleResult.MaxFortyHoursPerWeekError), ColumnType.Checkbox),
                ColumnConfig.Create<WorkerScheduleResult>(1, nameof(WorkerScheduleResult.MaxSixDaysPerWeekError), ColumnType.Checkbox),
                ColumnConfig.Create<WorkerScheduleResult>(1, nameof(WorkerScheduleResult.ContractChangeError), ColumnType.Checkbox),
                ColumnConfig.Create<WorkerScheduleResult>(1, nameof(WorkerScheduleResult.Leave), ColumnType.Checkbox),
                ColumnConfig.Create<WorkerScheduleResult>(1, nameof(WorkerScheduleResult.SickLeave), ColumnType.Checkbox),
                ColumnConfig.Create<WorkerScheduleResult>(1, nameof(WorkerScheduleResult.WorkingHoursValue), style: textAlign),
                ColumnConfig.Create<WorkerScheduleResult>(1, nameof(WorkerScheduleResult.WeeklyTotalHoursValue), style: textAlign)
            };

            return columns;
        }

        private List<ColumnConfig> CreateKendoGridDetailsColumnConfig()
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<WorkerScheduleDetailsResult>(1, nameof(WorkerScheduleDetailsResult.WorkerVat), hidden: true),
                ColumnConfig.Create<WorkerScheduleDetailsResult>(1, nameof(WorkerScheduleDetailsResult.WorkerName), hidden: true),
                ColumnConfig.Create<WorkerScheduleDetailsResult>(1, nameof(WorkerScheduleDetailsResult.WorkingDate), ColumnType.Date),
                ColumnConfig.Create<WorkerScheduleDetailsResult>(1, nameof(WorkerScheduleDetailsResult.DailyNonstopValue), style: textAlign),
                ColumnConfig.Create<WorkerScheduleDetailsResult>(1, nameof(WorkerScheduleDetailsResult.DailySplitValue), style: textAlign),
                ColumnConfig.Create<WorkerScheduleDetailsResult>(1, nameof(WorkerScheduleDetailsResult.DailyBreakValue), style: textAlign),
                ColumnConfig.Create<WorkerScheduleDetailsResult>(1, nameof(WorkerScheduleDetailsResult.DailyTotalHoursValue), style: textAlign),
                ColumnConfig.Create<WorkerScheduleDetailsResult>(1, nameof(WorkerScheduleDetailsResult.Leave), ColumnType.Checkbox),
                ColumnConfig.Create<WorkerScheduleDetailsResult>(1, nameof(WorkerScheduleDetailsResult.SickLeave), ColumnType.Checkbox)
            };

            return columns;
        }

    }
}