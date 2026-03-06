using App.Core;
using App.Core.Domain.Gdpr;
using App.Core.Domain.Logging;
using App.Core.Infrastructure;
using App.Models.Traders;
using App.Services.Assignment;
using App.Services.Customers;
using App.Services.Directory;
using App.Services.Employees;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Logging;
using App.Services.Messages;
using App.Services.Offices;
using App.Services.SimpleTask;
using App.Services.Stores;
using App.Services.Traders;
using App.Services.VatExemption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services
{
    public partial class ModelFactoryService : IModelFactoryService
    {
        private const string FLAGS_PATH = @"assets\images\flags";

        private readonly ITraderService _traderService;
        private readonly IEmployeeService _employeeService;
        private readonly IEducationService _educationService;
        private readonly IDepartmentService _departmentService;
        private readonly ISpecialtyService _specialtyService;
        private readonly IJobTitleService _jobTitleService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly INopFileProvider _fileProvider;
        private readonly IStoreService _storeService;
        private readonly ITraderGroupService _traderGroupService;
        private readonly IWorkingAreaService _workingAreaService;
        private readonly IChamberService _chamberService;
        private readonly ISimpleTaskCategoryService _simpleTaskCategoryService;
        private readonly ISimpleTaskDepartmentService _simpleTaskDepartmentService;
        private readonly ISimpleTaskNatureService _simpleTaskNatureService;
        private readonly ISimpleTaskSectorService _simpleTaskSectorService;
        private readonly IVatExemptionApprovalService _vatExemptionApprovalService;
        private readonly IAssignmentPrototypeService _assignmentPrototypeService;
        private readonly IAssignmentPrototypeActionService _assignmentPrototypeActionService;
        private readonly IAssignmentReasonService _assignmentReasonService;
        private readonly ITraderRatingCategoryService _traderRatingCategoryService;

        public ModelFactoryService(
            ITraderService traderService,
            IEmployeeService employeeService,
            IEducationService educationService,
            IDepartmentService departmentService,
            ISpecialtyService specialtyService,
            IJobTitleService jobTitleService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IEmailAccountService emailAccountService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            INopFileProvider fileProvider,
            IStoreService storeService,
            ITraderGroupService traderGroupService,
            IWorkingAreaService workingAreaService,
            IChamberService chamberService,
            ISimpleTaskCategoryService simpleTaskCategoryService,
            ISimpleTaskDepartmentService simpleTaskDepartmentService,
            ISimpleTaskNatureService simpleTaskNatureService,
            ISimpleTaskSectorService simpleTaskSectorService,
            IVatExemptionApprovalService vatExemptionApprovalService,
            IAssignmentPrototypeService assignmentPrototypeService,
            IAssignmentPrototypeActionService assignmentPrototypeActionService,
            IAssignmentReasonService assignmentReasonService,
            ITraderRatingCategoryService traderRatingCategoryService
            )
        {
            _traderService = traderService;
            _employeeService = employeeService;
            _educationService = educationService;
            _departmentService = departmentService;
            _specialtyService = specialtyService;
            _jobTitleService = jobTitleService;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _emailAccountService = emailAccountService;
            _languageService = languageService;
            _localizationService = localizationService;
            _fileProvider = fileProvider;
            _storeService = storeService;
            _traderGroupService = traderGroupService;
            _workingAreaService = workingAreaService;
            _chamberService = chamberService;
            _simpleTaskCategoryService = simpleTaskCategoryService;
            _simpleTaskDepartmentService = simpleTaskDepartmentService;
            _simpleTaskNatureService = simpleTaskNatureService;
            _simpleTaskSectorService = simpleTaskSectorService;
            _vatExemptionApprovalService = vatExemptionApprovalService;
            _assignmentPrototypeService = assignmentPrototypeService;
            _assignmentPrototypeActionService = assignmentPrototypeActionService;
            _assignmentReasonService = assignmentReasonService;
            _traderRatingCategoryService = traderRatingCategoryService;
        }

        protected virtual async Task PrepareDefaultItemAsync(IList<SelectionItemList> items, bool withSpecialDefaultItem)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //whether to insert the first special item for the default value
            if (!withSpecialDefaultItem)
                return;

            //prepare item text
            var defaultItemText = await _localizationService.GetResourceAsync("App.Common.Choice");

            //insert this default item at first
            items.Insert(0, new SelectionItemList { Label = defaultItemText, Value = 0 });
        }

        public virtual async Task<IList<SelectionItemList>> GetAllChambersAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _chamberService.GetAllChambersAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.ChamberName }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public virtual async Task<IList<SelectionItemList>> GetAllCurrenciesAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _currencyService.GetAllCurrenciesAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Name }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public virtual async Task<IList<SelectionItemList>> GetAllWorkingAreasAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _workingAreaService.GetAllWorkingAreasAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Description }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public virtual async Task<IList<SelectionList>> GetAllFlagFileNamesAsync(bool withSpecialDefaultItem = true)
        {
            var flagNames = _fileProvider
                .EnumerateFiles(_fileProvider.GetAbsolutePath(FLAGS_PATH), "*.png")
                .Select(_fileProvider.GetFileName)
                .ToList();

            var availableFlagFileNames = flagNames.Select(flagName => new SelectionList
            {
                Label = flagName,
                Value = flagName
            }).ToList();

            //whether to insert the first special item for the default value
            if (withSpecialDefaultItem)
            {
                //prepare item text
                var defaultItemText = await _localizationService.GetResourceAsync("App.Common.Choice");

                //insert this default item at first
                availableFlagFileNames.Insert(0, new SelectionList { Label = defaultItemText, Value = null });
            }

            return availableFlagFileNames;
        }

        public virtual async Task<IList<SelectionList>> GetAllCulturesAsync(bool withSpecialDefaultItem = true)
        {
            var cultures = System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.SpecificCultures)
                .OrderBy(x => x.EnglishName)
                .Select(x => new SelectionList
                {
                    Value = x.IetfLanguageTag,
                    Label = $"{x.EnglishName}. {x.IetfLanguageTag}"
                }).ToList();

            //whether to insert the first special item for the default value
            if (withSpecialDefaultItem)
            {
                //prepare item text
                var defaultItemText = await _localizationService.GetResourceAsync("App.Common.Choice");

                //insert this default item at first
                cultures.Insert(0, new SelectionList { Label = defaultItemText, Value = null });
            }

            return cultures;
        }

        public virtual async Task<IList<SelectionItemList>> GetAllTraderGroupsAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _traderGroupService.GetAllTraderGroupsAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Description }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public virtual async Task<IList<SelectionItemList>> GetAllTradersAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _traderService.GetAllTradersAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.ToTraderFullName(), Disabled = x.Deleted || !x.Active || !x.ConnectionAccountingActive })
                .OrderBy(o => o.Label)
                .ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public virtual async Task<IList<SelectionItemList>> GetAllTradersAsync(FieldConfigType type)
        {
            var items = (await _traderService.GetAllTradersAsync(type))
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.ToTraderFullName(), Disabled = x.Deleted || !x.Active || !x.ConnectionAccountingActive })
                .OrderBy(o => o.Label)
                .ToList();

            return items;
        }

        public virtual async Task<IList<SelectionItemList>> GetAllEmployeesAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _employeeService.GetAllEmployeesAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.FullName() }).OrderBy(o => o.Label).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public virtual async Task<IList<SelectionItemList>> GetAllDepartmentsAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _departmentService.GetAllDepartmentsAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Description }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public virtual async Task<IList<SelectionItemList>> GetAllSupervisorsAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _employeeService.GetAllEmployeesAsync(true))
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.FullName() }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public virtual async Task<IList<SelectionItemList>> GetAllEducationsAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _educationService.GetAllEducationsAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Description }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public virtual async Task<IList<SelectionItemList>> GetAllSpecialtiesAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _specialtyService.GetAllSpecialtiesAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Description }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public virtual async Task<IList<SelectionItemList>> GetAllJobTitlesAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _jobTitleService.GetAllJobTitlesAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Description }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public virtual async Task<IList<SelectionItemList>> GetAllSimpleTaskCategoriesAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _simpleTaskCategoryService.GetAllSimpleTaskCategoriesAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Description }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public virtual async Task<IList<SelectionItemList>> GetAllSimpleTaskDepartmentsAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _simpleTaskDepartmentService.GetAllSimpleTaskDepartmentsAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Description }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public virtual async Task<IList<SelectionItemList>> GetAllSimpleTaskNaturesAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _simpleTaskNatureService.GetAllSimpleTaskNaturesAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Description }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public virtual async Task<IList<SelectionItemList>> GetAllSimpleTaskSectorsAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _simpleTaskSectorService.GetAllSimpleTaskSectorsAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Description }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public virtual async Task<IList<SelectionItemList>> GetAllVatExemptionApprovalsAsync(int traderId, bool withSpecialDefaultItem = true)
        {
            var items = (await _vatExemptionApprovalService.GetAllVatExemptionApprovalsAsync(traderId))
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.ApprovalNumber }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public virtual async Task<IList<SelectionItemList>> GetAllActiveEmployersAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _traderService.Table.Where(x => x.HasSubmissionSchedules).ToListAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.ToTraderFullName() }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }
        public virtual async Task<IList<SelectionItemList>> GetAllAssignmentPrototypesAsync(bool withSpecialDefaultItem = true, bool showInActive = false)
        {
            var items = (await _assignmentPrototypeService.GetAllAssignmentPrototypesAsync(showInActive))
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Name }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }
        public virtual async Task<IList<SelectionItemList>> GetAllAssignmentPrototypeActionsAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _assignmentPrototypeActionService.GetAllAssignmentPrototypeActionsAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Name }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }
        public virtual async Task<IList<SelectionItemList>> GetAllAssignmentReasonsAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _assignmentReasonService.GetAllAssignmentReasonsAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Description }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }
        public virtual async Task<IList<SelectionItemList>> GetAllEmailAccountsAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _emailAccountService.GetAllEmailAccountsAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Email }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public async Task<IList<SelectionItemList>> GetSelectionItemListAsync(Dictionary<int, string> list, bool withSpecialDefaultItem = true)
        {
            var items = await list
                .SelectAwait(async x => new SelectionItemList(x.Key, await _localizationService.GetResourceAsync(x.Value)))
                .ToListAsync();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public async Task<IList<SelectionItemList>> GetAllActivityLogTypesAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _customerActivityService.GetAllActivityTypesAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Name }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public async Task<IList<SelectionItemList>> GetAllCountriesAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _countryService.GetAllCountriesAsync(showHidden: true))
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Name }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public async Task<IList<SelectionItemList>> GetAllLanguagesAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _languageService.GetAllLanguagesAsync(showHidden: true))
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Name }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public async Task<IList<SelectionItemList>> GetAllCustomerRolesAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _customerService.GetAllCustomerRolesAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Name }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public async Task<IList<SelectionList>> GetAllTimeZonesAsync(bool withSpecialDefaultItem = true)
        {
            var items = _dateTimeHelper.GetSystemTimeZones()
                .Select(x => new SelectionList { Value = x.Id, Label = x.DisplayName }).ToList();

            if (withSpecialDefaultItem) 
            {
                //prepare item text
                var defaultItemText = await _localizationService.GetResourceAsync("App.Common.Choice");

                //insert this default item at first
                items.Insert(0, new SelectionList { Label = defaultItemText, Value = null });
            }

            return items;
        }

        public async Task<IList<SelectionItemList>> GetAllLogLevelsAsync(bool withSpecialDefaultItem = true)
        {
            var items = await LogLevel.Debug.ToSelectionItemListAsync(false);

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public async Task<IList<SelectionItemList>> PrepareGetAllGdprRequestTypesAsync(bool withSpecialDefaultItem = true)
        {
            var items = await GdprRequestType.ConsentAgree.ToSelectionItemListAsync(false);

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }

        public async Task<IList<SelectionItemList>> GetAllTraderRatingCategoriesAsync(bool withSpecialDefaultItem = true)
        {
            var items = (await _traderRatingCategoryService.GetAllTraderRatingCategoriesAsync())
                .Select(x => new SelectionItemList { Value = x.Id, Label = x.Description }).ToList();

            await PrepareDefaultItemAsync(items, withSpecialDefaultItem);

            return items;
        }
    }
}