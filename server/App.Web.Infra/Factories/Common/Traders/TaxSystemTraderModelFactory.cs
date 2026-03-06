using App.Core;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Framework.Models;
using App.Framework.Models.Extensions;
using App.Models.Traders;
using App.Services;
using App.Services.Localization;
using App.Web.Infra.Queries.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Traders
{
    public partial interface ITaxSystemTraderModelFactory
    {
        Task<TaxSystemTraderSearchModel> PrepareTaxSystemTraderSearchModelAsync(TaxSystemTraderSearchModel searchModel);
        Task<TaxSystemTraderListModel> PrepareTaxSystemTraderListModelAsync(TaxSystemTraderSearchModel searchModel, string connection, int year);
    }
    public partial class TaxSystemTraderModelFactory : ITaxSystemTraderModelFactory
    {
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public TaxSystemTraderModelFactory(
            IAppDataProvider dataProvider,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _dataProvider = dataProvider;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<IPagedList<TaxSystemTraderModel>> GetPagedListAsync(TaxSystemTraderSearchModel searchModel, string connection, int year)
        {
            var pYear = new LinqToDB.Data.DataParameter("pYear", year);

            var taxSystemTradersList = await _dataProvider.QueryAsync<TaxSystemTraderModel>(connection, TaxSystemTradersQuery.Party, pYear);

            var categoryBookTypes = await CategoryBookType.None.ToSelectionItemListAsync();
            var customerTypes = await CustomerType.Other.ToSelectionItemListAsync();

            var query = (from srf in taxSystemTradersList.AsEnumerable()
                         from cbt in categoryBookTypes.Where(x => x.Value == srf.CategoryBookTypeId).DefaultIfEmpty()
                         from ct in customerTypes.Where(x => x.Value == srf.CustomerTypeId).DefaultIfEmpty()
                         select new { srf, cbt, ct }
                         ).Select(x =>
                         {
                             var model = x.srf;
                             model.CategoryBookTypeName = x.cbt?.Label ?? "";
                             model.CustomerTypeName = x.ct?.Label ?? "";
                             model.LastName = model.LastName ?? "";
                             model.FirstName = model.FirstName ?? "";
                             model.Vat = model.Vat ?? "";
                             model.Email = model.Email ?? "";

                             return model;
                         }).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.LastName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.FirstName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Vat.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Email.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.CategoryBookTypeName.ContainsIgnoreCase(searchModel.QuickSearch)
                );
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<TaxSystemTraderSearchModel> PrepareTaxSystemTraderSearchModelAsync(TaxSystemTraderSearchModel searchModel)
        {
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.TaxSystemTraderModel.Title");
            searchModel.DataKey = "taxSystemId";
            searchModel.Height = 520;

            return searchModel;
        }

        public virtual async Task<TaxSystemTraderListModel> PrepareTaxSystemTraderListModelAsync(TaxSystemTraderSearchModel searchModel, string connection, int year)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var taxSystemTraders = await GetPagedListAsync(searchModel, connection, year);

            //prepare grid model
            var model = new TaxSystemTraderListModel().PrepareToGrid(searchModel, taxSystemTraders);

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TaxSystemTraderModel>(1, nameof(TaxSystemTraderModel.LastName)),
                ColumnConfig.Create<TaxSystemTraderModel>(2, nameof(TaxSystemTraderModel.FirstName)),
                ColumnConfig.Create<TaxSystemTraderModel>(3, nameof(TaxSystemTraderModel.Vat)),
                ColumnConfig.Create<TaxSystemTraderModel>(4, nameof(TaxSystemTraderModel.Email)),
                ColumnConfig.Create<TaxSystemTraderModel>(4, nameof(TaxSystemTraderModel.CategoryBookTypeName)),
                ColumnConfig.Create<TaxSystemTraderModel>(4, nameof(TaxSystemTraderModel.CustomerTypeName)),
                ColumnConfig.Create<TaxSystemTraderModel>(7, nameof(TaxSystemTraderModel.EmployerIkaUserName), hidden: true),
                ColumnConfig.Create<TaxSystemTraderModel>(8, nameof(TaxSystemTraderModel.EmployerIkaPassword), hidden: true),
                ColumnConfig.Create<TaxSystemTraderModel>(9, nameof(TaxSystemTraderModel.TaxisUserName), hidden: true),
                ColumnConfig.Create<TaxSystemTraderModel>(10, nameof(TaxSystemTraderModel.TaxisPassword), hidden: true),
                ColumnConfig.Create<TaxSystemTraderModel>(11, nameof(TaxSystemTraderModel.OaeeUserName), hidden: true),
                ColumnConfig.Create<TaxSystemTraderModel>(12, nameof(TaxSystemTraderModel.OaeePassword), hidden: true),
                ColumnConfig.Create<TaxSystemTraderModel>(13, nameof(TaxSystemTraderModel.SepePassword), hidden: true),
                ColumnConfig.Create<TaxSystemTraderModel>(14, nameof(TaxSystemTraderModel.SepeUserName), hidden: true)
            };

            return columns;
        }
    }
}