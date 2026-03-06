using App.Core;
using App.Core.Domain.Assignment;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Assignment;
using App.Services;
using App.Services.Assignment;
using App.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Assignment
{
    public partial interface IAssignmentPrototypeActionModelFactory
    {
        Task<AssignmentPrototypeActionSearchModel> PrepareAssignmentPrototypeActionSearchModelAsync(AssignmentPrototypeActionSearchModel searchModel);
        Task<AssignmentPrototypeActionListModel> PrepareAssignmentPrototypeActionListModelAsync(AssignmentPrototypeActionSearchModel searchModel);
        Task<AssignmentPrototypeActionModel> PrepareAssignmentPrototypeActionModelAsync(AssignmentPrototypeActionModel model, AssignmentPrototypeAction assignmentPrototypeAction);
        Task<AssignmentPrototypeActionFormModel> PrepareAssignmentPrototypeActionFormModelAsync(AssignmentPrototypeActionFormModel formModel);
    }
    public partial class AssignmentPrototypeActionModelFactory : IAssignmentPrototypeActionModelFactory
    {
        private readonly IAssignmentPrototypeActionService _assignmentPrototypeActionService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public AssignmentPrototypeActionModelFactory(
            IAssignmentPrototypeActionService assignmentPrototypeActionService,
            IModelFactoryService modelFactoryService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _assignmentPrototypeActionService = assignmentPrototypeActionService;
            _modelFactoryService = modelFactoryService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<AssignmentPrototypeActionModel>> GetPagedListAsync(AssignmentPrototypeActionSearchModel searchModel)
        {
            var departments = await _modelFactoryService.GetAllDepartmentsAsync(false);
            var assignmentReasons = await _modelFactoryService.GetAllAssignmentReasonsAsync(false);

            var query = _assignmentPrototypeActionService.Table.AsEnumerable()
                .Select(x =>
                {
                    var model = x.ToModel<AssignmentPrototypeActionModel>();
                    model.DepartmentName = departments.FirstOrDefault(d => d.Value == x.DepartmentId)?.Label ?? "";
                    model.AssignmentReasonName = assignmentReasons.FirstOrDefault(d => d.Value == x.AssignmentReasonId)?.Label ?? ""; ;

                    return model;
                })
                .AsQueryable();

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

        public virtual async Task<AssignmentPrototypeActionSearchModel> PrepareAssignmentPrototypeActionSearchModelAsync(AssignmentPrototypeActionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.AssignmentPrototypeActionModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<AssignmentPrototypeActionListModel> PrepareAssignmentPrototypeActionListModelAsync(AssignmentPrototypeActionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var assignmentPrototypeActions = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new AssignmentPrototypeActionListModel().PrepareToGrid(searchModel, assignmentPrototypeActions);

            return model;
        }

        public virtual Task<AssignmentPrototypeActionModel> PrepareAssignmentPrototypeActionModelAsync(AssignmentPrototypeActionModel model, AssignmentPrototypeAction assignmentPrototypeAction)
        {
            if (assignmentPrototypeAction != null)
            {
                //fill in model values from the entity
                model ??= assignmentPrototypeAction.ToModel<AssignmentPrototypeActionModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<AssignmentPrototypeActionModel>(1, nameof(AssignmentPrototypeActionModel.Name), ColumnType.RouterLink),
                ColumnConfig.Create<AssignmentPrototypeActionModel>(2, nameof(AssignmentPrototypeActionModel.DepartmentName)),
                ColumnConfig.Create<AssignmentPrototypeActionModel>(3, nameof(AssignmentPrototypeActionModel.AssignmentReasonName)),
                ColumnConfig.Create<AssignmentPrototypeActionModel>(4, nameof(AssignmentPrototypeActionModel.DisplayOrder))
            };

            return columns;
        }

        public virtual async Task<AssignmentPrototypeActionFormModel> PrepareAssignmentPrototypeActionFormModelAsync(AssignmentPrototypeActionFormModel formModel)
        {
            var departments = await _modelFactoryService.GetAllDepartmentsAsync();
            var assignmentReasons = await _modelFactoryService.GetAllAssignmentReasonsAsync();

            var top = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentPrototypeActionModel>(nameof(AssignmentPrototypeActionModel.Name), FieldType.Textarea, rows: 2, markAsRequired: true),
                FieldConfig.Create<AssignmentPrototypeActionModel>(nameof(AssignmentPrototypeActionModel.Description), FieldType.Textarea, rows: 19),
            };

            var bottom = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentPrototypeActionModel>(nameof(AssignmentPrototypeActionModel.DepartmentId), FieldType.Select, options: departments, markAsRequired: true),
                FieldConfig.Create<AssignmentPrototypeActionModel>(nameof(AssignmentPrototypeActionModel.AssignmentReasonId), FieldType.Select, options: assignmentReasons),
                FieldConfig.Create<AssignmentPrototypeActionModel>(nameof(AssignmentPrototypeActionModel.DisplayOrder), FieldType.Numeric)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12", "col-12 md:col-6" }, top, bottom);

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.AssignmentPrototypeActionModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }
    }
}