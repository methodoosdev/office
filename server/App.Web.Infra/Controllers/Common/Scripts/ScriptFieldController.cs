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
    public partial class ScriptFieldController : BaseProtectController
    {
        private readonly IScriptFieldService _scriptFieldService;
        private readonly IScriptService _scriptService;
        private readonly IScriptItemService _scriptItemService;
        private readonly IScriptPivotService _scriptPivotService;
        private readonly IScriptPivotItemService _scriptPivotItemService;
        private readonly ILocalizationService _localizationService;
        private readonly IScriptFieldModelFactory _scriptFieldModelFactory;

        public ScriptFieldController(
            IScriptFieldService scriptFieldService,
            IScriptService scriptService,
            IScriptItemService scriptItemService,
            IScriptPivotService scriptPivotService,
            IScriptPivotItemService scriptPivotItemService,
            ILocalizationService localizationService,
            IScriptFieldModelFactory scriptFieldModelFactory)
        {
            _scriptFieldService = scriptFieldService;
            _scriptService = scriptService;
            _scriptItemService = scriptItemService;
            _scriptPivotService = scriptPivotService;
            _scriptPivotItemService = scriptPivotItemService;
            _localizationService = localizationService;
            _scriptFieldModelFactory = scriptFieldModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _scriptFieldModelFactory.PrepareScriptFieldSearchModelAsync(new ScriptFieldSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ScriptFieldSearchModel searchModel, int parentId)
        {
            //prepare model
            var model = await _scriptFieldModelFactory.PrepareScriptFieldListModelAsync(searchModel, parentId);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create(int parentId)
        {
            //prepare model
            var model = await _scriptFieldModelFactory.PrepareScriptFieldModelAsync(new ScriptFieldModel(), null);
            model.TraderId = parentId;

            //prepare form
            var formModel = await _scriptFieldModelFactory.PrepareScriptFieldFormModelAsync(new ScriptFieldFormModel(), parentId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] ScriptFieldModel model)
        {
            if (ModelState.IsValid)
            {
                var scriptField = model.ToEntity<ScriptField>();
                await _scriptFieldService.InsertScriptFieldAsync(scriptField);

                return Json(scriptField.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var scriptField = await _scriptFieldService.GetScriptFieldByIdAsync(id);
            if (scriptField == null)
                return await AccessDenied();

            //prepare model
            var model = await _scriptFieldModelFactory.PrepareScriptFieldModelAsync(null, scriptField);

            //prepare form
            var formModel = await _scriptFieldModelFactory.PrepareScriptFieldFormModelAsync(new ScriptFieldFormModel(), scriptField.TraderId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] ScriptFieldModel model)
        {
            //try to get entity with the specified id
            var scriptField = await _scriptFieldService.GetScriptFieldByIdAsync(model.Id);
            if (scriptField == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    scriptField = model.ToEntity(scriptField);
                    await _scriptFieldService.UpdateScriptFieldAsync(scriptField);

                    return Json(scriptField.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptFields.Errors.TryToEdit");
            }
        }

        private async Task<int> CheckIfExistAsync(ScriptField scriptField)
        {
            var scripts = await _scriptService.GetAllScriptsAsync(scriptField.TraderId);
            foreach (var script in scripts)
            {
                var scriptItems = await _scriptItemService.GetAllScriptItemsAsync(script.Id);
                if (scriptItems.Any(x => x.ScriptFieldId == scriptField.Id))
                    return 1;
            }

            var scriptPivots = await _scriptPivotService.GetAllScriptPivotsAsync(scriptField.TraderId);
            foreach (var scriptPivot in scriptPivots)
            {
                var scriptPivotItems = await _scriptPivotItemService.GetAllScriptPivotItemsAsync(scriptPivot.Id);
                if (scriptPivotItems.Any(x => x.ScriptFieldId == scriptField.Id))
                    return 2;
            }

            return 0;
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var scriptField = await _scriptFieldService.GetScriptFieldByIdAsync(id);
            if (scriptField == null)
                return await AccessDenied();

            try
            {
                switch (await CheckIfExistAsync(scriptField))
                {
                    case 1: return await BadRequestMessageAsync("App.Models.ScriptFieldModel.Errors.ScriptItem");
                    case 2: return await BadRequestMessageAsync("App.Models.ScriptFieldModel.Errors.ScriptPivotItem");
                }

                await _scriptFieldService.DeleteScriptFieldAsync(scriptField);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptFields.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                var scriptFieldItems = (await _scriptFieldService.GetScriptFieldsByIdsAsync(selectedIds.ToArray())).ToList();
                foreach (var scriptField in scriptFieldItems)
                {
                    switch (await CheckIfExistAsync(scriptField))
                    {
                        case 1: return await BadRequestMessageAsync("App.Models.ScriptFieldModel.Errors.ScriptItem");
                        case 2: return await BadRequestMessageAsync("App.Models.ScriptFieldModel.Errors.ScriptPivotItem");
                    }
                }

                if (selectedIds != null)
                    await _scriptFieldService.DeleteScriptFieldAsync(scriptFieldItems);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptFields.Errors.TryToDelete");
            }
        }
    }
}