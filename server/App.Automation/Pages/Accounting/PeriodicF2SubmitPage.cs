using App.Core.Infrastructure;
using App.Core.Infrastructure.Dtos.Accounting;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace App.Automation.Pages.Accounting
{
    public class PeriodicF2SubmitPage : PageTest
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
        //private readonly string _displayLiabilitiesForYearPage = "https://www1.aade.gr/taxisnet/vat/protected/displayLiabilitiesForYear.htm?declarationType=vatF2&year={0}";

        private readonly string _searchVat = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/form/table/tbody/tr[4]/td/table/tbody/tr/td/input[1]";
        private readonly string _confirmVat = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/form/table/tbody/tr[6]/td/table/tbody/tr/td/table/tbody/tr/td[1]/a";
        private readonly string _buttonRoleSelect = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[1]/div/div[1]/table/tbody/tr[9]/td";
        private readonly string _buttonAccounntantSelect = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/form/div/table/tbody/tr[{1}]/td[2]/input";
        private readonly string _selectAccountant = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/form/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td[1]/a";
        private readonly string _registrationNumber = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/table/tbody/tr/td/table/tbody/tr[1]/td/span[6]";
        //private readonly string _exit = "//html/body/table/tbody/tr[3]/td/table/tbody/tr/td/table/tbody/tr[1]/td[9]/a";

        //F2 LOCATORS
        private readonly string _vatF2ByYear = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr[2]/td/table/tbody/tr[3]/td[4]/input";
        private readonly string _F2monthSelect = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr[2]/td/table/tbody/tr[3]/td[3]/select";
        private readonly string _F2Table = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr";
        private readonly string _F2selectRows = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr";


        private ILocator _userNameLocator;
        private ILocator _passwordLocator;
        private ILocator _signinButtonLocator;
        private ILocator _vatInputLocator;
        private ILocator _ibanLocator;

        private ILocator _523Locator;
        private ILocator _5071Locator;
        private ILocator _5072Locator;
        private ILocator _5073Locator;
        private ILocator _5074Locator;
        private ILocator _5075Locator;
        private ILocator _5076Locator;

        private Dictionary<int, ILocator> decimals;
        private Dictionary<int, ILocator> checkBoxes;

        public PeriodicF2SubmitPage(string connectionId) : base(connectionId: connectionId, headless: false)
        {
            _userNameLocator = Page.Locator("[name=username]");
            _passwordLocator = Page.Locator("[name=password]");
            _signinButtonLocator = Page.Locator("[name=btn_login]");
            _vatInputLocator = Page.Locator("[name=taxpayerVatNumberPattern]");
            _ibanLocator = Page.Locator("[id=iban]");

            _523Locator = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[10]/tbody/tr[7]/td[6]/select");
            //Checkboxes Locators
            _5071Locator = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[11]/tbody/tr/td[4]/input[2]");
            _5072Locator = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[11]/tbody/tr/td[6]/input[2]");
            _5073Locator = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[11]/tbody/tr/td[8]/input[2]");
            _5074Locator = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[11]/tbody/tr/td[10]/input[2]");
            _5075Locator = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[11]/tbody/tr/td[12]/input[2]");
            _5076Locator = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[11]/tbody/tr/td[14]/input[2]");

            decimals = new Dictionary<int, ILocator>()
            {
                [301] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[2]/td[3]/input"),
                [302] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[3]/td[2]/input"),
                [303] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[4]/td[2]/input"),
                [304] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[5]/td[3]/input"),
                [305] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[6]/td[2]/input"),
                [306] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[7]/td[2]/input"),
                [342] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[9]/td[3]/input"),
                [345] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[10]/td[3]/input"),
                [348] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[11]/td[3]/input"),
                [349] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[12]/td[3]/input"),
                [310] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[13]/td[3]/input"),
                [312] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[15]/td[3]/input"),
                [361] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[2]/td[9]/input"),
                [362] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[3]/td[8]/input"),
                [363] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[4]/td[8]/input"),
                [364] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[5]/td[9]/input"),
                [365] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[6]/td[8]/input"),
                [366] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[7]/td[8]/input"),
                [400] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[10]/td[7]/input"),
                [402] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[11]/td[7]/input"),
                [407] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[12]/td[6]/input"),
                [411] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[14]/td[6]/input"),
                [422] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[15]/td[6]/input"),
                [423] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[16]/td[5]/input"),
                [381] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[2]/td[11]/input"),
                [382] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[3]/td[10]/input"),
                [383] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[4]/td[10]/input"),
                [384] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[5]/td[11]/input"),
                [385] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[6]/td[10]/input"),
                [386] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[8]/tbody/tr[7]/td[10]/input"),
                [523] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[10]/tbody/tr[7]/td[6]/select"),
                [503] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[10]/tbody/tr[7]/td[3]/input[1]"),
                //[503] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[10]/tbody/tr[7]/td[3]/input"),
                [483] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[10]/tbody/tr[3]/td[6]/input"),
                [906] = Page.Locator("//html/body/form[1]/table/tbody/tr[6]/td/table/tbody/tr/td[4]/input"),
                [907] = Page.Locator("//html/body/form[1]/table/tbody/tr[6]/td/table/tbody/tr/td[7]/input"),
                [908] = Page.Locator("//html/body/form[1]/table/tbody/tr[6]/td/table/tbody/tr/td[10]/input")
            };

            checkBoxes = new Dictionary<int, ILocator>()
            {
                [5071] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[11]/tbody/tr/td[4]/input[2]"),
                [5072] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[11]/tbody/tr/td[6]/input[2]"),
                [5073] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[11]/tbody/tr/td[8]/input[2]"),
                [5074] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[11]/tbody/tr/td[10]/input[2]"),
                [5075] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[11]/tbody/tr/td[12]/input[2]"),
                [5076] = Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[11]/tbody/tr/td[14]/input[2]")
            };
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
            await FillAsync(_userNameLocator, userName);
            await FillAsync(_passwordLocator, password);

            await ClickToUrl(_signinButtonLocator, _taxisNetHomePage);

            return true;
        }
        public async Task<string> Execute(PeriodicF2Result model, string vat, int pageKindTypeId, bool f007, int year, int from, int to)
        {
            var script = Page.Locator("[language=javascript]");
            await script.EvaluateAsync("node => node.parentNode.removeChild(node)");

            var rowsLength = await Page.Locator("//html/body/table/tbody/tr").CountAsync();

            await Page.ReloadAsync();

            var trCount = rowsLength - 2;

            var _searchVatLocator = Page.Locator(string.Format(_searchVat, trCount));
            var _confirmVatLocator = Page.Locator(string.Format(_confirmVat, trCount));
            var _buttonRoleSelectLocator = Page.Locator(string.Format(_buttonRoleSelect, trCount));
            var _selectAccountantLocator = Page.Locator(string.Format(_selectAccountant, trCount));
            var _vatF2ByYearLocator = Page.Locator(string.Format(_vatF2ByYear, trCount));
            var _F2monthSelectLocator = Page.Locator(string.Format(_F2monthSelect, trCount));
            var _F2TableLocator = Page.Locator(string.Format(_F2Table, trCount));
            var _F2selectRowsLocator = Page.Locator(string.Format(_F2selectRows, trCount));
            var _registrationNumberLocator = Page.Locator(string.Format(_registrationNumber, trCount));

            if (pageKindTypeId == (int)PageCredentialType.IndividualCompany)
            {
                await GotoToUrl(Page, _displayDeclarationTypesPage);
            }
            else
            {
                var representative = pageKindTypeId == (int)PageCredentialType.Representative;
                var _buttonAccounntantSelectLocator = Page.Locator(string.Format(_buttonAccounntantSelect, trCount, representative ? 3 : 4));

                await GotoToUrl(Page, _displayAgentTaxpayerListPage, _displayAgentTaxpayerListPage);

                await ClickToUrl(_buttonRoleSelectLocator, _displayActorRolesPage);

                await ClickAsync(_buttonAccounntantSelectLocator);

                if (representative)
                {
                    await EnsurePageLoaded(Page, _displayLegalEntitiesListPage);
                    await _selectAccountantLocator.ClickAsync();
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

            foreach (var row in rows)
            {
                var rowLocator = await GetLocatorAsync(row, "> td");
                var columns = await GetAllAsync(rowLocator);
                var _value = (await columns[1].InnerTextAsync()).Split(" - ");
                var _from = ToDate(_value[0].Trim()).Month;
                var _to = ToDate(_value[1].Trim()).Month;

                if (from.Equals(_from) && to.Equals(_to))
                {
                    //"/html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr[7]/td[4]/div"
                    var button1 = columns[3].Locator("div[class=navbtn]");
                    var buttonName = await InnerTextAsync(button1);

                    await ClickAsync(button1);

                    if (buttonName.Contains("Υποβολή") && !f007)
                    {
                        await SetValues(model);
                    }
                    if (!buttonName.Contains("Υποβολή") && f007)
                    {
                        await EnsurePageLoaded(Page, _displaydisplayDeclarationsListPage);

                        var rows2List = await GetAllAsync(_F2selectRowsLocator);
                        var rows2 = rows2List.FirstOrDefault();

                        var rows2Locator = await GetLocatorAsync(rows2, "> td");
                        var columnList = await GetAllAsync(rows2Locator);

                        var columnList7Locator = columnList[7].Locator("//table/tbody/tr/td/div");
                        var ButtonList = await GetAllAsync(columnList7Locator);

                        var buttonView = ButtonList.FirstOrDefault();

                        await buttonView.ClickAsync();

                        await SetValues(model);
                    }

                    break;
                }
            }
            var f470 = (await Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[10]/tbody/tr[1]/td[3]/input").InputValueAsync())?.ToDecimal() ?? 0;
            var f401 = (await Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[10]/tbody/tr[3]/td[3]/input").InputValueAsync())?.ToDecimal() ?? 0;
            var f403 = (await Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[10]/tbody/tr[4]/td[3]/input").InputValueAsync())?.ToDecimal() ?? 0;
            var f404 = (await Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[10]/tbody/tr[5]/td[3]/input").InputValueAsync())?.ToDecimal() ?? 0;
            var f502 = (await Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[10]/tbody/tr[6]/td[3]/input").InputValueAsync())?.ToDecimal() ?? 0;
            var f480 = (await Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[10]/tbody/tr[1]/td[6]/input").InputValueAsync())?.ToDecimal() ?? 0;
            var f505 = (await Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[10]/tbody/tr[4]/td[6]/input").InputValueAsync())?.ToDecimal() ?? 0;
            var f511 = (await Page.Locator("//html/body/form[1]/table/tbody/tr[3]/td/table[10]/tbody/tr[6]/td[6]/input").InputValueAsync())?.ToDecimal() ?? 0;

            var success =
                f470 == model.F470 && f401 == model.F401 && f403 == model.F403 && f404 == model.F404 &&
                f502 == model.F502 && f480 == model.F480 && f505 == model.F505 && f511 == model.F511;

            string registrationNumber = string.Empty;

            if (success)
            {
                var submitLocator = Page.Locator("//html/body/form[1]/table/tbody/tr[1]/td/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td[2]/input");
                await ClickAsync(submitLocator);
                await Task.Delay(1000).ConfigureAwait(false);

                if (await Page.Locator("//html/body/div[2]").IsVisibleAsync())
                {
                    await ClickAsync(Page.Locator("//html/body/div[2]/div[11]/div/button[1]"));
                    await Task.Delay(1000).ConfigureAwait(false);
                }
                if (await Page.Locator("//html/body/div[5]").IsVisibleAsync())
                {
                    await ClickAsync(Page.Locator("//html/body/div[5]/div[11]/div/button[1]"));
                    await Task.Delay(1000).ConfigureAwait(false);
                }
                if (await Page.Locator("//html/body/div[9]").IsVisibleAsync())
                {
                    await ClickAsync(Page.Locator("//html/body/div[9]/div[11]/div/button[1]"));
                    await Task.Delay(1000).ConfigureAwait(false);
                }
                await ClickAsync(submitLocator);
                await Task.Delay(1000).ConfigureAwait(false);
                //await EnsurePageLoaded(Page, _displayLiabilitiesForYearPage);

                var exist = await _registrationNumberLocator.IsVisibleAsync();
                if (!exist)
                {
                    if (await Page.Locator("//html/body/div[2]").IsVisibleAsync())
                    {
                        await ClickAsync(Page.Locator("//html/body/div[2]/div[11]/div/button[1]"));///html/body/div[2]/div[11]/div/button
                        await Task.Delay(1000).ConfigureAwait(false);
                    }
                    if (await Page.Locator("//html/body/div[5]").IsVisibleAsync())
                    {
                        await ClickAsync(Page.Locator("//html/body/div[5]/div[11]/div/button[1]"));
                        await Task.Delay(1000).ConfigureAwait(false);
                    }
                    if (await Page.Locator("//html/body/div[9]").IsVisibleAsync())
                    {
                        await ClickAsync(Page.Locator("//html/body/div[9]/div[11]/div/button[1]"));
                        await Task.Delay(1000).ConfigureAwait(false);
                    }
                    await ClickAsync(submitLocator);
                    await Task.Delay(1000).ConfigureAwait(false);
                }

                registrationNumber = await _registrationNumberLocator.InnerTextAsync();

                await SendScreenshotAsync(Page.Locator("//html/body"));
            }
            else
            {
                throw new AppPlaywrightException("Η αντιστοίχηση του πινακα Γ' απέτυχε.");
            }

            return registrationNumber;

        }

        private async Task SetValues(PeriodicF2Result model)
        {
            var modelType = model.GetType();

            foreach (var item in decimals)
            {
                var property = modelType.GetProperty($"F{item.Key}");
                if (property != null)
                {
                    var value = property.GetValue(model, null) as decimal?;
                    if (value.HasValue)
                    {
                        if (await item.Value.IsEnabledAsync())
                        {
                            if (item.Key == 503 && value == 0)
                            {
                                await item.Value.FillAsync("");
                                await item.Value.PressAsync("Tab");
                                continue;
                            }
                            await item.Value.FillAsync(value.Value.ToStringGR());
                            await item.Value.PressAsync("Tab");
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(model.IBAN))
            {
                await _ibanLocator.FillAsync(model.IBAN);
                await _ibanLocator.PressAsync("Tab");
            }

            if (model.F5071)
            {
                await _5071Locator.ClickAsync();
            }

            if (model.F5072)
            {
                await _5072Locator.ClickAsync();
            }

            if (model.F5073)
            {
                await _5073Locator.ClickAsync();
            }

            if (model.F5074)
            {
                await _5074Locator.ClickAsync();
            }

            if (model.F5075)
            {
                await _5074Locator.ClickAsync();
            }

            if (model.F5076)
            {
                await _5076Locator.ClickAsync();
            }

            if (model.F502 > 0)
                await SelectOptionAsync(_523Locator, "2");
            else
            {
                if (model.F523 == 1)
                {
                    await SelectOptionAsync(_523Locator, "1");
                }
                else
                    await SelectOptionAsync(_523Locator, "0");
            }


            //void SelectOption1(int value, By locator, int seconds = 10)
            //{
            //    var element = Wait(seconds).Until(ExpectedConditions.FindElement(locator));
            //    var select = new OpenQA.Selenium.Support.UI.SelectElement(element);
            //    select.SelectByIndex(value);
            //}
        }
        public DateTime ToDate(string date)
        {
            return DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
