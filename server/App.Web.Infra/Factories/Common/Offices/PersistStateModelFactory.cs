using App.Core;
using App.Core.Domain.Customers;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Common;
using App.Services.Localization;
using App.Services.Offices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Offices
{
    public partial interface IPersistStateModelFactory
    {
        Task<PersistStateSearchModel> PreparePersistStateSearchModelAsync(PersistStateSearchModel searchModel);
        Task<PersistStateListModel> PreparePersistStateListModelAsync(PersistStateSearchModel searchModel);
    }
    public partial class PersistStateModelFactory : IPersistStateModelFactory
    {
        private readonly IPersistStateService _persistStateService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public PersistStateModelFactory(IPersistStateService persistStateService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _persistStateService = persistStateService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<PersistStateModel>> GetPagedListAsync(PersistStateSearchModel searchModel)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            var query = _persistStateService.Table.AsEnumerable()
                .Select(x =>
                {
                    var model = x.ToModel<PersistStateModel>();
                    model.CustomerEmail = customer.Email;

                    return model;
                }).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.ModelType.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<PersistStateSearchModel> PreparePersistStateSearchModelAsync(PersistStateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.PersistStateModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<PersistStateListModel> PreparePersistStateListModelAsync(PersistStateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var persistStates = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new PersistStateListModel().PrepareToGrid(searchModel, persistStates);

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<PersistStateModel>(1, nameof(PersistStateModel.ModelType), ColumnType.RouterLink)
            };

            return columns;
        }
    }
}