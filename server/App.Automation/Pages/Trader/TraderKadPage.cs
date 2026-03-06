using App.Core.Infrastructure.Dtos.Trader;
using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Automation.Pages.Trader
{
    public class TraderKadPage : PageTest
    {
        //TaxisNet
        private readonly string _taxisNetLaunchingPage = "https://www1.aade.gr/taxisnet/mytaxisnet";
        private readonly string _taxisNetLoginPage = "https://login.gsis.gr/mylogin/login.jsp";
        private readonly string _taxisNetHomePage = "https://www1.aade.gr/taxisnet/mytaxisnet/protected/home.htm";
        private readonly string _taxisNetFirmRegistryInfoPage = "https://www1.aade.gr/taxisnet/info/protected/displayFirmRegistryInfo.htm";

        private ILocator _messageLocator;
        private ILocator _usernameLocator;
        private ILocator _passwordLocator;
        private string _kadTablePath;

        public TraderKadPage(string connectionId) : base(connectionId: connectionId)
        {
            _messageLocator = Page.Locator("//html/body/table/tbody/tr[4]/td/table/tbody/tr/td[1]/img");
            _usernameLocator = Page.Locator("[name=username]");
            _passwordLocator = Page.Locator("[name=password]");
            _kadTablePath = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td/table/tbody/tr[3]/td/table/tbody/tr/td/table/tbody/tr[7]/td/table/tbody/tr/td/table/tbody/tr";
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

        public async Task<List<TraderKadDto>> Execute(string userName, string password, int traderId)
        {
            var result = new List<TraderKadDto>();

            await GotoToUrl(Page, _taxisNetLaunchingPage, _taxisNetLoginPage);

            // Login
            await FillAsync(_usernameLocator, userName);
            await FillAsync(_passwordLocator, password);
            await ClickToUrl(Page, "[name=btn_login]", _taxisNetHomePage);

            await GotoToUrl(Page, _taxisNetFirmRegistryInfoPage);

            var _kadTableLocator = Page.Locator(string.Format(_kadTablePath, await IsLocatorExist(_messageLocator) ? 6 : 5));

            var rows = await GetAllAsync(_kadTableLocator);
            rows.RemoveAt(0); //Remove header row

            foreach (var row in rows)
            {
                var rowLocator = await GetLocatorAsync(row, "> td");
                var columns = await GetAllAsync(rowLocator);
                var text = await InnerTextAsync(columns[4]);

                if (string.IsNullOrEmpty(text))
                {
                    var traderKad = new TraderKadDto
                    {
                        GroupId = 0,
                        Code = await InnerTextAsync(columns[0]),
                        Description = await InnerTextAsync(columns[1]),
                        Activity = (await InnerTextAsync(columns[2])).Equals("Κύρια"),
                        TraderId = traderId
                    };
                    result.Add(traderKad);
                }
            }

            return result;
        }
    }
}
