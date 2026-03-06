using Microsoft.Playwright;
using System.Threading.Tasks;

namespace App.Automation.Pages.Accounting
{
    public class PiraeusPage : PageTest
    {
        private ILocator _userNameLocator;
        private ILocator _passwordLocator;
        private ILocator _signinButtonLocator;

        public PiraeusPage(string connectionId) : base(connectionId: connectionId, headless: false)
        {
            _userNameLocator = Page.Locator("[id=username]");
            _passwordLocator = Page.Locator("[id=password]");
            _signinButtonLocator = Page.Locator("//html/body/div[2]/div/div[3]/div/div[2]/form/fieldset/div[3]/button");
        }

        public async Task<string> Execute(string userName, string password)
        {
            var client_id = "d39a400c-ade4-4095-8828-f49a55fa5533";
            var scope = "winbankAccess winbankAccess.info winbankAccess.monetaryTransactions offline_access";
            var redirect_uri = "https://www.piraeusbank.gr/serefprod/authcode.aspx";

            var authorize = "https://openbank.piraeusbank.gr/identityserver/connect/authorize";
            var authorize_url = $"{authorize}?response_type=code&client_id={client_id}&scope={scope}&redirect_uri={redirect_uri}";

            var decoded = System.Web.HttpUtility.UrlDecode(authorize_url);
            //Goto login page
            await Page.GotoAsync(decoded);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            //Entering UserName
            await FillAsync(_userNameLocator, userName);
            //Entering Password
            await FillAsync(_passwordLocator, password);
            // Click sign in button
            await ClickAsync(_signinButtonLocator);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            decoded = System.Web.HttpUtility.UrlDecode(Page.Url);

            var code = decoded.Split("code=")[1];

            return code.Split("&")[0];
        }
    }
}
