using App.Core.Infrastructure;
using App.Core.Infrastructure.Dtos.Accounting;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace App.Automation.Pages.Accounting
{
    public class PeriodicF2RetrievePage : PageTest
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

        public PeriodicF2RetrievePage(string connectionId) : base(connectionId: connectionId, headless: false)
        {
            _userNameLocator = Page.Locator("[name=username]");
            _passwordLocator = Page.Locator("[name=password]");
            _signinButtonLocator = Page.Locator("[name=btn_login]");
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
            await _userNameLocator.FillAsync(userName);
            await _passwordLocator.FillAsync(password);

            await ClickToUrl(_signinButtonLocator, _taxisNetHomePage);

            return true;
        }
        public async Task<IList<PeriodicF2Result>> Execute(string vat, int pageKindTypeId, int year, int from, int to)
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

            if (pageKindTypeId == (int)PageCredentialType.IndividualCompany)
            {
                await GotoToUrl(Page, _displayDeclarationTypesPage);
            }
            else
            {
                var representative =  pageKindTypeId == (int)PageCredentialType.Representative;
                var _buttonAccounntantSelectLocator = Page.Locator(string.Format(_buttonAccounntantSelect, trCount, representative ? 3 : 4));

                await GotoToUrl(Page, _displayAgentTaxpayerListPage, _displayAgentTaxpayerListPage);

                await ClickToUrl(_buttonRoleSelectLocator, _displayActorRolesPage);

                await _buttonAccounntantSelectLocator.ClickAsync();

                if (representative)
                {
                    await EnsurePageLoaded(Page, _displayLegalEntitiesListPage);
                    await _selectAccountantLocator.ClickAsync();
                }
                else
                {
                    await EnsurePageLoaded(Page, _displayAccountingOfficesListPage);

                    await ClickToUrl(_selectAccountantLocator, _displayAgentTaxpayerListPage);

                    await _vatInputLocator.FillAsync(vat);

                    await _searchVatLocator.ClickAsync();

                    await _confirmVatLocator.ClickAsync();
                }
            }

            await EnsurePageLoaded(Page, _displayDeclarationTypesPage);

            await SelectOptionAsync(_F2monthSelectLocator, year.ToString());

            await _vatF2ByYearLocator.ClickAsync();

            await EnsurePageLoaded(Page, _displayLiabilitiesForYearPage);

            var rows = await _F2TableLocator.AllAsync();

            var list = new List<PeriodicF2Result>();

            foreach (var row in rows)
            {
                var columns = await row.Locator("> td").AllAsync();
                var _value = (await columns[1].InnerTextAsync()).Split(" - ");
                var _from = _value[0].Trim().ToDateGR().Month;
                var _to = _value[1].Trim().ToDateGR().Month;
                if (from.Equals(_from) && to.Equals(_to))
                {
                    var button1Locator = columns[3].Locator("div[class=navbtn]");
                    if ((await button1Locator.InnerTextAsync()).Contains("Υποβολή"))
                        throw new Exception("There are not any submitions.");

                    await button1Locator.ClickAsync();
                    await EnsurePageLoaded(Page, _displaydisplayDeclarationsListPage);

                    var periodicF2Infos = new List<PeriodicF2Info>();
                    var rows1 = await _F2selectRowsLocator.AllAsync();
                    foreach (var row1 in rows1)
                    {
                        var columnList = await row1.Locator("> td").AllAsync();

                        var buttonList = await columnList[7].Locator("//table/tbody/tr/td/div").AllAsync();

                        var buttonView = await buttonList.FirstOrDefaultAwaitAsync(async x => (await x.InnerTextAsync()).Contains("Προβολή"));

                        if (buttonView != null)
                        {
                            var periodicF2Model = new PeriodicF2Info
                            {
                                RegistrationNumber = (await columnList[2].InnerTextAsync()).Trim(),
                                SubmitDate = (await columnList[3].InnerTextAsync()).ToDateGR(),
                                Modified = (await columnList[4].InnerTextAsync()).Contains("Τροποποιητική"),
                                ButtonLocator = buttonView
                            };
                            periodicF2Infos.Add(periodicF2Model);
                        }
                    }

                    if (periodicF2Infos.Count == 0)
                        throw new Exception("There are not any submitions.");

                    foreach (var info in periodicF2Infos)
                    {
                        var waitForDownloadTask = Page.WaitForDownloadAsync();
                        await info.ButtonLocator.ClickAsync();
                        var download = await waitForDownloadTask;
                        var fileNamePath = await download.PathAsync();

                        var item = GetItem(fileNamePath);

                        item.F002 = info.SubmitDate;
                        item.F007 = info.Modified;
                        item.RegistrationNumber = info.RegistrationNumber;

                        list.Add(item);
                    }

                    break;
                }
            }

            return list;
        }

        private PeriodicF2Result GetItem(string fileNamePath)
        {
            PeriodicF2Result model = (PeriodicF2Result)Activator.CreateInstance(typeof(PeriodicF2Result));
            var modelType = model.GetType();

            Rectangle[] regions = new Rectangle[]
            {
                            new Rectangle(180,386,110,371), // F312
                            new Rectangle(180,401,110,386), // F311
                            new Rectangle(180,416,110,401), // F310
                            new Rectangle(180,431,110,416), // F349
                            new Rectangle(180,446,110,432), // F348
                            new Rectangle(180,462,110,447), // F345
                            new Rectangle(180,476,110,462), // F342
                            new Rectangle(180,492,110,478), // F307
                            new Rectangle(180,508,110,493), // F306
                            new Rectangle(180,523,110,509), // F305
                            new Rectangle(180,539,110,524), // F304
                            new Rectangle(180,554,110,540), // F303
                            new Rectangle(180,571,110,556), // F302
                            new Rectangle(180,586,110,571), // F301

                            new Rectangle(297,586,228,571), // F331
                            new Rectangle(297,571,228,556), // F332
                            new Rectangle(297,554,228,540), // F333
                            new Rectangle(297,539,228,524), // F334
                            new Rectangle(297,523,228,509), // F335
                            new Rectangle(297,508,228,493), // F336
                            new Rectangle(297,492,228,478), // F337

                            new Rectangle(484,586,415,571), // F361
                            new Rectangle(484,571,415,556), // F362
                            new Rectangle(484,554,415,540), // F363
                            new Rectangle(484,539,415,524), // F364
                            new Rectangle(484,523,415,509), // F365
                            new Rectangle(484,508,415,493), // F366
                            new Rectangle(484,492,415,478), // F367

                            new Rectangle(468,461,398,446), // F400
                            new Rectangle(468,445,398,431), // F402
                            new Rectangle(468,430,398,416), // F407
                            new Rectangle(468,399,398,385), // F411
                            new Rectangle(468,384,398,370), // F422
                            new Rectangle(468,369,398,354), // F423

                            new Rectangle(576,586,507,571), // F381
                            new Rectangle(576,571,507,556), // F382
                            new Rectangle(576,554,507,540), // F383
                            new Rectangle(576,539,507,524), // F384
                            new Rectangle(576,523,507,509), // F385
                            new Rectangle(576,508,507,493), // F386
                            new Rectangle(576,492,507,478), // F387

                            new Rectangle(578,447,509,432), // F410
                            new Rectangle(578,385,509,370), // F428
                            new Rectangle(578,334,509,319), // F430

                            new Rectangle(191,296,122,282), // F470
                            new Rectangle(191,265,122,251), // F401
                            new Rectangle(191,250,122,236), // F403
                            new Rectangle(191,235,122,221), // F404
                            new Rectangle(191,219,122,205), // F502
                            new Rectangle(191,204,122,190), // F503

                            new Rectangle(376,296,306,281), // F480
                            new Rectangle(376,265,306,251), // F483
                            new Rectangle(376,250,306,236), // F505
                            new Rectangle(356,219,287,205), // F511

                            new Rectangle(244,176,144,162), // F508

                            new Rectangle(260,121,191,107), // F906
                            new Rectangle(419,121,349,107), // F907
                            new Rectangle(576,121,507,107)  // F908                                                       
            };
            string[] fields = new string[]
            {
                            "F312","F311","F310","F349","F348","F345","F342","F307","F306","F305","F304","F303","F302","F301", // 14
                            "F331","F332","F333","F334","F335","F336","F337", // 7
                            "F361","F362","F363","F364","F365","F366","F367", // 7
                            "F400","F402","F407","F411","F422","F423", // 6
                            "F381","F382","F383","F384","F385","F386","F387", // 7
                            "F410","F428","F430", // 3
                            "F470","F401","F403","F404","F502","F503", // 6
                            "F480","F483","F505","F511","F508","F906","F907","F908" // 8
            };

            Rectangle[] regionsF523 = new Rectangle[]
            {
                            new Rectangle(321,205,305,189), // F5231
                            new Rectangle(358,205,342,189), // F5232
            };
            string[] fieldsF523 = new string[]
            {
                            "F5231","F5232"
            };

            Rectangle[] regionsF507 = new Rectangle[] {
                            new Rectangle(187,191,171,174), // F5071
                            new Rectangle(224,191,208,174), // F5072
                            new Rectangle(263,191,246,174), // F5073
                            new Rectangle(304,191,288,174), // F5074
                            new Rectangle(339,191,323,174), // F5075
                            new Rectangle(378,191,362,174), // F5076
                        };
            string[] fieldsF507 = new string[] {
                            "F5071","F5072","F5073","F5074","F5075","F5076"
                        };

            // F523
            var dict = ExtractionStrategies(regionsF523, fieldsF523, fileNamePath);
            if (dict.TryGetValue("F5231", out string valueF5231) && valueF5231.Equals("X"))
                modelType.GetProperty("F523").SetValue(model, 1, null);

            if (dict.TryGetValue("F5232", out string valueF5232) && valueF5232.Equals("X"))
                modelType.GetProperty("F523").SetValue(model, 2, null);

            // F507
            dict = ExtractionStrategies(regionsF507, fieldsF507, fileNamePath);
            if (dict.TryGetValue("F5071", out string valueF5071) && valueF5071.Equals("X"))
                modelType.GetProperty("F5071").SetValue(model, true, null);

            if (dict.TryGetValue("F5072", out string valueF5072) && valueF5072.Equals("X"))
                modelType.GetProperty("F5072").SetValue(model, true, null);

            if (dict.TryGetValue("F5073", out string valueF5073) && valueF5073.Equals("X"))
                modelType.GetProperty("F5073").SetValue(model, true, null);

            if (dict.TryGetValue("F5074", out string valueF5074) && valueF5074.Equals("X"))
                modelType.GetProperty("F5074").SetValue(model, true, null);

            if (dict.TryGetValue("F5075", out string valueF5075) && valueF5075.Equals("X"))
                modelType.GetProperty("F5075").SetValue(model, true, null);

            if (dict.TryGetValue("F5076", out string valueF5076) && valueF5076.Equals("X"))
                modelType.GetProperty("F5076").SetValue(model, true, null);

            // All
            dict = ExtractionStrategies(regions, fields, fileNamePath);
            foreach (var item in dict)
            {
                if (decimal.TryParse(item.Value, NumberStyles.Any, new CultureInfo("el-GR"), out decimal value) && value > 0)
                {
                    var property = modelType.GetProperty(item.Key);
                    property?.SetValue(model, value, null);
                }
            }

            return model;
        }

        private Dictionary<string, string> ExtractionStrategies(Rectangle[] regions, string[] fields, string fileNamePath)
        {
            RegionTextRenderFilter[] regionFilters = new RegionTextRenderFilter[regions.Length];
            for (int i = 0; i < regions.Length; i++)
                regionFilters[i] = new RegionTextRenderFilter(regions[i]);


            var listener = new MyMultiFilteredRenderListener();
            MyLocationTextExtractionStrategy[] extractionStrategies = new MyLocationTextExtractionStrategy[regions.Length];
            for (int i = 0; i < regions.Length; i++)
                extractionStrategies[i] =
                    
                        listener.AttachRenderListener(new MyLocationTextExtractionStrategy(fields[i]), regionFilters[i]);

            using (PdfReader reader = new PdfReader(fileNamePath))
            {
                new PdfReaderContentParser(reader).ProcessContent(1, listener);
            }

            var dict = new Dictionary<string, string>();
            for (int i = 0; i < regions.Length; i++)
            {
                var field = extractionStrategies[i].FieldName;
                var actualText = extractionStrategies[i].GetResultantText();
                dict[field] = actualText;
            }

            return dict;
        }

    }

    public class PeriodicF2Info
    {
        public ILocator ButtonLocator { get; set; }
        public DateTime SubmitDate { get; set; }
        public bool Modified { get; set; }
        public string RegistrationNumber { get; set; }
    }
}
