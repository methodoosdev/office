using App.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Services
{
    public partial interface IModelFactoryService
    {
        Task<IList<SelectionItemList>> GetAllCurrenciesAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionList>> GetAllFlagFileNamesAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionList>> GetAllCulturesAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllChambersAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllWorkingAreasAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllTraderGroupsAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllTradersAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllTradersAsync(FieldConfigType type);
        Task<IList<SelectionItemList>> GetAllEmployeesAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllDepartmentsAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllSupervisorsAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllEducationsAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllSpecialtiesAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllJobTitlesAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllSimpleTaskCategoriesAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllSimpleTaskDepartmentsAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllSimpleTaskNaturesAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllSimpleTaskSectorsAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllVatExemptionApprovalsAsync(int traderId, bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllActiveEmployersAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllAssignmentPrototypesAsync(bool withSpecialDefaultItem = true, bool showInActive = false);
        Task<IList<SelectionItemList>> GetAllAssignmentPrototypeActionsAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllAssignmentReasonsAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllEmailAccountsAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetSelectionItemListAsync(Dictionary<int, string> list, bool withSpecialDefaultItem = true);

        Task<IList<SelectionItemList>> GetAllActivityLogTypesAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllCountriesAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllLanguagesAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllCustomerRolesAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionList>> GetAllTimeZonesAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> GetAllLogLevelsAsync(bool withSpecialDefaultItem = true);
        Task<IList<SelectionItemList>> PrepareGetAllGdprRequestTypesAsync(bool withSpecialDefaultItem = true);

        Task<IList<SelectionItemList>> GetAllTraderRatingCategoriesAsync(bool withSpecialDefaultItem = true);
    }
}