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
    public partial class AssignmentTaskActionController : BaseProtectController
    {
        private readonly IAssignmentTaskActionService _assignmentTaskActionService;
        private readonly ILocalizationService _localizationService;
        private readonly IAssignmentTaskActionModelFactory _assignmentTaskActionModelFactory;

        public AssignmentTaskActionController(
            IAssignmentTaskActionService assignmentTaskActionService,
            ILocalizationService localizationService,
            IAssignmentTaskActionModelFactory assignmentTaskActionModelFactory)
        {
            _assignmentTaskActionService = assignmentTaskActionService;
            _localizationService = localizationService;
            _assignmentTaskActionModelFactory = assignmentTaskActionModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _assignmentTaskActionModelFactory.PrepareAssignmentTaskActionSearchModelAsync(new AssignmentTaskActionSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] AssignmentTaskActionSearchModel searchModel, int parentId)
        {
            //prepare model
            var model = await _assignmentTaskActionModelFactory.PrepareAssignmentTaskActionListModelAsync(searchModel, parentId);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _assignmentTaskActionModelFactory.PrepareAssignmentTaskActionModelAsync(new AssignmentTaskActionModel(), null);

            //prepare form
            var formModel = await _assignmentTaskActionModelFactory.PrepareAssignmentTaskActionFormModelAsync(new AssignmentTaskActionFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] AssignmentTaskActionModel model)
        {
            if (ModelState.IsValid)
            {
                var assignmentTaskAction = model.ToEntity<AssignmentTaskAction>();
                await _assignmentTaskActionService.InsertAssignmentTaskActionAsync(assignmentTaskAction);

                return Json(assignmentTaskAction.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var assignmentTaskAction = await _assignmentTaskActionService.GetAssignmentTaskActionByIdAsync(id);
            if (assignmentTaskAction == null)
                return await AccessDenied();

            //prepare model
            var model = await _assignmentTaskActionModelFactory.PrepareAssignmentTaskActionModelAsync(null, assignmentTaskAction);

            //prepare form
            var formModel = await _assignmentTaskActionModelFactory.PrepareAssignmentTaskActionFormModelAsync(new AssignmentTaskActionFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] AssignmentTaskActionModel model)
        {
            //try to get entity with the specified id
            var assignmentAction = await _assignmentTaskActionService.GetAssignmentTaskActionByIdAsync(model.Id);
            if (assignmentAction == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    assignmentAction = model.ToEntity(assignmentAction);
                    await _assignmentTaskActionService.UpdateAssignmentTaskActionAsync(assignmentAction);

                    return Json(assignmentAction.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AssignmentTaskActions.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var assignmentTaskAction = await _assignmentTaskActionService.GetAssignmentTaskActionByIdAsync(id);
            if (assignmentTaskAction == null)
                return await AccessDenied();

            try
            {
                await _assignmentTaskActionService.DeleteAssignmentTaskActionAsync(assignmentTaskAction);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AssignmentTaskActions.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _assignmentTaskActionService.DeleteAssignmentTaskActionAsync((await _assignmentTaskActionService.GetAssignmentTaskActionsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AssignmentTaskActions.Errors.TryToDelete");
            }
        }

        public virtual async Task<IActionResult> LoadList(int parentId)
        {
            //prepare model
            var model = await _assignmentTaskActionModelFactory.GetLoadListAsync(parentId);

            return Json(model);
        }

    }
}