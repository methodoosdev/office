using App.Core;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Accounting;
using App.Services.Configuration;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Infra.Queries.Accounting;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Accounting
{
    public partial interface IIntertemporalBFactory
    {

        Task<IntertemporalSearchModel> PrepareIntertemporalSearchModelAsync(IntertemporalSearchModel searchModel);
        Task<IList<IntertemporalData>> PrepareIntertemporalQueryListAsync(TraderConnectionResult connectionResult);
    }

    public class IntertemporalBFactory : IIntertemporalBFactory
    {
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public IntertemporalBFactory(IFieldConfigService fieldConfigService, IAppDataProvider dataProvider, ILocalizationService localizationService, IWorkContext workContext)
        {
            _fieldConfigService = fieldConfigService;
            _dataProvider = dataProvider;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<IntertemporalSearchModel> PrepareIntertemporalSearchModelAsync(IntertemporalSearchModel searchModel)
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            searchModel.TraderId = trader?.Id ?? 0;

            var left = new List<Dictionary<string, object>>()
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<IntertemporalSearchModel>(nameof(IntertemporalSearchModel.TraderId), FieldConfigType.WithCategoryBookB)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<IntertemporalSearchModel>(nameof(IntertemporalSearchModel.ExpirationInventory), FieldType.Decimals, className: "col-12 md:col-6"),
                FieldConfig.Create<IntertemporalSearchModel>(nameof(IntertemporalSearchModel.ExpirationDepreciate), FieldType.Decimals, className: "col-12 md:col-6"),
            };

            var fields = (trader != null)
                ? FieldConfig.CreateFields(new string[] { "col-12 md:col-6" }, right)
                : FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6" }, left, right);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));
            searchModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.IntertemporalModel.Title"));

            return searchModel;
        }

        public async Task<IList<IntertemporalData>> PrepareIntertemporalQueryListAsync(TraderConnectionResult connectionResult)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);

            // When B'category book, parameter pSchema does not take part
            var results = await _dataProvider.QueryAsync<IntertemporalResult>(connectionResult.Connection, IntertemporalResultQuery.GetBCategory, pCompanyId);

            var years = results.Select(x => x.Year).Distinct().ToList();
            var intertemporalList = new List<IntertemporalModel>();

            foreach (var year in years)
            {
                var sales = results.FirstOrDefault(x => x.Year == year && x.Id == nameof(IntertemporalModel.Sales))?.Total ?? 0;
                var income = results.FirstOrDefault(x => x.Year == year && x.Id == nameof(IntertemporalModel.Income))?.Total ?? 0;
                var openingInventory = results.FirstOrDefault(x => x.Year == year && x.Id == nameof(IntertemporalModel.OpeningInventory))?.Total ?? 0;
                var purchases = results.FirstOrDefault(x => x.Year == year && x.Id == nameof(IntertemporalModel.Purchases))?.Total ?? 0;
                var closingInventory = results.FirstOrDefault(x => x.Year == year && x.Id == nameof(IntertemporalModel.ClosingInventory))?.Total ?? 0;
                var expenses = results.FirstOrDefault(x => x.Year == year && x.Id == nameof(IntertemporalModel.Expenses))?.Total ?? 0;
                var grossProfit = sales - openingInventory - purchases + closingInventory;
                var costOfGoodsSold = openingInventory + purchases - closingInventory;
                var netProfits = grossProfit - expenses + income;

                var model = new IntertemporalModel();
                model.Year = year;
                model.Sales = sales; // Πωλήσεις
                model.Income = income; // Άλλα έσοδα
                model.OpeningInventory = openingInventory; // Αποθέματα έναρξης
                model.Purchases = purchases; // Αγορές
                model.ClosingInventory = closingInventory; // Αποθέματα λήξης
                model.Expenses = expenses; // Έξοδα

                model.GrossProfit = grossProfit; // Μεικτό (Πωλήσεις - αξία υλικών)
                model.GrossProfitRate = sales == 0 ? 0 : Math.Round((grossProfit / sales) * 100, 2); // Μεικτό (Πωλήσεις - αξία υλικών) %

                model.CostOfGoodsSold = costOfGoodsSold; // Κόστος πωληθέντων (υλικά)
                model.CostOfGoodsSoldRate = sales == 0 ? 0 : Math.Round((costOfGoodsSold / sales) * 100, 2); // Κόστος πωληθέντων (υλικά) %

                model.ExpensesRate = sales == 0 ? 0 : Math.Round((expenses / sales) * 100, 2); // Έξοδα % 

                model.NetProfits = netProfits; // Καθαρά κέρδη
                model.NetProfitsRate = sales == 0 ? 0 : Math.Round((netProfits / sales) * 100, 2); // Καθαρά κέρδη %

                model.SalesClosingInventoryRate = sales == 0 ? 0 : Math.Round((closingInventory / sales) * 100, 2);
                model.PurchasesClosingInventoryRate = purchases == 0 ? 0 : Math.Round((closingInventory / purchases) * 100, 2);

                intertemporalList.Add(model);
            }

            var list = new List<IntertemporalData>();

            foreach (var item in intertemporalList)
            {
                //var properties = item.GetType().GetProperties();
                //var yearName = nameof(IntertemporalData.Year);
                //var year = (int)properties.First(x => x.Name == yearName).GetValue(item, null);

                //properties = properties.Where(x => x.PropertyType == typeof(decimal)).ToArray();

                //foreach (PropertyInfo property in properties)
                //{
                //    var model = new IntertemporalData();
                //    model.Year = year.ToString();
                //    model.Value = (decimal)property.GetValue(item, null);
                //    model.Description = await _localizationService.GetResourceAsync($"App.Models.IntertemporalModel.{property.Name}");

                //    list.Add(model);
                //}
                var data = await ToIntertemporalDataAsync(item);
                list.AddRange(data);
            }
            
            return list;
        }

        private async Task<IList<IntertemporalData>> ToIntertemporalDataAsync(IntertemporalModel model)
        {
            var intertemporalDataList = new List<IntertemporalData>();
            var yearString = model.Year.ToString();

            // Use reflection to get properties of the IntertemporalModel class
            PropertyInfo[] properties = typeof(IntertemporalModel).GetProperties();

            foreach (var property in properties)
            {
                // Only process decimal properties
                if (property.PropertyType == typeof(decimal))
                {
                    // Create a new IntertemporalData instance for each decimal property
                    intertemporalDataList.Add(new IntertemporalData
                    {
                        Description = await _localizationService.GetResourceAsync($"App.Models.IntertemporalModel.Columns.{property.Name}"), // Property name as Description
                        Year = yearString,
                        Value = (decimal)property.GetValue(model) // Get the value of the decimal property
                    });
                }
            }

            return intertemporalDataList;
        }
    }
}
