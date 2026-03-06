using App.Core;
using App.Core.Domain.Traders;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models;
using App.Framework.Models.Extensions;
using App.Models.Traders;
using App.Services.Localization;
using App.Services.Traders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Traders
{
    public partial interface ITraderGroupModelFactory
    {
        Task<TraderGroupSearchModel> PrepareTraderGroupSearchModelAsync(TraderGroupSearchModel searchModel);
        Task<TraderGroupListModel> PrepareTraderGroupListModelAsync(TraderGroupSearchModel searchModel);
        Task<TraderGroupModel> PrepareTraderGroupModelAsync(TraderGroupModel model, TraderGroup traderGroup);
        Task<TraderGroupFormModel> PrepareTraderGroupFormModelAsync(TraderGroupFormModel formModel);
    }
    public partial class TraderGroupModelFactory : ITraderGroupModelFactory
    {
        private readonly ITraderGroupService _traderGroupService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public TraderGroupModelFactory(ITraderGroupService traderGroupService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _traderGroupService = traderGroupService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<TraderGroupSearchModel> PrepareTraderGroupSearchModelAsync(TraderGroupSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.TraderGroupModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<TraderGroupListModel> PrepareTraderGroupListModelAsync(TraderGroupSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var traderGroups = await _traderGroupService.GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new TraderGroupListModel().PrepareToGrid(searchModel, traderGroups);

            return model;
        }

        public virtual Task<TraderGroupModel> PrepareTraderGroupModelAsync(TraderGroupModel model, TraderGroup traderGroup)
        {
            if (traderGroup != null)
            {
                //fill in model values from the entity
                model ??= traderGroup.ToModel<TraderGroupModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderGroupModel>(1, nameof(TraderGroupModel.Description), ColumnType.RouterLink),
                ColumnConfig.Create<TraderGroupModel>(2, nameof(TraderGroupModel.DisplayOrder))
            };

            return columns;
        }

        public virtual async Task<TraderGroupFormModel> PrepareTraderGroupFormModelAsync(TraderGroupFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderGroupModel>(nameof(TraderGroupModel.Description), FieldType.Text),
                FieldConfig.Create<TraderGroupModel>(nameof(TraderGroupModel.DisplayOrder), FieldType.Numeric)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.TraderGroupModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }
    }
}