using App.Core.Domain.Traders;
using App.Data.DataProviders;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Traders;
using App.Services.Common;
using App.Services.Security;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Traders;
using App.Web.Infra.Queries.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Traders
{
    public partial class SrfTraderController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ISqlConnectionService _connectionService;
        private readonly ISrfTraderModelFactory _srfTraderModelFactory;

        public SrfTraderController(ITraderService traderService,
            IAppDataProvider dataProvider,
            ISqlConnectionService connectionService,
            ISrfTraderModelFactory srfTraderModelFactory)
        {
            _traderService = traderService;
            _dataProvider = dataProvider;
            _connectionService = connectionService;
            _srfTraderModelFactory = srfTraderModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _srfTraderModelFactory.PrepareSrfTraderSearchModelAsync(new SrfTraderSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] SrfTraderSearchModel searchModel)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.Srf);
            if (!result.Success)
                return BadRequest(result.Error);

            //prepare model
            var model = await _srfTraderModelFactory.PrepareSrfTraderListModelAsync(searchModel, result.Connection);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Import([FromBody] ICollection<int> selectedIds)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.Srf);
            if (!result.Success)
                return BadRequest(result.Error);

            var pValue = "(" + string.Join(',', selectedIds.ToArray()) + ")";

            var pQuery = SrfTradersQuery.With.Replace("@@pList", pValue);

            var tradersList = (await _dataProvider.QueryAsync<SrfTraderModel>(result.Connection, pQuery)).Decrypt();

            foreach (var model in tradersList)
            {
                var trader = await _traderService.GetTraderByVatAsync(model.Vat);
                if (trader == null)
                {
                    trader = model.ToEntity<Trader>();
                    trader.Active = true;
                    await _traderService.InsertTraderAsync(trader);
                }
                else
                {
                    model.ToEntity(trader);
                    await _traderService.UpdateTraderAsync(trader);
                }
            }

            return Ok();
        }
    }
}