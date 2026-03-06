using App.Models.Payroll;
using App.Services.Common;
using App.Services.Security;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Payroll.WorkerLeave;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Payroll.WorkerLeave
{
    public partial class WorkerSickLeaveController : BaseProtectController
    {
        private readonly ISqlConnectionService _connectionService;
        private readonly IWorkerSickLeaveFactory _workerSickLeaveFactory;
        private readonly ITraderService _traderService;

        public WorkerSickLeaveController(
            ISqlConnectionService connectionService,
            IWorkerSickLeaveFactory workerSickLeaveFactory,
            ITraderService traderService)
        {
            _connectionService = connectionService;
            _workerSickLeaveFactory = workerSickLeaveFactory;
            _traderService = traderService;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _workerSickLeaveFactory.PrepareWorkerSickLeaveSearchModelAsync(new WorkerSickLeaveSearchModel());

            var tableModel = await _workerSickLeaveFactory.PrepareWorkerSickLeaveTableModelAsync(new WorkerSickLeaveTableModel());

            return Json(new { searchModel, tableModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] WorkerSickLeaveSearchModel searchModel)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            var trader = await _traderService.GetTraderByIdAsync(searchModel.TraderId);

            var model = await _workerSickLeaveFactory.PrepareWorkerSickLeaveListAsync(searchModel, trader, result.Connection);

            return Json(model);
        }

    }
}
