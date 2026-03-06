using App.Core;
using App.Core.Domain.Accounting;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Accounting;
using App.Services;
using App.Services.Common;
using App.Services.Configuration;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Infra.Queries.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Accounting
{
    public partial interface IPayoffLiabilitiesFactory
    {
        Task<PayoffLiabilitySearchModel> PreparePayoffLiabilitySearchModelAsync(PayoffLiabilitySearchModel searchModel);
        Task<(IList<PayoffLiabilityModel> list, IList<PayoffLiabilityFactorModel> factorList)> PreparePayoffLiabilityListAsync(TraderConnectionResult connectionResult, int year);
        Task<PayoffLiabilityTableModel> PreparePayoffLiabilityTableModelAsync(PayoffLiabilityTableModel tableModel);
        Task<PayoffLiabilityFactorTableModel> PreparePayoffLiabilityFactorTableModelAsync(PayoffLiabilityFactorTableModel tableModel);
    }
    public class PayoffLiabilitiesFactory : IPayoffLiabilitiesFactory
    {
        private readonly IAppDataProvider _dataProvider;
        private readonly ISqlConnectionService _connectionService;
        private readonly IFieldConfigService _fieldConfigService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public PayoffLiabilitiesFactory(
            IAppDataProvider dataProvider, 
            ISqlConnectionService connectionService,
            IFieldConfigService fieldConfigService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _dataProvider = dataProvider;
            _connectionService = connectionService;
            _fieldConfigService = fieldConfigService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        protected string GetExpression(string[] values)
        {
            string[] list = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
                list[i] = $"A.CODE LIKE '{values[i]}'";

            var text = string.Join(" OR ", list);

            return text;
        }

        public virtual async Task<(IList<PayoffLiabilityModel> list, IList<PayoffLiabilityFactorModel> factorList)> PreparePayoffLiabilityListAsync(TraderConnectionResult connectionResult, int year)
        {
            var months = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var days = new[] { 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
            var monthNames = new[] {
                "January", "February", "March", "April", "May", "June",
                "July", "August", "September", "October", "November", "December" };


            decimal[] GetFactorsPeriod(IList<PayoffLiabilityResult> results)
            {
                decimal[] values = new decimal[12];
                foreach (var month in months)
                    values[month - 1] = results.Where(x => x.Period >= 0 && x.Period <= month).Sum(s => s.Value);

                return values;
            }

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pYear = new LinqToDB.Data.DataParameter("pYear", year);
            var pSchema = new LinqToDB.Data.DataParameter("pSchema", connectionResult.AccountingSchema);

            // Κυκλοφορούν ενεργητικό
            var query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["CurrentAssets"]));
            var currentAssets = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var currentAssetsResult = GetFactorsPeriod(currentAssets);

            // Βραχυπρόθεσμες υποχρεώσεις
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["ShortTermLiabilities"]));
            var shortTermLiabilities = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var shortTermLiabilitiesResult = GetFactorsPeriod(shortTermLiabilities);

            // Αποθέματα
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["Inventories"]));
            var inventories = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var inventoriesResult = GetFactorsPeriod(inventories);

            // Διαθέσιμα
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["Available"]));
            var available = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var availableResult = GetFactorsPeriod(available);

            // Απαιτήσεις
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["Requirements"]));
            var requirements = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var requirementsResult = GetFactorsPeriod(requirements);

            // Ετήσιες πωλήσεις
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["AnnualSales"]));
            var annualSales = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var annualSalesResult = GetFactorsPeriod(annualSales);

            // Κόστος πωληθέντων
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(new string[] { "2%" }));
            var costOfGoodsSold = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var costOfGoodsSoldResult = GetFactorsPeriod(costOfGoodsSold);

            for (var i = 0; i < costOfGoodsSoldResult.Length; i++)
                costOfGoodsSoldResult[i] = costOfGoodsSoldResult[i] - inventoriesResult[i];

            // Αποσβέσεις
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["Depreciation"]));
            var depreciation = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var depreciationResult = GetFactorsPeriod(depreciation);

            // Λειτουργικά έξοδα 
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["OperatingExpenses"]));
            var operatingExpenses = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var operatingExpensesResult = GetFactorsPeriod(operatingExpenses);

            // 'Ιδια κεφάλαια 
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["Equity"]));
            var equity = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var equityResult = GetFactorsPeriod(equity);

            // Συνολικά κεφάλαια 
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["TotalFunds"]));
            var totalFunds = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var totalFundsResult = GetFactorsPeriod(totalFunds);

            // Ξένα κεφάλαια 
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["ForeignCapitals"]));
            var foreignCapitals = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var foreignCapitalsResult = GetFactorsPeriod(foreignCapitals);

            // Σύνολο υποχρεώσεων 
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["TotalLiabilities"]));
            var totalLiabilities = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var totalLiabilitiesResult = GetFactorsPeriod(totalLiabilities);

            // Σύνολο ενεργητικού 
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["TotalAssets"]));
            var totalAssets = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var totalAssetsResult = GetFactorsPeriod(totalAssets);

            // Δανειακές υποχρεώσεις 
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["LoanObligations"]));
            var loanObligations = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var loanObligationsResult = GetFactorsPeriod(loanObligations);

            // Ετήσιο εργατικό κόστος
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["AnnualLaborCost"]));
            var annualLaborCost = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var annualLaborCostResult = GetFactorsPeriod(annualLaborCost);

            // Σύνολο παθητικού
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["TotalLiability"]));
            var totalLiability = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var totalLiabilityResult = GetFactorsPeriod(totalLiability);

            // Αξία κτησης παγίων 
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["AcquisitionValueOfFixedAssets"]));
            var acquisitionValueOfFixedAssets = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var acquisitionValueOfFixedAssetsResult = GetFactorsPeriod(acquisitionValueOfFixedAssets);

            // Σωρευμένες αποσβέσεις
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["AccumulatedDepreciation"]));
            var accumulatedDepreciation = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var accumulatedDepreciationResult = GetFactorsPeriod(accumulatedDepreciation);

            // Ετήσιοι τόκοι 
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["AnnualInterest"]));
            var annualInterest = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var annualInterestResult = GetFactorsPeriod(annualInterest);

            var query1 = string.Format(PayoffLiabilityQuery.All, GetExpression(new string[] { "70%", "71%", "72%", "73%", "74%", "75%", "76%", "77%", "79%" }));
            var res1 = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query1, pCompanyId, pYear, pSchema);
            var result1 = GetFactorsPeriod(res1);

            var query2 = string.Format(PayoffLiabilityQuery.All, GetExpression(new string[] { "60%", "61%", "62%", "63%", "64%", "66%", "68%" }));
            var res2 = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query2, pCompanyId, pYear, pSchema);
            var result2 = GetFactorsPeriod(res2);

            var query3 = string.Format(PayoffLiabilityQuery.All, GetExpression(new string[] { "60%", "61%", "62%", "63%", "64%", "65%", "66%", "68%" }));
            var res3 = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query3, pCompanyId, pYear, pSchema);
            var result3 = GetFactorsPeriod(res3);

            // Κέρδη προ τόκων ή φόρων (ΕΒΙΤ ή λειτουργικά)
            decimal[] earningsBeforeInterestAndTaxesResult = new decimal[12];
            for (var i = 0; i < earningsBeforeInterestAndTaxesResult.Length; i++)
                earningsBeforeInterestAndTaxesResult[i] = result1[i] - (costOfGoodsSoldResult[i] + result2[i]);

            // Καθαρό κέρδος 
            decimal[] grossProfitMarginResult = new decimal[12];
            for (var i = 0; i < grossProfitMarginResult.Length; i++)
                grossProfitMarginResult[i] = result1[i] - (costOfGoodsSoldResult[i] + result3[i]);

            // Μικτό περιθώριο κέρδους
            decimal[] netProfitResult = new decimal[12];
            for (var i = 0; i < netProfitResult.Length; i++)
                netProfitResult[i] = annualSalesResult[i] - costOfGoodsSoldResult[i];

            // Έξοδα και αμοιβές προσωπικού 
            query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["StaffExpensesAndFees"]));
            var staffExpensesAndFees = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connectionResult.Connection, query, pCompanyId, pYear, pSchema);
            var staffExpensesAndFeesResult = GetFactorsPeriod(staffExpensesAndFees);

            // Αριθμός απασχολούμενων 
            //query = string.Format(PayoffLiabilityQuery.All, GetExpression(PayoffLiabilityResources.Numerals["NumberOfEmployees"]));
            //var numberOfEmployees = await _dataProvider.QueryAsync<PayoffLiabilityResult>(connection, query, pCompanyId, pYear, pSchema);
            //var numberOfEmployeesResult = GetFactorsPeriod(numberOfEmployees);

            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                throw new Exception("Error");

            var pCompId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.HyperPayrollId);
            var results = await _dataProvider.QueryAsync<PayoffLiabilityWorkersResult>(result.Connection, WorkerScheduleQuery.Names, pCompId);
            var index = results.Count();
            var numberOfEmployeesResult = new int[] { index, index, index, index, index, index, index, index, index, index, index, index };



            decimal getGeneralLiquidity(int month)// Γενική ρευστότητα
            {
                var currentAsset = currentAssetsResult[month - 1]; // Κυκλοφορούν ενεργητικό
                var shortTermLiability = shortTermLiabilitiesResult[month - 1]; // Βραχυπρόθεσμες υποχρεώσεις

                return shortTermLiability == 0 ? currentAsset : currentAsset / shortTermLiability;
            }

            decimal getImmediateLiquidity(int month) // Άμεση ρευστότητα
            {
                var currentAsset = currentAssetsResult[month - 1]; // Κυκλοφορούν ενεργητικό
                var inventory = inventoriesResult[month - 1]; // Αποθέματα 
                var shortTermLiability = shortTermLiabilitiesResult[month - 1]; // Βραχυπρόθεσμες υποχρεώσεις

                var value = currentAsset - inventory;

                return shortTermLiability == 0 ? value : value / shortTermLiability;
            }

            decimal getCashFlow(int month) // Ταμειακή ρευστότητα 
            {
                var available = availableResult[month - 1]; // Διαθέσιμα
                var shortTermLiability = shortTermLiabilitiesResult[month - 1]; // Βραχυπρόθεσμες υποχρεώσεις

                return shortTermLiability == 0 ? available : available / shortTermLiability;
            }

            decimal getAverageReceivablesCollectionTime(int month) // Μέσος χρόνος είσπραξης απαιτήσεων
            {
                var requirements = requirementsResult[month - 1] * days[month - 1]; // Απαιτήσεις
                var annualSales = annualSalesResult[month - 1]; // Ετήσιες πωλήσεις

                return annualSales == 0 ? requirements : requirements / annualSales;
            }

            decimal getStockRecycling(int month) // Ανακύκλωση αποθεμάτων
            {
                var costOfGoodsSold = costOfGoodsSoldResult[month - 1]; // Κόστος πωληθέντων
                var inventory = inventoriesResult[month - 1]; // Αποθέματα 

                return inventory == 0 ? costOfGoodsSold : costOfGoodsSold / inventory;

            }

            decimal getPaymentTimeOfShortTermLiabilities(int month) // Χρόνος εξόφλησης βραχυπρόθεσμων υποχρεώσεων
            {
                var costOfGoodsSold = costOfGoodsSoldResult[month - 1]; // Κόστος πωληθέντων
                var shortTermLiability = shortTermLiabilitiesResult[month - 1] * days[month - 1]; // Βραχυπρόθεσμες υποχρεώσεις
                var depreciation = depreciationResult[month - 1]; // Αποσβέσεις 

                var value = costOfGoodsSold - depreciation;

                return value == 0 ? shortTermLiability : shortTermLiability / value;
            }

            decimal getReceivableCollectionTime(int month) // Χρόνος είσπραξης απαιτήσεων
            {
                var annualSales = annualSalesResult[month - 1]; // Ετήσιες πωλήσεις
                var requirements = requirementsResult[month - 1]; // Απαιτήσεις

                var value = annualSales / requirements;

                //var annualSales = days[month - 1] / (value == 0 ? 1 : value); // Πωλήσεις περιόδου

                return value == 0 ? days[month - 1] : days[month - 1] / value;
            }

            decimal getDefenseTimePeriod(int month) // Αμυντικού χρονικού διαστήματος
            {
                var value = currentAssetsResult[month - 1] - inventoriesResult[month - 1]; // Κυκλοφορούν ενεργητικό - Αποθέματα
                var operatingExpenses = operatingExpensesResult[month - 1] / days[month - 1]; // Ημερήσια Λειτουργικά έξοδα 

                return operatingExpenses == 0 ? value : value / operatingExpenses;
            }

            // Κεφαλαιακής δομής και βιωσιμότητας
            decimal getAutonomy(int month) // Αυτονομίας
            {
                var equity = equityResult[month - 1];
                var totalFunds = totalFundsResult[month - 1];

                return totalFunds == 0 ? equity : equity / totalFunds;
            }

            decimal getOverdraft(int month) // Υπερχρέωσης
            {
                var equity = equityResult[month - 1];
                var foreignCapitals = foreignCapitalsResult[month - 1];

                return equity == 0 ? foreignCapitals : foreignCapitals / equity;
            }

            decimal getCurrentAssetsToLiabilities(int month) // Κυκλοφορούν ενεργητικό προς υποχρεώσεις
            {
                var currentAssets = currentAssetsResult[month - 1];
                var totalLiabilities = totalLiabilitiesResult[month - 1];

                return totalLiabilities == 0 ? currentAssets : currentAssets / totalLiabilities;
            }

            decimal getDebtBurden(int month) // Δανειακής επιβάρυνσης
            {
                var loanObligations = loanObligationsResult[month - 1];
                var totalAssets = totalAssetsResult[month - 1];

                return totalAssets == 0 ? loanObligations : loanObligations / totalAssets;
            }

            decimal getAssetsToLiabilities(int month) // Πάγια προς παθητικό
            {
                return 0;
            }

            decimal getCapitalIntensive(int month) // Εντάσεως κεφαλαίου
            {
                var inventories = inventoriesResult[month - 1];
                var annualLaborCost = annualLaborCostResult[month - 1];

                return annualLaborCost == 0 ? inventories : inventories / annualLaborCost;
            }

            decimal getAgingOfFixedAssets(int month) // Παλαιότητας παγίων 
            {
                var accumulatedDepreciation = accumulatedDepreciationResult[month - 1];
                var acquisitionValueOfFixedAssets = acquisitionValueOfFixedAssetsResult[month - 1];

                return acquisitionValueOfFixedAssets == 0 ? accumulatedDepreciation : accumulatedDepreciation / acquisitionValueOfFixedAssets;
            }

            decimal getInterestCoverage(int month) // Κάλυψης τόκων
            {
                var earningsBeforeInterestAndTaxes = earningsBeforeInterestAndTaxesResult[month - 1];
                var annualInterest = annualInterestResult[month - 1];

                return annualInterest == 0 ? earningsBeforeInterestAndTaxes : earningsBeforeInterestAndTaxes / annualInterest;
            }

            // Αποδοτικότητας
            decimal getGrossProfitMargin(int month) // Μεικτού περιθωρίου κέρδους %
            {
                var netProfit = netProfitResult[month - 1];
                var annualSales = annualSalesResult[month - 1];

                return annualSales == 0 ? netProfit : netProfit / annualSales;
            }

            decimal getOperatingProfitMargin(int month) // Περιθωρίου λειτουργικών  κερδών %
            {
                var earningsBeforeInterestAndTaxes = earningsBeforeInterestAndTaxesResult[month - 1];
                var annualSales = annualSalesResult[month - 1];

                return annualSales == 0 ? earningsBeforeInterestAndTaxes : earningsBeforeInterestAndTaxes / annualSales;
            }

            decimal getNetProfit(int month) // Καθαρού κέρδους
            {
                var grossProfitMargin = grossProfitMarginResult[month - 1];
                var annualSales = annualSalesResult[month - 1];

                return annualSales == 0 ? grossProfitMargin : grossProfitMargin / annualSales;
            }

            decimal getOperatingPerformance(int month) // Απόδοση λειτουργίας
            {
                var earningsBeforeInterestAndTaxes = earningsBeforeInterestAndTaxesResult[month - 1];
                var totalAssets = totalAssetsResult[month - 1];

                return totalAssets == 0 ? earningsBeforeInterestAndTaxes : earningsBeforeInterestAndTaxes / totalAssets;
            }

            // Εξόδων
            decimal getAssetsMaintenance(int month) // Συντήρησης παγίων
            {
                return 0;
            }

            decimal getOperatingExpenses(int month) // Εξόδων λειτουργίας
            {
                return 0;
            }

            decimal getStaffPerformance(int month) // Απόδοσης προσωπικού 
            {
                var staffExpensesAndFees = staffExpensesAndFeesResult[month - 1];
                var annualSales = annualSalesResult[month - 1];

                return annualSales == 0 ? staffExpensesAndFees : staffExpensesAndFees / annualSales;
            }

            decimal getWagesToEmployeesNumber(int month) // Αμοιβές απασχολούμενων προς τον αριθμό τους
            {
                var staffExpensesAndFees = staffExpensesAndFeesResult[month - 1];
                var numberOfEmployees = numberOfEmployeesResult[month - 1];

                return numberOfEmployees == 0 ? staffExpensesAndFees : staffExpensesAndFees / numberOfEmployees;
            }

            decimal getProfitsToWages(int month) // Κέρδη προς αμοιβές απασχολούμενων
            {
                var staffExpensesAndFees = staffExpensesAndFeesResult[month - 1];
                var netProfit = netProfitResult[month - 1];

                return staffExpensesAndFees == 0 ? netProfit : netProfit / staffExpensesAndFees;
            }

            //decimal getOperatingPerformance(int month) // Εντάσεως κεφαλαίου
            //{
            //    var staffExpensesAndFees = staffExpensesAndFeesResult[month - 1];
            //    var netProfit = netProfitResult[month - 1];

            //    return staffExpensesAndFees == 0 ? netProfit : netProfit / staffExpensesAndFees;
            //}


            var payoffLiabilities = await PayoffLiabilityType.GeneralLiquidity.ToSelectionListAsync();
            var liabilities = Enum.GetValues(typeof(PayoffLiabilityType)).OfType<PayoffLiabilityType>()
                .Select(x => new SelectionItemList { Value = Convert.ToInt32(x), Label = x.ToString() }).ToList();

            var list = new List<PayoffLiabilityModel>();

            void payoffLiabilitiesFunc(Func<int, decimal> func, string payoffLiabilityType, string categoryType)
            {
                var model = new PayoffLiabilityModel();
                model.PayoffLiabilityTypeName = payoffLiabilityType;
                model.PayoffLiabilityCategoryTypeName = categoryType;

                foreach (var month in months)
                {
                    var prop = model.GetType().GetProperty(monthNames[month - 1]);
                    prop.SetValue(model, func(month), null);
                }
                list.Add(model);
            }

            // Ρευστότητας
            // Γενική ρευστότητα
            payoffLiabilitiesFunc(getGeneralLiquidity, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.GeneralLiquidity),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Liquidity));
            // Άμεση ρευστότητα
            payoffLiabilitiesFunc(getImmediateLiquidity, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.ImmediateLiquidity),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Liquidity));
            // Ταμειακή ρευστότητα 
            payoffLiabilitiesFunc(getCashFlow, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.CashFlow),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Liquidity));
            // Μέσος χρόνος είσπραξης απαιτήσεων
            payoffLiabilitiesFunc(getAverageReceivablesCollectionTime, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.AverageReceivablesCollectionTime),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Liquidity));
            // Ανακύκλωση αποθεμάτων 
            payoffLiabilitiesFunc(getStockRecycling, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.StockRecycling),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Liquidity));
            // Χρόνος εξόφλησης βραχυπρόθεσμων υποχρεώσεων
            payoffLiabilitiesFunc(getPaymentTimeOfShortTermLiabilities, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.PaymentTimeOfShortTermLiabilities),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Liquidity));
            // Χρόνος είσπραξης απαιτήσεων
            payoffLiabilitiesFunc(getReceivableCollectionTime, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.ReceivableCollectionTime),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Liquidity));
            // Αμυντικού χρονικού διαστήματος
            payoffLiabilitiesFunc(getDefenseTimePeriod, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.DefenseTimePeriod),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Liquidity));

            // Κεφαλαιακής δομής και βιωσιμότητας
            // Αυτονομίας
            payoffLiabilitiesFunc(getAutonomy, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.Autonomy),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.CapitalStructureSustainability));
            // Υπερχρέωσης
            payoffLiabilitiesFunc(getOverdraft, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.Overdraft),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.CapitalStructureSustainability));
            /// Κυκλοφορούν ενεργητικό προς υποχρεώσεις
            payoffLiabilitiesFunc(getCurrentAssetsToLiabilities, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.CurrentAssetsToLiabilities),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.CapitalStructureSustainability));
            // Δανειακής επιβάρυνσης
            payoffLiabilitiesFunc(getDebtBurden, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.DebtBurden),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.CapitalStructureSustainability));
            // Πάγια προς παθητικό
            payoffLiabilitiesFunc(getAssetsToLiabilities, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.AssetsToLiabilities),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.CapitalStructureSustainability));
            // Εντάσεως κεφαλαίου
            payoffLiabilitiesFunc(getCapitalIntensive, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.CapitalIntensive),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.CapitalStructureSustainability));
            // Παλαιότητας παγίων
            payoffLiabilitiesFunc(getAgingOfFixedAssets, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.AgingOfFixedAssets),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.CapitalStructureSustainability));
            // Κάλυψης τόκων
            payoffLiabilitiesFunc(getInterestCoverage, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.InterestCoverage),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.CapitalStructureSustainability));

            // Αποδοτικότητας
            // Μεικτού περιθωρίου κέρδους %
            payoffLiabilitiesFunc(getGrossProfitMargin, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.GrossProfitMargin),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Efficiency));
            // Περιθωρίου λειτουργικών  κερδών %
            payoffLiabilitiesFunc(getOperatingProfitMargin, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.OperatingProfitMargin),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Efficiency));
            // Καθαρού κέρδους
            payoffLiabilitiesFunc(getNetProfit, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.NetProfit),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Efficiency));
            // Απόδοση λειτουργίας
            payoffLiabilitiesFunc(getOperatingPerformance, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.OperatingPerformance),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Efficiency));

            // Εξόδων
            // Συντήρησης παγίων
            payoffLiabilitiesFunc(getAssetsMaintenance, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.AssetsMaintenance),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Expenses));
            // Εξόδων λειτουργίας
            payoffLiabilitiesFunc(getOperatingExpenses, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.OperatingExpenses),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Expenses));
            // Απόδοσης προσωπικού
            payoffLiabilitiesFunc(getStaffPerformance, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.StaffPerformance),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Expenses));
            // Αμοιβές απασχολούμενων προς τον αριθμό τους
            payoffLiabilitiesFunc(getWagesToEmployeesNumber, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.WagesToEmployeesNumber),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Expenses));
            // Κέρδη προς αμοιβές απασχολούμενων
            payoffLiabilitiesFunc(getProfitsToWages, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.ProfitsToWages),
                await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Expenses));
            // Εντάσεως κεφαλαίου
            //payoffLiabilitiesFunc(getCapitalIntensive, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityType.CapitalIntensive),
            //    await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityCategoryType.Expenses));

            var factorList = new List<PayoffLiabilityFactorModel>();

            void factorsFunc(decimal[] values, string factorType)
            {
                var model = new PayoffLiabilityFactorModel();
                model.PayoffLiabilityFactorTypeName = factorType;

                foreach (var month in months)
                {
                    var prop = model.GetType().GetProperty(monthNames[month - 1]);
                    prop.SetValue(model, values[month - 1], null);
                }
                factorList.Add(model);
            }

            factorsFunc(currentAssetsResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.CurrentAssets));
            factorsFunc(shortTermLiabilitiesResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.ShortTermLiabilities));
            factorsFunc(inventoriesResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.Inventories));
            factorsFunc(availableResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.Available));
            factorsFunc(requirementsResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.Requirements));
            factorsFunc(annualSalesResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.AnnualSales));
            factorsFunc(costOfGoodsSoldResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.CostOfGoodsSold));
            factorsFunc(depreciationResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.Depreciation));
            factorsFunc(operatingExpensesResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.OperatingExpenses));
            factorsFunc(equityResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.Equity));
            factorsFunc(totalFundsResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.TotalFunds));
            factorsFunc(foreignCapitalsResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.ForeignCapitals));
            factorsFunc(totalLiabilitiesResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.TotalLiabilities));
            factorsFunc(totalAssetsResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.TotalAssets));
            factorsFunc(loanObligationsResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.LoanObligations));
            factorsFunc(annualLaborCostResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.AnnualLaborCost));
            factorsFunc(totalLiabilityResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.TotalLiability));
            factorsFunc(acquisitionValueOfFixedAssetsResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.AcquisitionValueOfFixedAssets));
            factorsFunc(accumulatedDepreciationResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.AccumulatedDepreciation));
            factorsFunc(annualInterestResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.AnnualInterest));
            factorsFunc(earningsBeforeInterestAndTaxesResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.EarningsBeforeInterestAndTaxes));
            factorsFunc(grossProfitMarginResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.GrossProfitMargin));
            factorsFunc(netProfitResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.NetProfit));
            factorsFunc(staffExpensesAndFeesResult, await _localizationService.GetLocalizedEnumAsync(PayoffLiabilityFactorType.StaffExpensesAndFees));

            return (list, factorList);
        }

        public virtual async Task<PayoffLiabilitySearchModel> PreparePayoffLiabilitySearchModelAsync(PayoffLiabilitySearchModel searchModel)
        {
            var trader = await _workContext.GetCurrentTraderAsync();

            searchModel.Period = DateTime.Now.ToUtcRelative();
            searchModel.TraderId = trader?.Id ?? 0;

            var left = new List<Dictionary<string, object>>()
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<PayoffLiabilitySearchModel>(nameof(PayoffLiabilitySearchModel.TraderId), FieldConfigType.WithCategoryBookC)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<PayoffLiabilitySearchModel>(nameof(PayoffLiabilitySearchModel.Period), FieldType.YearDate)
            };

            var fields = trader != null
                ? FieldConfig.CreateFields(new string[] { "col-12 md:col-3" }, right)
                : FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-3" }, left, right);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return searchModel;
        }

        public virtual async Task<PayoffLiabilityTableModel> PreparePayoffLiabilityTableModelAsync(PayoffLiabilityTableModel tableModel)
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<PayoffLiabilityModel>(1, nameof(PayoffLiabilityModel.PayoffLiabilityTypeName), width: 260),
                ColumnConfig.Create<PayoffLiabilityModel>(2, nameof(PayoffLiabilityModel.PayoffLiabilityCategoryTypeName), width: 260, hidden: true),
                ColumnConfig.Create<PayoffLiabilityModel>(3, nameof(PayoffLiabilityModel.January), ColumnType.Decimal, style: rightAlign, headerStyle: centerAlign),
                ColumnConfig.Create<PayoffLiabilityModel>(4, nameof(PayoffLiabilityModel.February), ColumnType.Decimal, style: rightAlign, headerStyle: centerAlign),
                ColumnConfig.Create<PayoffLiabilityModel>(5, nameof(PayoffLiabilityModel.March), ColumnType.Decimal, style: rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityModel>(6, nameof(PayoffLiabilityModel.April), ColumnType.Decimal, style: rightAlign, headerStyle: centerAlign),
                ColumnConfig.Create<PayoffLiabilityModel>(7, nameof(PayoffLiabilityModel.May), ColumnType.Decimal, style: rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityModel>(8, nameof(PayoffLiabilityModel.June), ColumnType.Decimal, style : rightAlign, headerStyle: centerAlign),
                ColumnConfig.Create<PayoffLiabilityModel>(9, nameof(PayoffLiabilityModel.July), ColumnType.Decimal, style : rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityModel>(10, nameof(PayoffLiabilityModel.August), ColumnType.Decimal, style : rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityModel>(11, nameof(PayoffLiabilityModel.September), ColumnType.Decimal, style : rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityModel>(12, nameof(PayoffLiabilityModel.October), ColumnType.Decimal, style : rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityModel>(13, nameof(PayoffLiabilityModel.November), ColumnType.Decimal, style: rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityModel>(14, nameof(PayoffLiabilityModel.December), ColumnType.Decimal, style : rightAlign, headerStyle : centerAlign)
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.PayoffLiabilityModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }

        public virtual async Task<PayoffLiabilityFactorTableModel> PreparePayoffLiabilityFactorTableModelAsync(PayoffLiabilityFactorTableModel tableModel)
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<PayoffLiabilityFactorModel>(1, nameof(PayoffLiabilityFactorModel.PayoffLiabilityFactorTypeName), width: 260),
                ColumnConfig.Create<PayoffLiabilityFactorModel>(2, nameof(PayoffLiabilityFactorModel.January), ColumnType.Decimal, style : rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityFactorModel>(3, nameof(PayoffLiabilityFactorModel.February), ColumnType.Decimal, style : rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityFactorModel>(4, nameof(PayoffLiabilityFactorModel.March), ColumnType.Decimal, style : rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityFactorModel>(5, nameof(PayoffLiabilityFactorModel.April), ColumnType.Decimal, style : rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityFactorModel>(6, nameof(PayoffLiabilityFactorModel.May), ColumnType.Decimal, style : rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityFactorModel>(7, nameof(PayoffLiabilityFactorModel.June), ColumnType.Decimal, style : rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityFactorModel>(8, nameof(PayoffLiabilityFactorModel.July), ColumnType.Decimal, style : rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityFactorModel>(9, nameof(PayoffLiabilityFactorModel.August), ColumnType.Decimal, style : rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityFactorModel>(10, nameof(PayoffLiabilityFactorModel.September), ColumnType.Decimal, style : rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityFactorModel>(11, nameof(PayoffLiabilityFactorModel.October), ColumnType.Decimal, style : rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityFactorModel>(12, nameof(PayoffLiabilityFactorModel.November), ColumnType.Decimal, style : rightAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayoffLiabilityFactorModel>(13, nameof(PayoffLiabilityFactorModel.December), ColumnType.Decimal, style : rightAlign, headerStyle : centerAlign)
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.PayoffLiabilityFactorModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }
    }
}