using App.Core.Domain.SimpleTask;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.SimpleTask;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Security;
using App.Services.SimpleTask;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.SimpleTask;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.SimpleTask
{
    public partial class SimpleTaskNatureController : BaseProtectController
    {
        private readonly ISimpleTaskNatureService _simpleTaskNatureService;
        private readonly ILocalizationService _localizationService;
        private readonly ISimpleTaskNatureModelFactory _simpleTaskNatureModelFactory;

        public SimpleTaskNatureController(
            ISimpleTaskNatureService simpleTaskNatureService,
            ILocalizationService localizationService,
            ISimpleTaskNatureModelFactory simpleTaskNatureModelFactory)
        {
            _simpleTaskNatureService = simpleTaskNatureService;
            _localizationService = localizationService;
            _simpleTaskNatureModelFactory = simpleTaskNatureModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _simpleTaskNatureModelFactory.PrepareSimpleTaskNatureSearchModelAsync(new SimpleTaskNatureSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] SimpleTaskNatureSearchModel searchModel)
        {
            //prepare model
            var model = await _simpleTaskNatureModelFactory.PrepareSimpleTaskNatureListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _simpleTaskNatureModelFactory.PrepareSimpleTaskNatureModelAsync(new SimpleTaskNatureModel(), null);

            //prepare form
            var formModel = await _simpleTaskNatureModelFactory.PrepareSimpleTaskNatureFormModelAsync(new SimpleTaskNatureFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] SimpleTaskNatureModel model)
        {
            if (ModelState.IsValid)
            {
                var simpleTaskNature = model.ToEntity<SimpleTaskNature>();
                await _simpleTaskNatureService.InsertSimpleTaskNatureAsync(simpleTaskNature);

                return Json(simpleTaskNature.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var simpleTaskNature = await _simpleTaskNatureService.GetSimpleTaskNatureByIdAsync(id);
            if (simpleTaskNature == null)
                return await AccessDenied();

            //prepare model
            var model = await _simpleTaskNatureModelFactory.PrepareSimpleTaskNatureModelAsync(null, simpleTaskNature);

            //prepare form
            var formModel = await _simpleTaskNatureModelFactory.PrepareSimpleTaskNatureFormModelAsync(new SimpleTaskNatureFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] SimpleTaskNatureModel model)
        {
            //try to get entity with the specified id
            var simpleTaskNature = await _simpleTaskNatureService.GetSimpleTaskNatureByIdAsync(model.Id);
            if (simpleTaskNature == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    simpleTaskNature = model.ToEntity(simpleTaskNature);
                    await _simpleTaskNatureService.UpdateSimpleTaskNatureAsync(simpleTaskNature);

                    return Json(simpleTaskNature.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.SimpleTaskNatures.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var simpleTaskNature = await _simpleTaskNatureService.GetSimpleTaskNatureByIdAsync(id);
            if (simpleTaskNature == null)
                return await AccessDenied();

            try
            {
                await _simpleTaskNatureService.DeleteSimpleTaskNatureAsync(simpleTaskNature);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.SimpleTaskNatures.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _simpleTaskNatureService.DeleteSimpleTaskNatureAsync((await _simpleTaskNatureService.GetSimpleTaskNaturesByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.SimpleTaskNatures.Errors.TryToDelete");
            }
        }
    }
}