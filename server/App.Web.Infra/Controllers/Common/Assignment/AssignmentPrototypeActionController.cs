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
    public partial class AssignmentPrototypeActionController : BaseProtectController
    {
        private readonly IAssignmentPrototypeActionService _assignmentPrototypeActionService;
        private readonly IAssignmentPrototypeAssignmentPrototypeActionMappingService _assignmentPrototypeMappingService;
        private readonly ILocalizationService _localizationService;
        private readonly IAssignmentPrototypeActionModelFactory _assignmentPrototypeActionModelFactory;

        public AssignmentPrototypeActionController(
            IAssignmentPrototypeActionService assignmentPrototypeActionService,
            IAssignmentPrototypeAssignmentPrototypeActionMappingService assignmentPrototypeMappingService,
            ILocalizationService localizationService,
            IAssignmentPrototypeActionModelFactory assignmentPrototypeActionModelFactory)
        {
            _assignmentPrototypeActionService = assignmentPrototypeActionService;
            _assignmentPrototypeMappingService = assignmentPrototypeMappingService;
            _localizationService = localizationService;
            _assignmentPrototypeActionModelFactory = assignmentPrototypeActionModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _assignmentPrototypeActionModelFactory.PrepareAssignmentPrototypeActionSearchModelAsync(new AssignmentPrototypeActionSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] AssignmentPrototypeActionSearchModel searchModel)
        {
            //prepare model
            var model = await _assignmentPrototypeActionModelFactory.PrepareAssignmentPrototypeActionListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _assignmentPrototypeActionModelFactory.PrepareAssignmentPrototypeActionModelAsync(new AssignmentPrototypeActionModel(), null);

            //prepare form
            var formModel = await _assignmentPrototypeActionModelFactory.PrepareAssignmentPrototypeActionFormModelAsync(new AssignmentPrototypeActionFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] AssignmentPrototypeActionModel model)
        {
            if (ModelState.IsValid)
            {
                var assignmentPrototypeAction = model.ToEntity<AssignmentPrototypeAction>();
                await _assignmentPrototypeActionService.InsertAssignmentPrototypeActionAsync(assignmentPrototypeAction);

                return Json(assignmentPrototypeAction.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var assignmentPrototypeAction = await _assignmentPrototypeActionService.GetAssignmentPrototypeActionByIdAsync(id);
            if (assignmentPrototypeAction == null)
                return await AccessDenied();

            //prepare model
            var model = await _assignmentPrototypeActionModelFactory.PrepareAssignmentPrototypeActionModelAsync(null, assignmentPrototypeAction);

            //prepare form
            var formModel = await _assignmentPrototypeActionModelFactory.PrepareAssignmentPrototypeActionFormModelAsync(new AssignmentPrototypeActionFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] AssignmentPrototypeActionModel model)
        {
            //try to get entity with the specified id
            var assignmentPrototypeAction = await _assignmentPrototypeActionService.GetAssignmentPrototypeActionByIdAsync(model.Id);
            if (assignmentPrototypeAction == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    assignmentPrototypeAction = model.ToEntity(assignmentPrototypeAction);
                    await _assignmentPrototypeActionService.UpdateAssignmentPrototypeActionAsync(assignmentPrototypeAction);

                    return Json(assignmentPrototypeAction.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AssignmentPrototypeActions.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var assignmentPrototypeAction = await _assignmentPrototypeActionService.GetAssignmentPrototypeActionByIdAsync(id);
            if (assignmentPrototypeAction == null)
                return await AccessDenied();

            if (await _assignmentPrototypeMappingService.Table.AnyAsync(x => x.AssignmentPrototypeActionId == assignmentPrototypeAction.Id))
                return await BadRequestMessageAsync("App.Models.AssignmentPrototypeActions.Errors.ExistOnAssignmentPrototypeAction");

            try
            {
                await _assignmentPrototypeActionService.DeleteAssignmentPrototypeActionAsync(assignmentPrototypeAction);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AssignmentPrototypeActions.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _assignmentPrototypeActionService.DeleteAssignmentPrototypeActionAsync((await _assignmentPrototypeActionService.GetAssignmentPrototypeActionsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AssignmentPrototypeActions.Errors.TryToDelete");
            }
        }
    }
}