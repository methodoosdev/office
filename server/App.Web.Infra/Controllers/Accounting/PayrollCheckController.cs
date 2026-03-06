using App.Models.Accounting;
using App.Services.Common;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Accounting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Accounting
{
    public partial class PayrollCheckController : BaseProtectController
    {
        private readonly ISqlConnectionService _connectionService;
        private readonly IPayrollCheckModelFactory _payrollCheckModelFactory;

        public PayrollCheckController(
            ISqlConnectionService connectionService,
            IPayrollCheckModelFactory payrollCheckModelFactory)
        {
            _connectionService = connectionService;
            _payrollCheckModelFactory = payrollCheckModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //searchModel
            var searchModel = await _payrollCheckModelFactory.PreparePayrollCheckSearchModelAsync(new PayrollCheckSearchModel());

            //formModel
            var formModel = await _payrollCheckModelFactory.PreparePayrollCheckSearchFormModelAsync(new PayrollCheckSearchFormModel(), 0);

            //tableModel
            var tableModel = await _payrollCheckModelFactory.PreparePayrollCheckTableModelAsync(new PayrollCheckTableModel());

            return Json(new { searchModel, formModel, tableModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] PayrollCheckSearchModel searchModel)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            var model = await _payrollCheckModelFactory.PreparePayrollCheckModelListAsync(result.Connection, searchModel);

            return Json(model);
        }

    }
}
