using App.Models.Payroll;
using App.Services.Common;
using App.Services.Security;
using App.Services.Traders;
using App.Web.Common.Models.Payroll;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using App.Web.Infra.Factories.Payroll;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Payroll.PayrollStatus
{
    public partial class PayrollStatusController : BaseProtectController
    {
        private readonly ISqlConnectionService _connectionService;
        private readonly IPermissionService _permissionService;
        private readonly IPayrollStatusModelFactory _payrollStatusModelFactory;
        private readonly ITraderService _traderService;

        public PayrollStatusController(
            ISqlConnectionService connectionService,
            IPermissionService permissionService,
            IPayrollStatusModelFactory payrollStatusModelFactory,
            ITraderService traderService)
        {
            _connectionService = connectionService;
            _permissionService = permissionService;
            _payrollStatusModelFactory = payrollStatusModelFactory;
            _traderService = traderService;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _payrollStatusModelFactory.PreparePayrollStatusSearchModelAsync(new PayrollStatusSearchModel());

            var tableModel = await _payrollStatusModelFactory.PreparePayrollStatusTableModelAsync(new PayrollStatusTableModel());

            return Json(new { searchModel, tableModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] PayrollStatusSearchModel searchModel)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            var trader = await _traderService.GetTraderByIdAsync(searchModel.TraderId);

            var model = await _payrollStatusModelFactory.PreparePayrollStatusListModelAsync(searchModel, trader, result.Connection);

            return Json(model);
        }

    }
}
