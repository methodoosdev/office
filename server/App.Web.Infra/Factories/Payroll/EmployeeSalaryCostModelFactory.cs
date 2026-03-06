using App.Core;
using App.Core.Domain.Payroll;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Payroll;
using App.Services.Configuration;
using App.Services.Localization;
using App.Web.Infra.Queries.Payroll;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Payroll
{
    public partial interface IEmployeeSalaryCostModelFactory
    {
        Task<EmployeeSalaryCostSearchModel> PrepareEmployeeSalaryCostSearchModelAsync(EmployeeSalaryCostSearchModel searchModel);
        Task<EmployeeSalaryCostSearchFormModel> PrepareEmployeeSalaryCostSearchFormModelAsync(EmployeeSalaryCostSearchFormModel formModel, string connection, int? companyId, bool allPackages);
        Task<EmployeeSalaryCostModel> PrepareEmployeeSalaryCostModelAsync(EmployeeSalaryCostSearchModel model, string connection);
        Task<EmployeeSalaryCostFormModel> PrepareEmployeeSalaryCostResultFormModelAsync(EmployeeSalaryCostFormModel formModel);
        Task<EmployeeSalaryCostFormModel> PrepareEmployeeSalaryCostFormModelAsync(EmployeeSalaryCostFormModel formModel);
        Task<IList<SelectionItemList>> PrepareInsurancePackagesAsync(EmployeeSalaryCostSearchModel model, string connection);
    }

    public partial class EmployeeSalaryCostModelFactory : IEmployeeSalaryCostModelFactory
    {
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        public EmployeeSalaryCostModelFactory(
            IFieldConfigService fieldConfigService,
            IAppDataProvider dataProvider,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _fieldConfigService = fieldConfigService;
            _dataProvider = dataProvider;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public virtual async Task<EmployeeSalaryCostSearchModel> PrepareEmployeeSalaryCostSearchModelAsync(EmployeeSalaryCostSearchModel searchModel)
        {
            var trader = await _workContext.GetCurrentTraderAsync();
            searchModel.CompanyId = trader?.HyperPayrollId ?? null;
            searchModel.NetAmountPayable = 1000;

            return searchModel;
        }

        public virtual async Task<EmployeeSalaryCostSearchFormModel> PrepareEmployeeSalaryCostSearchFormModelAsync(EmployeeSalaryCostSearchFormModel formModel, string connection, int? companyId, bool allPackages)
        {
            var top = new List<Dictionary<string, object>>();
            var bottom = new List<Dictionary<string, object>>();
            var packages = new List<SelectionItemList>();

            if (!companyId.HasValue)
            {
                var _employers = await _dataProvider.QueryAsync<EmployerLookupItem>(connection, PayrollQuery.EmployerLookupItem);
                var employers = _employers.Select(x => new SelectionItemList { Label = $"{x.FullName()} - {x.Vat}", Value = x.CompanyId }).ToList();
                var companyIdField = FieldConfig.Create<EmployeeSalaryCostSearchModel>(nameof(EmployeeSalaryCostSearchModel.CompanyId), FieldType.GridSelect, options: employers);
                top.Add(companyIdField);
            }
            else
            {
                var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId.Value);

                var _packages = await _dataProvider.QueryAsync<InsurancePackageModel>(connection, PayrollQuery.InsurancePackages, pCompanyId);
                packages = _packages.Select((x, i) => new SelectionItemList { Label = $"({x.Employee.ToString("#0.##")}-{x.Employer.ToString("#0.##")}) {x.Description}", Value = i + 1 }).ToList();
                //packages.Insert(0, new SelectionItemList { Label = await _localizationService.GetResourceAsync("App.Common.Choice"), Value = 0 });
            }

            var insurancePackageIdField = FieldConfig.Create<EmployeeSalaryCostSearchModel>(nameof(EmployeeSalaryCostSearchModel.InsurancePackageId), FieldType.GridSelect, options: packages, disableExpression: "!(model.companyId > 0)");
            bottom.Add(insurancePackageIdField);

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmployeeSalaryCostSearchModel>(nameof(EmployeeSalaryCostSearchModel.Undeclared), FieldType.Decimals),
                FieldConfig.Create<EmployeeSalaryCostSearchModel>(nameof(EmployeeSalaryCostSearchModel.AllPackages), FieldType.Checkbox, disableExpression: "!(model.companyId > 0)")
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmployeeSalaryCostSearchModel>(nameof(EmployeeSalaryCostSearchModel.NetAmountPayable), FieldType.Decimals),
                FieldConfig.Create<EmployeeSalaryCostSearchModel>(nameof(EmployeeSalaryCostSearchModel.NumberOfChildren), FieldType.Numeric),
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12", "col-12", "col-12 md:col-6", "col-12 md:col-6" }, top, bottom, left, right);

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.EmployeeSalaryCostModel.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }

        public virtual async Task<IList<SelectionItemList>> PrepareInsurancePackagesAsync(EmployeeSalaryCostSearchModel model, string connection)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", model.CompanyId.Value);
            
            var _packages = model.AllPackages
                ? await _dataProvider.QueryAsync<InsurancePackageModel>(connection, PayrollQuery.InsurancePackagesAll)
                : await _dataProvider.QueryAsync<InsurancePackageModel>(connection, PayrollQuery.InsurancePackages, pCompanyId);
            var packages = _packages.Select((x, i) => new SelectionItemList { Label = $"({x.Employee.ToString("#0.##")}-{x.Employer.ToString("#0.##")}) {x.Description}", Value = i + 1 }).ToList();
            //packages.Insert(0, new SelectionItemList { Label = await _localizationService.GetResourceAsync("App.Common.Choice"), Value = 0 });
            
            return packages;
        }

        public virtual Task<EmployeeSalaryCostFormModel> PrepareEmployeeSalaryCostResultFormModelAsync(EmployeeSalaryCostFormModel formModel)
        {
            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.CompanyCost), FieldType.Decimals, _readonly: true, className: "text-bold"),
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.FinalPayable), FieldType.Decimals, _readonly: true, className: "text-bold"),
            };

            var right1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.CompanyCost), FieldType.Decimals, _readonly: true, className: "col-12 md:col-6 text-bold"),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.FinalPayable), FieldType.Decimals, _readonly: true, className: "col-12 md:col-6 text-bold"),
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6" }, left, right);

            //formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, className: "col-12"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(right1, className: "col-12"));

            return Task.FromResult(formModel);
        }

        public virtual Task<EmployeeSalaryCostFormModel> PrepareEmployeeSalaryCostFormModelAsync(EmployeeSalaryCostFormModel formModel)
        {
            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.NetAmountPayable), FieldType.Decimals, _readonly: true),
                FieldConfig.CreateDivider(),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.EmployeeContributionsRate), FieldType.Decimals, _readonly: true, format: "#0.00 '%'"),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.EmployerContributionsRate), FieldType.Decimals, _readonly: true, format: "#0.00 '%'"),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.TotalContributionsRate), FieldType.Decimals, _readonly: true, format: "#0.00 '%'"),
                FieldConfig.CreateDivider(),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.AnualIncome), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.TaxIncome), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.TaxDeduction), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.TaxDeductionMinus), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.FinalTax), FieldType.Decimals, _readonly: true)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.MixedEarnings), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.EmployeeContributions), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.EmployerContributions), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.TotalContributions), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.NetTaxable), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.Fmy), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.CompanyCost), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.UndeclaredCost), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.UndeclaredBenefit), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.TaxDifference), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.ChargeUndeclared), FieldType.Decimals, _readonly: true),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.ChargeUndeclaredPayee), FieldType.Decimals, _readonly: true, format: "#0.00 '%'"),
                FieldConfig.Create<EmployeeSalaryCostModel>(nameof(EmployeeSalaryCostModel.FinalPayable), FieldType.Decimals, _readonly: true)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6" }, left, right);

            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, className: "col-12"));

            return Task.FromResult(formModel);
        }

        public virtual async Task<EmployeeSalaryCostModel> PrepareEmployeeSalaryCostModelAsync(EmployeeSalaryCostSearchModel model, string connection)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", model.CompanyId);
            var packages = model.AllPackages
                ? await _dataProvider.QueryAsync<InsurancePackageModel>(connection, PayrollQuery.InsurancePackagesAll)
                : await _dataProvider.QueryAsync<InsurancePackageModel>(connection, PayrollQuery.InsurancePackages, pCompanyId);
            var i = 1;
            packages.ForEach(x => x.Id = i++);

            var package = packages.FirstOrDefault(x => x.Id == model.InsurancePackageId);

            // Εισφορές Εργαζόμενου ποσοστό
            decimal employeeContributionsRate = package.Employee;
            // Εισφορές Εργοδότη ποσοστό
            decimal employerContributionsRate = package.Employer;
            // Σύνολο κρατήσεων ποσοστό
            decimal totalContributionsRate = (package.Employee) + (package.Employer);

            model.NetAmountPayable = model.NetAmountPayable < 350 ? 350 : model.NetAmountPayable;
            // Μεικτές αποδοχές
            var mixedEarnings = FindGrossIncome(model.NetAmountPayable, employeeContributionsRate, model.NumberOfChildren);

            /*
            1. Υπολογισμός καθαρών αποδοχών: 
            ακαθάριστες μηνιαίες αποδοχές μείον κρατήσεις ΕΦΚΑ : 
            δηλαδή 1.500,00 – (1.500,00 Χ 14,12%) = 1.500,00 – 211,80 = 1.288,20 ευρώ. 
            
            2. Αναγωγή σε ετήσιο εισόδημα: 1.288,20 Χ 14 = 18.034,80 ευρώ. 
            
            3. Τα πρώτα 10.000 ευρώ φορολογούνται με 9%,άρα 900 ευρώ ο φόρος. 
            Τα υπόλοιπα 8.034,80 ευρώ φορολογούνται με 22%, 
            δηλαδή 1.767,66 ευρώ. Οπότε ο συνολικός φόρος 900,00 ευρώ + 1767,66 ευρώ = 2.667,66 ευρώ. 
            4. Έκπτωση φόρου: 656,30,00 ευρώ 
            (μέχρι τα 12.000 ευρώ η μείωση φόρου είναι 777,00 ευρώ. Για τα υπόλοιπα 18.034,80 – 12.000,00= 6.034,80. 
            Δηλαδή 6034,80 χ2%= 120,69. 
            Άρα τελική μείωση φόρου 777,00 – 120,69= 656,31 Άρα τελικός φόρος είναι : 2.667,66 ευρώ – 656,31 ευρώ = 2011,35 ευρώ. 
             
            5. Το μήνα παρακρατείται 2.011,35 /14 = 143,67 ευρώ. 
            
            Όπως είπαμε το ετήσιο καθαρό εισόδημα είναι : 18.034,80 ευρώ. 
            Φόρος 2.667,66 
             
            Μείωση φόρου με δυο τέκνα : ως 12.000,00 ευρώ είναι 900,00 ευρώ. 
            Για το υπερβάλλον εισόδημα από τα 12.000,00 ευρώ και πάνω δηλαδή για τα υπόλοιπα 6.034,80 ευρώ 
            έχουμε έκπτωση 2% άρα (6.034,80 Χ 2%=120,69),  οπότε 900,00 -120,69 = 779,31 ευρώ. 
            
            Άρα ο φόρος εισοδήματος είναι : 2.667,66ευρώ -779,31ευρώ = 1.888,35 ευρώ /14 = 134,88 ευρώ θα παρακρατηθούν
            */

            var employeeContributions = Round(mixedEarnings * employeeContributionsRate);
            var employerContributions = Round(mixedEarnings * employerContributionsRate);
            var totalContributions = Round(employeeContributions + employerContributions);
            var netTaxable = Round(mixedEarnings - employeeContributions);
            var anoualIncome = Round(netTaxable * 14);

            var calc = IncomeTaxCalculation(anoualIncome, model.NumberOfChildren);
            var taxIncome = calc.taxIncome;
            var taxDeduction = calc.taxDeduction;
            var taxDeductionMinus = calc.taxDeductionMinus;
            var tax = calc.tax;

            var fmy = Round(tax / 14);
            var netAmountPayable = Round(netTaxable - fmy);
            var companyCost = Round(netAmountPayable + fmy + totalContributions + model.Undeclared);
            var undeclaredCost = Round(netAmountPayable + model.Undeclared);
            var undeclaredBenefit = Round(companyCost - undeclaredCost);
            var taxDifference = Round((mixedEarnings + employerContributions) * 0.22m);
            var chargeUndeclared = Round(undeclaredBenefit - taxDifference);
            var chargeUndeclaredPayee = Round(chargeUndeclared / undeclaredCost * 100);
            var finalPayable = Round(netAmountPayable + model.Undeclared);

            return new EmployeeSalaryCostModel
            {
                // Πληροφοριακά πεδία
                EmployeeContributionsRate = Round(employeeContributionsRate),
                EmployerContributionsRate = Round(employerContributionsRate),
                TotalContributionsRate = Round(totalContributionsRate),
                AnualIncome = Round(anoualIncome),
                TaxIncome = Round(taxIncome),
                TaxDeduction = Round(taxDeduction),
                TaxDeductionMinus = Round(taxDeductionMinus),
                FinalTax = Round(tax),

                NetAmountPayable = netAmountPayable,
                MixedEarnings = mixedEarnings,
                EmployeeContributions = employeeContributions,
                EmployerContributions = employerContributions,
                TotalContributions = totalContributions,
                NetTaxable = netTaxable,
                Fmy = fmy,
                CompanyCost = companyCost,
                UndeclaredCost = undeclaredCost,
                UndeclaredBenefit = undeclaredBenefit,
                TaxDifference = taxDifference,
                ChargeUndeclared = chargeUndeclared,
                ChargeUndeclaredPayee = chargeUndeclaredPayee,
                FinalPayable = finalPayable
            };
        }

        private (decimal tax, decimal taxIncome, decimal taxDeduction, decimal taxDeductionMinus) IncomeTaxCalculation(decimal anoualIncome, int children)
        {
            // Ο φόρος που του αναλογεί
            var taxIncome = TaxIncome(anoualIncome);
            // Έκπτωση φόρου
            var taxDeduction = TaxDeduction(children);
            taxDeduction = taxDeduction > taxIncome ? 0 : taxDeduction; 
            // Μείωση εκπτωσης φόρου
            var taxDeductionMinus = TaxDeductionMinus(anoualIncome, children);

            // Τελικός φόρος εισοδήματος
            var tax = Round(taxIncome - taxDeduction + taxDeductionMinus);

            return (tax, taxIncome, taxDeduction, taxDeductionMinus);
        }

        private decimal TaxIncome(decimal income)
        {
            if (income <= 10000)
                return income * 0.09m;
            else if (income <= 20000)
                return 10000 * 0.09m + (income - 10000) * 0.22m;
            else if (income <= 30000)
                return 10000 * 0.09m + 10000 * 0.22m + (income - 20000) * 0.28m;
            else if (income <= 40000)
                return 10000 * 0.09m + 10000 * 0.22m + 10000 * 0.28m + (income - 30000) * 0.36m;
            else
                return 10000 * 0.09m + 10000 * 0.22m + 10000 * 0.28m + 10000 * 0.36m + (income - 40000) * 0.44m;
        }

        private decimal TaxDeduction(int children)
        {
            if (children == 0) return 777;
            if (children == 1) return 810;
            if (children == 2) return 900;
            if (children == 3) return 1120;
            if (children == 4) return 1340;

            return 1340 + ((children - 4) * 220);
        }

        private decimal TaxDeductionMinus(decimal declaredIncome, int children)
        {
            if (declaredIncome <= 12000 || children < 1 || children > 4)
                return 0m;

            var income = declaredIncome - 12000;
            var taxPerThousand = Math.Round(income / 1000, 0, MidpointRounding.ToZero);

            return taxPerThousand * 20;
        }

        // Find Gross Income given the Net Amount Payable
        private decimal FindGrossIncome(decimal targetNetAmount, decimal employeeContributionRate, int children = 0)
        {
            decimal lowerBound = targetNetAmount;
            decimal upperBound = targetNetAmount * 5; // Arbitrary multiplier to ensure coverage
            const decimal epsilon = 0.001m; // Tolerance for approximation

            while (upperBound - lowerBound > epsilon)
            {
                decimal mid = (lowerBound + upperBound) / 2;

                // Step 1: Calculate Employee Contributions
                decimal employeeContributions = mid * employeeContributionRate;

                // Step 2: Calculate Net Taxable Income
                decimal netTaxable = mid - employeeContributions;

                // Step 3: Calculate Temporary Income (e) for FMY Calculation
                decimal tempIncomeForFMY = netTaxable * 14;

                // Step 4: Calculate FMY Amount (z)
                decimal fmyAmount = IncomeTaxCalculation(tempIncomeForFMY, children).tax;

                // Step 5: Calculate d
                decimal d = fmyAmount / 14;

                // Step 6: Calculate Net Amount Payable (h)
                decimal netAmountPayable = netTaxable - d;

                // Compare the calculated net amount payable with the target
                if (netAmountPayable > targetNetAmount)
                    upperBound = mid;
                else
                    lowerBound = mid;
            }

            return lowerBound;
        }

        private decimal Round(decimal value, int decimals = 2, MidpointRounding mode = MidpointRounding.AwayFromZero) 
        {
            return Math.Round(value, decimals, mode);
        }
    }
}
