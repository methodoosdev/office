using App.Core;
using App.Core.Domain.Offices;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Offices;
using App.Services;
using App.Services.Localization;
using App.Services.Offices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Offices
{
    public partial interface IPeriodicityItemModelFactory
    {
        Task<PeriodicityItemSearchModel> PreparePeriodicityItemSearchModelAsync(PeriodicityItemSearchModel searchModel);
        Task<PeriodicityItemListModel> PreparePeriodicityItemListModelAsync(PeriodicityItemSearchModel searchModel);
        Task<PeriodicityItemModel> PreparePeriodicityItemModelAsync(PeriodicityItemModel model, PeriodicityItem periodicityItem);
        Task<PeriodicityItemFormModel> PreparePeriodicityItemFormModelAsync(PeriodicityItemFormModel formModel);
    }
    public partial class PeriodicityItemModelFactory : IPeriodicityItemModelFactory
    {
        private readonly IPeriodicityItemService _periodicityItemService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public PeriodicityItemModelFactory(IPeriodicityItemService periodicityItemService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _periodicityItemService = periodicityItemService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<PeriodicityItemModel>> GetPagedListAsync(PeriodicityItemSearchModel searchModel)
        {
            var query = _periodicityItemService.Table.SelectAwait(async value =>
            {
                var item = value.ToModel<PeriodicityItemModel>();
                item.PeriodicityItemTypeName = await _localizationService.GetLocalizedEnumAsync(value.PeriodicityItemType);

                return item;
            });

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Paragraph.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<PeriodicityItemSearchModel> PreparePeriodicityItemSearchModelAsync(PeriodicityItemSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.PeriodicityItemModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<PeriodicityItemListModel> PreparePeriodicityItemListModelAsync(PeriodicityItemSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var periodicityItems = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new PeriodicityItemListModel().PrepareToGrid(searchModel, periodicityItems);

            return model;
        }

        public virtual Task<PeriodicityItemModel> PreparePeriodicityItemModelAsync(PeriodicityItemModel model, PeriodicityItem periodicityItem)
        {
            if (periodicityItem != null)
            {
                //fill in model values from the entity
                model ??= periodicityItem.ToModel<PeriodicityItemModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<PeriodicityItemModel>(1, nameof(PeriodicityItemModel.Paragraph)),
                ColumnConfig.Create<PeriodicityItemModel>(2, nameof(PeriodicityItemModel.PeriodicityItemTypeName), ColumnType.RouterLink)
            };

            return columns;
        }

        public virtual async Task<PeriodicityItemFormModel> PreparePeriodicityItemFormModelAsync(PeriodicityItemFormModel formModel)
        {
            var periodicityItemTypes = await PeriodicityItemType.RealEstateRent.ToSelectionItemListAsync();

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<PeriodicityItemModel>(nameof(PeriodicityItemModel.Paragraph), FieldType.Text),
                FieldConfig.Create<PeriodicityItemModel>(nameof(PeriodicityItemModel.Notes), FieldType.Textarea),
                FieldConfig.Create<PeriodicityItemModel>(nameof(PeriodicityItemModel.PeriodicityItemTypeId), FieldType.Select, options: periodicityItemTypes)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.PeriodicityItemModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}