using App.Core.Infrastructure.Dtos.Accounting;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace App.Automation.Pages.Accounting
{
    public class MyDataPage : PageTest
    {
        private readonly string _launchingPage = "https://www1.aade.gr/saadeapps2/bookkeeper-web/bookkeeper/#!/invoiceSearch";
        private readonly string _loginPage = "https://login.gsis.gr/mylogin/login.jsp";
        private readonly string _homePage = "https://www1.aade.gr/saadeapps2/bookkeeper-web/bookkeeper/";

        private ILocator _userNameLocator;
        private ILocator _passwordLocator;
        private ILocator _submitLocator;
        private ILocator _searchCriteriaLocator;

        public MyDataPage(string connectionId) : base(connectionId: connectionId, headless: false)
        {
            _userNameLocator = Page.Locator("[name=username]");
            _passwordLocator = Page.Locator("[name=password]");
            _submitLocator = Page.Locator("[name=btn_login]");
            _searchCriteriaLocator = Page.Locator("[id=searchCriteria]");
        }

        protected override async Task LogoutAsync()
        {
            var _exitLocator = Page.Locator("//*[@id=\"myAccountSpan\"]");
            await ClickAsync(_exitLocator);

            var _logoutLocator = Page.Locator("div.btn.myAccountBtn.logout");
            await ClickToUrl(_logoutLocator, _loginPage);
        }

        public async Task<bool> Login(string userName, string password)
        {
            await GotoToUrl(Page, _launchingPage, _loginPage);

            // Login
            await FillAsync(_userNameLocator, userName);
            await FillAsync(_passwordLocator, password);

            await ClickToUrl(_submitLocator, _homePage);

            return true;
        }
        public async Task<IList<MyDataResult>> Execute()
        {
            var result = new List<MyDataResult>();

            var results = new List<MyDataInExport>();

            await ClickToUrl(Page, "div.welcomeLink.welcomeBtn3", "https://www1.aade.gr/saadeapps2/bookkeeper-web/bookkeeper/invoiceSearch.html");

            var searchCriteriaLocator = await GetLocatorAsync(_searchCriteriaLocator);

            var fromDateLocator = searchCriteriaLocator.Locator("[id=fromDate]");
            var toDateLocator = searchCriteriaLocator.Locator("[id=toDate]");

            await fromDateLocator.FillAsync("14/10/2023");

            await toDateLocator.FillAsync("24/10/2023");

            var selectLocator = (await GetAllAsync(searchCriteriaLocator.Locator("select"))).Last();

            await SelectOptionAsync(selectLocator, "number:2");

            await ClickAsync(Page.GetByRole(AriaRole.Button).And(Page.GetByText("Αναζήτηση")));
            //await Task.Delay(500).ConfigureAwait(false);

            var successModal = Page.Locator("div.successModal");
            await successModal.WaitForAsync();

            var success = await IsLocatorExist(successModal);

            await ClickAsync(Page.GetByRole(AriaRole.Button).And(Page.GetByText("Κλείσιμο")));

            var rowsLocator = Page.Locator("//*[@id=\"context\"]").Locator("table.searchResults tbody tr");
            var rows = await GetAllAsync(rowsLocator);

            foreach (var row in rows)
            {
                await row.ClickAsync();
                var data = await GetCurrentPageAsync();
                results.Add(data);

                await ClickAsync(Page.Locator(".backButton button"));
            }

            return result;
        }

        public async Task<IList<InvoiceDetail>> GetInvoiceDetailsAsync()
        {
            var invoiceRowDetails = Page.Locator("//*[@id=\"invoiceDetailsSection\"] >> > div > div.invoiceRowDetails");
            var invoiceDetailsLocator = await GetAllAsync(invoiceRowDetails);

            var invoiceDetails = new List<InvoiceDetail>();

            foreach (var itemLocator in invoiceDetailsLocator)
            {
                var netValueLocator = itemLocator.Locator(".groupedElements .netValue input");
                var netValue = await InputValueAsync(netValueLocator);

                var vatCategoryLocator = itemLocator.Locator(".groupedElements .vatCategory select option[selected=selected]");
                var vatCategory = await GetAttributeAsync(vatCategoryLocator, "label");

                var vatAmountLocator = itemLocator.Locator(".groupedElements .vatAmount input");
                var vatAmount = await InputValueAsync(vatAmountLocator);

                var invoiceDetail = new InvoiceDetail(netValue, vatCategory, vatAmount);
                invoiceDetails.Add(invoiceDetail);
            }

            return invoiceDetails;
        }
        public async Task<MyDataInExport> GetCurrentPageAsync()
        {
            var parentAfmLocator = Page.Locator("table.mitrooTable tbody tr td >> nth=0");
            await parentAfmLocator.WaitForAsync();

            var parentAfm = await InnerTextAsync(parentAfmLocator);
            var mark = await InnerTextAsync(Page.Locator("#invoiceTypeSelection .invoiceElement.mark span >> nth=1"));
            var series = await InputValueAsync(Page.Locator("#series.invoiceElement input"));
            var invoiceNo = await InputValueAsync(Page.Locator("#invoiceNumber.invoiceElement input"));
            var date = await InputValueAsync(Page.Locator("#issueDate.invoiceElement input"));
            var afm = await InputValueAsync(Page.Locator("#issuerSection .invoiceElement.vatNumber input"));
            var surname = await InputValueAsync(Page.Locator("#issuerSection .invoiceElement.name input"));

            var myData = new MyDataInExport();
            myData.ParentAfm = parentAfm.Trim();
            myData.Afm = afm.Trim();
            myData.Mark = mark.Trim();
            myData.Document = series.Trim() + "-" + invoiceNo.Trim();
            myData.DocDate = Convert.ToDateTime(date.Trim(), new CultureInfo("el-GR"));
            myData.Surname = surname.Trim();
            myData.Details = await GetInvoiceDetailsAsync();

            return myData;
        }


        public async Task<decimal> GetValue(ILocator locator)
        {
            var value = await InnerTextAsync(locator);

            return Convert.ToDecimal(value, new CultureInfo("en-US"));
        }
    }
    public class InvoiceDetail
    {
        public InvoiceDetail(string netValue, string vatCategory, string vatAmount)
        {
            NetValue = Convert.ToDecimal(netValue.Trim(), new CultureInfo("el-GR"));
            VatAmount = Convert.ToDecimal(vatAmount.Trim(), new CultureInfo("el-GR"));
            VatCategory = vatCategory.Trim();
        }

        public decimal NetValue { get; set; }
        public string VatCategory { get; set; }
        public decimal VatAmount { get; set; }
    }
    public class MyDataInExport
    {
        public string ParentAfm { get; set; }
        public string Afm { get; set; }
        public string Vat { get; set; }
        public string Mark { get; set; }

        public string Document { get; set; }
        public string Surname { get; set; }
        public DateTime DocDate { get; set; }
        public IList<InvoiceDetail> Details { get; set; }
    }
}
