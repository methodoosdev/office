using App.Core;
using App.Core.Domain.Accounting;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Core.Infrastructure.Dtos.Accounting;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Accounting;
using App.Models.Offices;
using App.Models.Traders;
using App.Services;
using App.Services.Accounting;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Offices;
using App.Services.Traders;
using App.Web.Infra.Factories.Common.Offices;
using App.Web.Infra.Queries.Accounting;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Accounting.Factories
{
    public partial interface IPeriodicF2ModelFactory
    {
        Task<PeriodicF2SearchModel> PreparePeriodicF2SearchModelAsync(PeriodicF2SearchModel searchModel);
        Task<PeriodicF2ListModel> PreparePeriodicF2ListModelAsync(PeriodicF2SearchModel searchModel);
        Task<PeriodicF2Model> PreparePeriodicF2ModelAsync(PeriodicF2Model model, PeriodicF2 periodicF2);
        Task<PeriodicF2Model> PreparePeriodicF2GenerateAsync(PeriodicF2Model model, PeriodicF2DialogModel dialog, string connection);
        Task<PeriodicF2FormModel> PreparePeriodicF2FormModelAsync(PeriodicF2FormModel formModel);
        Task<PeriodicF2DialogFormModel> PreparePeriodicF2DialogFormModelAsync(PeriodicF2DialogFormModel formModel);
        int[] TaxPeriodByCategoryBookType(int categoryBookTypeId, int period);
        void PrepareCalcPeriodicF2ModelAsync(PeriodicF2Model model);
        Task<PageCredentialResult> GetPageCredentialAsync(TraderModel trader, bool representative);
    }
    public partial class PeriodicF2ModelFactory : IPeriodicF2ModelFactory
    {
        private readonly ITraderService _traderService;
        private readonly IPeriodicF2Service _periodicF2Service;
        private readonly ILocalizationService _localizationService;
        private readonly ITraderEmployeeMappingService _traderEmployeeMappingService;
        private readonly ITraderLookupModelFactory _traderLookupModelFactory;
        private readonly IAccountingOfficeService _accountingOfficeService;
        private readonly IAppDataProvider _dataProvider;
        private readonly IPersistStateService _persistStateService;
        private readonly IModelFactoryService _modelFactoryService;

        public PeriodicF2ModelFactory(
            ITraderService traderService,
            IPeriodicF2Service periodicF2Service,
            ILocalizationService localizationService,
            ITraderEmployeeMappingService traderEmployeeMappingService,
            ITraderLookupModelFactory traderLookupModelFactory,
            IAccountingOfficeService accountingOfficeService,
            IAppDataProvider dataProvider,
            IPersistStateService persistStateService,
            IModelFactoryService modelFactoryService
            )
        {
            _traderService = traderService;
            _periodicF2Service = periodicF2Service;
            _localizationService = localizationService;
            _traderEmployeeMappingService = traderEmployeeMappingService;
            _traderLookupModelFactory = traderLookupModelFactory;
            _accountingOfficeService = accountingOfficeService;
            _dataProvider = dataProvider;
            _persistStateService = persistStateService;
            _modelFactoryService = modelFactoryService;
        }

        private async Task<IPagedList<PeriodicF2Model>> GetPagedListAsync(PeriodicF2SearchModel searchModel)
        {
            var submitModeTypes = await SubmitModeType.Submited.ToSelectionItemListAsync();
            var f523Types = await F523Type.Yes.ToSelectionItemListAsync();
            var f507Types = await F507Type.Exemption.ToSelectionItemListAsync();

            var traderIds = (await _periodicF2Service.GetAllPeriodicF2Async())
                .Select( x => x.TraderId ).Distinct().ToArray();

            var traders = await _traderService.GetTradersByIdsAsync(traderIds);
            var categoryBookTraders = traders
                .Select(x => new TraderCategoryBookModel { TraderId = x.Id, CategoryBookTypeId = x.CategoryBookTypeId }).ToList();

            var query = _periodicF2Service.Table.SelectAwait(async x =>
            {
                var categoryBookTrader = categoryBookTraders.First(k => x.TraderId == k.TraderId);
                var b_CategoryBook = IsCategoryBookB(categoryBookTrader.CategoryBookTypeId);
                var employees = await _traderEmployeeMappingService.GetEmployeesByTraderIdAsync(x.TraderId);
                var employeeName = string.Join(", ", employees.Where(x => x.DepartmentId == 2).Select(s => s.FullName()).ToArray());

                var model = x.ToModel<PeriodicF2Model>();
                model.TaxPeriodName = $"{x.F006}{(b_CategoryBook ? "ο Τρίμηνο" : "ος Μήνας")} {x.F004}";
                model.SubmitModeTypeIdName = submitModeTypes.Where(x => x.Value == model.SubmitModeTypeId).FirstOrDefault()?.Label ?? "";
                model.F523TypeIdName = f523Types.Where(x => x.Value == model.F523).FirstOrDefault()?.Label ?? "";
                model.F007Name = x.F007 ? "Τροποποιητική" : "Αρχική";
                model.EmployeeName = employeeName;

                return model;
            });

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.F101.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.EmployeeName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<PeriodicF2SearchModel> PreparePeriodicF2SearchModelAsync(PeriodicF2SearchModel searchModel)
        {
            var persistState = await _persistStateService.GetModelInstance<PeriodicF2SearchModel>();

            if(persistState.Exist)
                return persistState.Model;

            //prepare page parameters
            searchModel.SetGridPageSize(); 
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.PeriodicF2Model.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<PeriodicF2ListModel> PreparePeriodicF2ListModelAsync(PeriodicF2SearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get customer roles
            var periodicF2s = await GetPagedListAsync(searchModel);

            //prepare grid model
            var model = new PeriodicF2ListModel().PrepareToGrid(searchModel, periodicF2s);

            return model;
        }

        private bool IsCategoryBookB(int categoryBookTypeId) 
        {
            return categoryBookTypeId == (int)CategoryBookType.B;
        }

        public virtual async Task<PeriodicF2Model> PreparePeriodicF2ModelAsync(PeriodicF2Model model, PeriodicF2 periodicF2)
        {
            if (periodicF2 != null) // Edit
            {
                //fill in model values from the entity
                model ??= periodicF2.ToModel<PeriodicF2Model>();

                var trader = await _traderService.GetTraderByIdAsync(model.TraderId);
                var b_CategoryBook = IsCategoryBookB(trader.CategoryBookTypeId);

                model.TaxPeriodName = $"{model.F006}{(b_CategoryBook ? "ο Τρίμηνο" : "ος Μήνας")} {model.F004}";
                model.F002Value = model.F002.ToString("dd/MM/yyyy");
                model.F005aValue = model.F005a.ToString("dd/MM/yyyy");
                model.F005bValue = model.F005b.ToString("dd/MM/yyyy");
            }

            if (periodicF2 == null) // Create
            {
                model.SubmitModeTypeId = (int)SubmitModeType.Pending;
                model.F523 = 1;
            }

            return model;
        }

        public virtual async Task<PeriodicF2Model> PreparePeriodicF2GenerateAsync(PeriodicF2Model model, PeriodicF2DialogModel dialog, string connection)
        {
            //var customer = await _workContext.GetCurrentCustomerAsync();
            var _trader = await _traderService.GetTraderByIdAsync(dialog.TraderId);
            var trader = _trader.ToModel<TraderModel>();
            var b_CategoryBook = IsCategoryBookB(trader.CategoryBookTypeId);
            var taxPeriod = TaxPeriodByCategoryBookType(trader.CategoryBookTypeId, dialog.Period);
            var year = dialog.Date.Year;

            model.TraderId = trader.Id;
            model.SubmitModeTypeId = (int)SubmitModeType.Pending;
            model.F001b = year;
            model.F002 = DateTime.UtcNow;
            model.F004 = year;
            model.F005a = new DateTime(year, taxPeriod.First(), 1).Date;
            model.F005b = new DateTime(year, taxPeriod.Last(), 1).AddMonths(1).AddDays(-1).Date;
            model.F006 = dialog.Period;
            model.F007 = dialog.F007;
            model.F101 = trader.FullName();
            model.F104 = trader.Vat;

            model.TaxPeriodName = $"{model.F006}{(b_CategoryBook ? "ο Τρίμηνο" : "ος Μήνας")} {model.F004}";
            model.F002Value = model.F002.ToString("dd/MM/yyyy");
            model.F005aValue = model.F005a.ToString("dd/MM/yyyy");
            model.F005bValue = model.F005b.ToString("dd/MM/yyyy");

            var queryOrigin = new PeriodicF2Query(trader.LogistikiProgramTypeId, trader.CategoryBookTypeId).Get();
            var queryEightGroup = new PeriodicF2EightGroupQuery(trader.LogistikiProgramTypeId, trader.CategoryBookTypeId).Get();

            var pCompanyId = new DataParameter("pCompanyId", trader.CompanyId);
            var pSchema = new DataParameter("pSchema", trader.AccountingSchema);
            var pYear = new DataParameter("pYear", year);
            var parameters = new DataParameter[] { pCompanyId, pSchema, pYear };

            try
            {
                await PeriodF2Values(F2CodeAccountResources.F2DebitDict, 1, model, queryOrigin, parameters, taxPeriod, connection);
                await PeriodF2Values(F2CodeAccountResources.F2F337CalcDict, 1, model, queryEightGroup, parameters, taxPeriod, connection);
                await PeriodF2Values(F2CodeAccountResources.F2CreditDict, b_CategoryBook ? 1 : (-1), model, queryOrigin, parameters, taxPeriod, connection);
                await PeriodF2Values(F2CodeAccountResources.F2EightGroup, b_CategoryBook ? 1 : (-1), model, queryEightGroup, parameters, taxPeriod, connection);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }

            //calc F401
            var previousF2 = GetPreviousF2(b_CategoryBook, year, model.F006);
            var f2All = await _periodicF2Service.GetAllPeriodicF2Async(model.TraderId);

            model.F502Previous = f2All
                .Where(x => x.F004 == previousF2.Year && x.F006 == previousF2.Period)
                .OrderBy(x => x.F002).LastOrDefault()?.F502 ?? 0;

            model.F404 = f2All // 404 not used - create migration for F483
                .Where(x => x.F004 == previousF2.Year && x.F006 == previousF2.Period)
                .OrderBy(x => x.F002).LastOrDefault()?.F483 ?? 0;

            //calc F403:F505
            if (model.F007)
            {
                model.F511Previous = f2All
                    .Where(x => x.F004 == model.F004 && x.F006 == model.F006)
                    .OrderBy(x => x.F002).LastOrDefault()?.F511 ?? 0;

                model.F503Previous = f2All
                    .Where(x => x.F004 == model.F004 && x.F006 == model.F006)
                    .OrderBy(x => x.F002).LastOrDefault()?.F503 ?? 0;
            }

            return model;
        }

        private (int Year, int Period) GetPreviousF2(bool b_CategoryBook, int year, int period)
        {
            var _period = period - 1;
            if (_period == 0)
                return (year - 1, b_CategoryBook ? 4 : 12);
            else
                return (year, _period);
        }

        private async Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var submit = await _localizationService.GetResourceAsync("App.Common.Submit");
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<PeriodicF2Model>(1, nameof(PeriodicF2Model.F101), ColumnType.RouterLink),
                ColumnConfig.Create<PeriodicF2Model>(2, nameof(PeriodicF2Model.TaxPeriodName)),
                ColumnConfig.Create<PeriodicF2Model>(2, nameof(PeriodicF2Model.F007Name)),
                ColumnConfig.Create<PeriodicF2Model>(2, nameof(PeriodicF2Model.SubmitModeTypeIdName)),
                ColumnConfig.Create<PeriodicF2Model>(2, nameof(PeriodicF2Model.F004), style: centerAlign),
                ColumnConfig.Create<PeriodicF2Model>(2, nameof(PeriodicF2Model.F002), ColumnType.Date, style: centerAlign),
                ColumnConfig.Create<PeriodicF2Model>(2, nameof(PeriodicF2Model.F005a), ColumnType.Date, style: centerAlign, hidden: true),
                ColumnConfig.Create<PeriodicF2Model>(2, nameof(PeriodicF2Model.F005b), ColumnType.Date, style: centerAlign, hidden: true),
                ColumnConfig.Create<PeriodicF2Model>(2, nameof(PeriodicF2Model.RegistrationNumber)),
                ColumnConfig.Create<PeriodicF2Model>(2, nameof(PeriodicF2Model.F502), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<PeriodicF2Model>(2, nameof(PeriodicF2Model.F503), ColumnType.Decimal, style : rightAlign),
                ColumnConfig.Create<PeriodicF2Model>(2, nameof(PeriodicF2Model.F511), ColumnType.Decimal, style : rightAlign),
                ColumnConfig.Create<PeriodicF2Model>(2, nameof(PeriodicF2Model.EmployeeName)),
                //ColumnConfig.CreateButton<PeriodicF2Model>(9, ColumnType.RowButton, "submit", "info", submit)
            };
            return columns;
        }

        public virtual async Task<PeriodicF2FormModel> PreparePeriodicF2FormModelAsync(PeriodicF2FormModel formModel)
        {
            //var employees = await _baseModelFactory.GetAllEmployeesAsync();

            //var submitTypes = await SubmitType.Initial.ToSelectionItemListAsync();
            var submitModeTypes = await SubmitModeType.Submited.ToSelectionItemListAsync();
            //var taxPeriodTypes = await TaxPeriodType.Month.ToSelectionItemListAsync();
            var f523Types = await F523Type.Yes.ToSelectionItemListAsync();
            var f507Types = await F507Type.Exemption.ToSelectionItemListAsync();

            var fields = new List<Dictionary<string, object>>()
            {
                //FieldConfig.Create<PeriodicF2Model>(nameof(PeriodicF2Model.SubmitTypeId), FieldType.Select, options: submitTypes, className: "field f301"),
                
                //FieldConfig.Create<PeriodicF2Model>(nameof(PeriodicF2Model.F001b), FieldType.Text, className: "field f001b"),
                //FieldConfig.Create<PeriodicF2Model>(nameof(PeriodicF2Model.F002Value), FieldType.Text, className: "field f002"),
                //FieldConfig.Create<PeriodicF2Model>(nameof(PeriodicF2Model.F004), FieldType.Text, className: "field f004"),
                //FieldConfig.Create<PeriodicF2Model>(nameof(PeriodicF2Model.F005aValue), FieldType.DateTime, className: "field f005a"),
                //FieldConfig.Create<PeriodicF2Model>(nameof(PeriodicF2Model.F005bValue), FieldType.DateTime, className: "field f005b"),
                //FieldConfig.Create<PeriodicF2Model>(nameof(PeriodicF2Model.F007), FieldType.Checkbox, className: "f007"),
                //FieldConfig.Create<PeriodicF2Model>(nameof(PeriodicF2Model.F101), FieldType.Text, className: "field f101"),
                //FieldConfig.Create<PeriodicF2Model>(nameof(PeriodicF2Model.F104), FieldType.Text, className: "field f104"),
                //FieldConfig.Create<PeriodicF2Model>(nameof(PeriodicF2Model.TaxPeriodName), FieldType.Text, className: "field taxPeriodName"),
                //FieldConfig.Create<PeriodicF2Model>(nameof(PeriodicF2Model.F301), FieldType.Decimals, className: "field f301"),
                //FieldConfig.Create<PeriodicF2Model>(nameof(PeriodicF2Model.F302), FieldType.Decimals, className: "field f302"),
                //FieldConfig.Create<PeriodicF2Model>(nameof(PeriodicF2Model.F303), FieldType.Decimals, className: "field f303"),
                //FieldConfig.Create<PeriodicF2Model>(nameof(PeriodicF2Model.F304), FieldType.Decimals, className: "field f304"),
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.PeriodicF2Model.Title"));
            formModel.CustomProperties.Add("fields", fields);

            return formModel;
        }

        public virtual async Task<PeriodicF2DialogFormModel> PreparePeriodicF2DialogFormModelAsync(PeriodicF2DialogFormModel formModel)
        {
            var selectCaption = await _localizationService.GetResourceAsync("App.Common.Choice");

            var traderList = await _traderLookupModelFactory.GetAllTraderLookupItemsAsync();
            traderList.Where(x => x.CategoryBookTypeId == 2 && x.CategoryBookTypeId == 3).ToList();

            var traders = new List<SelectionItemList>();
            foreach (var item in traderList)
                traders.Add(new SelectionItemList(item.Id, item.FullName, item.CategoryBookTypeId == 2));

            var periodsC = await _modelFactoryService.GetSelectionItemListAsync(DateLocaleResources.LocaleMonthResourceDict);
            var periodsB = await _modelFactoryService.GetSelectionItemListAsync(DateLocaleResources.LocalePeriodResourceDict);

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<PeriodicF2DialogModel>(nameof(PeriodicF2DialogModel.TraderId), FieldType.GridSelect, options: traders, placeholder: selectCaption, hideExpression: "formState.status === 'representative'"),
                FieldConfig.Create<PeriodicF2DialogModel>(nameof(PeriodicF2DialogModel.Date), FieldType.YearDate, hideExpression: "formState.status === 'representative'"),
                FieldConfig.Create<PeriodicF2DialogModel>(nameof(PeriodicF2DialogModel.Period), FieldType.Select, defaultValue: 0, hideExpression: "formState.status === 'representative'"),
                FieldConfig.Create<PeriodicF2DialogModel>(nameof(PeriodicF2DialogModel.F007), FieldType.Checkbox, hideExpression: "formState.status === 'retrieve' || formState.status === 'representative'"),
                FieldConfig.Create<PeriodicF2DialogModel>(nameof(PeriodicF2DialogModel.Representative), FieldType.Checkbox),
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.PeriodicF2DialogModel.Title"));
            formModel.CustomProperties.Add("fields", fields);
            formModel.CustomProperties.Add("periodsC", periodsC);
            formModel.CustomProperties.Add("periodsB", periodsB);

            return formModel;
        }

        public int[] TaxPeriodByCategoryBookType(int categoryBookTypeId, int period)
        {
            var b_CategoryBook = IsCategoryBookB(categoryBookTypeId);
            if (b_CategoryBook)
            {
                var periods = new Dictionary<int, int[]>
                {
                    [1] = new int[] { 1, 2, 3 },
                    [2] = new int[] { 4, 5, 6 },
                    [3] = new int[] { 7, 8, 9 },
                    [4] = new int[] { 10, 11, 12 }
                };
                if (periods.TryGetValue(period, out int[] value))
                    return (value);
                else
                    throw new Exception("Error period not exist.");
            }
            else
                return new int[] { period, period };

        }

        protected string CreateLikeExpressionValues(string column, string[] values)
        {
            string[] list = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
                list[i] = $"{column} LIKE '{values[i]}'";

            var text = string.Join(" OR ", list);

            return text;
        }

        protected string CreateLikeExpression(string column, string param, int length)
        {
            //var paramName = "pCode";
            //var _params = CreateParams(paramName, item.Value);
            //_params.AddRange(new[] { pCompanyId, pSchema, pYear });
            //var expression = CreateLikeExpression("M.CODE", paramName, item.Value.Length);
            //var query = string.Format(queryOrigin, expression, string.Join(",", taxPeriod));
            //var value = (await _externalDataProvider.QueryAsync<decimal?>(connection, query, _params.ToArray())).FirstOrDefault();

            string[] list = new string[length];
            for (int i = 0; i < length; i++)
                list[i] = $"{column} LIKE @{param}{i}"; //column LIKE 'Text%' OR 

            return string.Join(" OR ", list);
        }

        protected List<DataParameter> CreateParams(string param, string[] values)
        {
            var list = new List<DataParameter>();
            for (int i = 0; i < values.Length; i++)
                list.Add(new DataParameter($"@{param}{i}", values[i]));

            return list;
        }

        protected async Task PeriodF2Values(Dictionary<string, string[]> dict, int sign, PeriodicF2Model model, string queryFormat, DataParameter[] parameters, int[] taxPeriod, string connection, string exlude = null)
        {
            var modelType = model.GetType();

            foreach (var item in dict)
            {
                var prop = modelType.GetProperty(item.Key);
                if (prop != null)
                {
                    var expression = CreateLikeExpressionValues("M.CODE", item.Value);
                    var query = string.Format(queryFormat, expression, string.Join(",", taxPeriod));
                    var value = (await _dataProvider.QueryAsync<decimal?>(connection, query, parameters)).FirstOrDefault() ?? 0m;
                    prop.SetValue(model, value * sign, null);

                    Console.WriteLine($"{item.Key}: {value} -> {query}");
                }
            }
        }

        protected async Task _PeriodF2Values(Dictionary<int, string[]> dict, PeriodicF2Model model, string queryFormat, DataParameter[] parameters, int[] taxPeriod, string connection)
        {
            var modelType = model.GetType();
            var paramName = "pCode";

            foreach (var item in dict)
            {
                var prop = modelType.GetProperty($"F{item.Key}");
                if (prop != null)
                {
                    var _params = CreateParams(paramName, item.Value);
                    _params.AddRange(parameters);
                    var expression = CreateLikeExpression("M.CODE", paramName, item.Value.Length);
                    var query = string.Format(queryFormat, expression, string.Join(",", taxPeriod));
                    var value = (await _dataProvider.QueryAsync<decimal?>(connection, query, _params.ToArray())).FirstOrDefault();
                    if (value != null)
                        prop.SetValue(model, value, null);
                }
            }
        }

        public void PrepareCalcPeriodicF2ModelAsync(PeriodicF2Model model)
        {
            // :F307
            model.F307 = model.F301 + model.F302 + model.F303 +
                model.F304 + model.F305 + model.F306;
            // :F311
            model.F311 = model.F307 + model.F342 + model.F345 +
                model.F348 + model.F349 + model.F310;
            // :F312
            model.F312 = model.F311 - model.F364 - model.F365 -
                model.F366 - model.F5402Calc; // - Πωλήσεις παγίων - Επιδοτήσεις - Αυτοπαραδόσεις - Χρημ.Πράξεις
            // :F331
            model.F331 = Math.Round(model.F301 * 13 / 100, 2, MidpointRounding.AwayFromZero);//
            // :F332
            model.F332 = Math.Round(model.F302 * 6 / 100, 2, MidpointRounding.AwayFromZero);
            // :F333
            model.F333 = Math.Round(model.F303 * 24 / 100, 2, MidpointRounding.AwayFromZero);
            // :F334
            model.F334 = Math.Round(model.F304 * 9 / 100, 2, MidpointRounding.AwayFromZero);
            // :F335
            model.F335 = Math.Round(model.F305 * 4 / 100, 2, MidpointRounding.AwayFromZero);
            // :F336
            model.F336 = Math.Round(model.F306 * 17 / 100, 2, MidpointRounding.AwayFromZero);

            // :F337
            model.F337 = model.F331 + model.F332 + model.F333 +
                model.F334 + model.F335 + model.F336;
            // :F367
            model.F367 = model.F361 + model.F362 + model.F363 +
                model.F364 + model.F365 + model.F366;
            // :F387
            model.F387 = model.F381 + model.F382 + model.F383 +
                model.F384 + model.F385 + model.F386;

            // :F402 :F422
            var diff = model.F337Calc - model.F337;
            if (diff > 0)
                model.F422 = diff;
            else if (diff < 0)
                model.F402 = Math.Abs(diff);
            // :F410
            model.F410 = model.F400 + model.F402 + model.F407;
            // :F428
            model.F428 = model.F411 + model.F422 + model.F423;
            // :F430
            model.F430 = model.F387 + model.F410 - model.F428;
            // 404 not used - create migration for F483
            model.F483 = model.F404;
            model.F404 = 0;

            // :F470 :F480
            var total = model.F337 - model.F430;
            if (total > 0)
            {
                model.F480 = Math.Abs(total);
                var partialTotal = model.F480 - model.F502Previous - model.F511Previous + model.F483;
                if (partialTotal < 0)
                    model.F502 = Math.Abs(partialTotal);
                else if (partialTotal > 0)
                    model.F511 = partialTotal;
            }
            else if (total < 0)
            {
                model.F470 = Math.Abs(total);
                var partialTotal = model.F470 + model.F502Previous + model.F511Previous - model.F483;
                if (partialTotal < 0)
                    model.F511 = Math.Abs(partialTotal);
                else if (partialTotal > 0)
                    model.F502 = partialTotal;
            }
            else
            { // total == 0
                
                //var partialTotal = model.F502Previous + model.F511Previous - model.F483;
                //if (partialTotal < 0)
                //    model.F502 = Math.Abs(partialTotal);
                //else if (partialTotal > 0) { 
                //    model.F511 = partialTotal;

                var partialTotal = model.F502Previous + model.F511Previous - model.F483;
                if (partialTotal < 0)
                    model.F502 = Math.Abs(partialTotal);
                else if (partialTotal > 0)
                {
                    if (model.F502Previous > 0)
                        model.F502 = partialTotal;

                    if (model.F511Previous > 0)
                        model.F511 = partialTotal;
                }
            }
            // :F401
            model.F401 = model.F502Previous; // f007 Τροποποιητική
            // :F403 & :F505 Μόνο τροποποιητική
            if (model.F007 == true)
            {
                model.F403 = model.F511Previous;
                model.F505 = model.F503Previous;
            }
        }

        public virtual async Task<PageCredentialResult> GetPageCredentialAsync(TraderModel trader, bool representative)
        {
            var result = new PageCredentialResult();

            if (trader.CustomerTypeId == (int)CustomerType.IndividualCompany)
            {
                result.UserName = trader.TaxisUserName.Trim();
                result.Password = trader.TaxisPassword.Trim();
                result.PageCredentialTypeId = (int)PageCredentialType.IndividualCompany;
            }
            else
            {
                if (representative)
                {
                    result.UserName = trader.RepresentativeUserName.Trim();
                    result.Password = trader.RepresentativePassword.Trim();
                    result.PageCredentialTypeId = (int)PageCredentialType.Representative;
                }
                else
                {
                    var office = await _accountingOfficeService.GetAccountingOfficeModelAsync();
                    result.UserName = office.TaxisNetUserName.Trim();
                    result.Password = office.TaxisNetPassword.Trim();
                    result.PageCredentialTypeId = (int)PageCredentialType.Accountant;
                }
            }

            return result;
        }
    }
}