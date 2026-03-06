using App.Core.Infrastructure;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Accounting;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Accounting;
using App.Services.Configuration;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Infra.Queries.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Accounting.Factories
{
    public partial interface IListingF5Factory
    {
        Task<ListingF5SearchModel> PrepareListingF5SearchModelAsync(ListingF5SearchModel searchModel);
        Task<ListingF5TableModel> PrepareListingF5TableModelAsync(ListingF5TableModel tableModel);
        Task<IList<ListingF5Result>> PrepareListingF5ListAsync(TraderConnectionResult connectionResult, int year, int month);
    }
    public class ListingF5Factory: IListingF5Factory
    {
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;

        public ListingF5Factory(
            IFieldConfigService fieldConfigService,
            IAppDataProvider dataProvider,
            ILocalizationService localizationService)
        {
            _fieldConfigService = fieldConfigService;
            _dataProvider = dataProvider;
            _localizationService = localizationService;
        }

        public virtual async Task<ListingF5SearchModel> PrepareListingF5SearchModelAsync(ListingF5SearchModel searchModel)
        {
            searchModel.Period = DateTime.Now.ToUtcRelative();

            var left = new List<Dictionary<string, object>>() 
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<ListingF5SearchModel>(nameof(ListingF5SearchModel.TraderId)) 
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ListingF5SearchModel>(nameof(ListingF5SearchModel.Period), FieldType.MonthDate)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-3" }, left, right);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return searchModel;
        }

        public virtual async Task<ListingF5TableModel> PrepareListingF5TableModelAsync(ListingF5TableModel tableModel)
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ListingF5Result>(0, nameof(ListingF5Result.Error)),
                ColumnConfig.Create<ListingF5Result>(2, nameof(ListingF5Result.CountryCode)),
                ColumnConfig.Create<ListingF5Result>(3, nameof(ListingF5Result.Vat)),
                ColumnConfig.Create<ListingF5Result>(4, nameof(ListingF5Result.Goods), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<ListingF5Result>(5, nameof(ListingF5Result.TriangleExchange), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<ListingF5Result>(6, nameof(ListingF5Result.Services), ColumnType.Decimal, style: textAlign)
            };

            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }

        public virtual async Task<IList<ListingF5Result>> PrepareListingF5ListAsync(TraderConnectionResult connectionResult, int year, int month)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pYear = new LinqToDB.Data.DataParameter("pYear", year);
            var pPeriod = new LinqToDB.Data.DataParameter("pPeriod", month);
            var pSchema = new LinqToDB.Data.DataParameter("pSchema", connectionResult.AccountingSchema);

            var query = new ListingF5Query(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get();
            var list = await _dataProvider.QueryAsync<ListingF5QueryResult>(connectionResult.Connection, query, pCompanyId, pYear, pPeriod, pSchema);

            var _shipKind = 9999; // Τριγωνική διακίνηση σταθερά

            var groups1 = list.GroupBy(g => g.InvId)
                .Select(x => new
                {
                    Vat = x.Select(s => s.Vat).First(),
                    VatNumber = x.Select(s => s.VatNumber).First(),
                    CountryCode = x.Select(s => s.CountryCode).First(),
                    Goods = x.Where(w => !w.Part.StartsWith("64") && !(w.ShipKind == _shipKind)).Sum(s => decimal.Round(s.Value, 2)),
                    Services = x.Where(w => w.Part.StartsWith("64") && !(w.ShipKind == _shipKind)).Sum(s => decimal.Round(s.Value, 2)),
                    TriangleExchange = x.Where(w => w.ShipKind == _shipKind).Sum(s => decimal.Round(s.Value, 2))
                }).ToList();

            var groups2 = groups1.GroupBy(g => g.Vat)
                .Select(x => new ListingF5Result()
                {
                    Vat = x.Key,
                    Group = "Αρχική",
                    VatNumber = x.Select(s => s.VatNumber).First(),
                    CountryCode = x.Select(s => s.CountryCode).First(),
                    Goods = x.Sum(s => s.Goods),
                    Services = x.Sum(s => s.Services),
                    TriangleExchange = x.Sum(s => s.TriangleExchange)
                }).ToList();

            foreach (var item in groups2)
            {
                if (!CommonHelper.IsLetterOrDigit(item.VatNumber))
                    item.Error = await _localizationService.GetResourceAsync("App.Errors.WrongVat");
                if (!ListingCountryResources.CountryDict.TryGetValue(item.CountryCode ?? "", out string countryCode))
                    item.Error = await _localizationService.GetResourceAsync("App.Errors.WrongCountryCode");
            }

            return groups2.OrderBy(x => x.Vat).ToList();
        }
    }
}