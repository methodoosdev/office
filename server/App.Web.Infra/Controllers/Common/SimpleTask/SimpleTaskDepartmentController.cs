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
    public partial class SimpleTaskDepartmentController : BaseProtectController
    {
        private readonly ISimpleTaskDepartmentService _simpleTaskDepartmentService;
        private readonly ILocalizationService _localizationService;
        private readonly ISimpleTaskDepartmentModelFactory _simpleTaskDepartmentModelFactory;

        public SimpleTaskDepartmentController(
            ISimpleTaskDepartmentService simpleTaskDepartmentService,
            ILocalizationService localizationService,
            ISimpleTaskDepartmentModelFactory simpleTaskDepartmentModelFactory)
        {
            _simpleTaskDepartmentService = simpleTaskDepartmentService;
            _localizationService = localizationService;
            _simpleTaskDepartmentModelFactory = simpleTaskDepartmentModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _simpleTaskDepartmentModelFactory.PrepareSimpleTaskDepartmentSearchModelAsync(new SimpleTaskDepartmentSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] SimpleTaskDepartmentSearchModel searchModel)
        {
            //prepare model
            var model = await _simpleTaskDepartmentModelFactory.PrepareSimpleTaskDepartmentListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _simpleTaskDepartmentModelFactory.PrepareSimpleTaskDepartmentModelAsync(new SimpleTaskDepartmentModel(), null);

            //prepare form
            var formModel = await _simpleTaskDepartmentModelFactory.PrepareSimpleTaskDepartmentFormModelAsync(new SimpleTaskDepartmentFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] SimpleTaskDepartmentModel model)
        {
            if (ModelState.IsValid)
            {
                var simpleTaskDepartment = model.ToEntity<SimpleTaskDepartment>();
                await _simpleTaskDepartmentService.InsertSimpleTaskDepartmentAsync(simpleTaskDepartment);

                return Json(simpleTaskDepartment.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var simpleTaskDepartment = await _simpleTaskDepartmentService.GetSimpleTaskDepartmentByIdAsync(id);
            if (simpleTaskDepartment == null)
                return await AccessDenied();

            //prepare model
            var model = await _simpleTaskDepartmentModelFactory.PrepareSimpleTaskDepartmentModelAsync(null, simpleTaskDepartment);

            //prepare form
            var formModel = await _simpleTaskDepartmentModelFactory.PrepareSimpleTaskDepartmentFormModelAsync(new SimpleTaskDepartmentFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] SimpleTaskDepartmentModel model)
        {
            //try to get entity with the specified id
            var simpleTaskDepartment = await _simpleTaskDepartmentService.GetSimpleTaskDepartmentByIdAsync(model.Id);
            if (simpleTaskDepartment == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    simpleTaskDepartment = model.ToEntity(simpleTaskDepartment);
                    await _simpleTaskDepartmentService.UpdateSimpleTaskDepartmentAsync(simpleTaskDepartment);

                    return Json(simpleTaskDepartment.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.SimpleTaskDepartments.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var simpleTaskDepartment = await _simpleTaskDepartmentService.GetSimpleTaskDepartmentByIdAsync(id);
            if (simpleTaskDepartment == null)
                return await AccessDenied();

            try
            {
                await _simpleTaskDepartmentService.DeleteSimpleTaskDepartmentAsync(simpleTaskDepartment);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.SimpleTaskDepartments.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _simpleTaskDepartmentService.DeleteSimpleTaskDepartmentAsync((await _simpleTaskDepartmentService.GetSimpleTaskDepartmentsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.SimpleTaskDepartments.Errors.TryToDelete");
            }
        }
    }
}