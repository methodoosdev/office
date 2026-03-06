using App.Core;
using App.Core.Domain.SimpleTask;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.SimpleTask;
using App.Services.Localization;
using App.Services.SimpleTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.SimpleTask
{
    public partial interface ISimpleTaskCategoryModelFactory
    {
        Task<SimpleTaskCategorySearchModel> PrepareSimpleTaskCategorySearchModelAsync(SimpleTaskCategorySearchModel searchModel);
        Task<SimpleTaskCategoryListModel> PrepareSimpleTaskCategoryListModelAsync(SimpleTaskCategorySearchModel searchModel);
        Task<SimpleTaskCategoryModel> PrepareSimpleTaskCategoryModelAsync(SimpleTaskCategoryModel model, SimpleTaskCategory simpleTaskCategory);
        Task<SimpleTaskCategoryFormModel> PrepareSimpleTaskCategoryFormModelAsync(SimpleTaskCategoryFormModel formModel);
    }
    public partial class SimpleTaskCategoryModelFactory : ISimpleTaskCategoryModelFactory
    {
        private readonly ISimpleTaskCategoryService _simpleTaskCategoryService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public SimpleTaskCategoryModelFactory(ISimpleTaskCategoryService simpleTaskCategoryService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _simpleTaskCategoryService = simpleTaskCategoryService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<SimpleTaskCategoryModel>> GetPagedListAsync(SimpleTaskCategorySearchModel searchModel)
        {
            var query = _simpleTaskCategoryService.Table.AsEnumerable()
                .Select(x => x.ToModel<SimpleTaskCategoryModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<SimpleTaskCategorySearchModel> PrepareSimpleTaskCategorySearchModelAsync(SimpleTaskCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.SimpleTaskCategoryModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<SimpleTaskCategoryListModel> PrepareSimpleTaskCategoryListModelAsync(SimpleTaskCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var simpleTaskCategorys = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new SimpleTaskCategoryListModel().PrepareToGrid(searchModel, simpleTaskCategorys);

            return model;
        }

        public virtual Task<SimpleTaskCategoryModel> PrepareSimpleTaskCategoryModelAsync(SimpleTaskCategoryModel model, SimpleTaskCategory simpleTaskCategory)
        {
            if (simpleTaskCategory != null)
            {
                //fill in model values from the entity
                model ??= simpleTaskCategory.ToModel<SimpleTaskCategoryModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<SimpleTaskCategoryModel>(1, nameof(SimpleTaskCategoryModel.Description), ColumnType.RouterLink),
                ColumnConfig.Create<SimpleTaskCategoryModel>(2, nameof(SimpleTaskCategoryModel.DisplayOrder))
            };

            return columns;
        }

        public virtual async Task<SimpleTaskCategoryFormModel> PrepareSimpleTaskCategoryFormModelAsync(SimpleTaskCategoryFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<SimpleTaskCategoryModel>(nameof(SimpleTaskCategoryModel.Description), FieldType.Text),
                FieldConfig.Create<SimpleTaskCategoryModel>(nameof(SimpleTaskCategoryModel.DisplayOrder), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.SimpleTaskCategoryModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}