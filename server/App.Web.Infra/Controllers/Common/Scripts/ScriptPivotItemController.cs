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
    public partial class ScriptPivotItemController : BaseProtectController
    {
        private readonly IScriptPivotService _scriptPivotService;
        private readonly IScriptPivotItemService _scriptPivotItemService;
        private readonly ILocalizationService _localizationService;
        private readonly IScriptPivotItemModelFactory _scriptPivotItemModelFactory;

        public ScriptPivotItemController(
            IScriptPivotService scriptPivotService,
            IScriptPivotItemService scriptPivotItemService,
            ILocalizationService localizationService,
            IScriptPivotItemModelFactory scriptPivotItemModelFactory)
        {
            _scriptPivotService = scriptPivotService;
            _scriptPivotItemService = scriptPivotItemService;
            _localizationService = localizationService;
            _scriptPivotItemModelFactory = scriptPivotItemModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _scriptPivotItemModelFactory.PrepareScriptPivotItemSearchModelAsync(new ScriptPivotItemSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ScriptPivotItemSearchModel searchModel, int parentId)
        {
            //prepare model
            var model = await _scriptPivotItemModelFactory.PrepareScriptPivotItemListModelAsync(searchModel, parentId);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create(int parentId)
        {
            //prepare model
            var model = await _scriptPivotItemModelFactory.PrepareScriptPivotItemModelAsync(new ScriptPivotItemModel(), null);
            model.ScriptPivotId = parentId;

            //prepare form
            var script = await _scriptPivotService.GetScriptPivotByIdAsync(parentId);
            var formModel = await _scriptPivotItemModelFactory.PrepareScriptPivotItemFormModelAsync(new ScriptPivotItemFormModel(), parentId, script.TraderId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] ScriptPivotItemModel model)
        {
            if (ModelState.IsValid)
            {
                var scriptPivotItem = model.ToEntity<ScriptPivotItem>();
                await _scriptPivotItemService.InsertScriptPivotItemAsync(scriptPivotItem);

                return Json(scriptPivotItem.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var scriptPivotItem = await _scriptPivotItemService.GetScriptPivotItemByIdAsync(id);
            if (scriptPivotItem == null)
                return await AccessDenied();

            //prepare model
            var model = await _scriptPivotItemModelFactory.PrepareScriptPivotItemModelAsync(null, scriptPivotItem);

            //prepare form
            var scriptPivot = await _scriptPivotService.GetScriptPivotByIdAsync(scriptPivotItem.ScriptPivotId);
            var formModel = await _scriptPivotItemModelFactory.PrepareScriptPivotItemFormModelAsync(new ScriptPivotItemFormModel(), scriptPivot.Id, scriptPivot.TraderId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] ScriptPivotItemModel model)
        {
            //try to get entity with the specified id
            var scriptPivotItem = await _scriptPivotItemService.GetScriptPivotItemByIdAsync(model.Id);
            if (scriptPivotItem == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    scriptPivotItem = model.ToEntity(scriptPivotItem);
                    await _scriptPivotItemService.UpdateScriptPivotItemAsync(scriptPivotItem);

                    return Json(scriptPivotItem.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptPivotItems.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var scriptPivotItem = await _scriptPivotItemService.GetScriptPivotItemByIdAsync(id);
            if (scriptPivotItem == null)
                return await AccessDenied();

            try
            {
                await _scriptPivotItemService.DeleteScriptPivotItemAsync(scriptPivotItem);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptPivotItems.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _scriptPivotItemService.DeleteScriptPivotItemAsync((await _scriptPivotItemService.GetScriptPivotItemsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptPivotItems.Errors.TryToDelete");
            }
        }
    }
}