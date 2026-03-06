using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Accounting;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace App.Automation.Pages.Accounting
{
    public class ListingF4RetrievePage : PageTest
    {
        private readonly string _taxisNetLaunchingPage = "https://www1.aade.gr/taxisnet/mytaxisnet";
        private readonly string _taxisNetLoginPage = "https://login.gsis.gr/mylogin/login.jsp";
        private readonly string _taxisNetHomePage = "https://www1.aade.gr/taxisnet/mytaxisnet/protected/home.htm";
        private readonly string _displayAgentTaxpayerListPage = "https://www1.aade.gr/taxisnet/vat/protected/displayAgentTaxpayerList.htm";
        private readonly string _displayDeclarationTypesPage = "https://www1.aade.gr/taxisnet/vat/protected/displayDeclarationTypes.htm";
        private readonly string _displayLiabilitiesForYearPage = "https://www1.aade.gr/taxisnet/vat/protected/displayLiabilitiesForYear.htm";
        private readonly string _displayActorRolesPage = "https://www1.aade.gr/taxisnet/vat/protected/displayActorRoles.htm";
        private readonly string _displayAccountingOfficesListPage = "https://www1.aade.gr/taxisnet/vat/protected/displayAccountingOfficesList.htm";
        private readonly string _displayDeclarationsListPage = "https://www1.aade.gr/taxisnet/vat/protected/displayDeclarationsList.htm";

        private ILocator _usernameLocator;
        private ILocator _passwordLocator;
        private ILocator _buttonRoleSelectLocator;
        private ILocator _buttonAccounntantSelectLocator;
        private ILocator _selectAccountantLocator;
        private ILocator _vatInputLocator;
        private ILocator _searchVatLocator;
        private ILocator _confirmVatLocator;
        private ILocator _yearSelectLocator;
        private ILocator _vatF4ByYearLocator;
        private ILocator _F4TableLocator;
        private ILocator _selectTableLocator;

        public ListingF4RetrievePage(string connectionId) : base(connectionId: connectionId, headless: false)
        {
            _usernameLocator = Page.Locator("[name=username]");
            _passwordLocator = Page.Locator("[name=password]");
            _buttonRoleSelectLocator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[1]/div/div[1]/table/tbody/tr[9]/td");
            _buttonAccounntantSelectLocator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/form/div/table/tbody/tr[4]/td[2]/input");
            _selectAccountantLocator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/form/table/tbody/tr[2]/td/table/tbody/tr/td/table/tbody/tr/td[1]/a");
            _vatInputLocator = Page.Locator("[name=taxpayerVatNumberPattern]");
            _searchVatLocator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/form/table/tbody/tr[4]/td/table/tbody/tr/td/input[1]");
            _confirmVatLocator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/form/table/tbody/tr[6]/td/table/tbody/tr/td/table/tbody/tr/td[1]/a");
            _yearSelectLocator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr[2]/td/table/tbody/tr[4]/td[3]/select");
            _vatF4ByYearLocator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr[2]/td/table/tbody/tr[4]/td[4]/input");
            _F4TableLocator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr");
            _selectTableLocator = Page.Locator("//html/body/table/tbody/tr[7]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr");
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
            await FillAsync(_usernameLocator, userName);
            await FillAsync(_passwordLocator, password);
            await ClickToUrl(Page, "[name=btn_login]", _taxisNetHomePage);

            return true;
        }
        public async Task<IList<ListingF4Result>> Execute(string vat, int year, int month)
        {
            await GotoToUrl(Page, _displayAgentTaxpayerListPage);

            await ClickToUrl(_buttonRoleSelectLocator, _displayActorRolesPage);

            await ClickToUrl(_buttonAccounntantSelectLocator, _displayAccountingOfficesListPage);

            await ClickToUrl(_selectAccountantLocator, _displayAgentTaxpayerListPage);

            await FillAsync(_vatInputLocator, vat);

            await ClickAsync(_searchVatLocator);

            await ClickToUrl(_confirmVatLocator, _displayDeclarationTypesPage);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            await SelectOptionAsync(_yearSelectLocator, year.ToString());

            await ClickToUrl(_vatF4ByYearLocator, _displayLiabilitiesForYearPage);

            var rows = await GetAllAsync(_F4TableLocator);

            var listModel = new List<ListingF4Result>();

            foreach (var row in rows)
            {
                var rowLocator = await GetLocatorAsync(row, "> td");
                var cols = await GetAllAsync(rowLocator);
                var period = await InnerTextAsync(cols[1]);
                if (GetMonth(period).Equals(month))
                {
                    var element = await GetByTextAsync(cols[3], "ΕπεξεργασίαΔηλώσεων");
                    if (await IsLocatorExist(element))
                    {
                        await ClickToUrl(element, _displayDeclarationsListPage);

                        var listingInfos = new List<ListingF4Info>();
                        var rows1 = await GetAllAsync(_selectTableLocator);
                        foreach (var row1 in rows1)
                        {
                            var row1Locator = await GetLocatorAsync(row1, "> td");
                            var cols1 = await GetAllAsync(row1Locator);
                            var buttonView = await GetByTextAsync(cols1[7], "Προβολή");
                            if (await IsLocatorExist(buttonView))
                            {
                                listingInfos.Add(new ListingF4Info { DateSubmit = await InnerTextAsync(cols1[3]), Button = buttonView });
                            }
                        }

                        if (listingInfos.Count == 0)
                            throw new Exception("There is not submitions.");

                        var firstTime = true;
                        foreach (var info in listingInfos)
                        {
                            var pdfText = await DownloadPdf(info.Button);
                            var title = string.Format("{0} {1}", firstTime ? "Αρχική" : "Τροπ/κή", info.DateSubmit);

                            var list = ExtractPdf(pdfText, title, title);
                            listModel.AddRange(list);
                            firstTime = false;
                        }

                        break;
                    }
                    else
                        break;
                }
            }

            //await SendData(listModel.OrderBy(x => x.Vat).ToList());

            return listModel.OrderBy(x => x.Vat).ToList();
        }

        private int GetMonth(string period)
        {
            var value = period.Split(" - ").First().Trim();
            var date = DateTime.ParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return date.Month;
        }
        private IList<ListingF4Result> ExtractPdf(string pdfText, string info, string group)
        {
            var listModel = new List<ListingF4Result>();

            var lines = pdfText.Split('\n');
            foreach (var line in lines)
            {
                var array = line.Split(' ');
                if (array.Length == 8)
                {
                    if (ListingCountryResources.CountryDict.TryGetValue(array[2], out string countryCode))
                    {
                        var model = new ListingF4Result
                        {
                            Error = info,
                            Group = group,
                            CountryCode = array[2],
                            Vat = array[3],
                            Goods = decimal.Parse(array[4], new CultureInfo("el-GR")),
                            TriangleExchange = decimal.Parse(array[5], new CultureInfo("el-GR")),
                            Services = decimal.Parse(array[6], new CultureInfo("el-GR")),
                            Products4200 = decimal.Parse(array[7], new CultureInfo("el-GR")),
                        };

                        info = string.Empty;
                        listModel.Add(model);
                    }
                }
            }

            return listModel;
        }
    }

    public class ListingF4Info
    {
        public string DateSubmit { get; set; }
        public ILocator Button { get; set; }
    }
}
