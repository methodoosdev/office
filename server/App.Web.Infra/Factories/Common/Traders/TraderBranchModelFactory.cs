using App.Core;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models;
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
    public partial interface ITraderBranchModelFactory
    {
        Task<TraderBranchSearchModel> PrepareTraderBranchSearchModelAsync(TraderBranchSearchModel searchModel);
        Task<TraderBranchListModel> PrepareTraderBranchListModelAsync(TraderBranchSearchModel searchModel, int parentId);
    }
    public partial class TraderBranchModelFactory : ITraderBranchModelFactory
    {
        private readonly ITraderBranchService _traderBranchService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public TraderBranchModelFactory(ITraderBranchService traderBranchService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _traderBranchService = traderBranchService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<TraderBranchModel>> GetPagedListAsync(TraderBranchSearchModel searchModel, int parentId)
        {
            var query = _traderBranchService.Table.AsEnumerable()
                .Select(x => x.ToModel<TraderBranchModel>())
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.Title.ContainsIgnoreCase(searchModel.QuickSearch) ||
                     c.Address.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.Where(x => x.TraderId == parentId);

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<TraderBranchSearchModel> PrepareTraderBranchSearchModelAsync(TraderBranchSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.TraderBranchModel.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<TraderBranchListModel> PrepareTraderBranchListModelAsync(TraderBranchSearchModel searchModel, int parentId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var traderBranchs = await GetPagedListAsync(searchModel, parentId);

            //prepare grid model
            var model = new TraderBranchListModel().PrepareToGrid(searchModel, traderBranchs);

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderBranchModel>(1, nameof(TraderBranchModel.GroupId)),
                ColumnConfig.Create<TraderBranchModel>(2, nameof(TraderBranchModel.Kind)),
                ColumnConfig.Create<TraderBranchModel>(3, nameof(TraderBranchModel.Title)),
                ColumnConfig.Create<TraderBranchModel>(4, nameof(TraderBranchModel.Address)),
                ColumnConfig.Create<TraderBranchModel>(5, nameof(TraderBranchModel.Doy))
            };

            return columns;
        }
    }
}