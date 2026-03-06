using App.Core.Domain.Traders;
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
    public partial interface IListingF4Factory
    {
        Task<ListingF4SearchModel> PrepareListingF4SearchModelAsync(ListingF4SearchModel searchModel);
        Task<ListingF4TableModel> PrepareListingF4TableModelAsync(ListingF4TableModel tableModel);
        Task<IList<ListingF4Result>> PrepareListingF4ListAsync(TraderConnectionResult connectionResult, int year, int month);
    }
    public class ListingF4Factory : IListingF4Factory
    {
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;

        public ListingF4Factory(
            IFieldConfigService fieldConfigService,
            IAppDataProvider dataProvider,
            ILocalizationService localizationService)
        {
            _fieldConfigService = fieldConfigService;
            _dataProvider = dataProvider;
            _localizationService = localizationService;
        }

        public virtual async Task<ListingF4SearchModel> PrepareListingF4SearchModelAsync(ListingF4SearchModel searchModel)
        {
            searchModel.Period = DateTime.Now.ToUtcRelative();

            var left = new List<Dictionary<string, object>>() 
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<ListingF4SearchModel>(nameof(ListingF4SearchModel.TraderId)) 
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ListingF4SearchModel>(nameof(ListingF4SearchModel.Period), FieldType.MonthDate)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-3" }, left, right);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return searchModel;
        }

        public virtual async Task<ListingF4TableModel> PrepareListingF4TableModelAsync(ListingF4TableModel tableModel)
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ListingF4Result>(0, nameof(ListingF4Result.Error)),
                ColumnConfig.Create<ListingF4Result>(2, nameof(ListingF4Result.CountryCode)),
                ColumnConfig.Create<ListingF4Result>(3, nameof(ListingF4Result.Vat)),
                ColumnConfig.Create<ListingF4Result>(4, nameof(ListingF4Result.Goods), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<ListingF4Result>(5, nameof(ListingF4Result.TriangleExchange), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<ListingF4Result>(6, nameof(ListingF4Result.Services), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<ListingF4Result>(7, nameof(ListingF4Result.Products4200), ColumnType.Decimal, style: textAlign)
            };

            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }

        public virtual async Task<IList<ListingF4Result>> PrepareListingF4ListAsync(TraderConnectionResult connectionResult, int year, int month)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pYear = new LinqToDB.Data.DataParameter("pYear", year);
            var pPeriod = new LinqToDB.Data.DataParameter("pPeriod", month);
            var pSchema = new LinqToDB.Data.DataParameter("pSchema", connectionResult.AccountingSchema);

            var query = new ListingF4Query(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get();
            var list = await _dataProvider.QueryAsync<ListingF4QueryResult>(connectionResult.Connection, query, pCompanyId, pYear, pPeriod, pSchema);

            var _shipKind = 9999; // Τριγωνική διακίνηση σταθερά

            var groups1 = list.GroupBy(g => g.InvId)
                .Select(x => new
                {
                    Vat = x.Select(s => s.Vat).First(),
                    VatNumber = x.Select(s => s.VatNumber).First(),
                    CountryCode = x.Select(s => s.CountryCode).First(),
                    Goods = x.Where(w => !w.Part.Equals("70.07") && !(w.ShipKind == _shipKind)).Sum(s => decimal.Round(s.Value, 2)),
                    Services = x.Where(w => w.Part.Equals("70.07") && !(w.ShipKind == _shipKind)).Sum(s => decimal.Round(s.Value, 2)),
                    TriangleExchange = x.Where(w => w.ShipKind == _shipKind).Sum(s => decimal.Round(s.Value, 2))
                }).ToList();

            var groups2 = groups1.GroupBy(g => g.Vat)
                .Select(x => new ListingF4Result()
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