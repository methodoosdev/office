using App.Core;
using App.Core.Infrastructure;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Employees;
using App.Services.Employees;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Framework.Mvc.Filters;
using App.Web.Infra.Factories.Common.Employees;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Traders
{
    [CheckCustomerPermission(true)]
    public partial class EmployeesByTraderController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IEmployeeModelFactory _employeeModelFactory;
        private readonly ITraderEmployeeMappingService _traderEmployeeMappingService;
        private readonly IWorkContext _workContext;

        public EmployeesByTraderController(
            ITraderService traderService,
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IEmployeeModelFactory employeeModelFactory,
            ITraderEmployeeMappingService traderEmployeeMappingService,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _employeeService = employeeService;
            _departmentService = departmentService;
            _employeeModelFactory = employeeModelFactory;
            _traderEmployeeMappingService = traderEmployeeMappingService;
            _workContext = workContext;
        }

        private async Task<IPagedList<EmployeeModel>> GetEmployeesByTraderIdAsync(EmployeeSearchModel searchModel, int traderId)
        {
            var employeesAll = await _employeeService.GetAllEmployeesAsync();
            var departmentsAll = await _departmentService.GetAllDepartmentsAsync();
            var employees = await _traderEmployeeMappingService.GetEmployeesByTraderIdAsync(traderId);
            var query = employees.Select(x =>
            {
                var model = x.ToModel<EmployeeModel>();

                model.FullName = x.FullName() ?? "";
                model.EmailContact = x.EmailContact ?? "";
                model.Mobile = x.Mobile ?? "";
                model.InternalPhoneNumber = x.InternalPhoneNumber ?? "";
                model.SupervisorName = employeesAll.Where(e => x.SupervisorId == e.Id).FirstOrDefault()?.FullName() ?? "";
                model.DepartmentName = departmentsAll.Where(d => x.DepartmentId == d.Id).FirstOrDefault()?.Description ?? "";

                return model;
            }).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.FullName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.EmailContact.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Mobile.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.InternalPhoneNumber.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _employeeModelFactory.PrepareEmployeeSearchModelAsync(new EmployeeSearchModel());
            searchModel.Length = int.Parse(searchModel.AvailablePageSizes.Split(',').First());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] EmployeeSearchModel searchModel, int parentId)
        {
            //try to get entity with the specified id
            var trader = await _traderService.GetTraderByIdAsync(parentId);
            if (trader == null)
                return await AccessDenied();

            var employees = await GetEmployeesByTraderIdAsync(searchModel, trader.Id);

            //prepare grid model
            var model = new EmployeeListModel().PrepareToGrid(searchModel, employees);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportMapping([FromBody] ICollection<int> selectedIds, int parentId)
        {
            //try to get entity with the specified id
            var trader = await _traderService.GetTraderByIdAsync(parentId);
            if (trader == null)
                return await AccessDenied();

            var selectedEmployees = await _employeeService.GetEmployeesByIdsAsync(selectedIds.ToArray());

            var employees = await _traderEmployeeMappingService.GetEmployeesByTraderIdAsync(trader.Id);
            var departmentIds = employees.Where(k => k.DepartmentId.HasValue).Select(x => x.DepartmentId.Value).ToList();
            var selectedEmployeeIds = selectedEmployees.Where(k => k.DepartmentId.HasValue).Select(x => x.DepartmentId.Value).ToList();

            if (selectedEmployeeIds.Any(x => departmentIds.Contains(x)))
                return await BadRequestMessageAsync("App.Models.Departments.Errors.ExistOnTrader");

            foreach (var selected in selectedEmployees)
            {
                await _traderEmployeeMappingService.InsertTraderEmployeeAsync(trader, selected);
            }

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> RemoveMapping([FromBody] ICollection<int> selectedIds, int parentId)
        {
            var trader = await _traderService.GetTraderByIdAsync(parentId);
            if (trader == null)
                return await AccessDenied();

            var selectedEmployees = await _employeeService.GetEmployeesByIdsAsync(selectedIds.ToArray());

            foreach (var selected in selectedEmployees)
            {
                await _traderEmployeeMappingService.RemoveTraderEmployeeAsync(trader, selected);
            }

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> CanEmployeeTraderRating(int traderId)
        {
            var currentEmployee = await _workContext.GetCurrentEmployeeAsync();
            if (currentEmployee == null)
                return Json(new { valid = true });

            if (traderId == 0)
                return Json(new { valid = false });

            var trader = await _traderService.GetTraderByIdAsync(traderId);
            if (trader == null)
                return await AccessDenied();

            var employees = await _traderEmployeeMappingService.GetEmployeesByTraderIdAsync(traderId);

            var valid = employees.Any(x => x.DepartmentId == currentEmployee.DepartmentId);

            return Json(new { valid });
        }
    }
}