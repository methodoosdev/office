using App.Core.ComponentModel;
using App.Core.Domain.Traders;
using App.Core.Infrastructure.Mapper;
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
    public partial class TaxSystemTraderController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ISqlConnectionService _connectionService;
        private readonly ITaxSystemTraderModelFactory _srfTraderModelFactory;

        public TaxSystemTraderController(ITraderService traderService,
            IAppDataProvider dataProvider,
            ISqlConnectionService connectionService,
            ITaxSystemTraderModelFactory srfTraderModelFactory)
        {
            _traderService = traderService;
            _dataProvider = dataProvider;
            _connectionService = connectionService;
            _srfTraderModelFactory = srfTraderModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _srfTraderModelFactory.PrepareTaxSystemTraderSearchModelAsync(new TaxSystemTraderSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] TaxSystemTraderSearchModel searchModel)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.TaxSystem);
            if (!result.Success)
                return BadRequest(result.Error);

            //prepare model
            var year = DateTime.UtcNow.Year;
            var model = await _srfTraderModelFactory.PrepareTaxSystemTraderListModelAsync(searchModel, result.Connection, year);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Import([FromBody] ICollection<int> selectedIds)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.TaxSystem);
            if (!result.Success)
                return BadRequest(result.Error);

            var year = DateTime.UtcNow.Year;
            var pYear = new LinqToDB.Data.DataParameter("pYear", year);

            var tradersList = await _dataProvider.QueryAsync<TaxSystemTraderModel>(result.Connection, TaxSystemTradersQuery.Party, pYear);
            tradersList = tradersList.Where(x => selectedIds.Contains(x.TaxSystemId)).ToList();

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
                    var _active = trader.Active;
                    model.ToEntity(trader);
                    trader.Active = _active;
                    await _traderService.UpdateTraderAsync(trader);
                }
            }

            return Ok();
        }
        [HttpPost]
        public virtual async Task<IActionResult> ImportKeaoGredentials()
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.TaxSystem);
            if (!result.Success)
                return BadRequest(result.Error);

            var updateTraders = new List<Trader>();
            var keaoGredentialItemList = await _dataProvider.QueryAsync<TaxSystemKeaoGredentialItem>(result.Connection, TaxSystemTradersQuery.KeaoGredentials);

            foreach (var item in keaoGredentialItemList.OrderBy(x => x.TaxSystemId).ToList())
            {
                var updatedTrader = updateTraders.FirstOrDefault(x => x.TaxSystemId == item.TaxSystemId);

                var trader = updatedTrader != null ? updatedTrader : await _traderService.GetTraderByTaxSystemIdAsync(item.TaxSystemId);
                if (trader != null)// If trader exist in DataBase
                {
                    switch (item.CarrierId)
                    {
                        case 2:
                            trader.KeaoIkaUserName = AesEncryption.Encrypt(item.UserName);
                            trader.KeaoIkaPassword = AesEncryption.Encrypt(item.Password);
                            break;
                        case 3:
                            trader.KeaoOaeeUserName = AesEncryption.Encrypt(item.UserName);
                            trader.KeaoOaeePassword = AesEncryption.Encrypt(item.Password);
                            break;
                        case 39:
                            trader.KeaoEfkaUserName = AesEncryption.Encrypt(item.UserName);
                            trader.KeaoEfkaPassword = AesEncryption.Encrypt(item.Password);
                            break;
                    }
                    if (updatedTrader == null)
                        updateTraders.Add(trader);
                }
            }

            updateTraders = updateTraders.OrderBy(x => x.Id).ToList();

            await _traderService.UpdateTraderAsync(updateTraders);

            return Ok();
        }
    }
}