using App.Models.Accounting;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Accounting;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Accounting
{
    public partial class PayoffLiabilitiesController : BaseProtectController
    {
        private readonly IPayoffLiabilitiesFactory _payoffLiabilitiesFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ITraderConnectionService _traderConnectionService;

        public PayoffLiabilitiesController(
            IPayoffLiabilitiesFactory payoffLiabilitiesFactory,
            ILocalizationService localizationService,
            ITraderConnectionService traderConnectionService)
        {
            _payoffLiabilitiesFactory = payoffLiabilitiesFactory;
            _localizationService = localizationService;
            _traderConnectionService = traderConnectionService;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _payoffLiabilitiesFactory.PreparePayoffLiabilitySearchModelAsync(new PayoffLiabilitySearchModel());

            //prepare model
            var tableModel = await _payoffLiabilitiesFactory.PreparePayoffLiabilityTableModelAsync(new PayoffLiabilityTableModel());

            //prepare model
            var factorTableModel = await _payoffLiabilitiesFactory.PreparePayoffLiabilityFactorTableModelAsync(new PayoffLiabilityFactorTableModel());

            return Json(new { searchModel, tableModel, factorTableModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] PayoffLiabilitySearchModel searchModel)
        {
            var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(searchModel.TraderId);
            if (!connectionResult.Success)
                return await BadRequestMessageAsync(connectionResult.Error);

            if (connectionResult.CategoryBookTypeId != (int)Core.Domain.Traders.CategoryBookType.C)
                return await BadRequestMessageAsync(await _localizationService.GetResourceAsync("App.Models.PayoffLiabilityModel.Errors.OnlyCategoryBookTypeC")); ;

            var model = await _payoffLiabilitiesFactory.PreparePayoffLiabilityListAsync(connectionResult, searchModel.Period.Year);

            return Json(new { model.list, model.factorList });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ExportToPdf(int traderId, int year)
        {
            return Ok();
        }
    }
}