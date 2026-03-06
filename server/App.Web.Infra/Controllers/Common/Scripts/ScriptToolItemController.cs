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
    public partial class ScriptToolItemController : BaseProtectController
    {
        private readonly IScriptToolService _scriptToolService;
        private readonly IScriptToolItemService _scriptToolItemService;
        private readonly ILocalizationService _localizationService;
        private readonly IScriptToolItemModelFactory _scriptToolItemModelFactory;

        public ScriptToolItemController(
            IScriptToolService scriptToolService,
            IScriptToolItemService scriptToolItemService,
            ILocalizationService localizationService,
            IScriptToolItemModelFactory scriptToolItemModelFactory)
        {
            _scriptToolService = scriptToolService;
            _scriptToolItemService = scriptToolItemService;
            _localizationService = localizationService;
            _scriptToolItemModelFactory = scriptToolItemModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _scriptToolItemModelFactory.PrepareScriptToolItemSearchModelAsync(new ScriptToolItemSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ScriptToolItemSearchModel searchModel, int parentId)
        {
            //prepare model
            var scriptTool = await _scriptToolService.GetScriptToolByIdAsync(parentId);
            var model = await _scriptToolItemModelFactory.PrepareScriptToolItemListModelAsync(searchModel, parentId, scriptTool.TraderId);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create(int parentId)
        {
            //prepare model
            var model = await _scriptToolItemModelFactory.PrepareScriptToolItemModelAsync(new ScriptToolItemModel(), null);
            model.ScriptToolId = parentId;

            //prepare form
            var scriptTool = await _scriptToolService.GetScriptToolByIdAsync(parentId);
            var formModel = await _scriptToolItemModelFactory.PrepareScriptToolItemFormModelAsync(new ScriptToolItemFormModel(), parentId, scriptTool.TraderId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] ScriptToolItemModel model)
        {
            if (ModelState.IsValid)
            {
                var scriptToolItem = model.ToEntity<ScriptToolItem>();
                await _scriptToolItemService.InsertScriptToolItemAsync(scriptToolItem);

                return Json(scriptToolItem.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var scriptToolItem = await _scriptToolItemService.GetScriptToolItemByIdAsync(id);
            if (scriptToolItem == null)
                return await AccessDenied();

            //prepare model
            var model = await _scriptToolItemModelFactory.PrepareScriptToolItemModelAsync(null, scriptToolItem);

            //prepare form
            var scriptTool = await _scriptToolService.GetScriptToolByIdAsync(scriptToolItem.ScriptToolId);
            var formModel = await _scriptToolItemModelFactory.PrepareScriptToolItemFormModelAsync(new ScriptToolItemFormModel(), scriptTool.Id, scriptTool.TraderId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] ScriptToolItemModel model)
        {
            //try to get entity with the specified id
            var scriptToolItem = await _scriptToolItemService.GetScriptToolItemByIdAsync(model.Id);
            if (scriptToolItem == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    scriptToolItem = model.ToEntity(scriptToolItem);
                    await _scriptToolItemService.UpdateScriptToolItemAsync(scriptToolItem);

                    return Json(scriptToolItem.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptToolItems.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var scriptToolItem = await _scriptToolItemService.GetScriptToolItemByIdAsync(id);
            if (scriptToolItem == null)
                return await AccessDenied();

            try
            {
                await _scriptToolItemService.DeleteScriptToolItemAsync(scriptToolItem);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptToolItems.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _scriptToolItemService.DeleteScriptToolItemAsync((await _scriptToolItemService.GetScriptToolItemsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptToolItems.Errors.TryToDelete");
            }
        }
    }
}