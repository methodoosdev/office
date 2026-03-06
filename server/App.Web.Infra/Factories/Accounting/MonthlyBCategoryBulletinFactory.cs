using App.Core;
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

namespace App.Web.Accounting.Factories
{
    public partial interface IMonthlyBCategoryBulletinFactory
    {
        Task<MonthlyBCategoryBulletinSearchModel> PrepareMonthlyBCategoryBulletinSearchModelAsync(MonthlyBCategoryBulletinSearchModel searchModel);
        Task<decimal> PrepareExpirationAsync(TraderConnectionResult connectionResult, int year, int period, string code);
        Task<IList<MonthlyBCategoryBulletinQuery>> GetMonthlyBCategoryBulletinQueryListAsync(TraderConnectionResult connectionResult, int year, int period);
        Task<MonthlyBCategoryBulletinTableModel> PrepareMonthlyBCategoryBulletinTableModelAsync(MonthlyBCategoryBulletinTableModel tableModel);
        Task<MonthlyBCategoryBulletinResultFormModel> PrepareMonthlyBCategoryBulletinResultFormModelAsync(MonthlyBCategoryBulletinResultFormModel resultModel, int year);
        Task<MonthlyBCategoryBulletinRemodelingCostsQueryModel> PrepareMonthlyBCategoryBulletinRemodelingCostsQueryModelAsync(MonthlyBCategoryBulletinRemodelingCostsQueryModel remodelingCostsModel);
        Task<PrepareMonthlyBCategoryBulletinModel> PrepareMonthlyBCategoryBulletinAsync(TraderConnectionResult connectionResult, MonthlyBCategoryBulletinSearchModel searchModel);
        Task<IList<MonthlyBCategoryBulletinPdfModel>> PrepareMonthlyBCategoryBulletinPdfAsync(TraderConnectionResult connectionResult, MonthlyBCategoryBulletinSearchModel searchModel);
        Task<MonthlyBCategoryExpirationPdfModel> GetBeginningxpirationAsync(TraderConnectionResult connectionResult, MonthlyBCategoryBulletinSearchModel searchModel);
    }
    public partial class MonthlyBCategoryBulletinFactory : IMonthlyBCategoryBulletinFactory
    {
        private readonly IFieldConfigService _fieldConfigService;
        private readonly ITaxFactorService _taxFactorService;
        private readonly ILocalizationService _localizationService;
        private readonly IAppDataProvider _dataProvider;
        private readonly IWorkContext _workContext;

        public MonthlyBCategoryBulletinFactory(
            IFieldConfigService fieldConfigService,
            ITaxFactorService taxFactorService,
            ILocalizationService localizationService,
            IAppDataProvider dataProvider,
            IWorkContext workContext)
        {
            _fieldConfigService = fieldConfigService;
            _localizationService = localizationService;
            _taxFactorService = taxFactorService;
            _localizationService = localizationService;
            _dataProvider = dataProvider;
            _workContext = workContext;
        }

        public async Task<IList<MonthlyBCategoryBulletinQuery>> GetMonthlyBCategoryBulletinQueryListAsync(
            TraderConnectionResult connectionResult, int year, int period)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pYear = new LinqToDB.Data.DataParameter("pYear", year);
            var pPeriod = new LinqToDB.Data.DataParameter("pPeriod", period);

            // When B'category book, parameter pSchema does not take part
            // Takes all the accounting accounts for the current year
            var results = await _dataProvider.QueryAsync<MonthlyBCategoryBulletinQuery>(connectionResult.Connection,
               new MonthlyBCategoryBulletinAccountingResultQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCompanyId, pYear, pPeriod);

            return results;
        }

        public virtual async Task<decimal> PrepareExpirationAsync(TraderConnectionResult connectionResult, int year, int period, string code)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pYear = new LinqToDB.Data.DataParameter("pYear", year);
            var pPeriod = new LinqToDB.Data.DataParameter("pPeriod", period);
            var pCode = new LinqToDB.Data.DataParameter("pCode", code);

