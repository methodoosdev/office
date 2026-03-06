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
    public partial class AssignmentReasonController : BaseProtectController
    {
        private readonly IAssignmentReasonService _assignmentReasonService;
        private readonly ILocalizationService _localizationService;
        private readonly IAssignmentReasonModelFactory _assignmentReasonModelFactory;

        public AssignmentReasonController(
            IAssignmentReasonService assignmentReasonService,
            ILocalizationService localizationService,
            IAssignmentReasonModelFactory assignmentReasonModelFactory)
        {
            _assignmentReasonService = assignmentReasonService;
            _localizationService = localizationService;
            _assignmentReasonModelFactory = assignmentReasonModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _assignmentReasonModelFactory.PrepareAssignmentReasonSearchModelAsync(new AssignmentReasonSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] AssignmentReasonSearchModel searchModel)
        {
            //prepare model
            var model = await _assignmentReasonModelFactory.PrepareAssignmentReasonListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _assignmentReasonModelFactory.PrepareAssignmentReasonModelAsync(new AssignmentReasonModel(), null);

            //prepare form
            var formModel = await _assignmentReasonModelFactory.PrepareAssignmentReasonFormModelAsync(new AssignmentReasonFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] AssignmentReasonModel model)
        {
            if (ModelState.IsValid)
            {
                var assignmentReason = model.ToEntity<AssignmentReason>();
                await _assignmentReasonService.InsertAssignmentReasonAsync(assignmentReason);

                return Json(assignmentReason.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var assignmentReason = await _assignmentReasonService.GetAssignmentReasonByIdAsync(id);
            if (assignmentReason == null)
                return await AccessDenied();

            //prepare model
            var model = await _assignmentReasonModelFactory.PrepareAssignmentReasonModelAsync(null, assignmentReason);

            //prepare form
            var formModel = await _assignmentReasonModelFactory.PrepareAssignmentReasonFormModelAsync(new AssignmentReasonFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] AssignmentReasonModel model)
        {
            //try to get entity with the specified id
            var assignmentReason = await _assignmentReasonService.GetAssignmentReasonByIdAsync(model.Id);
            if (assignmentReason == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    assignmentReason = model.ToEntity(assignmentReason);
                    await _assignmentReasonService.UpdateAssignmentReasonAsync(assignmentReason);

                    return Json(assignmentReason.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AssignmentReasons.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var assignmentReason = await _assignmentReasonService.GetAssignmentReasonByIdAsync(id);
            if (assignmentReason == null)
                return await AccessDenied();

            try
            {
                await _assignmentReasonService.DeleteAssignmentReasonAsync(assignmentReason);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AssignmentReasons.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _assignmentReasonService.DeleteAssignmentReasonAsync((await _assignmentReasonService.GetAssignmentReasonsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AssignmentReasons.Errors.TryToDelete");
            }
        }
    }
}