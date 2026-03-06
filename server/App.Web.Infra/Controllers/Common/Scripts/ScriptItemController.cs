using App.Core.Domain.Scripts;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Scripts;
using App.Services.Localization;
using App.Services.Scripts;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Scripts;
using Microsoft.AspNetCore.Mvc;
using NPOI.POIFS.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Scripts
{
    public partial class ScriptItemController : BaseProtectController
    {
        private readonly IScriptService _scriptService;
        private readonly IScriptItemService _scriptItemService;
        private readonly ILocalizationService _localizationService;
        private readonly IScriptItemModelFactory _scriptItemModelFactory;

        public ScriptItemController(
            IScriptService scriptService,
            IScriptItemService scriptItemService,
            ILocalizationService localizationService,
            IScriptItemModelFactory scriptItemModelFactory)
        {
            _scriptService = scriptService;
            _scriptItemService = scriptItemService;
            _localizationService = localizationService;
            _scriptItemModelFactory = scriptItemModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _scriptItemModelFactory.PrepareScriptItemSearchModelAsync(new ScriptItemSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ScriptItemSearchModel searchModel, int parentId)
        {
            //prepare model
            var script = await _scriptService.GetScriptByIdAsync(parentId);
            var model = await _scriptItemModelFactory.PrepareScriptItemListModelAsync(searchModel, parentId, script.TraderId);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create(int parentId)
        {
            //prepare model
            var model = await _scriptItemModelFactory.PrepareScriptItemModelAsync(new ScriptItemModel(), null);
            model.ScriptId = parentId;

            //prepare form
            var script = await _scriptService.GetScriptByIdAsync(parentId);
            var formModel = await _scriptItemModelFactory.PrepareScriptItemFormModelAsync(new ScriptItemFormModel(), parentId, script.TraderId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] ScriptItemModel model)
        {
            if (ModelState.IsValid)
            {
                var scriptItem = model.ToEntity<ScriptItem>();
                await _scriptItemService.InsertScriptItemAsync(scriptItem);

                return Json(scriptItem.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var scriptItem = await _scriptItemService.GetScriptItemByIdAsync(id);
            if (scriptItem == null)
                return await AccessDenied();

            //prepare model
            var model = await _scriptItemModelFactory.PrepareScriptItemModelAsync(null, scriptItem);

            //prepare form
            var script = await _scriptService.GetScriptByIdAsync(scriptItem.ScriptId);
            var formModel = await _scriptItemModelFactory.PrepareScriptItemFormModelAsync(new ScriptItemFormModel(), script.Id, script.TraderId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] ScriptItemModel model)
        {
            //try to get entity with the specified id
            var scriptItem = await _scriptItemService.GetScriptItemByIdAsync(model.Id);
            if (scriptItem == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    scriptItem = model.ToEntity(scriptItem);
                    await _scriptItemService.UpdateScriptItemAsync(scriptItem);

                    return Json(scriptItem.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptItems.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var scriptItem = await _scriptItemService.GetScriptItemByIdAsync(id);
            if (scriptItem == null)
                return await AccessDenied();

            try
            {
                await _scriptItemService.DeleteScriptItemAsync(scriptItem);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptItems.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _scriptItemService.DeleteScriptItemAsync((await _scriptItemService.GetScriptItemsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptItems.Errors.TryToDelete");
            }
        }
    }
}