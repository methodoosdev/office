using App.Core.Infrastructure.Dtos.Accounting;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace App.Automation.Pages.Accounting
{
    public class ESendPage : PageTest
    {
        private readonly string _eSendLaunchingPage = "https://www1.gsis.gr/tameiakes/myweb/esend.php";
        private readonly string _loginPage = "https://www1.gsis.gr/tameiakes/myweb/esendN.php?FUNCTION=1";
        private readonly string _homePage = "https://www1.gsis.gr/tameiakes/myweb/esendN.php?FUNCTION=6";
        private readonly string _tameiakesPage = "https://www1.gsis.gr/tameiakes/myweb/esendN.php?FUNCTION=101&CLF=1";

        private ILocator _companyButtonLocator;
        private ILocator _userNameLocator;
        private ILocator _passwordLocator;
        private ILocator _tameiakesLocator;
        private ILocator _tameiakesTableLocator;

        public ESendPage(string connectionId) : base(connectionId: connectionId, headless: false)
        {
            _companyButtonLocator = Page.Locator("//html/body/section/div/button[3]");
            _userNameLocator = Page.Locator("[name=EMAIL]");
            _passwordLocator = Page.Locator("[name=PASSWD]");
            _tameiakesLocator = Page.Locator("//html/body/section/div/div[2]/div[1]/div/div/div/button[1]");
            _tameiakesTableLocator = Page.Locator("//html/body/section/div/div[4]/div[2]/div/table/tbody/tr");
        }

        protected override async Task LogoutAsync()
        {
            var _exitLocator = Page.Locator("//html/body/header/div/ul[2]/li/a");
            await ClickAsync(_exitLocator);

            var _logoutLocator = Page.Locator("//html/body/header/div/ul[2]/li/ul/li/a");
            var _logoutPage = "https://www1.gsis.gr/tameiakes/myweb/esendN.php?FUNCTION=7";
            await ClickToUrl(_logoutLocator, _logoutPage);
        }

        public async Task<bool> Login(string userName, string password)
        {
            await GotoToUrl(Page, _eSendLaunchingPage);

            await ClickToUrl(_companyButtonLocator, _loginPage);

            // Login
            await FillAsync(_userNameLocator, userName);
            await FillAsync(_passwordLocator, password);

            await ClickToUrl(Page, "//html/body/section/div/form/table/tbody/tr[3]/td[2]/input[1]", _homePage);

            return true;
        }
        public async Task<List<ESendDto>> Execute(DateTime period)
        {
            var result = new List<ESendDto>();

            await ClickToUrl(_tameiakesLocator, _tameiakesPage);

            var selectPageLocator = Page.Locator("[name=fhmTable_length]");
            await SelectOptionAsync(selectPageLocator, "50");

            var tameiakesRows = await GetAllAsync(_tameiakesTableLocator);

            if (tameiakesRows.Count == 0)
                throw new AppPlaywrightException("Tameiakes not exist.");

            var cols0Locator = await GetLocatorAsync(tameiakesRows[0], "> td");
            var tameiakesCols = await GetAllAsync(cols0Locator);
            if (tameiakesCols.Count == 1)
                throw new AppPlaywrightException("Tameiakes not exist.");

            var tameiakesList = new List<string>();

            foreach (var row in tameiakesRows)
            {
                var rowLocator = await GetLocatorAsync(row, "> td");
                var rowCols = await GetAllAsync(rowLocator);
                var tameiakiValue = await InnerTextAsync(rowCols[2]);
                tameiakesList.Add(tameiakiValue);
            }

            var tameiakesPage = "https://www1.gsis.gr/tameiakes/myweb/esendN.php?FUNCTION=110&F_EAFDSS={0}&CLF=1";
            foreach (var tameiaki in tameiakesList)
            {
                await GotoToUrl(Page, string.Format(tameiakesPage, tameiaki));

                await RepeatSettings(period.ToString("yyyy-MM")); // Close alert, paging, sorting, filtering

                var rowsLocator = Page.Locator("//html/body/section/div/div/div[2]/div/table/tbody/tr");
                var rowsList = await GetAllAsync(rowsLocator);

                var zhtalist = new List<(string zhta, string date)>();

                foreach (var row in rowsList)
                {
                    var rowLocator = await GetLocatorAsync(row, "> td");
                    var cols = await GetAllAsync(rowLocator);
                    if (cols.Count == 1)
                    {
                        break;
                    }
                    else
                    {
                        var zhtaValue = await InnerTextAsync(cols[0]);
                        var dateValue = await InnerTextAsync(cols[2]);
                        zhtalist.Add((zhtaValue, dateValue));
                    }
                }

                if (zhtalist.Count > 0)
                {
                    var zhtaPage = "https://www1.gsis.gr/tameiakes/myweb/esendN.php?FUNCTION=142&RECORDID={0};{1}";

                    foreach (var item in zhtalist)
                    {
                        await GotoToUrl(Page, string.Format(zhtaPage, tameiaki, item.zhta));

                        var zhtaTotalLocator = Page.Locator("//html/body/section/div/button[1]");
                        await ClickAsync(zhtaTotalLocator);

                        var valueA6 = Page.Locator("//html/body/section/div/div/div/div/div[2]/table/tbody/tr[2]/td[2]");
                        var valueB13 = Page.Locator("//html/body/section/div/div/div/div/div[2]/table/tbody/tr[2]/td[3]");
                        var valueC24 = Page.Locator("//html/body/section/div/div/div/div/div[2]/table/tbody/tr[2]/td[4]");
                        var valueD36 = Page.Locator("//html/body/section/div/div/div/div/div[2]/table/tbody/tr[2]/td[5]");
                        var valueE0 = Page.Locator("//html/body/section/div/div/div/div/div[2]/table/tbody/tr[2]/td[6]");

                        result.Add(new ESendDto
                        {
                            ZhtaNo = item.zhta,
                            Tameiaki = tameiaki,
                            Date = DateTime.Parse(item.date),
                            A6 = await GetValue(valueA6),
                            B13 = await GetValue(valueB13),
                            C24 = await GetValue(valueC24),
                            D36 = await GetValue(valueD36),
                            E0 = await GetValue(valueE0),
                        });

                        var closePopupLocator = Page.Locator("//html/body/section/div/div[2]/div/div/div[1]/button");
                        await ClickAsync(closePopupLocator);

                        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                    }
                }
            }

            return result;
        }

        public async Task RepeatSettings(string searchDate)
        {
            var alertCloseButtonLocator = Page.Locator("//html/body/section/div/div[1]/button");
            var existButtton = await alertCloseButtonLocator.CountAsync();
            if (existButtton > 0)
                await ClickAsync(alertCloseButtonLocator);

            var selectEntriesLocator = Page.Locator("[name=mainTable_length]");

            await SelectOptionAsync(selectEntriesLocator, "50");

            var searchLocator = Page.Locator("//html/body/section/div/div/div[1]/div[2]/div/label/input");
            await FillAsync(searchLocator, searchDate);

            var orderByLocator = Page.Locator("//html/body/section/div/div/div[2]/div/table/thead/tr/th[1]");
            await ClickAsync(orderByLocator);
        }

        public async Task<decimal> GetValue(ILocator locator)
        {
            var value = await InnerTextAsync(locator);

            return Convert.ToDecimal(value, new CultureInfo("en-US"));
        }
    }
}
