using App.Services.Common;
using App.Web.Common.Models.Payroll;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Payroll;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Payroll
{
    public partial class ApdContributionController : BaseProtectController
    {
        private readonly ISqlConnectionService _connectionService;
        private readonly IApdContributionModelFactory _apdContributionModelFactory;

        public ApdContributionController(
           ISqlConnectionService connectionService,
           IApdContributionModelFactory apdContributionModelFactory)
        {
            _connectionService = connectionService;
            _apdContributionModelFactory = apdContributionModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _apdContributionModelFactory.PrepareApdContributionSearchModelAsync(new ApdContributionSearchModel());

            //prepare model
            var tableModel = await _apdContributionModelFactory.PrepareApdContributionTableModelAsync(new ApdContributionTableModel(), searchModel);

            return Json(new { searchModel, tableModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ApdContributionSearchModel searchModel)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            var model = await _apdContributionModelFactory.PrepareApdContributionListModelAsync(searchModel, result.Connection);

            return Json(model);
        }
    }
}
