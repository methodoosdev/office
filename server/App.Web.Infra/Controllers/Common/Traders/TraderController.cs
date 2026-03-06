using App.Core;
using App.Core.Domain.Payroll;
using App.Core.Domain.Traders;
using App.Core.Infrastructure.Mapper;
using App.Data.DataProviders;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Traders;
using App.Services.Common;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Traders;
using App.Web.Infra.Queries.Payroll;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Traders
{
    public partial class TraderController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ILocalizationService _localizationService;
        private readonly ITraderModelFactory _traderModelFactory;
        private readonly ISqlConnectionService _connectionService;
        private readonly IAppDataProvider _dataProvider;

        public TraderController(
            ITraderService traderService,
            ITraderConnectionService traderConnectionService,
            ILocalizationService localizationService,
            ITraderModelFactory traderModelFactory,
            ISqlConnectionService connectionService,
            IAppDataProvider dataProvider)
        {
            _traderService = traderService;
            _traderConnectionService = traderConnectionService;
            _localizationService = localizationService;
            _traderModelFactory = traderModelFactory;
            _connectionService = connectionService;
            _dataProvider = dataProvider;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _traderModelFactory.PrepareTraderSearchModelAsync(new TraderSearchModel());

            var filterModel = await _traderModelFactory.PrepareTraderFilterModelAsync();
            var filterFormModel = await _traderModelFactory.PrepareTraderFilterFormModelAsync(new TraderFilterFormModel());
            var filterDefaultModel = new TraderFilterModel();

            return Json(new { searchModel, filterModel, filterFormModel, filterDefaultModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] TraderSearchModel searchModel)
        {
            //prepare model
            var model = await _traderModelFactory.PrepareTraderListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _traderModelFactory.PrepareTraderModelAsync(new TraderModel(), null);

            //prepare form
            var formModel = await _traderModelFactory.PrepareTraderFormModelAsync(new TraderFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TraderModel model)
        {
            if (ModelState.IsValid)
            {
                var trader = model.ToEntity<Trader>();
                await _traderService.InsertTraderAsync(trader);

                return Json(trader.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var trader = await _traderService.GetTraderByIdAsync(id);
            if (trader == null)
                return await AccessDenied();

            //prepare model
            var model = await _traderModelFactory.PrepareTraderModelAsync(null, trader);

            //prepare form
            var formModel = await _traderModelFactory.PrepareTraderFormModelAsync(new TraderFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] TraderModel model)
        {
            //try to get entity with the specified id
            var trader = await _traderService.GetTraderByIdAsync(model.Id);
            if (trader == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    trader = model.ToEntity(trader);
                    await _traderService.UpdateTraderAsync(trader);

                    return Json(trader.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Traders.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var trader = await _traderService.GetTraderByIdAsync(id);
            if (trader == null)
                return await AccessDenied();

            try
            {
                await _traderService.DeleteTraderAsync(trader);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Traders.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _traderService.DeleteTraderAsync((await _traderService.GetTradersByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Traders.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> CheckConnection([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                var errors = new List<TradeErrorResult>();
                if (selectedIds != null)
                {
                    foreach (var id in selectedIds.ToArray())
                    {
                        var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(id);
                        if (connectionResult.Success)
                        {
                            var trader = await _traderService.GetTraderByIdAsync(id);
                            trader.ConnectionAccountingActive = true;
                            await _traderService.UpdateTraderAsync(trader);
                        }
                        else
                        {
                            var errorMessage = await _localizationService.GetResourceAsync(connectionResult.Error);
                            errors.Add(new TradeErrorResult(connectionResult.TraderName, errorMessage));
                        }
                    }
                }

                return Json(new { errors });
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Traders.Errors.CheckConnection");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> UndoTraderDeletion([FromBody] ICollection<int> selectedIds)
        {
            var traders = await _traderService.GetTradersByIdsAsync(selectedIds.ToArray());

            foreach (var trader in traders)
                trader.Deleted = false;

            await _traderService.UpdateTraderAsync(traders);

            return Ok();

        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportPayrollIds([FromBody] ICollection<int> selectedIds)
        {
            var traders = await _traderService.GetTradersByIdsAsync(selectedIds.ToArray());

            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return BadRequest(result.Error);

            var employers = await _dataProvider.QueryAsync<EmployerLookupItem>(result.Connection, PayrollQuery.EmployerLookupItem);

            foreach (var trader in traders)
            {
                var vat = AesEncryption.Decrypt(trader.Vat);
                var employer = employers.FirstOrDefault(x => x.Vat.Equals(vat));
                if (employer != null)
                    trader.HyperPayrollId = employer.CompanyId;
            }

            await _traderService.UpdateTraderAsync(traders);

            return Ok();
        }
    }
}