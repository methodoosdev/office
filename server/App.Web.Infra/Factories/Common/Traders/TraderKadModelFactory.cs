using App.Core;
using App.Framework.Components;
using App.Framework.Models.Extensions;
using App.Models.Traders;
using App.Services.Localization;
using App.Services.Traders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Traders
{
    public partial interface ITraderKadModelFactory
    {
        Task<TraderKadSearchModel> PrepareTraderKadSearchModelAsync(TraderKadSearchModel searchModel);
        Task<TraderKadListModel> PrepareTraderKadListModelAsync(TraderKadSearchModel searchModel, int parentId);
    }
    public partial class TraderKadModelFactory : ITraderKadModelFactory
    {
        private readonly ITraderKadService _traderKadService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public TraderKadModelFactory(ITraderKadService traderKadService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _traderKadService = traderKadService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<TraderKadSearchModel> PrepareTraderKadSearchModelAsync(TraderKadSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.TraderKadModel.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<TraderKadListModel> PrepareTraderKadListModelAsync(TraderKadSearchModel searchModel, int parentId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var traderKads = await _traderKadService.GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new TraderKadListModel().PrepareToGrid(searchModel, traderKads);

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderKadModel>(1, nameof(TraderKadModel.GroupId)),
                ColumnConfig.Create<TraderKadModel>(2, nameof(TraderKadModel.Code)),
                ColumnConfig.Create<TraderKadModel>(3, nameof(TraderKadModel.Description)),
                ColumnConfig.Create<TraderKadModel>(4, nameof(TraderKadModel.Activity), ColumnType.Checkbox)
            };

            return columns;
        }
    }
}