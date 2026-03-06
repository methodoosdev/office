using App.Core.Infrastructure;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Accounting;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Automation.Pages.Accounting
{
    public class ListingF5Page : PageTest
    {
        //TaxisNet
        private readonly string _taxisNetLaunchingPage = "https://www1.aade.gr/taxisnet/mytaxisnet";
        private readonly string _taxisNetLoginPage = "https://login.gsis.gr/mylogin/login.jsp";
        private readonly string _taxisNetHomePage = "https://www1.aade.gr/taxisnet/mytaxisnet/protected/home.htm";
        private readonly string _displayAgentTaxpayerListPage = "https://www1.aade.gr/taxisnet/vat/protected/displayAgentTaxpayerList.htm";
        private readonly string _displayDeclarationTypesPage = "https://www1.aade.gr/taxisnet/vat/protected/displayDeclarationTypes.htm";
        private readonly string _displayLiabilitiesForYearPage = "https://www1.aade.gr/taxisnet/vat/protected/displayLiabilitiesForYear.htm";
        private readonly string _displayActorRolesPage = "https://www1.aade.gr/taxisnet/vat/protected/displayActorRoles.htm";
        private readonly string _displayAccountingOfficesListPage = "https://www1.aade.gr/taxisnet/vat/protected/displayAccountingOfficesList.htm";
        private readonly string _submitFlowPage = "https://www1.aade.gr/taxisnet/vat/protected/submit-flow.htm";

        private ILocator _userNameLocator;
        private ILocator _passwordLocator;
        private ILocator _vatInputLocator;
        private ILocator _searchVatLocator;
        private ILocator _confirmVatLocator;
        private ILocator _vatF5ByYearLocator;
        private ILocator _buttonRoleSelectLocator;
        private ILocator _F5TableLocator;
        private ILocator _buttonAccounntantSelectLocator;
        private ILocator _selectAccountantLocator;
        private ILocator _modifiedF5Locator;
        private ILocator _yearSelectLocator;
        private ILocator _submitF5TableLocator;

        public ListingF5Page(string connectionId) : base(connectionId: connectionId, headless: false)
        {
            _userNameLocator = Page.Locator("[name=username]");
            _passwordLocator = Page.Locator("[name=password]");
            _vatInputLocator = Page.Locator("[name=taxpayerVatNumberPattern]");
            _buttonRoleSelectLocator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[1]/div/div[1]/table/tbody/tr[9]/td");
            _buttonAccounntantSelectLocator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/form/div/table/tbody/tr[4]/td[2]/input");
            _selectAccountantLocator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/form/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td[1]/a");
            _searchVatLocator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/form/table/tbody/tr[4]/td/table/tbody/tr/td/input[1]");
            _confirmVatLocator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/form/table/tbody/tr[6]/td/table/tbody/tr/td/table/tbody/tr/td[1]/a");
            _yearSelectLocator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr[2]/td/table/tbody/tr[5]/td[3]/select");
            _vatF5ByYearLocator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr[2]/td/table/tbody/tr[5]/td[4]/input");
            _F5TableLocator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr");
            _modifiedF5Locator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr/td[8]/table/tbody/tr[2]/td/div");
            _submitF5TableLocator = Page.Locator("//html/body/form[1]/table/tbody/tr[6]/td/table[11]/tbody/tr");
        }

        protected override async Task LogoutAsync()
        {
            await GotoToUrl(Page, _taxisNetHomePage);

            Page.Dialog += async (_, dialog) =>
            {
                await dialog.AcceptAsync();
            };

            var _exitLocator = Page.Locator("//html/body/table/tbody/tr[3]/td/table/tbody/tr/td/table/tbody/tr[1]/td[9]/a");
            await ClickAsync(_exitLocator);
        }

        public async Task<bool> Login(string userName, string password)
        {
            await GotoToUrl(Page, _taxisNetLaunchingPage, _taxisNetLoginPage);

            // Login
            await FillAsync(_userNameLocator, userName);
            await FillAsync(_passwordLocator, password);

            await ClickToUrl(Page, "[name=btn_login]", _taxisNetHomePage);

            return true;
        }
        public async Task Execute(string vat, ListingF5Data model)
        {
            await GotoToUrl(Page, _displayAgentTaxpayerListPage);

            await ClickToUrl(_buttonRoleSelectLocator, _displayActorRolesPage);

            await ClickToUrl(_buttonAccounntantSelectLocator, _displayAccountingOfficesListPage);

            await ClickToUrl(_selectAccountantLocator, _displayAgentTaxpayerListPage);

            await _vatInputLocator.FillAsync(vat);

            await _searchVatLocator.ClickAsync();

            try
            {
                await ClickToUrl(_confirmVatLocator, _displayDeclarationTypesPage);
            }
            catch
            {
                throw new AppPlaywrightException("Δε βρέθηκε εκπρόσωπος.");
            }

            await SelectOptionAsync(_yearSelectLocator, model.Year.ToString());

            await ClickToUrl(_vatF5ByYearLocator, _displayLiabilitiesForYearPage);

            var rows = await _F5TableLocator.AllAsync();

            foreach (var row in rows)
            {
                var columns = await row.Locator("> td").AllAsync();
                var period = await columns[1].InnerTextAsync();
                if (GetMonth(period).Equals(model.Month))
                {
                    var button = columns[3].Locator("> div");

                    if (await button.CountAsync() > 0)
                    {
                        var buttonText = await button.InnerTextAsync();

                        var isModifying = buttonText.Contains("Επεξεργασία");
                        await ClickAsync(button);

                        if (isModifying) // Είναι τροποποιητική
                        {
                            await ClickToUrl(_modifiedF5Locator, _submitFlowPage);
                        }
                    }
                    break;
                }

            }

            await Task.Delay(2000).ConfigureAwait(false);

            async Task<IList<ILocator>> findRows()
            {
                var rows = await _submitF5TableLocator.AllAsync();

                return rows.Skip(2).Take(25).ToList();
            }

            var goodsTotalLocator = Page.Locator("//html/body/form[1]/table/tbody/tr[6]/td/table[11]/tbody/tr[28]/td[3]/input");
            var triangleExchangeTotalLocator = Page.Locator("//html/body/form[1]/table/tbody/tr[6]/td/table[11]/tbody/tr[28]/td[4]/input");
            var servicesTotalLocator = Page.Locator("//html/body/form[1]/table/tbody/tr[6]/td/table[11]/tbody/tr[28]/td[5]/input");
            var insertPageLocator = Page.Locator("//html/body/form[1]/table/tbody/tr[2]/td/table[2]/tbody/tr/td[6]/input");

            //Document opened ...
            decimal goodsTotal = model.Data.Sum(x => x.Goods);
            decimal triangleExchangeTotal = model.Data.Sum(x => x.TriangleExchange);
            decimal servicesTotal = model.Data.Sum(x => x.Services);

            var rows1 = await findRows();

            var pages = Math.Ceiling(model.Data.Count() / 25m) - 1;
            var count = 0;

            for (var p = 0; p <= pages; p++)
            {
                if (p > 0)
                {
                    await ClickAsync(insertPageLocator);
                    rows1 = await findRows();
                }

                var items = model.Data.Skip(p * 25).Take(25).ToList();
                foreach (var item in items)
                {
                    var columns1 = await rows1[count].Locator("> td").AllAsync(); // td of tr

                    if (ListingCountryResources.CountryDict.TryGetValue(item.CountryCode, out string countryCode))
                    {
                        var tagElement = columns1[1].Locator("> select");
                        await SelectOptionAsync(tagElement, countryCode);
                    }
                    else
                        await NewExceptionAsync("App.Playwright.WrongCountrySelection");

                    var afm = columns1[2].Locator("> input");
                    await afm.FillAsync(item.Vat);
                    await afm.PressAsync("Tab");

                    if (!item.Goods.Equals(0))
                    {
                        var goods = columns1[3].Locator("> input");
                        await goods.FillAsync(item.Goods.ToStringGR());
                        await goods.PressAsync("Tab");
                    }

                    if (!item.TriangleExchange.Equals(0))
                    {
                        var triangleExchange = columns1[4].Locator("> input");
                        await triangleExchange.FillAsync(item.TriangleExchange.ToStringGR());
                        await triangleExchange.PressAsync("Tab");
                    }

                    if (!item.Services.Equals(0))
                    {
                        var services = columns1[5].Locator("> input");
                        await services.FillAsync(item.Services.ToStringGR());
                        await services.PressAsync("Tab");
                    }
                    count++;
                }
                count = 0;

                goodsTotal -= await GetDecimalValueIfLocatorEnabled(goodsTotalLocator);
                triangleExchangeTotal -= await GetDecimalValueIfLocatorEnabled(triangleExchangeTotalLocator);
                servicesTotal -= await GetDecimalValueIfLocatorEnabled(servicesTotalLocator);
            }

            async Task<decimal> GetDecimalValueIfLocatorEnabled(ILocator locator)
            {
                try
                {
                    return (await locator.InputValueAsync()).ToDecimal();
                }
                catch
                {
                    return 0m;
                }
            }

            var success = goodsTotal + triangleExchangeTotal + servicesTotal == 0;

            if (!success)
                await NewExceptionAsync("App.Playwright.TypingFailed");

            var submitLocator = Page.Locator("//html/body/form[1]/table/tbody/tr[2]/td/table[1]/tbody/tr/td[2]/input");
            await submitLocator.ClickAsync();

            await submitLocator.ClickAsync();

            await Task.Delay(3000).ConfigureAwait(false);

            var statementLocator = "//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/table/tbody/tr/td/table";
            await SendScreenshotAsync(Page.Locator(statementLocator));
        }

        private int GetMonth(string period)
        {
            var value = period.Split(" - ").First().Trim();
            var date = DateTime.ParseExact(value, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            return date.Month;
        }

    }
}
