using Microsoft.Playwright;
using System.Threading.Tasks;

namespace App.Automation.Pages.Financial
{
    public class OaeeRetrievePage : BaseKeaoRetrievePage
    {
        //ΟΑΕΕ
        private readonly string _keaoLoginPage = "https://apps.e-efka.gov.gr/eAccess/login.xhtml";
        private readonly string _debtorMainPage = "https://apps.e-efka.gov.gr/eDebtor/secure/index.xhtml";
        private readonly string _messagePage = "https://apps.e-efka.gov.gr/eMessages/secure/Messages.xhtml";

        private ILocator _keaoLoginLocator;

        public OaeeRetrievePage(string connectionId) : base(connectionId)
        {
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
                    await ClickAsync(Page.GetByText("Παράλειψη"));
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
        }
    }
}
