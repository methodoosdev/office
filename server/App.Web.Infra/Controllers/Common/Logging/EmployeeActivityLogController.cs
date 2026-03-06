using App.Models.Logging;
using App.Services.Common;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Logging
{
    public partial class EmployeeActivityLogController : BaseProtectController
    {
        private readonly ISqlConnectionService _connectionService;
        private readonly IEmployeeActivityLogModelFactory _traderActivityLogModelFactory;

        public EmployeeActivityLogController(
            ISqlConnectionService connectionService,
            IEmployeeActivityLogModelFactory traderActivityLogModelFactory)
        {
            _connectionService = connectionService;
            _traderActivityLogModelFactory = traderActivityLogModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //searchModel
            var searchModel = await _traderActivityLogModelFactory.PrepareEmployeeActivityLogSearchModelAsync(new EmployeeActivityLogSearchModel());

            //formModel
            var formModel = await _traderActivityLogModelFactory.PrepareEmployeeActivityLogSearchFormModelAsync(new EmployeeActivityLogSearchFormModel());

            //tableModel
            var tableModel = await _traderActivityLogModelFactory.PrepareEmployeeActivityLogTableModelAsync(new EmployeeActivityLogTableModel());

            return Json(new { searchModel, formModel, tableModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] EmployeeActivityLogSearchModel searchModel)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.Office);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            var model = await _traderActivityLogModelFactory.PrepareEmployeeActivityLogModelListAsync(result.Connection, searchModel.From, searchModel.To);

            return Json(model);
        }

    }
}
