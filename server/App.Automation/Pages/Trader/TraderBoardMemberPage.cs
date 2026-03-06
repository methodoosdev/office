using App.Core.Infrastructure;
using App.Core.Infrastructure.Dtos;
using App.Core.Infrastructure.Dtos.Trader;
using Microsoft.Playwright;
using System.Globalization;
using System.Threading.Tasks;

namespace App.Automation.Pages.Trader
{
    public class TraderBoardMemberPage : PageTest
    {
        //TaxisNet
        private readonly string _taxisNetLaunchingPage = "https://www1.aade.gr/taxisnet/mytaxisnet";
        private readonly string _taxisNetLoginPage = "https://login.gsis.gr/mylogin/login.jsp";
        private readonly string _taxisNetHomePage = "https://www1.aade.gr/taxisnet/mytaxisnet/protected/home.htm";
        private readonly string _taxisNetRegistryInfoPage = "https://www1.aade.gr/taxisnet/info/protected/displayRegistryInfo.htm";

        //private ILocator _messageLocator;
        private ILocator _usernameLocator;
        private ILocator _passwordLocator;

        private string _relationshipsTablePath;
        private string _membershipsTablePath;

        public TraderBoardMemberPage(string connectionId) : base(connectionId: connectionId, headless: false)
        {
            //_messageLocator = Page.Locator("//html/body/table/tbody/tr[4]/td/table/tbody/tr/td[1]/img");
            _usernameLocator = Page.Locator("[name=username]");
            _passwordLocator = Page.Locator("[name=password]");

            _relationshipsTablePath = "//html/body/table/tbody/tr[4]/td/table/tbody/tr[1]/td/table/tbody/tr[3]/td/table/tbody/tr/td/table/tbody/tr[7]/td/table/tbody/tr/td/table/tbody/tr";
            _membershipsTablePath = "//html/body/table/tbody/tr[4]/td/table/tbody/tr[1]/td/table/tbody/tr[3]/td/table/tbody/tr/td/table/tbody/tr[11]/td/table/tbody/tr/td/table/tbody/tr";
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

            // Login
            await FillAsync(_usernameLocator, userName);
            await FillAsync(_passwordLocator, password);
            await ClickToUrl(Page, "[name=btn_login]", _taxisNetHomePage);

            return true;
        }
        public async Task<TraderBoardMemberDto> Execute()
        {
            var result = new TraderBoardMemberDto();

            await GotoToUrl(Page, _taxisNetRegistryInfoPage);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            //var messageExist = await IsLocatorExist(_messageLocator);
            //var _branchesTableLocator = Page.Locator(string.Format(_branchesTablePath, messageExist ? 6 : 5));

            var _relationshipTableLocator = Page.Locator(_relationshipsTablePath);
            var rows = await GetAllAsync(_relationshipTableLocator);
            rows.RemoveAt(0); //Remove header row

            foreach (var row in rows)
            {
                var rowLocator = await GetLocatorAsync(row, "> td");
                var columns = await GetAllAsync(rowLocator);

                //Check if the member is active or not
                var endDate = await InnerTextAsync(columns[4]);
                if (!string.IsNullOrEmpty(endDate))
                    continue;

                var relationship = new TraderRelationshipDto
                {
                    Vat = await InnerTextAsync(columns[0]),
                    SurnameFatherName = await InnerTextAsync(columns[1]),
                    RelationshipName = await InnerTextAsync(columns[2]),
                    StartDateOnUtc = (await InnerTextAsync(columns[3])).Trim().ToDateGR()
                };

                result.TraderRelationships.Add(relationship);
            }

            var _membershipTableLocator = Page.Locator(_membershipsTablePath);
            var membershipRows = await GetAllAsync(_membershipTableLocator);
            membershipRows.RemoveAt(0); //Remove header row

            foreach (var row in membershipRows)
            {
                var rowLocator = await GetLocatorAsync(row, "> td");
                var columns = await GetAllAsync(rowLocator);

                //Check if the member is active or not
                var memberEndDate = await InnerTextAsync(columns[3]);
                if (!string.IsNullOrEmpty(memberEndDate))
                    continue;

                var numberFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };
                var fractionText = await InnerTextAsync(columns[6]);
                var fractionArray = fractionText.Split('/');
                var percent = CalculatePercentage(fractionArray[0], fractionArray[1]);

                var membership = new TraderMembershipDto
                {
                    Vat = await InnerTextAsync(columns[0]),
                    SurnameFatherName = await InnerTextAsync(columns[1]),
                    StartDateOnUtc = (await InnerTextAsync(columns[2])).ToDateGR(),
                    ExpireDateOnUtc = string.IsNullOrEmpty(memberEndDate) ? null : memberEndDate.ToDateGR(),
                    ParticipationName = await InnerTextAsync(columns[4]),
                    ParticipationRate = decimal.Parse(await InnerTextAsync(columns[5]), numberFormatInfo),
                    ParticipatingFraction = percent.ToString()
                };

                result.TraderMemberships.Add(membership);
            }

            return result;

        }

        private decimal CalculatePercentage(string x, string k)
        {
            int.TryParse(x, out int a);
            int.TryParse(k, out int b);

            if (b == 0) // Prevent division by zero
            {
                return 0;
            }
            return a / b * 100;
        }
    }
}
