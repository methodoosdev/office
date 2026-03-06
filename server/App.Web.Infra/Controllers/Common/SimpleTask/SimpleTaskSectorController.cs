using App.Core.Domain.SimpleTask;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.SimpleTask;
using App.Services.Localization;
using App.Services.SimpleTask;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.SimpleTask;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.SimpleTask
{
    public partial class SimpleTaskSectorController : BaseProtectController
    {
        private readonly ISimpleTaskSectorService _simpleTaskSectorService;
        private readonly ILocalizationService _localizationService;
        private readonly ISimpleTaskSectorModelFactory _simpleTaskSectorModelFactory;

        public SimpleTaskSectorController(
            ISimpleTaskSectorService simpleTaskSectorService,
            ILocalizationService localizationService,
            ISimpleTaskSectorModelFactory simpleTaskSectorModelFactory)
        {
            _simpleTaskSectorService = simpleTaskSectorService;
            _localizationService = localizationService;
            _simpleTaskSectorModelFactory = simpleTaskSectorModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _simpleTaskSectorModelFactory.PrepareSimpleTaskSectorSearchModelAsync(new SimpleTaskSectorSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] SimpleTaskSectorSearchModel searchModel)
        {
            //prepare model
            var model = await _simpleTaskSectorModelFactory.PrepareSimpleTaskSectorListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _simpleTaskSectorModelFactory.PrepareSimpleTaskSectorModelAsync(new SimpleTaskSectorModel(), null);

            //prepare form
            var formModel = await _simpleTaskSectorModelFactory.PrepareSimpleTaskSectorFormModelAsync(new SimpleTaskSectorFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] SimpleTaskSectorModel model)
        {
            if (ModelState.IsValid)
            {
                var simpleTaskSector = model.ToEntity<SimpleTaskSector>();
                await _simpleTaskSectorService.InsertSimpleTaskSectorAsync(simpleTaskSector);

                return Json(simpleTaskSector.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var simpleTaskSector = await _simpleTaskSectorService.GetSimpleTaskSectorByIdAsync(id);
            if (simpleTaskSector == null)
                return await AccessDenied();

            //prepare model
            var model = await _simpleTaskSectorModelFactory.PrepareSimpleTaskSectorModelAsync(null, simpleTaskSector);

            //prepare form
            var formModel = await _simpleTaskSectorModelFactory.PrepareSimpleTaskSectorFormModelAsync(new SimpleTaskSectorFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] SimpleTaskSectorModel model)
        {
            //try to get entity with the specified id
            var simpleTaskSector = await _simpleTaskSectorService.GetSimpleTaskSectorByIdAsync(model.Id);
            if (simpleTaskSector == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    simpleTaskSector = model.ToEntity(simpleTaskSector);
                    await _simpleTaskSectorService.UpdateSimpleTaskSectorAsync(simpleTaskSector);

                    return Json(simpleTaskSector.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.SimpleTaskSectors.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var simpleTaskSector = await _simpleTaskSectorService.GetSimpleTaskSectorByIdAsync(id);
            if (simpleTaskSector == null)
                return await AccessDenied();

            try
            {
                await _simpleTaskSectorService.DeleteSimpleTaskSectorAsync(simpleTaskSector);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.SimpleTaskSectors.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _simpleTaskSectorService.DeleteSimpleTaskSectorAsync((await _simpleTaskSectorService.GetSimpleTaskSectorsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.SimpleTaskSectors.Errors.TryToDelete");
            }
        }
    }
}