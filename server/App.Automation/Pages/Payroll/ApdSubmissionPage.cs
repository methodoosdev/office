using App.Core.Infrastructure.Dtos.Payroll;
using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Automation.Pages.Payroll
{
    public class ApdSubmissionPage : UserHasMessagesPage
    {
        private readonly string _efkaLoginPage = "https://apps.e-efka.gov.gr/eAPDsso";
        private readonly string _efkaHomePage = "https://services.e-efka.gov.gr/ssp.efka.apd/secure/submissions.xhtml";
        private readonly string _efkaSubmissionsPage = "https://apps.e-efka.gov.gr/eAPDsso/faces/secureAdmin/submissions.xhtml";
        private readonly string _messagePage = "https://apps.e-efka.gov.gr/eMessages/secure/Messages.xhtml";
        protected readonly string _errorLoginPage = "https://apps.e-efka.gov.gr/eAccess/j_security_check";

        private ILocator _amoeLocator;
        private ILocator _employerLocator;
        private ILocator _usernameLocator;
        private ILocator _passwordLocator;
        private ILocator _submitButtonLocator;
        private ILocator _submitAmoeButtonLocator;

        private ILocator _nextLocator;
        private ILocator _paginatorInfoLocator;
        private ILocator _apdTableLocator;

        public ApdSubmissionPage(string connectionId) : base(connectionId: connectionId, emulateBrowserEnable: false, headless: false)
        {
            _amoeLocator = Page.Locator("[id=ame]");
            _employerLocator = Page.Locator("[id=social-taxisnet-employer]");
            _usernameLocator = Page.Locator("[name=j_username]");
            _passwordLocator = Page.Locator("[name=j_password]");
            _submitButtonLocator = Page.Locator("[id=btn-login-submit]");
            _submitAmoeButtonLocator = Page.Locator("[id=submit-role-attribute]");

            _nextLocator = Page.Locator("//html/body/div[2]/div[1]/form/table[1]/tbody/tr/td/div/table/thead/tr[1]/th/span[4]");
            _paginatorInfoLocator = Page.Locator("//html/body/div[2]/div[1]/form/table[1]/tbody/tr/td/div/table/thead/tr[1]/th/span[3]");
            _apdTableLocator = Page.Locator("//html/body/div[2]/div[1]/form/table[1]/tbody/tr/td/div/table/tbody/tr");
        }

        protected override async Task LogoutAsync()
        {
            if (!LoginIn)
                return;

            var growlItems = Page.Locator(".ui-growl-item-container:visible");
            int count = await growlItems.CountAsync();

            if (count > 0)
            {
                await Page.EvaluateAsync(@"() => {
                     document.querySelectorAll('.ui-growl').forEach(g => g.remove());
                 }");
            }

            var _backLocator = Page.Locator("//html/body/div[1]/div[1]/div[3]/ul/li[1]/a").First;
            await ClickAsync(_backLocator);

            var _logoutLocator = Page.GetByRole(AriaRole.Link, new() { Name = "Αποσύνδεση" }).First;
            await ClickAsync(_logoutLocator);
        }

        public async Task Login(string userName, string password, string amIka)
        {
            await Goto(Page, "https://services.e-efka.gov.gr/ssp.efka.apd/secure/submissions.xhtml");
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            EnsurePageExist(Page, "https://services.e-efka.gov.gr/eEfkaExtAuth/realms/EFKA/protocol/openid-connect/auth");

            await ClickAsync(_employerLocator);
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            EnsurePageExist(Page, "https://oauth2.gsis.gr/oauth2server/login.jsp");

            await FillAsync(_usernameLocator, userName);
            await FillAsync(_passwordLocator, password); 
            await ClickAsync(_submitButtonLocator);
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            EnsurePageExist(Page, "https://oauth2.gsis.gr/oauth2server/oauth/authorize");

            var confirmLocator = Page.Locator("[id=btn-submit]");
            await ClickAsync(confirmLocator);
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            EnsurePageExist(Page, "https://services.e-efka.gov.gr/eEfkaExtAuth/realms/EFKA/login-actions/post-broker-login");

            var selectPageLocator = Page.Locator("//html/body/div/div/div/div/div/div[1]/div[2]/div[1]/div/form/div[1]/select");
            await SelectOptionAsync(selectPageLocator, "Εργοδότης Κοινής Επιχείρησης", SelectOptionValueType.Label);
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            await FillAsync(_amoeLocator, amIka);
            await ClickAsync(_submitAmoeButtonLocator);
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            EnsurePageExist(Page, "https://services.e-efka.gov.gr/ssp.efka.apd/secure/submissions.xhtml");

            LoginIn = true;
        }
        public async Task<List<ApdSubmissionDto>> Execute(int month, int year)
        {
            var apds = new List<ApdSubmissionDto>();

            var submissionPeriodFrom_input = Page.Locator("[id=submissionPeriodFrom_input]").First;
            await submissionPeriodFrom_input.FillAsync($"{month}/{year}");
            await submissionPeriodFrom_input.DispatchEventAsync("input");
            await submissionPeriodFrom_input.DispatchEventAsync("change");
            await submissionPeriodFrom_input.PressAsync("Enter");
            await submissionPeriodFrom_input.BlurAsync();

            var submissionPeriodΤο_input = Page.Locator("[id=submissionPeriodΤο_input]").First;
            await submissionPeriodΤο_input.FillAsync($"{month}/{year}");
            await submissionPeriodΤο_input.DispatchEventAsync("input");
            await submissionPeriodΤο_input.DispatchEventAsync("change");
            await submissionPeriodΤο_input.PressAsync("Enter");
            await submissionPeriodΤο_input.BlurAsync();

            //await ClickAsync(Page.Locator("[id=executeCriteriaBtn]"));
            //await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            var table = Page.Locator("[id='submissionTable_data']");
            var rows = table.Locator("tr");
            int rowCount = await rows.CountAsync();

            for (int r = 0; r < rowCount; r++)
            {
                var row = rows.Nth(r);

                // Αν υπάρχει περίπτωση header row:
                // if (await row.Locator("th").CountAsync() > 0) continue;

                var pdfLocator = row.Locator("td").Nth(4).Locator("a").Last;
                if (!await IsLocatorExist(pdfLocator))
                    break;

                var download = await Page.RunAndWaitForDownloadAsync(async () =>
                {
                    await pdfLocator.ClickAsync();
                });

                // 1) Πάρε path (συνήθως δουλεύει τοπικά)
                var path = await download.PathAsync();

                var text = ExtractTextFromPdf(path);

                apds.Add(new ApdSubmissionDto { Found = true, Year = year, PdfText = text });

                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }


            // Με αυτο παιρνουμε μονο ΑΠΔ

            //var total = 0m;
            //foreach (var row in rows1)
            //{
            //    var rowLocator = row.Locator("> td");
            //    var columns = await GetAllAsync(rowLocator);

            //    await ClickAsync(columns[0]);
            //    var valStr = await InputValueAsync(Page.Locator("[id=sumsForm:totalContributionsEmp]"));
            //    var value = Convert.ToDecimal(valStr.Trim(), new CultureInfo("el-GR"));
            //    total += value;

            //    await ClickAsync(Page.Locator("[id=sumsForm:j_idt367]"));
            //}

            //var apd = new ApdSubmissionDto { Found = true, PdfText = "hack", ApdTotal = total };

            return apds;
        }
    }
}
