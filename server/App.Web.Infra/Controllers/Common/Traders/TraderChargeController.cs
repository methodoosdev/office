using App.Core.Domain.Logging;
using App.Models.Accounting;
using App.Services.Common;
using App.Services.Logging;
using App.Services.Traders;
using App.Web.Common.Factories;
using App.Web.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Traders
{ 
    public partial class TraderChargeController : BaseProtectController
    {
        private readonly ITraderChargeFactory _traderChargeFactory;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ISqlConnectionService _connectionService;
        private readonly ICustomerActivityService _customerActivityService;

        public TraderChargeController(
            ITraderChargeFactory traderChargeFactory,
            ITraderConnectionService traderConnectionService,
            ISqlConnectionService connectionService,
            ICustomerActivityService customerActivityService
            )
        {
            _traderChargeFactory = traderChargeFactory;
            _traderConnectionService = traderConnectionService;
            _connectionService = connectionService;
            _customerActivityService = customerActivityService;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _traderChargeFactory.PrepareTraderChargeSearchModelAsync(new TraderChargeSearchModel());

            //prepare model
            var tableModel = await _traderChargeFactory.PrepareTraderChargeTableModelAsync(new TraderChargeTableModel());

            return Json(new { searchModel, tableModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] TraderChargeSearchModel searchModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            var hyperMConnection = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!hyperMConnection.Success)
                return BadRequest(hyperMConnection.Error);

            var model = await _traderChargeFactory.PrepareTraderChargeListAsync(connectionResult, hyperMConnection.Connection);

            //await _customerActivityService.InsertActivityOnceAsync(ActivityLogTypeType.TraderCharge);

            return Json(model);
        }

    }
}