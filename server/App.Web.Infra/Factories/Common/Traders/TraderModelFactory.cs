using App.Core;
using App.Core.Domain.Common;
using App.Core.Domain.Traders;
using App.Core.Infrastructure;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Traders;
using App.Services;
using App.Services.Customers;
using App.Services.Employees;
using App.Services.Localization;
using App.Services.Offices;
using App.Services.Traders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Common.Traders
{
    public partial interface ITraderModelFactory
    {
        Task<TraderSearchModel> PrepareTraderSearchModelAsync(TraderSearchModel searchModel);
        Task<TraderListModel> PrepareTraderListModelAsync(TraderSearchModel searchModel);
        Task<TraderModel> PrepareTraderModelAsync(TraderModel model, Trader trader);
        Task<TraderFormModel> PrepareTraderFormModelAsync(TraderFormModel formModel);
        Task<TraderFilterModel> PrepareTraderFilterModelAsync();
        Task<TraderFilterFormModel> PrepareTraderFilterFormModelAsync(TraderFilterFormModel filterFormModel);
        Task<TraderSearchModel> PrepareTraderDialogSearchModelAsync(TraderSearchModel searchModel);
    }
    public partial class TraderModelFactory : ITraderModelFactory
    {
        private readonly ITraderService _traderService;
        private readonly ITraderKadService _traderKadService;
        private readonly ITraderEmployeeMappingService _traderEmployeeMappingService;
        private readonly IEmployeeService _employeeService;
        private readonly ICustomerService _customerService;
        private readonly ITraderGroupService _traderGroupService;
        private readonly IWorkingAreaService _workingAreaService;
        private readonly IChamberService _chamberService;
        private readonly IModelFactoryService _modelFactoryService;
        private readonly ILocalizationService _localizationService;
        private readonly IPersistStateService _persistStateService;
        private readonly IWorkContext _workContext;

        public TraderModelFactory(ITraderService traderService,
            ITraderKadService traderKadService,
            ITraderEmployeeMappingService traderEmployeeMappingService,
            IEmployeeService employeeService,
            ICustomerService customerService,
            ITraderGroupService traderGroupService,
            IWorkingAreaService workingAreaService,
            IChamberService chamberService,
            IModelFactoryService modelFactoryService,
            ILocalizationService localizationService,
            IPersistStateService persistStateService,
            IWorkContext workContext)
        {
            _traderService = traderService;
            _traderKadService = traderKadService;
            _traderEmployeeMappingService = traderEmployeeMappingService;
            _employeeService = employeeService;
            _customerService = customerService;
            _traderGroupService = traderGroupService;
            _workingAreaService = workingAreaService;
            _chamberService = chamberService;
            _modelFactoryService = modelFactoryService;
            _localizationService = localizationService;
            _persistStateService = persistStateService;
            _workContext = workContext;
        }

        private async Task<IPagedList<TraderModel>> GetPagedListAsync(TraderSearchModel searchModel, TraderFilterModel filterModel, bool filterExist)
        {
            var traderEmployeeMappings = await _traderEmployeeMappingService.GetAllTraderEmployeeMappingAsync();
            var employees = await _employeeService.GetAllEmployeesAsync();
            var traderGroups = await _traderGroupService.GetAllTraderGroupsAsync();
            var workingAreas = await _workingAreaService.GetAllWorkingAreasAsync();
            var chambers = await _chamberService.GetAllChambersAsync();

            var professionTypes = await ProfessionType.Active.ToSelectionItemListAsync();
            var categoryBookTypes = await CategoryBookType.None.ToSelectionItemListAsync();
            var legalFormTypes = await LegalFormType.None.ToSelectionItemListAsync();
            var logistikiProgramTypes = await LogistikiProgramType.SoftOne.ToSelectionItemListAsync(withSpecialDefaultItem: true);
            var payrollProgramTypes = await PayrollProgramType.Prosvasis.ToSelectionItemListAsync(withSpecialDefaultItem: true);
            var customerTypes = await CustomerType.Other.ToSelectionItemListAsync();

            var employeeList = await _traderEmployeeMappingService.GetEmployeesNamesAsync();

            var accountingDepartm = 2;
            var payrollDepartm = 3;

            var query = _traderService.Table.AsEnumerable().Select(x =>
            {
                var model = x.ToModel<TraderModel>();

                model.FullName = model.FullName() ?? "";
                model.Vat = model.Vat ?? "";
                model.Doy = model.Doy ?? "";
                model.Email = model.Email ?? "";
                model.LogistikiIpAddress = model.LogistikiIpAddress ?? "";
                model.CompEngineer = model.CompEngineer ?? "";

                model.ChamberName = chambers.FirstOrDefault(a => a.Id == x.ChamberId)?.ChamberName ?? "";
                model.EmployeeName = employeeList.FirstOrDefault(a => a.TraderId == x.Id)?.Employees ?? "";
                model.TraderGroupName = traderGroups.FirstOrDefault(a => a.Id == x.TraderGroupId)?.Description ?? "";
                model.WorkingAreaName = workingAreas.FirstOrDefault(a => a.Id == x.WorkingAreaId)?.Description ?? "";
                model.ProfessionTypeName = professionTypes.FirstOrDefault(a => a.Value == x.ProfessionTypeId)?.Label ?? "";
                model.CategoryBookTypeName = categoryBookTypes.FirstOrDefault(a => a.Value == x.CategoryBookTypeId)?.Label ?? "";
                model.CustomerTypeName = customerTypes.FirstOrDefault(a => a.Value == x.CustomerTypeId)?.Label ?? "";
                model.LegalFormTypeName = legalFormTypes.FirstOrDefault(a => a.Value == x.LegalFormTypeId)?.Label ?? "";
                model.LogistikiProgramTypeName = logistikiProgramTypes.FirstOrDefault(a => a.Value == x.LogistikiProgramTypeId)?.Label ?? "";
                model.PayrollProgramTypeName = payrollProgramTypes.FirstOrDefault(a => a.Value == x.PayrollProgramTypeId)?.Label ?? "";

                model.EmployeeAccountingName = _traderEmployeeMappingService.GetEmployeeByTraderByDepartment(x.Id, accountingDepartm) ?? "";
                model.EmployeePayrollName = _traderEmployeeMappingService.GetEmployeeByTraderByDepartment(x.Id, payrollDepartm) ?? "";

                return model;
            }).AsQueryable();

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c =>
                    c.FullName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Vat.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Doy.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.Email.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.EmployeeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.TraderGroupName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.WorkingAreaName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.ProfessionTypeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.CategoryBookTypeName.ContainsIgnoreCase(searchModel.QuickSearch) ||
                    c.LegalFormTypeName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            if (filterExist)
            {
                if (!string.IsNullOrEmpty(filterModel.FullName))
                    query = query.Where(c => c.FullName.ContainsIgnoreCase(filterModel.FullName));

                if (!string.IsNullOrEmpty(filterModel.Doy))
                    query = query.Where(c => c.Doy.ContainsIgnoreCase(filterModel.Doy));

                if (filterModel.ProfessionTypeId.HasValue && filterModel.ProfessionTypeId.Value > -1)
                    query = query.Where(c => c.ProfessionTypeId == filterModel.ProfessionTypeId.Value);

                if (filterModel.CategoryBookTypeId.HasValue && filterModel.CategoryBookTypeId.Value > -1)
                    query = query.Where(c => c.CategoryBookTypeId == filterModel.CategoryBookTypeId.Value);

                if (filterModel.CustomerTypeId.HasValue && filterModel.CustomerTypeId.Value > -1)
                    query = query.Where(c => c.CustomerTypeId == filterModel.CustomerTypeId.Value);

                if (filterModel.LegalFormTypeId.HasValue && filterModel.LegalFormTypeId.Value > -1)
                    query = query.Where(c => c.LegalFormTypeId == filterModel.LegalFormTypeId.Value);

                if (filterModel.LogistikiProgramTypeId.HasValue && filterModel.LogistikiProgramTypeId.Value > -1)
                    query = query.Where(c => c.LogistikiProgramTypeId == filterModel.LogistikiProgramTypeId.Value);

                if (filterModel.TraderGroupId.HasValue && filterModel.TraderGroupId.Value > 0)
                    query = query.Where(c => c.TraderGroupId == filterModel.TraderGroupId.Value);

                if (filterModel.WorkingAreaId.HasValue && filterModel.WorkingAreaId.Value > 0)
                    query = query.Where(c => c.WorkingAreaId == filterModel.WorkingAreaId.Value);

                if (filterModel.EmployeeId.HasValue && filterModel.EmployeeId.Value > 0)
                {
                    var traderList = await _traderEmployeeMappingService.GetTradersByEmployeeIdAsync(filterModel.EmployeeId.Value);
                    var traderIds = traderList.Select(x => x.Id).ToList();
                    query = query.Where(x => traderIds.Contains(x.Id));
                }

                if (filterModel.IntraTrade > 0)
                    query = query.Where(c => c.IntraTrade == (filterModel.IntraTrade == 1));

                if (filterModel.ConnectionAccountingActive > 0)
                    query = query.Where(c => c.ConnectionAccountingActive == (filterModel.ConnectionAccountingActive == 1));

                if (filterModel.HyperPayrollId > 0)
                {
                    if (filterModel.HyperPayrollId == 1)
                        query = query.Where(c => c.HyperPayrollId > 0);
                    else
                        query = query.Where(c => c.HyperPayrollId == 0);
                }

                if (!string.IsNullOrEmpty(filterModel.CompEngineer))
                    query = query.Where(c => c.CompEngineer.ContainsIgnoreCase(filterModel.CompEngineer));

                if (!string.IsNullOrEmpty(filterModel.IpAddress))
                    query = query.Where(c => c.LogistikiIpAddress.ContainsIgnoreCase(filterModel.IpAddress));

                if (filterModel.Active == 1) // true
                    query = query.Where(c => c.Active && !c.Deleted);

                if (filterModel.Active == 2) // false
                    query = query.Where(c => !c.Active);

                if (filterModel.HasFinancialObligation > 0)
                    query = query.Where(c => c.HasFinancialObligation == (filterModel.HasFinancialObligation == 1));

                if (filterModel.PayrollProgramTypeId.HasValue && filterModel.PayrollProgramTypeId.Value > -1)
                    query = query.Where(c => c.PayrollProgramTypeId == filterModel.PayrollProgramTypeId.Value);

                if (filterModel.HasSubmissionSchedules > 0)
                    query = query.Where(c => c.HasSubmissionSchedules == (filterModel.HasSubmissionSchedules == 1));

                if (filterModel.HyperWeeklySchedule > 0)
                    query = query.Where(c => c.HyperWeeklySchedule == (filterModel.HyperWeeklySchedule == 1));

                if (filterModel.HyperMonthlySchedule > 0)
                    query = query.Where(c => c.HyperMonthlySchedule == (filterModel.HyperMonthlySchedule == 1));

                if (!string.IsNullOrEmpty(filterModel.Kad))
                {
                    var kads = filterModel.Kad.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    var traderIds = _traderKadService.Table.Where(x => kads.Contains(x.Code)).Select(x => x.TraderId).ToList();

                    query = query.Where(x => traderIds.Contains(x.Id));
                }
            }

            query = query.OrderBy(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<TraderSearchModel> PrepareTraderSearchModelAsync(TraderSearchModel searchModel)
        {
            var persistState = await _persistStateService.GetModelInstance<TraderSearchModel>();

            if (persistState.Exist)
                return persistState.Model;

            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.Columns = await CreateKendoGridColumnConfigAsync();

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.TraderModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<TraderListModel> PrepareTraderListModelAsync(TraderSearchModel searchModel)
        {
            var filterState = await _persistStateService.GetModelInstance<TraderFilterModel>();

            //get customer roles
            var traders = await GetPagedListAsync(searchModel, filterState.Model, filterState.Exist);

            //prepare grid model
            var model = new TraderListModel().PrepareToGrid(searchModel, traders);
            model.FilterExist = filterState.Exist;

            return model;
        }

        public virtual Task<TraderModel> PrepareTraderModelAsync(TraderModel model, Trader trader)
        {
            if (trader != null)
            {
                //fill in model values from the entity
                model ??= trader.ToModel<TraderModel>();
            }

            //set default values for the new model
            if (trader == null)
            {
                model.Active = true;
                model.TaxesFee = 650;
                model.AccountingSchema = 500;
                model.EmployerBreakLimit = 60;
            }

            return Task.FromResult(model);
        }

        private async Task<List<ColumnConfig>> CreateKendoGridColumnConfigAsync()
        {
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderModel>(1, nameof(TraderModel.FullName), ColumnType.RouterLink),
                ColumnConfig.Create<TraderModel>(2, nameof(TraderModel.Vat)),
                ColumnConfig.Create<TraderModel>(3, nameof(TraderModel.Email)),
                ColumnConfig.Create<TraderModel>(3, nameof(TraderModel.JobPhoneNumber1)),
                ColumnConfig.Create<TraderModel>(4, nameof(TraderModel.Amka)),
                ColumnConfig.Create<TraderModel>(5, nameof(TraderModel.TaxisUserName), hidden: true),
                ColumnConfig.Create<TraderModel>(6, nameof(TraderModel.TaxisPassword), hidden: true),
                ColumnConfig.Create<TraderModel>(7, nameof(TraderModel.CustomerTypeName)),
                ColumnConfig.Create<TraderModel>(8, nameof(TraderModel.CategoryBookTypeName)),
                ColumnConfig.Create<TraderModel>(9, nameof(TraderModel.LegalFormTypeName)),
                ColumnConfig.Create<TraderModel>(8, nameof(TraderModel.HasFinancialObligation), ColumnType.Checkbox),
                ColumnConfig.Create<TraderModel>(8, nameof(TraderModel.BoardMemberExpiryDate), ColumnType.Date, hidden: true),
                ColumnConfig.Create<TraderModel>(10, nameof(TraderModel.ConnectionAccountingActive), ColumnType.Checkbox, filterType: "boolean"),
                ColumnConfig.Create<TraderModel>(11, nameof(TraderModel.HyperPayrollId), style: rightAlign),
                ColumnConfig.Create<TraderModel>(12, nameof(TraderModel.ChamberNumber), hidden: true),
                ColumnConfig.Create<TraderModel>(12, nameof(TraderModel.ChamberName), hidden: true),
                ColumnConfig.Create<TraderModel>(12, nameof(TraderModel.LogistikiUsername), hidden: true),
                ColumnConfig.Create<TraderModel>(13, nameof(TraderModel.LogistikiPassword), hidden: true),
                ColumnConfig.Create<TraderModel>(14, nameof(TraderModel.LogistikiIpAddress), hidden: true),
                ColumnConfig.Create<TraderModel>(15, nameof(TraderModel.Active), ColumnType.Checkbox, hidden: true, filterType: "boolean"),
                ColumnConfig.Create<TraderModel>(16, nameof(TraderModel.Doy), hidden: true),
                ColumnConfig.Create<TraderModel>(17, nameof(TraderModel.ProfessionTypeName), hidden: true),
                ColumnConfig.Create<TraderModel>(18, nameof(TraderModel.LogistikiProgramTypeName), hidden: true),
                ColumnConfig.Create<TraderModel>(19, nameof(TraderModel.ProfessionTypeName), hidden: true),
                ColumnConfig.Create<TraderModel>(20, nameof(TraderModel.CompanyId), hidden: true),
                ColumnConfig.Create<TraderModel>(21, nameof(TraderModel.CompanyPassword), hidden: true),
                ColumnConfig.Create<TraderModel>(22, nameof(TraderModel.EmployeeName), hidden: true),
                ColumnConfig.Create<TraderModel>(23, nameof(TraderModel.TraderGroupName), hidden: true),
                ColumnConfig.Create<TraderModel>(24, nameof(TraderModel.WorkingAreaName), hidden: true),
                ColumnConfig.Create<TraderModel>(25, nameof(TraderModel.IntraTrade), ColumnType.Checkbox, hidden: true, filterType: "boolean"),
                ColumnConfig.Create<TraderModel>(26, nameof(TraderModel.AccountingSchema), hidden: true),
                ColumnConfig.Create<TraderModel>(27, nameof(TraderModel.EmployerIkaUserName), hidden: true),
                ColumnConfig.Create<TraderModel>(28, nameof(TraderModel.EmployerIkaPassword), hidden: true),
                ColumnConfig.Create<TraderModel>(29, nameof(TraderModel.CompEngineer), hidden: true),
                ColumnConfig.Create<TraderModel>(30, nameof(TraderModel.PayrollProgramTypeName), hidden: true),
                ColumnConfig.Create<TraderModel>(31, nameof(TraderModel.HasSubmissionSchedules), ColumnType.Checkbox, filterType: "boolean", hidden: true),
                ColumnConfig.Create<TraderModel>(32, nameof(TraderModel.HyperMonthlySchedule), ColumnType.Checkbox, filterType: "boolean", hidden: true),
                ColumnConfig.Create<TraderModel>(33, nameof(TraderModel.HyperWeeklySchedule), ColumnType.Checkbox, filterType: "boolean", hidden: true),
                ColumnConfig.Create<TraderModel>(34, nameof(TraderModel.LogistikiIpAddress), hidden: true),
                ColumnConfig.Create<TraderModel>(34, nameof(TraderModel.EmployeeAccountingName), hidden: true),
                ColumnConfig.Create<TraderModel>(34, nameof(TraderModel.EmployeePayrollName), hidden: true)
            };

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsAdminAsync(customer) || await _customerService.IsOfficeAsync(customer))
            {
                columns.Add(ColumnConfig.Create<TraderModel>(35, nameof(TraderModel.TraderPayment), ColumnType.Numeric));
                columns.Add(ColumnConfig.Create<TraderModel>(36, nameof(TraderModel.TraderExpense), ColumnType.Numeric));
            }

            return columns;
        }

        public virtual async Task<TraderFormModel> PrepareTraderFormModelAsync(TraderFormModel formModel)
        {
            var professionTypes = await ProfessionType.Active.ToSelectionItemListAsync();
            var employmentLevelTypes = await EmploymentLevelType.Medium.ToSelectionItemListAsync();
            var chambers = await _modelFactoryService.GetAllChambersAsync();
            var accountingPlanTypes = await AccountingPlanType.Standard.ToSelectionItemListAsync();
            var populationTypes = await PopulationType.AreaUpTwoHundredThousandAndMore.ToSelectionItemListAsync();
            var traderGroups = await _modelFactoryService.GetAllTraderGroupsAsync();
            var workingAreas = await _modelFactoryService.GetAllWorkingAreasAsync();
            var logistikiProgramTypes = await LogistikiProgramType.SoftOne.ToSelectionItemListAsync(withSpecialDefaultItem: true);
            var payrollProgramTypes = await PayrollProgramType.HyperM.ToSelectionItemListAsync(withSpecialDefaultItem: true);
            var traders = await _modelFactoryService.GetAllTradersAsync();
            var statusTypes = await StatusType.Pending.ToSelectionItemListAsync();
            var activatedTypes = await ActivatedType.Active.ToSelectionItemListAsync();
            var customerTypes = await CustomerType.NaturalPerson.ToSelectionItemListAsync();
            var farmerTypes = await FarmerType.SpecialRegime.ToSelectionItemListAsync();
            var genderTypes = await GenderType.None.ToSelectionItemListAsync();
            var boardMemberTypes = await BoardMemberType.None.ToSelectionItemListAsync();

            var categoryBookTypes = await CategoryBookType.None.ToSelectionItemListAsync();
            var legalFormTypes = await LegalFormType.None.ToSelectionItemListAsync();
            var vatSystemTypes = await VatSystemType.None.ToSelectionItemListAsync();

            var aboutPanel1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.LastName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.Vat), FieldType.Text, markAsRequired: true)
            };

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsAdminAsync(customer) || await _customerService.IsOfficeAsync(customer))
            {
                aboutPanel1.Add(FieldConfig.Create<TraderModel>(nameof(TraderModel.TraderPayment), FieldType.Numeric));
                aboutPanel1.Add(FieldConfig.Create<TraderModel>(nameof(TraderModel.TraderExpense), FieldType.Numeric));
            }

            var aboutPanel2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.FirstName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.Email), FieldType.Text)
            };

            var aboutPanel3 = new List<Dictionary<string, object>>()
            {
                FieldConfig.CreateDivider(),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.Active), FieldType.Switch),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.ProfessionTypeId), FieldType.Select, options: professionTypes),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.CustomerTypeId), FieldType.Select, options: customerTypes),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.LegalFormTypeId), FieldType.Select, options: legalFormTypes),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.CategoryBookTypeId), FieldType.Select, options: categoryBookTypes),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.BoardMemberExpiryDate), FieldType.Date, disableExpression: "model.categoryBookTypeId != 3"),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.Email2), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.Email3), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.NonRepresentationOfNaturalPerson), FieldType.Switch),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.HasFinancialObligation), FieldType.Switch),
            };

            var aboutPanel4 = new List<Dictionary<string, object>>()
            {
                FieldConfig.CreateDivider(),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.TaxesFee), FieldType.Numeric),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.AccountingPlanTypeId), FieldType.Select, options: accountingPlanTypes),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.PopulationTypeId), FieldType.Select, options: populationTypes),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.TemporarilyItems), FieldType.Numeric),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.TraderGroupId), FieldType.Select, options: traderGroups),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.WorkingAreaId), FieldType.Select, options: workingAreas),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.ChamberNumber), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.ChamberId), FieldType.Select, options: chambers),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.EmploymentLevelTypeId), FieldType.Select, options: employmentLevelTypes),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.CompanyRestarting), FieldType.Switch),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.HasSecurityDoctor), FieldType.Switch),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.WebSite), FieldType.Text),
            };

            var accounting1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.LogistikiProgramTypeId), FieldType.Select, options: logistikiProgramTypes),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.AccountingSchema), FieldType.Numeric),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.CompanyId), FieldType.Numeric),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.CompanyUsername), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.CompanyPassword), FieldType.Text),
                FieldConfig.CreateDivider(),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.LogistikiDataBaseName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.LogistikiUsername), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.LogistikiPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.LogistikiIpAddress), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.LogistikiPort), FieldType.Text),
            };

            var accounting2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.DiscountPredictions), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.DeductibleCredits), FieldType.Text),
                //FieldConfig.CreateDivider(await _localizationService.GetResourceAsync("App.Models.TraderModel.Panels.BoardMembers")),
                //FieldConfig.Create<TraderModel>(nameof(TraderModel.InsuranceOption), FieldType.Text),
                //FieldConfig.Create<TraderModel>(nameof(TraderModel.MemberPercentage), FieldType.Decimals),
                //FieldConfig.Create<TraderModel>(nameof(TraderModel.MemberEmail), FieldType.Text),
                //FieldConfig.Create<TraderModel>(nameof(TraderModel.BoardMemberTypeId), FieldType.Select, options: boardMemberTypes),
                //FieldConfig.Create<TraderModel>(nameof(TraderModel.BoardMemberExpiryDate), FieldType.Date),
            };

            var payroll = new List<Dictionary<string, object>>()
            {
                FieldConfig.CreateDivider(await _localizationService.GetResourceAsync("App.Models.TraderModel.Sections.Payroll")),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.HyperPayrollId), FieldType.Numeric),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.PayrollProgramTypeId), FieldType.Select, options: payrollProgramTypes),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.HasSubmissionSchedules), FieldType.Switch),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.HyperWeeklySchedule), FieldType.Switch),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.HyperMonthlySchedule), FieldType.Switch),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.EmployerBreakLimit), FieldType.Numeric),
                FieldConfig.CreateDivider(),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.TaxSystemId), FieldType.Numeric),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.ProsvasisId), FieldType.Numeric),
  };

            var erpItemsInfo = new List<Dictionary<string, object>>()
            {
                FieldConfig.CreateDivider(await _localizationService.GetResourceAsync("App.Models.TraderModel.Sections.ErpItemsInfo")),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.ThroughVpn), FieldType.Switch),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.ContactPerson), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.ContactPersonPhone), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.CompEngineer), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.CompEngineerTo), FieldType.Select, options: traders),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.CompEngineerFrom), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.CompEngineerPhone), FieldType.Text),
            };

            var generalInfo1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.CodeName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.LastName2), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.FatherLastName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.FatherFirstName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.MotherLastName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.MotherFirstname), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.IdentityTypeId), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.IdentityTypeName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.IdentityNumber), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.IdentityDate), FieldType.Date),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.IdentityDepartment), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.Amka), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.Gemh), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.AmIka), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.AmOaee), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.AmOga), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.AmEtaa), FieldType.Text)
            };

            var generalInfo2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.AmDiasIka), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.AmDiasEtea), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.AmEmployer), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.AmRetirement), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.AmsOga), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.AmsNat), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.Eam), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.TradeName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.ProfessionalActivity), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.Doy), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.LocalOffice), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.EnvelopeNumber), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.Birthday), FieldType.Date),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.BirthPlace), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.MunicipalNumber), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.MarriedDate), FieldType.Date),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.ActivatedTypeId), FieldType.Select, options: activatedTypes),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.DeathDate), FieldType.Date)
            };

            var jobAddressInfo = new List<Dictionary<string, object>>()
            {
                FieldConfig.CreateDivider(await _localizationService.GetResourceAsync("App.Models.TraderModel.Sections.JobAddressInfo")),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.RegisterCode), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.JobAddress), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.JobStreetNumber), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.JobCity), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.JobMunicipality), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.JobPlace), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.JobPostcode), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.JobPhoneNumber1), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.JobPhoneNumber2), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.JobFax), FieldType.Text)
            };

            var homeAddressInfo = new List<Dictionary<string, object>>()
            {
                FieldConfig.CreateDivider(await _localizationService.GetResourceAsync("App.Models.TraderModel.Sections.HomeAddressInfo")),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.HomeAddress), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.HomeStreetNumber), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.HomeCity), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.HomeMunicipality), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.HomePlace), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.HomePostcode), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.HomePhoneNumber1), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.HomePhoneNumber2), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.HomeCellphone), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.HomeFax), FieldType.Text)
            };

            var otherItemsInfo1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.DeadlineSubmition), FieldType.Date),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.StatusTypeId), FieldType.Select, options: statusTypes),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.FarmerTypeId), FieldType.Select, options: farmerTypes),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.AbroatResident), FieldType.Switch),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.Bank), FieldType.Text)
            };

            var otherItemsInfo2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.GenderTypeId), FieldType.Select, options: genderTypes),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.Comments), FieldType.Textarea),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.Iban), FieldType.Text)
            };

            var registryInfo1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.VatSystemTypeId), FieldType.Select, options: vatSystemTypes),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.CommentRegistry), FieldType.Textarea)
            };

            var registryInfo2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.StartingDate), FieldType.Date),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.ExpiryDate), FieldType.Date),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.IntraTrade), FieldType.Switch),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.SeparateDeclaration), FieldType.Switch)
            };

            var legalRepresentativeInfoLeft = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.RepresentativeName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.RepresentativeVat), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.RepresentativeUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.RepresentativePassword), FieldType.Text)
            };

            var legalRepresentativeInfoRight = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.AccountantName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.AccountantVat), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.SpowseId), FieldType.Numeric),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.SpowseName), FieldType.Text)
            };

            var connectionItemsInfo1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.TaxisUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.TaxisPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.TaxisKeyNumber), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.SubmissionAuthorizedForms), FieldType.Switch),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.SpecialTaxisUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.SpecialTaxisPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.EfkaUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.EfkaPassword), FieldType.Text)
            };

            var connectionItemsInfo2 = new List<Dictionary<string, object>>()
            {
            };

            var connectionItemsInfo3 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.IntrastatUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.IntrastatPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.EstateUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.EstatePassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.KeaoUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.KeaoPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.OaeeUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.OaeePassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.OaeeKeynumber), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.EmployerIkaUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.EmployerIkaPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.EmployeeIkaUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.EmployeeIkaPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.IkaKeyNumber), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.OaedUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.OaedPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.OaedKeyNumber), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.GemhUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.GemhPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.ErmisUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.ErmisPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.OpsydUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.OpsydPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.SepeUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.SepePassword), FieldType.Text),
                FieldConfig.CreateDivider(await _localizationService.GetResourceAsync("App.Models.TraderModel.Sections.KeaoGredentials")),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.KeaoIkaUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.KeaoIkaPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.KeaoOaeeUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.KeaoOaeePassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.KeaoEfkaUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.KeaoEfkaPassword), FieldType.Text)
            };

            var connectionItemsInfo4 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderModel>(nameof(TraderModel.OeeUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.OeePassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.OpekepeUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.OpekepePassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.AgrotiUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.AgrotiPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.TepahUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.TepahPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.NatUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.NatPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.DipetheUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.DipethePassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.ModUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.ModPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.SmokePrdUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.SmokePrdPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.Article39UserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.Article39Password), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.MhdasoUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.MhdasoPassword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.MydataUserName), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.MydataPaswword), FieldType.Text),
                FieldConfig.Create<TraderModel>(nameof(TraderModel.MydataApi), FieldType.Text)
            };

            var panels = new List<Dictionary<string, object>>()
            {
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.TraderModel.Panels.About"), true, "col-12 md:col-6", aboutPanel1, aboutPanel2, aboutPanel3, aboutPanel4, payroll, erpItemsInfo),
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.TraderModel.Panels.Accounting"), false, "col-12 md:col-6", accounting1, accounting2),
                //FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.TraderModel.Panels.BoardMember"), false, "col-12 md:col-6", ),
                //FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.TraderModel.Sections.PersonalInfo"), false, "col-12 md:col-6", ),
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.TraderModel.Sections.GeneralInfo"), false, "col-12 md:col-6", generalInfo1, generalInfo2, jobAddressInfo, homeAddressInfo),
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.TraderModel.Sections.OtherItemsInfo"), false, "col-12 md:col-6", otherItemsInfo1, otherItemsInfo2),
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.TraderModel.Sections.RegistryInfo"), false, "col-12 md:col-6", registryInfo1, registryInfo2),
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.TraderModel.Sections.LegalRepresentativeInfo"), false, "col-12 md:col-6", legalRepresentativeInfoLeft, legalRepresentativeInfoRight),
                FieldConfig.CreatePanel(await _localizationService.GetResourceAsync("App.Models.TraderModel.Sections.ConnectionItemsInfo"), false, "col-12 md:col-6", connectionItemsInfo1,connectionItemsInfo2, connectionItemsInfo3, connectionItemsInfo4)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.TraderModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", panels);

            return formModel;
        }

        public virtual async Task<TraderFilterModel> PrepareTraderFilterModelAsync()
        {
            var persistState = await _persistStateService.GetModelInstance<TraderFilterModel>();
            return persistState.Model;
        }

        public virtual async Task<TraderFilterFormModel> PrepareTraderFilterFormModelAsync(TraderFilterFormModel filterFormModel)
        {
            var employees = await _modelFactoryService.GetAllEmployeesAsync(false);
            var traderGroups = await _modelFactoryService.GetAllTraderGroupsAsync(false);
            var workingAreas = await _modelFactoryService.GetAllWorkingAreasAsync(false);

            var tristateTypes = await TristateType.Null.ToSelectionItemListAsync();
            var professionTypes = await ProfessionType.Active.ToSelectionItemListAsync(withSpecialDefaultItem: true, index: -1);
            var categoryBookTypes = await CategoryBookType.None.ToSelectionItemListAsync(withSpecialDefaultItem: true, index: -1);
            var legalFormTypes = await LegalFormType.None.ToSelectionItemListAsync(withSpecialDefaultItem: true, index: -1);
            var logistikiProgramTypes = await LogistikiProgramType.SoftOne.ToSelectionItemListAsync(withSpecialDefaultItem: true);
            var payrollProgramTypes = await PayrollProgramType.Prosvasis.ToSelectionItemListAsync(withSpecialDefaultItem: true);
            var customerTypes = await CustomerType.Other.ToSelectionItemListAsync(withSpecialDefaultItem: true, index: -1);

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.FullName), FieldType.Text),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.Doy), FieldType.Text),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.EmployeeId), FieldType.GridSelect, options: employees),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.ProfessionTypeId), FieldType.Select, options: professionTypes),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.CustomerTypeId), FieldType.Select, options: customerTypes),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.CategoryBookTypeId), FieldType.Select, options: categoryBookTypes),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.LegalFormTypeId), FieldType.Select, options: legalFormTypes)
            };

            var center = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.ConnectionAccountingActive), FieldType.Select, options: tristateTypes),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.LogistikiProgramTypeId), FieldType.Select, options: logistikiProgramTypes),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.TraderGroupId), FieldType.GridSelect, options: traderGroups),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.WorkingAreaId), FieldType.GridSelect, options: workingAreas),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.CompEngineer), FieldType.Text),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.IpAddress), FieldType.Text),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.Kad), FieldType.Text),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.IntraTrade), FieldType.Select, options: tristateTypes)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.HyperPayrollId), FieldType.Select, options: tristateTypes),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.PayrollProgramTypeId), FieldType.Select, options: payrollProgramTypes),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.HasSubmissionSchedules), FieldType.Select, options: tristateTypes),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.HyperMonthlySchedule), FieldType.Select, options: tristateTypes),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.HyperWeeklySchedule), FieldType.Select, options: tristateTypes),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.Active), FieldType.Select, options: tristateTypes),
                FieldConfig.Create<TraderFilterModel>(nameof(TraderFilterModel.HasFinancialObligation), FieldType.Select, options: tristateTypes),
            };

            var saveState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderFilterModel>("SaveState", FieldType.Button, themeColor: "primary",
                label: await _localizationService.GetResourceAsync("App.Common.SaveState"))
            };

            var removeState = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<TraderFilterModel>("RemoveState", FieldType.Button, themeColor: "warning",
                label: await _localizationService.GetResourceAsync("App.Common.RemoveState"))
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-4", "col-12 md:col-4", "col-12 md:col-4", "col-12 md:col-2", "col-12 md:col-2" }, left, center, right, saveState, removeState);

            filterFormModel.CustomProperties.Add("fields", fields);

            return filterFormModel;
        }
        public virtual async Task<TraderSearchModel> PrepareTraderDialogSearchModelAsync(TraderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.Columns = await CreateKendoGridColumnConfigAsync();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.TraderModel.ListForm.Title");
            searchModel.DataKey = "id";

            return searchModel;
        }

    }
}