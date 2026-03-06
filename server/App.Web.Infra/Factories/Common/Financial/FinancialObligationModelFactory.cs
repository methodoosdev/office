using App.Core;
using App.Core.Domain.Common;
using App.Core.Domain.Customers;
using App.Core.Domain.Financial;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Core.Infrastructure.Dtos.Financial;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Financial;
using App.Models.Traders;
using App.Services;
using App.Services.Customers;
using App.Services.Financial;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Offices;
using App.Services.Payroll;
using App.Services.Traders;
using App.Web.Infra.Queries.Common.Financial;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Financial
{
    public partial interface IFinancialObligationModelFactory
    {
        Task<FinancialObligationSearchModel> PrepareFinancialObligationSearchModelAsync(FinancialObligationSearchModel searchModel);
        Task<FinancialObligationListModel> PrepareFinancialObligationListModelAsync(FinancialObligationSearchModel searchModel);
        Task<FinancialObligationModel> PrepareFinancialObligationModelAsync(FinancialObligationModel model, FinancialObligation financialObligation);
        Task<FinancialObligationFormModel> PrepareFinancialObligationFormModelAsync(FinancialObligationFormModel formModel, bool editMode);
        Task<List<FinancialObligationDto>> EfkaTekaResult(Trader trader, string connection);
        Task<FinancialObligationRequestModel> PrepareFinancialObligationRequestModelAsync(FinancialObligationRequestModel requestModel);
        Task<FinancialObligationRequestFormModel> PrepareFinancialObligationRequestFormModelAsync(FinancialObligationRequestFormModel requestFormModel);
        Task<FinancialObligationFilterFormModel> PrepareFinancialObligationFilterFormModelAsync(FinancialObligationFilterFormModel filterFormModel);

    }
    public partial class FinancialObligationModelFactory : IFinancialObligationModelFactory
    {
        private readonly IApdTekaService _apdTekaService;
        private readonly IFinancialObligationService _financialObligationService;
        private readonly ILocalizationService _localizationService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ITraderService _traderService;
        private readonly ICustomerService _customerService;
        private readonly IPersistStateService _persistStateService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly ITraderKadService _traderKadService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;

        public FinancialObligationModelFactory(
            IApdTekaService apdTekaService,
            IFinancialObligationService financialObligationService,
            ILocalizationService localizationService,
            IAppDataProvider dataProvider,
            ITraderService traderService,
            ICustomerService customerService,
            IPersistStateService persistStateService,
            IModelFactoryService modelFactoryService,
            ITraderKadService traderKadService,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext)
        {
            _apdTekaService = apdTekaService;
            _financialObligationService = financialObligationService;
            _localizationService = localizationService;
            _dataProvider = dataProvider;
            _traderService = traderService;
            _customerService = customerService;
            _persistStateService = persistStateService;
            _modelFactoryService = modelFactoryService;
            _traderKadService = traderKadService;
            _dateTimeHelper = dateTimeHelper;
            _workContext = workContext;
        }

        private async Task<IPagedList<FinancialObligationModel>> GetPagedListAsync(FinancialObligationSearchModel searchModel, FinancialObligationFilterModel filterModel, bool filterExist)
        {
            var customerTypes = await CustomerType.Other.ToSelectionItemListAsync();
            var legalFormTypes = await LegalFormType.None.ToSelectionItemListAsync();
            var categoryBookTypes = await CategoryBookType.None.ToSelectionItemListAsync();
            var months = await _modelFactoryService.GetSelectionItemListAsync(DateLocaleResources.LocaleMonthResourceDict, false);

            var customers = await _customerService.GetAllCustomersAsync();
            var traderList = await _traderService.GetAllTradersAsync(false, true);
            var traders = traderList.Select(x => new TraderHelperResult
            {
                TraderId = x.Id,
                FullName = x.ToTraderFullName(),
                CustomerTypeId = x.CustomerTypeId,
                LegalFormTypeId = x.LegalFormTypeId,
                CategoryBookTypeId = x.CategoryBookTypeId
            }).ToList();

            var query = _financialObligationService.Table.AsEnumerable()
                .Select(x =>
                {
                    var customer = customers.FirstOrDefault(k => k.Id == x.CustomerId);
                    var trader = traders.FirstOrDefault(k => k.TraderId == x.TraderId);

                    var model = x.ToModel<FinancialObligationModel>();
                    var date = _dateTimeHelper.ConvertToUserTimeAsync(x.CreatedOnUtc, DateTimeKind.Utc).Result;

                    model.TraderName = trader.FullName ?? string.Empty;

                    model.CustomerTypeId = customerTypes.FirstOrDefault(a => a.Value == trader.CustomerTypeId)?.Value ?? -1;
                    model.CustomerTypeName = customerTypes.FirstOrDefault(a => a.Value == trader.CustomerTypeId)?.Label ?? "";

                    model.LegalFormTypeId = legalFormTypes.FirstOrDefault(a => a.Value == trader.LegalFormTypeId)?.Value ?? -1;
                    model.LegalFormTypeName = legalFormTypes.FirstOrDefault(a => a.Value == trader.LegalFormTypeId)?.Label ?? "";

                    model.CategoryBookTypeId = categoryBookTypes.FirstOrDefault(a => a.Value == trader.CategoryBookTypeId)?.Value ?? -1;
                    model.CategoryBookTypeName = categoryBookTypes.FirstOrDefault(a => a.Value == trader.CategoryBookTypeId)?.Label ?? "";

                    model.CustomerName = customer?.FullName() ?? string.Empty;
                    model.PeriodName = $"{months.FirstOrDefault(k => k.Value == x.Period)?.Label ?? "Σφάλμα"} {date.Year}";
                    model.CreatedOn = date;

                    return model;
                }).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.TraderName.ContainsIgnoreCase(searchModel.QuickSearch));
            }


            if (filterExist)
            {
                if (filterModel.TraderIds.Any())
                    query = query.Where(x => filterModel.TraderIds.Contains(x.TraderId));

                if (!string.IsNullOrEmpty(filterModel.Institution))
                {
                    query = query.Where(c =>
                        c.Institution.ContainsIgnoreCase(filterModel.Institution));
                }

                if (!string.IsNullOrEmpty(filterModel.PaymentType))
                {
                    query = query.Where(c =>
                        c.PaymentType.ContainsIgnoreCase(filterModel.PaymentType));
                }

                //if (filterModel.PaymentDate.HasValue)
                //    query = query.Where(x =>
                //        x.PaymentDate.Year == filterModel.PaymentDate.Value.Year &&
                //        x.PaymentDate.Month == filterModel.PaymentDate.Value.Month);

                if (filterModel.PeriodDate.HasValue)
                {
                    query = query.Where(x =>
                        filterModel.PeriodDate.Value.Year == x.CreatedOn.Year &&
                        filterModel.PeriodDate.Value.Month == x.Period);
                }

                if (filterModel.CustomerId > 0)
                    query = query.Where(c => c.CustomerId == filterModel.CustomerId);

                if (filterModel.IsSent > 0)
                    query = query.Where(c => c.IsSent == (filterModel.IsSent == 1));

                if (filterModel.CustomerTypeId.HasValue && filterModel.CustomerTypeId.Value > -1)
                    query = query.Where(c => c.CustomerTypeId == filterModel.CustomerTypeId.Value);

                if (filterModel.LegalFormTypeId.HasValue && filterModel.LegalFormTypeId.Value > -1)
                    query = query.Where(c => c.LegalFormTypeId == filterModel.LegalFormTypeId.Value);

                if (filterModel.CategoryBookTypeId.HasValue && filterModel.CategoryBookTypeId.Value > -1)
                    query = query.Where(c => c.CategoryBookTypeId == filterModel.CategoryBookTypeId.Value);
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<FinancialObligationSearchModel> PrepareFinancialObligationSearchModelAsync(FinancialObligationSearchModel searchModel)
        {
            var persistState = await _persistStateService.GetModelInstance<FinancialObligationSearchModel>();

            if (persistState.Exist)
                return persistState.Model;

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.FinancialObligationModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<FinancialObligationListModel> PrepareFinancialObligationListModelAsync(FinancialObligationSearchModel searchModel)
        {
            var filterState = await _persistStateService.GetModelInstance<FinancialObligationFilterModel>();

            //get customer roles
            var financialObligations = await GetPagedListAsync(searchModel, filterState.Model, filterState.Exist);

            //prepare grid model
            var model = new FinancialObligationListModel().PrepareToGrid(searchModel, financialObligations);
            model.FilterExist = filterState.Exist;

            return model;
        }

        public virtual Task<FinancialObligationModel> PrepareFinancialObligationModelAsync(FinancialObligationModel model, FinancialObligation financialObligation)
        {
            if (financialObligation != null)
            {
                //fill in model values from the entity
                model ??= financialObligation.ToModel<FinancialObligationModel>();
            }

            if (financialObligation == null)
            {
                //fill in model values from the entity
                model.PaymentDate = DateTime.UtcNow;
                model.Period = 1;
                model.CreatedOnUtc = DateTime.UtcNow;
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<FinancialObligationModel>(1, nameof(FinancialObligationModel.TraderName), ColumnType.RouterLink, width: 260),
                ColumnConfig.Create<FinancialObligationModel>(2, nameof(FinancialObligationModel.Institution)),
                ColumnConfig.Create<FinancialObligationModel>(3, nameof(FinancialObligationModel.PaymentType)),
                ColumnConfig.Create<FinancialObligationModel>(4, nameof(FinancialObligationModel.PaymentValue), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<FinancialObligationModel>(5, nameof(FinancialObligationModel.PaymentIdentity)),
                ColumnConfig.Create<FinancialObligationModel>(6, nameof(FinancialObligationModel.PaymentDate), ColumnType.DateTime),
                ColumnConfig.Create<FinancialObligationModel>(6, nameof(FinancialObligationModel.CustomerName)),
                ColumnConfig.Create<FinancialObligationModel>(6, nameof(FinancialObligationModel.PeriodName)),
                ColumnConfig.Create<FinancialObligationModel>(7, nameof(FinancialObligationModel.IsSent), ColumnType.Checkbox),
                ColumnConfig.Create<FinancialObligationModel>(4, nameof(FinancialObligationModel.CreatedOn), ColumnType.DateTime),
                ColumnConfig.Create<FinancialObligationModel>(8, nameof(FinancialObligationModel.CustomerTypeName), hidden: true),
                ColumnConfig.Create<FinancialObligationModel>(8, nameof(FinancialObligationModel.LegalFormTypeName), hidden: true),
                ColumnConfig.Create<FinancialObligationModel>(8, nameof(FinancialObligationModel.CategoryBookTypeName), hidden: true)
            };

            return columns;
        }

        public virtual async Task<FinancialObligationFormModel> PrepareFinancialObligationFormModelAsync(FinancialObligationFormModel formModel, bool editMode)
        {
            var role = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.EmployeesRoleName);
            var customerList = _customerService.GetAllCustomersByRoles(new int[] { role.Id })
                .Where(x => x.Active).ToList();
            var customers = customerList.Select(x => new SelectionItemList { Value = x.Id, Label = x.FullName() })
                .OrderBy(o => o.Label).ToList();

            var traders = await _modelFactoryService.GetAllTradersAsync(false);
            var months = await _modelFactoryService.GetSelectionItemListAsync(DateLocaleResources.LocaleMonthResourceDict, false);

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<FinancialObligationModel>(nameof(FinancialObligationModel.TraderId), FieldType.Select, options: traders, _readonly: editMode),
                FieldConfig.Create<FinancialObligationModel>(nameof(FinancialObligationModel.Institution), FieldType.Text),
                FieldConfig.Create<FinancialObligationModel>(nameof(FinancialObligationModel.PaymentType), FieldType.Text),
                FieldConfig.Create<FinancialObligationModel>(nameof(FinancialObligationModel.PaymentValue), FieldType.Decimals),
                FieldConfig.Create<FinancialObligationModel>(nameof(FinancialObligationModel.PaymentIdentity), FieldType.Text),
                FieldConfig.Create<FinancialObligationModel>(nameof(FinancialObligationModel.CustomerId), FieldType.GridSelect, options: customers),
                FieldConfig.Create<FinancialObligationModel>(nameof(FinancialObligationModel.Period), FieldType.Select, options: months),
                FieldConfig.Create<FinancialObligationModel>(nameof(FinancialObligationModel.PaymentDate), FieldType.Date),
                FieldConfig.Create<FinancialObligationModel>(nameof(FinancialObligationModel.IsSent), FieldType.Switch),
                FieldConfig.Create<FinancialObligationModel>(nameof(FinancialObligationModel.CreatedOnUtc), FieldType.Date)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.FinancialObligationModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields));

            return formModel;
        }

        public virtual async Task<List<FinancialObligationDto>> EfkaTekaResult(Trader trader, string connection)
        {
            var date = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var checkedDate = lastDayOfMonth.CheckPaymentDate();

            //Έλεγχος αν η επιχείρηση είναι φραμακείο
            var kads = await _traderKadService.GetAllTraderKadsAsync(trader.Id);
            var isPharmacy = kads.Any(k => k.Code.StartsWith("4773") && k.Activity);

            var desiredDate = date.AddMonths(-1);
            var desiredMonth = desiredDate.Month;

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", trader.HyperPayrollId);
            var pYear = new LinqToDB.Data.DataParameter("pYear", desiredDate.Year);
            var pMonth = new LinqToDB.Data.DataParameter("pMonth", desiredDate.Month);

            var list = await _dataProvider.QueryAsync<EfkaTekaResult>(connection, FinancialObligationQuery.EfkaTeka, pCompanyId, pYear, pMonth);

            var totalEfkaEmployer = list.Sum(s => s.EfkaEmployer);
            var totalTeka = list.Sum(s => s.Teka);

            var christmasPresentEfkaEmployer = list.Where(x => x.Periodos.Equals(13)).Sum(s => s.EfkaEmployer);
            var easterPresentEfkaEmployer = list.Where(x => x.Periodos.Equals(14)).Sum(s => s.EfkaEmployer);

            var christmasPresentTeka = list.Where(x => x.Periodos.Equals(13)).Sum(s => s.Teka);
            var easterPresentTeka = list.Where(x => x.Periodos.Equals(14)).Sum(s => s.Teka);

            decimal efkaEmployer = 0m;
            decimal teka = 0m;

            if (desiredMonth.Equals(1))
            {
                var pLastYear = new LinqToDB.Data.DataParameter("pYear", date.Year - 1);
                var pLastMonth = new LinqToDB.Data.DataParameter("pMonth", 12);
                var lastYearTotal = await _dataProvider.QueryAsync<EfkaTekaResult>(connection, FinancialObligationQuery.EfkaTeka, pCompanyId, pLastYear, pLastMonth);

                var lastChristmasPresentEfkaEmployer = lastYearTotal.Where(x => x.Periodos.Equals(13)).Sum(s => s.EfkaEmployer);
                var lastChristmasPresentTeka = lastYearTotal.Where(x => x.Periodos.Equals(13)).Sum(s => s.Teka);

                efkaEmployer = totalEfkaEmployer + lastChristmasPresentEfkaEmployer;
                teka = totalTeka + lastChristmasPresentTeka;
            }
            else if (desiredMonth.Equals(4))
            {
                efkaEmployer = totalEfkaEmployer - easterPresentEfkaEmployer;
                teka = totalTeka - easterPresentTeka;
            }
            else if (desiredMonth.Equals(5))
            {
                var lastMonth = new LinqToDB.Data.DataParameter("pMonth", 4);
                var aprilTotal = await _dataProvider.QueryAsync<EfkaTekaResult>(connection, FinancialObligationQuery.EfkaTeka, pCompanyId, pYear, lastMonth);

                var easterPresentMayEfkaEmployer = aprilTotal.Where(x => x.Periodos.Equals(14)).Sum(s => s.EfkaEmployer);
                var easterPresentMayTeka = aprilTotal.Where(x => x.Periodos.Equals(14)).Sum(s => s.Teka);

                efkaEmployer = totalEfkaEmployer + easterPresentMayEfkaEmployer;
                teka = totalTeka + easterPresentMayTeka;
            }
            else if (desiredMonth.Equals(12))
            {
                efkaEmployer = totalEfkaEmployer - christmasPresentEfkaEmployer;
                teka = totalTeka - christmasPresentTeka;
            }

            var tpteList = await _dataProvider.QueryAsync<PaymentIdentity>(connection, FinancialObligationQuery.PaymentsIdentity, pCompanyId);
            var tpte = tpteList.FirstOrDefault();

            string paymentIdTeka = tpte?.TpteTeka ?? "";
            string paymentIdEfka = tpte?.TpteEfka ?? "";

            var financials = new List<FinancialObligationDto>();

            // Έλεγχος αν οι εισφορές είναι μόνο από δώρο
            var isPresentChristmasTeka = totalTeka == christmasPresentTeka;
            var isPresentEasterTeka = totalTeka == easterPresentTeka;
            var isPresentChristmasEfkaEmployer = totalEfkaEmployer == christmasPresentEfkaEmployer;
            var isPresentEasterEfkaEmployer = totalEfkaEmployer == easterPresentEfkaEmployer;

            var apdTeka = await _apdTekaService.Table
                .Where(x => x.TraderId == trader.Id && x.Year == desiredDate.Year && x.Period == desiredDate.Month)
                .ToListAsync();

            if (!isPharmacy && (teka + totalTeka > 0))
            {
                if (!((isPresentChristmasTeka && desiredMonth == 12) || (isPresentEasterTeka && desiredMonth == 4)))
                {
                    financials.Add(
                        new FinancialObligationDto
                        {
                            PaymentValue = teka > 0 ? teka : totalTeka,
                            Institution = "ΕΦΚΑ Εργοδοτικές Εισφορές ΤΕΚΑ",
                            PaymentType = "ΤΕΚΑ",
                            PaymentDate = checkedDate,
                            PaymentIdentity = paymentIdTeka,
                            TraderId = trader.Id,
                            Period = date.Month
                        });

                    if (apdTeka.Count() == 1)
                    { 
                        var item = apdTeka.First();
                        item.InfoDateOnUtc = DateTime.UtcNow;

                        await _apdTekaService.UpdateApdTekaAsync(item);
                    }
                }
            }

            if ((efkaEmployer + totalEfkaEmployer) > 0)
            {
                if (!((isPresentChristmasEfkaEmployer && desiredMonth == 12) || (isPresentEasterEfkaEmployer && desiredMonth == 4)))
                {
                    financials.Add(
                        new FinancialObligationDto
                        {
                            PaymentValue = efkaEmployer > 0 ? efkaEmployer : totalEfkaEmployer,
                            Institution = "ΕΦΚΑ Εργοδοτικές Εισφορές",
                            PaymentType = "Εργοδοτικές Εισφορές",
                            PaymentDate = checkedDate,
                            PaymentIdentity = paymentIdEfka,
                            TraderId = trader.Id,
                            Period = date.Month
                        });

                    if (apdTeka.Count() == 1)
                    {
                        var item = apdTeka.First();
                        item.InfoDateOnUtc = DateTime.UtcNow;

                        await _apdTekaService.UpdateApdTekaAsync(item);
                    }
                }
            }

            //financials = financials.Where(x => x.PaymentValue > 0).ToList();

            return financials;
        }

        public Task<FinancialObligationRequestModel> PrepareFinancialObligationRequestModelAsync(FinancialObligationRequestModel requestModel)
        {
            requestModel.ChoiceId = 1;

            return Task.FromResult(requestModel);
        }

        private async Task<IList<SelectionItemList>> GetAllTradersAsync(FieldConfigType type)
        {
            var items = (await _traderService.GetAllTradersAsync(type, nonRepresentationOfNaturalPerson: true))
                .Select(x => 
                {
                    var trader = x.ToTraderDecrypt();
                    return new SelectionItemList { Value = x.Id, Label = $"{trader.FullName} - {trader.Vat}" };
                }).OrderBy(o => o.Label).ToList();

            return items;
        }

        public async Task<FinancialObligationRequestFormModel> PrepareFinancialObligationRequestFormModelAsync(FinancialObligationRequestFormModel requestFormModel)
        {
            var serviceOptions1 = await FinancialObligationType.Aade.ToSelectionItemListAsync();
            serviceOptions1.RemoveRange(2, 3);
            var serviceOptions2 = await FinancialObligationType.Aade.ToSelectionItemListAsync();
            serviceOptions2.RemoveRange(0, 2);
            var financialObligationChoiceTypes = await FinancialObligationChoiceType.IndividualLeagal.ToSelectionItemListAsync();

            var traderIds = FieldConfig.Create<FinancialObligationRequestModel>(nameof(FinancialObligationRequestModel.TraderIds), FieldType.MultiSelectAll);
            var serviceIds = FieldConfig.Create<FinancialObligationRequestModel>(nameof(FinancialObligationRequestModel.ServiceIds), FieldType.MultiSelect);
            var financialObligationChoiceIds = FieldConfig.Create<FinancialObligationRequestModel>(nameof(FinancialObligationRequestModel.ChoiceId), FieldType.Radio, options: financialObligationChoiceTypes);

            var left = new List<Dictionary<string, object>>()
            {
                traderIds,
                serviceIds,
                financialObligationChoiceIds
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<FinancialObligationRequestModel>(nameof(FinancialObligationRequestModel.Progress), FieldType.Text, _readonly: true),
                FieldConfig.Create<FinancialObligationRequestModel>("Retrieve", FieldType.Button, themeColor: "warning",
                    className: "col-12 text-center p-5", disableExpression: "model.traderIds == 0 || model.serviceIds == 0",
                    label: await _localizationService.GetResourceAsync("App.Common.Create"))
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6" }, left, right);

            requestFormModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));
            requestFormModel.CustomProperties.Add("traderOptions1", await GetAllTradersAsync(FieldConfigType.IndividualLegal));
            requestFormModel.CustomProperties.Add("traderOptions2", await GetAllTradersAsync(FieldConfigType.IndividualNatural));
            requestFormModel.CustomProperties.Add("serviceOptions1", serviceOptions1);
            requestFormModel.CustomProperties.Add("serviceOptions2", serviceOptions2);
            requestFormModel.CustomProperties.Add("serviceIds1", new List<int> { 1, 2, 32 });
            requestFormModel.CustomProperties.Add("serviceIds2", new List<int> { 4, 8, 16, 32 });

            return requestFormModel;
        }

        public virtual async Task<FinancialObligationFilterFormModel> PrepareFinancialObligationFilterFormModelAsync(FinancialObligationFilterFormModel filterFormModel)
        {
            var tristateTypes = await TristateType.Null.ToSelectionItemListAsync();
            var customerTypes = await CustomerType.Other.ToSelectionItemListAsync(true, true, -1);
            var legalFormTypes = await LegalFormType.None.ToSelectionItemListAsync(true, true, -1);
            var categoryBookTypes = await CategoryBookType.None.ToSelectionItemListAsync(true, true, -1);
            var traders = await _modelFactoryService.GetAllTradersAsync(FieldConfigType.IndividualLegalNatural);

            var array = await _financialObligationService.Table
                .Where(x => x.CustomerId > 0).Select(k => k.CustomerId)
                .Distinct()
                .ToArrayAsync();
            var customerIds = await _customerService.GetCustomersByIdsAsync(array);

            var customers = customerIds.Select(x => new SelectionItemList(x.Id, x.FullName())).ToList();
            customers.Insert(0, new SelectionItemList { Label = "", Value = 0 });

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<FinancialObligationFilterModel>(nameof(FinancialObligationFilterModel.TraderIds), FieldType.MultiSelectAll, options: traders),
                FieldConfig.Create<FinancialObligationFilterModel>(nameof(FinancialObligationFilterModel.Institution), FieldType.Text),
                FieldConfig.Create<FinancialObligationFilterModel>(nameof(FinancialObligationFilterModel.PaymentType), FieldType.Text),
            };

            var center = new List<Dictionary<string, object>>()
            {
                //FieldConfig.Create<FinancialObligationFilterModel>(nameof(FinancialObligationFilterModel.PaymentDate), FieldType.MonthDate)
                FieldConfig.Create<FinancialObligationFilterModel>(nameof(FinancialObligationFilterModel.CustomerTypeId), FieldType.Select, options: customerTypes),
                FieldConfig.Create<FinancialObligationFilterModel>(nameof(FinancialObligationFilterModel.LegalFormTypeId), FieldType.Select, options: legalFormTypes),
                FieldConfig.Create<FinancialObligationFilterModel>(nameof(FinancialObligationFilterModel.CategoryBookTypeId), FieldType.Select, options: categoryBookTypes)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<FinancialObligationFilterModel>(nameof(FinancialObligationFilterModel.CustomerId), FieldType.Select, options: customers),
                FieldConfig.Create<FinancialObligationFilterModel>(nameof(FinancialObligationFilterModel.PeriodDate), FieldType.MonthDate),
                FieldConfig.Create<FinancialObligationFilterModel>(nameof(FinancialObligationFilterModel.IsSent), FieldType.Select, options: tristateTypes)
            };

            var saveState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<FinancialObligationFilterModel>("SaveState", FieldType.Button, themeColor: "primary",
                label: await _localizationService.GetResourceAsync("App.Common.SaveState"))
            };

            var removeState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<FinancialObligationFilterModel>("RemoveState", FieldType.Button, themeColor: "warning",
                label: await _localizationService.GetResourceAsync("App.Common.RemoveState"))
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-4", "col-12 md:col-4", "col-12 md:col-4", "col-12 md:col-2", "col-12 md:col-2" }, left, center, right, saveState, removeState);

            filterFormModel.CustomProperties.Add("fields", fields);

            return filterFormModel;
        }
    }
}