            var expirationInventory = await _dataProvider.QueryAsync<MonthlyBCategoryBulletinQuery>(connectionResult.Connection,
                new MonthlyBCategoryExpirationQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCompanyId, pYear, pPeriod, pCode);

            return expirationInventory.Select(x => x.Value).Sum();
        }


        public virtual async Task<MonthlyBCategoryBulletinSearchModel> PrepareMonthlyBCategoryBulletinSearchModelAsync(MonthlyBCategoryBulletinSearchModel searchModel)
        {
            var lookupPanel = new List<Dictionary<string, object>>()
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<MonthlyBCategoryBulletinSearchModel>(nameof(MonthlyBCategoryBulletinSearchModel.TraderId), FieldConfigType.WithCategoryBookB)
            };

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<MonthlyBCategoryBulletinSearchModel>(nameof(MonthlyBCategoryBulletinSearchModel.Period), FieldType.MonthDate, className: "col-12 md:col-6"),
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<MonthlyBCategoryBulletinSearchModel>(nameof(MonthlyBCategoryBulletinSearchModel.ExpirationInventory), FieldType.Decimals, className: "col-12 md:col-3"),
                FieldConfig.Create<MonthlyBCategoryBulletinSearchModel>(nameof(MonthlyBCategoryBulletinSearchModel.ExpirationDepreciate), FieldType.Decimals, className: "col-12 md:col-3")
            };


            var fields = (searchModel.TraderId > 0)
                ? FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12" }, left, right)
                : FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6", "col-12" }, lookupPanel, left, right);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return searchModel;
        }

        public virtual async Task<MonthlyBCategoryBulletinTableModel> PrepareMonthlyBCategoryBulletinTableModelAsync(MonthlyBCategoryBulletinTableModel tableModel)
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var centerAlign = new Dictionary<string, string> { ["justifyContent"] = "center" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<MonthlyBCategoryBulletinModel>(1, nameof(MonthlyBCategoryBulletinModel.Id), width: 160),
                ColumnConfig.Create<MonthlyBCategoryBulletinModel>(2, nameof(MonthlyBCategoryBulletinModel.Description), width: 190),
                ColumnConfig.Create<MonthlyBCategoryBulletinModel>(3, nameof(MonthlyBCategoryBulletinModel.January), ColumnType.Decimal, width: 108,headerStyle: centerAlign, style: textAlign),
                ColumnConfig.Create<MonthlyBCategoryBulletinModel>(4, nameof(MonthlyBCategoryBulletinModel.February), ColumnType.Decimal, width: 108, headerStyle: centerAlign,style: textAlign),
                ColumnConfig.Create<MonthlyBCategoryBulletinModel>(5, nameof(MonthlyBCategoryBulletinModel.March), ColumnType.Decimal, width: 108, headerStyle: centerAlign, style: textAlign),
                ColumnConfig.Create<MonthlyBCategoryBulletinModel>(5, nameof(MonthlyBCategoryBulletinModel.April), ColumnType.Decimal, width: 108, headerStyle: centerAlign,  style: textAlign),
                ColumnConfig.Create<MonthlyBCategoryBulletinModel>(7, nameof(MonthlyBCategoryBulletinModel.May), ColumnType.Decimal, width: 108, headerStyle: centerAlign,  style: textAlign),
                ColumnConfig.Create<MonthlyBCategoryBulletinModel>(8, nameof(MonthlyBCategoryBulletinModel.June), ColumnType.Decimal, width: 108, headerStyle: centerAlign,  style: textAlign),
                ColumnConfig.Create<MonthlyBCategoryBulletinModel>(9, nameof(MonthlyBCategoryBulletinModel.July), ColumnType.Decimal, width: 108, headerStyle: centerAlign,  style: textAlign),
                ColumnConfig.Create<MonthlyBCategoryBulletinModel>(10, nameof(MonthlyBCategoryBulletinModel.August), ColumnType.Decimal, width: 108, headerStyle: centerAlign,  style: textAlign),
                ColumnConfig.Create<MonthlyBCategoryBulletinModel>(11, nameof(MonthlyBCategoryBulletinModel.September), ColumnType.Decimal, width: 108, headerStyle: centerAlign,  style: textAlign),
                ColumnConfig.Create<MonthlyBCategoryBulletinModel>(12, nameof(MonthlyBCategoryBulletinModel.October), ColumnType.Decimal, width: 108, headerStyle: centerAlign,  style: textAlign),
                ColumnConfig.Create<MonthlyBCategoryBulletinModel>(13, nameof(MonthlyBCategoryBulletinModel.November), ColumnType.Decimal, width: 108, headerStyle: centerAlign,  style: textAlign),
                ColumnConfig.Create<MonthlyBCategoryBulletinModel>(14, nameof(MonthlyBCategoryBulletinModel.December), ColumnType.Decimal, width: 108, headerStyle: centerAlign,  style: textAlign),
                ColumnConfig.Create<MonthlyBCategoryBulletinModel>(15, nameof(MonthlyBCategoryBulletinModel.Total), ColumnType.Decimal, width: 108, headerStyle: centerAlign,  style: textAlign),
                ColumnConfig.Create<MonthlyBCategoryBulletinModel>(16, nameof(MonthlyBCategoryBulletinModel.Type), width: 108, hidden: true)
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.MonthlyBCategoryBulletinModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }
        public virtual async Task<MonthlyBCategoryBulletinRemodelingCostsQueryModel> PrepareMonthlyBCategoryBulletinRemodelingCostsQueryModelAsync(MonthlyBCategoryBulletinRemodelingCostsQueryModel remodelingCostsModel)
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<MonthlyBCategoryBulletinRemodelingCostsQueryModel>(1, nameof(MonthlyBCategoryBulletinRemodelingCostsQueryModel.Code)),
                ColumnConfig.Create<MonthlyBCategoryBulletinRemodelingCostsQueryModel>(1, nameof(MonthlyBCategoryBulletinRemodelingCostsQueryModel.Description)),
                ColumnConfig.Create<MonthlyBCategoryBulletinRemodelingCostsQueryModel>(1, nameof(MonthlyBCategoryBulletinRemodelingCostsQueryModel.Rate)),
                ColumnConfig.Create<MonthlyBCategoryBulletinRemodelingCostsQueryModel>(1, nameof(MonthlyBCategoryBulletinRemodelingCostsQueryModel.Total)),
                ColumnConfig.Create<MonthlyBCategoryBulletinRemodelingCostsQueryModel>(1, nameof(MonthlyBCategoryBulletinRemodelingCostsQueryModel.Result))
            };

            remodelingCostsModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.MonthlyBCategoryBulletinRemodelingCostsQueryModel.Title"));
            remodelingCostsModel.CustomProperties.Add("columns", columns);

            return remodelingCostsModel;
        }

        public virtual async Task<MonthlyBCategoryBulletinResultFormModel> PrepareMonthlyBCategoryBulletinResultFormModelAsync(MonthlyBCategoryBulletinResultFormModel resultModel, int year)
        {
            var taxFactors = await _taxFactorService.GetAllTaxFactorsAsync();
            var taxIncome = taxFactors.Where(x => x.Year == year).Select(s => s.TaxIncome).Sum(); // hack
            var taxAdvance = taxFactors.Where(x => x.Year == year).Select(s => s.TaxAdvance).Sum(); // hack
            var taxAdvanceBCategory = taxFactors.Where(x => x.Year == year).Select(s => s.TaxAdvanceBCategory).Sum(); // hack

            var taxIncomeFormat = await _localizationService.GetResourceAsync("App.Models.MonthlyBCategoryBulletinResultFormModel.Extra.TaxIncome");
            var taxAdvanceFormat = await _localizationService.GetResourceAsync("App.Models.MonthlyBCategoryBulletinResultFormModel.Extra.TaxAdvance");

            var taxIncomeTitle = string.Format(taxIncomeFormat, taxIncome);
            var taxAdvanceTitle = string.Format(taxAdvanceFormat, taxAdvance, taxAdvanceBCategory);

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<MonthlyBCategoryBulletinResultFormModel>(nameof(MonthlyBCategoryBulletinResultFormModel.ExpirationInventory), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyBCategoryBulletinResultFormModel>(nameof(MonthlyBCategoryBulletinResultFormModel.NetProfitPeriod), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyBCategoryBulletinResultFormModel>(nameof(MonthlyBCategoryBulletinResultFormModel.RemodelingCosts), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyBCategoryBulletinResultFormModel>(nameof(MonthlyBCategoryBulletinResultFormModel.PreviousYearDamage), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyBCategoryBulletinResultFormModel>(nameof(MonthlyBCategoryBulletinResultFormModel.TaxProfitPeriod), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyBCategoryBulletinResultFormModel>(nameof(MonthlyBCategoryBulletinResultFormModel.TaxIncome), FieldType.Decimals, _readonly: true, description: taxIncomeTitle),
                FieldConfig.Create<MonthlyBCategoryBulletinResultFormModel>(nameof(MonthlyBCategoryBulletinResultFormModel.HoldingTaxAdvance), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyBCategoryBulletinResultFormModel>(nameof(MonthlyBCategoryBulletinResultFormModel.TaxAdvance), FieldType.Decimals, _readonly: true, description: taxAdvanceTitle),
                FieldConfig.Create<MonthlyBCategoryBulletinResultFormModel>(nameof(MonthlyBCategoryBulletinResultFormModel.PaymentPreviousYear), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyBCategoryBulletinResultFormModel>(nameof(MonthlyBCategoryBulletinResultFormModel.TaxesFee), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyBCategoryBulletinResultFormModel>(nameof(MonthlyBCategoryBulletinResultFormModel.AmountPayable), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyBCategoryBulletinResultFormModel>(nameof(MonthlyBCategoryBulletinResultFormModel.TaxReturn), FieldType.Decimals, _readonly: true)
            };

            resultModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.MonthlyBCategoryBulletinResultFormModel.Title"));
            resultModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, className: "col-12"));

            return resultModel;
        }

        public virtual async Task<PrepareMonthlyBCategoryBulletinModel> PrepareMonthlyBCategoryBulletinAsync(
            TraderConnectionResult connectionResult, MonthlyBCategoryBulletinSearchModel searchModel)
        {
            var customerTypeId = connectionResult.CustomerTypeId;

            var year = searchModel.Period.Year;
            var month = searchModel.Period.Month;

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pYear = new LinqToDB.Data.DataParameter("pYear", year);
            var pPeriod = new LinqToDB.Data.DataParameter("pPeriod", month);

            var results = await GetMonthlyBCategoryBulletinQueryListAsync(connectionResult, year, month);

            //current year 
            var currentYearCodes = results.Select(x => x.Code).Distinct().ToList();
            var currentPeriodFinancialResultList = new List<MonthlyBCategoryBulletinModel>();

            // Πωλήσεις
            var model = GetResulstByType(results);

            currentPeriodFinancialResultList.AddRange(model);

            var list = currentPeriodFinancialResultList.OrderBy(x => x.DisplayOrder).ThenBy(t => t.Id).ToList();

            // Από εδώ και κάτω πάμε για τα αποτελέσματα

            // Παίρνουμε τον αντίστοιχο συντελεστή φόρου για τη χρονιά που μας ενδιαφέρει
            var taxFactors = await _taxFactorService.GetAllTaxFactorsAsync();

            //netProfitPeriod: "Καθαρό κέρδος περιόδου"
            var expirationInventory = searchModel.ExpirationInventory;
            var expirationDepreciate = searchModel.ExpirationDepreciate;

            // Πωλήσεις
            var group7 = list.Where(x => x.Id.StartsWith("7")).Select(s => s.Total).Sum();
            // Αγορές
            var group2 = list.Where(x => x.Id.StartsWith("2")).Select(s => s.Total).Sum();
            // Δαπάνες
            var group6 = list.Where(x => x.Id.StartsWith("6")).Select(s => s.Total).Sum();
            // Δαπάνες
            var group8 = list.Where(x => x.Id.StartsWith("8")).Select(s => s.Total).Sum();

            var netProfitPeriod = group7 - group2 - group6 - group8 + expirationInventory - expirationDepreciate;

            //remodelingCosts: "Πλέον αναμορφούμενα έξοδα"
            var remodelingCostsList = await _dataProvider.QueryAsync<MonthlyBCategoryBulletinRemodelingCostsQueryModel>(connectionResult.Connection,
                new MonthlyBCategoryBulletinRemodelingCostsQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCompanyId, pYear, pPeriod);
            remodelingCostsList.ToList().ForEach(x => x.Result = Math.Round(x.Rate * x.Total / 100, 2));
            var remodelingCosts = remodelingCostsList.Select(x => x.Result).Sum();

            //previousYearDamage: "Μείον μεταφερόμενη ζημία προηγ.χρήσης"
            var pCode = new LinqToDB.Data.DataParameter("pCode", "49.02.00%");
            var previousYearDamageList = await _dataProvider.QueryAsync<MonthlyBCategoryBulletinQuery>(connectionResult.Connection,
                new MonthlyBCategoryBulletinAccountingCodeDebitQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCode, pCompanyId, pYear, pPeriod);
            var previousYearDamage = previousYearDamageList.Select(x => x.Total).Sum();

            //taxProfitPeriod "Φορολογητέο κέρδος περιόδου"
            var taxProfitPeriod = netProfitPeriod + remodelingCosts - previousYearDamage;

            //taxAdvance: "Φόρος εισοδήματος"
            var taxIncome = 0m;

            if(!(customerTypeId == 6))
            {
                var factor = taxFactors.Where(x => x.Year == year).Select(s => s.TaxIncome).Sum(); // hack
                taxIncome = taxProfitPeriod <= 0 ? 0m : Math.Round(taxProfitPeriod * factor / 100, 2);
            }

            else
            {
                taxIncome = CalcIndividualTaxIncome(taxProfitPeriod);
            }

            //holdingTaxAdvance: "Παρακρατούμενος φόρος"
            pCode = new LinqToDB.Data.DataParameter("pCode", "54.01.02%");
            var holdingTaxAdvanceList = await _dataProvider.QueryAsync<MonthlyBCategoryBulletinQuery>(connectionResult.Connection,
                new MonthlyBCategoryBulletinAccountingCodeCreditQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCode, pCompanyId, pYear, pPeriod);
            var holdingTaxAdvance = holdingTaxAdvanceList.Select(x => x.Total).Sum() * -1;

            //paymentTaxAdvance: "Προκαταβολή φόρου"
            var taxAdvance = 0m;

            if (!(customerTypeId == 6))
            {
                var tax = taxFactors.Where(x => x.Year == year).Select(s => s.TaxAdvance).Sum(); // hack
                var taxIncomeCalc = Math.Round(taxIncome * tax / 100, 2);
                taxAdvance = taxIncome <= 0 ? 0m : taxIncomeCalc <= holdingTaxAdvance ? 0m : taxIncomeCalc - holdingTaxAdvance;
            }

            else
            {
                var tax = taxFactors.Where(x => x.Year == year).Select(s => s.TaxAdvanceBCategory).Sum(); // hack
                var taxIncomeCalc = Math.Round(taxIncome * tax / 100, 2);
                taxAdvance = taxIncome <= 0 ? 0m : taxIncomeCalc <= holdingTaxAdvance ? 0m : taxIncomeCalc - holdingTaxAdvance;
            }

            //paymentPreviousYear: "Προκαταβολή προηγ.έτους"
            pCode = new LinqToDB.Data.DataParameter("pCode", "54.01.03%");
            var paymentPreviousYearList = await _dataProvider.QueryAsync<MonthlyBCategoryBulletinQuery>(connectionResult.Connection,
                new MonthlyBCategoryBulletinAccountingCodeDebitQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCode, pCompanyId, pYear, pPeriod);
            var paymentPreviousYear = paymentPreviousYearList.Select(x => x.Total).Sum();

            //taxesFee: "Τέλος επιτηδεύματος"
            var taxesFee = connectionResult.TaxesFee < 0 ? 0m : connectionResult.TaxesFee;

            //amountPayable: "Πληρωτέο ποσό"
            var amountPayable = taxIncome + taxAdvance - paymentPreviousYear - holdingTaxAdvance + taxesFee;

            //taxReturn: "Επιστροφή"
            var taxReturn = amountPayable < 0 ? amountPayable * -1 : 0m;

            var resultFormModel = new MonthlyBCategoryBulletinResultFormModel
            {
                ExpirationInventory = expirationInventory,
                NetProfitPeriod = netProfitPeriod,
                RemodelingCosts = remodelingCosts,
                PreviousYearDamage = previousYearDamage,
                TaxProfitPeriod = taxProfitPeriod,
                TaxIncome = taxIncome,
                HoldingTaxAdvance = holdingTaxAdvance,
                TaxAdvance = taxAdvance,
                PaymentPreviousYear = paymentPreviousYear < 0 ? 0 : paymentPreviousYear,
                TaxesFee = taxesFee,
                AmountPayable = amountPayable > 0 ? amountPayable : 0m,
                TaxReturn = amountPayable < 0 ? amountPayable * -1 : 0m
            };

            return new PrepareMonthlyBCategoryBulletinModel
            {
                CodeList = list,
                RemodelingCostsList = remodelingCostsList,
                ResultModel = resultFormModel
            };

        }

        //// Παίρνει τα σύνολα των κωδικών Πωλήσεις(7*), Αγορές(2*), Δαπάνες (6*), Δαπάνες(8*)
        public List<MonthlyBCategoryBulletinModel> GetResulstByType(IList<MonthlyBCategoryBulletinQuery> distinctCurrentYearList)
        {
            var currentPeriodResultList = new List<MonthlyBCategoryBulletinModel>();

            var currentGroup = distinctCurrentYearList
                .GroupBy(s => new { s.Code, s.Description, s.Type })
                .ToList();
            
                //.Where(g => !g.Key.Code.IsLike("2?.01*") && !g.Key.Code.IsLike("2?.06*"))

            foreach (var code in currentGroup)
            {
                var model = new MonthlyBCategoryBulletinModel
                {
                    Id = code.Key.Code, 
                    Description = code.Key.Description,
                    Type = code.Key.Type,
                    January = code.Where(x => x.Periodos == 1).Sum(s => s.Value),
                    February = code.Where(x => x.Periodos == 2).Sum(s => s.Value),
                    March = code.Where(x => x.Periodos == 3).Sum(s => s.Value),
                    April = code.Where(x => x.Periodos == 4).Sum(s => s.Value),
                    May = code.Where(x => x.Periodos == 5).Sum(s => s.Value),
                    June = code.Where(x => x.Periodos == 6).Sum(s => s.Value),
                    July = code.Where(x => x.Periodos == 7).Sum(s => s.Value),
                    August = code.Where(x => x.Periodos == 8).Sum(s => s.Value),
                    September = code.Where(x => x.Periodos == 9).Sum(s => s.Value),
                    October = code.Where(x => x.Periodos == 10).Sum(s => s.Value),
                    November = code.Where(x => x.Periodos == 11).Sum(s => s.Value),
                    December = code.Where(x => x.Periodos == 12).Sum(s => s.Value),
                    Total = code.Sum(s => s.Value)
                };

                if (model.Total == 0)
                    continue;

                currentPeriodResultList.Add(model);
            }

            return currentPeriodResultList;

        }

        public async Task<IList<MonthlyBCategoryBulletinPdfModel>> PrepareMonthlyBCategoryBulletinPdfAsync(TraderConnectionResult connectionResult, MonthlyBCategoryBulletinSearchModel searchModel)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pYear = new LinqToDB.Data.DataParameter("pYear", searchModel.Period.Year);
            var pPeriod = new LinqToDB.Data.DataParameter("pPeriod", searchModel.Period.Month);

            var results = await _dataProvider.QueryAsync<MonthlyBCategoryBulletinPdfResult>(connectionResult.Connection,
                new MonthlyBCategoryBulletinPdfQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCompanyId, pYear, pPeriod);

            var currentPeriodPdfResultList = new List<MonthlyBCategoryBulletinPdfModel>();

            var currentGroup = results
                .GroupBy(s => s.Type)
                .ToList();

            foreach (var type in currentGroup)
            {
                var model = new MonthlyBCategoryBulletinPdfModel
                {
                    Description = type.Key,
                    January = type.Where(x => x.Periodos == 1).Sum(s => s.Value),
                    February = type.Where(x => x.Periodos == 2).Sum(s => s.Value),
                    March = type.Where(x => x.Periodos == 3).Sum(s => s.Value),
                    April = type.Where(x => x.Periodos == 4).Sum(s => s.Value),
                    May = type.Where(x => x.Periodos == 5).Sum(s => s.Value),
                    June = type.Where(x => x.Periodos == 6).Sum(s => s.Value),
                    July = type.Where(x => x.Periodos == 7).Sum(s => s.Value),
                    August = type.Where(x => x.Periodos == 8).Sum(s => s.Value),
                    September = type.Where(x => x.Periodos == 9).Sum(s => s.Value),
                    October = type.Where(x => x.Periodos == 10).Sum(s => s.Value),
                    November = type.Where(x => x.Periodos == 11).Sum(s => s.Value),
                    December = type.Where(x => x.Periodos == 12).Sum(s => s.Value),
                    Total = type.Sum(s => s.Value)
                };

                currentPeriodPdfResultList.Add(model);
            }

            return currentPeriodPdfResultList;

        }

        public async Task<MonthlyBCategoryExpirationPdfModel> GetBeginningxpirationAsync(TraderConnectionResult connectionResult, MonthlyBCategoryBulletinSearchModel searchModel)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pYear = new LinqToDB.Data.DataParameter("pYear", searchModel.Period.Year);
            var pPeriod = new LinqToDB.Data.DataParameter("pPeriod", searchModel.Period.Month);

            var results = await _dataProvider.QueryAsync<MonthlyBCategoryExpirationPdfModel>(connectionResult.Connection,
                new MonthlyBCategoryBeginningPdfQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCompanyId, pYear, pPeriod);

            var beginningResult = results.FirstOrDefault();

            if(beginningResult != null)
            {
                beginningResult.Total = beginningResult.Goods + beginningResult.Consumables + beginningResult.Materials + 
                                        beginningResult.SpareParts + beginningResult.WarehouseOther;
            }
            else
            {
                beginningResult = new MonthlyBCategoryExpirationPdfModel()
                {
                    Type = await _localizationService.GetResourceAsync("App.Models.MonthlyBCategoryExpirationPdfModel.Titles.BeginningInventory"),
                    Goods = 0,
                    Materials = 0,
                    Consumables = 0,
                    SpareParts = 0,
                    WarehouseOther = 0,
                    Total = 0
                };
            }

            return beginningResult;

        }
        public decimal CalcIndividualTaxIncome(decimal taxProfitPeriod)
        {
            decimal tax = 0m;

            // Φορολογητέο κέρδος περιόδου 
            var periodProfit = taxProfitPeriod;

            // Υπολογισμός φόρου εισοδήματος
            if (periodProfit <= 10000)
                tax = periodProfit * (9.0m / 100);

            else if (10001 <= periodProfit && periodProfit <= 20000)
                tax = 900 + (periodProfit - 10000) * (22.0m / 100);

            else if (20001 <= periodProfit && periodProfit <= 30000)
                tax = 900 + 2200 + (periodProfit - 20000) * (28.0m / 100);

            else if (30001 <= periodProfit && periodProfit <= 40000)
                tax = 900 + 2200 + 2800 + (periodProfit - 30000) * (36.0m / 100);

            else
                tax = 900 + 2200 + 2800 + 3600 + (periodProfit - 40000) * (44.0m / 100);

            if (tax < 0)
                tax = 0;

            return tax;
        }
    }

}