using App.Core;
using App.Core.Domain.Offices;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Accounting;
using App.Services.Configuration;
using App.Services.Localization;
using App.Services.Offices;
using App.Services.Traders;
using App.Web.Infra.Queries.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Common.Factories
{
    public partial interface IPeriodicityItemsModelFactory
    {
        Task<PeriodicityItemsSearchModel> PreparePeriodicityItemsSearchModelAsync(PeriodicityItemsSearchModel searchModel);
        Task<IList<PeriodicityItemsModel>> PreparePeriodicityItemsListAsync(TraderConnectionResult connectionResult, int year, int month);
        Task<PeriodicityItemsTableModel> PreparePeriodicityItemsTableModelAsync(PeriodicityItemsTableModel tableModel);
    }
    public class PeriodicityItemsModelFactory : IPeriodicityItemsModelFactory
    {
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IPeriodicityItemService _periodicityItemService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public PeriodicityItemsModelFactory(
            IFieldConfigService fieldConfigService,
            IPeriodicityItemService periodicityItemService,
            IAppDataProvider dataProvider,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _fieldConfigService = fieldConfigService;
            _periodicityItemService = periodicityItemService;
            _dataProvider = dataProvider;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        private async Task<PeriodicityItemsModel> PrepareRealEstateRentAsync(PeriodicityItem periodicityItem, TraderConnectionResult connectionResult, int year, bool credit)
        {
            var paragraph = periodicityItem.Paragraph;
            var periodicityItemTypeName = await _localizationService.GetLocalizedEnumAsync(periodicityItem.PeriodicityItemType);

            var paramCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var paramYear = new LinqToDB.Data.DataParameter("pYear", year);

            var list = await _dataProvider.QueryAsync<RealEstateRentModel>(connectionResult.Connection, PeriodicityItemsQuery.RealEstateRent, paramCompanyId, paramYear);

            var realEstateRentResult = await list.Where(x => x.Credit == credit).ToListAsync();

            var result = new PeriodicityItemsModel
            {
                Paragraph = paragraph,
                PeriodicityItemTypeName = periodicityItemTypeName,
                January = realEstateRentResult.Where(x => x.Periodos == 1).Select(s => s.Price).Sum(),
                February = realEstateRentResult.Where(x => x.Periodos == 2).Select(s => s.Price).Sum(),
                March = realEstateRentResult.Where(x => x.Periodos == 3).Select(s => s.Price).Sum(),
                April = realEstateRentResult.Where(x => x.Periodos == 4).Select(s => s.Price).Sum(),
                May = realEstateRentResult.Where(x => x.Periodos == 5).Select(s => s.Price).Sum(),
                June = realEstateRentResult.Where(x => x.Periodos == 6).Select(s => s.Price).Sum(),
                July = realEstateRentResult.Where(x => x.Periodos == 7).Select(s => s.Price).Sum(),
                August = realEstateRentResult.Where(x => x.Periodos == 8).Select(s => s.Price).Sum(),
                September = realEstateRentResult.Where(x => x.Periodos == 9).Select(s => s.Price).Sum(),
                October = realEstateRentResult.Where(x => x.Periodos == 10).Select(s => s.Price).Sum(),
                November = realEstateRentResult.Where(x => x.Periodos == 11).Select(s => s.Price).Sum(),
                December = realEstateRentResult.Where(x => x.Periodos == 12).Select(s => s.Price).Sum(),
            };

            return result;
        }

        public virtual async Task<IList<PeriodicityItemsModel>> PreparePeriodicityItemsListAsync(TraderConnectionResult connectionResult, int year, int month)
        {
            var periodicityItems = await _periodicityItemService.GetAllPeriodicityItemsAsync();

            var list = new List<PeriodicityItemsModel>();

            foreach (var periodicityItem in periodicityItems)
            {
                var result = periodicityItem.PeriodicityItemType switch
                {
                    PeriodicityItemType.RealEstateRent => await PrepareRealEstateRentAsync(periodicityItem, connectionResult, year, true),
                    PeriodicityItemType.PaymentRealEstateRent => await PrepareRealEstateRentAsync(periodicityItem, connectionResult, year, false),
                    _ => new PeriodicityItemsModel
                    {
                        Paragraph = periodicityItem.Paragraph,
                        PeriodicityItemTypeName = await _localizationService.GetLocalizedEnumAsync(periodicityItem.PeriodicityItemType)
                    }
                };

                list.Add(result);
            }

            return list;
        }

        public virtual async Task<PeriodicityItemsSearchModel> PreparePeriodicityItemsSearchModelAsync(PeriodicityItemsSearchModel searchModel)
        {
            var trader = await _workContext.GetCurrentTraderAsync();

            searchModel.Period = DateTime.Now.ToUtcRelative();
            searchModel.TraderId = trader?.Id ?? 0;

            var left = new List<Dictionary<string, object>>() 
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<PeriodicityItemsSearchModel>(nameof(PeriodicityItemsSearchModel.TraderId)) 
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<PeriodicityItemsSearchModel>(nameof(PeriodicityItemsSearchModel.Period), FieldType.MonthDate)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-3" }, left, right);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return searchModel;
        }

        public virtual async Task<PeriodicityItemsTableModel> PreparePeriodicityItemsTableModelAsync(PeriodicityItemsTableModel tableModel)
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<PeriodicityItemsModel>(1, nameof(PeriodicityItemsModel.Paragraph)),
                ColumnConfig.Create<PeriodicityItemsModel>(2, nameof(PeriodicityItemsModel.PeriodicityItemTypeName)),
                ColumnConfig.Create<PeriodicityItemsModel>(3, nameof(PeriodicityItemsModel.January), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<PeriodicityItemsModel>(4, nameof(PeriodicityItemsModel.February), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<PeriodicityItemsModel>(5, nameof(PeriodicityItemsModel.March), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<PeriodicityItemsModel>(6, nameof(PeriodicityItemsModel.April), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<PeriodicityItemsModel>(7, nameof(PeriodicityItemsModel.May), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<PeriodicityItemsModel>(8, nameof(PeriodicityItemsModel.June), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<PeriodicityItemsModel>(9, nameof(PeriodicityItemsModel.July), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<PeriodicityItemsModel>(10, nameof(PeriodicityItemsModel.August), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<PeriodicityItemsModel>(11, nameof(PeriodicityItemsModel.September), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<PeriodicityItemsModel>(12, nameof(PeriodicityItemsModel.October), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<PeriodicityItemsModel>(13, nameof(PeriodicityItemsModel.November), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<PeriodicityItemsModel>(14, nameof(PeriodicityItemsModel.December), ColumnType.Decimal, style: textAlign)
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.PeriodicityItemsModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }
    }
}