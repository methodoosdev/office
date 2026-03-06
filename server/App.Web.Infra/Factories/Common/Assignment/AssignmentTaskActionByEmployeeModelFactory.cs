using App.Core;
using App.Core.Domain.Assignment;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Assignment;
using App.Models.Traders;
using App.Services;
using App.Services.Assignment;
using App.Services.Employees;
using App.Services.Localization;
using App.Services.Offices;
using App.Services.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Assignment
{
    public partial interface IAssignmentTaskActionByEmployeeModelFactory
    {
        Task<AssignmentTaskActionSearchModel> PrepareAssignmentTaskActionSearchModelAsync(AssignmentTaskActionSearchModel searchModel);
        Task<AssignmentTaskActionListModel> PrepareAssignmentTaskActionListModelAsync(AssignmentTaskActionSearchModel searchModel, int employeeId);
        Task<AssignmentTaskActionModel> PrepareAssignmentTaskActionModelAsync(AssignmentTaskActionModel model, AssignmentTaskAction assignmentTaskAction);
        Task<AssignmentTaskActionFormModel> PrepareAssignmentTaskActionFormModelAsync(AssignmentTaskActionFormModel formModel);
        Task<AssignmentTaskActionFilterFormModel> PrepareAssignmentTaskActionFilterFormModelAsync(AssignmentTaskActionFilterFormModel filterFormModel);
    }
    public partial class AssignmentTaskActionByEmployeeModelFactory : IAssignmentTaskActionByEmployeeModelFactory
    {
        private readonly IEmployeeService _employeeService;
        private readonly ITraderService _traderService;
        private readonly IAssignmentTaskService _assignmentTaskService;
        private readonly IAssignmentTaskActionService _assignmentTaskActionService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly IPersistStateService _persistStateService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public AssignmentTaskActionByEmployeeModelFactory(
            IEmployeeService employeeService,
            ITraderService traderService,
            IAssignmentTaskService assignmentTaskService,
            IAssignmentTaskActionService assignmentTaskActionService,
            IModelFactoryService modelFactoryService,
            IPersistStateService persistStateService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _employeeService = employeeService;
            _traderService = traderService;
            _assignmentTaskService = assignmentTaskService;
            _assignmentTaskActionService = assignmentTaskActionService;
            _modelFactoryService = modelFactoryService;
            _persistStateService = persistStateService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<AssignmentTaskActionModel>> GetPagedListAsync(AssignmentTaskActionSearchModel searchModel, int employeeId, AssignmentTaskActionFilterModel filterModel, bool filterExist)
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var departments = await _modelFactoryService.GetAllDepartmentsAsync(false);
            var assignmentActionStatusTypes = await AssignmentActionStatusType.InProgress.ToSelectionItemListAsync();
            var assignmentTaskActionPriorityTypes = await AssignmentActionPriorityType.Normal.ToSelectionItemListAsync();

            var query = _assignmentTaskActionService.Table
                .Where(x => x.EmployeeId == employeeId)
                .SelectAwait(async x =>
                {
                    var employee = employees.FirstOrDefault(e => e.Id == x.EmployeeId);
                    var assignmentTask = await _assignmentTaskService.GetAssignmentTaskByIdAsync(x.AssignmentTaskId);
                    var model = x.ToModel<AssignmentTaskActionModel>();
                    var trader = await _traderService.GetTraderByIdAsync(assignmentTask.TraderId);

                    model.ActionDescription = x.ActionDescription ?? "";
                    model.AssignmentActionStatusTypeName = assignmentActionStatusTypes.FirstOrDefault(a => a.Value == x.AssignmentActionStatusTypeId)?.Label ?? "";
                    model.AssignmentActionPriorityTypeName = assignmentTaskActionPriorityTypes.FirstOrDefault(a => a.Value == x.AssignmentActionPriorityTypeId)?.Label ?? "";
                    model.EmployeeName = employee?.FullName() ?? "";
                    model.DepartmentName = departments.FirstOrDefault(d => d.Value == (employee?.DepartmentId ?? 0))?.Label ?? "";
                    model.AssignmentTaskName = assignmentTask.Name;
                    model.TraderId = assignmentTask.TraderId;
                    model.TraderName = trader.ToTraderFullName();
                    model.AssignorName = (await _employeeService.GetEmployeeByIdAsync(assignmentTask.AssignorId)).FullName();

                    return model;
                });

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.ActionName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.ActionDescription.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.AssignmentActionStatusTypeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.AssignmentActionPriorityTypeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.EmployeeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.DepartmentName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.AssignmentTaskName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.TraderName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            if (filterExist)
            {
                if (!string.IsNullOrEmpty(filterModel.AssignmentTaskName))
                    query = query.Where(c => c.AssignmentTaskName.ToUpperInvariant().Contains(filterModel.AssignmentTaskName.ToUpperInvariant()));

                if (!string.IsNullOrEmpty(filterModel.ActionName))
                    query = query.Where(c => c.ActionName.ToUpperInvariant().Contains(filterModel.ActionName.ToUpperInvariant()));

                if (filterModel.TraderId > 0)
                    query = query.Where(c => c.TraderId == filterModel.TraderId);

                if (filterModel.AssignmentActionPriorityTypeId.HasValue)
                    query = query.Where(c => c.AssignmentActionPriorityTypeId == filterModel.AssignmentActionPriorityTypeId.Value);

                if (filterModel.AssignmentActionStatusTypeId.HasValue)
                    query = query.Where(x => x.AssignmentActionStatusTypeId == filterModel.AssignmentActionStatusTypeId.Value);

                if (filterModel.AssignorId > 0)
                    query = query.Where(c => c.AssignorId == filterModel.AssignorId);

                if (filterModel.ExpiryFrom.HasValue && filterModel.ExpiryTo.HasValue)
                    query = query.Where(x => x.ExpiryDate >= filterModel.ExpiryFrom && x.ExpiryDate <= filterModel.ExpiryTo);
            }

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<AssignmentTaskActionSearchModel> PrepareAssignmentTaskActionSearchModelAsync(AssignmentTaskActionSearchModel searchModel)
        {
            var persistState = await _persistStateService.GetModelInstance<AssignmentTaskActionSearchModel>();

            if (persistState.Exist)
                return persistState.Model;

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.AssignmentTaskActionModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<AssignmentTaskActionListModel> PrepareAssignmentTaskActionListModelAsync(AssignmentTaskActionSearchModel searchModel, int employeeId)
        {
            var filterState = await _persistStateService.GetModelInstance<AssignmentTaskActionFilterModel>();

            //get customer roles
            var assignmentTaskActions = await GetPagedListAsync(searchModel, employeeId, filterState.Model, filterState.Exist);

            //prepare grid model
            var model = new AssignmentTaskActionListModel().PrepareToGrid(searchModel, assignmentTaskActions);
            model.FilterExist = filterState.Exist;

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

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<AssignmentTaskActionModel>(0, nameof(AssignmentTaskActionModel.AssignmentTaskName)),
                ColumnConfig.Create<AssignmentTaskActionModel>(0, nameof(AssignmentTaskActionModel.ActionName)),
                ColumnConfig.Create<AssignmentTaskActionModel>(1, nameof(AssignmentTaskActionModel.ActionDescription)),
                ColumnConfig.Create<AssignmentTaskActionModel>(2, nameof(AssignmentTaskActionModel.AssignmentActionStatusTypeName)),
                ColumnConfig.Create<AssignmentTaskActionModel>(2, nameof(AssignmentTaskActionModel.AssignmentActionPriorityTypeName)),
                ColumnConfig.Create<AssignmentTaskActionModel>(3, nameof(AssignmentTaskActionModel.TraderName)),
                ColumnConfig.Create<AssignmentTaskActionModel>(3, nameof(AssignmentTaskActionModel.EmployeeName), hidden: true),
                ColumnConfig.Create<AssignmentTaskActionModel>(3, nameof(AssignmentTaskActionModel.AssignorName), hidden: true),
                ColumnConfig.Create<AssignmentTaskActionModel>(4, nameof(AssignmentTaskActionModel.DepartmentName), hidden: true),
                ColumnConfig.Create<AssignmentTaskActionModel>(5, nameof(AssignmentTaskActionModel.ExpiryDate), ColumnType.Date, style: centerAlign),
                ColumnConfig.Create<AssignmentTaskActionModel>(6, nameof(AssignmentTaskActionModel.DisplayOrder), hidden: true, style: rightAlign)
            };
            return columns;
        }

        public virtual async Task<AssignmentTaskActionFormModel> PrepareAssignmentTaskActionFormModelAsync(AssignmentTaskActionFormModel formModel)
        {
            var employees = await _modelFactoryService.GetAllEmployeesAsync(false);
            var assignmentTaskActionStatusTypes = await AssignmentActionStatusType.InProgress.ToSelectionItemListAsync();
            var assignmentTaskActionPriorityTypes = await AssignmentActionPriorityType.Normal.ToSelectionItemListAsync();

            var top = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentTaskActionModel>(nameof(AssignmentTaskActionModel.ActionName), FieldType.Textarea, rows: 2, hideLabel: true, className: "no-label", disableExpression: "true"),
                FieldConfig.Create<AssignmentTaskActionModel>(nameof(AssignmentTaskActionModel.ActionDescription), FieldType.Textarea, rows: 8, hideLabel: true, className: "no-label", disableExpression: "true"),
                FieldConfig.Create<AssignmentTaskActionModel>(nameof(AssignmentTaskActionModel.Notes), FieldType.Textarea, rows: 8, hideLabel: true, className: "no-label")
            };

            var bottom = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentTaskActionModel>(nameof(AssignmentTaskActionModel.AssignmentActionStatusTypeId), FieldType.Select, options: assignmentTaskActionStatusTypes),
                FieldConfig.Create<AssignmentTaskActionModel>(nameof(AssignmentTaskActionModel.AssignmentActionPriorityTypeId), FieldType.Select, options: assignmentTaskActionPriorityTypes, disableExpression: "true"),
                FieldConfig.Create<AssignmentTaskActionModel>(nameof(AssignmentTaskActionModel.EmployeeId), FieldType.GridSelect, options: employees, markAsRequired: true, disableExpression: "true"),
                FieldConfig.Create<AssignmentTaskActionModel>(nameof(AssignmentTaskActionModel.ExpiryDate), FieldType.Date, disableExpression: "true"),
                FieldConfig.Create<AssignmentTaskActionModel>(nameof(AssignmentTaskActionModel.DisplayOrder), FieldType.Numeric, disableExpression: "true")
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12", "col-12 md:col-6" }, top, bottom);

            var panels = new List<Dictionary<string, object>>()
            {
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Common.About"), true, "col-12", fields)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.AssignmentTaskActionModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", panels);

            return formModel;
        }

        public virtual async Task<AssignmentTaskActionFilterFormModel> PrepareAssignmentTaskActionFilterFormModelAsync(AssignmentTaskActionFilterFormModel filterFormModel)
        {
            var traders = await _modelFactoryService.GetAllTradersAsync(false);
            var assignors = await _assignmentTaskService.GetAllAssignorsAsync();

            var assignmentTaskActionStatusTypes = await AssignmentActionStatusType.InProgress.ToSelectionItemListAsync();
            var assignmentTaskActionPriorityTypes = await AssignmentActionPriorityType.Normal.ToSelectionItemListAsync();

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentTaskActionFilterModel>(nameof(AssignmentTaskActionFilterModel.AssignmentTaskName), FieldType.Text),
                FieldConfig.Create<AssignmentTaskActionFilterModel>(nameof(AssignmentTaskActionFilterModel.ActionName), FieldType.Text),
                FieldConfig.Create<AssignmentTaskActionFilterModel>(nameof(AssignmentTaskActionFilterModel.AssignmentActionStatusTypeId), FieldType.Select, options: assignmentTaskActionStatusTypes),
                FieldConfig.Create<AssignmentTaskActionFilterModel>(nameof(AssignmentTaskActionFilterModel.AssignmentActionPriorityTypeId), FieldType.Select, options: assignmentTaskActionPriorityTypes),
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentTaskActionFilterModel>(nameof(AssignmentTaskActionFilterModel.TraderId), FieldType.GridSelect, options: traders),
                FieldConfig.Create<AssignmentTaskActionFilterModel>(nameof(AssignmentTaskActionFilterModel.AssignorId), FieldType.GridSelect, options: assignors),
                FieldConfig.Create<AssignmentTaskActionFilterModel>(nameof(AssignmentTaskActionFilterModel.ExpiryFrom), FieldType.Date),
                FieldConfig.Create<AssignmentTaskActionFilterModel>(nameof(AssignmentTaskActionFilterModel.ExpiryTo), FieldType.Date)
            };

            var saveState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentTaskActionFilterModel>("SaveState", FieldType.Button, themeColor: "primary",
                label: await _localizationService.GetResourceAsync("App.Common.SaveState"))
            };

            var removeState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentTaskActionFilterModel>("RemoveState", FieldType.Button, themeColor: "warning",
                label: await _localizationService.GetResourceAsync("App.Common.RemoveState"))
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6", "col-12 md:col-2", "col-12 md:col-2" }, left, right, saveState, removeState);

            filterFormModel.CustomProperties.Add("fields", fields);

            return filterFormModel;
        }
    }
}