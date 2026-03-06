using App.Core.Domain.Traders;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Traders;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Traders;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Traders
{
    public partial class WorkingAreaController : BaseProtectController
    {
        private readonly IWorkingAreaService _workingAreaService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkingAreaModelFactory _workingAreaModelFactory;

        public WorkingAreaController(
            IWorkingAreaService workingAreaService,
            ILocalizationService localizationService,
            IWorkingAreaModelFactory workingAreaModelFactory)
        {
            _workingAreaService = workingAreaService;
            _localizationService = localizationService;
            _workingAreaModelFactory = workingAreaModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _workingAreaModelFactory.PrepareWorkingAreaSearchModelAsync(new WorkingAreaSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] WorkingAreaSearchModel searchModel)
        {
            //prepare model
            var model = await _workingAreaModelFactory.PrepareWorkingAreaListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _workingAreaModelFactory.PrepareWorkingAreaModelAsync(new WorkingAreaModel(), null);

            //prepare form
            var formModel = await _workingAreaModelFactory.PrepareWorkingAreaFormModelAsync(new WorkingAreaFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] WorkingAreaModel model)
        {
            if (ModelState.IsValid)
            {
                var workingArea = model.ToEntity<WorkingArea>();
                await _workingAreaService.InsertWorkingAreaAsync(workingArea);

                return Json(workingArea.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var workingArea = await _workingAreaService.GetWorkingAreaByIdAsync(id);
            if (workingArea == null)
                return await AccessDenied();

            //prepare model
            var model = await _workingAreaModelFactory.PrepareWorkingAreaModelAsync(null, workingArea);

            //prepare form
            var formModel = await _workingAreaModelFactory.PrepareWorkingAreaFormModelAsync(new WorkingAreaFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] WorkingAreaModel model)
        {
            //try to get entity with the specified id
            var workingArea = await _workingAreaService.GetWorkingAreaByIdAsync(model.Id);
            if (workingArea == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    workingArea = model.ToEntity(workingArea);
                    await _workingAreaService.UpdateWorkingAreaAsync(workingArea);

                    return Json(workingArea.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.WorkingAreas.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var workingArea = await _workingAreaService.GetWorkingAreaByIdAsync(id);
            if (workingArea == null)
                return await AccessDenied();

            try
            {
                await _workingAreaService.DeleteWorkingAreaAsync(workingArea);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.WorkingAreas.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _workingAreaService.DeleteWorkingAreaAsync((await _workingAreaService.GetWorkingAreasByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.WorkingAreas.Errors.TryToDelete");
            }
        }
    }
}