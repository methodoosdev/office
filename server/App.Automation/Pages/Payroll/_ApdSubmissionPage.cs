using App.Core.Infrastructure.Dtos.Payroll;
using Microsoft.Playwright;
using System.Threading.Tasks;

namespace App.Automation.Pages.Payroll
{
    public class _ApdSubmissionPage : UserHasMessagesPage
    {
        private readonly string _efkaLoginPage = "https://apps.e-efka.gov.gr/eAPDsso";
        private readonly string _efkaHomePage = "https://apps.e-efka.gov.gr/eAPDsso/faces/secureAll/index.xhtml";//https://services.e-efka.gov.gr/ssp.efka.apd/secure/submissions.xhtml
        private readonly string _efkaSubmissionsPage = "https://apps.e-efka.gov.gr/eAPDsso/faces/secureAdmin/submissions.xhtml";
        private readonly string _messagePage = "https://apps.e-efka.gov.gr/eMessages/secure/Messages.xhtml";
        protected readonly string _errorLoginPage = "https://apps.e-efka.gov.gr/eAccess/j_security_check";

        private ILocator _usernameLocator;
        private ILocator _passwordLocator;
        private ILocator _submitButtonLocator;
        private ILocator _apdButtonLocator;
        private ILocator _nextLocator;
        private ILocator _paginatorInfoLocator;
        private ILocator _apdTableLocator;

        public _ApdSubmissionPage(string connectionId) : base(connectionId: connectionId, emulateBrowserEnable: false, headless: false)
        {
            _usernameLocator = Page.Locator("[name=j_username]");
            _passwordLocator = Page.Locator("[name=j_password]");
            _submitButtonLocator = Page.Locator("//html/body/div[1]/div/div/div[2]/form/div[3]/button");
            _apdButtonLocator = Page.Locator("//html/body/div[1]/div/div/div[2]/table[7]/tbody/tr/td[2]/a");
            _nextLocator = Page.Locator("//html/body/div[2]/div[1]/form/table[1]/tbody/tr/td/div/table/thead/tr[1]/th/span[4]");
            _paginatorInfoLocator = Page.Locator("//html/body/div[2]/div[1]/form/table[1]/tbody/tr/td/div/table/thead/tr[1]/th/span[3]");
            _apdTableLocator = Page.Locator("//html/body/div[2]/div[1]/form/table[1]/tbody/tr/td/div/table/tbody/tr");
        }

        protected override async Task LogoutAsync()
        {
            if (!LoginIn)
                return;

            var _backLocator = Page.Locator("//*[@id=\"backBtn\"]");
            await ClickAsync(_backLocator);

            var _footerLocator = Page.Locator("[id=bottom]");
            await WaitForAsync(_footerLocator, 300000);

            var _logoutLocator = await GetByTextAsync(Page, "Αποσύνδεση");
            await ClickAsync(_logoutLocator);
        }

        public async Task Login(string userName, string password)
        {
            await Goto(Page, _efkaLoginPage);
            var _bottomLinksLocator = Page.Locator("[class=bottom_links]");
            await WaitForAsync(_bottomLinksLocator, 300000);

            await FillAsync(_usernameLocator, userName);
            await FillAsync(_passwordLocator, password); 
            await ClickAsync(_submitButtonLocator);
            await WaitForAsync(Page, "[class=login_inputs]", 30000);

            if (_errorLoginPage.Equals(Page.Url))
                throw new AppPlaywrightException("Αποτυχία σύνδεσης");

            await ClickAsync(_apdButtonLocator);
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            if (_messagePage.Equals(Page.Url))
            {
                await TraderHasMessagesAsync();
                await Goto(Page, _efkaHomePage);
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
            else
            {
                var _footerLocator = Page.Locator("[id=bottom]");
                await WaitForAsync(_footerLocator, 300000);
            }

            await Goto(Page, _efkaSubmissionsPage);
            var _backBtnLocator = Page.Locator("[id=backBtn]");
            await WaitForAsync(_backBtnLocator);

            LoginIn = true;
        }
        public async Task<ApdSubmissionDto> Execute(int month, int year)
        {
            var apd = new ApdSubmissionDto { Found = false, Year = 0 };

            await _paginatorInfoLocator.WaitForAsync();
            var paginatorInfo = (await InnerTextAsync(_paginatorInfoLocator)).Replace(")", "");

            var currentPage = int.Parse(paginatorInfo.Split(' ')[1]);
            var allPages = int.Parse(paginatorInfo.Split(' ')[3]);

            for (int i = currentPage; i <= allPages; i++)
            {
                var canLoop = true;
                var rows = await GetAllAsync(_apdTableLocator);

                foreach (var row in rows)
                {
                    var rowLocator = row.Locator("> td");
                    var columns = await GetAllAsync(rowLocator);

                    if (columns.Count < 2)
                        continue;

                    var periodTagLocator = columns[2].Locator("> div");
                    var periodTag = await InnerTextAsync(periodTagLocator);
                    var period = periodTag.Split(" - ")[0];
                    var _month = int.Parse(period.Split('/')[0]);
                    var _year = int.Parse(period.Split('/')[1]);

                    if (_year < year)
                    {
                        canLoop = false;
                        break;
                    };

                    if (month.Equals(_month) && year.Equals(_year))
                    {
                        var link = await GetLocatorAsync(Page, GetXPathFromLocator(columns[4]));
                        var linkLocator = link.GetByRole(AriaRole.Link, new() { Name = "Επανέκδοση Αποδεικτικού" });
                        await ClickAsync(linkLocator);

                        var _formLocator = Page.Locator("[id=dapyFilea5a]");
                        await _formLocator.WaitForAsync();

                        async void Page_Popup(object sender, IPage popup)
                        {
                            popup.Download += async (_, download) =>
                            {
                                var path = await download.PathAsync();
                                if (string.IsNullOrEmpty(path))
                                    return;

                                apd.PdfText = ExtractTextFromPdf(path);
                                apd.Found = true;
                                apd.Year = year;
                            };

                            await popup.WaitForLoadStateAsync();
                        }

                        Page.Popup += Page_Popup;

                        var submitLocator = await GetLocatorAsync(Page, "//*[@id=\"dapyFilea5a:cmndButtonReSubmit\"]");
                        await ClickAsync(submitLocator);
                        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                        Page.Popup -= Page_Popup;

                        canLoop = false;
                        break;
                    }
                }

                if (canLoop)
                {
                    await ClickAsync(_nextLocator);
                    await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                }
                else
                    break;
            }

            return apd;
        }
    }
}
