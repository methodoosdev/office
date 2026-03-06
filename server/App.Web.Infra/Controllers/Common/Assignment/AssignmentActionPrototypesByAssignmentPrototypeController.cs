using App.Core;
using App.Core.Infrastructure;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Assignment;
using App.Services;
using App.Services.Assignment;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Assignment;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Assignment
{
    public partial class AssignmentPrototypeActionsByAssignmentPrototypeController : BaseProtectController
    {
        private readonly IAssignmentPrototypeService _assignmentPrototypeService;
        private readonly IAssignmentPrototypeActionService _assignmentPrototypeActionService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly IAssignmentPrototypeActionModelFactory _assignmentPrototypeActionModelFactory;
        private readonly IAssignmentPrototypeAssignmentPrototypeActionMappingService _assignmentPrototypeMappingService;

        public AssignmentPrototypeActionsByAssignmentPrototypeController(
            IAssignmentPrototypeService assignmentPrototypeService,
            IAssignmentPrototypeActionService assignmentPrototypeActionService,
            IModelFactoryService modelFactoryService,
            IAssignmentPrototypeActionModelFactory assignmentPrototypeActionModelFactory,
            IAssignmentPrototypeAssignmentPrototypeActionMappingService assignmentPrototypeMappingService)
        {
            _assignmentPrototypeService = assignmentPrototypeService;
            _assignmentPrototypeActionService = assignmentPrototypeActionService;
            _modelFactoryService = modelFactoryService;
            _assignmentPrototypeActionModelFactory = assignmentPrototypeActionModelFactory;
            _assignmentPrototypeMappingService = assignmentPrototypeMappingService;
        }

        private async Task<IPagedList<AssignmentPrototypeActionModel>> GetAssignmentPrototypeActionsByAssignmentPrototypeIdAsync(AssignmentPrototypeActionSearchModel searchModel, int assignmentPrototypeId)
        {
            var departments = await _modelFactoryService.GetAllDepartmentsAsync(false);
            var assignmentReasons = await _modelFactoryService.GetAllAssignmentReasonsAsync(false);
            var assignmentPrototypeActions = await _assignmentPrototypeMappingService.GetAssignmentPrototypeActionsByAssignmentPrototypeIdAsync(assignmentPrototypeId);

            var query = assignmentPrototypeActions.Select(x =>
            {
                var model = x.ToModel<AssignmentPrototypeActionModel>();

                model.DepartmentName = departments.FirstOrDefault(d => x.DepartmentId == d.Value)?.Label ?? "";
                model.AssignmentReasonName = assignmentReasons.FirstOrDefault(d => d.Value == x.AssignmentReasonId)?.Label ?? ""; ;

                return model;
            }).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.Name.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.DepartmentName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.AssignmentReasonName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _assignmentPrototypeActionModelFactory.PrepareAssignmentPrototypeActionSearchModelAsync(new AssignmentPrototypeActionSearchModel());
            searchModel.Length = int.Parse(searchModel.AvailablePageSizes.Split(',').First());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] AssignmentPrototypeActionSearchModel searchModel, int parentId)
        {
            //try to get entity with the specified id
            var assignmentPrototype = await _assignmentPrototypeService.GetAssignmentPrototypeByIdAsync(parentId);
            if (assignmentPrototype == null)
                return await AccessDenied();

            var assignmentPrototypeActions = await GetAssignmentPrototypeActionsByAssignmentPrototypeIdAsync(searchModel, assignmentPrototype.Id);

            //prepare grid model
            var model = new AssignmentPrototypeActionListModel().PrepareToGrid(searchModel, assignmentPrototypeActions);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportMapping([FromBody] ICollection<int> selectedIds, int parentId)
        {
            //try to get entity with the specified id
            var assignmentPrototype = await _assignmentPrototypeService.GetAssignmentPrototypeByIdAsync(parentId);
            if (assignmentPrototype == null)
                return await AccessDenied();

            var selectedAssignmentPrototypeActions = await _assignmentPrototypeActionService.GetAssignmentPrototypeActionsByIdsAsync(selectedIds.ToArray());

            foreach (var selected in selectedAssignmentPrototypeActions)
            {
                await _assignmentPrototypeMappingService.InsertAssignmentPrototypeAssignmentPrototypeActionAsync(assignmentPrototype, selected);
            }

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> RemoveMapping([FromBody] ICollection<int> selectedIds, int parentId)
        {
            var assignmentPrototype = await _assignmentPrototypeService.GetAssignmentPrototypeByIdAsync(parentId);
            if (assignmentPrototype == null)
                return await AccessDenied();

            var selectedAssignmentPrototypeActions = await _assignmentPrototypeActionService.GetAssignmentPrototypeActionsByIdsAsync(selectedIds.ToArray());

            foreach (var selected in selectedAssignmentPrototypeActions)
            {
                await _assignmentPrototypeMappingService.RemoveAssignmentPrototypeAssignmentPrototypeActionAsync(assignmentPrototype, selected);
            }

            return Ok();
        }
    }
}