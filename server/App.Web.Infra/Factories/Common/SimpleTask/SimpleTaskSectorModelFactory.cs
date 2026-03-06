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
    public partial interface ISimpleTaskSectorModelFactory
    {
        Task<SimpleTaskSectorSearchModel> PrepareSimpleTaskSectorSearchModelAsync(SimpleTaskSectorSearchModel searchModel);
        Task<SimpleTaskSectorListModel> PrepareSimpleTaskSectorListModelAsync(SimpleTaskSectorSearchModel searchModel);
        Task<SimpleTaskSectorModel> PrepareSimpleTaskSectorModelAsync(SimpleTaskSectorModel model, SimpleTaskSector simpleTaskSector);
        Task<SimpleTaskSectorFormModel> PrepareSimpleTaskSectorFormModelAsync(SimpleTaskSectorFormModel formModel);
    }
    public partial class SimpleTaskSectorModelFactory : ISimpleTaskSectorModelFactory
    {
        private readonly ISimpleTaskSectorService _simpleTaskSectorService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public SimpleTaskSectorModelFactory(ISimpleTaskSectorService simpleTaskSectorService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _simpleTaskSectorService = simpleTaskSectorService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<SimpleTaskSectorModel>> GetPagedListAsync(SimpleTaskSectorSearchModel searchModel)
        {
            var query = _simpleTaskSectorService.Table.AsEnumerable()
                .Select(x => x.ToModel<SimpleTaskSectorModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<SimpleTaskSectorSearchModel> PrepareSimpleTaskSectorSearchModelAsync(SimpleTaskSectorSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.SimpleTaskSectorModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<SimpleTaskSectorListModel> PrepareSimpleTaskSectorListModelAsync(SimpleTaskSectorSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var simpleTaskSectors = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new SimpleTaskSectorListModel().PrepareToGrid(searchModel, simpleTaskSectors);

            return model;
        }

        public virtual Task<SimpleTaskSectorModel> PrepareSimpleTaskSectorModelAsync(SimpleTaskSectorModel model, SimpleTaskSector simpleTaskSector)
        {
            if (simpleTaskSector != null)
            {
                //fill in model values from the entity
                model ??= simpleTaskSector.ToModel<SimpleTaskSectorModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<SimpleTaskSectorModel>(1, nameof(SimpleTaskSectorModel.Description), ColumnType.RouterLink),
                ColumnConfig.Create<SimpleTaskSectorModel>(2, nameof(SimpleTaskSectorModel.DisplayOrder))
            };

            return columns;
        }

        public virtual async Task<SimpleTaskSectorFormModel> PrepareSimpleTaskSectorFormModelAsync(SimpleTaskSectorFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<SimpleTaskSectorModel>(nameof(SimpleTaskSectorModel.Description), FieldType.Text),
                FieldConfig.Create<SimpleTaskSectorModel>(nameof(SimpleTaskSectorModel.DisplayOrder), FieldType.Text)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.SimpleTaskSectorModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}