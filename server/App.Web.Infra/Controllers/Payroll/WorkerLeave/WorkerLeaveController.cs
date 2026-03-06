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
    public partial class WorkerLeaveController : BaseProtectController
    {
        private readonly ISqlConnectionService _connectionService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkerLeaveFactory _workerLeaveFactory;
        private readonly ITraderService _traderService;

        public WorkerLeaveController(
            ISqlConnectionService connectionService,
            IPermissionService permissionService,
            IWorkerLeaveFactory workerLeaveFactory,
            ITraderService traderService)
        {
            _connectionService = connectionService;
            _permissionService = permissionService;
            _workerLeaveFactory = workerLeaveFactory;
            _traderService = traderService;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _workerLeaveFactory.PrepareWorkerLeaveSearchModelAsync(new WorkerLeaveSearchModel());

            var tableModel = await _workerLeaveFactory.PrepareWorkerLeaveTableModelAsync(new WorkerLeaveTableModel());

            return Json(new { searchModel, tableModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] WorkerLeaveSearchModel searchModel)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            var trader = await _traderService.GetTraderByIdAsync(searchModel.TraderId);

            var model = await _workerLeaveFactory.PrepareWorkerLeaveListAsync(searchModel, trader, result.Connection);

            return Json(model);
        }

    }
}
