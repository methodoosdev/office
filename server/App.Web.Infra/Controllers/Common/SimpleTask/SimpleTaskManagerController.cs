using App.Core.Domain.SimpleTask;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.SimpleTask;
using App.Services.Localization;
using App.Services.Offices;
using App.Services.SimpleTask;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.SimpleTask;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.SimpleTask
{
    public partial class SimpleTaskManagerController : BaseProtectController
    {
        private readonly ISimpleTaskManagerService _simpleTaskManagerService;
        private readonly IPersistStateService _persistStateService;
        private readonly ILocalizationService _localizationService;
        private readonly ISimpleTaskManagerModelFactory _simpleTaskManagerModelFactory;

        public SimpleTaskManagerController(
            ISimpleTaskManagerService simpleTaskManagerService,
            IPersistStateService persistStateService,
            ILocalizationService localizationService,
            ISimpleTaskManagerModelFactory simpleTaskManagerModelFactory)
        {
            _simpleTaskManagerService = simpleTaskManagerService;
            _persistStateService = persistStateService;
            _localizationService = localizationService;
            _simpleTaskManagerModelFactory = simpleTaskManagerModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _simpleTaskManagerModelFactory.PrepareSimpleTaskManagerSearchModelAsync(new SimpleTaskManagerSearchModel());

            var filterModel = (await _persistStateService.GetModelInstance<SimpleTaskManagerFilterModel>()).Model;
            var filterFormModel = await _simpleTaskManagerModelFactory.PrepareSimpleTaskManagerFilterFormModelAsync(new SimpleTaskManagerFilterFormModel());
            var filterDefaultModel = new SimpleTaskManagerFilterModel();

            return Json(new { searchModel, filterModel, filterFormModel, filterDefaultModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] SimpleTaskManagerSearchModel searchModel)
        {
            //prepare model
            var model = await _simpleTaskManagerModelFactory.PrepareSimpleTaskManagerListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _simpleTaskManagerModelFactory.PrepareSimpleTaskManagerModelAsync(new SimpleTaskManagerModel(), null);

            //prepare form
            var formModel = await _simpleTaskManagerModelFactory.PrepareSimpleTaskManagerFormModelAsync(new SimpleTaskManagerFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] SimpleTaskManagerModel model)
        {
            if (ModelState.IsValid)
            {
                var simpleTaskManager = model.ToEntity<SimpleTaskManager>();
                await _simpleTaskManagerService.InsertSimpleTaskManagerAsync(simpleTaskManager);

                return Json(simpleTaskManager.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var simpleTaskManager = await _simpleTaskManagerService.GetSimpleTaskManagerByIdAsync(id);
            if (simpleTaskManager == null)
                return await AccessDenied();

            //prepare model
            var model = await _simpleTaskManagerModelFactory.PrepareSimpleTaskManagerModelAsync(null, simpleTaskManager);

            //prepare form
            var formModel = await _simpleTaskManagerModelFactory.PrepareSimpleTaskManagerFormModelAsync(new SimpleTaskManagerFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] SimpleTaskManagerModel model)
        {
            //try to get entity with the specified id
            var simpleTaskManager = await _simpleTaskManagerService.GetSimpleTaskManagerByIdAsync(model.Id);
            if (simpleTaskManager == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    simpleTaskManager = model.ToEntity(simpleTaskManager);
                    await _simpleTaskManagerService.UpdateSimpleTaskManagerAsync(simpleTaskManager);

                    return Json(simpleTaskManager.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.SimpleTaskManagers.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var simpleTaskManager = await _simpleTaskManagerService.GetSimpleTaskManagerByIdAsync(id);
            if (simpleTaskManager == null)
                return await AccessDenied();

            try
            {
                await _simpleTaskManagerService.DeleteSimpleTaskManagerAsync(simpleTaskManager);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.SimpleTaskManagers.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _simpleTaskManagerService.DeleteSimpleTaskManagerAsync((await _simpleTaskManagerService.GetSimpleTaskManagersByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.SimpleTaskManagers.Errors.TryToDelete");
            }
        }
    }
}