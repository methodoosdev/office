using App.Core;
using App.Core.Domain.Payroll;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Accounting;
using App.Models.Traders;
using App.Services;
using App.Services.Configuration;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Infra.Queries.Payroll;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Accounting
{
    public partial interface IArticlesCheckModelFactory
    {
        Task<ArticlesCheckSearchModel> PrepareArticlesCheckSearchModelAsync(ArticlesCheckSearchModel searchModel);
        Task<ArticlesCheckSearchFormModel> PrepareArticlesCheckSearchFormModelAsync(ArticlesCheckSearchFormModel searchFormModel, int employeeId, string connection);
        Task<IList<ArticlesCheckModel>> PrepareArticlesCheckModelListAsync(string connection, ArticlesCheckSearchModel searchModel);
        Task<ArticlesCheckTableModel> PrepareArticlesCheckTableModelAsync(ArticlesCheckTableModel tableModel);
        Task<IList<ArticlesCheckAccountModel>> PrepareArticlesCheckAcountListAsync(string softOneCon, string connection, int companyId, int nglId, int year, int period);
        Task<ArticlesCheckAccountTableModel> PrepareArticlesCheckAccountTableModelAsync(ArticlesCheckAccountTableModel tableModel);
    }
    public partial class ArticlesCheckModelFactory : IArticlesCheckModelFactory
    {
        private readonly ITraderService _traderService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ITraderEmployeeMappingService _traderEmployeeMappingService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;

        private readonly string[] fmyPatterns = new[] { "54.03.01.1000.0000", "54.03.05.1000.0000", "54.04.02.1000.0000" };
        private readonly string[] efkaPatterns = new[] { "55.01.00.0000.0000" };
        private readonly string[] tekaPatterns = new[] { "55.03.00.0000.0000" };
        private readonly string[] salaryPatterns = new[] { "53.03.00.1000.*", "53.04.00.1000.*" };

        public ArticlesCheckModelFactory(
            ITraderService traderService,
            ITraderConnectionService traderConnectionService,
            ITraderEmployeeMappingService traderEmployeeMappingService,
            IModelFactoryService modelFactoryService,
            IFieldConfigService fieldConfigService,
            IAppDataProvider dataProvider,
            ILocalizationService localizationService)
        {
            _traderService = traderService;
            _traderConnectionService = traderConnectionService;
            _traderEmployeeMappingService = traderEmployeeMappingService;
            _modelFactoryService = modelFactoryService;
            _fieldConfigService = fieldConfigService;
            _dataProvider = dataProvider;
            _localizationService = localizationService;
        }

        public virtual Task<ArticlesCheckSearchModel> PrepareArticlesCheckSearchModelAsync(ArticlesCheckSearchModel searchModel)
        {
            searchModel.Periodos = DateTime.UtcNow.AddMonths(-1).Date;
            searchModel.NglId = 18; // ΠΡΟΤΥΠΟ ΛΟΓΙΣΤΙΚΟ ΑΡΘΡΟ ΕΤΑΙΡΙΩΝ-NEW   4-copy-cop

            return Task.FromResult(searchModel);
        }

        public virtual async Task<ArticlesCheckSearchFormModel> PrepareArticlesCheckSearchFormModelAsync(ArticlesCheckSearchFormModel formModel, int employeeId, string connection)
        {
            var employers = await _fieldConfigService.GetEmployers();
            var employeeList = await _modelFactoryService.GetAllEmployeesAsync();
            var employees = new List<SelectionTraderIdsList>();

            foreach (var employee in employeeList)
            {
                var traders = await _traderEmployeeMappingService.GetTradersByEmployeeIdAsync(employee.Value);
                var vats = traders.Select(x => x.ToTraderDecrypt().Vat).ToList();
                var employerList = employers.Where(x => vats.Contains(x.Vat)).ToList();
                var ids = employerList.Select(x => x.CompanyId).ToList();
                var selection = new SelectionTraderIdsList { Label = employee.Label, Value = employee.Value, Ids = ids };
                employees.Add(selection);
            }
            var nglList = await _dataProvider.QueryAsync<SelectionItemList>(connection, ArticlesCheckQuery.Ngl);

            var left = new List<Dictionary<string, object>>()
            {
                await _fieldConfigService.GetEmployersMultiColumnComboBox<ArticlesCheckSearchModel>(nameof(ArticlesCheckSearchModel.EmployerIds)),
                FieldConfig.Create<ArticlesCheckSearchModel>(nameof(ArticlesCheckSearchModel.NglId), FieldType.Select, options: nglList)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<ArticlesCheckSearchModel>(nameof(ArticlesCheckSearchModel.EmployeeId), FieldType.GridSelect, options: employees),
                FieldConfig.Create<ArticlesCheckSearchModel>(nameof(ArticlesCheckSearchModel.Periodos), FieldType.MonthDate)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6" }, left, right);

            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }

        private bool AreEqual(ArticlesCheckDto dto1, ArticlesCheckDto dto2, ArticlesCheckModel item)
        {
            var result = new List<bool>();
            if (dto1 == null || dto2 == null) return false;

            PropertyInfo[] properties = typeof(ArticlesCheckDto).GetProperties();

            foreach (PropertyInfo property in properties) 
            {
                if ((decimal)property.GetValue(dto1) == (decimal)property.GetValue(dto2))
                {
                    result.Add(true);
                }
                else
                {
                    var name = $"_{property.Name.ToLower()}";
                    var itemInfo = item.GetType().GetProperties().First(x => x.Name == name);
                    itemInfo.SetValue(item, true, null);
                    result.Add(false);
                }
            }

            return result.All(x => x == true);
        }

        public virtual async Task<IList<ArticlesCheckModel>> PrepareArticlesCheckModelListAsync(string connection, ArticlesCheckSearchModel searchModel)
        {
            var models = new List<ArticlesCheckModel>();
            var employers = await _dataProvider.QueryAsync<EmployerLookupItem>(connection, PayrollQuery.EmployerLookupItem);
            var errorMessage = await _localizationService.GetResourceAsync("App.Common.Error");

            var companyIds = searchModel.EmployerIds.ToList();
            if (companyIds.Count > 0)
                employers = employers.Where(x => companyIds.Contains(x.CompanyId)).ToList();

            var firstDateOfMonth = new DateTime(searchModel.Periodos.Year, searchModel.Periodos.Month, 1);
            var lastDateOfMonth = firstDateOfMonth.AddMonths(1).AddDays(-1);

            var pNgl = new LinqToDB.Data.DataParameter("pNgl", searchModel.NglId);
            var pYear = new LinqToDB.Data.DataParameter("pYear", searchModel.Periodos.Year);
            var pPeriod = new LinqToDB.Data.DataParameter("pPeriod", searchModel.Periodos.Month);
            var pStartingDate = new LinqToDB.Data.DataParameter("pStartingDate",
                firstDateOfMonth.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
            var pEndingDate = new LinqToDB.Data.DataParameter("pEndingDate",
                lastDateOfMonth.ToString("yyyyMMdd", CultureInfo.InvariantCulture));

            foreach (var employer in employers)
            {
                var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", employer.CompanyId);
                var employees = await _dataProvider
                    .QueryAsync<ArticlesCheckEmployeeId>(connection, ArticlesCheckQuery.PayrollEmployees, pCompanyId, pYear, pStartingDate, pEndingDate);
                var employeeIds = employees.Select(x => x.ID_EMP).ToList();

                var article1 = new ArticlesCheckDto();
                if (employeeIds.Any())
                {
                    var query = ArticlesCheckQuery.PayrollStatus.Replace("@@Employees", string.Join(", ", employeeIds));
                    var payrollStatusList = await _dataProvider
                        .QueryAsync<ArticlesCheckStatus>(connection, query, pCompanyId, pYear, pStartingDate, pEndingDate);

                    article1.Fmy = payrollStatusList.Sum(x => x.FMY) + payrollStatusList.Sum(x => x.XARTOSHMO);
                    article1.Salary = payrollStatusList.Sum(x => x.KATHARESAPODOXES);
                    article1.Total = payrollStatusList.Sum(x => x.S_KOSTOS);
                    article1.Efka = payrollStatusList.Sum(x => x.Efka);
                    article1.Teka = payrollStatusList.Sum(x => x.Teka);
                }

                employees = await _dataProvider
                    .QueryAsync<ArticlesCheckEmployeeId>(connection, ArticlesCheckQuery.AccountEmployees, pCompanyId, pStartingDate, pEndingDate);
                employeeIds = employees.Select(x => x.ID_EMP).ToList();

                var article2 = new ArticlesCheckDto();
                if (employeeIds.Any())
                {
                    var query = ArticlesCheckQuery.Accounts.Replace("@@Employees", string.Join(", ", employeeIds));
                    var accountList = await _dataProvider
                        .QueryAsync<ArticlesCheckAccount>(connection, query, pNgl, pCompanyId, pYear, pStartingDate, pEndingDate);

                    article2.Fmy = accountList.Where(x => x.LOGARIASMOS.IsLike(fmyPatterns)).Sum(x => x.PISTOSI);
                    article2.Salary = accountList.Where(x => x.LOGARIASMOS.IsLike(salaryPatterns)).Sum(x => x.PISTOSI);
                    article2.Total = accountList.Sum(x => x.PISTOSI);
                    article2.Efka = accountList.Where(x => x.LOGARIASMOS.IsLike(efkaPatterns)).Sum(x => x.PISTOSI);
                    article2.Teka = accountList.Where(x => x.LOGARIASMOS.IsLike(tekaPatterns)).Sum(x => x.PISTOSI);
                }

                var model1 = new ArticlesCheckModel
                {
                    Fmy = article1.Fmy,
                    Salary = article1.Salary,
                    Total = article1.Total,
                    Efka = article1.Efka,
                    Teka = article1.Teka
                };
                var model2 = new ArticlesCheckModel
                {
                    Fmy = article2.Fmy,
                    Salary = article2.Salary,
                    Total = article2.Total,
                    Efka = article2.Efka,
                    Teka = article2.Teka
                };
                var areEqual = AreEqual(article1, article2, model1);

                model1.EmployerId = model2.EmployerId = employer.CompanyId;
                model1.Employer = model2.Employer = employer.FullName();
                model1.ErrorMessage = areEqual ? "Μισθ.Κατ." : $"Μισθ.Κατ.{errorMessage}";
                model2.ErrorMessage = areEqual ? "Αρθ.Λογ." : $"Αρθ.Λογ.{errorMessage}";

                model1.Items.Add(model2);
                models.Add(model1);
            }

            return models;
        }

        public virtual async Task<ArticlesCheckTableModel> PrepareArticlesCheckTableModelAsync(ArticlesCheckTableModel tableModel)
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var textAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ArticlesCheckModel>(1, nameof(ArticlesCheckModel.Employer)),
                ColumnConfig.Create<ArticlesCheckModel>(1, nameof(ArticlesCheckModel.ErrorMessage)),
                ColumnConfig.Create<ArticlesCheckModel>(1, nameof(ArticlesCheckModel.Total), ColumnType.Decimal, style: textAlign, headerStyle: centerAlign),
                ColumnConfig.Create<ArticlesCheckModel>(1, nameof(ArticlesCheckModel.Salary), ColumnType.Decimal, style: textAlign, headerStyle: centerAlign),
                ColumnConfig.Create<ArticlesCheckModel>(1, nameof(ArticlesCheckModel.Efka), ColumnType.Decimal, style: textAlign, headerStyle: centerAlign),
                ColumnConfig.Create<ArticlesCheckModel>(1, nameof(ArticlesCheckModel.Teka), ColumnType.Decimal, style: textAlign, headerStyle: centerAlign),
                ColumnConfig.Create<ArticlesCheckModel>(1, nameof(ArticlesCheckModel.Fmy), ColumnType.Decimal, style: textAlign, headerStyle: centerAlign),
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ArticlesCheckModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }

        private static bool IsValid(string s)
        {
            // 2.2.2.4.4 or 2.2.2.4.4.4
            return Regex.IsMatch(s, @"^\d{2}\.\d{2}\.\d{2}\.\d{4}\.\d{4}(?:\.\d{4})?$");
        }
        public virtual async Task<IList<ArticlesCheckAccountModel>> PrepareArticlesCheckAcountListAsync(string softOneCon, string connection, int companyId, int nglId, int year, int period)
        {
            var firstDateOfMonth = new DateTime(year, period, 1);
            var lastDateOfMonth = firstDateOfMonth.AddMonths(1).AddDays(-1);

            var pNgl = new LinqToDB.Data.DataParameter("pNgl", nglId);
            var pYear = new LinqToDB.Data.DataParameter("pYear", year);
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
            var pStartingDate = new LinqToDB.Data.DataParameter("pStartingDate",
                firstDateOfMonth.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
            var pEndingDate = new LinqToDB.Data.DataParameter("pEndingDate",
                lastDateOfMonth.ToString("yyyyMMdd", CultureInfo.InvariantCulture));

            var employees = await _dataProvider
                .QueryAsync<ArticlesCheckEmployeeId>(connection, ArticlesCheckQuery.AccountEmployees, pCompanyId, pYear, pStartingDate, pEndingDate);
            var employeeIds = employees.Select(x => x.ID_EMP).ToList();

            if (employeeIds.Any())
            {
                var query = ArticlesCheckQuery.Accounts.Replace("@@Employees", string.Join(", ", employeeIds));
                var accounts = await _dataProvider
                    .QueryAsync<ArticlesCheckAccount>(connection, query, pNgl, pCompanyId, pYear, pStartingDate, pEndingDate);

                var accountList = accounts
                    .GroupBy(r => r.LOGARIASMOS)
                    .Select(g => new ArticlesCheckAccountModel
                    {
                        Code = g.Key,
                        NglId = g.First().NGL_GROUP1,
                        NglName = g.First().NGL_GROUP2,
                        Description = g.First().LOGARIASMOS_DSC,
                        Debit = g.Sum(x => x.XREOSI),
                        Credit = g.Sum(x => x.PISTOSI),
                        Date = lastDateOfMonth.ToString("dd/MM/yyyy")
                    })
                    .ToList();

                if (!string.IsNullOrEmpty(softOneCon) && accountList.Any())
                {
                    var codeList = accountList.Select(x => $"'{x.Code}'").ToList();
                    var codeQuery = ArticlesCheckQuery.AccountingCodes.Replace("@@Codes", string.Join(", ", codeList));
                    var codes = (await _dataProvider
                        .QueryAsync<ArticlesCheckCodeModel>(softOneCon, codeQuery, pYear))
                        .Select(x => x.AccountingCode).ToList();

                    foreach (var account in accountList)
                    {
                        account.FormatValid = IsValid(account.Code);
                        account.SchemaValid = codes.Contains(account.Code);
                    }
                }

                var xreosiList = accountList.Where(x => x.Code.IsLike(new[] { "6*" })).ToList();
                xreosiList.ForEach(x => x.Group = "5. Λογαριασμοί χρέωσης");

                var loansList = accountList.Where(x => x.Code.IsLike(new[] { "32*" })).ToList();
                loansList.ForEach(x => x.Group = "6. Δάνεια");

                var fmyList = accountList.Where(x => x.Code.IsLike(fmyPatterns)).ToList();
                fmyList.ForEach(x => x.Group = "4. ΦΜΥ");
                var salaryList = accountList.Where(x => x.Code.IsLike(salaryPatterns)).ToList();
                salaryList.ForEach(x => x.Group = "1. Πληρωτέο ποσό");
                var efkaList = accountList.Where(x => x.Code.IsLike(efkaPatterns)).ToList();
                efkaList.ForEach(x => x.Group = "2. ΕΦΚΑ");
                var tekaList = accountList.Where(x => x.Code.IsLike(tekaPatterns)).ToList();
                tekaList.ForEach(x => x.Group = "3. ΤΕΚΑ");

                return fmyList
                    .Concat(salaryList).ToList()
                    .Concat(efkaList).ToList()
                    .Concat(tekaList).ToList()
                    .Concat(xreosiList).ToList()
                    .Concat(loansList).ToList();
            }

            return new List<ArticlesCheckAccountModel>();
        }

        public virtual async Task<ArticlesCheckAccountTableModel> PrepareArticlesCheckAccountTableModelAsync(ArticlesCheckAccountTableModel tableModel)
        {
            var textAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<ArticlesCheckAccountModel>(1, nameof(ArticlesCheckAccountModel.Code)),
                ColumnConfig.Create<ArticlesCheckAccountModel>(2, nameof(ArticlesCheckAccountModel.Description)),
                ColumnConfig.Create<ArticlesCheckAccountModel>(3, nameof(ArticlesCheckAccountModel.Debit), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<ArticlesCheckAccountModel>(3, nameof(ArticlesCheckAccountModel.Credit), ColumnType.Decimal, style: textAlign),
                ColumnConfig.Create<ArticlesCheckAccountModel>(4, nameof(ArticlesCheckAccountModel.Group), hidden: true),
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.ArticlesCheckAccountModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }
    }
}

