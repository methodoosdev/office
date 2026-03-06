using App.Core;
using App.Models.Payroll;
using App.Services.Common;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using App.Web.Infra.Factories.Payroll;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Payroll
{
    [CheckCustomerPermission(true)]
    public partial class WorkerCatalogByTraderController : BaseProtectController
    {
        private readonly ISqlConnectionService _connectionService;
        private readonly IWorkerCatalogByTraderModelFactory _workerCatalogModelFactory;
        private readonly IWorkContext _workContext;

        public WorkerCatalogByTraderController(
            ISqlConnectionService connectionService,
            IWorkerCatalogByTraderModelFactory workerCatalogModelFactory,
            IWorkContext workContext)
        {
            _connectionService = connectionService;
            _workerCatalogModelFactory = workerCatalogModelFactory;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null)
                return await AccessDenied();

            //prepare model
            var searchModel = await _workerCatalogModelFactory.PrepareWorkerCatalogSearchModelAsync(new WorkerCatalogSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] WorkerCatalogSearchModel searchModel)
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            if (trader == null)
                return await AccessDenied();

            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            //prepare model
            var model = await _workerCatalogModelFactory.PrepareWorkerCatalogListModelAsync(searchModel, result.Connection, trader.HyperPayrollId);

            return Json(model);
        }

    }
}