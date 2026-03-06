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
    public partial class ScriptTableItemController : BaseProtectController
    {
        private readonly IScriptTableService _scriptTableService;
        private readonly IScriptTableItemService _scriptTableItemService;
        private readonly ILocalizationService _localizationService;
        private readonly IScriptTableItemModelFactory _scriptTableItemModelFactory;

        public ScriptTableItemController(
            IScriptTableService scriptTableService,
            IScriptTableItemService scriptTableItemService,
            ILocalizationService localizationService,
            IScriptTableItemModelFactory scriptTableItemModelFactory)
        {
            _scriptTableService = scriptTableService;
            _scriptTableItemService = scriptTableItemService;
            _localizationService = localizationService;
            _scriptTableItemModelFactory = scriptTableItemModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _scriptTableItemModelFactory.PrepareScriptTableItemSearchModelAsync(new ScriptTableItemSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ScriptTableItemSearchModel searchModel, int parentId)
        {
            //prepare model
            var model = await _scriptTableItemModelFactory.PrepareScriptTableItemListModelAsync(searchModel, parentId);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create(int parentId)
        {
            //prepare model
            var model = await _scriptTableItemModelFactory.PrepareScriptTableItemModelAsync(new ScriptTableItemModel(), null);
            model.ScriptTableId = parentId;

            //prepare form
            var scriptTable = await _scriptTableService.GetScriptTableByIdAsync(parentId);
            var formModel = await _scriptTableItemModelFactory.PrepareScriptTableItemFormModelAsync(new ScriptTableItemFormModel(), scriptTable.TraderId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] ScriptTableItemModel model)
        {
            if (ModelState.IsValid)
            {
                var scriptTableItem = model.ToEntity<ScriptTableItem>();
                await _scriptTableItemService.InsertScriptTableItemAsync(scriptTableItem);

                return Json(scriptTableItem.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var scriptTableItem = await _scriptTableItemService.GetScriptTableItemByIdAsync(id);
            if (scriptTableItem == null)
                return await AccessDenied();

            //prepare model
            var model = await _scriptTableItemModelFactory.PrepareScriptTableItemModelAsync(null, scriptTableItem);

            //prepare form
            var scriptTable = await _scriptTableService.GetScriptTableByIdAsync(scriptTableItem.ScriptTableId);
            var formModel = await _scriptTableItemModelFactory.PrepareScriptTableItemFormModelAsync(new ScriptTableItemFormModel(), scriptTable.TraderId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] ScriptTableItemModel model)
        {
            //try to get entity with the specified id
            var scriptTableItem = await _scriptTableItemService.GetScriptTableItemByIdAsync(model.Id);
            if (scriptTableItem == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    scriptTableItem = model.ToEntity(scriptTableItem);
                    await _scriptTableItemService.UpdateScriptTableItemAsync(scriptTableItem);

                    return Json(scriptTableItem.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptTableItems.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var scriptTableItem = await _scriptTableItemService.GetScriptTableItemByIdAsync(id);
            if (scriptTableItem == null)
                return await AccessDenied();

            try
            {
                await _scriptTableItemService.DeleteScriptTableItemAsync(scriptTableItem);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptTableItems.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _scriptTableItemService.DeleteScriptTableItemAsync((await _scriptTableItemService.GetScriptTableItemsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptTableItems.Errors.TryToDelete");
            }
        }
    }
}