using App.Core.Infrastructure;
using App.Core.Infrastructure.Dtos.Financial;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Automation.Pages.Financial
{
    public abstract class BaseKeaoRetrievePage : UserHasMessagesPage
    {
        private string _keao = "ΚΕΑΟ - {0}";
        protected readonly string _errorLoginPage = "https://apps.e-efka.gov.gr/eAccess/j_security_check";
        protected readonly string _debtorPaysPage = "https://apps.e-efka.gov.gr/eDebtor/secure/debtorPays.xhtml";
        protected readonly string _mitrwoPage = "https://apps.e-efka.gov.gr/eDebtor/secure/amo.xhtml";
        private readonly string _wrongCredentials = "https://www1.gsis.gr/oauth2server/login.jsp";
        private readonly string _messageNotice = "https://apps.e-efka.gov.gr/eDebtor/secure/notice.xhtml";
        private readonly string _messageInbox = "https://apps.e-efka.gov.gr/eDebtor/secure/inbox.xhtml";

        //keao
        private ILocator _bottomLocator;
        private ILocator _mitrwoTableLocator;
        private ILocator _paymentIdentityLocator1;
        private ILocator _paymentIdentityLocator2;
        private ILocator _paymentCovidLocator;
        private ILocator _adjustmentPaymentsLocator;
        private ILocator _outOfAdjustmentLocator;
        private ILocator _noDebtsTableLocator;
        private ILocator _nextButton1Locator;
        private ILocator _nextButton2Locator;
        private ILocator _userButtonLocator;

        private ILocator _debtsButtonLocator;
        private ILocator _inboxDataTableLocator;

        public BaseKeaoRetrievePage(string connectionId) : base(connectionId: connectionId, headless: false)
        {
            
            _bottomLocator = Page.Locator("[id=bottom]");
            _mitrwoTableLocator = Page.Locator("[id=dataTable_data]");
            _paymentIdentityLocator1 = Page.Locator("//*[@id=\'j_idt144\']"); //Page.Locator("[id=j_idt131]");
            _paymentIdentityLocator2 = Page.Locator("//html/body/div[2]/div[1]/div[2]/div[2]/div/span/div/div[2]/form/table/tbody/tr[1]/td/table[2]/tbody/tr/td[2]/a"); //Page.Locator("#j_idt55"); 
            _paymentCovidLocator = Page.Locator("//html/body/div[2]/div[1]/div[2]/div[2]/div/span/div/div[2]/form/table/tbody/tr[1]/td/table[2]/tbody/tr/td[5]"); 
            _adjustmentPaymentsLocator = Page.Locator("[id=linesTable1_data]");
            _outOfAdjustmentLocator = Page.Locator("[id=currentDebtsTable_data]");
            _noDebtsTableLocator = Page.Locator("//html/body/div[2]/div[2]/form/div[1]/div[2]/p");
            _nextButton1Locator = Page.Locator("//html/body/div[2]/div[1]/div[2]/div[2]/div/span/div/div[2]/form/div[1]/div[2]/div/div[1]/a[3]");
            _nextButton2Locator = Page.Locator("//html/body/div[2]/div[1]/div[2]/div[2]/div/span/div/div[2]/form/div[2]/div[2]/div/div[1]/a[3]");
            _userButtonLocator = Page.Locator("//*[@id=\"wrapper\"]/div[1]/div[2]/button[1]");

            _debtsButtonLocator = Page.Locator("//*[@id=\"MenuForm:mainMenu_eDebtorTransactions\"]/ul/li[2]/a");
            _inboxDataTableLocator = Page.Locator("//*[@id=\"inboxMessagesTable_data\"]/tr");

        }

        protected override async Task LogoutAsync()
        {
            if (Page.Url.Contains(_wrongCredentials))
                return;

            else if (_errorLoginPage.Equals(Page.Url))
                return;

            var exitLocator = Page.GetByText("Αποσύνδεση");

            if (await IsVisibleAsync(exitLocator))
            {
                //var _exitLocator1 = Page.GetByText("Αποσύνδεση");
                await ClickAsync(exitLocator);
            }

            else
            {
                await ClickAsync(_userButtonLocator, 100);
                //var _exitLocator2 = Page.GetByText("Αποσύνδεση");
                await ClickAsync(exitLocator);
            }
        }

        public async Task<IList<FinancialObligationDto>> Execute(int traderId, string institution)
        {
            await WaitForAsync(_debtsButtonLocator);
            await ClickAsync(_debtsButtonLocator, 100);
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            // Αν έχει μηνύματα πρέπει να διαβαστούν δεν είναι ολοκληρωμένη η υλοποίηση
            if (Page.Url.Contains(_messageNotice))
            {
                await ReadMessagesAsync();
            }

            await WaitForAsync(_outOfAdjustmentLocator, 30000);
            var debtsList = new List<FinancialObligationDto>();

            // Άντληση των οφειλών
            var list = await GetDebtsAsync(traderId, institution);
            debtsList.AddRange(list);

            return debtsList;
        }

        private async Task<IList<FinancialObligationDto>> GetDebtsAsync(int traderId, string institution)
        {
            var list = new List<FinancialObligationDto>();
            /** 'ΕΛΕΓΧΟΣ ΑΝ Ο ΠΙΝΑΚΑΣ ΕΙΝΑΙ ΚΕΝΟΣ **/

            var paymentIdentityLocator1Exist = await IsLocatorExist(_paymentIdentityLocator1);
            var paymentIdentity = paymentIdentityLocator1Exist ? await InnerTextAsync(_paymentIdentityLocator1) : await InnerTextAsync(_paymentIdentityLocator2);

            var currentDate = DateTime.UtcNow;
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var checkedDate = lastDayOfMonth.CheckPaymentDate();

            //Adjustment payments
            var isEnabled1 = true;

            while (isEnabled1)
            {
                var adjustment = await GetLocatorAsync(_adjustmentPaymentsLocator, "> tr");
                var rows = await GetAllAsync(adjustment);

                foreach (var row1 in rows)
                {
                    var row1Locator = await GetLocatorAsync(row1, "> td");
                    var cols1 = await GetAllAsync(row1Locator);
                    if (!cols1.Count.Equals(1))
                    {
                        var covidExist = (await InnerTextAsync(cols1[2])).Contains("COVID");
                        list.Add(new FinancialObligationDto
                        {
                            Institution = string.Format(_keao, institution),
                            PaymentType = await InnerTextAsync(cols1[2]),
                            PaymentValue = (await InnerTextAsync(cols1[3])).ToDecimal(),
                            PaymentIdentity = covidExist ? (await InnerTextAsync(_paymentCovidLocator)).Split("\n")[0] : paymentIdentity,
                            PaymentDate = checkedDate,
                            TraderId = traderId,
                            Period = currentDate.Month
                        });
                    }
                }

                var nameClass = await GetAttributeAsync(_nextButton1Locator, "class");

                isEnabled1 = !nameClass.Contains("ui-state-disabled");

                if (isEnabled1)
                    await ClickAsync(_nextButton1Locator);
            }

            var noDebtsTableLocatorExist = await IsLocatorExist(_noDebtsTableLocator);
            if (!noDebtsTableLocatorExist)
            {
                //Payments out of adjustment
                var isEnabled2 = true;
                var paymentValue = 0m;

                while (isEnabled2)
                {
                    var adjustment = await GetLocatorAsync(_outOfAdjustmentLocator, "> tr");
                    var rows2 = await GetAllAsync(adjustment);

                    foreach (var row2 in rows2)
                    {
                        var row2Locator = await GetLocatorAsync(row2, "> td");
                        var cols = await GetAllAsync(row2Locator);
                        if (!cols.Count.Equals(1))
                            paymentValue += (await InnerTextAsync(cols[4])).ToDecimal();
                    }

                    var nameClass = await GetAttributeAsync(_nextButton2Locator, "class");

                    isEnabled2 = !nameClass.Contains("ui-state-disabled");

                    if (isEnabled2)
                        await ClickAsync(_nextButton2Locator);
                }

                if (!paymentValue.Equals(0))
                {
                    list.Add(new FinancialObligationDto()
                    {
                        Institution = string.Format(_keao, institution),
                        PaymentType = "Όφειλές εκτός ρύθμισης",
                        PaymentDate = checkedDate,
                        PaymentIdentity = paymentIdentity,
                        PaymentValue = paymentValue,
                        TraderId = traderId,
                        Period = currentDate.Month
                    });
                }
            }

            //await Goto(Page, _mitrwoPage);

            return list;
        }
        private async Task ReadMessagesAsync()
        {
            await Goto(Page, _messageInbox);
            await WaitForAsync(_inboxDataTableLocator);

            var messagesRows = await GetAllAsync(_inboxDataTableLocator);

            foreach (var row in messagesRows)
            {
                var rowLocator = await GetLocatorAsync(row, "> td");
                var cols = await GetAllAsync(rowLocator);

                if (string.IsNullOrEmpty(await InnerTextAsync(cols[2])))
                {
                    var linkLocator = await GetLocatorAsync(cols[3], "> button");
                    await ClickAsync(linkLocator);
                    // Χρησιμοποιοείται για να κλείσουμε το Tab που άνοιξε
                    await Page.Keyboard.PressAsync("Control+w");

                    // Πρέπει να δούμε τι γίνεται μετά το κλικ
                }
            }

            await Goto(Page, _debtorPaysPage);
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            await WaitForAsync(_outOfAdjustmentLocator, 30000);
        }

    }
}
