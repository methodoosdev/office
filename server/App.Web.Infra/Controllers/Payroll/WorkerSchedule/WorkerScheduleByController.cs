using App.Core.Domain.Payroll;
using App.Services.Helpers;
using App.Services.Payroll;
using App.Web.Framework.Controllers;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Payroll.WorkerSchedule
{
    public abstract class WorkerScheduleByController : BaseProtectController
    {
        protected readonly IWorkerScheduleDateService _workerScheduleDateService;
        protected readonly IDateTimeHelper _dateTimeHelper;

        public WorkerScheduleByController(IWorkerScheduleDateService workerScheduleDateService,
            IDateTimeHelper dateTimeHelper)
        {
            _workerScheduleDateService = workerScheduleDateService;
            _dateTimeHelper = dateTimeHelper;
        }

        protected async Task UpdateActivePropertyFromWorkerScheduleDateAsync(int workerScheduleId, bool active)
        {
            var workerScheduleDates = await _workerScheduleDateService.GetAllWorkerScheduleDatesAsync(workerScheduleId);
            foreach (var model in workerScheduleDates)
                model.Active = active;

            await _workerScheduleDateService.UpdateWorkerScheduleDateAsync(workerScheduleDates);
        }

        protected async Task UpdateWorkerScheduleDateAsync(int workerScheduleId, int traderId)
        {
            //Get WorkerScheduleDates before delete
            var workerScheduleDates = await _workerScheduleDateService.GetAllWorkerScheduleDatesAsync(workerScheduleId);
            var dates = workerScheduleDates.Select(x => x.WorkingDate).Distinct().ToList();
            var workers = workerScheduleDates.Select(x => x.WorkerId).Distinct().ToList();

            //Get workerScheduleDates belong trader
            var list = await _workerScheduleDateService.Table
                .Where(x =>
                    x.WorkerScheduleId != workerScheduleId &&
                    x.TraderId == traderId &&
                    dates.Contains(x.WorkingDate) &&
                    workers.Contains(x.WorkerId) &&
                    !x.Active)
                .ToListAsync();

            var query = list.GroupBy(g => new { g.WorkerId, g.WorkingDate })
                .OrderBy(o => o.Key.WorkerId).ThenBy(o => o.Key.WorkingDate)
                .Select(x => x.OrderByDescending(o => o.Id).First());

            var updatedList = await query.ToListAsync();

            foreach (var item in updatedList)
                item.Active = true;

            await _workerScheduleDateService.UpdateWorkerScheduleDateAsync(updatedList);
        }

        protected string DailyName(WorkerScheduleDate model)
        {
            var dailyBreakNonstop = model.BreakNonstopToDate - model.BreakNonstopFromDate + (model.BreakNonstop2ToDate - model.BreakNonstop2FromDate);
            var dailyBreakSplit = model.BreakSplitToDate - model.BreakSplitFromDate;

            var nonstopFromDate = model.NonstopFromDate.ToString("HH:mm");
            var nonstopToDate = (model.NonstopToDate - dailyBreakNonstop).ToString("HH:mm");

            var fullName = $"{nonstopFromDate}-{nonstopToDate}";
            if (model.IsSplit)
            {
                var splitFromDate = model.SplitFromDate.ToString("HH:mm");
                var splitToDate = (model.SplitToDate - dailyBreakSplit).ToString("HH:mm");

                fullName = $"{nonstopFromDate}-{nonstopToDate} & {splitFromDate}-{splitToDate}";
            }

            if (fullName == "00:00-00:00")
                return null;

            return fullName;
        }
    }
}