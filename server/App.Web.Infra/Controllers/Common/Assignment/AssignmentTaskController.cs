using App.Core.Domain.Assignment;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Assignment;
using App.Services.Assignment;
using App.Services.Localization;
using App.Services.Offices;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Assignment;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Assignment
{
    public partial class AssignmentTaskController : BaseProtectController
    {
        private readonly ITraderEmployeeMappingService _traderEmployeeMappingService;
        private readonly IAssignmentTaskService _assignmentTaskService;
        private readonly IAssignmentTaskActionService _assignmentTaskActionService;
        private readonly IAssignmentPrototypeService _assignmentPrototypeService;
        private readonly IAssignmentPrototypeAssignmentPrototypeActionMappingService _assignmentPrototypeMappingService;
        private readonly IAssignmentTaskModelFactory _assignmentTaskModelFactory;
        private readonly IPersistStateService _persistStateService;
        private readonly ILocalizationService _localizationService;

        public AssignmentTaskController(
            ITraderEmployeeMappingService traderEmployeeMappingService,
            IAssignmentTaskService assignmentTaskService,
            IAssignmentTaskActionService assignmentTaskActionService,
            IAssignmentPrototypeService assignmentPrototypeService,
            IAssignmentPrototypeAssignmentPrototypeActionMappingService assignmentPrototypeMappingService,
            IAssignmentTaskModelFactory assignmentTaskModelFactory,
            IPersistStateService persistStateService,
            ILocalizationService localizationService)
        {
            _traderEmployeeMappingService = traderEmployeeMappingService;
            _assignmentTaskService = assignmentTaskService;
            _assignmentTaskActionService = assignmentTaskActionService;
            _assignmentPrototypeService = assignmentPrototypeService;
            _assignmentPrototypeMappingService = assignmentPrototypeMappingService;
            _assignmentTaskModelFactory = assignmentTaskModelFactory;
            _persistStateService = persistStateService;
            _localizationService = localizationService;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _assignmentTaskModelFactory.PrepareAssignmentTaskSearchModelAsync(new AssignmentTaskSearchModel());

            var filterModel = (await _persistStateService.GetModelInstance<AssignmentTaskFilterModel>()).Model;
            var filterFormModel = await _assignmentTaskModelFactory.PrepareAssignmentTaskFilterFormModelAsync(new AssignmentTaskFilterFormModel());
            var filterDefaultModel = new AssignmentTaskFilterModel();

            return Json(new { searchModel, filterModel, filterFormModel, filterDefaultModel });

        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] AssignmentTaskSearchModel searchModel)
        {
            //prepare model
            var model = await _assignmentTaskModelFactory.PrepareAssignmentTaskListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _assignmentTaskModelFactory.PrepareAssignmentTaskModelAsync(new AssignmentTaskModel(), null);

            //prepare form
            var formModel = await _assignmentTaskModelFactory.PrepareAssignmentTaskFormModelAsync(new AssignmentTaskFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] AssignmentTaskModel model)
        {
            if (ModelState.IsValid)
            {
                //try to get entity with the specified id
                var assignmentPrototype = await _assignmentPrototypeService.GetAssignmentPrototypeByIdAsync(model.AssignmentPrototypeId);
                if (assignmentPrototype == null)
                    return await AccessDenied();

                var assignmentTask = model.ToEntity<AssignmentTask>();
                assignmentTask.Name = assignmentPrototype.Name;
                assignmentTask.Description = assignmentPrototype.Description;

                await _assignmentTaskService.InsertAssignmentTaskAsync(assignmentTask);

                //create assignmentActions
                var assignmentPrototypeActions = await _assignmentPrototypeMappingService.GetAssignmentPrototypeActionsByAssignmentPrototypeIdAsync(model.AssignmentPrototypeId);

                var assignmentActions = new List<AssignmentTaskAction>();
                foreach (var assignmentPrototypeAction in assignmentPrototypeActions)
                {
                    var employees = await _traderEmployeeMappingService.GetEmployeesByTraderIdAsync(assignmentTask.TraderId);
                    var employee = employees?.FirstOrDefault(x => x.DepartmentId == assignmentPrototypeAction.DepartmentId);

                    var assignmentAction = new AssignmentTaskAction
                    {
                        AssignmentTaskId = assignmentTask.Id,
                        ActionName = assignmentPrototypeAction.Name,
                        ActionDescription = assignmentPrototypeAction.Description,
                        AssignmentActionStatusTypeId = (int)AssignmentActionStatusType.InProgress,
                        AssignmentActionPriorityTypeId = (int)AssignmentActionPriorityType.Normal,
                        EmployeeId = employee?.Id ?? 0,
                        ExpiryDate = assignmentTask.ExpiryDate,
                        DisplayOrder = assignmentPrototypeAction.DisplayOrder
                    };
                    assignmentActions.Add(assignmentAction);
                }

                await _assignmentTaskActionService.InsertAssignmentTaskActionAsync(assignmentActions);

                return Json(assignmentTask.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var assignmentTask = await _assignmentTaskService.GetAssignmentTaskByIdAsync(id);
            if (assignmentTask == null)
                return await AccessDenied();

            //prepare model
            var model = await _assignmentTaskModelFactory.PrepareAssignmentTaskModelAsync(null, assignmentTask);

            //prepare form
            var formModel = await _assignmentTaskModelFactory.PrepareAssignmentTaskFormModelAsync(new AssignmentTaskFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] AssignmentTaskModel model)
        {
            //try to get entity with the specified id
            var assignmentTask = await _assignmentTaskService.GetAssignmentTaskByIdAsync(model.Id);
            if (assignmentTask == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    assignmentTask = model.ToEntity(assignmentTask);
                    await _assignmentTaskService.UpdateAssignmentTaskAsync(assignmentTask);

                    return Json(assignmentTask.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AssignmentTasks.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var assignmentTask = await _assignmentTaskService.GetAssignmentTaskByIdAsync(id);
            if (assignmentTask == null)
                return await AccessDenied();

            try
            {
                await _assignmentTaskService.DeleteAssignmentTaskAsync(assignmentTask);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AssignmentTasks.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _assignmentTaskService.DeleteAssignmentTaskAsync((await _assignmentTaskService.GetAssignmentTasksByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.AssignmentTasks.Errors.TryToDelete");
            }
        }
    }
}