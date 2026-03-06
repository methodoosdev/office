using App.Core;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Traders;
using App.Services;
using App.Services.Localization;
using App.Services.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Traders
{
    public partial interface ITraderRatingModelFactory
    {
        Task<TraderRatingSearchModel> PrepareTraderRatingSearchModelAsync(TraderRatingSearchModel searchModel, bool hideCategory = false);
        Task<TraderRatingListModel> PrepareTraderRatingListModelAsync(TraderRatingSearchModel searchModel);
        Task<TraderRatingModel> PrepareTraderRatingModelAsync(TraderRatingModel model, TraderRating traderRating);
        Task<TraderRatingFormModel> PrepareTraderRatingFormModelAsync(TraderRatingFormModel formModel);
    }
    public partial class TraderRatingModelFactory : ITraderRatingModelFactory
    {
        private readonly ITraderRatingService _traderRatingService;
        private readonly ILocalizationService _localizationService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly IWorkContext _workContext;

        public TraderRatingModelFactory(ITraderRatingService traderRatingService,
            ILocalizationService localizationService,
            IModelFactoryService modelFactoryService,
            IWorkContext workContext)
        {
            _traderRatingService = traderRatingService;
            _localizationService = localizationService;
            _modelFactoryService = modelFactoryService;
            _workContext = workContext;
        }

        private async Task<IPagedList<TraderRatingModel>> GetPagedListAsync(TraderRatingSearchModel searchModel)
        {
            var categories = await _modelFactoryService.GetAllTraderRatingCategoriesAsync(false);
            var departments = await _modelFactoryService.GetAllDepartmentsAsync(false);

            var query = _traderRatingService.Table.AsEnumerable()
                .Select(x => 
                {
                    var model = x.ToModel<TraderRatingModel>();
                    model.CategoryName = categories.FirstOrDefault(c => c.Value == x.TraderRatingCategoryId).Label;
                    model.DepartmentName = departments.FirstOrDefault(c => c.Value == x.DepartmentId).Label;

                    return model;
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.Description.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.CategoryName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.DepartmentName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<TraderRatingSearchModel> PrepareTraderRatingSearchModelAsync(TraderRatingSearchModel searchModel, bool hideCategory = false)
        {
            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig(hideCategory);
            searchModel.SetGridPageSize(1000);
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.TraderRatingModel.ListForm.Title");
            searchModel.DataKey = "id";
            searchModel.Group.Add(new GridGroupDescriptor { Field = "departmentName", Dir = "asc" });
            if (!hideCategory)
                searchModel.Group.Add(new GridGroupDescriptor { Field = "categoryName", Dir = "asc" });

            return searchModel;
        }

        public virtual async Task<TraderRatingListModel> PrepareTraderRatingListModelAsync(TraderRatingSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var traderRatings = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new TraderRatingListModel().PrepareToGrid(searchModel, traderRatings);

            return model;
        }

        public virtual Task<TraderRatingModel> PrepareTraderRatingModelAsync(TraderRatingModel model, TraderRating traderRating)
        {
            if (traderRating != null)
            {
                //fill in model values from the entity
                model ??= traderRating.ToModel<TraderRatingModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig(bool hideCategory)
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderRatingModel>(2, nameof(TraderRatingModel.DepartmentName), hidden: true),
                ColumnConfig.Create<TraderRatingModel>(1, nameof(TraderRatingModel.CategoryName), hidden: !hideCategory),
                ColumnConfig.Create<TraderRatingModel>(3, nameof(TraderRatingModel.Description), ColumnType.RouterLink),
                ColumnConfig.Create<TraderRatingModel>(4, nameof(TraderRatingModel.Gravity), ColumnType.Numeric, style: rightAlign),
                ColumnConfig.Create<TraderRatingModel>(5, nameof(TraderRatingModel.DisplayOrder), ColumnType.Numeric, style: rightAlign)
            };

            return columns;
        }

        public virtual async Task<TraderRatingFormModel> PrepareTraderRatingFormModelAsync(TraderRatingFormModel formModel)
        {
            var categories = await _modelFactoryService.GetAllTraderRatingCategoriesAsync(false);
            var departments = await _modelFactoryService.GetAllDepartmentsAsync(false);

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderRatingModel>(nameof(TraderRatingModel.DepartmentId), FieldType.GridSelect, options: departments),
                FieldConfig.Create<TraderRatingModel>(nameof(TraderRatingModel.TraderRatingCategoryId), FieldType.GridSelect, options: categories),
                FieldConfig.Create<TraderRatingModel>(nameof(TraderRatingModel.Description), FieldType.Text),
                FieldConfig.Create<TraderRatingModel>(nameof(TraderRatingModel.Gravity), FieldType.Numeric),
                FieldConfig.Create<TraderRatingModel>(nameof(TraderRatingModel.DisplayOrder), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.TraderRatingModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}