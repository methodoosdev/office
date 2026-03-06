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
    public partial class ScriptGroupController : BaseProtectController
    {
        private readonly IScriptGroupService _scriptGroupService;
        private readonly IScriptService _scriptService;
        private readonly IScriptFieldService _scriptFieldService;
        private readonly IScriptPivotService _scriptPivotService;
        private readonly IScriptTableService _scriptTableService;
        private readonly ILocalizationService _localizationService;
        private readonly IScriptGroupModelFactory _scriptGroupModelFactory;

        public ScriptGroupController(
            IScriptGroupService scriptGroupService,
            IScriptService scriptService,
            IScriptFieldService scriptFieldService,
            IScriptPivotService scriptPivotService,
            IScriptTableService scriptTableService,
            ILocalizationService localizationService,
            IScriptGroupModelFactory scriptGroupModelFactory)
        {
            _scriptGroupService = scriptGroupService;
            _scriptService = scriptService;
            _scriptFieldService = scriptFieldService;
            _scriptPivotService = scriptPivotService;
            _scriptTableService = scriptTableService;
            _localizationService = localizationService;
            _scriptGroupModelFactory = scriptGroupModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _scriptGroupModelFactory.PrepareScriptGroupSearchModelAsync(new ScriptGroupSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ScriptGroupSearchModel searchModel, int parentId)
        {
            //prepare model
            var model = await _scriptGroupModelFactory.PrepareScriptGroupListModelAsync(searchModel, parentId);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create(int parentId)
        {
            //prepare model
            var model = await _scriptGroupModelFactory.PrepareScriptGroupModelAsync(new ScriptGroupModel(), null);
            model.TraderId = parentId;

            //prepare form
            var formModel = await _scriptGroupModelFactory.PrepareScriptGroupFormModelAsync(new ScriptGroupFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] ScriptGroupModel model)
        {
            if (ModelState.IsValid)
            {
                var scriptGroup = model.ToEntity<ScriptGroup>();
                await _scriptGroupService.InsertScriptGroupAsync(scriptGroup);

                return Json(scriptGroup.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var scriptGroup = await _scriptGroupService.GetScriptGroupByIdAsync(id);
            if (scriptGroup == null)
                return await AccessDenied();

            //prepare model
            var model = await _scriptGroupModelFactory.PrepareScriptGroupModelAsync(null, scriptGroup);

            //prepare form
            var formModel = await _scriptGroupModelFactory.PrepareScriptGroupFormModelAsync(new ScriptGroupFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] ScriptGroupModel model)
        {
            //try to get entity with the specified id
            var scriptGroup = await _scriptGroupService.GetScriptGroupByIdAsync(model.Id);
            if (scriptGroup == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    scriptGroup = model.ToEntity(scriptGroup);
                    await _scriptGroupService.UpdateScriptGroupAsync(scriptGroup);

                    return Json(scriptGroup.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptGroups.Errors.TryToEdit");
            }
        }

        private async Task<(bool Exist, string Message)> CheckScriptGroupExists(int scriptGroupId)
        {
            if (await _scriptService.Table.AnyAsync(x => x.ScriptGroupId == scriptGroupId))
                return (true, "App.Models.ScriptGroupModel.Errors.Script");
            if (await _scriptFieldService.Table.AnyAsync(x => x.ScriptGroupId == scriptGroupId))
                return (true, "App.Models.ScriptGroupModel.Errors.ScriptField");
            if (await _scriptPivotService.Table.AnyAsync(x => x.ScriptGroupId == scriptGroupId))
                return (true, "App.Models.ScriptGroupModel.Errors.ScriptPivot");
            if (await _scriptTableService.Table.AnyAsync(x => x.ScriptGroupId == scriptGroupId))
                return (true, "App.Models.ScriptGroupModel.Errors.ScriptTable");

            return (false, string.Empty);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var scriptGroup = await _scriptGroupService.GetScriptGroupByIdAsync(id);
            if (scriptGroup == null)
                return await AccessDenied();

            try
            {
                var result = await CheckScriptGroupExists(scriptGroup.Id);
                if (result.Exist)
                    return BadRequest(await _localizationService.GetResourceAsync(result.Message));

                await _scriptGroupService.DeleteScriptGroupAsync(scriptGroup);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptGroups.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                {
                    var scriptGroups = await _scriptGroupService.GetScriptGroupsByIdsAsync(selectedIds.ToArray());
                    foreach (var scriptGroup in scriptGroups)
                    {
                        var result = await CheckScriptGroupExists(scriptGroup.Id);
                        if (result.Exist)
                            return BadRequest(await _localizationService.GetResourceAsync(result.Message));
                    }

                    await _scriptGroupService.DeleteScriptGroupAsync(scriptGroups);
                }

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptGroups.Errors.TryToDelete");
            }
        }
    }
}