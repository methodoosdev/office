using App.Core.Domain.Scripts;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Scripts;
using App.Services.Localization;
using App.Services.Scripts;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Scripts;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Scripts
{
    public partial class ScriptController : BaseProtectController
    {
        private readonly IScriptService _scriptService;
        private readonly IScriptToolItemService _scriptToolItemService;
        private readonly ILocalizationService _localizationService;
        private readonly IScriptModelFactory _scriptModelFactory;

        public ScriptController(
            IScriptService scriptService,
            IScriptToolItemService scriptToolItemService,
            ILocalizationService localizationService,
            IScriptModelFactory scriptModelFactory)
        {
            _scriptService = scriptService;
            _scriptToolItemService = scriptToolItemService;
            _localizationService = localizationService;
            _scriptModelFactory = scriptModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _scriptModelFactory.PrepareScriptSearchModelAsync(new ScriptSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ScriptSearchModel searchModel, int parentId)
        {
            //prepare model
            var model = await _scriptModelFactory.PrepareScriptListModelAsync(searchModel, parentId);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create(int parentId)
        {
            //prepare model
            var model = await _scriptModelFactory.PrepareScriptModelAsync(new ScriptModel(), null);
            model.TraderId = parentId;

            //prepare form
            var formModel = await _scriptModelFactory.PrepareScriptFormModelAsync(new ScriptFormModel(), parentId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] ScriptModel model)
        {
            if (ModelState.IsValid)
            {
                var script = model.ToEntity<Script>();
                await _scriptService.InsertScriptAsync(script);

                return Json(script.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var script = await _scriptService.GetScriptByIdAsync(id);
            if (script == null)
                return await AccessDenied();

            //prepare model
            var model = await _scriptModelFactory.PrepareScriptModelAsync(null, script);

            //prepare form
            var formModel = await _scriptModelFactory.PrepareScriptFormModelAsync(new ScriptFormModel(), script.TraderId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] ScriptModel model)
        {
            //try to get entity with the specified id
            var script = await _scriptService.GetScriptByIdAsync(model.Id);
            if (script == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    script = model.ToEntity(script);
                    await _scriptService.UpdateScriptAsync(script);

                    return Json(script.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Scripts.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var script = await _scriptService.GetScriptByIdAsync(id);
            if (script == null)
                return await AccessDenied();

            try
            {
                var scriptToolItems = await _scriptToolItemService.Table.Where(x => x.ScriptId == script.Id).ToListAsync();
                if (scriptToolItems.Any(x => x.ScriptId == script.Id))
                    return await BadRequestMessageAsync("App.Models.ScriptModel.Errors.ScriptToolItem");

                await _scriptService.DeleteScriptAsync(script);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Scripts.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                {
                    var scripts = await _scriptService.GetScriptsByIdsAsync(selectedIds.ToArray());

                    foreach (var script in scripts)
                    {
                        var scriptToolItems = await _scriptToolItemService.Table.Where(x => x.ScriptId == script.Id).ToListAsync();
                        if (scriptToolItems.Any(x => x.ScriptId == script.Id))
                            return await BadRequestMessageAsync("App.Models.ScriptModel.Errors.ScriptToolItem");
                    }

                    await _scriptService.DeleteScriptAsync(scripts);
                }

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Scripts.Errors.TryToDelete");
            }
        }
    }
}