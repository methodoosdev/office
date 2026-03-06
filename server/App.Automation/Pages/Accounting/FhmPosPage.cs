using App.Core.Infrastructure;
using App.Core.Infrastructure.Dtos.Accounting;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Automation.Pages.Accounting
{
    public class FhmPosPage : PageTest
    {
        //TaxisNet
        private readonly string _aadeLaunchingPage = "https://www1.aade.gr/saadeapps3/comregistry/?#!/arxiki";
        private readonly string _taxisNetLoginPage = "https://login.gsis.gr/mylogin/login.jsp";
        private readonly string _taxisNetHomePage = "https://www1.aade.gr/taxisnet/mytaxisnet/protected/home.htm";

        private ILocator _messageLocator;
        private ILocator _usernameLocator;
        private ILocator _passwordLocator;
        private ILocator _selectAllCashRegistryLocator;
        private ILocator _cashRegisterTableLocator;
        private ILocator _selectAllPosLocator;
        private ILocator _posTableLocator;
        private ILocator _selectAllContractsLocator;
        private ILocator _contractsTableLocator;
        private ILocator _buttonPosition1Locator;
        private ILocator _buttonPosition3Locator;
        private ILocator _section6Locator;
        private ILocator _section7Locator;

        public FhmPosPage(string connectionId) : base(connectionId: connectionId, headless: false)
        {
            _messageLocator = Page.Locator("//html/body/table/tbody/tr[4]/td/table/tbody/tr/td[1]/img");
            _usernameLocator = Page.Locator("[name=username]");
            _passwordLocator = Page.Locator("[name=password]");
            _selectAllCashRegistryLocator = Page.Locator("//html/body/div/div/section/div/section[6]/div/div/div/div[2]/div/div[1]/div[1]/div/label/select");
            _cashRegisterTableLocator = Page.Locator("//html/body/div/div/section/div/section[6]/div/div/div/div[2]/div/div[2]/div/table/tbody/tr");
            _selectAllPosLocator = Page.Locator("//html/body/div/div/section/div/section[7]/div/div/div/div[2]/div[2]/div[1]/div[1]/div/label/select");
            _posTableLocator = Page.Locator("//html/body/div/div/section/div/section[7]/div/div/div/div[2]/div[2]/div[2]/div/table/tbody/tr");
            _selectAllContractsLocator = Page.Locator("/html/body/div/div/section/div/section[8]/div/div/div/div[2]/div[2]/div[1]/div[1]/div/label/select");
            _contractsTableLocator = Page.Locator("/html/body/div/div/section/div/section[8]/div/div/div/div[2]/div[2]/div[2]/div/table/tbody/tr");
            _buttonPosition1Locator = Page.Locator("//html/body/div/div/section/div/section[5]/div[1]/div[1]");
            _buttonPosition3Locator = Page.Locator("//html/body/div/div/section/div/section[4]/div[1]/div[3]");
            _section6Locator = Page.Locator("//html/body/div/div/section/div/section[7]");
            _section7Locator = Page.Locator("//html/body/div/div/section/div/section[8]");
        }

        public async Task<bool> LoginSucces(string userName, string password)
        {
            try
            {
                await GotoToUrl(Page, _aadeLaunchingPage, _taxisNetLoginPage);

                await _usernameLocator.FillAsync(userName);
                await _passwordLocator.FillAsync(password);
                await Page.Locator("[name=btn_login]").ClickAsync();
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                var succes = Page.Url.Equals("https://www1.aade.gr/saadeapps3/comregistry/?#!/arxiki");

                if (succes)
                    return true;
                else
                    throw new Exception();

            }
            catch
            {
                Console.WriteLine("${traderId}-{traderName}");
                return false;
            }
        }

        protected override async Task LogoutAsync()
        {
            await GotoToUrl(Page, _taxisNetHomePage);

            Page.Dialog += async (_, dialog) =>
            {
                await dialog.AcceptAsync();
            };

            var _exitLocator = Page.Locator("//html/body/table/tbody/tr[3]/td/table/tbody/tr/td/table/tbody/tr[1]/td[9]/a");
            await _exitLocator.ClickAsync();
        }

        public async Task<(IList<FhmItemModel> List1, IList<PosItemModel> List2, IList<ContractsItemModel> List3)> Execute()
        {
            // Login
            //await _usernameLocator.FillAsync(userName);
            //await _passwordLocator.FillAsync(password);
            //await Page.Locator("[name=btn_login]").ClickAsync();
            //await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var section4 = Page.Locator("//html/body/div/div/section/div/section[4]");

            var visible = await section4.IsVisibleAsync();

            if (visible)
                await _buttonPosition3Locator.ClickAsync();
            else
                await _buttonPosition1Locator.ClickAsync();

            await SelectOptionAsync(_selectAllCashRegistryLocator, "-1");

            var cashRows = (await _cashRegisterTableLocator.AllAsync()).ToList();

            var cashRegisterResult = new List<FhmItemModel>();

            foreach (var cashRow in cashRows)
            {
                var columns = await cashRow.Locator("> td").AllAsync();

                if (columns.Count > 1)
                {
                    var cashRegister = new FhmItemModel
                    {
                        CashRegisterId = int.Parse((await columns[0].InnerTextAsync()).Trim()),
                        StartingDate = (await columns[1].InnerTextAsync()).Trim().ToDateGR(),
                        Tameiaki = (await columns[2].InnerTextAsync()).Trim(),
                        InstallationNumber = (await columns[3].InnerTextAsync()).Trim(),
                        TameiakiType = (await columns[4].InnerTextAsync()).Trim()
                    };

                    cashRegisterResult.Add(cashRegister);
                }

            }

            var posRows = await GetTableRowsById(_section6Locator);
            var posResult = new List<PosItemModel>();

            foreach (var posRow in posRows)
            {
                var columns = await posRow.Locator("> td").AllAsync();

                if (columns.Count > 1)
                {
                    var date = await columns[7].InnerTextAsync();
                    DateTime? deactivateDate = string.IsNullOrEmpty(date) ? null : date.Trim().ToDateGR();

                    var pos = new PosItemModel
                    {
                        PosId = int.Parse((await columns[0].InnerTextAsync()).Trim()),
                        Provider = (await columns[1].InnerTextAsync()).Trim(),
                        DestinationTerminalID = (await columns[2].InnerTextAsync()).Trim(),
                        DestinationMerchantID = (await columns[3].InnerTextAsync()).Trim(),
                        PosType = (await columns[4].InnerTextAsync()).Trim(),
                        Status = (await columns[5].InnerTextAsync()).Trim(),
                        ActivationDate = (await columns[6].InnerTextAsync()).Trim().ToDateGR(),
                        DectivationDate = deactivateDate
                    };

                    posResult.Add(pos);
                }

            }

            var contractsRows = await GetTableRowsById(_section7Locator);

            var contractsResult = new List<ContractsItemModel>();

            foreach (var contractsRow in contractsRows)
            {
                var columns = await contractsRow.Locator("> td").AllAsync();

                if (columns.Count > 1)
                {
                    var contract = new ContractsItemModel
                    {
                        PosId = int.Parse((await columns[0].InnerTextAsync()).Trim()),
                        Αcquirer = (await columns[1].InnerTextAsync()).Trim(),
                        DestinationTerminalID = (await columns[2].InnerTextAsync()).Trim(),
                        DestinationMerchantID = (await columns[3].InnerTextAsync()).Trim(),
                        AccountNumber = (await columns[4].InnerTextAsync()).Trim(),
                        StartingDate = (await columns[5].InnerTextAsync()).Trim().ToDateGR(),
                        Provider = (await columns[6].InnerTextAsync()).Trim()
                    };

                    contractsResult.Add(contract);
                }

            }

            return (cashRegisterResult, posResult, contractsResult);
        }

        private async Task<IList<ILocator>> GetTableRowsById(ILocator xPath)
        {
            var select = (await xPath.Locator("select").AllAsync()).FirstOrDefault();
            await SelectOptionAsync(select, "-1");

            var table = xPath.Locator("[id=postable]");
            var rows = (await table.Locator("> tbody tr").AllAsync()).ToList();

            return rows;
        }
    }
}
