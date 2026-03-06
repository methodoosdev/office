using Microsoft.Playwright;
using System.Threading.Tasks;

namespace App.Automation.Pages.Financial
{
    public class KeaoRetrievePage : BaseKeaoRetrievePage
    {
        //ΚΕΑΟ
        private readonly string _keaoLoginPage = "https://apps.e-efka.gov.gr/eAccess/login.xhtml";
        private readonly string _debtorMainPage = "https://apps.e-efka.gov.gr/eDebtor/secure/index.xhtml";
        private readonly string _messagePage = "https://apps.e-efka.gov.gr/eMessages/secure/Messages.xhtml";
        //private readonly string _wrongCredentials = "https://www1.gsis.gr/oauth2server/login.jsp";


        private ILocator _taxisnetNextLocator;
        private ILocator _taxisLoginButtonLocator;
        private ILocator _sendButtonLocator;
        private ILocator _loginButtonLocator;
        private ILocator _userTypeLocator;
        private ILocator _bottomLocator;
        private ILocator _menuLocator;
        private ILocator _footerEfkaLocator;
        private ILocator _bottom_linksLocator;

        private ILocator _keaoLoginLocator;

        public KeaoRetrievePage(string connectionId) : base(connectionId)
        {
            _taxisnetNextLocator = Page.GetByText("Συνέχεια στο TAXISNET");
            _taxisLoginButtonLocator = Page.Locator("[id=btn-login-submit]");
            _sendButtonLocator = Page.Locator("[id=btn-submit]");
            _loginButtonLocator = Page.Locator("[id=j_idt38]");
            _userTypeLocator = Page.GetByText("Επιχείρηση/Πολίτης");
            _bottomLocator = Page.Locator("[id=bottom]");
            _menuLocator = Page.Locator("[id=MenuForm:mainMenu]");
            _footerEfkaLocator = Page.Locator("[class=login-footer-efka]");
            _bottom_linksLocator = Page.Locator("[class=bottom_links]");

            _keaoLoginLocator = Page.GetByText("Είσοδος");
        }

        public async Task<bool> Login(string userName, string password)
        {
            //   ***ΚΕΑΟ***
            await Goto(Page, _keaoLoginPage);
            await WaitForAsync(_keaoLoginLocator, 30000);

            await FillAsync(Page, "[id=j_username]", userName);
            await FillAsync(Page, "[id=j_password]", password);
            await ClickAsync(_keaoLoginLocator);

            if (_errorLoginPage.Equals(Page.Url))
                return false;

            await Goto(Page, _debtorMainPage);
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            // Έλγχος όταν γίνεται είσοδος αν χρειάζεται στοιχεία επικοινωνίας ή έχει μηνύματα
            if (Page.Url.Contains("https://apps.e-efka.gov.gr/eProfile/"))
            {
                var skipExists = await IsLocatorExist(Page.GetByText("Παράλειψη"));
                if (skipExists)
                {
                    await ClickAsync(Page.GetByText("Παράλειψη"));
                    await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                }
                else
                    throw new AppPlaywrightException($"Χρειάζεται επαλήθευση των στοιχείων επικοινωνίας");
            }

            if (_messagePage.Equals(Page.Url))
            {
                await TraderHasMessagesAsync();
                await Goto(Page, _debtorMainPage);
                await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            };

            return true;
            //    //   ***ΚΕΑΟ***
            //    await Goto(Page, _keaoLoginPage);
            //    await WaitForAsync(_taxisnetNextLocator, 30000);

            //    await ClickAsync(_taxisnetNextLocator);
            //    await WaitForAsync(_taxisLoginButtonLocator, 30000);

            //    await FillAsync(Page, "[id=v]", userName);
            //    await FillAsync(Page, "[id=j_password]", password);
            //    await ClickAsync(_taxisLoginButtonLocator);

            //    // Επιστροφή αν έχει λάθος κωδικούς taxis
            //    if (Page.Url.Contains(_wrongCredentials))
            //        return false;

            //    await WaitForAsync(_sendButtonLocator);

            //    await ClickAsync(_sendButtonLocator, 700);
            //    await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            //    await _bottom_linksLocator.WaitForAsync();;

            //    await ClickAsync(_userTypeLocator);
            //    await _bottom_linksLocator.WaitForAsync();
            //    await ClickAsync(_loginButtonLocator);
            //    await WaitForAsync(_footerEfkaLocator);

            //    if (_errorLoginPage.Equals(Page.Url))
            //        return false;

            //    await Goto(Page, _debtorMainPage);

            //    // Έλγχος όταν γίνεται είσοδος αν χρειάζεται στοιχεία επικοινωνίας ή έχει μηνύματα
            //    if (Page.Url.Contains("https://apps.e-efka.gov.gr/eProfile/"))
            //    {
            //        var skipExists = await IsLocatorExist(Page.GetByText("Παράλειψη"));
            //        if (skipExists)
            //            await ClickAsync(Page.GetByText("Παράλειψη"));
            //        else
            //            throw new AppPlaywrightException($"Δεν υπάρχουν στοιχεία επικοινωνίας");
            //    }

            //    return true;
        }
    }
}
