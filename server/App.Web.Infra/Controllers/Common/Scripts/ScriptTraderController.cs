using App.Models.Scripts;
using App.Models.Traders;
using App.Services.Localization;
using App.Services.Scripts;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Scripts;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Scripts
{
    public partial class ScriptTraderController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly ILocalizationService _localizationService;
        private readonly IScriptTraderModelFactory _scriptTraderModelFactory;
        private readonly IScriptCloneByTraderService _scriptCloneByTraderService;

        public ScriptTraderController(
            ITraderService traderService,
            ILocalizationService localizationService,
            IScriptTraderModelFactory scriptTraderModelFactory,
            IScriptCloneByTraderService scriptCloneByTraderService)
        {
            _traderService = traderService;
            _localizationService = localizationService;
            _scriptTraderModelFactory = scriptTraderModelFactory;
            _scriptCloneByTraderService = scriptCloneByTraderService;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _scriptTraderModelFactory.PrepareScriptTraderSearchModelAsync(new TraderSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] TraderSearchModel searchModel)
        {
            //prepare model
            var model = await _scriptTraderModelFactory.PrepareScriptTraderListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var trader = await _traderService.GetTraderByIdAsync(id);
            if (trader == null)
                return await AccessDenied();

            //prepare model
            var model = await _scriptTraderModelFactory.PrepareTraderModelAsync(null, trader);

            //prepare form
            var formModel = await _scriptTraderModelFactory.PrepareScriptTraderFormModelAsync(new TraderFormModel());

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
        public virtual async Task<IActionResult> Print([FromBody] ICollection<string> groups, int traderId, int categoryBookTypeId, string config)
        {
            //try to get entity with the specified id
            var trader = await _traderService.GetTraderByIdAsync(traderId);
            if (trader == null)
                return await AccessDenied();

            var traderName = trader.ToTraderFullName();

            var configValue = Newtonsoft.Json.JsonConvert.DeserializeObject<ScriptToolConfigModel>(config);
            //prepare model
            var model = await _scriptTraderModelFactory.PrepareScriptReportAsync(groups.ToList(), traderId, categoryBookTypeId, traderName, configValue);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Pivot([FromBody] ScriptTraderModel model)
        {
            //try to get entity with the specified id
            var trader = await _traderService.GetTraderByIdAsync(model.TraderId);
            if (trader == null)
                return await AccessDenied();

            var traderName = trader.ToTraderFullName();

            //prepare model
            model.TraderName = traderName;
            var list = await _scriptTraderModelFactory.PrepareScriptPivotAsync(model);

            return Json(list);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ToolReport(int id, int traderId, string config)
        {
            //try to get entity with the specified id
            var _trader = await _traderService.GetTraderByIdAsync(traderId);
            if (_trader == null)
                return await AccessDenied();

            var trader = _trader.ToTraderModel();

            var info = new ScriptCustomerInfo
            {
                Address = trader.JobAddress,
                City = trader.JobCity,
                Phone = trader.JobPhoneNumber1,
                Postcode = trader.JobPostcode,
                Email = trader.Email,
                TraderName = _trader.ToTraderFullName()
            };

            var configValue = Newtonsoft.Json.JsonConvert.DeserializeObject<ScriptToolConfigModel>(config);
            //prepare model
            var model = await _scriptTraderModelFactory.PrepareScriptToolAsync(id, traderId, configValue);

            return Json(new { info, model.Title, model.Data });
        }

        [HttpPost]
        public virtual async Task<IActionResult> ToolDiagram(int id, int traderId)
        {
            //try to get entity with the specified id
            var trader = await _traderService.GetTraderByIdAsync(traderId);
            if (trader == null)
                return await AccessDenied();

            //prepare model
            var model = await _scriptTraderModelFactory.PrepareDiagramAsync(id, traderId);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> CloneScripts(int sourceTraderId, int targetTraderId)
        {
            await _scriptCloneByTraderService.ClearAsync(targetTraderId);
            await _scriptCloneByTraderService.CloneAsync(sourceTraderId, targetTraderId);

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteScripts(int targetTraderId)
        {
            await _scriptCloneByTraderService.ClearAsync(targetTraderId);

            return Ok();
        }
    }
}