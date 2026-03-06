using App.Core;
using App.Core.Domain.Payroll;
using App.Core.Domain.Traders;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Traders;
using App.Services.Common;
using App.Services.Localization;
using App.Services.Traders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Services.Configuration
{
    public partial interface IFieldConfigService
    {
        Task<Dictionary<string, object>> GetTradersMultiColumnComboBox<T>(
            string property, FieldConfigType type = FieldConfigType.Accounting, 
            string valueProp = "id", string labelProp = "fullName", bool? hideLabel = null) where T : class;
        Task<Dictionary<string, object>> GetEmployersGridSelect<T>(string property, string hideExpression = null) where T : class;
        Task<IList<EmployerLookupItem>> GetEmployers();
        Task<Dictionary<string, object>> GetEmployersMultiColumnComboBox<T>(string property) where T : class;
    }
    public partial class FieldConfigService : IFieldConfigService
    {
        private readonly ITraderService _traderService;
        private readonly ILocalizationService _localizationService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ISqlConnectionService _connectionService;
        private string _employersQuery => @"
            SELECT ID_CMP As CompanyId, 
				ISNULL(NAME, '') AS LastName, ISNULL(FRSTNAME, '') AS FirstName, FORMAT(CAST(SVAT AS INTEGER), 'D9') AS Vat
				FROM vcmplist
				WHERE ISACTIVE = 1
				ORDER BY NAME
            ";

        public FieldConfigService(
            ITraderService traderService,
            ILocalizationService localizationService,
            IAppDataProvider dataProvider,
            ISqlConnectionService connectionService)
        {
            _traderService = traderService;
            _localizationService = localizationService;
            _dataProvider = dataProvider;
            _connectionService = connectionService;
        }

        public async Task<Dictionary<string, object>> GetTradersMultiColumnComboBox<T>(
            string property, FieldConfigType type = FieldConfigType.Accounting, 
            string valueProp = "id", string labelProp = "fullName", bool? hideLabel = null) where T : class
        {
            var placeholder = await _localizationService.GetResourceAsync("App.Common.Choice");

            var customerTypes = await CustomerType.Other.ToSelectionItemListAsync();
            var legalFormTypes = await LegalFormType.None.ToSelectionItemListAsync();
            var categoryBookTypes = await CategoryBookType.None.ToSelectionItemListAsync();

            var traderList = await _traderService.GetAllTradersAsync(type);
            var tradelModelList = traderList.Select(x => x.ToTraderModel()).ToList();
            var traders = from e in tradelModelList
                          from ct in customerTypes.Where(x => x.Value == e.CustomerTypeId).DefaultIfEmpty()
                          from lft in legalFormTypes.Where(x => x.Value == e.LegalFormTypeId).DefaultIfEmpty()
                          from cbt in categoryBookTypes.Where(x => x.Value == e.CategoryBookTypeId).DefaultIfEmpty()
                          select new TraderLookupItem
                          {
                              Id = e.Id,
                              FullName = e.FullName() ?? "",
                              Vat = e.Vat ?? "",
                              Email = string.Join(", ", (new List<string> { e.Email, e.Email2, e.Email3 }).Where(x => !string.IsNullOrEmpty(x)).ToList()),
                              CustomerTypeId = e.CustomerTypeId,
                              CustomerTypeName = ct?.Label ?? "",
                              LegalFormTypeId = e.LegalFormTypeId,
                              LegalFormTypeName = lft?.Label ?? "",
                              CategoryBookTypeId = e.CategoryBookTypeId,
                              CategoryBookTypeName = cbt?.Label ?? "",
                              ConnectionAccountingActive = e.ConnectionAccountingActive,
                              HyperPayrollId = e.HyperPayrollId,
                              Active = !e.Deleted && e.Active,
                              HasFinancialObligation = e.HasFinancialObligation
                          };

            traders = traders.OrderBy(x => x.FullName).ToList();

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<TraderLookupModel>(1, nameof(TraderLookupModel.FullName), width: 300),
                ColumnConfig.Create<TraderLookupModel>(2, nameof(TraderLookupModel.Vat), width: 100),
                ColumnConfig.Create<TraderLookupModel>(3, nameof(TraderLookupModel.Email), width: 150),
                ColumnConfig.Create<TraderLookupModel>(4, nameof(TraderLookupModel.CustomerTypeName), width: 100),
                ColumnConfig.Create<TraderLookupModel>(5, nameof(TraderLookupModel.LegalFormTypeName), width: 100),
                ColumnConfig.Create<TraderLookupModel>(6, nameof(TraderLookupModel.CategoryBookTypeName), width: 100)

            };

            if (type.Equals(FieldConfigType.Payroll)) {
                var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };
                columns.Insert(3, ColumnConfig.Create<TraderLookupModel>(4, nameof(TraderLookupModel.HyperPayrollId), width: 80, style: rightAlign)); 
            }
            else
                columns.Add(ColumnConfig.Create<TraderLookupModel>(7, nameof(TraderLookupModel.ConnectionAccountingActive), ColumnType.Checkbox, width: 80));

            var field = FieldConfig.Create<T>(property, FieldType.MultiColumnComboBox, options: traders,
                columns: columns, placeholder: placeholder, valueProp: valueProp, labelProp: labelProp, hideLabel: hideLabel);

            return field;
        }

        public async Task<Dictionary<string, object>> GetEmployersGridSelect<T>(string property, string hideExpression = null) where T : class
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            var employers = await _dataProvider.QueryAsync<EmployerLookupItem>(result.Connection, _employersQuery);

            var list = employers.Select(x => new SelectionItemList { Label = $"{x.FullName()} - {x.Vat}", Value = x.CompanyId }).ToList();

            var field = string.IsNullOrEmpty(hideExpression)
                ? FieldConfig.Create<T>(property, FieldType.GridSelect, options: list)
                : FieldConfig.Create<T>(property, FieldType.GridSelect, options: list, hideExpression: hideExpression);

            return field;
        }

        public async Task<IList<EmployerLookupItem>> GetEmployers()
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            var employers = await _dataProvider.QueryAsync<EmployerLookupItem>(result.Connection, _employersQuery);

            return employers;
        }

        public async Task<Dictionary<string, object>> GetEmployersMultiColumnComboBox<T>(string property) where T : class
        {
            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            var employers = await _dataProvider.QueryAsync<EmployerLookupItem>(result.Connection, _employersQuery);

            var list = employers.Select(x => new SelectionItemList { Label = $"{x.FullName()} - {x.Vat}", Value = x.CompanyId }).ToList();

            var field = FieldConfig.Create<T>(property, FieldType.MultiSelectAll, options: list);

            return field;
        }

        public async Task<IList<Dictionary<string, object>>> _GetEmployersMultiColumnComboBox<T>(string property,
            string valueProp = "id", string labelProp = "fullName") where T : class
        {
            var placeholder = await _localizationService.GetResourceAsync("App.Common.Choice");

            var result = await _connectionService.GetConnectionAsync(SqlConnectionType.HyperM);
            var list = await _dataProvider.QueryAsync<EmployerLookupItem>(result.Connection, _employersQuery);

            var employers = list.Select(x =>
            {
                var model = new EmployerLookupItem();
                model.CompanyId = x.CompanyId;
                model.LastName = x.FullName();
                model.Vat = x.Vat;

                return model;
            }).ToList();

            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<EmployerLookupItem>(1, nameof(EmployerLookupItem.CompanyId)),
                ColumnConfig.Create<EmployerLookupItem>(2, nameof(EmployerLookupItem.LastName), width: 350),
                ColumnConfig.Create<EmployerLookupItem>(3, nameof(EmployerLookupItem.Vat))
            };

            var field = FieldConfig.Create<T>(property, FieldType.MultiColumnComboBox, options: employers,
                columns: columns, placeholder: placeholder, valueProp: valueProp, labelProp: labelProp);

            return new List<Dictionary<string, object>> { field };
        }
    }
}