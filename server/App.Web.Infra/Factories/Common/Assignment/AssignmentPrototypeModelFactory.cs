using App.Core;
using App.Core.Domain.Assignment;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Assignment;
using App.Services.Assignment;
using App.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Assignment
{
    public partial interface IAssignmentPrototypeModelFactory
    {
        Task<AssignmentPrototypeSearchModel> PrepareAssignmentPrototypeSearchModelAsync(AssignmentPrototypeSearchModel searchModel);
        Task<AssignmentPrototypeListModel> PrepareAssignmentPrototypeListModelAsync(AssignmentPrototypeSearchModel searchModel);
        Task<AssignmentPrototypeModel> PrepareAssignmentPrototypeModelAsync(AssignmentPrototypeModel model, AssignmentPrototype assignmentPrototype);
        Task<AssignmentPrototypeFormModel> PrepareAssignmentPrototypeFormModelAsync(AssignmentPrototypeFormModel formModel);
    }
    public partial class AssignmentPrototypeModelFactory : IAssignmentPrototypeModelFactory
    {
        private readonly IAssignmentPrototypeService _assignmentPrototypeService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public AssignmentPrototypeModelFactory(IAssignmentPrototypeService assignmentPrototypeService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _assignmentPrototypeService = assignmentPrototypeService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<AssignmentPrototypeModel>> GetPagedListAsync(AssignmentPrototypeSearchModel searchModel)
        {
            var query = _assignmentPrototypeService.Table.AsEnumerable()
                .Select(x => x.ToModel<AssignmentPrototypeModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Name.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<AssignmentPrototypeSearchModel> PrepareAssignmentPrototypeSearchModelAsync(AssignmentPrototypeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.AssignmentPrototypeModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<AssignmentPrototypeListModel> PrepareAssignmentPrototypeListModelAsync(AssignmentPrototypeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var assignmentPrototypes = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new AssignmentPrototypeListModel().PrepareToGrid(searchModel, assignmentPrototypes);

            return model;
        }

        public virtual Task<AssignmentPrototypeModel> PrepareAssignmentPrototypeModelAsync(AssignmentPrototypeModel model, AssignmentPrototype assignmentPrototype)
        {
            if (assignmentPrototype != null)
            {
                //fill in model values from the entity
                model ??= assignmentPrototype.ToModel<AssignmentPrototypeModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<AssignmentPrototypeModel>(1, nameof(AssignmentPrototypeModel.Name), ColumnType.RouterLink),
                ColumnConfig.Create<AssignmentPrototypeModel>(2, nameof(AssignmentPrototypeModel.InActive), ColumnType.Checkbox, style: centerAlign),
                ColumnConfig.Create<AssignmentPrototypeModel>(3, nameof(AssignmentPrototypeModel.DisplayOrder))
            };

            return columns;
        }

        public virtual async Task<AssignmentPrototypeFormModel> PrepareAssignmentPrototypeFormModelAsync(AssignmentPrototypeFormModel formModel)
        {
            var top = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentPrototypeModel>(nameof(AssignmentPrototypeModel.Name), FieldType.Textarea, rows: 2, markAsRequired: true),
                FieldConfig.Create<AssignmentPrototypeModel>(nameof(AssignmentPrototypeModel.Description), FieldType.Textarea, rows: 8),
            };

            var bottom = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<AssignmentPrototypeModel>(nameof(AssignmentPrototypeModel.InActive), FieldType.Checkbox),
                FieldConfig.Create<AssignmentPrototypeModel>(nameof(AssignmentPrototypeModel.DisplayOrder), FieldType.Numeric)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12", "col-12 md:col-6" }, top, bottom);

            var panels = new List<Dictionary<string, object>>()
            {
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Common.About"), true, "col-12", fields)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.AssignmentPrototypeModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", panels);

            return formModel;
        }
    }
}