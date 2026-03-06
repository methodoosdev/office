using App.Core;
using App.Core.Domain.Assignment;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Assignment;
using App.Services;
using App.Services.Assignment;
using App.Services.Employees;
using App.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Assignment
{
    public partial interface IAssignmentTaskActionModelFactory
    {
        Task<AssignmentTaskActionSearchModel> PrepareAssignmentTaskActionSearchModelAsync(AssignmentTaskActionSearchModel searchModel);
        Task<AssignmentTaskActionListModel> PrepareAssignmentTaskActionListModelAsync(AssignmentTaskActionSearchModel searchModel, int parentId);
        Task<AssignmentTaskActionModel> PrepareAssignmentTaskActionModelAsync(AssignmentTaskActionModel model, AssignmentTaskAction assignmentTaskAction);
        Task<AssignmentTaskActionFormModel> PrepareAssignmentTaskActionFormModelAsync(AssignmentTaskActionFormModel formModel);
        Task<List<AssignmentTaskActionModel>> GetLoadListAsync(int parentId);
    }
    public partial class AssignmentTaskActionModelFactory : IAssignmentTaskActionModelFactory
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;
        private readonly IAssignmentTaskActionService _assignmentTaskActionService;
        private readonly IAssignmentPrototypeActionService _assignmentPrototypeActionService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public AssignmentTaskActionModelFactory(
            IEmployeeService employeeService,
            IDepartmentService departmentService,
            IAssignmentTaskActionService assignmentTaskActionService,
            IAssignmentPrototypeActionService assignmentPrototypeActionService,
            IModelFactoryService modelFactoryService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
            _assignmentTaskActionService = assignmentTaskActionService;
            _assignmentPrototypeActionService = assignmentPrototypeActionService;
            _modelFactoryService = modelFactoryService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<AssignmentTaskActionModel>> GetPagedListAsync(AssignmentTaskActionSearchModel searchModel, int parentId)
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var departments = await _modelFactoryService.GetAllDepartmentsAsync(false);
            var assignmentPrototypeActions = await _assignmentPrototypeActionService.GetAllAssignmentPrototypeActionsAsync();
            var assignmentActionStatusTypes = await AssignmentActionStatusType.InProgress.ToSelectionItemListAsync();
            var assignmentTaskActionPriorityTypes = await AssignmentActionPriorityType.Normal.ToSelectionItemListAsync();

            var query = _assignmentTaskActionService.Table.AsEnumerable()
                .Select(x =>
                {
                    var employee = employees.FirstOrDefault(e => e.Id == x.EmployeeId);
                    var model = x.ToModel<AssignmentTaskActionModel>();

                    model.AssignorName = x.ActionName ?? "";
                    model.ActionDescription = x.ActionDescription ?? "";
                    model.AssignmentActionStatusTypeName = assignmentActionStatusTypes.FirstOrDefault(a => a.Value == x.AssignmentActionStatusTypeId)?.Label ?? "";
                    model.AssignmentActionPriorityTypeName = assignmentTaskActionPriorityTypes.FirstOrDefault(a => a.Value == x.AssignmentActionPriorityTypeId)?.Label ?? "";
                    model.EmployeeName = employee?.FullName() ?? "";
                    model.DepartmentName = departments.FirstOrDefault(d => d.Value == (employee?.DepartmentId ?? 0))?.Label ?? "";

                    return model;
                })
                .AsQueryable();

            query = query.Where(x => x.AssignmentTaskId == parentId);

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.ActionName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.ActionDescription.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.AssignmentActionStatusTypeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.AssignmentActionPriorityTypeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.EmployeeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.DepartmentName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<AssignmentTaskActionSearchModel> PrepareAssignmentTaskActionSearchModelAsync(AssignmentTaskActionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.AssignmentTaskActionModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<AssignmentTaskActionListModel> PrepareAssignmentTaskActionListModelAsync(AssignmentTaskActionSearchModel searchModel, int parentId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var assignmentTaskActions = await GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new AssignmentTaskActionListModel().PrepareToGrid(searchModel, assignmentTaskActions);

            return model;
        }

        public virtual Task<AssignmentTaskActionModel> PrepareAssignmentTaskActionModelAsync(AssignmentTaskActionModel model, AssignmentTaskAction assignmentTaskAction)
        {
            if (assignmentTaskAction != null)
            {
                //fill in model values from the entity
                model ??= assignmentTaskAction.ToModel<AssignmentTaskActionModel>();
            }

            return Task.FromResult(model);
        }

        private async Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<AssignmentTaskActionModel>(0, nameof(AssignmentTaskActionModel.ActionName)),
                ColumnConfig.Create<AssignmentTaskActionModel>(1, nameof(AssignmentTaskActionModel.ActionDescription)),
                ColumnConfig.Create<AssignmentTaskActionModel>(2, nameof(AssignmentTaskActionModel.AssignmentActionStatusTypeName)),
                ColumnConfig.Create<AssignmentTaskActionModel>(2, nameof(AssignmentTaskActionModel.AssignmentActionPriorityTypeName)),
                ColumnConfig.Create<AssignmentTaskActionModel>(3, nameof(AssignmentTaskActionModel.EmployeeName)),
                ColumnConfig.Create<AssignmentTaskActionModel>(4, nameof(AssignmentTaskActionModel.DepartmentName)),
                ColumnConfig.Create<AssignmentTaskActionModel>(5, nameof(AssignmentTaskActionModel.ExpiryDate), ColumnType.Date, style: centerAlign),
                ColumnConfig.Create<AssignmentTaskActionModel>(6, nameof(AssignmentTaskActionModel.DisplayOrder), style: rightAlign, hidden: true),
                ColumnConfig.CreateButton<AssignmentTaskActionModel>(7, ColumnType.RowButton, "modify", "info",
                    await _localizationService.GetResourceAsync("App.Common.Modify"), centerAlign, centerAlign)
            };
            return columns;
        }

        public virtual async Task<AssignmentTaskActionFormModel> PrepareAssignmentTaskActionFormModelAsync(AssignmentTaskActionFormModel formModel)
        {
            var employees = await _modelFactoryService.GetAllEmployeesAsync(false);
            //var assignmentPrototypeActions = await _baseModelFactory.GetAllAssignmentPrototypeActionsAsync();
            var assignmentTaskActionStatusTypes = await AssignmentActionStatusType.InProgress.ToSelectionItemListAsync();
            var assignmentTaskActionPriorityTypes = await AssignmentActionPriorityType.Normal.ToSelectionItemListAsync();

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentTaskActionModel>(nameof(AssignmentTaskActionModel.ActionName), FieldType.Textarea, rows: 2, hideLabel: true, className: "no-label"),
                FieldConfig.Create<AssignmentTaskActionModel>(nameof(AssignmentTaskActionModel.ActionDescription), FieldType.Textarea, rows: 18, hideLabel: true, className: "no-label"),
            };

            var right = new List<Dictionary<string, object>>()
            {
                //FieldConfig.Create<AssignmentTaskActionModel>(nameof(AssignmentTaskActionModel.AssignmentPrototypeActionId), FieldType.GridSelect, options: assignmentPrototypeActions),
                FieldConfig.Create<AssignmentTaskActionModel>(nameof(AssignmentTaskActionModel.AssignmentActionStatusTypeId), FieldType.Select, options: assignmentTaskActionStatusTypes),
                FieldConfig.Create<AssignmentTaskActionModel>(nameof(AssignmentTaskActionModel.Notes), FieldType.Textarea, rows: 13, hideLabel: true, className: "no-label"),
                FieldConfig.Create<AssignmentTaskActionModel>(nameof(AssignmentTaskActionModel.AssignmentActionPriorityTypeId), FieldType.Select, options: assignmentTaskActionPriorityTypes),
                FieldConfig.Create<AssignmentTaskActionModel>(nameof(AssignmentTaskActionModel.EmployeeId), FieldType.GridSelect, options: employees, markAsRequired : true),
                FieldConfig.Create<AssignmentTaskActionModel>(nameof(AssignmentTaskActionModel.ExpiryDate), FieldType.Date),
                //FieldConfig.Create<AssignmentTaskActionModel>(nameof(AssignmentTaskActionModel.DisplayOrder), FieldType.Numeric)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6" }, left, right);

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.AssignmentTaskActionModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }

        public async Task<List<AssignmentTaskActionModel>> GetLoadListAsync(int parentId)
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var departments = await _departmentService.GetAllDepartmentsAsync();
            var assignmentActionStatusTypes = await AssignmentActionStatusType.InProgress.ToSelectionItemListAsync();
            var assignmentTaskActionPriorityTypes = await AssignmentActionPriorityType.Normal.ToSelectionItemListAsync();

            var query = _assignmentTaskActionService.Table.AsEnumerable()
                .Where(w => w.AssignmentTaskId == parentId)
                .Select(x =>
                {
                    var employee = employees.FirstOrDefault(e => e.Id == x.EmployeeId);
                    var departmentName = departments.FirstOrDefault(d => d.Id == (employee?.DepartmentId ?? 0))?.Description ?? "";
                    var model = x.ToModel<AssignmentTaskActionModel>();

                    model.AssignmentActionStatusTypeName = assignmentActionStatusTypes.FirstOrDefault(a => a.Value == x.AssignmentActionStatusTypeId)?.Label ?? "";
                    model.AssignmentActionPriorityTypeName = assignmentTaskActionPriorityTypes.FirstOrDefault(a => a.Value == x.AssignmentActionPriorityTypeId)?.Label ?? "";
                    model.EmployeeName = employee?.FullName() ?? "";
                    model.DepartmentName = departmentName;
                    model.LetterName = departmentName.Length > 0 ? char.ToUpperInvariant(departmentName[0]).ToString() : "";
                    model.Background = departments.FirstOrDefault(d => d.Id == (employee?.DepartmentId ?? 0))?.Background ?? "#ffffff";
                    model.Color = departments.FirstOrDefault(d => d.Id == (employee?.DepartmentId ?? 0))?.Color ?? "#008080";

                    return model;
                })
                .AsQueryable();

            var list = query.ToList();

            return list.OrderBy(x => x.DepartmentName).ThenBy(t => t.ExpiryDate).ToList();
        }

    }
}