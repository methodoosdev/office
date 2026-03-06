using App.Models.Banking;
using App.Models.Traders;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Banking;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Banking
{
    public partial class BankingTraderController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly ILocalizationService _localizationService;
        private readonly IBankingTraderModelFactory _bankingTraderModelFactory;

        public BankingTraderController(
            ITraderService traderService,
            ILocalizationService localizationService,
            IBankingTraderModelFactory bankingTraderModelFactory)
        {
            _traderService = traderService;
            _localizationService = localizationService;
            _bankingTraderModelFactory = bankingTraderModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _bankingTraderModelFactory.PrepareScriptTraderSearchModelAsync(new TraderSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] TraderSearchModel searchModel)
        {
            //prepare model
            var model = await _bankingTraderModelFactory.PrepareScriptTraderListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var trader = await _traderService.GetTraderByIdAsync(id);
            if (trader == null)
                return await AccessDenied();

            //prepare model
            var model = await _bankingTraderModelFactory.PrepareTraderModelAsync(null, trader);

            //prepare form
            var formModel = await _bankingTraderModelFactory.PrepareScriptTraderFormModelAsync(new TraderFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] TraderModel model)
        {
            //try to get entity with the specified id
            var trader = await _traderService.GetTraderByIdAsync(model.Id);
            if (trader == null)
                return await AccessDenied();

            return Json(model.Id);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Account(string bankBIC, string clientUserId, string resourceId, DateTime dateFrom, DateTime dateTo)
        {
            //prepare model
            var searchModel = await _bankingTraderModelFactory.PrepareAccountTransactionSearchModelAsync(new AccountTransactionSearchModel());

            //prepare model
            var model = await _bankingTraderModelFactory.PrepareAccountTransactionsAsync(searchModel, bankBIC, clientUserId, resourceId, dateFrom, dateTo);

            return Json(model);
        }

    }
}