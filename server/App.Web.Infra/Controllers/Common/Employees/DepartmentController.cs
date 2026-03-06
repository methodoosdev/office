using App.Core.Domain.Employees;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Employees;
using App.Services.Assignment;
using App.Services.Employees;
using App.Services.Localization;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Employees;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Employees
{
    public partial class DepartmentController : BaseProtectController
    {
        private readonly IDepartmentService _departmentService;
        private readonly IEmployeeService _employeeService;
        private readonly IAssignmentPrototypeActionService _assignmentPrototypeActionService;
        private readonly ILocalizationService _localizationService;
        private readonly IDepartmentModelFactory _departmentModelFactory;

        public DepartmentController(
            IDepartmentService departmentService,
            IEmployeeService employeeService,
            IAssignmentPrototypeActionService assignmentPrototypeActionService,
            ILocalizationService localizationService,
            IDepartmentModelFactory departmentModelFactory)
        {
            _departmentService = departmentService;
            _employeeService = employeeService;
            _assignmentPrototypeActionService = assignmentPrototypeActionService;
            _localizationService = localizationService;
            _departmentModelFactory = departmentModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _departmentModelFactory.PrepareDepartmentSearchModelAsync(new DepartmentSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] DepartmentSearchModel searchModel)
        {
            //prepare model
            var model = await _departmentModelFactory.PrepareDepartmentListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _departmentModelFactory.PrepareDepartmentModelAsync(new DepartmentModel(), null);

            //prepare form
            var formModel = await _departmentModelFactory.PrepareDepartmentFormModelAsync(new DepartmentFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] DepartmentModel model)
        {
            if (ModelState.IsValid)
            {
                var department = model.ToEntity<Department>();
                await _departmentService.InsertDepartmentAsync(department);

                return Json(department.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
                return await AccessDenied();

            //prepare model
            var model = await _departmentModelFactory.PrepareDepartmentModelAsync(null, department);

            //prepare form
            var formModel = await _departmentModelFactory.PrepareDepartmentFormModelAsync(new DepartmentFormModel());

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] DepartmentModel model)
        {
            //try to get entity with the specified id
            var department = await _departmentService.GetDepartmentByIdAsync(model.Id);
            if (department == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    department = model.ToEntity(department);
                    await _departmentService.UpdateDepartmentAsync(department);

                    return Json(department.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Departments.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
                return await AccessDenied();

            var onEmployee = await _employeeService.Table.AnyAsync(x => x.DepartmentId == department.Id);
            if (onEmployee)
                return await BadRequestMessageAsync("App.Models.Departments.Errors.ExistOnEmployee");

            var onAssignmentPrototypeAction = await _assignmentPrototypeActionService.Table.AnyAsync(x => x.DepartmentId == department.Id);
            if (onAssignmentPrototypeAction)
                return await BadRequestMessageAsync("App.Models.Departments.Errors.ExistOnAssignmentPrototypeAction");

            try
            {
                await _departmentService.DeleteDepartmentAsync(department);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Departments.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _departmentService.DeleteDepartmentAsync((await _departmentService.GetDepartmentsByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Departments.Errors.TryToDelete");
            }
        }
    }
}