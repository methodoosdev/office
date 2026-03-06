using App.Core;
using App.Core.Domain.Common;
using App.Core.Domain.SimpleTask;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Common;
using App.Models.Payroll;
using App.Models.SimpleTask;
using App.Models.Traders;
using App.Services;
using App.Services.Employees;
using App.Services.Localization;
using App.Services.Offices;
using App.Services.SimpleTask;
using App.Services.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.SimpleTask
{
    public partial interface ISimpleTaskManagerModelFactory
    {
        Task<SimpleTaskManagerSearchModel> PrepareSimpleTaskManagerSearchModelAsync(SimpleTaskManagerSearchModel searchModel);
        Task<SimpleTaskManagerListModel> PrepareSimpleTaskManagerListModelAsync(SimpleTaskManagerSearchModel searchModel);
        Task<SimpleTaskManagerModel> PrepareSimpleTaskManagerModelAsync(SimpleTaskManagerModel model, SimpleTaskManager simpleTaskManager);
        Task<SimpleTaskManagerFormModel> PrepareSimpleTaskManagerFormModelAsync(SimpleTaskManagerFormModel formModel);
        Task<SimpleTaskManagerFilterFormModel> PrepareSimpleTaskManagerFilterFormModelAsync(SimpleTaskManagerFilterFormModel filterFormModel);
    }
    public partial class SimpleTaskManagerModelFactory : ISimpleTaskManagerModelFactory
    {
        private readonly ISimpleTaskCategoryService _simpleTaskCategoryService;
        private readonly ISimpleTaskDepartmentService _simpleTaskDepartmentService;
        private readonly ISimpleTaskNatureService _simpleTaskNatureService;
        private readonly ISimpleTaskSectorService _simpleTaskSectorService;
        private readonly ISimpleTaskManagerService _simpleTaskManagerService;
        private readonly IEmployeeService _employeeService;
        private readonly ITraderService _traderService;
        private readonly ILocalizationService _localizationService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly IPersistStateService _persistStateService;
        private readonly IWorkContext _workContext;

        public SimpleTaskManagerModelFactory(ISimpleTaskCategoryService simpleTaskCategoryService,
            ISimpleTaskDepartmentService simpleTaskDepartmentService,
            ISimpleTaskNatureService simpleTaskNatureService,
            ISimpleTaskSectorService simpleTaskSectorService,
            ISimpleTaskManagerService simpleTaskManagerService,
            IEmployeeService employeeService,
            ITraderService traderService,
            ILocalizationService localizationService,
            IModelFactoryService modelFactoryService,
            IPersistStateService persistStateService,
            IWorkContext workContext)
        {
            _simpleTaskCategoryService = simpleTaskCategoryService;
            _simpleTaskDepartmentService = simpleTaskDepartmentService;
            _simpleTaskNatureService = simpleTaskNatureService;
            _simpleTaskSectorService = simpleTaskSectorService;
            _simpleTaskManagerService = simpleTaskManagerService;
            _employeeService = employeeService;
            _traderService = traderService;
            _localizationService = localizationService;
            _modelFactoryService = modelFactoryService;
            _persistStateService = persistStateService;
            _workContext = workContext;
        }

        private async Task<IPagedList<SimpleTaskManagerModel>> GetPagedListAsync(SimpleTaskManagerSearchModel searchModel, SimpleTaskManagerFilterModel filterModel, bool filterExist)
        {
            var simpleTaskPriorityTypes = await SimpleTaskPriorityType.Normal.ToSelectionItemListAsync();
            var simpleTaskTypes = await SimpleTaskType.ToStart.ToSelectionItemListAsync();

            var query = _simpleTaskManagerService.Table.SelectAwait(async x =>
            {
                var model = x.ToModel<SimpleTaskManagerModel>();
                var trader = await _traderService.GetTraderByIdAsync(model.TraderId);

                var simpleTaskCategoryId = model.SimpleTaskCategoryId.HasValue ? model.SimpleTaskCategoryId.Value : 0;
                var simpleTaskDepartmentId = model.SimpleTaskDepartmentId.HasValue ? model.SimpleTaskDepartmentId.Value : 0;
                var simpleTaskNatureId = model.SimpleTaskNatureId.HasValue ? model.SimpleTaskNatureId.Value : 0;
                var simpleTaskSectorId = model.SimpleTaskSectorId.HasValue ? model.SimpleTaskSectorId.Value : 0;

                model.AssignorName = (await _employeeService.GetEmployeeByIdAsync(model.AssignorId))?.FullName() ?? "";
                model.EmployeeName = (await _employeeService.GetEmployeeByIdAsync(model.EmployeeId))?.FullName() ?? "";
                model.TraderName = trader?.ToTraderFullName() ?? "";
                model.SimpleTaskPriorityTypeName = simpleTaskPriorityTypes.Where(x => x.Value == model.SimpleTaskPriorityTypeId).FirstOrDefault()?.Label ?? "";
                model.SimpleTaskTypeName = simpleTaskTypes.Where(x => x.Value == model.SimpleTaskTypeId).FirstOrDefault()?.Label ?? "";
                model.SimpleTaskCategoryName = (await _simpleTaskCategoryService.GetSimpleTaskCategoryByIdAsync(simpleTaskCategoryId))?.Description ?? "";
                model.SimpleTaskDepartmentName = (await _simpleTaskDepartmentService.GetSimpleTaskDepartmentByIdAsync(simpleTaskDepartmentId))?.Description ?? "";
                model.SimpleTaskNatureName = (await _simpleTaskNatureService.GetSimpleTaskNatureByIdAsync(simpleTaskNatureId))?.Description ?? "";
                model.SimpleTaskSectorName = (await _simpleTaskSectorService.GetSimpleTaskSectorByIdAsync(simpleTaskSectorId))?.Description ?? "";

                int priorityIndex = Array.IndexOf(Enum.GetValues(typeof(SimpleTaskPriorityType)), (SimpleTaskPriorityType)x.SimpleTaskPriorityTypeId);
                int priorityValue = 95 - priorityIndex * 10;
                string priorityBackground = $"hsl(180,100%,{priorityValue}%)";

                int taskIndex = Array.IndexOf(Enum.GetValues(typeof(SimpleTaskType)), (SimpleTaskType)x.SimpleTaskTypeId);
                int taskValue = 95 - taskIndex * 10;
                string taskBackground = $"hsl(90,100%,{taskValue}%)";

                model.SimpleTaskPriorityTypeBackground = priorityBackground;
                model.SimpleTaskTypeBackground = taskBackground;

                return model;
            });

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.Name.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.AssignorName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.EmployeeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.TraderName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.SimpleTaskPriorityTypeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.SimpleTaskTypeName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            if (filterExist)
            {
                if (!string.IsNullOrEmpty(filterModel.Name))
                    query = query.Where(c => c.Name.ContainsIgnoreCase(filterModel.Name));

                if (filterModel.AssignorId > 0)
                    query = query.Where(c => c.AssignorId == filterModel.AssignorId);

                if (filterModel.EmployeeId > 0)
                    query = query.Where(c => c.EmployeeId == filterModel.EmployeeId);

                if (filterModel.TraderId > 0)
                    query = query.Where(c => c.TraderId == filterModel.TraderId);

                if (filterModel.SimpleTaskPriorityTypeId.Count > 0)
                {
                    var simpleTaskPriorityType = (SimpleTaskPriorityType)filterModel.SimpleTaskPriorityTypeId.Sum(x => x);
                    query = query.Where(x => simpleTaskPriorityType.HasFlag((SimpleTaskPriorityType)x.SimpleTaskPriorityTypeId));
                }

                if (filterModel.SimpleTaskTypeId.Count > 0)
                {
                    var simpleTaskType = (SimpleTaskType)filterModel.SimpleTaskTypeId.Sum(x => x);
                    query = query.Where(x => simpleTaskType.HasFlag((SimpleTaskType)x.SimpleTaskTypeId));
                }

                if (filterModel.SimpleTaskCategoryId > 0)
                    query = query.Where(c => c.SimpleTaskCategoryId == filterModel.SimpleTaskCategoryId);

                if (filterModel.SimpleTaskDepartmentId > 0)
                    query = query.Where(c => c.SimpleTaskDepartmentId == filterModel.SimpleTaskDepartmentId);

                if (filterModel.SimpleTaskNatureId > 0)
                    query = query.Where(c => c.SimpleTaskNatureId == filterModel.SimpleTaskNatureId);

                if (filterModel.SimpleTaskSectorId > 0)
                    query = query.Where(c => c.SimpleTaskSectorId == filterModel.SimpleTaskSectorId);
            }

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<SimpleTaskManagerSearchModel> PrepareSimpleTaskManagerSearchModelAsync(SimpleTaskManagerSearchModel searchModel)
        {
            var persistState = await _persistStateService.GetModelInstance<SimpleTaskManagerSearchModel>();

            if (persistState.Exist)
                return persistState.Model;

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.SimpleTaskManagerModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<SimpleTaskManagerListModel> PrepareSimpleTaskManagerListModelAsync(SimpleTaskManagerSearchModel searchModel)
        {
            var filterState = await _persistStateService.GetModelInstance<SimpleTaskManagerFilterModel>();

            //get customer roles
            var simpleTaskManagers = await GetPagedListAsync(searchModel, filterState.Model, filterState.Exist);

            //prepare grid model
            var model = new SimpleTaskManagerListModel().PrepareToGrid(searchModel, simpleTaskManagers);
            model.FilterExist = filterState.Exist;

            return model;
        }

        public virtual async Task<SimpleTaskManagerModel> PrepareSimpleTaskManagerModelAsync(SimpleTaskManagerModel model, SimpleTaskManager simpleTaskManager)
        {
            if (simpleTaskManager != null)
            {
                //fill in model values from the entity
                model ??= simpleTaskManager.ToModel<SimpleTaskManagerModel>();
            }

            if (simpleTaskManager == null)
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                var date = DateTime.Now;

                model.CustomerId = customer.Id;
                model.StartingDate = date.ToUtcRelative(false);
                model.EndingDate = date.ToUtcRelative(false);
                model.CreatedDate = date.ToUtcRelative();

                model.SimpleTaskPriorityTypeId = 4;
                model.SimpleTaskTypeId = 2;
            }

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<SimpleTaskManagerModel>(1, nameof(SimpleTaskManagerModel.EndingDate), ColumnType.Date),
                ColumnConfig.Create<SimpleTaskManagerModel>(2, nameof(SimpleTaskManagerModel.Name), ColumnType.RouterLink),
                ColumnConfig.Create<SimpleTaskManagerModel>(3, nameof(SimpleTaskManagerModel.AssignorName)),
                ColumnConfig.Create<SimpleTaskManagerModel>(4, nameof(SimpleTaskManagerModel.EmployeeName)),
                ColumnConfig.Create<SimpleTaskManagerModel>(5, nameof(SimpleTaskManagerModel.TraderName)),
                ColumnConfig.Create<SimpleTaskManagerModel>(6, nameof(SimpleTaskManagerModel.SimpleTaskPriorityTypeName), ColumnType.Colorized, backgroundField: "simpleTaskPriorityTypeBackground", _class: "no-padding"),
                ColumnConfig.Create<SimpleTaskManagerModel>(7, nameof(SimpleTaskManagerModel.SimpleTaskTypeName), ColumnType.Colorized, backgroundField: "simpleTaskTypeBackground", _class: "no-padding"),
                ColumnConfig.Create<SimpleTaskManagerModel>(8, nameof(SimpleTaskManagerModel.SimpleTaskCategoryName), hidden: true),
                ColumnConfig.Create<SimpleTaskManagerModel>(9, nameof(SimpleTaskManagerModel.SimpleTaskDepartmentName), hidden: true),
                ColumnConfig.Create<SimpleTaskManagerModel>(10, nameof(SimpleTaskManagerModel.SimpleTaskNatureName), hidden: true),
                ColumnConfig.Create<SimpleTaskManagerModel>(11, nameof(SimpleTaskManagerModel.SimpleTaskSectorName), hidden: true),

                ColumnConfig.Create<SimpleTaskManagerModel>(12, nameof(SimpleTaskManagerModel.StartingDate), ColumnType.Date, hidden: true),
                ColumnConfig.Create<SimpleTaskManagerModel>(13, nameof(SimpleTaskManagerModel.CreatedDate), ColumnType.Date, hidden: true),

                ColumnConfig.Create<SimpleTaskManagerModel>(14, nameof(SimpleTaskManagerModel.RelateTo), hidden: true),
                ColumnConfig.Create<SimpleTaskManagerModel>(15, nameof(SimpleTaskManagerModel.GovService), hidden: true),
                ColumnConfig.Create<SimpleTaskManagerModel>(16, nameof(SimpleTaskManagerModel.LawType), hidden: true),
                ColumnConfig.Create<SimpleTaskManagerModel>(17, nameof(SimpleTaskManagerModel.DocumentType), hidden: true)
            };

            return columns;
        }

        public virtual async Task<SimpleTaskManagerFormModel> PrepareSimpleTaskManagerFormModelAsync(SimpleTaskManagerFormModel formModel)
        {
            var employees = await _modelFactoryService.GetAllEmployeesAsync();
            var traders = await _modelFactoryService.GetAllTradersAsync(false);

            var categories = await _modelFactoryService.GetAllSimpleTaskCategoriesAsync();
            var departments = await _modelFactoryService.GetAllSimpleTaskDepartmentsAsync();
            var natures = await _modelFactoryService.GetAllSimpleTaskNaturesAsync();
            var sectors = await _modelFactoryService.GetAllSimpleTaskSectorsAsync();

            var simpleTaskPriorityTypes = await SimpleTaskPriorityType.Normal.ToSelectionItemListAsync();
            var simpleTaskTypes = await SimpleTaskType.ToStart.ToSelectionItemListAsync();

            var about = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.Name), FieldType.Textarea, rows: 2, markAsRequired: true)
            };

            var assignment1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.AssignorId), FieldType.Select, options: employees, markAsRequired: true),
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.EmployeeId), FieldType.Select, options: employees)
            };

            var assignment2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.StartingDate), FieldType.Date, markAsRequired: true),
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.EndingDate), FieldType.Date, markAsRequired: true)
            };

            var trader1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.TraderId), FieldType.GridSelect, options: traders),
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.Branch), FieldType.Text),
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.Contact), FieldType.Text)
            };

            var trader2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.CreatedDate), FieldType.Date, markAsRequired: true),
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.SimpleTaskPriorityTypeId), FieldType.Select, options: simpleTaskPriorityTypes),
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.SimpleTaskTypeId), FieldType.Select, options: simpleTaskTypes)
            };

            var general1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.SimpleTaskCategoryId), FieldType.Select, options: categories),
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.SimpleTaskDepartmentId), FieldType.Select, options: departments),
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.SimpleTaskNatureId), FieldType.Select, options: natures),
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.SimpleTaskSectorId), FieldType.Select, options: sectors)
            };

            var general2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.RelateTo), FieldType.Text),
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.GovService), FieldType.Text),
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.LawType), FieldType.Text),
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.DocumentType), FieldType.Text)
            };

            var notes = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.Notes), FieldType.Textarea)
            };

            var participants = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<SimpleTaskManagerModel>(nameof(SimpleTaskManagerModel.Participants), FieldType.Textarea)
            };

            var sections = new List<Dictionary<string, object>>()
            {
                FieldConfig.CreateSection(await _localizationService.GetResourceAsync("App.Models.SimpleTaskManagerModel.Sections.About"), new string[] { "col-12 md:col-6" }, about),
                FieldConfig.CreateSection(await _localizationService.GetResourceAsync("App.Models.SimpleTaskManagerModel.Sections.Assignment"), new string[] { "col-12 md:col-6", "col-12 md:col-6" }, assignment1, assignment2),
                FieldConfig.CreateSection(await _localizationService.GetResourceAsync("App.Models.SimpleTaskManagerModel.Sections.Traders"), new string[] { "col-12 md:col-6", "col-12 md:col-6" }, trader1, trader2),
                FieldConfig.CreateSection(await _localizationService.GetResourceAsync("App.Models.SimpleTaskManagerModel.Sections.GeneralItems"), new string[] { "col-12 md:col-6", "col-12 md:col-6" }, general1, general2),
                FieldConfig.CreateSection(await _localizationService.GetResourceAsync("App.Models.SimpleTaskManagerModel.Sections.Participants"), new string[] { "col-12 md:col-6", "col-12 md:col-6" }, notes, participants)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.SimpleTaskManagerModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", sections);

            return formModel;
        }

        public virtual async Task<SimpleTaskManagerFilterFormModel> PrepareSimpleTaskManagerFilterFormModelAsync(SimpleTaskManagerFilterFormModel filterFormModel)
        {
            var employees = await _modelFactoryService.GetAllEmployeesAsync();
            var traders = await _modelFactoryService.GetAllTradersAsync();

            var categories = await _modelFactoryService.GetAllSimpleTaskCategoriesAsync();
            var departments = await _modelFactoryService.GetAllSimpleTaskDepartmentsAsync();
            var natures = await _modelFactoryService.GetAllSimpleTaskNaturesAsync();
            var sectors = await _modelFactoryService.GetAllSimpleTaskSectorsAsync();

            var simpleTaskPriorityTypes = await SimpleTaskPriorityType.Normal.ToSelectionItemListAsync();
            var simpleTaskTypes = await SimpleTaskType.ToStart.ToSelectionItemListAsync();

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<SimpleTaskManagerFilterModel>(nameof(SimpleTaskManagerFilterModel.Name), FieldType.Text),
                FieldConfig.Create<SimpleTaskManagerFilterModel>(nameof(SimpleTaskManagerFilterModel.AssignorId), FieldType.Select, options: employees),
                FieldConfig.Create<SimpleTaskManagerFilterModel>(nameof(SimpleTaskManagerFilterModel.EmployeeId), FieldType.Select, options: employees),
                FieldConfig.Create<SimpleTaskManagerFilterModel>(nameof(SimpleTaskManagerFilterModel.SimpleTaskPriorityTypeId), FieldType.MultiSelect, options: simpleTaskPriorityTypes),
                FieldConfig.Create<SimpleTaskManagerFilterModel>(nameof(SimpleTaskManagerFilterModel.SimpleTaskTypeId), FieldType.MultiSelect, options: simpleTaskTypes)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<SimpleTaskManagerFilterModel>(nameof(SimpleTaskManagerFilterModel.TraderId), FieldType.GridSelect, options: traders),
                FieldConfig.Create<SimpleTaskManagerFilterModel>(nameof(SimpleTaskManagerFilterModel.SimpleTaskCategoryId), FieldType.Select, options: categories),
                FieldConfig.Create<SimpleTaskManagerFilterModel>(nameof(SimpleTaskManagerFilterModel.SimpleTaskDepartmentId), FieldType.Select, options: departments),
                FieldConfig.Create<SimpleTaskManagerFilterModel>(nameof(SimpleTaskManagerFilterModel.SimpleTaskNatureId), FieldType.Select, options: natures),
                FieldConfig.Create<SimpleTaskManagerFilterModel>(nameof(SimpleTaskManagerFilterModel.SimpleTaskSectorId), FieldType.Select, options: sectors)
            };

            var saveState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerScheduleFilterModel>("SaveState", FieldType.Button, themeColor: "primary",
                label: await _localizationService.GetResourceAsync("App.Common.SaveState"))
            };

            var removeState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<WorkerScheduleFilterModel>("RemoveState", FieldType.Button, themeColor: "warning",
                label: await _localizationService.GetResourceAsync("App.Common.RemoveState"))
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6", "col-12 md:col-2", "col-12 md:col-2" }, left, right, saveState, removeState);

            filterFormModel.CustomProperties.Add("fields", fields);

            return filterFormModel;
        }

    }
}