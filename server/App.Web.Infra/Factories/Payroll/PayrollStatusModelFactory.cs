using App.Core;
using App.Core.Domain.Traders;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Services;
using App.Services.Configuration;
using App.Services.Localization;
using App.Web.Common.Models.Payroll;
using App.Web.Infra.Queries.Payroll;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Payroll
{
    public partial interface IPayrollStatusModelFactory
    {
        Task<PayrollStatusSearchModel> PreparePayrollStatusSearchModelAsync(PayrollStatusSearchModel searchModel);
        Task<IList<PayrollStatusModel>> PreparePayrollStatusListModelAsync(PayrollStatusSearchModel searchModel, Trader trader, string connection);
        Task<PayrollStatusTableModel> PreparePayrollStatusTableModelAsync(PayrollStatusTableModel tableModel);

    }

    public partial class PayrollStatusModelFactory : IPayrollStatusModelFactory
    {
        private readonly ILocalizationService _localizationService;
        private readonly IAppDataProvider _dataProvider;
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IWorkContext _workContext;

        public PayrollStatusModelFactory(
            ILocalizationService localizationService,
            IAppDataProvider dataProvider,
            IFieldConfigService fieldConfigService,
            IWorkContext workContext
            )
        {
            _localizationService = localizationService;
            _dataProvider = dataProvider;
            _fieldConfigService = fieldConfigService;
            _workContext = workContext;
        }

        public async Task<PayrollStatusSearchModel> PreparePayrollStatusSearchModelAsync(PayrollStatusSearchModel searchModel)
        {
            var trader = await _workContext.GetCurrentTraderAsync();

            var periodOptions = await PayrollStatusType.Day.ToSelectionItemListAsync();
            searchModel.TraderId = trader?.Id ?? 0;

            var left = new List<Dictionary<string, object>>()
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<PayrollStatusSearchModel>(nameof(PayrollStatusSearchModel.TraderId), FieldConfigType.Payroll)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<PayrollStatusSearchModel>(nameof(PayrollStatusSearchModel.PayrollStatusTypeId), FieldType.Select, options: periodOptions, className: "col-12 md:col-6"),
            };

            //var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12" }, left, right);

            var fields = trader != null
                ? FieldConfig.CreateFields(new string[] { "col-12 md:col-3" }, right)
                : FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-3" }, left, right);

            searchModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));
            searchModel.PayrollStatusTypeId = 1;

            return searchModel;
        }

        public async Task<IList<PayrollStatusModel>> PreparePayrollStatusListModelAsync(PayrollStatusSearchModel searchModel, Trader trader, string connection)
        {
            var mode = searchModel.PayrollStatusTypeId;
            int days;

            var companyId = trader.HyperPayrollId;

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);

            var list = await _dataProvider.QueryAsync<PayrollStatusModel>(connection, PayrollStatusInfoQuery.Get, pCompanyId);

            var payrollStatusItems = new List<PayrollStatusModel>();

            foreach (var employee in list)
            {
                var model = new PayrollStatusModel();

                var empoyeeType = employee.EmployeeType == "ΗΜΕΡΟΜΙΣΘΙΟΣ" ? 1 : employee.EmployeeType == "ΕΜΜΙΣΘΟΣ" ? 2 : 0;

                if (empoyeeType == 1)
                {
                    days = 22;

                    // Υπολογισμός αντίστοιχων ΦΜΥ
                    employee.W_Fmy = CalcFmy(employee.W_NetSalaryPreTax, employee.Children, days);
                    employee.G_Fmy = CalcFmy(employee.G_NetSalaryPreTax, employee.Children, days);

                    if (mode == 1)
                    {
                        // Ημεήσια αποτελέσματα ημερομίσθιων
                        model = CalculateAmountsByPeriod(employee, days, mode);
                    }
                    else if (mode == 2)
                    {
                        // Μηνιαία αποτελέσματα ημερομίσθιων
                        model = CalculateAmountsByPeriod(employee, days, mode);

                    }
                    else if (mode == 3)
                    {
                        // Ετήσια αποτελέσματα ημερομίσθιων
                        model = CalculateAmountsByPeriod(employee, days, mode);

                    }
                }

                if (empoyeeType == 2)
                {
                    days = 25;

                    // Υπολογισμός αντίστοιχων ΦΜΥ
                    employee.W_Fmy = CalcFmy(employee.W_NetSalaryPreTax, employee.Children, days);
                    employee.G_Fmy = CalcFmy(employee.G_NetSalaryPreTax, employee.Children, days);

                    if (mode == 1)
                    {
                        // Ημεήσια αποτελέσματα μισθωτών
                        model = CalculateAmountsByPeriod(employee, days, mode);

                    }
                    else if (mode == 2)
                    {
                        // Μηνιαία αποτελέσματα μισθωτών
                        model = CalculateAmountsByPeriod(employee, days, mode);
                    }
                    else if (mode == 3)
                    {
                        // Ετήσια αποτελέσματα μισθωτών
                        model = CalculateAmountsByPeriod(employee, days, mode);
                    }
                }
                payrollStatusItems.Add(model);
            }

            return payrollStatusItems;
        }

        public async Task<PayrollStatusTableModel> PreparePayrollStatusTableModelAsync(PayrollStatusTableModel tableModel)
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns1 = new List<ColumnConfig>()
            {
                ColumnConfig.Create<PayrollStatusModel>(1, nameof(PayrollStatusModel.Employee), sticky: true),
                ColumnConfig.Create<PayrollStatusModel>(2, nameof(PayrollStatusModel.HireDate), ColumnType.Date, style: centerAlign),
                ColumnConfig.Create<PayrollStatusModel>(3, nameof(PayrollStatusModel.Specialty)),
                ColumnConfig.Create<PayrollStatusModel>(4, nameof(PayrollStatusModel.EmployeeType)),
                ColumnConfig.Create<PayrollStatusModel>(5, nameof(PayrollStatusModel.ContractType)),
                ColumnConfig.Create<PayrollStatusModel>(6, nameof(PayrollStatusModel.FixedContractStartDate), ColumnType.Date, style: centerAlign),
                ColumnConfig.Create<PayrollStatusModel>(7, nameof(PayrollStatusModel.FixedContractEndDate), ColumnType.Date, style: centerAlign),

            };

            // columns for agreed salary
            var columns2 = new List<ColumnConfig>()
            {
                ColumnConfig.Create<PayrollStatusModel>(8, nameof(PayrollStatusModel.W_HourlyWages), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<PayrollStatusModel>(9, nameof(PayrollStatusModel.W_Salary), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<PayrollStatusModel>(10, nameof(PayrollStatusModel.W_EmployeeContribution), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<PayrollStatusModel>(11, nameof(PayrollStatusModel.W_EmployerContribution), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<PayrollStatusModel>(12, nameof(PayrollStatusModel.W_Fmy), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<PayrollStatusModel>(13, nameof(PayrollStatusModel.W_NetSalary), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<PayrollStatusModel>(14, nameof(PayrollStatusModel.W_NetSalaryPreTax), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<PayrollStatusModel>(15, nameof(PayrollStatusModel.W_Cost), ColumnType.Decimal, style: rightAlign),

            };

            // columns for legal salary
            var columns3 = new List<ColumnConfig>()
            {
                ColumnConfig.Create<PayrollStatusModel>(16, nameof(PayrollStatusModel.G_HourlyWages), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<PayrollStatusModel>(17, nameof(PayrollStatusModel.G_Salary), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<PayrollStatusModel>(18, nameof(PayrollStatusModel.G_EmployeeContribution), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<PayrollStatusModel>(19, nameof(PayrollStatusModel.G_EmployerContribution), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<PayrollStatusModel>(20, nameof(PayrollStatusModel.G_Fmy), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<PayrollStatusModel>(21, nameof(PayrollStatusModel.G_NetSalary), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<PayrollStatusModel>(22, nameof(PayrollStatusModel.G_NetSalaryPreTax), ColumnType.Decimal, style: rightAlign),
                ColumnConfig.Create<PayrollStatusModel>(23, nameof(PayrollStatusModel.G_Cost), ColumnType.Decimal, style: rightAlign),
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.PayrollStatusModel.Title"));
            tableModel.CustomProperties.Add("columns1", columns1);
            tableModel.CustomProperties.Add("columns2", columns2);
            tableModel.CustomProperties.Add("columns3", columns3);

            return tableModel;
        }

        private PayrollStatusModel CalculateAmountsByPeriod(PayrollStatusModel model, int days, int mode)
        {
            int multiplier = 1;
            decimal fmyMultiplier = 1.0m;

            // Επιλογή του πολλαπλασιαστή ανάλογα αν ο χρήστης επέλεξε μέρα,μήνα,χρόνο
            switch (mode)
            {
                case 1:
                    fmyMultiplier = (1.0m / 14) * (1.0m / days);
                    break;
                case 2:
                    multiplier = days;
                    fmyMultiplier = 1.0m / 14;
                    break;
                case 3:
                    multiplier = days * 12;
                    break;
            }

            var result = new PayrollStatusModel();

            result.Employee = model.Employee;
            result.HireDate = model.HireDate;
            result.Specialty = model.Specialty;
            result.EmployeeType = model.EmployeeType;
            result.ContractType = model.ContractType;
            result.FixedContractStartDate = model.FixedContractStartDate;
            result.FixedContractEndDate = model.FixedContractEndDate;

            // Συμφωνηθέντα με δώρα και επιδόματα - Agreed with gifts
            result.W_HourlyWages = model.W_HourlyWages;
            result.W_Salary = model.W_Salary * multiplier;
            result.W_EmployeeContribution = model.W_EmployeeContribution * multiplier;
            result.W_EmployerContribution = model.W_EmployerContribution * multiplier;
            result.W_Fmy = model.W_Fmy * fmyMultiplier;
            result.W_NetSalaryPreTax = model.W_NetSalaryPreTax * multiplier;
            result.W_NetSalary = result.W_NetSalaryPreTax - result.W_Fmy;
            result.W_Cost = model.W_Cost * multiplier + result.W_Fmy;   

            // Νόμιμα με δώρα και επιδόματα - Legal with gifts
            result.G_HourlyWages = model.G_HourlyWages;
            result.G_Salary = model.G_Salary * multiplier;
            result.G_EmployeeContribution = model.G_EmployeeContribution * multiplier;
            result.G_EmployerContribution = model.G_EmployerContribution * multiplier;
            result.G_Fmy = model.G_Fmy * fmyMultiplier;
            result.G_NetSalaryPreTax = model.G_NetSalaryPreTax * multiplier;
            result.G_NetSalary = result.G_NetSalaryPreTax - result.G_Fmy;
            result.G_Cost = model.G_Cost * multiplier + result.W_Fmy;

            return result;
        }

        public decimal CalcFmy(decimal salaryPreTax, int children, int days)
        {
            // Προβλεπόμενη έκπτωση ανάλογα τον αρθιμό των παιδιών
            var discount = 0;

            switch (children)
            {
                case 0:
                    discount = 777;
                    break;
                case 1:
                    discount = 900;
                    break;
                case 2:
                    discount = 1120;
                    break;
                case 3:
                    discount = 1340;
                    break;
                case 5:
                    discount = 1780;
                    break;
                default:
                    discount = 1780 + (children - 5) * 220;
                    break;
            }

            decimal totalTax = 0m;
            decimal totalDiscount = 0m;
            decimal fmy = 0m;

            // Υπολογισμός μηνιαίου καθαρού εισοδήματος προ φόρου 
            var monthlySalaryPreTax = days * salaryPreTax;

            // Υπολογισμός ετήσιου καθαρού εισοδήματος προ φόρου 
            var yearlySalaryPreTax = monthlySalaryPreTax * 14;

            // Υπολογισμός έξτρα μείωσης φόρου , +20 ευρω για κάθε 1000 πάνω από τις 12000
            var discountCounter = (int)(yearlySalaryPreTax - 12000) / 1000;
            var discountReduction = discountCounter * 20;

            // Υπολογισμός ετήσιου ΦΜΥ για 14 μισθούς
            if (yearlySalaryPreTax <= 10000)
            {
                fmy = yearlySalaryPreTax * (9.0m / 100) - discount;
            }
            else if (10001 <= yearlySalaryPreTax && yearlySalaryPreTax <= 20000)
            {
                totalTax = 900 + (yearlySalaryPreTax - 10000) * (22.0m / 100);

                // Μέχρι τα 12000€ ισχύει η παραπάνω έκπτωση
                if (yearlySalaryPreTax <= 12000)
                {
                    totalDiscount = discount;
                }
                else
                {
                    //totalDiscount = discount - (yearlySalaryPreTax - 12000) * (2.0m / 100);
                    totalDiscount = discount - discountReduction;
                }

                fmy = totalTax - totalDiscount;
            }
            else if (20001 <= yearlySalaryPreTax && yearlySalaryPreTax <= 30000)
            {
                totalTax = 900 + 2200 + (yearlySalaryPreTax - 20000) * (28.0m / 100);
                //totalDiscount = discount - (yearlySalaryPreTax - 12000) * (2.0m / 100);
                totalDiscount = discount - discountReduction;
                fmy = totalTax - totalDiscount;
            }
            else if (30001 <= yearlySalaryPreTax && yearlySalaryPreTax <= 40000)
            {
                totalTax = 900 + 2200 + 2800 + (yearlySalaryPreTax - 30000) * (36.0m / 100);
                //totalDiscount = discount - (yearlySalaryPreTax - 12000) * (2.0m / 100);
                totalDiscount = discount - discountReduction;
                fmy = totalTax - totalDiscount;
            }
            else
            {
                totalTax = 900 + 2200 + 2800 + 3600 + (yearlySalaryPreTax - 40000) * (44.0m / 100);
                //totalDiscount = discount - (yearlySalaryPreTax - 12000) * (2.0m / 100);
                totalDiscount = discount - discountReduction;
                fmy = totalTax - totalDiscount;
            }

            if (fmy < 0)
                fmy = 0;

            return fmy;
        }

    }
}
