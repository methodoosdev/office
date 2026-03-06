using App.Services.Common;
using App.Web.Common.Models.Payroll;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Payroll;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Payroll
{
    public partial class FmyContributionController : BaseProtectController
    {
        private readonly ISqlConnectionService _connectionService;
        private readonly IFmyContributionModelFactory _fmyContributionModelFactory;

        public FmyContributionController(
           ISqlConnectionService connectionService,
           IFmyContributionModelFactory fmyContributionModelFactory)
        {
            _connectionService = connectionService;
            _fmyContributionModelFactory = fmyContributionModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _fmyContributionModelFactory.PrepareFmyContributionSearchModelAsync(new FmyContributionSearchModel());

            //prepare model
            var tableModel = await _fmyContributionModelFactory.PrepareFmyContributionTableModelAsync(new FmyContributionTableModel(), searchModel);

            return Json(new { searchModel, tableModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] FmyContributionSearchModel searchModel)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            var tableModel = await _fmyContributionModelFactory.PrepareFmyContributionTableModelAsync(new FmyContributionTableModel(), searchModel);

            var columns = tableModel.CustomProperties["columns"];
            var list = await _fmyContributionModelFactory.PrepareFmyContributionListModelAsync(searchModel, result.Connection);

            return Json(new { list, columns });
        }
    }
}
