using App.Core.Domain.Assignment;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Assignment;
using App.Services.Assignment;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Security;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Assignment;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Assignment
{
    public partial class AssignmentPrototypeController : BaseProtectController
    {
        private readonly IAssignmentPrototypeService _assignmentPrototypeService;
        private readonly ILocalizationService _localizationService;
        private readonly IAssignmentPrototypeModelFactory _assignmentPrototypeModelFactory;

        public AssignmentPrototypeController(
            IAssignmentPrototypeService assignmentPrototypeService,
            ILocalizationService localizationService,
            IAssignmentPrototypeModelFactory assignmentPrototypeModelFactory)
        {
            _assignmentPrototypeService = assignmentPrototypeService;
            _localizationService = localizationService;
            _assignmentPrototypeModelFactory = assignmentPrototypeModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _assignmentPrototypeModelFactory.PrepareAssignmentPrototypeSearchModelAsync(new AssignmentPrototypeSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] AssignmentPrototypeSearchModel searchModel)
        {
            //prepare model
            var model = await _assignmentPrototypeModelFactory.PrepareAssignmentPrototypeListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _assignmentPrototypeModelFactory.PrepareAssignmentPrototypeModelAsync(new AssignmentPrototypeModel(), null);

            //prepare form
            var formModel = await _assignmentPrototypeModelFactory.PrepareAssignmentPrototypeFormModelAsync(new AssignmentPrototypeFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] AssignmentPrototypeModel model)
        {
            if (ModelState.IsValid)
            {
                var assignmentPrototype = model.ToEntity<AssignmentPrototype>();
                await _assignmentPrototypeService.InsertAssignmentPrototypeAsync(assignmentPrototype);

                return Json(assignmentPrototype.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var assignmentPrototype = await _assignmentPrototypeService.GetAssignmentPrototypeByIdAsync(id);
            if (assignmentPrototype == null)
                return await AccessDenied();

            //prepare model
            var model = await _assignmentPrototypeModelFactory.PrepareAssignmentPrototypeModelAsync(null, assignmentPrototype);

            //prepare form
            var formModel = await _assignmentPrototypeModelFactory.PrepareAssignmentPrototypeFormModelAsync(new AssignmentPrototypeFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] AssignmentPrototypeModel model)
        {
            //try to get entity with the specified id
            var assignmentPrototype = await _assignmentPrototypeService.GetAssignmentPrototypeByIdAsync(model.Id);
            if (assignmentPrototype == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    assignmentPrototype = model.ToEntity(assignmentPrototype);
                    await _assignmentPrototypeService.UpdateAssignmentPrototypeAsync(assignmentPrototype);

                    return Json(assignmentPrototype.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AssignmentPrototypes.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var assignmentPrototype = await _assignmentPrototypeService.GetAssignmentPrototypeByIdAsync(id);
            if (assignmentPrototype == null)
                return await AccessDenied();

            try
            {
                await _assignmentPrototypeService.DeleteAssignmentPrototypeAsync(assignmentPrototype);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AssignmentPrototypes.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _assignmentPrototypeService.DeleteAssignmentPrototypeAsync((await _assignmentPrototypeService.GetAssignmentPrototypesByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AssignmentPrototypes.Errors.TryToDelete");
            }
        }
    }
}