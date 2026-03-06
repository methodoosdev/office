using App.Core.Domain.Scripts;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Scripts;
using App.Services.Localization;
using App.Services.Scripts;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.ScriptPivots;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.ScriptPivots
{
    public partial class ScriptPivotController : BaseProtectController
    {
        private readonly IScriptPivotService _scriptPivotService;
        private readonly ILocalizationService _localizationService;
        private readonly IScriptPivotModelFactory _scriptPivotModelFactory;

        public ScriptPivotController(
            IScriptPivotService scriptPivotService,
            ILocalizationService localizationService,
            IScriptPivotModelFactory scriptPivotModelFactory)
        {
            _scriptPivotService = scriptPivotService;
            _localizationService = localizationService;
            _scriptPivotModelFactory = scriptPivotModelFactory;
        }

        public virtual async Task<IActionResult> Config(int parentId)
        {
            //prepare model
            var configModel = await _scriptPivotModelFactory.PrepareScriptPivotConfigModelAsync(new ScriptPivotConfigModel(), parentId);

            return Json(new { configModel });
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _scriptPivotModelFactory.PrepareScriptPivotSearchModelAsync(new ScriptPivotSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ScriptPivotSearchModel searchModel, int parentId)
        {
            //prepare model
            var model = await _scriptPivotModelFactory.PrepareScriptPivotListModelAsync(searchModel, parentId);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create(int parentId)
        {
            //prepare model
            var model = await _scriptPivotModelFactory.PrepareScriptPivotModelAsync(new ScriptPivotModel(), null);
            model.TraderId = parentId;

            //prepare form
            var formModel = await _scriptPivotModelFactory.PrepareScriptPivotFormModelAsync(new ScriptPivotFormModel(), parentId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] ScriptPivotModel model)
        {
            if (ModelState.IsValid)
            {
                var scriptPivot = model.ToEntity<ScriptPivot>();
                await _scriptPivotService.InsertScriptPivotAsync(scriptPivot);

                return Json(scriptPivot.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var scriptPivot = await _scriptPivotService.GetScriptPivotByIdAsync(id);
            if (scriptPivot == null)
                return await AccessDenied();

            //prepare model
            var model = await _scriptPivotModelFactory.PrepareScriptPivotModelAsync(null, scriptPivot);

            //prepare form
            var formModel = await _scriptPivotModelFactory.PrepareScriptPivotFormModelAsync(new ScriptPivotFormModel(), scriptPivot.TraderId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] ScriptPivotModel model)
        {
            //try to get entity with the specified id
            var scriptPivot = await _scriptPivotService.GetScriptPivotByIdAsync(model.Id);
            if (scriptPivot == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    scriptPivot = model.ToEntity(scriptPivot);
                    await _scriptPivotService.UpdateScriptPivotAsync(scriptPivot);

                    return Json(scriptPivot.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptPivots.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var scriptPivot = await _scriptPivotService.GetScriptPivotByIdAsync(id);
            if (scriptPivot == null)
                return await AccessDenied();

            try
            {
                await _scriptPivotService.DeleteScriptPivotAsync(scriptPivot);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptPivots.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _scriptPivotService.DeleteScriptPivotAsync((await _scriptPivotService.GetScriptPivotsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptPivots.Errors.TryToDelete");
            }
        }
    }
}