using App.Core;
using App.Models.Payroll;
using App.Services.Common;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using App.Web.Infra.Factories.Payroll;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Payroll
{
    public partial class EmployeeSalaryCostController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly IEmployeeSalaryCostModelFactory _employeeSalaryCostModelFactory;
        private readonly ISqlConnectionService _connectionService;
        private readonly IWorkContext _workContext;

        public EmployeeSalaryCostController(
            ITraderService traderService,
            IEmployeeSalaryCostModelFactory employeeSalaryCostModelFactory,
            ISqlConnectionService connectionService,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _employeeSalaryCostModelFactory = employeeSalaryCostModelFactory;
            _connectionService = connectionService;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            var searchModel = await _employeeSalaryCostModelFactory.PrepareEmployeeSalaryCostSearchModelAsync(new EmployeeSalaryCostSearchModel());

            int? companyId = searchModel.CompanyId.HasValue ? searchModel.CompanyId.Value : null;
            var searchFormModel = await _employeeSalaryCostModelFactory.PrepareEmployeeSalaryCostSearchFormModelAsync(new EmployeeSalaryCostSearchFormModel(), result.Connection, companyId, searchModel.AllPackages);

            var dataResultFormModel = await _employeeSalaryCostModelFactory.PrepareEmployeeSalaryCostResultFormModelAsync(new EmployeeSalaryCostFormModel());

            var dataFormModel = await _employeeSalaryCostModelFactory.PrepareEmployeeSalaryCostFormModelAsync(new EmployeeSalaryCostFormModel());

            return Json(new { searchModel, dataResultFormModel, searchFormModel, dataFormModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] EmployeeSalaryCostSearchModel searchModel)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            var model = await _employeeSalaryCostModelFactory.PrepareEmployeeSalaryCostModelAsync(searchModel, result.Connection);

            return Json(new { model });
        }

        [HttpPost]
        [CheckCustomerPermission(true)]
        public virtual async Task<IActionResult> GetPackages([FromBody] EmployeeSalaryCostSearchModel searchModel)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            var data = await _employeeSalaryCostModelFactory.PrepareInsurancePackagesAsync(searchModel, result.Connection);

            return Json(new { data });
        }
    }
}
