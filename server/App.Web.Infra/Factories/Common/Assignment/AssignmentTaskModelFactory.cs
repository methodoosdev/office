using App.Core;
using App.Core.Domain.Assignment;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Assignment;
using App.Services;
using App.Services.Assignment;
using App.Services.Customers;
using App.Services.Localization;
using App.Services.Offices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Assignment
{
    public partial interface IAssignmentTaskModelFactory
    {
        Task<AssignmentTaskSearchModel> PrepareAssignmentTaskSearchModelAsync(AssignmentTaskSearchModel searchModel);
        Task<AssignmentTaskListModel> PrepareAssignmentTaskListModelAsync(AssignmentTaskSearchModel searchModel);
        Task<AssignmentTaskModel> PrepareAssignmentTaskModelAsync(AssignmentTaskModel model, AssignmentTask assignmentTask);
        Task<AssignmentTaskFormModel> PrepareAssignmentTaskFormModelAsync(AssignmentTaskFormModel formModel);
        Task<AssignmentTaskFilterFormModel> PrepareAssignmentTaskFilterFormModelAsync(AssignmentTaskFilterFormModel filterFormModel);
    }
    public partial class AssignmentTaskModelFactory : IAssignmentTaskModelFactory
    {
        private readonly ICustomerService _customerService;
        private readonly IAssignmentTaskService _assignmentTaskService;
        private readonly IAssignmentTaskActionService _assignmentTaskActionService;
        private readonly IAssignmentPrototypeService _assignmentPrototypeService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly IPersistStateService _persistStateService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public AssignmentTaskModelFactory(
            ICustomerService customerService,
            IAssignmentTaskService assignmentTaskService,
            IAssignmentTaskActionService assignmentTaskActionService,
            IAssignmentPrototypeService assignmentPrototypeService,
            IModelFactoryService modelFactoryService,
            IPersistStateService persistStateService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _customerService = customerService;
            _assignmentTaskService = assignmentTaskService;
            _assignmentTaskActionService = assignmentTaskActionService;
            _assignmentPrototypeService = assignmentPrototypeService;
            _modelFactoryService = modelFactoryService;
            _persistStateService = persistStateService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<AssignmentTaskModel>> GetPagedListAsync(AssignmentTaskSearchModel searchModel, AssignmentTaskFilterModel filterModel, bool filterExist)
        {
            var traders = await _modelFactoryService.GetAllTradersAsync(false);
            var employees = await _modelFactoryService.GetAllEmployeesAsync(false);
            var assignmentReasons = await _modelFactoryService.GetAllAssignmentReasonsAsync(false);
            var assignmentTaskActions = await _assignmentTaskActionService.GetAllAssignmentTaskActionsAsync();

            var query = _assignmentTaskService.Table
                .SelectAwait(async x =>
                {
                    var model = x.ToModel<AssignmentTaskModel>();
                    model.Description = x.Description ?? "";
                    model.Name = x.Name ?? "";
                    model.AssignmentReasonName = assignmentReasons.FirstOrDefault(a => a.Value == x.AssignmentReasonId)?.Label ?? "";
                    model.TraderName = traders.FirstOrDefault(t => t.Value == x.TraderId)?.Label ?? "";
                    model.AssignorName = employees.FirstOrDefault(t => t.Value == x.AssignorId)?.Label ?? "";

                    //status
                    var _assignmentTaskActions = await _assignmentTaskActionService.GetAllAssignmentTaskActionsAsync(x.Id);
                    var actions = _assignmentTaskActions
                        .Where(x => x.AssignmentActionStatusTypeId != (int)AssignmentActionStatusType.Canceled).ToList();
                    var _actions = actions
                        .Where(x => x.AssignmentActionStatusTypeId == (int)AssignmentActionStatusType.Completed).ToList();
                    model.AssignmentTaskStatus =
                        await _localizationService.GetLocalizedEnumAsync(
                            actions.Count == _actions.Count ? AssignmentActionStatusType.Completed : AssignmentActionStatusType.InProgress);

                    return model;
                });

            var customer = await _workContext.GetCurrentCustomerAsync();
            var roleIsOffice = await _customerService.IsOfficeAsync(customer);

            if (!roleIsOffice && customer.EmployeeId > 0)
            {
                var ids = assignmentTaskActions.Where(x => x.EmployeeId == customer.EmployeeId)
                    .Select(x => x.AssignmentTaskId).Distinct().ToList();

                query = query.Where(x => ids.Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.Name.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Description.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.AssignmentReasonName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.TraderName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.AssignmentTaskStatus.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.AssignorName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            if (filterExist)
            {
                if (!string.IsNullOrEmpty(filterModel.AssignmentTaskName))
                    query = query.Where(c => c.Name.ToUpperInvariant().Contains(filterModel.AssignmentTaskName.ToUpperInvariant()));

                if (filterModel.TraderId > 0)
                    query = query.Where(c => c.TraderId == filterModel.TraderId);

                if (filterModel.EmployeeId > 0)
                {
                    var ids = assignmentTaskActions.Where(x => x.EmployeeId == filterModel.EmployeeId)
                        .Select(x => x.AssignmentTaskId).Distinct().ToList();

                    query = query.Where(x => ids.Contains(x.Id));
                }

                if (filterModel.AssignmentActionPriorityTypeId.HasValue)
                {
                    var ids = assignmentTaskActions.Where(x => x.AssignmentActionPriorityTypeId == filterModel.AssignmentActionPriorityTypeId)
                        .Select(x => x.AssignmentTaskId).Distinct().ToList();

                    query = query.Where(x => ids.Contains(x.Id));
                }

                if (filterModel.AssignmentActionStatusTypeId.HasValue)
                {
                    var ids = assignmentTaskActions.Where(x => x.AssignmentActionStatusTypeId == filterModel.AssignmentActionStatusTypeId)
                        .Select(x => x.AssignmentTaskId).Distinct().ToList();

                    query = query.Where(x => ids.Contains(x.Id));
                }

                if (filterModel.AssignorId > 0)
                    query = query.Where(c => c.AssignorId == filterModel.AssignorId);

                if (filterModel.AssignmentReasonId > 0)
                    query = query.Where(c => c.AssignmentReasonId == filterModel.AssignmentReasonId);

                if (filterModel.ExpiryFrom.HasValue && filterModel.ExpiryTo.HasValue)
                {
                    query = query.Where(x => x.ExpiryDate >= filterModel.ExpiryFrom && x.ExpiryDate <= filterModel.ExpiryTo);
                }
            }

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<AssignmentTaskSearchModel> PrepareAssignmentTaskSearchModelAsync(AssignmentTaskSearchModel searchModel)
        {
            var persistState = await _persistStateService.GetModelInstance<AssignmentTaskSearchModel>();

            if (persistState.Exist)
                return persistState.Model;

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.AssignmentTaskModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<AssignmentTaskListModel> PrepareAssignmentTaskListModelAsync(AssignmentTaskSearchModel searchModel)
        {
            var filterState = await _persistStateService.GetModelInstance<AssignmentTaskFilterModel>();

            //get customer roles
            var assignmentTasks = await GetPagedListAsync(searchModel, filterState.Model, filterState.Exist);

            //prepare grid model
            var model = new AssignmentTaskListModel().PrepareToGrid(searchModel, assignmentTasks);
            model.FilterExist = filterState.Exist;

            return model;
        }

        public virtual Task<AssignmentTaskModel> PrepareAssignmentTaskModelAsync(AssignmentTaskModel model, AssignmentTask assignmentTask)
        {
            if (assignmentTask != null)
            {
                //fill in model values from the entity
                model ??= assignmentTask.ToModel<AssignmentTaskModel>();
            }

            if (assignmentTask == null)
            {
                model.CreatedDate = DateTime.UtcNow.Date;
                model.ExpiryDate = DateTime.UtcNow.Date.AddMonths(1);
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<AssignmentTaskModel>(0, nameof(AssignmentTaskModel.Name)),
                ColumnConfig.Create<AssignmentTaskModel>(1, nameof(AssignmentTaskModel.Description), hidden: true),
                ColumnConfig.Create<AssignmentTaskModel>(2, nameof(AssignmentTaskModel.AssignmentReasonName)),
                ColumnConfig.Create<AssignmentTaskModel>(3, nameof(AssignmentTaskModel.TraderName)),
                ColumnConfig.Create<AssignmentTaskModel>(4, nameof(AssignmentTaskModel.AssignorName)),
                ColumnConfig.Create<AssignmentTaskModel>(5, nameof(AssignmentTaskModel.CreatedDate), ColumnType.Date, style: centerAlign),
                ColumnConfig.Create<AssignmentTaskModel>(6, nameof(AssignmentTaskModel.ExpiryDate), ColumnType.Date, style: centerAlign),
                ColumnConfig.Create<AssignmentTaskModel>(7, nameof(AssignmentTaskModel.Rejection), ColumnType.Checkbox),
                ColumnConfig.Create<AssignmentTaskModel>(8, nameof(AssignmentTaskModel.AssignmentTaskStatus)),
            };

            return columns;
        }

        public virtual async Task<AssignmentTaskFormModel> PrepareAssignmentTaskFormModelAsync(AssignmentTaskFormModel formModel)
        {
            var traders = await _modelFactoryService.GetAllTradersAsync(false);
            var employees = await _modelFactoryService.GetAllEmployeesAsync(false);
            var assignmentPrototypes = (await _assignmentPrototypeService.GetAllAssignmentPrototypesAsync())
                .Select(x => new SelectionItemList(x.Id, x.Name)).ToList();
            var assignmentReasons = await _modelFactoryService.GetAllAssignmentReasonsAsync();

            var top = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentTaskModel>(nameof(AssignmentTaskModel.Name), FieldType.Textarea, rows: 2, hideExpression: "model.id == 0"),
                FieldConfig.Create<AssignmentTaskModel>(nameof(AssignmentTaskModel.Description), FieldType.Textarea, rows: 8, hideExpression: "model.id == 0"),
            };

            var bottom = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentTaskModel>(nameof(AssignmentTaskModel.AssignmentPrototypeId), FieldType.GridSelect, options: assignmentPrototypes, markAsRequired: true, hideExpression: "model.id > 0"),
                FieldConfig.Create<AssignmentTaskModel>(nameof(AssignmentTaskModel.TraderId), FieldType.GridSelect, options: traders, markAsRequired: true, disableExpression: "model.id > 0"),
                FieldConfig.Create<AssignmentTaskModel>(nameof(AssignmentTaskModel.AssignmentReasonId), FieldType.Select, options: assignmentReasons),
                FieldConfig.Create<AssignmentTaskModel>(nameof(AssignmentTaskModel.CreatedDate), FieldType.Date),
                FieldConfig.Create<AssignmentTaskModel>(nameof(AssignmentTaskModel.ExpiryDate), FieldType.Date),
                FieldConfig.Create<AssignmentTaskModel>(nameof(AssignmentTaskModel.AssignorId), FieldType.GridSelect, options: employees),
                FieldConfig.Create<AssignmentTaskModel>(nameof(AssignmentTaskModel.Rejection), FieldType.Checkbox)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12", "col-12 md:col-6" }, top, bottom);

            var panels = new List<Dictionary<string, object>>()
            {
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Common.About"), true, "col-12", fields)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.AssignmentTaskModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", panels);

            return formModel;
        }

        public virtual async Task<AssignmentTaskFilterFormModel> PrepareAssignmentTaskFilterFormModelAsync(AssignmentTaskFilterFormModel filterFormModel)
        {
            var traders = await _modelFactoryService.GetAllTradersAsync(false);
            var employees = await _modelFactoryService.GetAllEmployeesAsync(false);
            var assignors = await _assignmentTaskService.GetAllAssignorsAsync();

            var assignmentTaskActionStatusTypes = await AssignmentActionStatusType.InProgress.ToSelectionItemListAsync();
            var assignmentTaskActionPriorityTypes = await AssignmentActionPriorityType.Normal.ToSelectionItemListAsync();

            var assignmentReasons = await _modelFactoryService.GetAllAssignmentReasonsAsync(false);

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentTaskFilterModel>(nameof(AssignmentTaskFilterModel.AssignmentTaskName), FieldType.Text),
                FieldConfig.Create<AssignmentTaskFilterModel>(nameof(AssignmentTaskFilterModel.TraderId), FieldType.GridSelect, options: traders),
                FieldConfig.Create<AssignmentTaskFilterModel>(nameof(AssignmentTaskFilterModel.EmployeeId), FieldType.GridSelect, options: employees),
                FieldConfig.Create<AssignmentTaskFilterModel>(nameof(AssignmentTaskFilterModel.AssignmentActionStatusTypeId), FieldType.Select, options: assignmentTaskActionStatusTypes),
                FieldConfig.Create<AssignmentTaskFilterModel>(nameof(AssignmentTaskFilterModel.AssignmentActionPriorityTypeId), FieldType.Select, options: assignmentTaskActionPriorityTypes),
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentTaskFilterModel>(nameof(AssignmentTaskFilterModel.AssignorId), FieldType.GridSelect, options: assignors),
                FieldConfig.Create<AssignmentTaskFilterModel>(nameof(AssignmentTaskFilterModel.AssignmentReasonId), FieldType.GridSelect, options: assignmentReasons),
                FieldConfig.Create<AssignmentTaskFilterModel>(nameof(AssignmentTaskFilterModel.ExpiryFrom), FieldType.Date),
                FieldConfig.Create<AssignmentTaskFilterModel>(nameof(AssignmentTaskFilterModel.ExpiryTo), FieldType.Date)
            };

            var saveState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentTaskFilterModel>("SaveState", FieldType.Button, themeColor: "primary",
                label: await _localizationService.GetResourceAsync("App.Common.SaveState"))
            };

            var removeState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentTaskFilterModel>("RemoveState", FieldType.Button, themeColor: "warning",
                label: await _localizationService.GetResourceAsync("App.Common.RemoveState"))
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6", "col-12 md:col-2", "col-12 md:col-2" }, left, right, saveState, removeState);

            filterFormModel.CustomProperties.Add("fields", fields);

            return filterFormModel;
        }
    }
}