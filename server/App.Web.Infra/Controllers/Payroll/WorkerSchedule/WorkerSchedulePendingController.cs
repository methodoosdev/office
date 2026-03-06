using App.Models.Payroll;
using App.Services.Common;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Payroll.WorkerSchedules;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Payroll.WorkerSchedule
{
    public partial class WorkerSchedulePendingController : BaseProtectController
    {
        private readonly IWorkerSchedulePendingModelFactory _workerSchedulePendingModelFactory;
        private readonly ISqlConnectionService _connectionService;

        public WorkerSchedulePendingController(
            IWorkerSchedulePendingModelFactory workerSchedulePendingModelFactory,
            ISqlConnectionService connectionService)
        {
            _workerSchedulePendingModelFactory = workerSchedulePendingModelFactory;
            _connectionService = connectionService;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _workerSchedulePendingModelFactory.PrepareWorkerSchedulePendingSearchModelAsync(new WorkerSchedulePendingSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] WorkerSchedulePendingSearchModel searchModel)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.Office);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            //prepare model
            var model = await _workerSchedulePendingModelFactory.PrepareWorkerSchedulePendingListModelAsync(searchModel, result.Connection);

            return Json(model);
        }

    }
}