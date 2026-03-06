using App.Core.Infrastructure.Dtos.Accounting;
using Microsoft.Playwright;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace App.Automation.Pages.Accounting
{
    public class PeriodicF2IdentityPage : PageTest
    {
        //TaxisNet
        private readonly string _taxisNetLaunchingPage = "https://www1.aade.gr/taxisnet/mytaxisnet";
        private readonly string _taxisNetLoginPage = "https://login.gsis.gr/mylogin/login.jsp";
        private readonly string _taxisNetHomePage = "https://www1.aade.gr/taxisnet/mytaxisnet/protected/home.htm";
        private readonly string _displayDeclarationTypesPage = "https://www1.aade.gr/taxisnet/vat/protected/displayDeclarationTypes.htm";
        private readonly string _displayActorRolesPage = "https://www1.aade.gr/taxisnet/vat/protected/displayActorRoles.htm";
        private readonly string _displaydisplayDeclarationsListPage = "https://www1.aade.gr/taxisnet/vat/protected/displayDeclarationsList.htm";
        private readonly string _displayAccountingOfficesListPage = "https://www1.aade.gr/taxisnet/vat/protected/displayAccountingOfficesList.htm";
        private readonly string _displayAgentTaxpayerListPage = "https://www1.aade.gr/taxisnet/vat/protected/displayAgentTaxpayerList.htm";
        private readonly string _displayLegalEntitiesListPage = "https://www1.aade.gr/taxisnet/vat/protected/displayLegalEntitiesList.htm";

        //F2 
        private readonly string _displayLiabilitiesForYearPage = "https://www1.aade.gr/taxisnet/vat/protected/displayLiabilitiesForYear.htm";

        private readonly string _searchVat = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/form/table/tbody/tr[4]/td/table/tbody/tr/td/input[1]";
        private readonly string _confirmVat = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/form/table/tbody/tr[6]/td/table/tbody/tr/td/table/tbody/tr/td[1]/a";
        private readonly string _buttonRoleSelect = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[1]/div/div[1]/table/tbody/tr[9]/td";
        private readonly string _buttonAccounntantSelect = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/form/div/table/tbody/tr[{1}]/td[2]/input";
        private readonly string _selectAccountant = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/form/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td[1]/a";
        private readonly string _vatF2ByYear = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr[2]/td/table/tbody/tr[3]/td[4]/input";
        private readonly string _F2monthSelect = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr[2]/td/table/tbody/tr[3]/td[3]/select";
        private readonly string _F2Table = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr";
        private readonly string _F2selectRows = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr";
        //private readonly string _exit = "//html/body/table/tbody/tr[3]/td/table/tbody/tr/td/table/tbody/tr[1]/td[9]/a";

        private ILocator _usernameLocator;
        private ILocator _passwordLocator;
        private ILocator _vatInputLocator;
        private ILocator _searchVatLocator;
        private ILocator _confirmVatLocator;
        private ILocator _buttonRoleSelectLocator;
        private ILocator _buttonAccounntantSelectLocator;
        private ILocator _selectAccountantLocator;
        private ILocator _vatF2ByYearLocator;
        private ILocator _F2monthSelectLocator;
        private ILocator _F2TableLocator;
        private ILocator _F2selectRowsLocator;

        public PeriodicF2IdentityPage(string connectionId) : base(connectionId: connectionId, headless: false)
        {
            _usernameLocator = Page.Locator("[name=username]");
            _passwordLocator = Page.Locator("[name=password]");
            _vatInputLocator = Page.Locator("[name=taxpayerVatNumberPattern]");
        }

        protected override async Task LogoutAsync()
        {
            //await GotoToUrl(Page, _taxisNetHomePage);

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
            await FillAsync(_usernameLocator, userName);
            await FillAsync(_passwordLocator, password);

            await ClickToUrl(Page, "[name=btn_login]", _taxisNetHomePage);

            return true;
        }
        public async Task Execute(string vat,int pageKindTypeId, bool f007, int year, int from, int to)
        {
            var script = Page.Locator("[language=javascript]");
            await script.EvaluateAsync("node => node.parentNode.removeChild(node)");

            var rowsLocator = await GetLocatorAsync(Page, "//html/body/table/tbody/tr");
            var rowsLength = await rowsLocator.CountAsync();

            await Page.ReloadAsync();

            var rowCount = rowsLength - 2;

            _searchVatLocator = Page.Locator(string.Format(_searchVat, rowCount));
            _confirmVatLocator = Page.Locator(string.Format(_confirmVat, rowCount));
            _buttonRoleSelectLocator = Page.Locator(string.Format(_buttonRoleSelect, rowCount));
            _selectAccountantLocator = Page.Locator(string.Format(_selectAccountant, rowCount));
            _vatF2ByYearLocator = Page.Locator(string.Format(_vatF2ByYear, rowCount));
            _F2monthSelectLocator = Page.Locator(string.Format(_F2monthSelect, rowCount));
            _F2TableLocator = Page.Locator(string.Format(_F2Table, rowCount));
            _F2selectRowsLocator = Page.Locator(string.Format(_F2selectRows, rowCount));

            if (pageKindTypeId == (int)PageCredentialType.IndividualCompany)
            {
                await GotoToUrl(Page, _displayDeclarationTypesPage);
            }
            else
            {
                var representative = pageKindTypeId == (int)PageCredentialType.Representative;
                _buttonAccounntantSelectLocator = Page.Locator(string.Format(_buttonAccounntantSelect, rowCount, representative ? 3 : 4));

                await GotoToUrl(Page, _displayAgentTaxpayerListPage, _displayAgentTaxpayerListPage);

                await ClickToUrl(_buttonRoleSelectLocator, _displayActorRolesPage);

                await ClickAsync(_buttonAccounntantSelectLocator);

                if (representative)
                {
                    await EnsurePageLoaded(Page, _displayLegalEntitiesListPage);
                    await ClickAsync(_selectAccountantLocator);
                }
                else
                {
                    await EnsurePageLoaded(Page, _displayAccountingOfficesListPage);

                    await ClickToUrl(_selectAccountantLocator, _displayAgentTaxpayerListPage);

                    await FillAsync(_vatInputLocator, vat);

                    await ClickAsync(_searchVatLocator);

                    await ClickAsync(_confirmVatLocator);
                }
            }

            await EnsurePageLoaded(Page, _displayDeclarationTypesPage);

            await SelectOptionAsync(_F2monthSelectLocator, year.ToString());

            await ClickAsync(_vatF2ByYearLocator);

            await EnsurePageLoaded(Page, _displayLiabilitiesForYearPage);

            var rows = await GetAllAsync(_F2TableLocator);

            DateTime toDate(string date)
            {
                return DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            foreach (var row in rows)
            {
                var rowLocator = await GetLocatorAsync(row, "> td");
                var columns = await GetAllAsync(rowLocator);
                var _value = (await InnerTextAsync(columns[1])).Split(" - ");
                var _from = toDate(_value[0].Trim()).Month;
                var _to = toDate(_value[1].Trim()).Month;

                if (from.Equals(_from) && to.Equals(_to))
                {
                    var element = await GetByTextAsync(columns[3], "ΕπεξεργασίαΔηλώσεων");
                    if (await IsLocatorExist(element))
                    {
                        await ClickToUrl(element, _displaydisplayDeclarationsListPage);
                        var rows1 = await GetAllAsync(_F2selectRowsLocator);
                        foreach (var row1 in rows1)
                        {
                            var rows1Locator = await GetLocatorAsync(row1, "> td");
                            var columnList = await GetAllAsync(rows1Locator);

                            if (await InnerTextAsync(columnList[4]) == (f007 ? "Τροποποιητική" : "Αρχική"))
                            {
                                var buttonView = await GetByTextAsync(columnList[7], "Ταυτότητα");
                                if (await IsLocatorExist(buttonView))
                                {
                                    await ClickAsync(buttonView);

                                    var footer = Page.Locator(".contenttd > form:nth-child(1) > div:nth-child(2) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child(1) > td:nth-child(1) > table:nth-child(1) > tbody:nth-child(1) > tr:nth-child(5)");
                                    await footer.EvaluateAsync("node => node.parentNode.removeChild(node)");

                                    var screenshotLocator = Page.Locator(".contenttd > form:nth-child(1)");

                                    await SendScreenshotAsync(screenshotLocator);

                                    break;
                                }
                            }
                        }
                    }

                    break;
                }
            }
        }
    }

}
