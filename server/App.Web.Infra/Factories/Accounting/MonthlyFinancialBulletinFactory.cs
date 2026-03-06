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
    public partial interface IMonthlyFinancialBulletinFactory
    {
        Task<IList<SelectionList>> PrepareBranchesAsync(TraderConnectionResult connectionResult, int year, int month);
        Task<IList<MonthlyFinancialBulletinQuery>> GetMonthlyFinancialBulletinQueryListAsync(TraderConnectionResult connectionResult, int period, int currentYear, int previousYear);
        Task<decimal> PrepareExpirationAsync(TraderConnectionResult connectionResult, int year, string code);
        Task<MonthlyFinancialBulletinSearchModel> PrepareMonthlyFinancialBulletinSearchModelAsync(MonthlyFinancialBulletinSearchModel searchModel, IList<SelectionList> branches);
        Task<MonthlyFinancialBulletinTableModel> PrepareMonthlyFinancialBulletinTableModelAsync(MonthlyFinancialBulletinTableModel tableModel);
        //Task<MonthlyFinancialBulletinFormModel> PrepareMonthlyFinancialBulletinFormModelAsync(Trader trader, string connection, MonthlyFinancialBulletinSearchModel searchModel);
        Task<MonthlyFinancialBulletinResultFormModel> PrepareMonthlyFinancialBulletinResultFormModelAsync(MonthlyFinancialBulletinResultFormModel resultModel, int year);
        Task<MonthlyFinancialBulletinRemodelingCostsQueryModel> PrepareMonthlyFinancialBulletinRemodelingCostsQueryModelAsync(MonthlyFinancialBulletinRemodelingCostsQueryModel remodelingCostsModel);
        Task<PrepareMonthlyFinancialBulletinModel> PrepareMonthlyFinancialBulletinAsync(TraderConnectionResult connectionResult, MonthlyFinancialBulletinSearchModel searchModel);
        IList<MonthlyFinancialBulletinCodes> GetPredictionCodes(bool sixDegree);
    }
    public partial class MonthlyFinancialBulletinFactory : IMonthlyFinancialBulletinFactory
    {
        private readonly IFieldConfigService _fieldConfigService;
        private readonly ITaxFactorService _taxFactorService;
        private readonly ILocalizationService _localizationService;
        private readonly IAppDataProvider _dataProvider;

        public MonthlyFinancialBulletinFactory(
            IFieldConfigService fieldConfigService,
            ITaxFactorService taxFactorService,
            ILocalizationService localizationService,
            IAppDataProvider dataProvider)
        {
            _fieldConfigService = fieldConfigService;
            _localizationService = localizationService;
            _taxFactorService = taxFactorService;
            _localizationService = localizationService;
            _dataProvider = dataProvider;
        }

        public virtual async Task<IList<SelectionList>> PrepareBranchesAsync(TraderConnectionResult connectionResult, int year, int month)
        {
            var results = await GetMonthlyFinancialBulletinQueryListAsync(connectionResult, month, year, year - 1);

            var list = results.Select(x => x.Branch).Distinct().OrderBy(o => o).ToList();

            var branches = await (list.SelectAwait(async x => new SelectionList
            {
                Label = x == "00" ? await _localizationService.GetResourceAsync("App.Common.Central") : x,
                Value = x
            })).ToListAsync();

            var defaultItemText = await _localizationService.GetResourceAsync("App.Common.Choice");
            branches.Insert(0, new SelectionList { Label = defaultItemText, Value = null });

            return branches;
        }

        public async Task<IList<MonthlyFinancialBulletinQuery>> GetMonthlyFinancialBulletinQueryListAsync(
            TraderConnectionResult connectionResult, int period, int currentYear, int previousYear)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pPreviousYear = new LinqToDB.Data.DataParameter("pPreviousYear", previousYear);
            var pCurrentYear = new LinqToDB.Data.DataParameter("pCurrentYear", currentYear);
            var pPeriod = new LinqToDB.Data.DataParameter("pPeriod", period);
            var pSchema = new LinqToDB.Data.DataParameter("pSchema", connectionResult.AccountingSchema);

            // When B'category book, parameter pSchema does not take part
            var results = await _dataProvider.QueryAsync<MonthlyFinancialBulletinQuery>(connectionResult.Connection,
               new MonthlyFinancialBulletinAccountingResultQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCompanyId, pPreviousYear, pCurrentYear, pPeriod, pSchema);

            return results;
        }

        public virtual async Task<decimal> PrepareExpirationAsync(TraderConnectionResult connectionResult, int year, string code)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pCurrentYear = new LinqToDB.Data.DataParameter("pCurrentYear", year);
            var pSchema = new LinqToDB.Data.DataParameter("pSchema", connectionResult.AccountingSchema);
            var pCode = new LinqToDB.Data.DataParameter("pCode", code);

            var expirationInventory = await _dataProvider.QueryAsync<MonthlyFinancialBulletinQuery>(connectionResult.Connection,
                new MonthlyFinancialBulletinExpirationQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCompanyId, pCurrentYear, pSchema, pCode);

            return expirationInventory.Select(x => x.Total).Sum();
        }

        private async Task<IList<MonthlyFinancialBulletinQuery>> GetMonthlyFinancialBulletinQueryPredictionListAsync(bool isCredit,
            TraderConnectionResult connectionResult, int period, int currentYear, int previousYear, string docTypeIds,
            string id, string description, int level)
        {
            var pId = new LinqToDB.Data.DataParameter("pId", id);
            var pDescription = new LinqToDB.Data.DataParameter("pDescription", description);
            var pLevel = new LinqToDB.Data.DataParameter("pLevel", level);

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pPreviousYear = new LinqToDB.Data.DataParameter("pPreviousYear", previousYear);
            var pCurrentYear = new LinqToDB.Data.DataParameter("pCurrentYear", currentYear);
            var pPeriod = new LinqToDB.Data.DataParameter("pPeriod", period);

            var query = string.Format(isCredit ?
                    new MonthlyFinancialBulletinPredictionsCreditQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get() :
                    new MonthlyFinancialBulletinPredictionsResultQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), docTypeIds);

            var results = await _dataProvider.QueryAsync<MonthlyFinancialBulletinQuery>(connectionResult.Connection,
                query, pId, pDescription, pLevel, pCompanyId, pPreviousYear, pCurrentYear, pPeriod);

            return results;
        }

        public virtual async Task<MonthlyFinancialBulletinSearchModel> PrepareMonthlyFinancialBulletinSearchModelAsync(MonthlyFinancialBulletinSearchModel searchModel, IList<SelectionList> branches)
        {
            var lookupPanel = new List<Dictionary<string, object>>()
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<MonthlyFinancialBulletinSearchModel>(nameof(MonthlyFinancialBulletinSearchModel.TraderId), FieldConfigType.WithCategoryBookC)
            };

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<MonthlyFinancialBulletinSearchModel>(nameof(MonthlyFinancialBulletinSearchModel.Periodos), FieldType.MonthDate, className: "col-12 md:col-6"),
                FieldConfig.Create<MonthlyFinancialBulletinSearchModel>(nameof(MonthlyFinancialBulletinSearchModel.Branch), FieldType.Select, options: branches, className: "col-12 md:col-6"),
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<MonthlyFinancialBulletinSearchModel>(nameof(MonthlyFinancialBulletinSearchModel.ExpirationInventory), FieldType.Decimals, className: "col-12 md:col-3"),
                FieldConfig.Create<MonthlyFinancialBulletinSearchModel>(nameof(MonthlyFinancialBulletinSearchModel.ExpirationDepreciate), FieldType.Decimals, className: "col-12 md:col-3"),
                FieldConfig.Create<MonthlyFinancialBulletinSearchModel>(nameof(MonthlyFinancialBulletinSearchModel.Predictions), FieldType.Checkbox, className: "col-12 md:col-3")
            };

            var fields = (searchModel.TraderId > 0)
                ? FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12" }, left, right)
                : FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6", "col-12" }, lookupPanel, left, right);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return searchModel;
        }

        public virtual async Task<MonthlyFinancialBulletinTableModel> PrepareMonthlyFinancialBulletinTableModelAsync(MonthlyFinancialBulletinTableModel tableModel)
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<MonthlyFinancialBulletinModel>(1, nameof(MonthlyFinancialBulletinModel.Id), width: 90),
                ColumnConfig.Create<MonthlyFinancialBulletinModel>(2, nameof(MonthlyFinancialBulletinModel.Description), width: 260),
                ColumnConfig.Create<MonthlyFinancialBulletinModel>(3, nameof(MonthlyFinancialBulletinModel.PeriodCurrentYear), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<MonthlyFinancialBulletinModel>(4, nameof(MonthlyFinancialBulletinModel.PeriodPreviousYear), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<MonthlyFinancialBulletinModel>(5, nameof(MonthlyFinancialBulletinModel.PeriodDefference), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<MonthlyFinancialBulletinModel>(6, nameof(MonthlyFinancialBulletinModel.PeriodRate), ColumnType.Percent, width: 120, style: textAlign),
                ColumnConfig.Create<MonthlyFinancialBulletinModel>(7, nameof(MonthlyFinancialBulletinModel.ProgressivelyCurrentYear), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<MonthlyFinancialBulletinModel>(8, nameof(MonthlyFinancialBulletinModel.ProgressivelyPreviousYear), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<MonthlyFinancialBulletinModel>(9, nameof(MonthlyFinancialBulletinModel.ProgressivelyDefference), ColumnType.Decimal, width: 120, style: textAlign),
                ColumnConfig.Create<MonthlyFinancialBulletinModel>(10, nameof(MonthlyFinancialBulletinModel.ProgressivelyRate), ColumnType.Percent, width: 120, style: textAlign)
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.MonthlyFinancialBulletinModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }

        public virtual async Task<MonthlyFinancialBulletinFormModel> PrepareMonthlyFinancialBulletinFormModelAsync(TraderConnectionResult connectionResult, MonthlyFinancialBulletinSearchModel searchModel)
        {
            var results = await GetMonthlyFinancialBulletinQueryListAsync(connectionResult, searchModel.Periodos.Month, searchModel.Periodos.Year, searchModel.Periodos.Year - 1);

            var list = results.Select(x => x.Branch).Distinct().OrderBy(o => o).ToList();

            var branches = await (list.SelectAwait(async x => new SelectionList
            {
                Label = x == "00" ? await _localizationService.GetResourceAsync("App.Common.Central") : x,
                Value = x
            })).ToListAsync();

            var defaultItemText = await _localizationService.GetResourceAsync("App.Common.Choice");
            branches.Insert(0, new SelectionList { Label = defaultItemText, Value = null });

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<MonthlyFinancialBulletinSearchModel>(nameof(MonthlyFinancialBulletinSearchModel.Periodos), FieldType.MonthDate, className: "col-12 md:col-3"),
                FieldConfig.Create<MonthlyFinancialBulletinSearchModel>(nameof(MonthlyFinancialBulletinSearchModel.Branch), FieldType.Select, options: branches, className: "col-12 md:col-3"),
                FieldConfig.Create<MonthlyFinancialBulletinSearchModel>(nameof(MonthlyFinancialBulletinSearchModel.ExpirationInventory), FieldType.Decimals, className: "col-12 md:col-3"),
                FieldConfig.Create<MonthlyFinancialBulletinSearchModel>(nameof(MonthlyFinancialBulletinSearchModel.ExpirationDepreciate), FieldType.Decimals, className: "col-12 md:col-3"),
                FieldConfig.Create<MonthlyFinancialBulletinSearchModel>(nameof(MonthlyFinancialBulletinSearchModel.Predictions), FieldType.Checkbox, className: "col-12 md:col-3"),
            };

            var formModel = new MonthlyFinancialBulletinFormModel();
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }

        public virtual async Task<MonthlyFinancialBulletinResultFormModel> PrepareMonthlyFinancialBulletinResultFormModelAsync(MonthlyFinancialBulletinResultFormModel resultModel, int year)
        {
            var taxFactors = await _taxFactorService.GetAllTaxFactorsAsync();
            var taxIncome = taxFactors.Where(x => x.Year == year).Select(s => s.TaxIncome).Sum(); // hack
            var taxAdvance = taxFactors.Where(x => x.Year == year).Select(s => s.TaxAdvance).Sum(); // hack
            var taxIncomeFormat = await _localizationService.GetResourceAsync("App.Models.MonthlyFinancialBulletinResultFormModel.Extra.TaxIncome");
            var taxAdvanceFormat = await _localizationService.GetResourceAsync("App.Models.MonthlyFinancialBulletinResultFormModel.Extra.TaxAdvance");

            var taxIncomeTitle = string.Format(taxIncomeFormat, taxIncome);
            var taxAdvanceTitle = string.Format(taxAdvanceFormat, taxAdvance);

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<MonthlyFinancialBulletinResultFormModel>(nameof(MonthlyFinancialBulletinResultFormModel.ExpirationInventory), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyFinancialBulletinResultFormModel>(nameof(MonthlyFinancialBulletinResultFormModel.NetProfitPeriod), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyFinancialBulletinResultFormModel>(nameof(MonthlyFinancialBulletinResultFormModel.RemodelingCosts), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyFinancialBulletinResultFormModel>(nameof(MonthlyFinancialBulletinResultFormModel.PreviousYearDamage), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyFinancialBulletinResultFormModel>(nameof(MonthlyFinancialBulletinResultFormModel.TaxProfitPeriod), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyFinancialBulletinResultFormModel>(nameof(MonthlyFinancialBulletinResultFormModel.TaxIncome), FieldType.Decimals, _readonly: true, description: taxIncomeTitle),
                FieldConfig.Create<MonthlyFinancialBulletinResultFormModel>(nameof(MonthlyFinancialBulletinResultFormModel.HoldingTaxAdvance), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyFinancialBulletinResultFormModel>(nameof(MonthlyFinancialBulletinResultFormModel.TaxAdvance), FieldType.Decimals, _readonly: true, description: taxAdvanceTitle),
                FieldConfig.Create<MonthlyFinancialBulletinResultFormModel>(nameof(MonthlyFinancialBulletinResultFormModel.PaymentPreviousYear), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyFinancialBulletinResultFormModel>(nameof(MonthlyFinancialBulletinResultFormModel.TaxesFee), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyFinancialBulletinResultFormModel>(nameof(MonthlyFinancialBulletinResultFormModel.AmountPayable), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<MonthlyFinancialBulletinResultFormModel>(nameof(MonthlyFinancialBulletinResultFormModel.TaxReturn), FieldType.Decimals, _readonly: true)
            };

            resultModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.MonthlyFinancialBulletinResultFormModel.Title"));
            resultModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, className: "col-12"));

            return resultModel;
        }

        public virtual async Task<MonthlyFinancialBulletinRemodelingCostsQueryModel> PrepareMonthlyFinancialBulletinRemodelingCostsQueryModelAsync(MonthlyFinancialBulletinRemodelingCostsQueryModel remodelingCostsModel)
        {
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<MonthlyFinancialBulletinRemodelingCostsQueryModel>(1, nameof(MonthlyFinancialBulletinRemodelingCostsQueryModel.Id)),
                ColumnConfig.Create<MonthlyFinancialBulletinRemodelingCostsQueryModel>(1, nameof(MonthlyFinancialBulletinRemodelingCostsQueryModel.Description)),
                ColumnConfig.Create<MonthlyFinancialBulletinRemodelingCostsQueryModel>(1, nameof(MonthlyFinancialBulletinRemodelingCostsQueryModel.Rate)),
                ColumnConfig.Create<MonthlyFinancialBulletinRemodelingCostsQueryModel>(1, nameof(MonthlyFinancialBulletinRemodelingCostsQueryModel.Total)),
                ColumnConfig.Create<MonthlyFinancialBulletinRemodelingCostsQueryModel>(1, nameof(MonthlyFinancialBulletinRemodelingCostsQueryModel.Result))
            };

            remodelingCostsModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.MonthlyFinancialBulletinRemodelingCostsQueryModel.Title"));
            remodelingCostsModel.CustomProperties.Add("columns", columns);

            return remodelingCostsModel;
        }

        public IList<MonthlyFinancialBulletinCodes> GetPredictionCodes(bool sixDegree)
        {
            var list1 = new List<MonthlyFinancialBulletinCodes>
            {
                new MonthlyFinancialBulletinCodes{ Id = "20.98", Description = "Προβλεπόμενες εκπτώσεις", Level = 2 },
                new MonthlyFinancialBulletinCodes{ Id = "20.98.00", Description = "Προβλεπόμενες εκπτώσεις", Level = 3 },
                new MonthlyFinancialBulletinCodes{ Id = "20.98.00.0000", Description = "Προβλεπόμενες εκπτώσεις", Level = 4 },
                new MonthlyFinancialBulletinCodes{ Id = "20.98.00.0000.0000", Description = "Προβλεπόμενες εκπτώσεις", Level = 5 },
                new MonthlyFinancialBulletinCodes{ Id = "20.99", Description = "Αφαιρούμενα πιστωτικά", Level = 2 },
                new MonthlyFinancialBulletinCodes{ Id = "20.99.00", Description = "Αφαιρούμενα πιστωτικά", Level = 3 },
                new MonthlyFinancialBulletinCodes{ Id = "20.99.00.0000", Description = "Αφαιρούμενα πιστωτικά", Level = 4 },
                new MonthlyFinancialBulletinCodes{ Id = "20.99.00.0000.0000", Description = "Αφαιρούμενα πιστωτικά", Level = 5 },
            };
            var list2 = new List<MonthlyFinancialBulletinCodes>
            {
                new MonthlyFinancialBulletinCodes{ Id = "20.98.00.0000.0000.0000", Description = "Προβλεπόμενες εκπτώσεις", Level = 5 },
                new MonthlyFinancialBulletinCodes{ Id = "20.99.00.0000.0000.0000", Description = "Αφαιρούμενα πιστωτικά", Level = 5 },
            };

            return sixDegree ? list2.Concat(list1).ToList() : list1;
        }

        public virtual async Task<PrepareMonthlyFinancialBulletinModel> PrepareMonthlyFinancialBulletinAsync(
            TraderConnectionResult connectionResult, MonthlyFinancialBulletinSearchModel searchModel)
        {
            var month = searchModel.Periodos.Month;
            var currentYear = searchModel.Periodos.Year;
            var previousYear = searchModel.Periodos.Year - 1;

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", connectionResult.CompanyId);
            var pCurrentYear = new LinqToDB.Data.DataParameter("pCurrentYear", currentYear);
            var pPeriod = new LinqToDB.Data.DataParameter("pPeriod", month);
            var pSchema = new LinqToDB.Data.DataParameter("pSchema", connectionResult.AccountingSchema);

            var codes = await _dataProvider.QueryAsync<MonthlyFinancialBulletinCodes>(connectionResult.Connection,
                new MonthlyFinancialBulletinAccountingCodesQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pSchema, pCompanyId);

            var sixDegree = codes.Any(x => x.Level == 6);
            var results = await GetMonthlyFinancialBulletinQueryListAsync(connectionResult, month, currentYear, previousYear);

            if (!string.IsNullOrEmpty(searchModel.Branch))
                results = results.Where(x => x.Branch == searchModel.Branch).ToList();

            // if Predictions
            if (searchModel.Predictions)
            {
                codes = codes.Concat(GetPredictionCodes(sixDegree)).ToList();
                var list1 = await GetMonthlyFinancialBulletinQueryPredictionListAsync(false, connectionResult, month, currentYear, previousYear, connectionResult.DiscountPredictions,
                    sixDegree ? "20.98.00.0000.0000.0000" : "20.98.00.0000.0000", "Προβλεπόμενες εκπτώσεις", sixDegree ? 6 : 5);

                var list2 = await GetMonthlyFinancialBulletinQueryPredictionListAsync(true, connectionResult, month, currentYear, previousYear, connectionResult.DeductibleCredits,
                    sixDegree ? "20.99.00.0000.0000.0000" : "20.99.00.0000.0000", "Αφαιρούμενα πιστωτικά", sixDegree ? 6 : 5);

                (list1 as List<MonthlyFinancialBulletinQuery>).ForEach(x => x.Total = x.Total * -1);

                //join two lists
                results = results.Concat(list1).ToList().Concat(list2).ToList();
            }

            var currentYearList = results.Where(x => x.Year == currentYear).ToList();
            var previousYearList = results.Where(x => x.Year == previousYear).ToList();

            //current year 
            var currentYearIds = currentYearList.Select(x => x.Id).Distinct().ToList();
            var currentPeriodFinancialResultList = new List<MonthlyFinancialBulletinModel>();

            foreach (var id in currentYearIds)
            {
                var periodCurrentYear = currentYearList.Where(x => x.Id == id && x.Periodos == month).FirstOrDefault()?.Total ?? 0m;
                var progressivelyCurrentYear = currentYearList.Where(x => x.Id == id).Select(s => s.Total).Sum();
                var model = new MonthlyFinancialBulletinModel
                {
                    Id = id,
                    Description = codes.Where(x => x.Id == id).FirstOrDefault()?.Description,
                    PeriodCurrentYear = periodCurrentYear,
                    ProgressivelyCurrentYear = progressivelyCurrentYear,
                    Level = sixDegree ? 6 : 5
                };
                currentPeriodFinancialResultList.Add(model);
            }

            //previous year
            var previousYearIds = previousYearList.Select(x => x.Id).Distinct().ToList();
            var previousPeriodFinancialResultList = new List<MonthlyFinancialBulletinModel>();

            foreach (var id in previousYearIds)
            {
                var periodPreviousYear = previousYearList.Where(x => x.Id == id && x.Periodos == month).FirstOrDefault()?.Total ?? 0m;
                var progressivelyPreviousYear = previousYearList.Where(x => x.Id == id).Select(s => s.Total).Sum();
                var model = new MonthlyFinancialBulletinModel
                {
                    Id = id,
                    Description = codes.Where(x => x.Id == id).FirstOrDefault()?.Description,
                    PeriodPreviousYear = periodPreviousYear,
                    ProgressivelyPreviousYear = progressivelyPreviousYear,
                    Level = sixDegree ? 6 : 5
                };
                previousPeriodFinancialResultList.Add(model);
            }

            var previousPeriodFinancialResultListTemp = new List<MonthlyFinancialBulletinModel>();
            foreach (var previousItem in previousPeriodFinancialResultList)
            {
                var currentItem = currentPeriodFinancialResultList.Where(x => x.Id == previousItem.Id).FirstOrDefault();
                if (currentItem != null)
                {
                    currentItem.PeriodPreviousYear = previousItem.PeriodPreviousYear;
                    currentItem.ProgressivelyPreviousYear = previousItem.ProgressivelyPreviousYear;
                }
                else
                {
                    previousPeriodFinancialResultListTemp.Add(previousItem);
                }
            }

            //join two lists
            currentPeriodFinancialResultList = currentPeriodFinancialResultList.Concat(previousPeriodFinancialResultListTemp).ToList();

            List<MonthlyFinancialBulletinModel> CreateLevelList(int startIndex, int length, List<MonthlyFinancialBulletinModel> currentList, int level)
            {
                var resultList = currentPeriodFinancialResultList.Select(x => x.Id.Substring(startIndex, length)).Distinct().ToList();

                var levelList = new List<MonthlyFinancialBulletinModel>();
                foreach (var id in resultList)
                {
                    var list = currentList.Where(x => x.Id.StartsWith(id)).ToList();
                    list.ForEach(x => x.ParentId = id);
                    var model = new MonthlyFinancialBulletinModel
                    {
                        Id = id,
                        Description = codes.Where(x => x.Id == id).FirstOrDefault()?.Description,
                        PeriodCurrentYear = list.Select(x => x.PeriodCurrentYear).Sum(),
                        ProgressivelyCurrentYear = list.Select(x => x.ProgressivelyCurrentYear).Sum(),
                        PeriodPreviousYear = list.Select(x => x.PeriodPreviousYear).Sum(),
                        ProgressivelyPreviousYear = list.Select(x => x.ProgressivelyPreviousYear).Sum(),
                        Level = level
                    };
                    levelList.Add(model);
                }

                return levelList;
            };

            var level5 = sixDegree ? CreateLevelList(0, 18, currentPeriodFinancialResultList, 5) : new List<MonthlyFinancialBulletinModel>();
            var level4 = CreateLevelList(0, 13, sixDegree ? level5 : currentPeriodFinancialResultList, 4);
            var level3 = CreateLevelList(0, 8, level4, 3);
            var level2 = CreateLevelList(0, 5, level3, 2);
            var level1 = CreateLevelList(0, 2, level2, 1);

            currentPeriodFinancialResultList = currentPeriodFinancialResultList.Concat(level5).ToList()
                .Concat(level4).ToList().Concat(level3).ToList().Concat(level2).ToList().Concat(level1).ToList();

            currentPeriodFinancialResultList.ForEach(x => {
                var periodDefference = x.PeriodCurrentYear - x.PeriodPreviousYear;
                x.PeriodDefference = periodDefference;
                x.PeriodRate = periodDefference == 0 ? 0m : x.PeriodPreviousYear == 0 ? 100m : Math.Round(periodDefference / x.PeriodPreviousYear * 100, 2);
                var progressivelyDefference = x.ProgressivelyCurrentYear - x.ProgressivelyPreviousYear;
                x.ProgressivelyDefference = progressivelyDefference;
                x.ProgressivelyRate = progressivelyDefference == 0 ? 0m : x.ProgressivelyPreviousYear == 0 ? 100m : Math.Round(progressivelyDefference / x.ProgressivelyPreviousYear * 100, 2);

                x.DisplayOrder = x.Id.StartsWith("7") ? 10 : x.Id.StartsWith("2") ? 20 : 30;
            });

            var list = currentPeriodFinancialResultList.OrderBy(x => x.DisplayOrder).ThenBy(t => t.Id).ToList();

            //var json = Newtonsoft.Json.JsonConvert.SerializeObject(currentPeriodFinancialResultList.OrderBy(x => x.Id), Newtonsoft.Json.Formatting.Indented);

            //
            var taxFactors = await _taxFactorService.GetAllTaxFactorsAsync();

            //expirationInventory: "Αποθέματα λήξης"
            //var _expirationInventory = results.Where(x => x.Year == previousYear && x.Id.StartsWith("2") && x.Id.Substring(4).StartsWith("1"))
            //    .Select(s => s.Total).Sum();

            //netProfitPeriod: "Καθαρό κέρδος περιόδου"
            var expirationInventory = searchModel.ExpirationInventory;
            var expirationDepreciate = searchModel.ExpirationDepreciate;

            var group7 = level1.Where(x => x.Id.StartsWith("7")).Select(s => s.ProgressivelyCurrentYear).Sum();
            var group2 = level1.Where(x => x.Id.StartsWith("2")).Select(s => s.ProgressivelyCurrentYear).Sum();
            var group6 = level1.Where(x => x.Id.StartsWith("6")).Select(s => s.ProgressivelyCurrentYear).Sum();
            var group8 = level1.Where(x => x.Id.StartsWith("8")).Select(s => s.ProgressivelyCurrentYear).Sum();

            var netProfitPeriod = group7 - group2 - group6 - group8 + expirationInventory - expirationDepreciate;

            //remodelingCosts: "Πλέον αναμορφούμενα έξοδα"
            var remodelingCostsList = await _dataProvider.QueryAsync<MonthlyFinancialBulletinRemodelingCostsQueryModel>(connectionResult.Connection,
                new MonthlyFinancialBulletinRemodelingCostsQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCompanyId, pCurrentYear, pSchema);
            remodelingCostsList.ToList().ForEach(x => x.Result = Math.Round(x.Rate * x.Total / 100, 2));
            var remodelingCosts = remodelingCostsList.Select(x => x.Result).Sum();

            //previousYearDamage: "Μείον μεταφερόμενη ζημία προηγ.χρήσης"
            var pCode = new LinqToDB.Data.DataParameter("pCode", "49.02.00%");
            var previousYearDamageList = await _dataProvider.QueryAsync<MonthlyFinancialBulletinQuery>(connectionResult.Connection,
                new MonthlyFinancialBulletinAccountingCodeDebitQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCode, pCompanyId, pCurrentYear, pPeriod, pSchema);
            var previousYearDamage = previousYearDamageList.Select(x => x.Total).Sum();

            //taxProfitPeriod "Φορολογητέο κέρδος περιόδου"
            var taxProfitPeriod = netProfitPeriod + remodelingCosts - previousYearDamage;

            //taxAdvance: "Φόρος εισοδήματος"
            var factor = taxFactors.Where(x => x.Year == currentYear).Select(s => s.TaxIncome).Sum(); // hack
            var taxIncome = taxProfitPeriod <= 0 ? 0m : Math.Round(taxProfitPeriod * factor / 100, 2);

            //holdingTaxAdvance: "Παρακρατούμενος φόρος"
            pCode = new LinqToDB.Data.DataParameter("pCode", "54.01.02%");
            var holdingTaxAdvanceList = await _dataProvider.QueryAsync<MonthlyFinancialBulletinQuery>(connectionResult.Connection,
                new MonthlyFinancialBulletinAccountingCodeCreditQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCode, pCompanyId, pCurrentYear, pPeriod, pSchema);
            var holdingTaxAdvance = holdingTaxAdvanceList.Select(x => x.Total).Sum() * -1;

            //paymentTaxAdvance: "Προκαταβολή φόρου"
            var tax = taxFactors.Where(x => x.Year == currentYear).Select(s => s.TaxAdvance).Sum(); // hack
            var taxIncomeCalc = Math.Round(taxIncome * tax / 100, 2);
            var taxAdvance = taxIncome <= 0 ? 0m : taxIncomeCalc <= holdingTaxAdvance ? 0m : taxIncomeCalc - holdingTaxAdvance;

            //paymentPreviousYear: "Προκαταβολή προηγ.έτους"
            pCode = new LinqToDB.Data.DataParameter("pCode", "54.01.03%");
            var paymentPreviousYearList = await _dataProvider.QueryAsync<MonthlyFinancialBulletinQuery>(connectionResult.Connection,
                new MonthlyFinancialBulletinAccountingCodeDebitQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(), pCode, pCompanyId, pCurrentYear, pPeriod, pSchema);
            var paymentPreviousYear = paymentPreviousYearList.Select(x => x.Total).Sum();

            //taxesFee: "Τέλος επιτηδεύματος"
            var taxesFee = connectionResult.TaxesFee < 0 ? 0m : connectionResult.TaxesFee;

            //amountPayable: "Πληρωτέο ποσό"
            var amountPayable = taxIncome + taxAdvance - paymentPreviousYear - holdingTaxAdvance + taxesFee;

            //taxReturn: "Επιστροφή"
            var taxReturn = amountPayable < 0 ? amountPayable * -1 : 0m;

            var resultFormModel = new MonthlyFinancialBulletinResultFormModel
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

            return new PrepareMonthlyFinancialBulletinModel
            {
                TreeList = list,
                RemodelingCostsList = remodelingCostsList,
                ResultModel = resultFormModel
            };
        }

    }
}