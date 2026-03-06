using App.Core.Domain.Employees;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Models.Employees;
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
    public partial class EmployeeController : BaseProtectController
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILocalizationService _localizationService;
        private readonly IEmployeeModelFactory _employeeModelFactory;

        public EmployeeController(
            IEmployeeService employeeService,
            ILocalizationService localizationService,
            IEmployeeModelFactory employeeModelFactory)
        {
            _employeeService = employeeService;
            _localizationService = localizationService;
            _employeeModelFactory = employeeModelFactory;
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _employeeModelFactory.PrepareEmployeeSearchModelAsync(new EmployeeSearchModel());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] EmployeeSearchModel searchModel)
        {
            //prepare model
            var model = await _employeeModelFactory.PrepareEmployeeListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            //prepare model
            var model = await _employeeModelFactory.PrepareEmployeeModelAsync(new EmployeeModel(), null);

            //prepare form
            var formModel = await _employeeModelFactory.PrepareEmployeeFormModelAsync(new EmployeeFormModel(), model);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] EmployeeModel model)
        {
            if (ModelState.IsValid)
            {
                var employee = model.ToEntity<Employee>();
                await _employeeService.InsertEmployeeAsync(employee);

                return Json(employee.Id);
            }

            //if we got this far, something failed, redisplay form
            return BadRequestFromModel();
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            //try to get entity with the specified id
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return await AccessDenied();

            //prepare model
            var model = await _employeeModelFactory.PrepareEmployeeModelAsync(null, employee);

            //prepare form
            var formModel = await _employeeModelFactory.PrepareEmployeeFormModelAsync(new EmployeeFormModel(), model);

            return Json(new { model, formModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> Edit([FromBody] EmployeeModel model)
        {
            //try to get entity with the specified id
            var employee = await _employeeService.GetEmployeeByIdAsync(model.Id);
            if (employee == null)
                return await AccessDenied();

            try
            {
                if (ModelState.IsValid)
                {
                    employee = model.ToEntity(employee);
                    await _employeeService.UpdateEmployeeAsync(employee);

                    return Json(employee.Id);
                }

                //if we got this far, something failed, redisplay form
                return BadRequestFromModel();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Employees.Errors.TryToEdit");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            //try to get a customer role with the specified id
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return await AccessDenied();

            try
            {
                await _employeeService.DeleteEmployeeAsync(employee);

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Employees.Errors.TryToDelete");
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected([FromBody] ICollection<int> selectedIds)
        {
            try
            {
                if (selectedIds != null)
                    await _employeeService.DeleteEmployeeAsync((await _employeeService.GetEmployeesByIdsAsync(selectedIds.ToArray())).ToList());

                return Ok();
            }
            catch
            {
                return await BadRequestMessageAsync("App.Models.Employees.Errors.TryToDelete");
            }
        }
    }
}