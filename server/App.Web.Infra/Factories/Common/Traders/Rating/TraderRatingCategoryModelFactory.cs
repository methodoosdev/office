using App.Core;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Traders;
using App.Services.Localization;
using App.Services.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Traders
{
    public partial interface ITraderRatingCategoryModelFactory
    {
        Task<TraderRatingCategorySearchModel> PrepareTraderRatingCategorySearchModelAsync(TraderRatingCategorySearchModel searchModel);
        Task<TraderRatingCategoryListModel> PrepareTraderRatingCategoryListModelAsync(TraderRatingCategorySearchModel searchModel);
        Task<TraderRatingCategoryModel> PrepareTraderRatingCategoryModelAsync(TraderRatingCategoryModel model, TraderRatingCategory traderRatingCategory);
        Task<TraderRatingCategoryFormModel> PrepareTraderRatingCategoryFormModelAsync(TraderRatingCategoryFormModel formModel);
    }
    public partial class TraderRatingCategoryModelFactory : ITraderRatingCategoryModelFactory
    {
        private readonly ITraderRatingCategoryService _traderRatingCategoryService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public TraderRatingCategoryModelFactory(ITraderRatingCategoryService traderRatingCategoryService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _traderRatingCategoryService = traderRatingCategoryService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<TraderRatingCategoryModel>> GetPagedListAsync(TraderRatingCategorySearchModel searchModel)
        {
            var query = _traderRatingCategoryService.Table.AsEnumerable()
                .Select(x => x.ToModel<TraderRatingCategoryModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Description.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<TraderRatingCategorySearchModel> PrepareTraderRatingCategorySearchModelAsync(TraderRatingCategorySearchModel searchModel)
        {
            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.TraderRatingCategoryModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<TraderRatingCategoryListModel> PrepareTraderRatingCategoryListModelAsync(TraderRatingCategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var traderRatingCategories = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new TraderRatingCategoryListModel().PrepareToGrid(searchModel, traderRatingCategories);

            return model;
        }

        public virtual Task<TraderRatingCategoryModel> PrepareTraderRatingCategoryModelAsync(TraderRatingCategoryModel model, TraderRatingCategory traderRatingCategory)
        {
            if (traderRatingCategory != null)
            {
                //fill in model values from the entity
                model ??= traderRatingCategory.ToModel<TraderRatingCategoryModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderRatingCategoryModel>(1, nameof(TraderRatingCategoryModel.Description), ColumnType.RouterLink),
                ColumnConfig.Create<TraderRatingCategoryModel>(2, nameof(TraderRatingCategoryModel.DisplayOrder))
            };

            return columns;
        }

        public virtual async Task<TraderRatingCategoryFormModel> PrepareTraderRatingCategoryFormModelAsync(TraderRatingCategoryFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderRatingCategoryModel>(nameof(TraderRatingCategoryModel.Description), FieldType.Text),
                FieldConfig.Create<TraderRatingCategoryModel>(nameof(TraderRatingCategoryModel.DisplayOrder), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.TraderRatingCategoryModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}