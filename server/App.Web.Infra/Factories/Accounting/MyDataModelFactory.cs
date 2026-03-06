using App.Core;
using App.Core.Infrastructure;
using App.Data.DataProviders;
using App.Framework.Components;
using App.Models.Accounting;
using App.Services;
using App.Services.Accounting;
using App.Services.Common;
using App.Services.Configuration;
using App.Services.Localization;
using App.Web.Infra.Queries.Accounting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace App.Web.Accounting.Factories
{
    public partial interface IMyDataModelFactory
    {
        Task<MyDataInfoModel> PrepareMyDataInfoModelAsync(MyDataInfoModel infoModel);
        Task<MyDataInfoFormModel> PrepareMyDataInfoFormModelAsync(MyDataInfoFormModel infoFormModel);
        Task<MyDataTableModel> PrepareMyDataTableModelAsync(MyDataTableModel tableModel);
        Task<MyDataDialogFormModel> PrepareMyDataDialogFormModelAsync(MyDataDialogFormModel formModel, bool isIssuer, int companyId, string connection);
        Task<MyDataDetailTableModel> PrepareMyDataDetailTableModelAsync(MyDataDetailTableModel tableModel);
        Task<MyDataDetailDialogFormModel> PrepareMyDataDetailDialogFormModelAsync(MyDataDetailDialogFormModel formModel, int companyId, string connection);
        Task<IList<MyDataModel>> PrepareMyDataModelListAsync(bool isIssuer, DateTime startDate, DateTime endDate, string content, int companyId, string connection, string traderVat);

        MyDataRequestItem PrepareMyDataRequest(bool isIssuer, DateTime start, DateTime end, string userName, string password);
        bool CheckIfMyDataTooMuch(string content);
    }
    public class MyDataModelFactory : IMyDataModelFactory
    {
        private readonly IMyDataItemService _myDataItemService;
        private readonly IFieldConfigService _fieldConfigService;
        private readonly IAppDataProvider _dataProvider;
        private readonly ILocalizationService _localizationService;

        public MyDataModelFactory(
            IMyDataItemService myDataItemService,
            IFieldConfigService fieldConfigService,
            IAppDataProvider dataProvider,
            ILocalizationService localizationService)
        {
            _myDataItemService = myDataItemService;
            _fieldConfigService = fieldConfigService;
            _dataProvider = dataProvider;
            _localizationService = localizationService;
        }

        public virtual Task<MyDataInfoModel> PrepareMyDataInfoModelAsync(MyDataInfoModel infoModel)
        {
            var date = DateTime.UtcNow.AddMonths(-1);
            var startDate = new DateTime(date.Year, date.Month, 1);
            infoModel.StartDate = startDate;
            infoModel.EndDate = startDate.AddMonths(1).AddDays(-1);
            infoModel.IsIssuer = true;

            return Task.FromResult(infoModel);
        }

        public async Task<MyDataInfoFormModel> PrepareMyDataInfoFormModelAsync(MyDataInfoFormModel infoFormModel)
        {
            var issuer = await _localizationService.GetResourceAsync("App.Common.Issuer");
            var counterPart = await _localizationService.GetResourceAsync("App.Common.CounterPart");

            var left = new List<Dictionary<string, object>>()
            {
                await _fieldConfigService.GetTradersMultiColumnComboBox<MyDataInfoModel>(nameof(MyDataInfoModel.TraderId), FieldConfigType.Accounting)
            };

            var middle1 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<MyDataInfoModel>(nameof(MyDataInfoModel.StartDate), FieldType.Date)
            };

            var middle2 = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<MyDataInfoModel>(nameof(MyDataInfoModel.EndDate), FieldType.Date)
            };

            var right = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<MyDataInfoModel>(nameof(MyDataInfoModel.IsIssuer), FieldType.Switch, style: "width: 94px;", onLabel: counterPart, offLabel: issuer)
            };

            var fields = FieldConfig.CreateFields(new string[] { "col-12 md:col-6", "col-12 md:col-2", "col-12 md:col-2", "col-12 md:col-2" }, left, middle1, middle2, right);

            infoFormModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));

            return infoFormModel;
        }

        public virtual async Task<MyDataTableModel> PrepareMyDataTableModelAsync(MyDataTableModel tableModel)
        {
            var colorStyle0 = new Dictionary<string, string> { ["background-color"] = "aliceblue", ["text-align"] = "center" };
            var colorStyle1 = new Dictionary<string, string> { ["background-color"] = "aliceblue" };
            var colorStyle2 = new Dictionary<string, string> { ["background-color"] = "aliceblue", ["color"] = "#9C27B0" };
            var colorStyle3 = new Dictionary<string, string> { ["background-color"] = "aliceblue", ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<MyDataModel>(0, nameof(MyDataModel.Id), style : colorStyle3, width: 60),
                ColumnConfig.Create<MyDataModel>(1, nameof(MyDataModel.Register), ColumnType.Checkbox, style : colorStyle1, width: 100, filterable: true, filterType: "boolean"),
                ColumnConfig.Create<MyDataModel>(1, nameof(MyDataModel.ExportToExcel), ColumnType.Checkbox, style : colorStyle1, width: 100, filterable: true, filterType: "boolean"),
                ColumnConfig.Create<MyDataModel>(1, nameof(MyDataModel.InvoiceTypeName), style : colorStyle1),
                ColumnConfig.Create<MyDataModel>(2, nameof(MyDataModel.IssueDate), ColumnType.Date, style : colorStyle1, filterable: true, filterType: "date", format: "d"),
                ColumnConfig.Create<MyDataModel>(3, nameof(MyDataModel.Series), style : colorStyle1),
                ColumnConfig.Create<MyDataModel>(4, nameof(MyDataModel.Aa), style : colorStyle1, filterable: true),
                ColumnConfig.Create<MyDataModel>(5, nameof(MyDataModel.Branch), style : colorStyle0),
                ColumnConfig.Create<MyDataModel>(6, nameof(MyDataModel.VatNumber), style : colorStyle1, filterable: true),
                ColumnConfig.Create<MyDataModel>(7, nameof(MyDataModel.TraderName), style : colorStyle1, filterable: true),
                ColumnConfig.Create<MyDataModel>(9, nameof(MyDataModel.TotalNetValue), ColumnType.Decimal, style : colorStyle3, filterable: true, filterType: "numeric"),
                ColumnConfig.Create<MyDataModel>(10, nameof(MyDataModel.TotalVatAmount), ColumnType.Decimal, style: colorStyle3),
                ColumnConfig.Create<MyDataModel>(11, nameof(MyDataModel.TotalGrossValue), ColumnType.Decimal, style: colorStyle3),
                ColumnConfig.Create<MyDataModel>(8, nameof(MyDataModel.PaymentMethodName), style: colorStyle2),
                ColumnConfig.Create<MyDataModel>(13, nameof(MyDataModel.SeriesName), style: colorStyle2, filterable: true),
                ColumnConfig.Create<MyDataModel>(12, nameof(MyDataModel.VatProvisionName), style: colorStyle2)
            };

            tableModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.MyDataModel.Title"));
            tableModel.CustomProperties.Add("columns", columns);

            return tableModel;
        }

        public virtual async Task<MyDataDialogFormModel> PrepareMyDataDialogFormModelAsync(MyDataDialogFormModel formModel, bool isIssuer, int companyId, string connection)
        {
            var docTypes = await MyDataDocType.Sales.ToSelectionItemListAsync();
            if (isIssuer)
                docTypes.RemoveAt(0);
            else
                docTypes.RemoveRange(1, 2);
            var paymentMethods = MyDataResources.PaymentMethodType.Select(x => new SelectionItemList { Value = x.Key, Label = x.Value }).ToList();

            var vatProvisionList = await _dataProvider.QueryAsync<MyDataVatProvision>(connection, MyDataQuery.VatProvisions);
            var vatProvisions = vatProvisionList.Select(x => new SelectionItemList { Value = x.Code, Label = x.Description }).ToList();

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
            var query = isIssuer ? MyDataQuery.PurchasesSeries : MyDataQuery.SalesSeries;
            var seriesList = await _dataProvider.QueryAsync<MyDataSeries>(connection, query, pCompanyId);
            var expensesList = isIssuer ? await _dataProvider.QueryAsync<MyDataSeries>(connection, MyDataQuery.ExpensesSeries, pCompanyId) : null;
            var series = seriesList.Select(x => new SelectionItemList { Value = x.Code, Label = x.Description }).ToList();
            var expenses = expensesList?.Select(x => new SelectionItemList { Value = x.Code, Label = x.Description }).ToList();

            var fields = new List<Dictionary<string, object>> 
            {
                FieldConfig.Create<MyDataModel>(nameof(MyDataModel.DocTypeId), FieldType.Select, options: docTypes),
                FieldConfig.Create<MyDataModel>(nameof(MyDataModel.PaymentMethodId), FieldType.GridSelect, options: paymentMethods),
                FieldConfig.Create<MyDataModel>(nameof(MyDataModel.SeriesId), FieldType.GridSelect),
                FieldConfig.Create<MyDataModel>(nameof(MyDataModel.VatProvisionId), FieldType.GridSelect, options: vatProvisions),
                FieldConfig.Create<MyDataModel>(nameof(MyDataModel.ExportToExcel), FieldType.Checkbox)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.MyDataDialogFormModel.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));
            formModel.CustomProperties.Add("series", series);
            formModel.CustomProperties.Add("expenses", expenses);

            return formModel;
        }

        public virtual Task<MyDataDetailTableModel> PrepareMyDataDetailTableModelAsync(MyDataDetailTableModel tableModel)
        {
            var colorStyle1 = new Dictionary<string, string> { ["background-color"] = "#F9FBE7", ["color"] = "#9C27B0" };
            var colorStyle2 = new Dictionary<string, string> { ["background-color"] = "#F9FBE7" };
            var colorStyle3 = new Dictionary<string, string> { ["background-color"] = "#F9FBE7", ["text-align"] = "right" };

            var columns = new List<ColumnConfig>()
            {
                ColumnConfig.Create<MyDataDetailModel>(1, nameof(MyDataDetailModel.VatCategoryName), style : colorStyle2),
                ColumnConfig.Create<MyDataDetailModel>(2, nameof(MyDataDetailModel.TaxCategoryName), style : colorStyle2),
                ColumnConfig.Create<MyDataDetailModel>(3, nameof(MyDataDetailModel.NetValue), ColumnType.Decimal, style: colorStyle3),
                ColumnConfig.Create<MyDataDetailModel>(4, nameof(MyDataDetailModel.VatAmount), ColumnType.Decimal, style: colorStyle3),
                ColumnConfig.Create<MyDataDetailModel>(6, nameof(MyDataDetailModel.ProductCodeName), style : colorStyle1),
                //ColumnConfig.Create<MyDataDetailModel>(7, nameof(MyDataDetailModel.VatName), style: colorStyle1),
                //ColumnConfig.Create<MyDataDetailModel>(8, nameof(MyDataDetailModel.CurrencyName), style: colorStyle1)
            };

            tableModel.CustomProperties.Add("columns", columns);

            return Task.FromResult(tableModel);
        }

        public virtual async Task<MyDataDetailDialogFormModel> PrepareMyDataDetailDialogFormModelAsync(MyDataDetailDialogFormModel formModel, int companyId, string connection)
        {
            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
            var productList = await _dataProvider.QueryAsync<MyDataProduct>(connection, MyDataQuery.Products, pCompanyId);
            var specialSupplierList = await _dataProvider.QueryAsync<MyDataProduct>(connection, MyDataQuery.SpecialSuppliers, pCompanyId);

            var products = productList.Select(x => new SelectionList { Value = x.Code, Label = x.Description }).ToList();
            var specialSuppliers = specialSupplierList.Select(x => new SelectionList { Value = x.Code, Label = x.Description }).ToList();

            var fields = new List<Dictionary<string, object>>()
            {
                FieldConfig.Create<MyDataDetailModel>(nameof(MyDataDetailModel.ProductCode), FieldType.GridSelect)
            };

            formModel.CustomProperties.Add("title", await _localizationService.GetResourceAsync("App.Models.MyDataDetailDialogFormModel.Title"));
            formModel.CustomProperties.Add("fields", FieldConfig.CreateFields(fields, "col-12"));
            formModel.CustomProperties.Add("products", products);
            formModel.CustomProperties.Add("specialSuppliers", specialSuppliers);

            return formModel;
        }

        public virtual async Task<IList<MyDataModel>> PrepareMyDataModelListAsync(bool isIssuer, DateTime startDate, DateTime endDate, string content, int companyId, string connection, string traderVat)
        {
            var list = GetMyDataInvoices(content);
            list = list.OrderBy(x => Convert.ToDateTime(x.invoiceHeader.issueDate)).ToList();

            var pCompanyId = new LinqToDB.Data.DataParameter("pCompanyId", companyId);
            var query = isIssuer ? MyDataQuery.Suppliers : MyDataQuery.Customers;
            var traders = await _dataProvider.QueryAsync<MyDataTrader>(connection, query, pCompanyId);

            var exlcudeInvoiceTypes = new string[] 
            {
                "3.1", "3.2", "6.1", "6.2", "7.1", "8.1", "8.2", "11.1", "11.2", "11.3", "11.4", "11.5", "13.1", "13.2",
                "13.3", "13.4", "13.31", "14.31", "15.1", "17.1", "17.2", "17.3", "17.4", "17.5", "17.6"
            };

            var index = 1;
            var detailIndex = 1;
            var myDataModels = new List<MyDataModel>();

            foreach (var item in list)
            {
                var invoiceType = item.invoiceHeader.invoiceType;
                var invoiceTypeName = MyDataResources.InvoiceType.TryGetValue(invoiceType, out string invoiceTypeValue) ? invoiceTypeValue : "Σφάλμα";
                int? paymentMethod = item.paymentMethods?.paymentMethodDetails.First().type;
                var paymentMethodName = paymentMethod.HasValue ? MyDataResources.PaymentMethodType.TryGetValue(paymentMethod.Value, out string paymentMethodValue) ? paymentMethodValue : "Σφάλμα" : null;
                var vatNumber = isIssuer ? item.issuer.vatNumber : item.counterpart?.vatNumber;

                var traderName = string.Empty;
                var traderCode = string.Empty;
                if (!exlcudeInvoiceTypes.Contains(invoiceType))
                {
                    var trader = traders.Where(x => x.Vat == vatNumber).FirstOrDefault();
                    traderName = trader?.Description ?? "";
                    traderCode = trader?.Code ?? "";
                }

                var model = new MyDataModel();

                model.Id = index;
                model.InvoiceType = invoiceType;
                model.IssueDate = item.invoiceHeader.issueDate;
                model.Series = item.invoiceHeader.series;
                model.Aa = item.invoiceHeader.aa;
                model.VatNumber = vatNumber;
                model.Branch = isIssuer ? item.counterpart.branch : item.issuer?.branch ?? -1;
                model.Mark = item.mark;
                model.PaymentMethodId = paymentMethod.HasValue ? paymentMethod.Value : 0;
                model.TotalNetValue = item.invoiceSummary.totalNetValue;
                model.TotalVatAmount = item.invoiceSummary.totalVatAmount;
                model.TotalGrossValue = item.invoiceSummary.totalGrossValue;
                model.InvoiceTypeName = invoiceTypeName;
                model.TraderName = traderName;
                model.TraderCode = traderCode;
                model.PaymentMethodName = paymentMethodName;
                model.IsIssuer = isIssuer;

                foreach (var val in item.invoiceDetails)
                {
                    var taxCategoryKey = GetTaxCategory(val.recType, val.feesPercentCategory, val.vatCategory);
                    var taxCategoryName = MyDataResources.TaxCategory.TryGetValue(taxCategoryKey, out string taxCategoryValue) ? taxCategoryValue : "Σφάλμα";

                    var vatCategoryName = MyDataResources.VatCategory.TryGetValue(val.vatCategory, out string vatCategoryValue) ? vatCategoryValue : "Σφάλμα";

                    var detail = new MyDataDetailModel();

                    detail.Id = detailIndex;
                    detail.ParentId = index;
                    detail.VatCategoryId = val.vatCategory;
                    detail.VatCategoryName = vatCategoryName;
                    detail.NetValue = val.netValue;
                    detail.VatAmount = val.vatAmount;

                    detail.TaxCategoryId = taxCategoryKey;
                    detail.TaxCategoryName = taxCategoryName;
                    detail.DocTypeId = isIssuer ? (int)MyDataDocType.Purchases : (int)MyDataDocType.Sales;

                    model.Details.Add(detail);
                    detailIndex++;
                }

                if (item.taxesTotals != null)
                    foreach (var val in item.taxesTotals.taxes)
                    {
                        var taxCategoryKey = GetTaxCategory(val.taxType,val.taxCategory);
                        var taxCategoryName = MyDataResources.TaxCategory.TryGetValue(taxCategoryKey, out string taxCategoryValue) ? taxCategoryValue : "Σφάλμα";

                        var detail = new MyDataDetailModel();

                        detail.Id = detailIndex;
                        detail.ParentId = index;
                        detail.TaxCategoryId = taxCategoryKey;
                        detail.TaxCategoryName = taxCategoryName;
                        detail.NetValue = val.taxAmount;
                        detail.DocTypeId = isIssuer ? (int)MyDataDocType.Expenses : (int)MyDataDocType.Sales;

                        model.Details.Add(detail);
                        detailIndex++;
                    }

                var docTypeId = isIssuer ? (int)MyDataDocType.Purchases : (int)MyDataDocType.Sales;
                if (model.Details.Any(x => x.DocTypeId == (int)MyDataDocType.Expenses))
                    docTypeId = (int)MyDataDocType.Expenses;
                model.DocTypeId = docTypeId;
                model.DocTypeName = await _localizationService.GetLocalizedEnumAsync((MyDataDocType)docTypeId);

                myDataModels.Add(model);
                index++;
            }

            IList<MyDataInvoice> invoices = new List<MyDataInvoice>();
            var pStartDate = new LinqToDB.Data.DataParameter("pStartDate", startDate.ToString("yyyyMMdd"));
            var pEndDate = new LinqToDB.Data.DataParameter("pEndDate", endDate.ToString("yyyyMMdd"));

            if (isIssuer)
            {
                var expenses = await _dataProvider.QueryAsync<MyDataInvoice>(connection, MyDataQuery.Expenses, pCompanyId, pStartDate, pEndDate);
                invoices = await _dataProvider.QueryAsync<MyDataInvoice>(connection, MyDataQuery.Purchases, pCompanyId, pStartDate, pEndDate);

                invoices = invoices.Concat(expenses).ToList();
            }
            else
                invoices = await _dataProvider.QueryAsync<MyDataInvoice>(connection, MyDataQuery.Sales, pCompanyId, pStartDate, pEndDate);

            foreach (var x in myDataModels)
            {

                var register =
                    invoices.Any(k => 
                        //(x.Mark.ToString() == k.Mark?.ToString()) ||
                        x.IssueDate == k.InvoiceDate && x.VatNumber == k.Vat && x.TotalNetValue == k.NetAmount);

                x.Register = register;
                x.ExportToExcel = !register;
            }

            var salesSeriesList = await _dataProvider.QueryAsync<MyDataSeries>(connection, MyDataQuery.SalesSeries, pCompanyId);
            var salesSeries = salesSeriesList.Select(x => new SelectionItemList { Value = x.Code, Label = x.Description }).ToList();

            var purchasesSeriesList = await _dataProvider.QueryAsync<MyDataSeries>(connection, MyDataQuery.PurchasesSeries, pCompanyId);
            var purchasesSeries = purchasesSeriesList.Select(x => new SelectionItemList { Value = x.Code, Label = x.Description }).ToList();

            var expensesSeriesList = await _dataProvider.QueryAsync<MyDataSeries>(connection, MyDataQuery.ExpensesSeries, pCompanyId);
            var expensesSeries = expensesSeriesList.Select(x => new SelectionItemList { Value = x.Code, Label = x.Description }).ToList();

            var specialSuppliersList = await _dataProvider.QueryAsync<MyDataProduct>(connection, MyDataQuery.SpecialSuppliers, pCompanyId);
            var specialSuppliers = specialSuppliersList.Select(x => new SelectionList { Value = x.Code, Label = x.Description }).ToList();

            var productList = await _dataProvider.QueryAsync<MyDataProduct>(connection, MyDataQuery.Products, pCompanyId);
            var products = productList.Select(x => new SelectionList { Value = x.Code, Label = x.Description }).ToList();

            foreach (var model in myDataModels)
            {
                var items = await _myDataItemService.GetMyDataItemByAsync(traderVat, model.VatNumber, model.InvoiceType,
                    model.Series, model.PaymentMethodId);

                foreach (var detail in model.Details)
                {
                    var item = items.Where(x => x.VatCategoryId == detail.VatCategoryId && x.TaxCategoryId == detail.TaxCategoryId).FirstOrDefault();
                    if (item != null)
                    {
                        detail.ProductCode = item.ProductCode;
                        detail.ProductCodeName = (MyDataDocType)detail.DocTypeId switch
                        {
                            MyDataDocType.Sales => products.FirstOrDefault(f => f.Value == item.ProductCode)?.Label ?? "",
                            MyDataDocType.Purchases => products.FirstOrDefault(f => f.Value == item.ProductCode)?.Label ?? "",
                            MyDataDocType.Expenses => specialSuppliers.FirstOrDefault(f => f.Value == item.ProductCode)?.Label ?? "",
                            _ => throw new NopException($"Not supported MyDataDocType name: '{model.DocTypeId}'"),
                        };
                        model.SeriesId = item.SeriesId;
                        model.SeriesName = (MyDataDocType)model.DocTypeId switch
                        {
                            MyDataDocType.Sales => salesSeries.FirstOrDefault(f => f.Value == item.SeriesId)?.Label ?? "",
                            MyDataDocType.Purchases => purchasesSeries.FirstOrDefault(f => f.Value == item.SeriesId)?.Label ?? "",
                            MyDataDocType.Expenses => expensesSeries.FirstOrDefault(f => f.Value == item.SeriesId)?.Label ?? "",
                            _ => throw new NopException($"Not supported MyDataDocType name: '{model.DocTypeId}'"),
                        };
                    }
                }
            }

            return myDataModels;
        }

        private int GetTaxCategory(int taxType, int taxCategory, int vatCategory = 0)
        {
            var taxCategoryId = taxType * 100 + taxCategory;

            if (taxCategoryId == 501)
                return 599;
            if (taxCategoryId == 316 && vatCategory > 0)
                return 397;
            if (taxCategoryId == 317 && vatCategory > 0)
                return 398;
            if (taxCategoryId == 318 && vatCategory > 0)
                return 399;

            return taxCategoryId;
        }
        public virtual MyDataRequestItem PrepareMyDataRequest(bool isIssuer, DateTime start, DateTime end, string userName, string password)
        {
            var errors = new AppErrorResult();

            var firstDay = start.ToString("dd/MM/yyyy");
            var lastDay = end.ToString("dd/MM/yyyy");

            var requestDocs = isIssuer ? MyDataRequestDefaults.RequestDocs : MyDataRequestDefaults.RequestTransmittedDocs;

            var response = ExecuteRequest(requestDocs, firstDay, lastDay, userName, password);

            return new MyDataRequestItem
            {
                Status = (int)response.StatusCode, IsSuccessful = response.IsSuccessful, Content = response.Content
            };
        }

        private RestResponse ExecuteRequest(string requestDocs, string start, string end, string userName, string password)
        {
            //Εκδοτης,Issuer RequestDocs
            //Ληπτης,CounterPart RequestTransmittedDocs
            var url = $"https://mydatapi.aade.gr/myDATA/{requestDocs}?mark=0&dateFrom={start}&dateTo={end}";
            // Your RestSharp client
            var client = new RestClient(url);

            // Your RestRequest
            var request = new RestRequest()
                //.AddHeader("Accept", "application/xml")
                .AddHeader("aade-user-id", userName)
                .AddHeader("ocp-apim-subscription-key", password);

            // Execute the request
            var response = client.Execute(request);

            return response;
        }

        public bool CheckIfMyDataTooMuch(string content)
        {
            var obj = JsonConvert.DeserializeObject(content);

            var node = SerializeXmlNode(obj.ToString());

            JObject jobj = JObject.Parse(node);

            var json = jobj.SelectToken("RequestedDoc.continuationToken");

            return json != null;
        }
        private IList<invoiceResult> GetMyDataInvoices(string content)
        {
            var list = new List<invoiceResult>();

            var obj = JsonConvert.DeserializeObject(content);

            var node = SerializeXmlNode(obj.ToString());

            JObject jobj = JObject.Parse(node);

            var json = jobj.SelectToken("RequestedDoc.invoicesDoc.invoice");

            if (json == null)
                return list;

            JToken token = JToken.Parse(json.ToString());

            if (token.Type == JTokenType.Array)
            {
                list.AddRange(token.ToObject<List<invoiceResult>>());
            }
            else if (token.Type == JTokenType.Object)
            {
                list.Add(token.ToObject<invoiceResult>());
            }

            foreach (var item in list)
            {
                var invoiceDetails = item.invoiceDetails?.GroupBy(g => new { g.vatCategory, g.recType, g.feesPercentCategory })
                    .Select(x =>
                    {
                        var model = new invoiceRowResult();
                        model.vatCategory = x.Key.vatCategory;
                        model.recType = x.Key.recType;
                        model.feesPercentCategory = x.Key.feesPercentCategory;
                        model.vatAmount = x.Sum(x => x.vatAmount);
                        model.netValue = x.Sum(x => x.netValue);

                        return model;
                    })
                    .ToList();

                item.invoiceDetails = invoiceDetails;

                var taxes = item.taxesTotals?.taxes.GroupBy(g => new { g.taxType, g.taxCategory })
                    .Select(x =>
                    {
                        var model = new taxesResult();

                        model.taxType = x.Key.taxType;
                        model.taxCategory = x.Key.taxCategory;
                        model.taxAmount = x.Sum(x => x.taxAmount);
                        model.underlyingValue = x.Sum(x => x.underlyingValue);

                        return model;
                    })
                    .ToList();

                if (taxes != null)
                    item.taxesTotals.taxes = taxes;
            }

            return list;
        }

        private string SerializeXmlNode(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            // Process the XML document as needed
            return JsonConvert.SerializeXmlNode(xmlDoc);
        }
    }

    public class MyDataRequestItem
    {
        public int Status { get; set; }
        public bool IsSuccessful { get; set; }
        public string Content { get; set; }
    }
}