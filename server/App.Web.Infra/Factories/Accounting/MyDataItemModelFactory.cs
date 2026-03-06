using App.Core;
using App.Core.Domain.Accounting;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Framework.Infrastructure.Mapper.Extensions;
using App.Framework.Models.Extensions;
using App.Models.Accounting;
using App.Models.Traders;
using App.Services;
using App.Services.Accounting;
using App.Services.Configuration;
using App.Services.Helpers;
using App.Services.Localization;
using App.Services.Traders;
using App.Web.Infra.Queries.Accounting;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Web.Infra.Factories.Accounting
{
    public partial interface IMyDataItemModelFactory
    {
        Task<MyDataItemSearchModel> PrepareMyDataItemSearchModelAsync(MyDataItemSearchModel searchModel);
        Task<MyDataItemListModel> PrepareMyDataItemListModelAsync(MyDataItemSearchModel infoModel, int traderId, bool isIssuer, int docTypeId, string connection);
        Task<MyDataItemModel> PrepareMyDataItemModelAsync(MyDataItemModel model, MyDataItem myDataItem, MyDataItemInfoModel infoModel, string traderName, string traderVat);
        Task<MyDataItemFormModel> PrepareMyDataItemFormModelAsync(MyDataItemFormModel formModel, MyDataItemInfoModel infoModel, string connection, int companyId);
        Task<MyDataItemInfoFormModel> PrepareMyDataItemInfoFormModelAsync(MyDataItemInfoFormModel infoFormModel);
    }
    public partial class MyDataItemModelFactory : IMyDataItemModelFactory
    {
        private readonly IMyDataItemService _myDataItemService;
        private readonly ITraderService _traderService;
        private readonly ILocalizationService _localizationService;
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IAppDataProvider _dataProvider;
        private readonly IWorkContext _workContext;

        public MyDataItemModelFactory(
            IMyDataItemService myDataItemService,
            ITraderService traderService,
            ILocalizationService localizationService,
            IFieldConfigService fieldConfigService,
            IDateTimeHelper dateTimeHelper,
            IAppDataProvider dataProvider,
            IWorkContext workContext)
        {
            _myDataItemService = myDataItemService;
            _traderService = traderService;
            _localizationService = localizationService;
            _fieldConfigService = fieldConfigService;
            _dateTimeHelper = dateTimeHelper;
            _dataProvider = dataProvider;
            _workContext = workContext;
        }

        private async Task<IPagedList<MyDataItemModel>> GetPagedListAsync(MyDataItemSearchModel searchModel, int traderId, bool isIssuer, int docTypeId, string connection)
        {
            var issuer = await _localizationService.GetResourceAsync("App.Common.Issuer");
            var counterPart = await _localizationService.GetResourceAsync("App.Common.CounterPart");

            var _trader = await _traderService.GetTraderByIdAsync(traderId);
            var trader = _trader.ToTraderModel();

            //var docTypes = await MyDataDocType.Sales.ToSelectionItemListAsync();
            var vatCategories = MyDataResources.VatCategory.Select(x => new SelectionItemList { Value = x.Key, Label = x.Value }).ToList();
            var taxTypes = MyDataResources.TaxType.Select(x => new SelectionItemList { Value = x.Key, Label = x.Value }).ToList();
            var taxCategories = MyDataResources.TaxCategory.Select(x => new SelectionItemList { Value = x.Key, Label = x.Value }).ToList();
            var paymentMethods = MyDataResources.PaymentMethodType.Select(x => new SelectionItemList { Value = x.Key, Label = x.Value }).ToList();
            var invoiceTypes = MyDataResources.InvoiceType.Select(x => new SelectionList { Value = x.Key, Label = x.Value }).ToList();

            var vatProvisionList = await _dataProvider.QueryAsync<MyDataVatProvision>(connection, MyDataQuery.VatProvisions);
            var vatProvisions = vatProvisionList.Select(x => new SelectionItemList { Value = x.Code, Label = $"{x.Code} {x.Description}" }).ToList();

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", _trader.CompanyId);

            IList<SelectionList> counterpartList = null;
            if (!isIssuer)
            {
                var customerList = await _dataProvider.QueryAsync<MyDataTrader>(connection, MyDataQuery.Customers, pCompanyId);
                counterpartList = customerList.Select(x => new SelectionList { Value = x.Vat, Label = x.Description }).ToList();
            }
            else
            {
                var supplierList = await _dataProvider.QueryAsync<MyDataTrader>(connection, MyDataQuery.Suppliers, pCompanyId);
                counterpartList = supplierList.Select(x => new SelectionList { Value = x.Vat, Label = x.Description }).ToList();
            }

            IList<SelectionItemList> seriesList = null;
            if (docTypeId == (int)MyDataDocType.Sales)
            {
                var salesSeriesList = await _dataProvider.QueryAsync<MyDataSeries>(connection, MyDataQuery.SalesSeries, pCompanyId);
                seriesList = salesSeriesList.Select(x => new SelectionItemList { Value = x.Code, Label = x.Description }).ToList();
            }
            else if (docTypeId == (int)MyDataDocType.Purchases)
            {
                var purchasesSeriesList = await _dataProvider.QueryAsync<MyDataSeries>(connection, MyDataQuery.PurchasesSeries, pCompanyId);
                seriesList = purchasesSeriesList.Select(x => new SelectionItemList { Value = x.Code, Label = x.Description }).ToList();
            }
            else if (docTypeId == (int)MyDataDocType.Expenses)
            {
                var expensesSeriesList = await _dataProvider.QueryAsync<MyDataSeries>(connection, MyDataQuery.ExpensesSeries, pCompanyId);
                seriesList = expensesSeriesList.Select(x => new SelectionItemList { Value = x.Code, Label = x.Description }).ToList();
            }

            IList<MyDataProduct> productList = null;
            IList<SelectionList> products = null;
            if (docTypeId == (int)MyDataDocType.Expenses)
            {
                productList = await _dataProvider.QueryAsync<MyDataProduct>(connection, MyDataQuery.SpecialSuppliers, pCompanyId);
                products = productList.Select(x => new SelectionList { Value = x.Code, Label = x.Description }).ToList();
            }
            else
            {
                productList = await _dataProvider.QueryAsync<MyDataProduct>(connection, MyDataQuery.Products, pCompanyId);
                products = productList.Select(x => new SelectionList { Value = x.Code, Label = x.Description }).ToList();
            }

            var query = _myDataItemService.Table
                .SelectAwait(async x =>
                {
                    var prCode = productList.FirstOrDefault(f => f.Code == x.ProductCode);
                    var model = x.ToModel<MyDataItemModel>();
                    model.LastDateOn = await _dateTimeHelper.ConvertToUserTimeAsync(x.LastDateOnUtc, DateTimeKind.Utc);
                    model.PaymentMethodName = paymentMethods.FirstOrDefault(f => f.Value == x.PaymentMethodId)?.Label ?? "";
                    model.InvoiceTypeName = invoiceTypes.FirstOrDefault(f => f.Value == x.InvoiceType)?.Label ?? "";
                    model.VatProvisionName = vatProvisions.FirstOrDefault(f => f.Value == x.VatProvisionId)?.Label ?? "";
                    //model.DocTypeName = docTypes.FirstOrDefault(f => f.Value == x.DocTypeId)?.Label ?? "";
                    model.TraderName = trader.FullName();
                    model.IsIssuerName = isIssuer ? issuer : counterPart;
                    model.SeriesName = seriesList.FirstOrDefault(f => f.Value == x.SeriesId)?.Label ?? "";
                    model.ProductCodeName = products.FirstOrDefault(f => f.Value == x.ProductCode)?.Label ?? "";
                    model.VatName = x.VatId > 0 ? $"({prCode.VatId}) {prCode.VatName}" : string.Empty;
                    model.CurrencyName = x.CurrencyId > 0 ? $"({prCode.CurrencyId}) {prCode.CurrencyName}" : string.Empty;

                    if (!string.IsNullOrEmpty(x.CounterpartVat))
                        model.CounterpartName = counterpartList.FirstOrDefault(f => f.Value == x.CounterpartVat)?.Label ?? "";

                    if (x.TaxCategoryId > 0)
                        model.TaxCategoryName = taxCategories.FirstOrDefault(f => f.Value == x.TaxCategoryId)?.Label ?? "";

                    if (x.VatCategoryId > 0)
                        model.VatCategoryName = vatCategories.FirstOrDefault(f => f.Value == x.VatCategoryId)?.Label ?? "";

                    return model;
                });

            query = query.Where(x =>
                x.TraderVat == trader.Vat &&
                x.IsIssuer == isIssuer &&
                x.DocTypeId == docTypeId);

            if (!string.IsNullOrEmpty(searchModel.QuickSearch))
            {
                query = query.Where(c => c.TraderName.ContainsIgnoreCase(searchModel.QuickSearch));
            }

            query = query.OrderByAsync(searchModel.SortField.ToPascalCase(), searchModel.SortOrder);

            return await query.ToPagedListAsync(searchModel.Page - 1, searchModel.PageSize);
        }

        public virtual async Task<MyDataItemSearchModel> PrepareMyDataItemSearchModelAsync(MyDataItemSearchModel searchModel)
        {
            //prepare page parameters
            searchModel.Columns = CreateKendoGridColumnConfig();
            searchModel.SetGridPageSize();
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);

            searchModel.Title = await _localizationService.GetResourceAsync("App.Models.MyDataItemModel.ListForm.Title"); 
            searchModel.PagerSettings = new PagerSettings(searchModel.AvailablePageSizes);
            searchModel.DataKey = "id";

            return searchModel;
        }

        public virtual async Task<MyDataItemListModel> PrepareMyDataItemListModelAsync(MyDataItemSearchModel searchModel, int traderId, bool isIssuer, int docTypeId, string connection)
        {
            var list = await GetPagedListAsync(searchModel, traderId, isIssuer, docTypeId, connection);

            //prepare grid model
            var model = new MyDataItemListModel().PrepareToGrid(searchModel, list);

            return model;
        }

        public virtual async Task<MyDataItemModel> PrepareMyDataItemModelAsync(MyDataItemModel model, MyDataItem myDataItem, MyDataItemInfoModel infoModel, string traderName, string traderVat)
        {
            var issuer = await _localizationService.GetResourceAsync("App.Common.Issuer");
            var counterPart = await _localizationService.GetResourceAsync("App.Common.CounterPart");

            var docTypes = await MyDataDocType.Sales.ToSelectionItemListAsync();

            if (myDataItem != null)
            {
                //fill in model values from the entity
                model ??= myDataItem.ToModel<MyDataItemModel>();
            }

            if (myDataItem == null)
            {
                model.LastDateOnUtc = DateTime.UtcNow;
                model.TraderVat = traderVat;
                model.IsIssuer = infoModel.IsIssuer;
                model.DocTypeId = infoModel.DocTypeId;
                model.InvoiceType = string.Empty;
                model.PaymentMethodId = 1;
                model.VatProvisionId = 0;
                model.VatCategoryId = 1;
                model.TaxCategoryId = 1;
            }

            model.TraderName = traderName;
            model.DocTypeName = docTypes.FirstOrDefault(f => f.Value == infoModel.DocTypeId)?.Label ?? "";
            model.IsIssuerName = infoModel.IsIssuer ? issuer : counterPart;

            return model;
        }

        private List<ColumnConfig> CreateKendoGridColumnConfig()
        {
            var centerAlign = new Dictionary<string, string> { ["text-align"] = "center" };
            var rightAlign = new Dictionary<string, string> { ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<MyDataItemModel>(1, nameof(MyDataItemModel.LastDateOn), ColumnType.DateTime, style: centerAlign, hidden: true),
                ColumnConfig.Create<MyDataItemModel>(2, nameof(MyDataItemModel.InvoiceTypeName), ColumnType.RouterLink),
                ColumnConfig.Create<MyDataItemModel>(3, nameof(MyDataItemModel.Series)),
                ColumnConfig.Create<MyDataItemModel>(3, nameof(MyDataItemModel.Branch), style: rightAlign, hidden: true),
                ColumnConfig.Create<MyDataItemModel>(4, nameof(MyDataItemModel.PaymentMethodName)),
                ColumnConfig.Create<MyDataItemModel>(5, nameof(MyDataItemModel.VatProvisionName), hidden: true),
                //ColumnConfig.Create<MyDataItemModel>(6, nameof(MyDataItemModel.DocTypeName)),
                ColumnConfig.Create<MyDataItemModel>(7, nameof(MyDataItemModel.CounterpartName)),
                ColumnConfig.Create<MyDataItemModel>(8, nameof(MyDataItemModel.SeriesName)),
                ColumnConfig.Create<MyDataItemModel>(9, nameof(MyDataItemModel.VatCategoryName)),
                ColumnConfig.Create<MyDataItemModel>(10, nameof(MyDataItemModel.TaxCategoryName)),
                ColumnConfig.Create<MyDataItemModel>(11, nameof(MyDataItemModel.ProductCodeName)),
                ColumnConfig.Create<MyDataItemModel>(12, nameof(MyDataItemModel.VatName), hidden: true),
                ColumnConfig.Create<MyDataItemModel>(13, nameof(MyDataItemModel.CurrencyName), hidden: true),
            };

            return columns;
        }

        public virtual async Task<MyDataItemFormModel> PrepareMyDataItemFormModelAsync(MyDataItemFormModel formModel, MyDataItemInfoModel infoModel, string connection, int companyId)
        {
            var issuer = await _localizationService.GetResourceAsync("App.Common.Issuer");
            var counterPart = await _localizationService.GetResourceAsync("App.Common.CounterPart");
            var invoiceDevider = await _localizationService.GetResourceAsync("App.Models.MyDataItemModel.Dividers.Invoice");
            var invoiceDetailsDevider = await _localizationService.GetResourceAsync("App.Models.MyDataItemModel.Dividers.InvoiceDetails");

            var isIssuer = infoModel.IsIssuer;
            //var trader = await _traderService.GetTraderByIdAsync(infoModel.TraderId);
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);

            IList<SelectionList> counterpartList = null;
            if (!isIssuer)
            {
                var customerList = await _dataProvider.QueryAsync<MyDataTrader>(connection, MyDataQuery.Customers, pCompanyId);
                counterpartList = customerList.Select(x => new SelectionList { Value = x.Vat, Label = x.Description }).ToList();
            }
            else
            {
                var supplierList = await _dataProvider.QueryAsync<MyDataTrader>(connection, MyDataQuery.Suppliers, pCompanyId);
                counterpartList = supplierList.Select(x => new SelectionList { Value = x.Vat, Label = x.Description }).ToList();
            }

            IList<SelectionItemList> seriesList = null;
            if (infoModel.DocTypeId == (int)MyDataDocType.Sales)
            {
                var salesSeriesList = await _dataProvider.QueryAsync<MyDataSeries>(connection, MyDataQuery.SalesSeries, pCompanyId);
                seriesList = salesSeriesList.Select(x => new SelectionItemList { Value = x.Code, Label = x.Description }).ToList();
            }
            else if (infoModel.DocTypeId == (int)MyDataDocType.Purchases)
            {
                var purchasesSeriesList = await _dataProvider.QueryAsync<MyDataSeries>(connection, MyDataQuery.PurchasesSeries, pCompanyId);
                seriesList = purchasesSeriesList.Select(x => new SelectionItemList { Value = x.Code, Label = x.Description }).ToList();
            }
            else if (infoModel.DocTypeId == (int)MyDataDocType.Expenses)
            {
                var expensesSeriesList = await _dataProvider.QueryAsync<MyDataSeries>(connection, MyDataQuery.ExpensesSeries, pCompanyId);
                seriesList = expensesSeriesList.Select(x => new SelectionItemList { Value = x.Code, Label = x.Description }).ToList();
            }


            //var customerList = await _dataProvider.QueryAsync<MyDataTrader>(connection, MyDataQuery.Customers, pCompanyId);
            //var customers = customerList.Select(x => new SelectionList { Value = x.Vat, Label = x.Description }).ToList();
            //customers.Insert(0, new SelectionList { Value = "", Label = "" });

            //var supplierList = await _dataProvider.QueryAsync<MyDataTrader>(connection, MyDataQuery.Suppliers, pCompanyId);
            //var suppliers = supplierList.Select(x => new SelectionList { Value = x.Vat, Label = x.Description }).ToList();
            //suppliers.Insert(0, new SelectionList { Value = "", Label = "" });

            //var docTypes = await MyDataDocType.Sales.ToSelectionItemListAsync();
            //if (isIssuer)
            //    docTypes = new List<SelectionItemList> { docTypes[1], docTypes[2] };
            //else
            //    docTypes = new List<SelectionItemList> { docTypes[0] };

            var vatCategories = MyDataResources.VatCategory.Select(x => new SelectionItemList { Value = x.Key, Label = x.Value }).ToList();
            var taxTypes = MyDataResources.TaxType.Select(x => new SelectionItemList { Value = x.Key, Label = x.Value }).ToList();
            var taxCategories = MyDataResources.TaxCategory.Select(x => new SelectionItemList { Value = x.Key, Label = x.Value }).ToList();
            var paymentMethods = MyDataResources.PaymentMethodType.Select(x => new SelectionItemList { Value = x.Key, Label = x.Value }).ToList();
            var invoiceTypes = MyDataResources.InvoiceType.Select(x => new SelectionList { Value = x.Key, Label = x.Value }).ToList();

            var vatProvisionList = await _dataProvider.QueryAsync<MyDataVatProvision>(connection, MyDataQuery.VatProvisions);
            var vatProvisions = vatProvisionList.Select(x => new SelectionItemList { Value = x.Code, Label = $"{x.Code} {x.Description}" }).ToList();
            vatProvisions.Insert(0, new SelectionItemList { Value = 0, Label = "Καμμία" });

            //var purchasesSeriesList = await _dataProvider.QueryAsync<MyDataSeries>(connection, MyDataQuery.PurchasesSeries, pCompanyId);
            //var purchases = purchasesSeriesList.Select(x => new SelectionItemList { Value = x.Code, Label = x.Description }).ToList();
            //var expensesSeriesList = await _dataProvider.QueryAsync<MyDataSeries>(connection, MyDataQuery.ExpensesSeries, pCompanyId);
            //var expenses = expensesSeriesList.Select(x => new SelectionItemList { Value = x.Code, Label = x.Description }).ToList();
            //var salesSeriesList = await _dataProvider.QueryAsync<MyDataSeries>(connection, MyDataQuery.SalesSeries, pCompanyId);
            //var sales = salesSeriesList.Select(x => new SelectionItemList { Value = x.Code, Label = x.Description }).ToList();

            //var productList = await _dataProvider.QueryAsync<MyDataProduct>(connection, MyDataQuery.Products, pCompanyId);
            //var products = productList.Select(x => new SelectionList { Value = x.Code, Label = x.Description }).ToList();

            //var specialSupplierList = await _dataProvider.QueryAsync<MyDataProduct>(connection, MyDataQuery.SpecialSuppliers, pCompanyId);
            //var specialSuppliers = specialSupplierList.Select(x => new SelectionList { Value = x.Code, Label = x.Description }).ToList();

            IList<SelectionList> products = null;
            if (infoModel.DocTypeId == (int)MyDataDocType.Expenses)
            {
                var specialSupplierList = await _dataProvider.QueryAsync<MyDataProduct>(connection, MyDataQuery.SpecialSuppliers, pCompanyId);
                products = specialSupplierList.Select(x => new SelectionList { Value = x.Code, Label = x.Description }).ToList();
            }
            else
            {
                var productList = await _dataProvider.QueryAsync<MyDataProduct>(connection, MyDataQuery.Products, pCompanyId);
                products = productList.Select(x => new SelectionList { Value = x.Code, Label = x.Description }).ToList();
            }

            var leftTop = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<MyDataItemModel>(nameof(MyDataItemModel.TraderVat), FieldType.Text, _readonly: true),
                FieldConfig.Create<MyDataItemModel>(nameof(MyDataItemModel.TraderName), FieldType.Text, _readonly: true)
            };

            var rightTop = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<MyDataItemModel>(nameof(MyDataItemModel.IsIssuerName), FieldType.Text, _readonly: true),
                FieldConfig.Create<MyDataItemModel>(nameof(MyDataItemModel.DocTypeName), FieldType.Text, _readonly: true)
            };

            var left = new List<Dictionary<string, object>>()
            {
                FieldConfig.CreateDivider(invoiceDevider),
                FieldConfig.Create<MyDataItemModel>(nameof(MyDataItemModel.Series), FieldType.Text),
                FieldConfig.Create<MyDataItemModel>(nameof(MyDataItemModel.LastDateOnUtc), FieldType.Date),
                FieldConfig.Create<MyDataItemModel>(nameof(MyDataItemModel.CounterpartVat), FieldType.GridSelect, options: counterpartList),
                FieldConfig.Create<MyDataItemModel>(nameof(MyDataItemModel.InvoiceType), FieldType.GridSelect, options: invoiceTypes),
                FieldConfig.Create<MyDataItemModel>(nameof(MyDataItemModel.Branch), FieldType.Numeric),
                FieldConfig.Create<MyDataItemModel>(nameof(MyDataItemModel.SeriesId), FieldType.GridSelect, options: seriesList),
                FieldConfig.Create<MyDataItemModel>(nameof(MyDataItemModel.PaymentMethodId), FieldType.Select, options: paymentMethods),
                FieldConfig.Create<MyDataItemModel>(nameof(MyDataItemModel.VatProvisionId), FieldType.Select, options: vatProvisions),
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.CreateDivider(invoiceDetailsDevider),
                FieldConfig.Create<MyDataItemModel>(nameof(MyDataItemModel.VatCategoryId), FieldType.Select, options: vatCategories),
                FieldConfig.Create<MyDataItemModel>(nameof(MyDataItemModel.TaxCategoryId), FieldType.Select, options: taxCategories),
                FieldConfig.Create<MyDataItemModel>(nameof(MyDataItemModel.ProductCode), FieldType.GridSelect, options: products),
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-6", "col-12 md:col-6", "col-12 md:col-6" }, leftTop, rightTop, left, right);

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.MyDataItemModel.EditForm.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return formModel;
        }

        public virtual async Task<MyDataItemInfoFormModel> PrepareMyDataItemInfoFormModelAsync(MyDataItemInfoFormModel infoFormModel)
        {
            var docTypes = await MyDataDocType.Sales.ToSelectionItemListAsync();

            var issuer = await _localizationService.GetResourceAsync("App.Common.Issuer");
            var counterPart = await _localizationService.GetResourceAsync("App.Common.CounterPart");

            var left = new List<Dictionary<string, object>>()
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<MyDataItemInfoModel>(nameof(MyDataItemInfoModel.TraderId), FieldConfigType.OnlySoftone)
            };

            var middle = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<MyDataItemInfoModel>(nameof(MyDataItemInfoModel.IsIssuer), FieldType.Switch, style: "width: 94px;", onLabel: counterPart, offLabel: issuer)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<MyDataItemInfoModel>(nameof(MyDataItemInfoModel.DocTypeId), FieldType.Select, options: docTypes)
            };

            var listButton = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<MyDataItemInfoModel>("ListButton", FieldType.Button, themeColor: "primary",
                className: "col-12 text-right p-5", disableExpression: "!(model.traderId > 0)", 
                label: await _localizationService.GetResourceAsync("App.Common.List"))
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-2", "col-12 md:col-2", "col-12 md:col-2" }, left, middle, right, listButton);

            infoFormModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return infoFormModel;
        }
    }
}