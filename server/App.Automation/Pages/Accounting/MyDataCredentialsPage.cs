using App.Core.Infrastructure.Dtos.Accounting;
using Microsoft.Playwright;
using System.Linq;
using System.Threading.Tasks;

namespace App.Automation.Pages.Accounting
{
    public class MyDataCredentialsPage : PageTest
    {
        private readonly string _myDataHomePage = "https://www1.aade.gr/saadeapps2/bookkeeper-web/bookkeeper";
        private readonly string _taxisNetLoginPage = "https://login.gsis.gr/mylogin/login.jsp";
        private readonly string _apiSubscriptionPage = "https://www1.aade.gr/saadeapps2/bookkeeper-web/bookkeeper/#!/apiSubscription";

        private ILocator _usernameLocator;
        private ILocator _passwordLocator;
        private ILocator _subscriptionTableLocator;
        private ILocator _loginButton;

        public MyDataCredentialsPage(string connectionId) : base(connectionId: connectionId, headless: false)
        {
            _usernameLocator = Page.Locator("[name=username]");
            _passwordLocator = Page.Locator("[name=password]");
            //_subscriptionTableLocator = _page.Locator("[id=subscriptions]");
            _subscriptionTableLocator = Page.Locator("//html/body/div[5]/div/div[7]/div/div[1]/table/tbody/tr");
            _loginButton = Page.Locator("[name=btn_login]");
        }

        protected override async Task LogoutAsync()
        {
            var _exitLocator = Page.Locator("//*[@id=\"myAccountSpan\"]");
            await ClickAsync(_exitLocator);

            var _logoutLocator = Page.Locator("div.btn.myAccountBtn.logout");
            await ClickToUrl(_logoutLocator, _taxisNetLoginPage);
        }

        public async Task<bool> Login(string userName, string password)
        {
            await GotoToUrl(Page, _myDataHomePage, _taxisNetLoginPage);

            // Login
            await FillAsync(_usernameLocator, userName);
            await FillAsync(_passwordLocator, password);
            //await ClickToUrl(_page, "[name=btn_login]", _myDataHomePage);
            await ClickAsync(_loginButton);

            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            return true;
        }
        public async Task<MyDataCredentialDto> Execute()
        {
            await Goto(Page, _apiSubscriptionPage);
            //await GotoToUrl(_page, _apiSubscriptionPage);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var notRegistered = await IsLocatorExist(Page.GetByText("Δεν βρέθηκαν χρήστες"));
            if (notRegistered)
                throw new AppPlaywrightException("Δεν βρέθηκαν χρήστες");

            var subscriptionRow = (await GetAllAsync(_subscriptionTableLocator)).FirstOrDefault();

            var rowLocator = await GetLocatorAsync(subscriptionRow, "> td");
            var columns = await GetAllAsync(rowLocator);

            var credentials = new MyDataCredentialDto
            {
                SubscriptionKey = await InnerTextAsync(columns[0]),
                UserName = await InnerTextAsync(columns[1])
            };

            return credentials;
        }
    }
}
