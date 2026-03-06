using App.Core.Infrastructure;
using App.Core.Infrastructure.Dtos.Financial;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Automation.Pages.Financial
{
    public class EfkaRetrievePage : PageTest
    {
        private readonly string _homePage = "https://www.idika.org.gr/EfkaServices/Account/GsisOAuth2Authenticate.aspx";
        //private readonly string _loginPage = "https://apps.e-efka.gov.gr/eAccess/login.xhtml";
        //private readonly string _taxisnetLoginPage = "https://www1.gsis.gr/oauth2server/login.jsp";
        private readonly string _taxisnetNextPage = "https://www1.gsis.gr/oauth2server/oauth/authorize";
        private readonly string _efkaLoginPage = "https://www.idika.org.gr/EfkaServices/Account/SocSecAuthenticate.aspx";
        //private readonly string _efkaMainPage = "https://apps.e-efka.gov.gr/eAccess/index.xhtml";
        //private readonly string _contributionsMainPage = "https://www.idika.org.gr/EfkaServices/Application/Contributions.aspx";
        private readonly string _debtsMainPage = "https://www.idika.org.gr/EfkaServices/Application/Debts.aspx";

        private ILocator _nonSalaryLocator;
        private ILocator _taxisnetNextLocator;
        private ILocator _userNameLocator;
        private ILocator _passwordLocator;
        private ILocator _taxisLoginButtonLocator;
        private ILocator _sendButtonLocator;
        private ILocator _vatInputLocator;
        private ILocator _amkaInputLocator;
        private ILocator _loginButtonLocator;
        private ILocator _currentRfLocator;
        private ILocator _debts2020TableLocator;
        private ILocator _debts2019TableLocator;
        private ILocator _messageLocator;
        private ILocator _submitLogin;

        public EfkaRetrievePage(string connectionId) : base(connectionId: connectionId, headless: false)
        {
            _nonSalaryLocator = Page.GetByText("Εισφορές Μη Μισθωτών");
            _taxisnetNextLocator = Page.GetByText("Συνέχεια στο TAXISNET");
            _userNameLocator = Page.Locator("[id=v]");
            _passwordLocator = Page.Locator("[id=j_password]");
            _taxisLoginButtonLocator = Page.Locator("[id=btn-login-submit]");
            _sendButtonLocator = Page.GetByText("Αποστολή");
            //_vatInputLocator = Page.Locator("//html/body/div[1]/div/div/div[2]/form/div[1]/div/div/div[1]/span/input");
            _vatInputLocator = Page.Locator("[id=ContentPlaceHolder1_ASPxFormLayout1_ASPxFormLayout1_E2AFM_I]");
            _amkaInputLocator = Page.Locator("[id=ContentPlaceHolder1_ASPxFormLayout1_ASPxFormLayout1_E1AMKA_I]");
            _loginButtonLocator = Page.Locator("[id=ContentPlaceHolder1_ASPxFormLayout1_ASPxFormLayout1_E2btnEisodos_CD]");
            _currentRfLocator = Page.Locator("[id=ContentPlaceHolder1_panelOikEkkr_ePaymentRFPanel_ctl03_elabelRFcode]");
            //_currentRfLocator = Page.Locator("//html/body/form/div[3]/div[1]/div/section/div/div/div/table[1]/tbody/tr[2]/td/div/div/div[2]/div/div[2]/div/div/div[2]/span");
            _debts2020TableLocator = Page.Locator("//html/body/form/div[3]/div[1]/div/section/div/div/div[1]/table[1]/tbody/tr/td/table[2]/tbody/tr");
            _debts2019TableLocator = Page.Locator("//html/body/form/div[3]/div[1]/div/section/div/div/div[1]/table[2]/tbody/tr/td/table[2]/tbody/tr");
            _messageLocator = Page.Locator("//html/body/form/div[3]/div[1]/div/section/div/div/div[2]/div/div[2]/div/div/table/tbody/tr/td[1]/table/tbody/tr/td[2]/div/div/span");
            _submitLogin = Page.Locator("[id=\"ContentPlaceHolder1_btnGGPSAuth_CD\"]");
        }

        protected override async Task LogoutAsync()
        {
            if (!LoginIn)
                return;

            var _currentUserLocator = Page.Locator("[id=\"TopPanel_NavMenu_DXI2_T\"]");
            var _logoutButtonLocator = Page.GetByText("Αποσύνδεση");

            await ClickAsync(_currentUserLocator);
            await ClickAsync(_logoutButtonLocator);
        }

        public async Task Login(string userName, string password, string vat, string amka)
        {
            await GotoToUrl(Page, _homePage);
            await ClickAsync(_submitLogin);

            await WaitForAsync(_taxisLoginButtonLocator, 300000);

            await FillAsync(_userNameLocator, userName);
            await FillAsync(_passwordLocator, password);
            await ClickToUrl(_taxisLoginButtonLocator, _taxisnetNextPage, _efkaLoginPage);

            if (Page.Url.Contains(_taxisnetNextPage))
            {
                await WaitForAsync(_sendButtonLocator, 300000);
                await ClickAsync(_sendButtonLocator);
            }

            await WaitForAsync(_loginButtonLocator, 300000);
            await FillAsync(_amkaInputLocator, amka);

            await ClickAsync(_loginButtonLocator);

            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            var errorMessageLocator = Page.Locator("[id=ContentPlaceHolder1_cbpAMKA_errdiv]");
            if (await IsLocatorExist(errorMessageLocator))
                throw new AppPlaywrightException($"Αποτυχία Ταυτοποίησης ΑΦΜ: {vat}");
            else
                LoginIn = true;
        }

        public async Task<IList<FinancialObligationDto>> Execute(int traderId)
        {
            var list = new List<FinancialObligationDto>();

            var eisforaPanel = Page.Locator("[id=ContentPlaceHolder1_panelOikEkkr_eEisforaPanel]");
            await WaitForAsync(eisforaPanel, 300000);

            if (!await IsVisibleAsync(_currentRfLocator))
                return list;

            var paymentDateLocator = Page.Locator("[id=ContentPlaceHolder1_panelOikEkkr_eEisforaPanel_BootstrapFormLayouteEFKA_elabelPaymentExpireDate]");

            //ΕΦΚΑ ΜΗ ΜΙΣΘΩΤΩΝ 
            var currentRF = await InnerTextAsync(_currentRfLocator);
            var paymentDate = (await InnerTextAsync(paymentDateLocator)).ToDateGR();

            await Page.GotoAsync(_debtsMainPage);

            var secondTable = Page.Locator("[id=ContentPlaceHolder1_GridDebts_DXMainTable]");
            await WaitForAsync(secondTable, 300000);

            if (await IsVisibleAsync(_messageLocator))
                await ClickAsync(_messageLocator);

            list.AddRange(await GetRows(_debts2020TableLocator, currentRF, paymentDate, traderId));//ContentPlaceHolder1_GridE20Debts_DXMainTable
            list.AddRange(await GetRows(_debts2019TableLocator, currentRF, paymentDate, traderId));//ContentPlaceHolder1_GridDebts_DXMainTable

            return list;
        }

        public async Task<List<FinancialObligationDto>> GetRows(ILocator locator, string currentRf, DateTime paymentDate, int traderId)
        {
            var currentDate = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Remove specific indexes of rows 
            var rows = await GetAllAsync(locator);
            int[] num = { 0, 1, rows.Count - 1 };

            rows = rows.Where((x, i) => !num.Contains(i)).ToList();

            var debts = new List<FinancialObligationDto>();

            foreach (var row in rows)
            {
                var cols = await GetAllAsync(row.Locator("> td"));

                if (cols.Count < 3)
                    break;
                else
                {
                    debts.Add(new FinancialObligationDto
                    {
                        Institution = "ΕΦΚΑ Μη μισθωτών",
                        PaymentType = await cols[0].InnerTextAsync(),
                        PaymentValue = (await cols[1].InnerTextAsync()).Split(' ')[0].ToDecimal(),
                        PaymentIdentity = currentRf,
                        PaymentDate = paymentDate,
                        //PaymentDate = lastDayOfMonth.CheckPaymentDate(),
                        TraderId = traderId,
                        Period = currentDate.Month
                    });
                }
            }

            return debts;
        }
    }
}
