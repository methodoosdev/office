using App.Core;
using App.Core.Domain.Payroll;
using App.Core.Infrastructure;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Payroll;
using App.Core.Infrastructure.Mapper;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Payroll;
using App.Models.Traders;
using App.Services;
using App.Services.Common;
using App.Services.Customers;
using App.Services.Employees;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Offices;
using App.Services.Payroll;
using App.Services.Traders;
using App.Web.Infra.Queries.Payroll;
using LinqToDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Messages
{
    public partial interface IApdTekaModelFactory
    {
        Task<ApdTekaSearchModel> PrepareApdTekaSearchModelAsync(ApdTekaSearchModel searchModel);
        Task<ApdTekaListModel> PrepareApdTekaListModelAsync(ApdTekaSearchModel searchModel);
        Task<ApdTekaModel> PrepareApdTekaModelAsync(ApdTekaModel model, ApdTeka apdTeka);
        Task<ApdTekaFormModel> PrepareApdTekaFormModelAsync(ApdTekaFormModel formModel);
        Task<ApdTekaFilterFormModel> PrepareApdTekaFilterFormModelAsync(ApdTekaFilterFormModel apdTekaFilterFormModel);
        Task PrepareCreatePeriodAsync(int year, int period);
        Task<ApdTekaDialogFormModel> PrepareApdTekaDialogFormModelAsync(ApdTekaDialogFormModel formModel);
        Task PreparePayrollStatusAsync(ICollection<int> selectedIds, string connection);
        Task<CustomerActivityResult> PrepareApdSubmitAsync(ICollection<int> selectedIds);
        Task<CustomerActivityResult> PrepareTekaSubmitAsync(ICollection<int> selectedIds);
    }
    public partial class ApdTekaModelFactory : IApdTekaModelFactory
    {
        private readonly IApdTekaService _apdTekaService;
        private readonly IEmployeeService _employeeService;
        private readonly ITraderService _traderService;
        private readonly ITraderEmployeeMappingService _traderEmployeeMappingService;
        private readonly ILocalizationService _localizationService;
        private readonly IPersistStateService _persistStateService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ISqlConnectionService _connectionService;
        private readonly IAppDataProvider _dataProvider;
        private readonly IWorkContext _workContext;
        private readonly PlaywrightHttpClient _httpClient;

        public ApdTekaModelFactory(
            IApdTekaService apdTekaService,
            IEmployeeService employeeService,
            ITraderService traderService,
            ITraderEmployeeMappingService traderEmployeeMappingService,
            ILocalizationService localizationService,
            IModelFactoryService modelFactoryService,
            IPersistStateService persistStateService,
            IDateTimeHelper dateTimeHelper,
            ISqlConnectionService connectionService,
            IAppDataProvider dataProvider,
            IWorkContext workContext,
            PlaywrightHttpClient httpClient)
        {
            _apdTekaService = apdTekaService;
            _employeeService = employeeService;
            _traderService = traderService;
            _traderEmployeeMappingService = traderEmployeeMappingService;
            _localizationService = localizationService;
            _persistStateService = persistStateService;
            _dateTimeHelper = dateTimeHelper;
            _connectionService = connectionService;
            _dataProvider = dataProvider;
            _workContext = workContext;
            _httpClient = httpClient;
        }

        private async Task<IPagedList<ApdTekaModel>> GetPagedListAsync(ApdTekaSearchModel searchModel, ApdTekaFilterModel filterModel, bool filterExist)
        {
            var traders = await _traderService.Table.Where(x => x.HyperPayrollId > 0).ToListAsync();
            var employees = await _employeeService.GetAllEmployeesAsync();
            var customer = await _workContext.GetCurrentCustomerAsync();

            var query = _apdTekaService.Table.AsEnumerable()
                .Select(x =>
                {
                    var model = x.ToModel<ApdTekaModel>();

                    model.CompanyName = traders.FirstOrDefault(t => t.HyperPayrollId == x.CompanyId)?.ToTraderFullName() ?? string.Empty;
                    model.EmployeeName = x.EmployeeId.HasValue ? employees.FirstOrDefault(e => e.Id == x.EmployeeId.Value)?.FullName() ?? string.Empty : string.Empty;

                    if (x.ApdSubmitDateOnUtc.HasValue)
                        model.ApdSubmitDateOn = _dateTimeHelper.ConvertToUserTimeAsync(x.ApdSubmitDateOnUtc.Value, DateTimeKind.Utc).Result;

                    if (x.TekaSubmitDateOnUtc.HasValue)
                        model.TekaSubmitDateOn = _dateTimeHelper.ConvertToUserTimeAsync(x.TekaSubmitDateOnUtc.Value, DateTimeKind.Utc).Result;

                    if (x.InfoDateOnUtc.HasValue)
                        model.InfoDateOn = _dateTimeHelper.ConvertToUserTimeAsync(x.InfoDateOnUtc.Value, DateTimeKind.Utc).Result;

                    model.PeriodName = $"{x.Period.ToString("00")}/{x.Year}";

                    model.ErrorMessage =
                        x.WorkersError + Environment.NewLine + Environment.NewLine +
                        x.ApdError + Environment.NewLine + Environment.NewLine +
                        x.TekaError + Environment.NewLine;

                    return model;
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.CompanyName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            if (filterExist)
            {
                if (filterModel.CompanyId.HasValue && filterModel.CompanyId.Value > 0)
                    query = query.Where(x => x.CompanyId == filterModel.CompanyId.Value);

                if (filterModel.EmployeeId.HasValue && filterModel.EmployeeId.Value > 0)
                    query = query.Where(x => x.EmployeeId == filterModel.EmployeeId.Value);

                if (filterModel.Period.HasValue)
                {
                    var period = filterModel.Period.Value;
                    query = query.Where(x => x.Year == period.Year && x.Period == period.Month);
                }
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<ApdTekaSearchModel> PrepareApdTekaSearchModelAsync(ApdTekaSearchModel searchModel)
        {
            var persistState = await _persistStateService.GetModelInstance<ApdTekaSearchModel>();

            if (persistState.Exist)
                return persistState.Model;

            //prepare page parameters
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = CreateKendoGridColumnConfig();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.ApdTekaModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<ApdTekaListModel> PrepareApdTekaListModelAsync(ApdTekaSearchModel searchModel)
        {
            var filterState = await _persistStateService.GetModelInstance<ApdTekaFilterModel>();

            //get customer roles
            var apdTekas = await GetPagedListAsync(searchModel, filterState.Model, filterState.Exist);

            //prepare grid model
            var model = new ApdTekaListModel().PrepareToGrid(searchModel, apdTekas);
            model.FilterExist = filterState.Exist;

            return model;
        }

        public virtual Task<ApdTekaModel> PrepareApdTekaModelAsync(ApdTekaModel model, ApdTeka apdTeka)
        {
            if (apdTeka != null)
            {
                //fill in model values from the entity
                model ??= apdTeka.ToModel<ApdTekaModel>();
            }

            return Task.FromResult(model);
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ApdTekaModel>(2, nameof(ApdTekaModel.CompanyName), ColumnType.RouterLink),
                ColumnConfig.Create<ApdTekaModel>(1, nameof(ApdTekaModel.PeriodName), style: centerAlign),
                ColumnConfig.Create<ApdTekaModel>(1, nameof(ApdTekaModel.EmployeeName), hidden: true),
                ColumnConfig.Create<ApdTekaModel>(3, nameof(ApdTekaModel.CheckWorkers), ColumnType.Checkbox),
                ColumnConfig.Create<ApdTekaModel>(4, nameof(ApdTekaModel.DxApd), ColumnType.Decimal, hidden: true),
                ColumnConfig.Create<ApdTekaModel>(4, nameof(ApdTekaModel.DxTeka), ColumnType.Decimal, hidden: true),
                ColumnConfig.Create<ApdTekaModel>(4, nameof(ApdTekaModel.DpApd), ColumnType.Decimal, hidden: true),
                ColumnConfig.Create<ApdTekaModel>(4, nameof(ApdTekaModel.DpTeka), ColumnType.Decimal, hidden: true),
                ColumnConfig.Create<ApdTekaModel>(4, nameof(ApdTekaModel.Contributions), ColumnType.Decimal),
                ColumnConfig.Create<ApdTekaModel>(4, nameof(ApdTekaModel.Subsidiary), ColumnType.Decimal),
                ColumnConfig.Create<ApdTekaModel>(4, nameof(ApdTekaModel.Apd), ColumnType.Decimal),
                ColumnConfig.Create<ApdTekaModel>(4, nameof(ApdTekaModel.Teka), ColumnType.Decimal),
                ColumnConfig.Create<ApdTekaModel>(4, nameof(ApdTekaModel.ApdSubmit), ColumnType.Decimal),
                ColumnConfig.Create<ApdTekaModel>(4, nameof(ApdTekaModel.TekaSubmit), ColumnType.Decimal),
                ColumnConfig.Create<ApdTekaModel>(5, nameof(ApdTekaModel.ApdSubmitDateOn), ColumnType.DateTime, style: centerAlign),
                ColumnConfig.Create<ApdTekaModel>(5, nameof(ApdTekaModel.TekaSubmitDateOn), ColumnType.DateTime, style: centerAlign),
                ColumnConfig.Create<ApdTekaModel>(6, nameof(ApdTekaModel.InfoDateOn), ColumnType.DateTime, style: centerAlign),
                ColumnConfig.Create<ApdTekaModel>(4, nameof(ApdTekaModel.ErrorMessage)),
                ColumnConfig.Create<ApdTekaModel>(4, nameof(ApdTekaModel.Notes), hidden: true)
            };

            return columns;
        }

        public virtual async Task<ApdTekaFormModel> PrepareApdTekaFormModelAsync(ApdTekaFormModel formModel)
        {
            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ApdTekaModel>(nameof(ApdTekaModel.InfoDateOnUtc), FieldType.Date, nullable: true)
            };

            var editor = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ApdTekaModel>(nameof(ApdTekaModel.ApdSubmit), FieldType.Decimals),
                FieldConfig.Create<ApdTekaModel>(nameof(ApdTekaModel.TekaSubmit), FieldType.Decimals),
                FieldConfig.Create<ApdTekaModel>(nameof(ApdTekaModel.Notes), FieldType.Textarea),
                FieldConfig.Create<ApdTekaModel>(nameof(ApdTekaModel.WorkersError), FieldType.Textarea, rows: 5),
                FieldConfig.Create<ApdTekaModel>(nameof(ApdTekaModel.ApdError), FieldType.Textarea, rows: 5),
                FieldConfig.Create<ApdTekaModel>(nameof(ApdTekaModel.TekaError), FieldType.Textarea, rows: 5)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12" }, left, editor);

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ApdTekaModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", fields);

            return formModel;
        }

        public virtual async Task<ApdTekaFilterFormModel> PrepareApdTekaFilterFormModelAsync(ApdTekaFilterFormModel filterFormModel)
        {
            //var traderList = await _traderService.Table.Where(x => x.HyperPayrollId > 0).ToListAsync();
            //var traders = traderList
            //    .Select(x => new SelectionItemList(x.HyperPayrollId, x.ToTraderFullName())).ToList();
            //traders = traders.OrderBy(x => x.Label).ToList();

            var traderEmployees = await _traderEmployeeMappingService.GetPayrollTraderEmployeesAsync();
            var traderIds = traderEmployees.Select(x => x.TraderId).ToList();
            var traderList = await _traderService.GetTradersByIdsAsync(traderIds.ToArray());

            var traders = traderList
                .Select(x => new SelectionItemList(x.HyperPayrollId, x.ToTraderFullName())).ToList();
            traders = traders.OrderBy(x => x.Label).ToList();

            var employeeList = await _employeeService.GetAllEmployeesAsync();
            var employees = employeeList
                .Where(x => x.DepartmentId == 3)
                .Select(x => new SelectionItemList(x.Id, x.FullName())).ToList();

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ApdTekaFilterModel>(nameof(ApdTekaFilterModel.CompanyId), FieldType.GridSelect, options: traders),
            };

            var center = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ApdTekaFilterModel>(nameof(ApdTekaFilterModel.EmployeeId), FieldType.GridSelect, options: employees),
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ApdTekaFilterModel>(nameof(ApdTekaFilterModel.Period), FieldType.MonthDate, nullable: true)
            };

            var saveState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ApdTekaFilterModel>("SaveState", FieldType.Button, themeColor: "primary",
                label: await _localizationService.GetResourceAsync("App.Common.SaveState"))
            };

            var removeState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ApdTekaFilterModel>("RemoveState", FieldType.Button, themeColor: "warning",
                label: await _localizationService.GetResourceAsync("App.Common.RemoveState"))
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-4", "col-12 md:col-4", "col-12 md:col-4", "col-12 md:col-2", "col-12 md:col-2" }, left, center, right, saveState, removeState);

            filterFormModel.CustomProperties.Add("fields", fields);

            return filterFormModel;
        }

        public async Task PrepareCreatePeriodAsync(int _year, int _period)
        {
            var apdTekas = await _apdTekaService.GetAllApdTekasAsync(year: _year, period: _period);
            var traderEmployees = await _traderEmployeeMappingService.GetPayrollTraderEmployeesAsync();
            var list = new List<ApdTeka>();

            foreach (var item in traderEmployees)
            {
                var exist = apdTekas
                    .Any(x => x.TraderId == item.TraderId && x.CompanyId == item.CompanyId && x.Year == _year && x.Period == _period);

                if (exist)
                    continue;

                var model = new ApdTeka();

                model.TraderId = item.TraderId;
                model.CompanyId = item.CompanyId;
                model.EmployeeId = item.EmployeeId;
                model.Year = _year;
                model.Period = _period;

                list.Add(model);
            }

            await _apdTekaService.InsertApdTekaAsync(list);
        }

        public virtual async Task<ApdTekaDialogFormModel> PrepareApdTekaDialogFormModelAsync(ApdTekaDialogFormModel formModel)
        {
            var fields = new List<Dictionary<string, object>>()
            {
                //FieldConfig.Create<ApdTekaDialogModel>(nameof(ApdTekaDialogModel.CompanyId), FieldType.GridSelect, options: traders, min: 1, required: true),
                FieldConfig.Create<ApdTekaDialogModel>(nameof(ApdTekaDialogModel.Period), FieldType.MonthDate)
            };

            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }

        public async Task PreparePayrollStatusAsync(ICollection<int> selectedIds, string connection)
        {
            var apdTekas = await _apdTekaService.GetApdTekasByIdsAsync(selectedIds.ToArray());

            foreach (var apdTeka in apdTekas)
            {
                var firstDateOfMonth = new DateTime(apdTeka.Year, apdTeka.Period, 1);
                var lastDateOfMonth = firstDateOfMonth.AddMonths(1).AddDays(-1);

                var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", apdTeka.CompanyId);
                var pYear = new LinqToDB.Data.DataParameter("pYear", apdTeka.Year);
                var pPeriod = new LinqToDB.Data.DataParameter("pPeriod", apdTeka.Period);
                var pStartingDate = new LinqToDB.Data.DataParameter("pStartingDate",
                    firstDateOfMonth.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
                var pEndingDate = new LinqToDB.Data.DataParameter("pEndingDate",
                    lastDateOfMonth.ToString("yyyyMMdd", CultureInfo.InvariantCulture));

                var employees = await _dataProvider
                    .QueryAsync<ApdTekaEmployees>(connection, ApdTekaQuery.Employees, pCompanyId, pYear, pPeriod);
                var empIds = employees.Select(x => x.EmployeeId).ToList();
                var payrollEmployees = await _dataProvider
                    .QueryTimeoutAsync<ApdTekaEmployees>(
                        connection, 300, ApdTekaQuery.PayrollEmployees, pCompanyId, pYear, pStartingDate, pEndingDate);
                var payrollIds = payrollEmployees.Select(x => x.EmployeeId).Distinct().ToList();

                apdTeka.CheckWorkers = empIds.Count() == payrollEmployees.Count();

                var list1 = empIds.Count() > 0 ? empIds.Except(payrollIds).ToList() : new();
                var list2 = payrollIds.Count() > 0 ? payrollIds.Except(empIds).ToList() : new();
                var workersError = string.Empty;

                if (list1.Count() > 0)
                {
                    var employeeNames = employees
                        .Where(x => list1.Contains(x.EmployeeId)).Select(x => x.EmployeeName).ToList();
                    workersError += "Εργ.->Μισθ. " + string.Join(", ", employeeNames) + Environment.NewLine;
                }

                if (list2.Count() > 0)
                {
                    var employeeNames = payrollEmployees
                        .Where(x => list2.Contains(x.EmployeeId)).Select(x => x.EmployeeName).ToList();
                    workersError += "Μισθ.->Εργ. " + string.Join(", ", employeeNames) + Environment.NewLine;
                }

                apdTeka.WorkersError = workersError;

                if (payrollIds.Count() == 0)
                    continue;

                IList<ApdTekaPayrollStatus> payrollStatusList = new List<ApdTekaPayrollStatus>();
                try
                {
                    var payrollStatusQuery = ApdTekaQuery.PayrollStatus.Replace("@@Employees", string.Join(", ", payrollIds));

                    payrollStatusList = await _dataProvider
                        .QueryAsync<ApdTekaPayrollStatus>(
                        connection, payrollStatusQuery, pCompanyId, pYear, pStartingDate, pEndingDate);
                }
                catch { }

                var dxApd = payrollStatusList.Where(x => x.PeriodId == 13).Sum(x => x.Contributions);
                var dxTeka = payrollStatusList.Where(x => x.PeriodId == 13).Sum(x => x.Subsidiary);
                var dpApd = payrollStatusList.Where(x => x.PeriodId == 14).Sum(x => x.Contributions);
                var dpTeka = payrollStatusList.Where(x => x.PeriodId == 14).Sum(x => x.Subsidiary);
                var contributions = payrollStatusList.Where(x => !(x.PeriodId == 13) && !(x.PeriodId == 14)).Sum(x => x.Contributions);
                var subsidiary = payrollStatusList.Where(x => !(x.PeriodId == 13) && !(x.PeriodId == 14)).Sum(x => x.Subsidiary);

                if (apdTeka.Period == 4)
                {
                    apdTeka.DpApd = dpApd;
                    apdTeka.DpTeka = dpTeka;
                    apdTeka.DxApd = 0;
                    apdTeka.DxTeka = 0;
                    apdTeka.Contributions = contributions;
                    apdTeka.Subsidiary = subsidiary;

                }
                else if (apdTeka.Period == 12)
                {
                    apdTeka.DpApd = 0;
                    apdTeka.DpTeka = 0;
                    apdTeka.DxApd = dxApd;
                    apdTeka.DxTeka = dxTeka;
                    apdTeka.Contributions = contributions;
                    apdTeka.Subsidiary = subsidiary;
                }
                else
                {
                    apdTeka.DpApd = 0;
                    apdTeka.DpTeka = 0;
                    apdTeka.DxApd = 0;
                    apdTeka.DxTeka = 0;
                    apdTeka.Contributions = contributions + dpApd + dxApd;
                    apdTeka.Subsidiary = subsidiary + dpTeka + dxTeka;
                }

                var efkaList = await _dataProvider.QueryAsync<ApdTekaEfka>(connection, ApdTekaQuery.Efka, pCompanyId, pYear, pPeriod);
                var efkaApd = efkaList.FirstOrDefault(x => x.Mode == 0);
                var efkaTeka = efkaList.FirstOrDefault(x => x.Mode == 3);

                if (efkaApd != null)
                    apdTeka.Apd = efkaApd.TotalContribution;

                if (efkaTeka != null)
                    apdTeka.Teka = efkaTeka.TotalContribution;

                await _apdTekaService.UpdateApdTekaAsync(apdTeka);
            }

        }

        private async Task<(ApdSubmissionModel Apd, string Message, string Error)> ApdSendAsync(string url, string traderName)
        {
            var result = await _httpClient.SendAsync(HttpMethod.Post, url);
            if (result.Success)
            {
                var response = JsonConvert.DeserializeObject<DtoListResponse<ApdSubmissionDto>>(result.Content);
                if (response.Success)
                {
                    var total = 0m;
                    var models = new List<ApdSubmissionModel>();
                    foreach (var item in response.List)
                    {
                        var model = await ExtractSubmissionApdModel(item.Found, item.PdfText, traderName);
                        total += model.TotalContributions;
                        models.Add(model);
                    }
                    var apd = models.First(x => x.Type == "ΚΑΝΟΝΙΚΗ");
                    apd.TotalContributions = total;

                    return (apd, response.Message, null);
                }
                else
                    return (null, null, await _localizationService.GetResourceAsync(response.Error));
            }
            else
                return (null, null, await _localizationService.GetResourceAsync(result.Error));
        }

        private async Task<(ApdSubmissionModel Apd, string Message, string Error)> TekaSendAsync(string url, string traderName)
        {
            var result = await _httpClient.SendAsync(HttpMethod.Post, url);
            if (result.Success)
            {
                var response = JsonConvert.DeserializeObject<DtoListResponse<ApdSubmissionDto>>(result.Content);
                if (response.Success)
                {
                    var total = 0m;
                    var models = new List<ApdSubmissionModel>();
                    foreach (var item in response.List)
                    {
                        var model = await ExtractSubmissionTekaModel(item.Found, item.PdfText, traderName);
                        total += model.TotalContributions;
                        models.Add(model);
                    }
                    var apd = models.First(x => x.Type == "01 Κανονική");
                    apd.TotalContributions = total;

                    return (apd, response.Message, null);
                }
                else
                    return (null, null, await _localizationService.GetResourceAsync(response.Error));
            }
            else
                return (null, null, await _localizationService.GetResourceAsync(result.Error));
        }

        private async Task<string> GetAmIkaAsync(int companyId)
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            if (!result.Success)
                return string.Empty;

            var employers = await _dataProvider.QueryAsync<EmployerLookupItem>(result.Connection, PayrollQuery.EmployerLookupItem);

            var employer = employers.FirstOrDefault(x => x.CompanyId == companyId);

            return employer?.AmIka.Trim() ?? string.Empty;
        }

        public async Task<CustomerActivityResult> PrepareApdSubmitAsync(ICollection<int> selectedIds)
        {
            var custActivity = new CustomerActivityResult();
            var apdTekas = await _apdTekaService.GetApdTekasByIdsAsync(selectedIds.ToArray());

            var index = 1;

            foreach (var apdTeka in apdTekas)
            {
                var trader = await _traderService.GetTraderByIdAsync(apdTeka.TraderId);
                var traderName = trader.ToTraderFullName();

                var userName = AesEncryption.Decrypt(trader.TaxisUserName)?.Trim();
                var password = AesEncryption.Decrypt(trader.TaxisPassword)?.Trim();

                var amIka = await GetAmIkaAsync(trader.HyperPayrollId);

                if (string.IsNullOrEmpty(amIka))
                {
                    var errorMessage = "Δεν έχει καταχωρημενο ΑΜΕ στη μισθοδοσία.";
                    apdTeka.ApdError = errorMessage;
                    custActivity.AddError($"<b>ΑΜΕ:</b> {traderName}. {errorMessage}");

                    continue;
                }
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    var errorMessage = await _localizationService.GetResourceAsync("App.Errors.WrongCredentials");
                    apdTeka.ApdError = errorMessage;
                    custActivity.AddError($"<b>Σύνδεση:</b> {traderName}. {errorMessage}");

                    continue;
                }

                var format = "{0}?index={1}&traderName={2}&month={3}&year={4}&userName={5}&password={6}&amIka={7}&connectionId={8}";
                var url = string.Format(format,
                    "api/apdSubmission/list",
                    index,
                    WebUtility.UrlEncode(traderName),
                    apdTeka.Period,
                    apdTeka.Year,
                    WebUtility.UrlEncode(userName),
                    WebUtility.UrlEncode(password),
                    WebUtility.UrlEncode(amIka),
                    null); // connectionId

                var result = await ApdSendAsync(url, traderName);

                if (string.IsNullOrEmpty(result.Error))
                {
                    apdTeka.ApdSubmitDateOnUtc = DateTime.Parse(result.Apd.SubmissionDate, new CultureInfo("el-GR"));
                    apdTeka.ApdSubmit = result.Apd.TotalContributions;
                    apdTeka.ApdError = string.Empty;
                    custActivity.AddSuccess(result.Message);
                }
                else
                {
                    apdTeka.ApdError = result.Error;
                    custActivity.AddError(result.Error);
                }

                index++;
            }

            await _apdTekaService.UpdateApdTekaAsync(apdTekas);

            return custActivity;
        }

        public async Task<CustomerActivityResult> PrepareTekaSubmitAsync(ICollection<int> selectedIds)
        {
            var custActivity = new CustomerActivityResult();
            var apdTekas = await _apdTekaService.GetApdTekasByIdsAsync(selectedIds.ToArray());

            var index = 1;

            foreach (var apdTeka in apdTekas)
            {
                var trader = await _traderService.GetTraderByIdAsync(apdTeka.TraderId);
                var traderName = trader.ToTraderFullName();

                var userName = AesEncryption.Decrypt(trader.EmployerIkaUserName)?.Trim();
                var password = AesEncryption.Decrypt(trader.EmployerIkaPassword)?.Trim();

                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    var errorMessage = await _localizationService.GetResourceAsync("App.Errors.WrongCredentials");
                    apdTeka.TekaError = errorMessage;
                    custActivity.AddError($"<b>Σύνδεση:</b> {traderName}. {errorMessage}");

                    continue;
                }

                var format = "{0}?index={1}&traderName={2}&month={3}&year={4}&userName={5}&password={6}&connectionId={7}";
                var url = string.Format(format,
                    "api/tekaSubmission/list",
                    index,
                    WebUtility.UrlEncode(traderName),
                    apdTeka.Period,
                    apdTeka.Year,
                    WebUtility.UrlEncode(userName),
                    WebUtility.UrlEncode(password),
                    null); // connectionId

                var result = await TekaSendAsync(url, traderName);

                if (string.IsNullOrEmpty(result.Error))
                {
                    apdTeka.TekaSubmitDateOnUtc = DateTime.Parse(result.Apd.SubmissionDate, new CultureInfo("el-GR"));
                    apdTeka.TekaSubmit = result.Apd.TotalContributions;
                    apdTeka.TekaError = string.Empty;
                    custActivity.AddSuccess(result.Message);
                }
                else
                {
                    apdTeka.TekaError = result.Error;
                    custActivity.AddError(result.Error);
                }

                index++;
            }

            await _apdTekaService.UpdateApdTekaAsync(apdTekas);

            return custActivity;
        }

        public async Task<ApdSubmissionModel> ExtractSubmissionApdModel(bool found, string pdfText, string traderName)
        {
            var model = new ApdSubmissionModel();
            if (found)
            {
                var lines = pdfText.Split('\n').ToList();
                lines.RemoveAt(0);

                var date = lines[0].Split(':');
                var period = lines[17].Split('/');
                var month = period[0];
                var year = period[1];

                model.SubmissionNumber = lines[27];
                model.Type = lines[3];
                model.Ame = lines[9];
                model.Surname = lines[7];
                model.Vat = lines[10];
                model.Period = lines[17];
                model.Month = month;
                model.Year = year;
                model.Amoe = "";
                model.TotalInsuranceDays = int.Parse(lines[19]);
                model.TotalEarnings = decimal.Parse(lines[21], new CultureInfo("el-GR"));
                model.TotalContributions = decimal.Parse(lines[23], new CultureInfo("el-GR"));
                model.SubmissionDate = date[1].Trim();
                model.Tpte = lines[24];

                model.Submitted = true;
            }
            else
            {
                model.Surname = traderName;
                model.Error = await _localizationService.GetResourceAsync("App.Errors.NotSubmitted");
            }

            return model;
        }

        public async Task<ApdSubmissionModel> ExtractSubmissionTekaModel(bool found, string pdfText, string traderName)
        {
            var model = new ApdSubmissionModel();
            if (found)
            {
                var lines = pdfText.Split('\n');

                var types = lines[5].Split(' ');
                var period = lines[15].Split('/');
                var month = period[0];
                var year = period[1];
                var numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };

                model.SubmissionNumber = lines[3];
                model.Type = types[2] + " " + types[3];
                model.Ame = lines[10];
                model.Surname = lines[6];
                model.Vat = lines[13];
                model.Period = $"{month}/{year}";
                model.Month = month;
                model.Year = year;
                model.Amoe = "";
                model.TotalInsuranceDays = int.Parse(lines[18]);
                model.TotalEarnings = decimal.Parse(lines[20], numberFormatInfo);
                model.TotalContributions = decimal.Parse(lines[22], numberFormatInfo);
                model.SubmissionDate = lines[8];
                model.Tpte = lines[24];

                model.Submitted = true;
            }
            else
            {
                model.Surname = traderName;
                model.Error = await _localizationService.GetResourceAsync("App.Errors.NotSubmitted");
            }

            return model;
        }
    }
}