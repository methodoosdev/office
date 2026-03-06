using App.Core;
using App.Core.Domain.Payroll;
using App.Core.Domain.Traders;
using App.Core.Infrastructure.Mapper;
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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Accounting
{
    public partial interface IPayrollCheckModelFactory
    {
        Task<PayrollCheckSearchModel> PreparePayrollCheckSearchModelAsync(PayrollCheckSearchModel searchModel);
        Task<PayrollCheckSearchFormModel> PreparePayrollCheckSearchFormModelAsync(PayrollCheckSearchFormModel searchFormModel, int employeeId);
        Task<IList<PayrollCheckModel>> PreparePayrollCheckModelListAsync(string connection, PayrollCheckSearchModel searchModel);
        Task<PayrollCheckTableModel> PreparePayrollCheckTableModelAsync(PayrollCheckTableModel tableModel);
    }
    public partial class PayrollCheckModelFactory : IPayrollCheckModelFactory
    {
        private readonly ITraderService _traderService;
        private readonly ITraderConnectionService _traderConnectionService;
        private readonly ITraderEmployeeMappingService _traderEmployeeMappingService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;

        public PayrollCheckModelFactory(
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

        public virtual Task<PayrollCheckSearchModel> PreparePayrollCheckSearchModelAsync(PayrollCheckSearchModel searchModel)
        {
            searchModel.Periodos = DateTime.UtcNow.AddYears(-1).Date;

            return Task.FromResult(searchModel);
        }

        public virtual async Task<PayrollCheckSearchFormModel> PreparePayrollCheckSearchFormModelAsync(PayrollCheckSearchFormModel formModel, int employeeId)
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

            var left = new List<Dictionary<string, object>>()
            {
                await _fieldConfigService.GetEmployersMultiColumnComboBox<PayrollCheckSearchModel>(
                    nameof(PayrollCheckSearchModel.EmployerIds))
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<PayrollCheckSearchModel>(nameof(PayrollCheckSearchModel.EmployeeId), FieldType.GridSelect, options: employees),
                FieldConfig.Create<PayrollCheckSearchModel>(nameof(PayrollCheckSearchModel.Periodos), FieldType.YearDate)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6" }, left, right);

            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }

        private bool AreEqual(PayrollCheckDto dto1, PayrollCheckDto dto2, PayrollCheckModel item)
        {
            var result = new List<bool>();
            if (dto1 == null || dto2 == null) return false;

            PropertyInfo[] properties = typeof(PayrollCheckDto).GetProperties();

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

        private class _AccountingCode
        {
            public string AccountingCode { get; set; }
            public string Description { get; set; }
            public int Type { get; set; }
            public int Schema { get; set; }
            public int Grande { get; set; }
            public int Tranfer { get; set; }
            public int Moving { get; set; }
        }

        public virtual async Task<IList<PayrollCheckModel>> PreparePayrollCheckModelListAsync(string connection, PayrollCheckSearchModel searchModel)
        {
            var models = new List<PayrollCheckModel>();
            var employers = await _dataProvider.QueryAsync<EmployerLookupItem>(connection, PayrollQuery.EmployerLookupItem);

            var companyIds = searchModel.EmployerIds.ToList();
            if (companyIds.Count > 0)
                employers = employers.Where(x => companyIds.Contains(x.CompanyId)).ToList();

            foreach (var employer in employers)
            {
                var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", employer.CompanyId);
                var pYear = new LinqToDB.Data.DataParameter("pYear", searchModel.Periodos.Year);

                var hyperResults = await _dataProvider.QueryAsync<PayrollCheckDto>(connection, PayrollCheckQuery.Employer, pCompanyId, pYear);
                if (hyperResults.Count == 0) // if this year not exist
                    continue;

                var hyperDto = hyperResults.First();
                var traderDto = new PayrollCheckDto();

                var trader = await _traderService.GetTraderByVatAsync(employer.Vat);
                if (trader != null && (trader.Active || !trader.Deleted))
                {
                    var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(trader.Id);
                    if (connectionResult.Success)
                    {
                        try
                        {
                            pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", trader.CompanyId);
                            var traderResults = await _dataProvider.QueryAsync<PayrollCheckDto>(connectionResult.Connection,
                                new PayrollCheckQuery(connectionResult.LogistikiProgramTypeId, connectionResult.CategoryBookTypeId).Get(),
                                pCompanyId, pYear);

                            if (traderResults.Count > 0)
                                traderDto = traderResults.First();
                        }
                        catch { }
                    }
                }

                var hyperModel = AutoMapperConfiguration.Mapper.Map<PayrollCheckModel>(hyperDto);
                var traderModel = AutoMapperConfiguration.Mapper.Map<PayrollCheckModel>(traderDto);
                var areEqual = AreEqual(hyperDto, traderDto, hyperModel);

                hyperModel.ErrorMessage = areEqual ? "" : await _localizationService.GetResourceAsync("App.Common.Error");
                hyperModel.TraderName = employer.FullName();
                traderModel.ErrorMessage = areEqual ? "" :trader?.LogistikiProgramType.ToString() ?? "";
                traderModel.TraderName = employer.FullName();

                hyperModel.Items.Add(traderModel);
                models.Add(hyperModel);
            }

            return models;
        }

        public virtual async Task<PayrollCheckTableModel> PreparePayrollCheckTableModelAsync(PayrollCheckTableModel tableModel)
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var textAlign = new Dictionary<string, string> { ["text-align"] = "right" };
            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<PayrollCheckModel>(1, nameof(PayrollCheckModel.TraderName)),
                ColumnConfig.Create<PayrollCheckModel>(1, nameof(PayrollCheckModel.ErrorMessage)),
                ColumnConfig.Create<PayrollCheckModel>(1, nameof(PayrollCheckModel.Jan), ColumnType.Decimal, style: textAlign, headerStyle: centerAlign),
                ColumnConfig.Create<PayrollCheckModel>(1, nameof(PayrollCheckModel.Feb), ColumnType.Decimal, style: textAlign, headerStyle: centerAlign),
                ColumnConfig.Create<PayrollCheckModel>(1, nameof(PayrollCheckModel.Mar), ColumnType.Decimal, style: textAlign, headerStyle: centerAlign),
                ColumnConfig.Create<PayrollCheckModel>(1, nameof(PayrollCheckModel.Apr), ColumnType.Decimal, style: textAlign, headerStyle: centerAlign),
                ColumnConfig.Create<PayrollCheckModel>(1, nameof(PayrollCheckModel.May), ColumnType.Decimal, style: textAlign, headerStyle: centerAlign),
                ColumnConfig.Create<PayrollCheckModel>(1, nameof(PayrollCheckModel.Jun), ColumnType.Decimal, style: textAlign, headerStyle: centerAlign),
                ColumnConfig.Create<PayrollCheckModel>(1, nameof(PayrollCheckModel.Jul), ColumnType.Decimal, style: textAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayrollCheckModel>(1, nameof(PayrollCheckModel.Aug), ColumnType.Decimal, style: textAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayrollCheckModel>(1, nameof(PayrollCheckModel.Sep), ColumnType.Decimal, style: textAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayrollCheckModel>(1, nameof(PayrollCheckModel.Oct), ColumnType.Decimal, style: textAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayrollCheckModel>(1, nameof(PayrollCheckModel.Nov), ColumnType.Decimal, style: textAlign, headerStyle : centerAlign),
                ColumnConfig.Create<PayrollCheckModel>(1, nameof(PayrollCheckModel.Dec), ColumnType.Decimal, style: textAlign, headerStyle : centerAlign),
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.PayrollCheckModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }
    }
}

