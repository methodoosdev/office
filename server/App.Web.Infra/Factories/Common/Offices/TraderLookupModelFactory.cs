using App.Core;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Core.Infrastructure.Mapper;
using App.Framework.Components;
using App.Framework.Models.Extensions;
using App.Models.Traders;
using App.Services;
using App.Services.Localization;
using App.Services.Traders;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Offices
{
    public partial interface ITraderLookupModelFactory
    {
        Task<IQueryable<TraderLookupItem>> GetAllTraderLookupItemsQueryAsync();
        Task<IList<TraderLookupItem>> GetAllTraderLookupItemsAsync(IList<int> HyperPayrollIds = null);
        Task<TraderLookupSearchModel> PrepareTraderLookupSearchModelAsync(TraderLookupSearchModel searchModel);
        Task<TraderLookupListModel> PrepareTraderLookupListModelAsync(TraderLookupSearchModel searchModel);
    }
    public partial class TraderLookupModelFactory : ITraderLookupModelFactory
    {
        private readonly ITraderService _traderService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public TraderLookupModelFactory(
            ITraderService traderService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public async Task<IQueryable<TraderLookupItem>> GetAllTraderLookupItemsQueryAsync()
        {
            var customerTypes = await CustomerType.Other.ToSelectionItemListAsync();
            var legalFormTypes = await LegalFormType.None.ToSelectionItemListAsync();
            var categoryBookTypes = await CategoryBookType.None.ToSelectionItemListAsync();

            var tradelModelList = _traderService.Table.Select(x => x.ToTraderModel()).ToList();
            var query = (from e in tradelModelList.AsEnumerable()
                         from ct in customerTypes.Where(x => x.Value == e.CustomerTypeId).DefaultIfEmpty()
                         from lft in legalFormTypes.Where(x => x.Value == e.LegalFormTypeId).DefaultIfEmpty()
                         from cbt in categoryBookTypes.Where(x => x.Value == e.CategoryBookTypeId).DefaultIfEmpty()
                         select new TraderLookupItem
                         {
                             Id = e.Id,
                             FullName = e.FullName() ?? "",
                             Vat = e.Vat ?? "",
                             Email = string.Join(", ", (new List<string> { e.Email, e.Email2, e.Email3 }).Where(x => !string.IsNullOrEmpty(x)).ToList()),
                             CustomerTypeId = e.CustomerTypeId,
                             CustomerTypeName = ct?.Label ?? "",
                             LegalFormTypeId = e.LegalFormTypeId,
                             LegalFormTypeName = lft?.Label ?? "",
                             CategoryBookTypeId = e.CategoryBookTypeId,
                             CategoryBookTypeName = cbt?.Label ?? "",
                             ConnectionAccountingActive = e.ConnectionAccountingActive,
                             HyperPayrollId = e.HyperPayrollId,
                             Active = !e.Deleted && e.Active,
                             HasFinancialObligation = e.HasFinancialObligation
                         }).AsQueryable();

            return query;
        }

        public async Task<IList<TraderLookupItem>> GetAllTraderLookupItemsAsync(IList<int> HyperPayrollIds = null)
        {
            var query = await GetAllTraderLookupItemsQueryAsync();

            if (HyperPayrollIds != null)
                query = query.Where(x => HyperPayrollIds.Contains(x.HyperPayrollId));

            query = query.Where(x => x.Active);

            query = query.OrderBy(x => x.FullName);

            return query.ToList();
        }

        private async Task<IPagedList<TraderLookupModel>> GetPagedListAsync(TraderLookupSearchModel searchModel)
        {
            var query = (await GetAllTraderLookupItemsQueryAsync())
                .Select(x => AutoMapperConfiguration.Mapper.Map<TraderLookupModel>(x));

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.FullName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Vat.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Email.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.CustomerTypeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.LegalFormTypeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.CategoryBookTypeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.HyperPayrollId.ToString().Contains(searchModel.QuickSearch));
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<TraderLookupSearchModel> PrepareTraderLookupSearchModelAsync(TraderLookupSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.TraderLookupSearchModel.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<TraderLookupListModel> PrepareTraderLookupListModelAsync(TraderLookupSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var traderLookups = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new TraderLookupListModel().PrepareToGrid(searchModel, traderLookups);

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderLookupModel>(1, nameof(TraderLookupModel.FullName), width: 350),
                ColumnConfig.Create<TraderLookupModel>(2, nameof(TraderLookupModel.Vat)),
                ColumnConfig.Create<TraderLookupModel>(3, nameof(TraderLookupModel.Email)),
                ColumnConfig.Create<TraderLookupModel>(4, nameof(TraderLookupModel.HyperPayrollId), style: rightAlign),
                ColumnConfig.Create<TraderLookupModel>(4, nameof(TraderLookupModel.CustomerTypeName)),
                ColumnConfig.Create<TraderLookupModel>(5, nameof(TraderLookupModel.LegalFormTypeName)),
                ColumnConfig.Create<TraderLookupModel>(6, nameof(TraderLookupModel.CategoryBookTypeName)),
                ColumnConfig.Create<TraderLookupModel>(7, nameof(TraderLookupModel.ConnectionAccountingActive), ColumnType.Checkbox),
                ColumnConfig.Create<TraderLookupModel>(8, nameof(TraderLookupModel.Active), ColumnType.Checkbox, hidden: true)
            };

            return columns;
        }
    }
}