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
    public partial class ScriptTableController : BaseProtectController
    {
        private readonly IScriptTableService _scriptTableService;
        private readonly IScriptFieldService _scriptFieldService;
        private readonly ILocalizationService _localizationService;
        private readonly IScriptTableModelFactory _scriptTableModelFactory;

        public ScriptTableController(
            IScriptTableService scriptTableService,
            IScriptFieldService scriptFieldService,
            ILocalizationService localizationService,
            IScriptTableModelFactory scriptTableModelFactory)
        {
            _scriptTableService = scriptTableService;
            _scriptFieldService = scriptFieldService;
            _localizationService = localizationService;
            _scriptTableModelFactory = scriptTableModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _scriptTableModelFactory.PrepareScriptTableSearchModelAsync(new ScriptTableSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ScriptTableSearchModel searchModel, int parentId)
        {
            //prepare model
            var model = await _scriptTableModelFactory.PrepareScriptTableListModelAsync(searchModel, parentId);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create(int parentId)
        {
            //prepare model
            var model = await _scriptTableModelFactory.PrepareScriptTableModelAsync(new ScriptTableModel(), null);
            model.TraderId = parentId;

            //prepare form
            var formModel = await _scriptTableModelFactory.PrepareScriptTableFormModelAsync(new ScriptTableFormModel(), parentId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] ScriptTableModel model)
        {
            if (ModelState.IsValid)
            {
                var scriptTable = model.ToEntity<ScriptTable>();
                await _scriptTableService.InsertScriptTableAsync(scriptTable);

                return Json(scriptTable.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var scriptTable = await _scriptTableService.GetScriptTableByIdAsync(id);
            if (scriptTable == null)
                return await AccessDenied();

            //prepare model
            var model = await _scriptTableModelFactory.PrepareScriptTableModelAsync(null, scriptTable);

            //prepare form
            var formModel = await _scriptTableModelFactory.PrepareScriptTableFormModelAsync(new ScriptTableFormModel(), scriptTable.TraderId);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] ScriptTableModel model)
        {
            //try to get entity with the specified id
            var scriptTable = await _scriptTableService.GetScriptTableByIdAsync(model.Id);
            if (scriptTable == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    scriptTable = model.ToEntity(scriptTable);
                    await _scriptTableService.UpdateScriptTableAsync(scriptTable);

                    return Json(scriptTable.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptTables.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var scriptTable = await _scriptTableService.GetScriptTableByIdAsync(id);
            if (scriptTable == null)
                return await AccessDenied();

            try
            {
                var exists = await _scriptFieldService.Table.AnyAsync(x => x.ScriptTableId == scriptTable.Id);
                if (exists)
                    return await BadRequestMessageAsync("App.Models.ScriptTableModel.Errors.ScriptField");

                await _scriptTableService.DeleteScriptTableAsync(scriptTable);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptTables.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                {
                    var scriptTables = (await _scriptTableService.GetScriptTablesByIdsAsync(selectedIds.ToArray())).ToList();
                    foreach (var scriptTable in scriptTables)
                    {
                        var exists = await _scriptFieldService.Table.AnyAsync(x => x.ScriptTableId == scriptTable.Id);
                        if (exists)
                            return await BadRequestMessageAsync("App.Models.ScriptTableModel.Errors.ScriptField");
                    }

                    await _scriptTableService.DeleteScriptTableAsync(scriptTables);
                }

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptTables.Errors.TryToDelete");
            }
        }
    }
}