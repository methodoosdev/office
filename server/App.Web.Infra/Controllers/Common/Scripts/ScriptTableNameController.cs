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
    public partial class ScriptTableNameController : BaseProtectController
    {
        private readonly IScriptTableNameService _scriptTableNameService;
        private readonly ILocalizationService _localizationService;
        private readonly IScriptTableNameModelFactory _scriptTableNameModelFactory;

        public ScriptTableNameController(
            IScriptTableNameService scriptTableNameService,
            ILocalizationService localizationService,
            IScriptTableNameModelFactory scriptTableNameModelFactory)
        {
            _scriptTableNameService = scriptTableNameService;
            _localizationService = localizationService;
            _scriptTableNameModelFactory = scriptTableNameModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _scriptTableNameModelFactory.PrepareScriptTableNameSearchModelAsync(new ScriptTableNameSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] ScriptTableNameSearchModel searchModel)
        {
            //prepare model
            var model = await _scriptTableNameModelFactory.PrepareScriptTableNameListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _scriptTableNameModelFactory.PrepareScriptTableNameModelAsync(new ScriptTableNameModel(), null);

            //prepare form
            var formModel = await _scriptTableNameModelFactory.PrepareScriptTableNameFormModelAsync(new ScriptTableNameFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] ScriptTableNameModel model)
        {
            if (ModelState.IsValid)
            {
                var scriptTableName = model.ToEntity<ScriptTableName>();
                await _scriptTableNameService.InsertScriptTableNameAsync(scriptTableName);

                return Json(scriptTableName.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var scriptTableName = await _scriptTableNameService.GetScriptTableNameByIdAsync(id);
            if (scriptTableName == null)
                return await AccessDenied();

            //prepare model
            var model = await _scriptTableNameModelFactory.PrepareScriptTableNameModelAsync(null, scriptTableName);

            //prepare form
            var formModel = await _scriptTableNameModelFactory.PrepareScriptTableNameFormModelAsync(new ScriptTableNameFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] ScriptTableNameModel model)
        {
            //try to get entity with the specified id
            var scriptTableName = await _scriptTableNameService.GetScriptTableNameByIdAsync(model.Id);
            if (scriptTableName == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    scriptTableName = model.ToEntity(scriptTableName);
                    await _scriptTableNameService.UpdateScriptTableNameAsync(scriptTableName);

                    return Json(scriptTableName.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptTableNames.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var scriptTableName = await _scriptTableNameService.GetScriptTableNameByIdAsync(id);
            if (scriptTableName == null)
                return await AccessDenied();

            try
            {
                await _scriptTableNameService.DeleteScriptTableNameAsync(scriptTableName);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptTableNames.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _scriptTableNameService.DeleteScriptTableNameAsync((await _scriptTableNameService.GetScriptTableNamesByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.ScriptTableNames.Errors.TryToDelete");
            }
        }
    }
}