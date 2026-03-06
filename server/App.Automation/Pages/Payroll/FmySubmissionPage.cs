using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Automation.Pages.Payroll
{
    public class FmySubmissionPage : PageTest
    {
        private readonly string _taxisNetLaunchingPage = "https://www1.aade.gr/taxisnet/mytaxisnet";
        private readonly string _taxisNetLoginPage = "https://login.gsis.gr/mylogin/login.jsp";
        private readonly string _taxisNetHomePage = "https://www1.aade.gr/taxisnet/mytaxisnet/protected/home.htm";
        private readonly string _taxisNetDisplayActorRolesPage = "https://www1.aade.gr/taxisnet/deduction/protected/displayActorRoles.htm";
        private readonly string _taxisNetDisplayLiabilitiesForYearPage = "https://www1.aade.gr/taxisnet/deduction/protected/displayLiabilitiesForYear.htm?declarationType=deductFMYTemporary&year={0}&periodType=oneMonth&typeBtn=Μήνας";
        private readonly string _displayLiabilitiesForYearPage = "https://www1.aade.gr/taxisnet/deduction/protected/displayLiabilitiesForYear.htm";

        private ILocator _userNameLocator;
        private ILocator _passwordLocator;
        private ILocator _pageFooterLocator;
        private ILocator _signinButtonLocator;
        private ILocator _noPrintLocator;
        private ILocator _pageHasMessagesLocator;
        private string _displayLiabilitiesForYearPath = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr";
        private string _displayDeclarationsListPath = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td[2]/table/tbody/tr[4]/td/table/tbody/tr/td[1]/table/tbody/tr[2]/td/table/tbody/tr";

        public FmySubmissionPage(string connectionId) : base(connectionId: connectionId, emulateBrowserEnable: false, headless: false)
        {
            _userNameLocator = Page.Locator("[name=username]");
            _passwordLocator = Page.Locator("[name=password]");
            _pageFooterLocator = Page.Locator("[id=pageFooter]");
            _signinButtonLocator = Page.Locator("[name=btn_login]");
            _noPrintLocator = Page.Locator("[class=noprint]");
            _pageHasMessagesLocator = Page.Locator("//html/body/table/tbody/tr");
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

            await FillAsync(_userNameLocator, userName);
            await FillAsync(_passwordLocator, password);
            await WaitForAsync(_pageFooterLocator);
            //await ClickToUrl(_signinButtonLocator, _taxisNetHomePage);
            await ClickAsync(_signinButtonLocator);

            return true;
        }
        public async Task<IList<string>> Execute(int monthFrom, int monthTo, int year, bool mySelf = false)
        {
            if (mySelf)
            {
                // Goto firm registry info page
                await Goto(Page, string.Format(_taxisNetDisplayActorRolesPage, year));
                var chooseIndividual = Page.Locator("[name=_eventId_chooseIndividual]");
                await chooseIndividual.ClickAsync();
            }

            await Goto(Page, string.Format(_taxisNetDisplayLiabilitiesForYearPage, year));
            await WaitForAsync(_noPrintLocator);

            var tr = await GetAllAsync(_pageHasMessagesLocator);
            var rowsLocator = Page.Locator(string.Format(_displayLiabilitiesForYearPath, tr.Count - 2));

            var rows = await GetAllAsync(rowsLocator);

            var submissions = new List<FmyDisplayLiabilitiesForYear>();
            foreach (var row in rows)
            {
                var rowLocator = await GetLocatorAsync(row, "> td");
                var columns = await GetAllAsync(rowLocator);
                var period = (await InnerTextAsync(columns[0])).Split(" - ");
                var startDate = period[0].Trim();
                var buttonLocator = await GetLocatorAsync(columns[2], "> div[class=navbtn]");

                submissions.Add(new FmyDisplayLiabilitiesForYear
                {
                    Month = ToDate2(startDate).Month,
                    Year = year,
                    Status = (await InnerTextAsync(buttonLocator)).Trim(),
                    Locator = buttonLocator
                });
            }

            var months = new List<int>();
            for (var i = monthFrom; i <= monthTo; i++)
                months.Add(i);

            submissions = submissions.Where(x => x.Status.Contains("Επεξεργασία") && months.Contains(x.Month)).ToList();

            foreach (var item in submissions)
            {
                await ClickAsync(item.Locator);

                //Find table rows elements
                var rows1Locator = Page.Locator(string.Format(_displayDeclarationsListPath, tr.Count - 2));
                var rows1 = await GetAllAsync(rows1Locator);

                var list = new List<FmyDisplayDeclaration>();

                foreach (var row1 in rows1)
                {
                    var row1Locator = await GetLocatorAsync(row1, "> td");
                    var columns1 = await GetAllAsync(row1Locator);
                    var taxisNet = await InnerTextAsync(columns1[0]);

                    if (taxisNet.Trim() == "TAXISnet")
                    {
                        var id = (await InnerTextAsync(columns1[2])).Trim();
                        var kind = (await InnerTextAsync(columns1[4])).Trim();

                        var elemsLocator = await GetLocatorAsync(columns1[7], "div[class=navbtn]");
                        var elems = await GetAllAsync(elemsLocator);
                        var button1 = await elems.FirstOrDefaultAwaitAsync(async x => (await InnerTextAsync(x)).Contains("Προβολή TAXISNet"));

                        if (button1 != null && (kind.Contains("Τροποποιητική ( ΦΜΥ )") || kind.Contains("Αρχική ( ΦΜΥ )")))
                            list.Add(new FmyDisplayDeclaration { Id = id, Kind = kind, Locator = button1 });
                    }
                }

                list = list.OrderByDescending(x => x.Id).ToList();

                var xPath = list.Where(x => x.Kind.Contains("Τροποποιητική ( ΦΜΥ )")).Select(s => s.Locator).FirstOrDefault();
                if (xPath == null)
                    xPath = list.Where(x => x.Kind.Contains("Αρχική ( ΦΜΥ )")).Select(s => s.Locator).FirstOrDefault();

                if (xPath != null)
                    item.PdfText = await DownloadPdf(xPath);

                await Page.GoBackAsync();

                await EnsurePageLoaded(Page, _displayLiabilitiesForYearPage);
            }

            var pdfList = submissions.Where(x => x.PdfText != null).Select(x => x.PdfText).ToList();

            return pdfList;
        }

        public DateTime ToDate2(string value)
        {
            return DateTime.ParseExact(value, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    internal class FmyDisplayLiabilitiesForYear
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string Status { get; set; }
        public ILocator Locator { get; set; }
        public string PdfText { get; set; }
    }

    internal class FmyDisplayDeclaration
    {
        public string Id { get; set; }
        public string Kind { get; set; }
        public ILocator Locator { get; set; }
    }
}
