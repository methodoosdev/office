using App.Core.Infrastructure;
using App.Core.Infrastructure.Dtos.Financial;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Automation.Pages.Financial
{
    public class DoyRetrievePage : PageTest
    {
        private readonly string _aadeDebtPage = "https://www1.gsis.gr/taxisnet/info/protected/displayDebtInfoAndPay.htm";
        private readonly string _arrangmentInfoPayPage = "https://www1.aade.gr/taxisnet/info/protected/displayArrangementInfoAndPay.htm";

        private ILocator _userNameLocator;
        private ILocator _passwordLocator;
        private ILocator _signinButtonLocator;
        private ILocator _debtsTableLocator;
        private ILocator _arrangmentInfoTableLocator;
        private ILocator _footerLocator;

        public DoyRetrievePage(string connectionId) : base(connectionId: connectionId, headless: false)
        {
            _userNameLocator = Page.Locator("[name=username]");
            _passwordLocator = Page.Locator("[name=password]");
            _signinButtonLocator = Page.Locator("[name=btn_login]");
            _debtsTableLocator = Page.Locator("//html/body/table[1]/tbody/tr[4]/td/table/tbody/tr/td/table/tbody/tr[3]/td/table/tbody/tr/td/form/table/tbody/tr[4]/td/table/tbody/tr/td/table/tbody/tr");
            _arrangmentInfoTableLocator = Page.Locator("//html/body/table[1]/tbody/tr[4]/td/table/tbody/tr/td/table/tbody/tr[3]/td/table/tbody/tr/td/table/tbody/tr[4]/td/form/table/tbody/tr/td/table/tbody/tr");
            _footerLocator = Page.Locator("[class=main-footer]");

        }

        protected override async Task LogoutAsync()
        {
            if (!LoginIn)
                return;

            var _exitLocator = Page.Locator("//html/body/table/tbody/tr[2]/td/div/nav/div[4]/div/div[1]/div/div/span");
            var _logoutLocator = Page.Locator("//html/body/table[1]/tbody/tr[2]/td/div/nav/div[4]/div/div[2]/a[4]");

            await ClickAsync(_exitLocator);
            await ClickAsync(_logoutLocator);
        }

        public async Task<bool> Login(string userName, string password)
        {
            await GotoToUrl(Page, _aadeDebtPage);

            // Login
            await FillAsync(_userNameLocator, userName);
            await FillAsync(_passwordLocator, password);
            await ClickAsync(_signinButtonLocator);

            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            var isLogin = await IsLocatorExist(_footerLocator);

            return isLogin;
        }

        public async Task<IList<FinancialObligationDto>> Execute(int traderId)
        {
            //await WaitForAsync(_footerLocator, 300000);

            var doyDebts = new List<FinancialObligationDto>();
            var rows = await GetAllAsync(_debtsTableLocator);
            rows.RemoveAt(0);

            var nonArrangmentDebts = await GetDebtsAsync(rows, _footerLocator, traderId, string.Empty);
            doyDebts.AddRange(nonArrangmentDebts);

            await Goto(Page, _arrangmentInfoPayPage);
            await WaitForAsync(_footerLocator, 300000);

            var arrangmentRows = await GetAllAsync(_arrangmentInfoTableLocator);
            arrangmentRows.RemoveAt(0);

            var arrangmentDebts = await GetDebtsAsync(arrangmentRows, _footerLocator, traderId, "ΡΥΘΜΙΣΗ");
            doyDebts.AddRange(arrangmentDebts);

            return doyDebts;
        }

        public async Task<List<FinancialObligationDto>> GetDebtsAsync(List<ILocator> rows, ILocator footerLocator, int traderId, string arrangeType)
        {
            var doyDebts = new List<FinancialObligationDto>();
            var paymentIdentity = string.Empty;
            var index = 0;

            foreach (var row in rows)
            {
                var debtColsLocator = await GetLocatorAsync(row, "> td");
                var debtCols = await GetAllAsync(debtColsLocator);

                if (debtCols.Count < 12)
                    break;

                // Παίρνω φορέα και είδος φόρου από τον πρώτο πίνακα
                var institution = await InnerTextAsync(debtCols[0]);
                var taxType = await InnerTextAsync(debtCols[1]);

                // Get the locator of table with installments for each row  
                var tableLocator = Page.Locator(string.Format("//*[@id=\"installmentInfo_{0}\"]", index));

                //Check if locator exists for the given index or if no more tables are available

                if (!(await IsLocatorExist(tableLocator)))
                    break;

                index++;

                var nonArrangment = debtCols.Count == 14;

                // Check the right column if the debt is non arranged or arranged
                var checkColNumber = nonArrangment ? 10 : 9;

                await CheckAsync(debtCols[checkColNumber].Locator("> input"));

                var installmentDebts = await GetInstallmentsInfoAsync(tableLocator, traderId, institution, taxType, arrangeType);

                // Click the right button if the debt is non arranged or arranged
                var clickColNumber = nonArrangment ? 11 : 10;

                await ClickAsync(debtCols[clickColNumber].Locator("> button"));

                //await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                //var _noPrintLocator = Page.Locator("[class=noPrint]");
                var _noPrintLocator = Page.Locator("fieldset:nth-child(2) table tbody");
                await WaitForAsync(_noPrintLocator, 300000);

                var debtRows = await GetAllAsync(Page.Locator("fieldset:nth-child(2) table tbody tr"));

                var dict = new Dictionary<string, string>();

                foreach (var item in debtRows)
                {
                    int count = await item.Locator("> td").CountAsync();
                    if (count > 1)
                    {
                        var cols = await GetAllAsync(item.Locator("> td"));
                        dict.Add(await InnerTextAsync(cols[0]), await InnerTextAsync(cols[1]));
                    }
                }

                foreach (var item in dict)
                    if (item.Key.Contains("Ταυτότητα"))
                    {
                        paymentIdentity = item.Value;
                        break;
                    }

                foreach (var debt in installmentDebts)
                {
                    debt.PaymentIdentity = paymentIdentity;
                }

                doyDebts.AddRange(installmentDebts);

                //await GotoToUrl(Page, Page.Url.Contains("Arrangement") ? _arrangmentInfoPayPage : _aadeDebtPage);
                await Goto(Page, Page.Url.Contains("Arrangement") ? _arrangmentInfoPayPage : _aadeDebtPage);
                await WaitForAsync(footerLocator, 300000);
                //END

            }

            doyDebts = doyDebts.Where(x => !x.PaymentValue.Equals(0)).ToList();

            return doyDebts;

        }
        public async Task<List<FinancialObligationDto>> GetInstallmentsInfoAsync(
                        ILocator tableLocator, int traderId, string institution, string taxType, string arrangeType)
        {

            var installmentDebts = new List<FinancialObligationDto>();

            var startDate = DateTime.UtcNow;
            var year = startDate.Year;
            var month = startDate.Month;
            var firstDayOfMonth = new DateTime(year, month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var checkedDate = lastDayOfMonth.CheckPaymentDate(); // Τελευταία εργάσιμη του τρέχων μήνα 
            var checkedDate2 = lastDayOfMonth.CheckPaymentDate2(); // Πρώτη εργάσιμη του επόμενου μήνα 

            var lastDayNextMonth = firstDayOfMonth.AddMonths(2).AddDays(-1); // Τελευταία μέρα του επόμενου μήνα 
            var nextCheckedDate = lastDayNextMonth.CheckPaymentDate(); // Τελευταία εργάσιμη του επόμενου μήνα 

            //Get installment rows and info
            var installmentInfo = await GetLocatorAsync(tableLocator, "tbody");
            var tbody = await GetAllAsync(installmentInfo);
            var tbodyCount = tbody.Count();
            var installmentRows = tbodyCount > 1 ? await GetLocatorAsync(tbody[1], "tr") : await GetLocatorAsync(tableLocator, "tbody tr");
            //var installmentsRows = await GetLocatorAsync(tbody[1], "tr");
            var installments = await GetAllAsync(installmentRows);
            installments.RemoveAt(0);

            foreach (var installment in installments)
            {
                var columnLocator = await GetLocatorAsync(installment, "> td");
                var columns = await GetAllAsync(columnLocator);

                var value = (await InnerTextAsync(columns[2])).ToEuroDecimal();

                if (!(value > 0))
                    continue;

                // Initialize values
                DateTime? PaymentDate = null;
                decimal installmentValue = 0m;
                //decimal expiredValue = 0m;
                //decimal totalValue = 0m;

                var hasArrangedLabel = taxType.Contains("ΡΥΘΜΙΣΗ");

                var infoDate = (await InnerTextAsync(columns[1])).ToDateGR();
                var expiredValue = (await InnerTextAsync(columns[3])).ToEuroDecimal(); //+++
                var totalValue = (await InnerTextAsync(columns[4])).ToEuroDecimal(); //+++

                if (infoDate < nextCheckedDate)
                {
                    if (infoDate.Month.Equals(month) && infoDate.Year.Equals(year) ||
                        (infoDate.Month.Equals(nextCheckedDate.Month) && infoDate.Year.Equals(nextCheckedDate.Year)))
                    {
                        PaymentDate = infoDate.Equals(checkedDate) ? checkedDate :
                                                infoDate.Equals(checkedDate2) ? checkedDate2 : infoDate;

                        // Changed value to totalValue
                        installmentValue = totalValue;
                    }
                    else
                    {
                        PaymentDate = infoDate;
                        expiredValue = (await InnerTextAsync(columns[4])).ToEuroDecimal();
                    }
                }

                // Έλεγχος αν είναι ληξιπρόθεσμη οφειλή ή τρέχουσα δόση
                if (expiredValue > 0 && infoDate < checkedDate)
                {
                    installmentDebts.Add(new FinancialObligationDto
                    {
                        Institution = "ΑΑΔΕ ΔΟΥ " + institution,
                        PaymentType = "Ληξ/σμο,προσαυξήσεις,τέλη " + taxType,
                        PaymentValue = totalValue,
                        PaymentDate = PaymentDate.HasValue ? PaymentDate.Value : infoDate,
                        TraderId = traderId,
                        Period = startDate.Month
                    });
                }
                else if (infoDate < nextCheckedDate)
                {
                    installmentDebts.Add(new FinancialObligationDto
                    {
                        Institution = "ΑΑΔΕ ΔΟΥ " + institution,
                        PaymentType = arrangeType == "ΡΥΘΜΙΣΗ" ? hasArrangedLabel ? taxType : "ΡΥΘΜΙΣΗ " + taxType : taxType,
                        PaymentValue = installmentValue,
                        PaymentDate = PaymentDate.HasValue ? PaymentDate.Value : infoDate,
                        TraderId = traderId,
                        Period = startDate.Month
                    });
                }

                if (infoDate >= nextCheckedDate)
                {
                    if (taxType.Contains("ΠΡΟΣΤΙΜΟ"))
                    {
                        totalValue = (await InnerTextAsync(columns[4])).ToEuroDecimal();
                        installmentDebts.Add(new FinancialObligationDto
                        {
                            Institution = "ΑΑΔΕ ΔΟΥ " + institution,
                            PaymentType = string.Format("ΠΡΟΣΤΙΜΟ ({0} ευρώ)", totalValue),
                            PaymentDate = infoDate,
                            TraderId = traderId,
                            Period = startDate.Month
                        });
                    }

                    break;
                }

            }

            return installmentDebts;
        }

    }

}
