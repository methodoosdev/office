using App.Core;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Traders;
using App.Services;
using App.Services.Employees;
using App.Services.Traders;
using App.Web.Framework.Controllers;
using App.Web.Infra.Factories.Common.Traders;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Controllers.Common.Traders
{
    public partial class TradersByEmployeeController : BaseProtectController
    {
        private readonly ITraderService _traderService;
        private readonly IEmployeeService _employeeService;
        private readonly ITraderGroupService _traderGroupService;
        private readonly IWorkingAreaService _workingAreaService;
        private readonly ITraderModelFactory _traderModelFactory;
        private readonly ITraderEmployeeMappingService _traderEmployeeMappingService;

        public TradersByEmployeeController(
            ITraderService traderService,
            IEmployeeService employeeService,
            ITraderGroupService traderGroupService,
            IWorkingAreaService workingAreaService,
            ITraderModelFactory traderModelFactory,
            ITraderEmployeeMappingService traderEmployeeMappingService)
        {
            _traderService = traderService;
            _employeeService = employeeService;
            _traderGroupService = traderGroupService;
            _workingAreaService = workingAreaService;
            _traderModelFactory = traderModelFactory;
            _traderEmployeeMappingService = traderEmployeeMappingService;
        }

        private async Task<IPagedList<TraderModel>> GetTradersByEmployeeIdAsync(TraderSearchModel searchModel, int employeeId)
        {
            var traders = await _traderEmployeeMappingService.GetTradersByEmployeeIdAsync(employeeId);

            var traderGroups = await _traderGroupService.GetAllTraderGroupsAsync();
            var workingAreas = await _workingAreaService.GetAllWorkingAreasAsync();

            var professionTypes = await ProfessionType.Active.ToSelectionItemListAsync();
            var categoryBookTypes = await CategoryBookType.None.ToSelectionItemListAsync();
            var legalFormTypes = await LegalFormType.None.ToSelectionItemListAsync();
            var logistikiProgramTypes = await LogistikiProgramType.SoftOne.ToSelectionItemListAsync(withSpecialDefaultItem: true);

            var query = traders.Select(x =>
            {
                var model = x.ToModel<TraderModel>();

                model.FullName = model.FullName() ?? "";
                model.Vat = model.Vat ?? "";
                model.Doy = model.Doy ?? "";
                model.Email = model.Email ?? "";

                model.EmployeeName = string.Join(", ", _traderEmployeeMappingService.GetEmployeesByTraderIdAsync(x.Id).Result.Select(x => x.FullName()).ToArray());
                model.TraderGroupName = traderGroups.FirstOrDefault(a => a.Id == x.TraderGroupId)?.Description ?? "";
                model.WorkingAreaName = workingAreas.FirstOrDefault(a => a.Id == x.WorkingAreaId)?.Description ?? "";
                model.ProfessionTypeName = professionTypes.FirstOrDefault(a => a.Value == x.ProfessionTypeId)?.Label ?? "";
                model.CategoryBookTypeName = categoryBookTypes.FirstOrDefault(a => a.Value == x.CategoryBookTypeId)?.Label ?? "";
                model.LegalFormTypeName = legalFormTypes.FirstOrDefault(a => a.Value == x.LegalFormTypeId)?.Label ?? "";
                model.LogistikiProgramTypeName = logistikiProgramTypes.FirstOrDefault(a => a.Value == x.LogistikiProgramTypeId)?.Label ?? "";

                return model;
            }).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.FullName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Vat.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Email.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<IActionResult> List()
        {
            //prepare model
            var searchModel = await _traderModelFactory.PrepareTraderDialogSearchModelAsync(new TraderSearchModel());
            searchModel.Length = int.Parse(searchModel.AvailablePageSizes.Split(',').First());

            return Json(new { searchModel });
        }

        [HttpPost]
        public virtual async Task<IActionResult> List([FromBody] TraderSearchModel searchModel, int parentId)
        {
            //try to get entity with the specified id
            var employee = await _employeeService.GetEmployeeByIdAsync(parentId);
            if (employee == null)
                return await AccessDenied();

            var traders = await GetTradersByEmployeeIdAsync(searchModel, employee.Id);

            //prepare grid model
            var model = new TraderListModel().PrepareToGrid(searchModel, traders);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ImportMapping([FromBody] ICollection<int> selectedIds, int parentId)
        {
            //try to get entity with the specified id
            var employee = await _employeeService.GetEmployeeByIdAsync(parentId);
            if (employee == null)
                return await AccessDenied();

            var selectedTraders = await _traderService.GetTradersByIdsAsync(selectedIds.ToArray());

            foreach (var selected in selectedTraders)
            {
                await _traderEmployeeMappingService.InsertTraderEmployeeAsync(selected, employee);
            }

            return Ok();
        }

        [HttpPost]
        public virtual async Task<IActionResult> RemoveMapping([FromBody] ICollection<int> selectedIds, int parentId)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(parentId);
            if (employee == null)
                return await AccessDenied();

            var selectedTraders = await _traderService.GetTradersByIdsAsync(selectedIds.ToArray());

            foreach (var selected in selectedTraders)
            {
                await _traderEmployeeMappingService.RemoveTraderEmployeeAsync(selected, employee);
            }

            return Ok();
        }
    }
}