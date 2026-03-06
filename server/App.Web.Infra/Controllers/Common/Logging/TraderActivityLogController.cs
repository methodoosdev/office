using App.Models.Logging;
using App.Services.Common;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Logging
{
    public partial class TraderActivityLogController : BaseProtectController
    {
        private readonly ISqlConnectionService _connectionService;
        private readonly ITraderActivityLogModelFactory _traderActivityLogModelFactory;

        public TraderActivityLogController(
            ISqlConnectionService connectionService,
            ITraderActivityLogModelFactory traderActivityLogModelFactory)
        {
            _connectionService = connectionService;
            _traderActivityLogModelFactory = traderActivityLogModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //searchModel
            var searchModel = await _traderActivityLogModelFactory.PrepareTraderActivityLogSearchModelAsync(new TraderActivityLogSearchModel());

            //formModel
            var formModel = await _traderActivityLogModelFactory.PrepareTraderActivityLogSearchFormModelAsync(new TraderActivityLogSearchFormModel());

            //tableModel
            var tableModel = await _traderActivityLogModelFactory.PrepareTraderActivityLogTableModelAsync(new TraderActivityLogTableModel());

            return Json(new { searchModel, formModel, tableModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] TraderActivityLogSearchModel searchModel)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.Office);
            if (!result.Success)
                return await BadRequestMessageAsync(result.Error);

            var model = await _traderActivityLogModelFactory.PrepareTraderActivityLogModelListAsync(result.Connection, searchModel.From, searchModel.To);

            return Json(model);
        }

    }
}
