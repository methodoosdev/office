using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Trader;
using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Automation.Pages.Trader
{
    public class TraderBranchPage : PageTest
    {
        //TaxisNet
        private readonly string _taxisNetLaunchingPage = "https://www1.aade.gr/taxisnet/mytaxisnet";
        private readonly string _taxisNetLoginPage = "https://login.gsis.gr/mylogin/login.jsp";
        private readonly string _taxisNetHomePage = "https://www1.aade.gr/taxisnet/mytaxisnet/protected/home.htm";
        private readonly string _taxisNetFirmRegistryInfoPage = "https://www1.aade.gr/taxisnet/info/protected/displayFirmRegistryInfo.htm";
        private readonly string _taxisNetFirmBranchInfoPage = "https://www1.aade.gr/taxisnet/info/protected/displayFirmBranchInfo.htm";

        private ILocator _messageLocator;
        private ILocator _usernameLocator;
        private ILocator _passwordLocator;
        private string _branchesTablePath;
        private string _branchKadTablePath;
        private string _branchKindPath;
        private string _branchAddressPath;
        private string _branchDoyPath;
        private string _branchExpiredPath;
        private string _goBackButtonPath;

        public TraderBranchPage(string connectionId) : base(connectionId: connectionId)
        {
            _messageLocator = Page.Locator("//html/body/table/tbody/tr[4]/td/table/tbody/tr/td[1]/img");
            _usernameLocator = Page.Locator("[name=username]");
            _passwordLocator = Page.Locator("[name=password]");

            _branchesTablePath = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td/table/tbody/tr[3]/td/table/tbody/tr/td/table/tbody/tr[11]/td/table/tbody/tr/td/table/tbody/tr";
            _branchKadTablePath = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td/table/tbody/tr[4]/td/table/tbody/tr/td/table/tbody/tr[7]/td/table/tbody/tr/td/table/tbody/tr";
            _branchKindPath = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td/table/tbody/tr[4]/td/table/tbody/tr/td/table/tbody/tr[3]/td/table/tbody/tr[2]/td[2]";
            _branchAddressPath = "//html/body/table/tbody/tr[{0}]/td/table/tbody[1]/tr/td/table/tbody/tr[4]/td/table/tbody/tr/td/table/tbody/tr[3]/td/table/tbody/tr[10]/td[2]";
            _branchDoyPath = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td/table/tbody/tr[4]/td/table/tbody/tr/td/table/tbody/tr[3]/td/table/tbody/tr[7]/td[2]";
            _branchExpiredPath = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td/table/tbody/tr[4]/td/table/tbody/tr/td/table/tbody/tr[3]/td/table/tbody/tr[5]/td[2]";
            _goBackButtonPath = "//html/body/table/tbody/tr[{0}]/td/table/tbody/tr[1]/td/table/tbody/tr[4]/td/table/tbody/tr/td/table/tbody/tr[9]/td/input";
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

        public async Task<TraderBranchPageDto> Execute(string userName, string password, int traderId)
        {
            var result = new TraderBranchPageDto();

            await GotoToUrl(Page, _taxisNetLaunchingPage, _taxisNetLoginPage);

            // Login
            await FillAsync(_usernameLocator, userName);
            await FillAsync(_passwordLocator, password);
            await ClickToUrl(Page, "[name=btn_login]", _taxisNetHomePage);

            await GotoToUrl(Page, _taxisNetFirmRegistryInfoPage);

            var messageExist = await IsLocatorExist(_messageLocator);
            var _branchesTableLocator = Page.Locator(string.Format(_branchesTablePath, messageExist ? 6 : 5));
            var _branchKadTableLocator = Page.Locator(string.Format(_branchKadTablePath, messageExist ? 6 : 5));
            var _branchKindLocator = Page.Locator(string.Format(_branchKindPath, messageExist ? 6 : 5));
            var _branchAddressLocator = Page.Locator(string.Format(_branchAddressPath, messageExist ? 6 : 5));
            var _branchDoyLocator = Page.Locator(string.Format(_branchDoyPath, messageExist ? 6 : 5));
            var _branchExpiredLocator = Page.Locator(string.Format(_branchExpiredPath, messageExist ? 6 : 5));
            var _goBackButtonLocator = Page.Locator(string.Format(_goBackButtonPath, messageExist ? 6 : 5));

            var rows = await GetAllAsync(_branchesTableLocator);
            rows.RemoveAt(0); //Remove header row

            var branches = new Dictionary<int, string>();

            foreach (var row in rows)
            {
                var rowLocator = await GetLocatorAsync(row, "> td");
                var columns = await GetAllAsync(rowLocator);
                var key = int.Parse(await InnerTextAsync(columns[0]));

                var buttonXPath = GetXPathFromLocator(columns[4].GetByRole(AriaRole.Button));
                branches.Add(key, buttonXPath);
            }

            foreach (var branch in branches)
            {
                await ClickToUrl(Page, branch.Value, _taxisNetFirmBranchInfoPage);

                var branchExpiredDate = await InnerTextAsync(_branchExpiredLocator);
                if (string.IsNullOrEmpty(branchExpiredDate))
                {
                    var item = new Core.Infrastructure.Dtos.Trader.TraderBranchDto
                    {
                        GroupId = branch.Key,
                        Kind = await InnerTextAsync(_branchKindLocator),
                        Address = await InnerTextAsync(_branchAddressLocator),
                        Doy = await InnerTextAsync(_branchDoyLocator),
                        TraderId = traderId
                    };
                    result.TraderBranches.Add(item);

                    var rows1 = await GetAllAsync(_branchKadTableLocator);
                    rows1.RemoveAt(0);

                    foreach (var row1 in rows1)
                    {
                        var rowLocator1 = await GetLocatorAsync(row1, "> td");
                        var columns1 = await GetAllAsync(rowLocator1);

                        var text = await InnerTextAsync(columns1[4]);
                        if (string.IsNullOrEmpty(text))
                        {
                            var traderKad = new TraderKadDto
                            {
                                GroupId = branch.Key,
                                Code = await InnerTextAsync(columns1[0]),
                                Description = await InnerTextAsync(columns1[1]),
                                Activity = (await InnerTextAsync(columns1[2])).Equals("Κύρια"),
                                TraderId = traderId
                            };
                            result.TraderKads.Add(traderKad);
                        }
                    }
                }
                await ClickToUrl(_goBackButtonLocator, _taxisNetFirmRegistryInfoPage);
            }

            return result;
        }
    }
}
