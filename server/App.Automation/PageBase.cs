using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace App.Automation
{
    public abstract class PageBase
    {
        protected abstract Task<string> GetResourceAsync(string resource);
        protected abstract Task SendScreenshotAsync(ILocator locator, string method);
        protected abstract Task EmulateBrowserAsync();
        protected abstract Task EmulateBrowserDialogOpen();
        protected abstract Task EmulateBrowserDialogClose();
        public abstract Task SendProgressLabelAsync(string label);
        protected virtual Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

        protected async Task FillAsync(IPage page, string xPath, string value)
        {
            var locator = page.Locator(xPath);

            await FillAsync(locator, value);
        }

        protected async Task FillAsync(ILocator locator, string value)
        {
            locator = await GetLocatorAsync(locator);

            await locator.FillAsync(value ?? string.Empty);
        }

        protected string GetXPathFromLocator(ILocator locator)
        {
            var value = locator.ToString();
            if (value.StartsWith("Locator@"))
                return value.Split('@')[1];
            return string.Empty;
        }

        protected async Task<List<ILocator>> GetAllAsync(ILocator locator)
        {
            locator = await GetLocatorAsync(locator);

            var list = (await locator.AllAsync()).ToList();

            return list;
        }
        protected async Task<string> InputValueAsync(ILocator locator)
        {
            locator = await GetLocatorAsync(locator);

            var value = await locator.InputValueAsync();

            return value?.Trim() ?? string.Empty;
        }

        protected async Task<string> InnerTextAsync(ILocator locator)
        {
            locator = await GetLocatorAsync(locator);

            var text = await locator.InnerTextAsync();

            return text?.Trim() ?? string.Empty;
        }

        protected async Task<ILocator> GetByTextAsync(ILocator locator, string text)
        {
            locator = await GetLocatorAsync(locator);

            return locator.GetByText(text?.Trim() ?? string.Empty);
        }

        protected async Task<ILocator> GetByTextAsync(IPage page, string text)
        {
            var locator = page.GetByText(text);

            return await GetLocatorAsync(locator);
        }

        protected async Task<string> GetAttributeAsync(ILocator locator, string attr)
        {
            locator = await GetLocatorAsync(locator);

            var name = await locator.GetAttributeAsync(attr);

            return name;
        }

        protected async Task ClickAsync(ILocator locator, float delay = 0 )
        {
            locator = await GetLocatorAsync(locator);

            await locator.ClickAsync(new() { Delay = delay });
        }

        protected async Task<bool> IsVisibleAsync(ILocator locator)
        {
            locator = await GetLocatorAsync(locator);

            var isVisible = await locator.IsVisibleAsync();

            return isVisible;
        }

        protected async Task<bool> IsEnabledAsync(ILocator locator, float timeout = 5000)
        {
            locator = await GetLocatorAsync(locator);

            var isEnabled = await locator.IsEnabledAsync(new() { Timeout = timeout });

            return isEnabled;
        }

        protected async Task<bool> IsCheckedAsync(ILocator locator)
        {
            locator = await GetLocatorAsync(locator);

            var isChecked = await locator.IsCheckedAsync();

            return isChecked;
        }

        protected async Task CheckAsync(ILocator locator)
        {
            locator = await GetLocatorAsync(locator);

            await locator.CheckAsync();
        }

        protected async Task HoverAsync(ILocator locator)
        {
            locator = await GetLocatorAsync(locator);

            await locator.HoverAsync();
        }

        protected async Task NewExceptionAsync(string resource, params object[] args)
        {
            var errorMessage = await GetResourceAsync(resource);
            throw new AppPlaywrightException(errorMessage, args);
        }

        //
        protected async Task<bool> IsLocatorExist(ILocator locator)
        {
            int count = await locator.CountAsync();
            return count > 0 ? true : false;
        }

        protected async Task<ILocator> GetLocatorAsync(IPage page, string xPath)
        {
            var locator = page.Locator(xPath);

            return await GetLocatorAsync(locator);
        }

        protected async Task<ILocator> GetLocatorAsync(ILocator locator)
        {
            int count = await locator.CountAsync();
            if (count > 0)
                return locator;

            var errorMessage = await GetResourceAsync("App.Playwright.ElementNotExist");
            throw new AppPlaywrightException(errorMessage, $"\r\nUrl: {locator.Page.Url},\r\n {locator.ToString()}");
        }

        /// <summary>
        /// Προυπόθεση ο parent να εχει ελεγχθεί πρώτα
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>
        protected async Task<ILocator> GetLocatorAsync(ILocator parent, string xPath)
        {
            var locator = parent.Locator(xPath);

            return await GetLocatorAsync(locator);
        }

        protected async Task SelectOptionAsync(ILocator locator, string value, SelectOptionValueType selectOptionValueType = SelectOptionValueType.Value)
        {
            locator = await GetLocatorAsync(locator);

            try
            {
                switch (selectOptionValueType)
                {
                    case SelectOptionValueType.Value:
                        var selection1 = await locator.SelectOptionAsync(new[] { new SelectOptionValue() { Value = value } });
                        break;
                    case SelectOptionValueType.Label:
                        await locator.SelectOptionAsync(new[] { new SelectOptionValue() { Label = value } });
                        break;
                    case SelectOptionValueType.Index:
                        await locator.SelectOptionAsync(new[] { new SelectOptionValue() { Index = int.Parse(value) } });
                        break;
                }
            }
            catch
            {
                var errorMessage = await GetResourceAsync("App.Playwright.FailedToSelectOption");
                throw new AppPlaywrightException(errorMessage, $"\r\nUrl: {locator.Page.Url},\r\n {locator.ToString()}");
            }
        }

        protected async Task<string> DownloadPdf(ILocator locator)
        {
            var download = await locator.Page.RunAndWaitForDownloadAsync(async () =>
            {
                await locator.ClickAsync();
            });

            var path = await download.PathAsync();
            var fname = download.SuggestedFilename;
            var url = download.Url;

            string pdfText = null;
            if (System.IO.File.Exists(path))
            {
                pdfText = ExtractTextFromPdf(path);
            }
            return pdfText;
        }
        protected string ExtractTextFromPdf(string filePath)
        {
            StringBuilder text = new StringBuilder();
            //ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

            using (PdfReader reader = new PdfReader(filePath))
            {
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    string currentText = PdfTextExtractor.GetTextFromPage(reader, page/*, strategy*/);
                    text.AppendLine(currentText);
                }
            }
            return text.ToString();
        }

        protected async Task ClickToUrl(IPage page, string selector, string url)
        {
            var locator = page.Locator(selector);
            await ClickToUrl(locator, url);
        }

        protected async Task ClickToUrl(ILocator locator, string url, bool exact = false)
        {
            locator = await GetLocatorAsync(locator);

            try
            {
                await locator.Page.RunAndWaitForResponseAsync(async () =>
                {
                    await locator.ClickAsync();
                }, x =>
                {
                    return exact ? x.Url.Equals(url) : x.Url.Contains(url);
                });

                //await locator.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await EmulateBrowserAsync();
            }
            catch
            {
                var errorMessage = await GetResourceAsync("App.Playwright.PageNotResponse");
                throw new AppPlaywrightException(errorMessage, $"\r\nUrl: {url}.");
            }
        }

        protected async Task GotoToUrl(IPage page, string url, string destination = null)
        {
            try
            {
                await page.RunAndWaitForResponseAsync(async () =>
                {
                    await page.GotoAsync(url);
                }, x =>
                {
                    return x.Url.Contains(destination ?? url);
                });

                //await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await EmulateBrowserAsync();
            }
            catch
            {
                var errorMessage = await GetResourceAsync("App.Playwright.PageNotResponse");
                throw new AppPlaywrightException(errorMessage, $"\r\nUrl: {url}.");
            }
        }

        protected async Task Goto(IPage page, string url)
        {
            //await page.GotoAsync(url, new PageGotoOptions
            //{
            //    WaitUntil = WaitUntilState.NetworkIdle
            //});

            try
            {
                await page.GotoAsync(url);
                //await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                await EmulateBrowserAsync();
            }
            catch
            {
                var errorMessage = await GetResourceAsync("App.Playwright.PageNotResponse");
                throw new AppPlaywrightException(errorMessage, $"\r\nUrl: {url}.");
            }
        }

        protected void EnsurePageExist(IPage page, string url)
        {
            if (!page.Url.StartsWith(url, StringComparison.OrdinalIgnoreCase)) 
                throw new AppPlaywrightException($"Δεν υπάρχει η τρέχουσα σελίδα: { url }");

        }

        protected async Task EnsurePageLoaded(IPage page, string url)
        {
            await page.WaitForURLAsync(new Regex(url), new()
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });
        }

        protected async Task WaitForAsync(IPage page, string selector, float timeout = 5000)
        {
            var locator = page.Locator(selector);

            await WaitForAsync(locator, timeout);
        }

        protected async Task WaitForAsync(ILocator locator, float timeout = 5000)
        {
            try
            {
                await locator.WaitForAsync(new() { Timeout = timeout });
            }
            catch
            {
                var xPath = GetXPathFromLocator(locator);
                throw new PlaywrightException($"Timeout locator: {xPath}");
            }
        }

        protected async Task ClickToUrl(ILocator locator, string url1, string url2, float timeout = 5000)
        {
            locator = await GetLocatorAsync(locator);

            try
            {
                await locator.Page.RunAndWaitForResponseAsync(async () =>
                {
                    await locator.ClickAsync();
                }, x =>
                {
                    return x.Url.Contains(url1) || x.Url.Contains(url2) ? true : false;
                }, new() { Timeout = timeout });

                //await locator.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await EmulateBrowserAsync();
            }
            catch
            {
                var errorMessage = await GetResourceAsync("App.Playwright.PageNotResponse");
                throw new AppPlaywrightException(errorMessage, $"\r\nUrl: {url1} or {url2}.");
            }
        }

        // Επιστρέφει rows -> columns (ILocator) ταξινομημένα από την 3η στήλη (date)
        protected async Task<List<List<ILocator>>> GetRowsSortedByDateColumnAsync(
            IPage page,
            string tbodySelector,
            int dateColIndex,
            bool descending = true)
        {
            var tbody = page.Locator(tbodySelector);
            var tr = tbody.Locator("tr");
            var count = await tr.CountAsync();

            var rowsWithDate = new List<(DateTime date, List<ILocator> cols)>(count);

            for (int i = 0; i < count; i++)
            {
                var row = tr.Nth(i);

                // columns as ILocator list
                var cols = (await row.Locator("td").AllAsync()).ToList();
                if (cols.Count <= dateColIndex) continue;

                // date text from 3rd column: "11/2025 - 11/2025"
                var dateText = (await cols[dateColIndex].InnerTextAsync()).Trim();

                if (!TryParseMonthYearRangeStart(dateText, out var dt))
                    continue;

                rowsWithDate.Add((dt, cols));
            }

            var sorted = descending
                ? rowsWithDate.OrderByDescending(x => x.date)
                : rowsWithDate.OrderBy(x => x.date);

            // Τελική λίστα rows (κάθε row είναι List<ILocator> columns)
            return sorted.Select(x => x.cols).ToList();
        }

        private bool TryParseMonthYearRangeStart(string s, out DateTime dt)
        {
            dt = default;

            if (string.IsNullOrWhiteSpace(s)) return false;

            // Παίρνουμε το πρώτο "M/yyyy" πριν το '-'
            var firstPart = s.Split('-', StringSplitOptions.RemoveEmptyEntries)[0].Trim();

            return DateTime.TryParseExact(
                firstPart,
                new[] { "M/yyyy", "MM/yyyy" },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dt
            );
        }
    }
}
