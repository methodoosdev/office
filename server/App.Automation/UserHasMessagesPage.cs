using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace App.Automation
{
    public class UserHasMessagesPage : PageTest
    {
        private ILocator _messageSelectLocator;
        private ILocator _pageNumberLocator;
        private ILocator _messageRowsLocator;
        private ILocator _backButtonLocator;
        private ILocator _nextMessageLocator;

        public UserHasMessagesPage(bool? headless = null, float? delay = null, string downLoadsPath = null, string browserName = null, bool emulateBrowserEnable = true, string connectionId = null) : base(headless, delay, downLoadsPath, browserName, emulateBrowserEnable, connectionId)
        {
            //Message page Locators
            _messageSelectLocator = Page.Locator("//*[@id=\"MessagesForm:MessagesResults:j_id8\"]");
            _pageNumberLocator = Page.Locator("//html/body/div[2]/div[1]/div[2]/div[2]/div/span/span[1]/div/form/div[3]/div/table[2]/tbody/tr/td/div/div[1]/span");
            _messageRowsLocator = Page.Locator("//html/body/div[2]/div[1]/div[2]/div[2]/div/span/span/div/form/div[3]/div/table[2]/tbody/tr/td/div/div[2]/table/tbody/tr");
            _backButtonLocator = Page.Locator("//*[@id=\"MessageForm:j_idt153\"]");
            _nextMessageLocator = Page.Locator("//html/body/div[2]/div[1]/div[2]/div[2]/div/span/span[1]/div/form/div[3]/div/table[2]/tbody/tr/td/div/div[1]/a[3]");
        }

        public async Task TraderHasMessagesAsync()
        {
            var pages = (await InnerTextAsync(_pageNumberLocator)).Split(" ")[2].Trim();
            var numPages = Convert.ToInt32(pages.Replace(",", ""));

            await SelectOptionAsync(_messageSelectLocator, "20");

            for (var count = 1; count <= numPages; count++)
            {
                var rows = await GetAllAsync(_messageRowsLocator);

                foreach (var row in rows)
                {
                    var rowLocator = await GetLocatorAsync(row, "> td");
                    var cols = await GetAllAsync(rowLocator);

                    if (string.IsNullOrEmpty(await InnerTextAsync(cols[2])))
                    {
                        var link = await GetLocatorAsync(cols[0], "> a");
                        await ClickAsync(link);
                        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                        await ClickAsync(_backButtonLocator);
                        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                    }
                    else
                        break;
                }

                if (count < numPages)
                    await ClickAsync(_nextMessageLocator);
            }
        }
    }
}
