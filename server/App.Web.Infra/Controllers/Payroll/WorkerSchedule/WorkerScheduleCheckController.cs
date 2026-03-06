using App.Core;
using App.Models.Payroll;
using App.Services.Common;
using App.Services.Payroll;
using App.Services.Security;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Payroll.WorkerSchedules;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using App.Models.Traders;
using App.Models.Common;

namespace App.Web.Infra.Controllers.Payroll.WorkerSchedule
{
    public partial class WorkerScheduleCheckController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly IWorkerScheduleService _workerScheduleService;
        private readonly IWorkerScheduleCheckModelFactory _workerScheduleCheckModelFactory;
        private readonly ISqlConnectionService _connectionService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        public WorkerScheduleCheckController(ITraderService traderService,
            IWorkerScheduleService workerScheduleService,
            IWorkerScheduleCheckModelFactory workerScheduleCheckModelFactory,
            ISqlConnectionService connectionService,
            IPermissionService permissionService,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _workerScheduleService = workerScheduleService;
            _workerScheduleCheckModelFactory = workerScheduleCheckModelFactory;
            _connectionService = connectionService;
            _permissionService = permissionService;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List(int parentId)
        {
            //try to get entity with the specified id
            var workerSchedule = await _workerScheduleService.GetWorkerScheduleByIdAsync(parentId);
            if (workerSchedule == null)
                return await AccessDenied();

            var trader = await _workContext.GetCurrentTraderAsync();
            //if true some trader edit other trader record
            if (trader != null && !workerSchedule.TraderId.Equals(trader.Id))
                return await AccessDenied();

            var isTrader = trader != null;
            if (trader == null)
                trader = await _traderService.GetTraderByIdAsync(workerSchedule.TraderId);

            //prepare model
            var traderName = trader.ToTraderFullName();
            var model = await _workerScheduleCheckModelFactory.PrepareWorkerScheduleCheckModelAsync(new WorkerScheduleCheckModel());
            model.TraderName = traderName;
            model.FileName = $"{traderName}_{workerSchedule.PeriodFromDate.ToString("dd-MM-yy")}_{workerSchedule.PeriodToDate.ToString("dd-MM-yy")}";
            model.IsTrader = isTrader;

            return Json(new { model });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] WorkerScheduleCheckModel model, int parentId)
        {
            //try to get entity with the specified id
            var workerSchedule = await _workerScheduleService.GetWorkerScheduleByIdAsync(parentId);
            if (workerSchedule == null)
                return await AccessDenied();

            var trader = await _workContext.GetCurrentTraderAsync();
            //if true some trader edit other trader record
            if (trader != null && !workerSchedule.TraderId.Equals(trader.Id))
                return await AccessDenied();

            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return BadRequest(result.Error);

            var resultList = await _workerScheduleCheckModelFactory.PrepareWorkerScheduleCheckListModelAsync(result.Connection, workerSchedule.Id);

            return Json(resultList);
        }
    }
}