using App.Core;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Assignment;
using App.Services.Assignment;
using App.Services.Localization;
using App.Services.Offices;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Assignment;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Assignment
{
    public partial class AssignmentTaskActionByEmployeeController : BaseProtectController
    {
        private readonly IAssignmentTaskActionService _assignmentTaskActionService;
        private readonly ILocalizationService _localizationService;
        private readonly IAssignmentTaskActionByEmployeeModelFactory _assignmentTaskActionByEmployeeModelFactory;
        private readonly IPersistStateService _persistStateService;
        private readonly IWorkContext _workContext;

        public AssignmentTaskActionByEmployeeController(
            IAssignmentTaskActionService assignmentTaskActionService,
            ILocalizationService localizationService,
            IAssignmentTaskActionByEmployeeModelFactory assignmentTaskActionByEmployeeModelFactory,
            IPersistStateService persistStateService,
            IWorkContext workContext)
        {
            _assignmentTaskActionService = assignmentTaskActionService;
            _localizationService = localizationService;
            _assignmentTaskActionByEmployeeModelFactory = assignmentTaskActionByEmployeeModelFactory;
            _persistStateService = persistStateService;
            _workContext = workContext;
        }

        public virtual async Task<IActionResult> List()
        {
            var employee = await _workContext.GetCurrentEmployeeAsync();
            if (employee == null)
                return await AccessDenied();

            //prepare model
            var searchModel = await _assignmentTaskActionByEmployeeModelFactory.PrepareAssignmentTaskActionSearchModelAsync(new AssignmentTaskActionSearchModel());

            var filterModel = (await _persistStateService.GetModelInstance<AssignmentTaskActionFilterModel>()).Model;
            var filterFormModel = await _assignmentTaskActionByEmployeeModelFactory.PrepareAssignmentTaskActionFilterFormModelAsync(new AssignmentTaskActionFilterFormModel());
            var filterDefaultModel = new AssignmentTaskActionFilterModel();

            return Json(new { searchModel, filterModel, filterFormModel, filterDefaultModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] AssignmentTaskActionSearchModel searchModel)
        {
            var employee = await _workContext.GetCurrentEmployeeAsync();
            if (employee == null)
                return await AccessDenied();

            //prepare model
            var model = await _assignmentTaskActionByEmployeeModelFactory.PrepareAssignmentTaskActionListModelAsync(searchModel, employee.Id);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var assignmentTaskAction = await _assignmentTaskActionService.GetAssignmentTaskActionByIdAsync(id);
            if (assignmentTaskAction == null)
                return await AccessDenied();

            //prepare model
            var model = await _assignmentTaskActionByEmployeeModelFactory.PrepareAssignmentTaskActionModelAsync(null, assignmentTaskAction);

            //prepare form
            var formModel = await _assignmentTaskActionByEmployeeModelFactory.PrepareAssignmentTaskActionFormModelAsync(new AssignmentTaskActionFormModel());

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
    }
}