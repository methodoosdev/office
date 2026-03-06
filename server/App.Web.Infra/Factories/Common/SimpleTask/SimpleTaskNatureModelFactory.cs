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
    public partial interface ISimpleTaskNatureModelFactory
    {
        Task<SimpleTaskNatureSearchModel> PrepareSimpleTaskNatureSearchModelAsync(SimpleTaskNatureSearchModel searchModel);
        Task<SimpleTaskNatureListModel> PrepareSimpleTaskNatureListModelAsync(SimpleTaskNatureSearchModel searchModel);
        Task<SimpleTaskNatureModel> PrepareSimpleTaskNatureModelAsync(SimpleTaskNatureModel model, SimpleTaskNature simpleTaskNature);
        Task<SimpleTaskNatureFormModel> PrepareSimpleTaskNatureFormModelAsync(SimpleTaskNatureFormModel formModel);
    }
    public partial class SimpleTaskNatureModelFactory : ISimpleTaskNatureModelFactory
    {
        private readonly ISimpleTaskNatureService _simpleTaskNatureService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public SimpleTaskNatureModelFactory(ISimpleTaskNatureService simpleTaskNatureService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _simpleTaskNatureService = simpleTaskNatureService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<SimpleTaskNatureModel>> GetPagedListAsync(SimpleTaskNatureSearchModel searchModel)
        {
            var query = _simpleTaskNatureService.Table.AsEnumerable()
                .Select(x => x.ToModel<SimpleTaskNatureModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<SimpleTaskNatureSearchModel> PrepareSimpleTaskNatureSearchModelAsync(SimpleTaskNatureSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var customer = await _workContext.GetCurrentCustomerAsync();

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.SimpleTaskNatureModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<SimpleTaskNatureListModel> PrepareSimpleTaskNatureListModelAsync(SimpleTaskNatureSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var simpleTaskNatures = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new SimpleTaskNatureListModel().PrepareToGrid(searchModel, simpleTaskNatures);

            return model;
        }

        public virtual Task<SimpleTaskNatureModel> PrepareSimpleTaskNatureModelAsync(SimpleTaskNatureModel model, SimpleTaskNature simpleTaskNature)
        {
            if (simpleTaskNature != null)
            {
                //fill in model values from the entity
                model ??= simpleTaskNature.ToModel<SimpleTaskNatureModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<SimpleTaskNatureModel>(1, nameof(SimpleTaskNatureModel.Description), ColumnType.RouterLink),
                ColumnConfig.Create<SimpleTaskNatureModel>(2, nameof(SimpleTaskNatureModel.DisplayOrder))
            };

            return columns;
        }

        public virtual async Task<SimpleTaskNatureFormModel> PrepareSimpleTaskNatureFormModelAsync(SimpleTaskNatureFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<SimpleTaskNatureModel>(nameof(SimpleTaskNatureModel.Description), FieldType.Text),
                FieldConfig.Create<SimpleTaskNatureModel>(nameof(SimpleTaskNatureModel.DisplayOrder), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.SimpleTaskNatureModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}